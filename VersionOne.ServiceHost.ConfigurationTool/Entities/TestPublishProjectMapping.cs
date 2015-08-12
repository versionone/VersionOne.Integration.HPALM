using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using VersionOne.ServiceHost.ConfigurationTool.Validation;

namespace VersionOne.ServiceHost.ConfigurationTool.Entities {
    [XmlRoot("V1Project")]
    public class TestPublishProjectMapping: INotifyPropertyChanged {
        public const string NameProperty = "Name";
        public const string IncludeChildrenProperty = "IncludeChildren";
        public const string DestinationProjectProperty = "DestinationProject";

        private string _name;
        private bool _includeChildren;
        private string _destinationProject;

        [NonEmptyStringValidator, XmlAttribute]
        public string Name {
            get { return _name; }
            set
            {
                if (!value.Equals(_name))
                {
                    _name = value;
                    OnPropertyChanged();
                }
            } }

        [XmlIgnore]
        public bool IncludeChildren {
            get
            {
                return _includeChildren;
            }
            set
            {
                if (!value.Equals(_includeChildren))
                {
                    _includeChildren = value;
                    OnPropertyChanged();
                }
            } }

        // TODO refactor TestReaderService
        /// <summary>
        /// Whether to include child projects
        /// This is supposed to represent a boolean value, "Y" for true and "N" (in fact, something not containing "Y") for false (by service design)
        /// </summary>
        [XmlAttribute("IncludeChildren")]
        public string IncludeChildrenString {
            get { return IncludeChildren ? "Y" : "N"; }
            set { IncludeChildren = value != null && value.ToUpperInvariant().Contains("Y"); }
        }

        [NonEmptyStringValidator, XmlText]
        public string DestinationProject {
            get { return _destinationProject; }
            set
            {
                if (!value.Equals(_destinationProject))
                {
                    _destinationProject = value;
                    OnPropertyChanged();
                }
            } }

        public override bool Equals(object obj) {
            if (obj == null || obj.GetType() != typeof(TestPublishProjectMapping)) {
                return false;
            }

            var other = (TestPublishProjectMapping) obj;

            return string.Equals(Name, other.Name) && string.Equals(DestinationProject, other.DestinationProject)
                && IncludeChildren == other.IncludeChildren;
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