using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using VersionOne.ServiceHost.ConfigurationTool.Attributes;
using VersionOne.ServiceHost.ConfigurationTool.Validation;

namespace VersionOne.ServiceHost.ConfigurationTool.Entities {
    [XmlRoot("QualityCenterHostedService")]
    [DependsOnVersionOne]
    [DependsOnService(typeof(TestServiceEntity))]
    [DependsOnService(typeof(WorkitemWriterEntity))]
    [HasSelfValidation]
    public class QCServiceEntity : BaseServiceEntity {
        private QCConnection connection;

        #region Property list

        public const string CreateStatusValueProperty = "CreateStatus";
        public const string CloseStatusValueProperty = "CloseStatus";
        public const string SourceFieldValueProperty = "SourceField";
        public const string DefectFiltersProperty = "DefectFilters";
        public const string ProjectMappingsProperty = "Projects";
        public const string PriorityMappingsProperty = "PriorityMappings";

        #endregion

        public QCConnection Connection {
            get { return connection; }
            set { connection = value; }
        }

        [XmlArray("QCProjects")]
        [XmlArrayItem("Project")]
        [HelpString(HelpResourceKey = "QcProjectMappings")]
        public List<QCProject> Projects { get; set; }

        [XmlArray("DefectFilters")]
        [XmlArrayItem("DefectFilter")]
        [HelpString(HelpResourceKey = "QcDefectFilters")]
        public List<QCDefectFilter> DefectFilters { get; set; }

        [NonEmptyStringValidator]
        [XmlElement("CreateStatusValue")]
        [HelpString(HelpResourceKey = "QcCreateStatusValue")]
        public string CreateStatus { get; set; }

        [NonEmptyStringValidator]
        [XmlElement("CloseStatusValue")]
        [HelpString(HelpResourceKey = "QcCloseStatusValue")]
        public string CloseStatus { get; set; }

        [NonEmptyStringValidator]
        [XmlElement("SourceFieldValue")]
        [HelpString(HelpResourceKey = "QcSourceFieldValue")]
        public string SourceField { get; set; }

        [XmlArray("PriorityMappings")]
        [XmlArrayItem("Mapping")]
        [HelpString(HelpResourceKey = "QcPriorityMappings")]
        public List<QCPriorityMapping> PriorityMappings { get; set; }

        public QCServiceEntity() {
            connection = new QCConnection();
            DefectFilters = new List<QCDefectFilter>();
            Projects = new List<QCProject>();
            PriorityMappings = new List<QCPriorityMapping>();
            CreateTimer(TimerEntity.DefaultTimerIntervalMinutes);
        }

        [SelfValidation]
        public void CheckUniqueProjectId(ValidationResults results) {
            var projectIds = new List<string>(Projects.Count);

            foreach(QCProject project in Projects) {
                if(projectIds.Contains(project.Id)) {
                    results.AddResult(new ValidationResult("Project ID values should be unique", this, "Unique", null, null));
                    return;
                }

                projectIds.Add(project.Id);
            }
        }

        [SelfValidation]
        public void CheckProjects(ValidationResults results) {
            var validator = ValidationFactory.CreateValidatorFromAttributes(typeof(QCProject), string.Empty);

            foreach(QCProject project in Projects) {
                results.AddAllResults(validator.Validate(project));
            }
        }

        [SelfValidation]
        public void CheckDefectFilters(ValidationResults results) {
            var validator = ValidationFactory.CreateValidatorFromAttributes(typeof(QCDefectFilter), string.Empty);

            foreach(QCDefectFilter filter in DefectFilters) {
                results.AddAllResults(validator.Validate(filter));
            }
        }

        [SelfValidation]
        public void CheckConnection(ValidationResults results) {
            var validator = ValidationFactory.CreateValidatorFromAttributes(typeof(QCConnection), string.Empty);
            var defectFilterResults = validator.Validate(Connection);
            results.AddAllResults(defectFilterResults);
        }

        [SelfValidation]
        public void CheckPriorityMappings(ValidationResults results) {
            var validator = ValidationFactory.CreateValidatorFromAttributes(typeof(QCPriorityMapping), string.Empty);

            foreach(QCPriorityMapping mapping in PriorityMappings) {
                results.AddAllResults(validator.Validate(mapping));                
            }
        }

        // TODO compare collections?
        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }

            var other = (QCServiceEntity) obj;
            return connection.Equals(other.Connection) && string.Equals(other.CreateStatus, CreateStatus) && 
                   string.Equals(other.CloseStatus, CloseStatus) && string.Equals(other.SourceField, SourceField);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}