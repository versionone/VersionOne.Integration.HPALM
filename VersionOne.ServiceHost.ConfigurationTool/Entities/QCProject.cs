using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using VersionOne.ServiceHost.ConfigurationTool.Validation;

namespace VersionOne.ServiceHost.ConfigurationTool.Entities {
    // TODO decide on whether we need some V1 Project wrapper class to bind comboboxes in grids
    public class QCProject : INotifyPropertyChanged{
        public const string IdProperty = "Id";
        public const string DomainProperty = "Domain";
        public const string ProjectProperty = "Project";
        public const string TestFolderProperty = "TestFolder";
    	public const string TestStatusProperty = "TestStatus";
        public const string VersionOneIdFieldProperty = "VersionOneIdField";
        public const string VersionOneProjectProperty = "VersionOneProject";

        private string _id;
        private string _domain;
        private string _project;
        private string _testFolder;
        private string _testStatus;
        private string _versionOneIdField;
        private string _versionOneProject;

        [XmlAttribute("id")]
        [NonEmptyStringValidator]
        public string Id {
            get { return _id; }
            set
            {
                if (!value.Equals(_id))
                {
                    _id = value;
                    OnPropertyChanged();
                }
            } }

        [NonEmptyStringValidator]
        public string Domain { get { return _domain; } set{
            if (!value.Equals(_domain))
            {
                _domain = value;
                OnPropertyChanged();
            }} }

        [NonEmptyStringValidator]
        public string Project { get { return _project; } set{
            if (!value.Equals(_project))
            {
                _project = value;
                OnPropertyChanged();
            }} }

        [NonEmptyStringValidator]
        public string TestFolder { get{return _testFolder;}
            set
            {
                if (!value.Equals(_testFolder))
                {
                    _testFolder = value;
                    OnPropertyChanged();
                }
            } }

		[NonEmptyStringValidator]
		public string TestStatus {
		    get
		    {
		        return _testStatus;
		    }
		    set
		    {
                if (!value.Equals(_testStatus))
		        {
		            _testStatus = value;
                    OnPropertyChanged();
		        }
		    } }

        [NonEmptyStringValidator]
        public string VersionOneIdField { get{return _versionOneIdField;}
            set
            {
                if (!value.Equals(_versionOneIdField))
                {
                    _versionOneIdField = value;
                    OnPropertyChanged();
                }
            } }

        [NonEmptyStringValidator]
        public string VersionOneProject {
            get
            {
                return _versionOneProject;
            } set{
                if (!value.Equals(_versionOneProject))
                {
                    _versionOneProject = value;
                    OnPropertyChanged();
                }} }

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}