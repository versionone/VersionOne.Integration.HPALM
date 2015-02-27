using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using VersionOne.ServiceHost.Core.Utility;
using VersionOne.ServiceHost.Core.Logging;
using TDAPIOLELib;

using IList = System.Collections.IList;

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

        private TDConnection server;
        private TestFactory testFactory;
        private BugFactory bugFactory;

        private readonly IDictionary<string, string> defectFilters = new Dictionary<string, string>();
        private string qcCreateStatusValue;
        private string qcCloseStatusValue;

        public QualityCenterClient(QCProject project, XmlNode config, ILogger log) {
            this.project = project;
            this.log = log;

            SetConfiguration(config);
        }

        public bool IsLoggedIn {
            get { return Server.LoggedIn; }
        }

        public bool IsConnected {
            get { return Server.Connected; }
        }

        public bool IsProjectConnected {
            get { return Server.ProjectConnected; }
        }

        public QCProject ProjectInfo {
            get { return project; }
        }


        private TDConnection Server {
            get {
                if (server == null) {
                    server = new TDConnection();
                }

                if (!server.Connected) {
                    server.InitConnectionEx(url);
                }
                
                return server;
            }
        }

        private TestFactory TestFoundry {
            // This name is used so we don't confuse our TestFactory w/ QC TestFactory
            get {
                if (testFactory == null) {
                    var treeManager = Server.TreeManager as TreeManager;
                    
                    if (treeManager == null) {
                        throw new Exception("Quality Center did not return a TreeManager");
                    }

                    var rootNode = treeManager["Subject"] as SubjectNode;
                    
                    if(rootNode == null) {
                        throw new Exception("Quality Center does not contain a root folder called \"Subject\"");
                    }

                    SubjectNode versionOneNode;
                    
                    try {
                        versionOneNode = (rootNode.FindChildNode(project.TestFolder) ?? rootNode.AddNode(project.TestFolder)) as SubjectNode;
                    } catch (COMException ex) {
                        log.Log(LogMessage.SeverityType.Debug, string.Format("Node Not Found"), ex);
                        versionOneNode = rootNode.AddNode(project.TestFolder) as SubjectNode;
                    }

                    if (versionOneNode == null) {
                        throw new Exception(
                            string.Format(
                                "Failed to create folder with the name {0}.  It must be created manually for project {1}",
                                project.TestFolder, project.Project));
                    }

                    testFactory = versionOneNode.TestFactory as TestFactory;
                }

                return testFactory;
            }
        }

        private BugFactory BugFoundry {
            // This name is used so we don't confuse our BugFactory w/ QC BugFactory
            get { return bugFactory ?? (bugFactory = (BugFactory) Server.BugFactory); }
        }

        public static bool HasLastRun(object testObj) {
            var test = (Test) testObj;
            return test.LastRun != null;
        }

        public static string TestID(object testObj) {
            var test = (Test) testObj;
            return test.ID.ToString();
        }

        public static DateTime TimeStamp(object testObj) {
            var test = (Test) testObj;
            return DateTime.Parse(test["TS_VTS"].ToString());
        }

        public static string LastRunStatus(object testObj) {
            var test = (Test) testObj;
            return ((Run) test.LastRun).Status;
        }

        public static string DefectID(object bugObj) {
            var bug = (Bug) bugObj;
            return bug.ID.ToString();
        }

        public static string DefectSummary(object bugObj) {
            var bug = (Bug) bugObj;
            return bug.Summary;
        }

        public static string DefectDescription(object bugObj) {
            var bug = (Bug) bugObj;
            return (string) bug["BG_DESCRIPTION"];
        }

        public static string DefectPriority(object bugObj) {
            var bug = (Bug) bugObj;
            return bug.Priority;
        }

        public void Dispose() {
            Server.Disconnect();
            Server.ReleaseConnection();
            server = null;
        }

        public void Login() {
            if (!IsLoggedIn) {
                Server.Login(userName, password);
            }
        }

        public void ConnectToProject() {
            Login();

            if (IsProjectConnected) {
                return;
            }

            try {
                Server.Connect(project.Domain, project.Project);
            } catch (COMException) {
                log.Log(LogMessage.SeverityType.Error,
                        string.Format("*** Exception connecting to Domain=\"{0}\" Project=\"{1}\"", project.Domain, project.Project));
                throw;
            }

            if (!IsConnected) {
                throw new ConfigurationException(
                    string.Format("*** Failed to connect.  Domain=\"{0}\" Project=\"{1}\"",
                    project.Domain, project.Project));
            }
        }

        public void Logout() {
            Server.Logout();
        }

        public string CreateQCTest(string title, string description, string externalId) {
            ConnectToProject();

            var test = TestFoundry.AddItem(DBNull.Value) as Test;

            test.Name = title;
            test["TS_DESCRIPTION"] = "<html><body>" + description + "</body></html>";
            test["TS_STATUS"] = project.TestStatus;
            test[project.V1IdField] = externalId;

            test.Post();
            return GetFullyQualifiedQCId(test.ID.ToString());
        }

        // TODO use generic collections
        public IList GetLatestTestRuns(DateTime lastCheck) {
            IList testRuns = new ArrayList();
            ConnectToProject();
            
            var filterString = GetLastCheckFilterString(lastCheck);
            var filter = (TDFilter) TestFoundry.Filter;
            filter["TS_VTS"] = filterString;
            filter[project.V1IdField] = "AT*";

            // This needs to be a global test factory so users can move test to other folders
            var factory = Server.TestFactory as TestFactory;
            
            if (factory == null) {
                throw new Exception("Quality Center failed to return a Test Factory");
            }

            var tdTestList = factory.NewList(filter.Text);
            
            foreach (var testRun in tdTestList) {
                testRuns.Add(testRun);
            }
            
            return testRuns;
        }

        // TODO use generic collections
        public IList GetLatestDefects(DateTime lastCheck) {
            IList bugList = new ArrayList();
            ConnectToProject();

            var filter = (TDFilter) BugFoundry.Filter;

            foreach (var entry in defectFilters) {
                filter[entry.Key] = entry.Value;
            }

            foreach (var bug in BugFoundry.NewList(filter.Text)) {
                bugList.Add(bug);
            }

            return bugList;
        }

        public void OnDefectCreated(string id, ICollection comments, string link) {
            ConnectToProject();
            var bug = UpdateDefectStatus(id, qcCreateStatusValue, comments);
            var attachmentFactory = (AttachmentFactory) bug.Attachments;
            var attachment = (Attachment) attachmentFactory.AddItem(link);
            attachment.Post();
        }

        public bool OnDefectStateChange(string id, ICollection comments) {
            ConnectToProject();
            return UpdateDefectStatus(id, qcCloseStatusValue, comments) != null;
        }

        public Bug GetQCDefect(string externalId) {
            var bugId = GetLocalQCId(externalId);
            return (Bug) BugFoundry[bugId];
        }

        #region Methods that Only Exist for Testing

        public int GetTestCount() {
            Login();
            ConnectToProject();
            return TestFoundry.NewList("").Count;
        }

        public Bug CreateQCDefect() {
            ConnectToProject();
            string summary = "A Test Defect " + Guid.NewGuid();
            Bug bug = (Bug) BugFoundry.AddItem(summary);
            bug.Summary = summary;
            bug.Status = "New";
            bug.AssignedTo = "VersionOne";
            bug["BG_DESCRIPTION"] = "DESCRIPTION";
            bug.Post();
            return bug;
        }

        #endregion

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

        private Bug UpdateDefectStatus(string externalId, string newStatus, IEnumerable comments) {
            var bug = GetQCDefect(externalId);
            bug.Status = newStatus;
            var existingComments = (string) bug["BG_DEV_COMMENTS"];
            bug["BG_DEV_COMMENTS"] = existingComments + BuildComments(comments, existingComments);
            bug.Post();
            
            return bug;
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