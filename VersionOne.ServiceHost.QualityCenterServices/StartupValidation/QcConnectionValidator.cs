using System;
using VersionOne.ServiceHost.Core.Logging;
using VersionOne.ServiceHost.Core.StartupValidation;

namespace VersionOne.ServiceHost.QualityCenterServices.StartupValidation {
    public class QcConnectionValidator : BaseValidationEntity, ISimpleValidator {
        private readonly string url;
        private readonly string username;
        private readonly string password;
        
        public QcConnectionValidator(string url, string username, string password) {
            this.url = url;
            this.username = username;
            this.password = password;
        }

        public bool Validate() {
            Log(LogMessage.SeverityType.Info, "Validating HP-ALM connection");
            Log(LogMessage.SeverityType.Info, string.Format("    Using HP-ALM URL '{0}'", url));
            Log(LogMessage.SeverityType.Info, string.Format("    Username: '{0}'", username));

            //ITDConnection2 connection = null;
            HPALMConnector.HPALMConnector connector = null;

            try {
                connector = new HPALMConnector.HPALMConnector(url);
                connector.Authenticate(username, password);

                //string majorVersion;
                //string buildNumber;

                //connection.GetTDVersion(out majorVersion, out buildNumber);
                //Log(LogMessage.SeverityType.Info, string.Format("    QualityCenter version is {0}.{1}", majorVersion, buildNumber));
                Log(LogMessage.SeverityType.Info, string.Format("    HP-ALM connection is valid"));
            } catch (Exception ex) {
                Log(LogMessage.SeverityType.Warning, "Failed to execute HP-ALM connection validation", ex);
                return false;
            } finally {
                if (connector != null) {
                    connector.Logout();
                    connector.Dispose();
                }
            }

            Log(LogMessage.SeverityType.Info, "HP-ALM connection settings are valid.");
            return true;
        }
    }
}
