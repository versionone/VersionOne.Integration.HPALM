using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using VersionOne.ServiceHost.ConfigurationTool.Validation;

namespace VersionOne.ServiceHost.ConfigurationTool.Entities {
    public class QCPriorityMapping : INotifyPropertyChanged {
       
        private Mapping qualityCenterPriority;
        
        public const string VersionOnePriorityNameProperty = "VersionOnePriorityName";
        public const string VersionOnePriorityIdProperty = "VersionOnePriorityId";
        public const string QCPriorityNameProperty = "QCPriorityName";
        public const string QCPriorityIdProperty = "QCPriorityId";

        public QCPriorityMapping() {
            QualityCenterPriority = new Mapping();
            VersionOnePriority = new Mapping();
        }

        public Mapping QualityCenterPriority {
            get { return qualityCenterPriority; }
            set { qualityCenterPriority = value; }
        }

        public Mapping VersionOnePriority { get; set; }

        [XmlIgnore]
        [NonEmptyStringValidator]
        public string VersionOnePriorityId {
            get { return VersionOnePriority.Id; }
            set { if (!value.Equals(VersionOnePriority.Id))
            {
                VersionOnePriority.Id = value;
                OnPropertyChanged();
            } }
        }

        [XmlIgnore]
        [NonEmptyStringValidator]
        public string QCPriorityName {
            get { return qualityCenterPriority.Name; }
            set { if (!qualityCenterPriority.Name.Equals(value))
            {
                qualityCenterPriority.Name = value;
                OnPropertyChanged();
            } }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}