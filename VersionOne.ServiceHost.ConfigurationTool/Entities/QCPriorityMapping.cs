using System.Xml.Serialization;
using VersionOne.ServiceHost.ConfigurationTool.Validation;

namespace VersionOne.ServiceHost.ConfigurationTool.Entities {
    public class QCPriorityMapping {
        private Mapping qualityCenterPriority;

        public const string VersionOnePriorityIdProperty = "VersionOnePriorityId";
        public const string QCPriorityNameProperty = "QCPriorityName";

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
            set { VersionOnePriority.Id = value; }
        }

        [XmlIgnore]
        [NonEmptyStringValidator]
        public string QCPriorityName {
            get { return qualityCenterPriority.Name; }
            set { qualityCenterPriority.Name = value; }
        }
    }
}