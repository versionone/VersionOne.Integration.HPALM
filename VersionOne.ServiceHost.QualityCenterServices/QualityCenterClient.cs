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
            var resource = string.Format("qcbin/rest/domains/{0}/projects/{1}/test-folders?query={{name[{2}]}}",
                project.Domain, project.Project, project.TestFolder);
            var folderDoc = _connector.Get(resource);

            var folderId = folderDoc.Descendants("Field").First(f => f.Attribute("Name").Value == "id").Value;
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

            resource = string.Format("/qcbin/rest/domains/{0}/projects/{1}/tests", project.Domain, project.Project);
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
            //ConnectToProject();
            var bugDoc = UpdateDefectStatus(id, qcCreateStatusValue, comments);
            //var attachmentFactory = (AttachmentFactory) bug.Attachments;
            //var attachment = (Attachment) attachmentFactory.AddItem(link);
            //attachment.Post();
            
            //TODO: create attachment
            //var bugId = bugDoc.Elements().First(e => e.Attribute("Name").Value == "id").Value;
            //var resource = string.Format("/qcbin/rest/domains/{0}/projects/{1}/defects/{2}/attachments", project.Domain,
            //    project.Project, bugId);
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

        //#region Methods that Only Exist for Testing

        //public int GetTestCount() {
        //    Login();
        //    //ConnectToProject();
        //    var resource = string.Format("/qcbin/rest/domains/{0}/projects/{1}/tests?page-size=1", project.Domain,
        //        project.Project);
        //    var testsDoc = _connector.Get(resource);
        //    var testCount = testsDoc.Element("TotalResults").Value;

        //    return int.Parse(testCount);
        //}

        //public XDocument CreateQCDefect()
        //{
        //    Login();
        //    //ConnectToProject();
        //    string summary = "A Test Defect " + Guid.NewGuid();
        //    //Bug bug = (Bug) BugFoundry.AddItem(summary);
        //    //bug.Summary = summary;
        //    //bug.Status = "New";
        //    //bug.AssignedTo = "VersionOne";
        //    //bug["BG_DESCRIPTION"] = "DESCRIPTION";
        //    //bug.Post();
        //    var payload = XDocument.Parse("<Entity Type=\"defect\"></Entity>");
        //    var fields = new XElement("Fields");
        //    fields.Add(new XElement("Field", new XAttribute("Name", "name"), new XElement("Value", summary)));
        //    fields.Add(new XElement("Field", new XAttribute("Name", "status"), new XElement("Value", "New")));
        //    fields.Add(new XElement("Field", new XAttribute("Name", "owner"), new XElement("Value", "VersionOne")));
        //    fields.Add(new XElement("Field", new XAttribute("Name", "detected-by"), new XElement("Value", "VersionOne")));
        //    fields.Add(new XElement("Field", new XAttribute("Name", "description"), new XElement("Value", "DESCRIPTION")));

        //    var resource = string.Format("/qcbin/rest/domains/{0}/projects/{1}/defects", project.Domain, project.Project);
        //    var createdDefectDoc = _connector.Post(resource, payload);
            
        //    return createdDefectDoc;
        //}

        //#endregion

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

        private static string GetLastCheckFilterString(DateTime lastCheck) {
            return ">=\"" + lastCheck.ToString("yyyy-MM-dd HH:mm:00") + "\"";
        }

        private static string BuildComments(IEnumerable messages, string existingComments) {
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

        #endregion
    }
}