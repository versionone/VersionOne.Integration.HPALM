using System;
using VersionOne.ServiceHost.ConfigurationTool.Entities;

namespace VersionOne.ServiceHost.ConfigurationTool.DL {
    public class QCConnectionValidator : IConnectionValidator {
        private readonly QCServiceEntity entity;

        public QCConnectionValidator(QCServiceEntity entity) {
            if (entity == null) {
                throw new ArgumentNullException();
            }

            this.entity = entity;
        }

        public bool Validate() {
            HPALMConnector.HPALMConnector connector = null;

            try {
                connector = new HPALMConnector.HPALMConnector(entity.Connection.ApplicationUrl);
                var couldAuthenticate = connector.Authenticate(entity.Connection.Username, entity.Connection.Password);

                return couldAuthenticate;
            } catch (Exception) {
                return false;
            } finally {
                if (connector != null) {
                    connector.Logout();
                    connector.Dispose();
                }
            }
        }
    }
}