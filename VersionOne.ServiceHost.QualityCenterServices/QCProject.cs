using System.Xml;

namespace VersionOne.ServiceHost.QualityCenterServices {
    public class QCProject {
        private readonly string domain;
        private readonly string project;
        private readonly string testFolder;
        private readonly string v1IdField;
        private readonly string v1Project;
        private readonly string tsStatus = "Imported";

        public string Domain {
            get { return domain; }
        }

        public string Project {
            get { return project; }
        }

        public string TestFolder {
            get { return testFolder; }
        }

        public string V1IdField {
            get { return v1IdField; }
        }

        public string V1Project {
            get { return v1Project; }
        }

        public string TestStatus {
            get { return tsStatus; }
        }

        public QCProject(XmlNode config) {
            domain = config["Domain"].InnerText;
            project = config["Project"].InnerText;
            testFolder = config["TestFolder"].InnerText;
            v1IdField = config["VersionOneIdField"].InnerText;
            v1Project = config["VersionOneProject"].InnerText;

            if (config["TestStatus"] != null) {
                tsStatus = config["TestStatus"].InnerText;
            }
        }
    }
}