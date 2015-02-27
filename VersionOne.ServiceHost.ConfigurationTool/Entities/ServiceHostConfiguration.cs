using System;
using System.Collections.Generic;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using VersionOne.ServiceHost.ConfigurationTool.UI.Interfaces;

namespace VersionOne.ServiceHost.ConfigurationTool.Entities {
    /// <summary>
    /// Container entity for all services settings.
    /// </summary>
    [HasSelfValidation]
    public class ServiceHostConfiguration {
        private readonly List<BaseServiceEntity> services = new List<BaseServiceEntity>();

        private ProxyConnectionSettings proxySettings = new ProxyConnectionSettings();

        public VersionOneSettings Settings { get; private set; }

        public ProxyConnectionSettings ProxySettings {
            get { return proxySettings; }
            private set { proxySettings = value; }
        }

        public bool HasChanged { get; set; }

        public IEnumerable<BaseServiceEntity> Services {
            get { return services; }
        }

        public ServiceHostConfiguration() {
            HasChanged = true;
        }

        public ServiceHostConfiguration(IEnumerable<BaseServiceEntity> entities) : this() {
            foreach(var entity in entities) {
                AddService(entity);
            }
        }

        public BaseServiceEntity this[Type type] {
            get { return services.Find(entity => entity.GetType() == type); }
        }

        public void AddService(BaseServiceEntity entity) {
            if(entity is IVersionOneSettingsConsumer) {
                var settingsConsumer = (IVersionOneSettingsConsumer) entity;

                if(Settings != null) {
                    settingsConsumer.Settings = Settings;
                } else {
                    Settings = settingsConsumer.Settings;
                    ProxySettings = settingsConsumer.Settings.ProxySettings;
                }
            }
            
            services.Add(entity);
        }

        [SelfValidation]
        public void CheckQualityCenterMappings(ValidationResults results) {
            var qcServiceEntity = (QCServiceEntity) this[typeof (QCServiceEntity)];
            var testServiceEntity = (TestServiceEntity) this[typeof (TestServiceEntity)];

            if(qcServiceEntity != null && testServiceEntity != null) {
                foreach (var mapping in testServiceEntity.Projects) {
                    var matchingProject = qcServiceEntity.Projects.Find(item => string.Equals(item.Id, mapping.DestinationProject));

                    if(matchingProject == null) {
                        var message = string.Format("Mapping '{0}' exists on Test Service page, but not on Quality Center page", mapping.DestinationProject);
                        results.AddResult(new ValidationResult(message, this, null, "Quality Center", null));
                    }
                }
            }
        }
    }
}