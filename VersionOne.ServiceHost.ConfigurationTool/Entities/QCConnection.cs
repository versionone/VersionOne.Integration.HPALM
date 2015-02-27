using System.Xml.Serialization;
using VersionOne.ServiceHost.ConfigurationTool.Validation;

namespace VersionOne.ServiceHost.ConfigurationTool.Entities {
    [XmlRoot("Connection")]
    public class QCConnection {
        public const string ApplicationUrlProperty = "ApplicationUrl";
        public const string UsernameProperty = "Username";
        public const string PasswordProperty = "Password";

        [NonEmptyStringValidator]
        public string ApplicationUrl { get; set; }

        [NonEmptyStringValidator]
        public string Username { get; set; }

        public string Password { get; set; }

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
    }
}