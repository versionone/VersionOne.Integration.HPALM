using System.Collections.Generic;
using VersionOne.ServiceHost.Core;
using VersionOne.ServiceHost.Core.StartupValidation;
using VersionOne.ServiceHost.Eventing;

namespace VersionOne.ServiceHost.QualityCenterServices.StartupValidation {
    public class StartupChecker : StartupCheckerBase {
        private readonly QcConfiguration configuration;

        public StartupChecker(QcConfiguration configuration, IEventManager eventManager, IDependencyInjector dependencyInjector) : base(eventManager, dependencyInjector) {
            this.configuration = configuration;
        }

        protected override IEnumerable<IValidationStep> CreateValidators() {
            return new List<IValidationStep> {
                new ValidationSimpleStep(new QcConnectionValidator(configuration.Url, configuration.Username, configuration.Password), null),
            };
        }
    }
}
