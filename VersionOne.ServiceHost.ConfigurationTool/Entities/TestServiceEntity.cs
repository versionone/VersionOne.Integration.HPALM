using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using VersionOne.ServiceHost.ConfigurationTool.Attributes;
using VersionOne.ServiceHost.ConfigurationTool.Validation;

namespace VersionOne.ServiceHost.ConfigurationTool.Entities {
    // NOTE Nobody is using TestReader service directly, so to decrease redundancy we inherit from TestWriter entity. It would also allow simpler XML serialization. 
    [XmlRoot("TestService")]
    [HasSelfValidation]
    public class TestServiceEntity : TestWriterEntity {
        public const string BaseQueryFilterProperty = "BaseQueryFilter";
        public const string ProjectsProperty = "Projects";

        private string _baseQueryFilter;
        private ObservableCollection<TestPublishProjectMapping> _projects;
            
        [NonEmptyStringValidator]
        [HelpString(HelpResourceKey = "TestServicesBaseQueryFilter")]
        public string BaseQueryFilter {
            get { return _baseQueryFilter; }
            set
            {
                if (!value.Equals(_baseQueryFilter))
                {
                    _baseQueryFilter = value;
                    NotifyPropertyChanged();
                }
            } }

        [XmlArray("TestPublishProjectMap")]
        [XmlArrayItem("V1Project")]
        [HelpString(HelpResourceKey = "TestServicesV1Project")]
        public ObservableCollection<TestPublishProjectMapping> Projects {
            get { return _projects; }
            set
            {
                if (value != _projects)
                {
                    _projects = value;
                    NotifyPropertyChanged();
                }
            } }

        public TestServiceEntity() {
            Projects = new ObservableCollection<TestPublishProjectMapping>();
            CreateTimer(TimerEntity.DefaultTimerIntervalMinutes);
        }

        [SelfValidation]
        public void CheckProjects(ValidationResults results) {
            var validator = ValidationFactory.CreateValidatorFromAttributes(typeof(TestPublishProjectMapping),
                string.Empty);

            foreach (TestPublishProjectMapping project in Projects) {
                var projectResults = validator.Validate(project);
                
                if(!projectResults.IsValid) {
                    results.AddAllResults(projectResults);
                }
            }
        }
    }
}