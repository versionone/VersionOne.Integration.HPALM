using System.Xml.Serialization;
using VersionOne.ServiceHost.ConfigurationTool.Validation;

namespace VersionOne.ServiceHost.ConfigurationTool.Entities {
    // TODO decide on whether we need some V1 Project wrapper class to bind comboboxes in grids
    public class QCProject {
        public const string IdProperty = "Id";
        public const string DomainProperty = "Domain";
        public const string ProjectProperty = "Project";
        public const string TestFolderProperty = "TestFolder";
    	public const string TestStatusProperty = "TestStatus";
        public const string VersionOneIdFieldProperty = "VersionOneIdField";
        public const string VersionOneProjectProperty = "VersionOneProject";

        [XmlAttribute("id")]
        [NonEmptyStringValidator]
        public string Id { get; set; }

        [NonEmptyStringValidator]
        public string Domain { get; set; }

        [NonEmptyStringValidator]
        public string Project { get; set; }

        [NonEmptyStringValidator]
        public string TestFolder { get; set; }

		[NonEmptyStringValidator]
		public string TestStatus { get; set; }

        [NonEmptyStringValidator]
        public string VersionOneIdField { get; set; }

        [NonEmptyStringValidator]
        public string VersionOneProject { get; set; }

        public override bool Equals(object obj) {
            if(obj == null || obj.GetType() != typeof(QCProject)) {
                return false;
            }

            var other = (QCProject) obj;

            return string.Equals(Id, other.Id) && string.Equals(Domain, other.Domain) &&
                string.Equals(Project, other.Project) &&
                    string.Equals(TestFolder, other.TestFolder) &&
						string.Equals(TestStatus, other.TestStatus) &&
							string.Equals(VersionOneIdField, other.VersionOneIdField) &&
								string.Equals(VersionOneProject, other.VersionOneProject);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}