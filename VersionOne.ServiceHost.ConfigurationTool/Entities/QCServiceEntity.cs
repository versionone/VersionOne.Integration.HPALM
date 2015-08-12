using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private ObservableCollection<QCProject> _projects;
        private ObservableCollection<QCDefectFilter> _defectFilters;
        private string _createStatus;
        private string _closeStatus;
        private string _sourceField;
        private ObservableCollection<QCPriorityMapping> _priorityMappings;

        public QCConnection Connection {
            get { return connection; }
            set
            {
                if (!value.Equals(connection))
                {
                    connection = value;
                    NotifyPropertyChanged();
                }
            }

        }

        [XmlArray("QCProjects")]
        [XmlArrayItem("Project")]
        [HelpString(HelpResourceKey = "QcProjectMappings")]
        public ObservableCollection<QCProject> Projects
        {
            get { return _projects; }
            set
            {
                if (_projects != value)
                {
                    _projects = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [XmlArray("DefectFilters")]
        [XmlArrayItem("DefectFilter")]
        [HelpString(HelpResourceKey = "QcDefectFilters")]
        public ObservableCollection<QCDefectFilter> DefectFilters {
            get { return _defectFilters; }
            set
            {
                if (_defectFilters != value)
                {
                    _defectFilters = value;
                    NotifyPropertyChanged();
                }
            } }

        [NonEmptyStringValidator]
        [XmlElement("CreateStatusValue")]
        [HelpString(HelpResourceKey = "QcCreateStatusValue")]
        public string CreateStatus {
            get { return _createStatus; }
            set
            {
                if (!value.Equals(_createStatus))
                {
                    _createStatus = value;
                    NotifyPropertyChanged();
                }
            } }

        [NonEmptyStringValidator]
        [XmlElement("CloseStatusValue")]
        [HelpString(HelpResourceKey = "QcCloseStatusValue")]
        public string CloseStatus {
            get { return _closeStatus; }
            set
            {
                if (!value.Equals(_closeStatus))
                {
                    _closeStatus = value;
                    NotifyPropertyChanged();
                }
            } }

        [NonEmptyStringValidator]
        [XmlElement("SourceFieldValue")]
        [HelpString(HelpResourceKey = "QcSourceFieldValue")]
        public string SourceField {
            get
            {
                return _sourceField;
            }
            set
            {
                if (!value.Equals(_sourceField))
                {
                    _sourceField = value;
                    NotifyPropertyChanged();
                }
            } }

        [XmlArray("PriorityMappings")]
        [XmlArrayItem("Mapping")]
        [HelpString(HelpResourceKey = "QcPriorityMappings")]
        public ObservableCollection<QCPriorityMapping> PriorityMappings {
            get { return _priorityMappings; }
            set
            {
                if (_priorityMappings != value)
                {
                    _priorityMappings = value;
                    NotifyPropertyChanged();
                }
            } }

        public QCServiceEntity() {
            connection = new QCConnection();
            DefectFilters = new ObservableCollection<QCDefectFilter>();
            Projects = new ObservableCollection<QCProject>();
            PriorityMappings = new ObservableCollection<QCPriorityMapping>();
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