using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using VersionOne.ServiceHost.ConfigurationTool.Validation;

namespace VersionOne.ServiceHost.ConfigurationTool.Entities {
    [XmlRoot("Connection")]
    public class QCConnection : INotifyPropertyChanged {
        public const string ApplicationUrlProperty = "ApplicationUrl";
        public const string UsernameProperty = "Username";
        public const string PasswordProperty = "Password";

        private string _appUrl;
        private string _username;
        private string _password;

        [NonEmptyStringValidator]
        public string ApplicationUrl { get { return _appUrl; }
            set
            {
                if (!value.Equals(_appUrl))
                {
                    _appUrl = value;
                    OnPropertyChanged();
                }
            } }

        [NonEmptyStringValidator]
        public string Username
        {
            get { return _username; }
            set
            {
                if (!value.Equals(_username))
                {
                    _username = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (!value.Equals(_password))
                {
                    _password = value;
                    OnPropertyChanged();
                }
            }
        }

        public override bool Equals(object obj) {
            if(obj == null || obj.GetType() != typeof(QCConnection)) {
                return false;
            }

            var other = (QCConnection) obj;
            return string.Equals(ApplicationUrl, other.ApplicationUrl) && 
                   string.Equals(Username, other.Username) &&
                   string.Equals(Password, other.Password);
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