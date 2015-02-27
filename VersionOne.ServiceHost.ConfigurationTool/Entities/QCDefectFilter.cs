using System.Xml.Serialization;
using VersionOne.ServiceHost.ConfigurationTool.Validation;

namespace VersionOne.ServiceHost.ConfigurationTool.Entities {
    [XmlRoot("DefectFilter")]
    public class QCDefectFilter {
        public const string FieldNameProperty = "FieldName";
        public const string FieldValueProperty = "FieldValue";

        [NonEmptyStringValidator]
        public string FieldName { get; set; }

        public string FieldValue { get; set; }

        public override bool Equals(object obj) {
            if(obj == null || obj.GetType() != typeof(QCDefectFilter)) {
                return false;
            }

            var other = (QCDefectFilter) obj;

            return string.Equals(FieldName, other.FieldName) && string.Equals(FieldValue, other.FieldValue);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}