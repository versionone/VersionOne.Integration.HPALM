using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using VersionOne.ServiceHost.Core.Logging;

namespace VersionOne.ServiceHost.QualityCenterServices {
    // TODO refactor and extract bunch of classes
    /// <summary>
    /// This class encapsulates the QualityCenter COM library. No other classes should directly use the COM library
    /// It does *not* know about the EventManager, or any event objects specific to ServiceHost
    /// The only knowledge it has of ServiceHost is use of a couple utility classes in ServiceHost.Core.Utility
    /// </summary>
    public class QualityCenterClient : IQualityCenterClient {
        private readonly ILogger log;
        private string url;
        private string userName;
        private string password;
        private string _testFolderId;
        private readonly QCProject project;
        private HPALMConnector.HPALMConnector _connector;
        private readonly IDictionary<string, string> defectFilters = new Dictionary<string, string>();
        private string qcCreateStatusValue;
        private string qcCloseStatusValue;

        public QualityCenterClient(QCProject project, XmlNode config, ILogger log) {
            this.project = project;
            this.log = log;

            SetConfiguration(config);
            _connector = new HPALMConnector.HPALMConnector(url);
        }

        public bool IsLoggedIn {
            get { return _connector.IsAuthenticated; }
        }

        public QCProject ProjectInfo {
            get { return project; }
        }
        
        public void Dispose() {
            _connector.Dispose();
            _connector = null;
        }

        public void Login() {
            if (!IsLoggedIn) {
                _connector.Authenticate(userName, password);
            }
        }

        public void Logout() {
            _connector.Logout();
        }

        public string CreateQCTest(string title, string description, string externalId) {
            Login();

            var folderId = GetTestFolderId();
            var testDesc = string.Format("<html><body>{0}</body></html>", description);
            
            var payload = XDocument.Parse("<Entity Type=\"test\"></Entity>");
            var fields = new XElement("Fields");
            fields.Add(new XElement("Field", new XAttribute("Name", "name"), new XElement("Value", title)));
            fields.Add(new XElement("Field", new XAttribute("Name", "description"), new XElement("Value", testDesc)));
            fields.Add(new XElement("Field", new XAttribute("Name", "status"), new XElement("Value", project.TestStatus)));
            fields.Add(new XElement("Field", new XAttribute("Name", "parent-id"), new XElement("Value", folderId)));
            fields.Add(new XElement("Field", new XAttribute("Name", "owner"), new XElement("Value", userName)));
            fields.Add(new XElement("Field", new XAttribute("Name", "subtype-id"), new XElement("Value", "MANUAL")));
            fields.Add(new XElement("Field", new XAttribute("Name", project.V1IdField), new XElement("Value", externalId)));
            payload.Elements().First().Add(fields);

            var resource = string.Format("/qcbin/rest/domains/{0}/projects/{1}/tests", project.Domain, project.Project);
            var createdTestDoc = _connector.Post(resource, payload);

            return
                GetFullyQualifiedQCId(
                    createdTestDoc.Descendants("Field").First(f => f.Attribute("Name").Value.Equals("id")).Value);
        }

        public IList<XDocument> GetLatestTestRuns(DateTime lastCheck) {
            Login();
            var filterParam = "{last-modified[" + GetLastCheckFilterString(lastCheck) + "]; user-01[AT*];exec-status[<>\"No Run\"]}";
            var resource = string.Format("/qcbin/rest/domains/{0}/projects/{1}/tests?query={2}", project.Domain,
                project.Project, filterParam);

            var latestTestsDoc = _connector.Get(resource);

            return latestTestsDoc.Descendants("Entity").Select(e => new XDocument(e)).ToList();
        }

        public IList<XDocument> GetLatestDefects(DateTime lastCheck) {
            Login();
            var filterParam = "{";
            foreach (var entry in defectFilters) {
                filterParam += string.Format("{0}[{1}]", entry.Key, entry.Value) + ";";
            }
            filterParam += "}";
            var resource = string.Format("/qcbin/rest/domains/{0}/projects/{1}/defects?query={2}", project.Domain,
                project.Project, filterParam);

            var defectsDoc = _connector.Get(resource);

            return defectsDoc.Descendants("Entity").Select(e => new XDocument(e)).ToList();
        }

        public void OnDefectCreated(string id, ICollection comments, string link) {
            UpdateDefectStatus(id, qcCreateStatusValue, comments);
        }

        public bool OnDefectStateChange(string id, ICollection comments) {
            //ConnectToProject();
            return UpdateDefectStatus(id, qcCloseStatusValue, comments) != null;
        }

        public XDocument GetQCDefect(string externalId) {
            Login();
            var bugId = GetLocalQCId(externalId);

            var resource = string.Format("/qcbin/rest/domains/{0}/projects/{1}/defects/{2}", project.Domain,
                project.Project, bugId);
            var defectDoc = _connector.Get(resource);

            return defectDoc;
        }

        public string GetFullyQualifiedQCId(string localId) {
            return string.Format("{0}.{1}.{2}", project.Domain, project.Project, localId);
        }

        public string GetLocalQCId(string fullyQualifiedId) {
            return fullyQualifiedId.Substring(fullyQualifiedId.LastIndexOf(".") + 1);
        }

        #region Private Methods

        private void SetConfiguration(XmlNode config) {
            XmlNode connection = config["Connection"];

            url = connection["ApplicationUrl"].InnerText;
            userName = connection["Username"].InnerText;
            password = connection["Password"].InnerText;

            foreach (XmlNode node in config["DefectFilters"].SelectNodes("DefectFilter")) {
                defectFilters.Add(node["FieldName"].InnerText, node["FieldValue"].InnerText);
            }

            qcCreateStatusValue = config["CreateStatusValue"].InnerText;
            qcCloseStatusValue = config["CloseStatusValue"].InnerText;
        }

        private XDocument UpdateDefectStatus(string externalId, string newStatus, IEnumerable comments)
        {
            Login();
            var bugDoc = GetQCDefect(externalId);
            var id = bugDoc.Descendants("Field").First(f => f.Attribute("Name").Value == "id").Value;
            var existingComments = bugDoc.Descendants("Field").First(f => f.Attribute("Name").Value == "dev-comments").Value;

            var payload = XDocument.Parse("<Entity Type=\"defect\"></Entity>");
            var fields = new XElement("Fields");
            fields.Add(new XElement("Field", new XAttribute("Name", "status"), new XElement("Value", newStatus)));
            fields.Add(new XElement("Field", new XAttribute("Name", "dev-comments"),
                new XElement("Value", existingComments + BuildComments(comments, existingComments))));
            payload.Elements().First().Add(fields);

            var resource = string.Format("/qcbin/rest/domains/{0}/projects/{1}/defects/{2}", project.Domain, project.Project, id);
            var updatedBugDoc = _connector.Put(resource, payload);

            return updatedBugDoc;
        }

        private string GetLastCheckFilterString(DateTime lastCheck) {
            return ">=\"" + lastCheck.ToString("yyyy-MM-dd HH:mm:00") + "\"";
        }

        private string BuildComments(IEnumerable messages, string existingComments) {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.Append("<html><body>");
            
            if (!string.IsNullOrEmpty(existingComments)) {
                stringBuilder.Append("<br><font color=\"#000080\"><b>________________________________________</b></font><br>");
            }

            stringBuilder.AppendFormat("<font color=\"#000080\"><b>VersionOne, {0}: </b></font><br/>", DateTime.Now);
            
            foreach (string comment in messages) {
                stringBuilder.Append(comment);
                stringBuilder.AppendLine("<br/>");
            }
            
            stringBuilder.AppendLine("</body></html>");
            return stringBuilder.ToString();
        }

        private string GetTestFolderId()
        {
            
            return _testFolderId ?? (_testFolderId = GetTestFolderId(project.TestFolder) ?? CreateTestFolder());
        }

        private string CreateTestFolder()
        {
            try
            {
                Login();
                var rootFolderId = GetTestFolderId("Subject");
                if (rootFolderId == null)
                    throw new Exception("HP-ALM does not contain a root folder called \"Subject\"");

                var payload = XDocument.Parse("<Entity Type=\"test-folder\"></Entity>");
                var fields = new XElement("Fields");
                fields.Add(new XElement("Field", new XAttribute("Name", "name"),
                    new XElement("Value", project.TestFolder)));
                fields.Add(new XElement("Field", new XAttribute("Name", "parent-id"),
                    new XElement("Value", rootFolderId)));
                payload.Elements().First().Add(fields);

                var resource = string.Format("qcbin/rest/domains/{0}/projects/{1}/test-folders", project.Domain,
                    project.Project);
                var folderDoc = _connector.Post(resource, payload);
                
                return folderDoc.Descendants("Field").First(f => f.Attribute("Name").Value == "id").Value;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("An error has occurred while creating Test Folder: {0}", project.TestFolder), e);
            }
        }

        private string GetTestFolderId(string testFolderName)
        {
            string result = null;
            var resource = string.Format("qcbin/rest/domains/{0}/projects/{1}/test-folders?query={{name[{2}]}}",
                    project.Domain, project.Project, testFolderName);
            var folderDoc = _connector.Get(resource);

            var totalResults =
                Convert.ToInt16(
                    folderDoc.Descendants("Entities").First().Attribute("TotalResults").Value);
            if (totalResults != 0)
            {
                result = folderDoc.Descendants("Field").First(f => f.Attribute("Name").Value == "id").Value;
            }

            return result;
        }

        #endregion
    }
}