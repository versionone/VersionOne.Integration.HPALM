using System.Xml;
using Ninject;
using VersionOne.Profile;
using VersionOne.ServiceHost.Core.Services;
using VersionOne.ServiceHost.Eventing;

namespace VersionOne.ServiceHost.TestServices {
    /// <summary>
    /// Wrapper class around TestReaderService and TestWriterService so we can have one declaration in the config file.
    /// </summary>
    internal class TestService : IHostedService {
        private readonly TestReaderService readerService = new TestReaderService();
        private readonly TestWriterService writerService = new TestWriterService();

        public void Initialize(XmlElement config, IEventManager eventManager, IProfile profile) {
            readerService.Initialize(config, eventManager, profile);
            writerService.Initialize(config, eventManager, profile);
        }

        public void Start() {
            // TODO move subscriptions to timer events, etc. here
        }
    }
}