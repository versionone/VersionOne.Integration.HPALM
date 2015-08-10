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
            Log(LogMessage.SeverityType.Info, "Validating QualityCenter connection");
            Log(LogMessage.SeverityType.Info, string.Format("    Using QualityCenter URL '{0}'", url));
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
                Log(LogMessage.SeverityType.Info, string.Format("    QualityCenter connection is valid"));
            } catch (Exception ex) {
                Log(LogMessage.SeverityType.Warning, "Failed to execute QualityCenter connection validation", ex);
                return false;
            } finally {
                if (connector != null) {
                    connector.Logout();
                    connector.Dispose();
                }
            }

            Log(LogMessage.SeverityType.Info, "QualityCenter connection settings are valid.");
            return true;
        }
    }
}
