using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using VersionOne.ServiceHost.ConfigurationTool.Validation;
using VersionOne.ServiceHost.ConfigurationTool.Attributes;
using VersionOne.ServiceHost.Core.Configuration;

namespace VersionOne.ServiceHost.ConfigurationTool.Entities {
    /// <summary>
    /// VersionOne connection settings node backing class.
    /// </summary>
    [XmlRoot("Settings")]
    public class VersionOneSettings {
        public const string AccessTokenAuthProperty = "AccessTokenAuth";
        public const string BasicAuthProperty = "BasicAuth";
        public const string IntegratedAuthProperty = "IntegratedAuth";

        public const string ApplicationUrlProperty = "ApplicationUrl";
        public const string AccessTokenProperty = "AccessToken";
        public const string UsernameProperty = "Username";
        public const string PasswordProperty = "Password";

       
        private string applicationUrl;
        private string accessToken;
        private string username;
        private string password;

        public VersionOneSettings() {
            ProxySettings = new ProxyConnectionSettings();
        }

        [XmlElement("APIVersion")]
        public string ApiVersion {
            get { return "7.2.0.0"; }
            set { }
        }

        [HelpString(HelpResourceKey="V1PageVersionOneUrl")]
        [NonEmptyStringValidator]
        public string ApplicationUrl
        {
            get { return applicationUrl; }
            set
            {
                if (applicationUrl != value)
                {
                    applicationUrl = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [NonEmptyStringValidator]
        public string Username
        {
            get { return username; }
            set
            {
                if (username != value)
                {
                    username = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [NonEmptyStringValidator]
        public string Password
        {
            get { return password; }
            set
            {
                if (password != value)
                {
                    password = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //[HelpString(HelpResourceKey="V1PageIntegratedAuth")]
        //public bool IntegratedAuth { get; set; }

        public ProxyConnectionSettings ProxySettings { get; set; }

        public AuthenticationTypes AuthenticationType { get; set; }

        [NonEmptyStringValidator]
        public string AccessToken
        {
            get { return accessToken; }
            set
            {
                if (accessToken != value)
                {
                    accessToken = value;
                    //NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


    }
}