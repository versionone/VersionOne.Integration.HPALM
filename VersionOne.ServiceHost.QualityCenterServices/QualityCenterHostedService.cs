using System.Xml;
using Ninject;
using VersionOne.Profile;
using VersionOne.ServiceHost.Core.Logging;
using VersionOne.ServiceHost.Core.Services;
using VersionOne.ServiceHost.Core.Utility;
using VersionOne.ServiceHost.Eventing;
using VersionOne.ServiceHost.QualityCenterServices.StartupValidation;

namespace VersionOne.ServiceHost.QualityCenterServices {
    /// <summary>
    /// Wraps the QualityCenterUpdaterService and QualityCenterReaderService, which use the same Configuration
    /// </summary>
    public class QualityCenterHostedService : IHostedService, IComponentProvider {
        private readonly QcConfiguration configuration = new QcConfiguration();
        
        private QualityCenterReaderService qcReader;
        private QualityCenterReaderUpdater qcServer;
        private QualityCenterUpdaterService qcWriter;

        private StartupChecker startupChecker;
        private ILogger logger;
        private IEventManager eventManager;

        public void Initialize(XmlElement config, IEventManager eventManager, IProfile profile) {
            ConfigurationReader.ProcessMappingSettings(configuration.PriorityMappings, config["PriorityMappings"], "VersionOnePriority", "QualityCenterPriority");
            
            this.eventManager = eventManager;
            logger = new Logger(eventManager);

            ParseConfiguration(config);

            qcServer = new QualityCenterReaderUpdater(configuration.PriorityMappings);
            qcWriter = new QualityCenterUpdaterService(qcServer);
            qcReader = new QualityCenterReaderService(qcServer);

            qcServer.Initialize(config, new Logger(eventManager));
            qcWriter.Initialize(config, eventManager, profile);
            qcReader.Initialize(config, eventManager, profile);
        }

        // TODO move all settings to entity, apply Core parsing algorithms & do not pass XML node everywhere
        private void ParseConfiguration(XmlElement config) {
            XmlNode connection = config["Connection"];

            configuration.Url = connection["ApplicationUrl"].InnerText;
            configuration.Username = connection["Username"].InnerText;
            configuration.Password = connection["Password"].InnerText;
        }

        public void Start() {
            startupChecker.Initialize();
        }

        public void RegisterComponents(IKernel container) {
            container.Rebind<IEventManager>().ToConstant(eventManager);
            container.Rebind<ILogger>().ToConstant(logger);

            container.Bind<QcConfiguration>().ToConstant(configuration);
            container.Bind<StartupChecker>().To<StartupChecker>();

            startupChecker = container.Get<StartupChecker>();
        }
    }
}