using System;
using TDAPIOLELib;
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
            TDConnection server = null;

            try {
                server = new TDConnection();
                server.InitConnectionEx(entity.Connection.ApplicationUrl);
                server.Login(entity.Connection.Username, entity.Connection.Password);
                return true;
            } catch (Exception) {
                return false;
            } finally {
                if (server != null && server.Connected) {
                    server.Disconnect();
                }
            }
        }
    }
}