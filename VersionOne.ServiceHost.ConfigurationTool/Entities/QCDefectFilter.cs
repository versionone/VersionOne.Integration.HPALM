using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using VersionOne.ServiceHost.ConfigurationTool.Validation;

namespace VersionOne.ServiceHost.ConfigurationTool.Entities {
    [XmlRoot("DefectFilter")]
    public class QCDefectFilter : INotifyPropertyChanged {
        public const string FieldNameProperty = "FieldName";
        public const string FieldValueProperty = "FieldValue";

        private string _fieldName;
        private string _fieldValue;

        [NonEmptyStringValidator]
        public string FieldName {
            get
            {
                return _fieldName;
            }
            set
            {
                if (!value.Equals(_fieldName))
                {
                    _fieldName = value;
                    OnPropertyChanged();
                }
            } }

        public string FieldValue
        {
            get
            {
                return _fieldValue;
            }
            set
            {
                if (!value.Equals(_fieldValue))
                {
                    _fieldValue = value;
                    OnPropertyChanged();
                }
            }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}