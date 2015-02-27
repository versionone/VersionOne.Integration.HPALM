using System;
using System.Collections.Generic;
using System.Xml;
using VersionOne.Profile;
using VersionOne.ServiceHost.Core.Logging;
using VersionOne.ServiceHost.Core.Services;
using VersionOne.ServiceHost.Eventing;
using VersionOne.ServiceHost.WorkitemServices;

namespace VersionOne.ServiceHost.QualityCenterServices {
    /// <summary>
    /// At the configured Interval, publishes events for:
    ///  updating Tests in V1 from Quality Center Test Runs
    ///  creating new Defects in V1 from Quality Center Defects
    ///  asking V1 to check for Closed Defects that have a Source of Quality Center
    /// </summary>
    public class QualityCenterReaderService : IHostedService {
        private readonly QualityCenterReaderUpdater server;
        private IEventManager eventManager;
        private DateTime lastScanTimestamp;
        private IProfile profile;
        private ILogger logger;

        public QualityCenterReaderService(QualityCenterReaderUpdater qcServer) {
            server = qcServer;
        }

        public void Initialize(XmlElement config, IEventManager eventManager, IProfile profile) {
            this.eventManager = eventManager;
            this.profile = profile;
            this.eventManager.Subscribe(typeof(IntervalSync), OnInterval);

            logger = new Logger(eventManager);
        }

        public void Start() {
            // TODO move subscriptions to timer events, etc. here
        }

        private void OnInterval(object pubobj) {
            CalculateLastScannedTimestamp();
            PublishTestRuns();
            PublishDefects();
            AskV1ToCheckForClosedDefects();
        }

        private void PublishDefects() {
            IList<Defect> defects;

            try {
                defects = server.GetLatestDefects(lastScanTimestamp);
            } catch(Exception ex) {
                logger.Log(LogMessage.SeverityType.Error, "Error getting Issues from Quality Center:");
                logger.Log(LogMessage.SeverityType.Error, ex.ToString());
                return;
            }

            //TODO now there is a quantity of issues twice more
            logger.Log(string.Format("Found {0} issues in Quality Center to create in VersionOne.", defects.Count));

            foreach(var defect in defects) {
                eventManager.Publish(defect);
            }
        }

        private void AskV1ToCheckForClosedDefects() {
            eventManager.Publish(server.ClosedWorkitemsSource);
        }

        private void PublishTestRuns() {
            var testRuns = server.GetLatestTestRuns(lastScanTimestamp);
            //TODO put in log info about publish tests from VersionOne
            logger.Log(LogMessage.SeverityType.Debug, string.Format("Update {0} test", testRuns.Count));
            
            foreach(var run in testRuns) {
                eventManager.Publish(run);
            }
        }

        private void CalculateLastScannedTimestamp() {
            var value = profile["LastScannedTimeStamp"].Value;
            logger.Log(LogMessage.SeverityType.Debug, string.Format("LastScannedTimeStamp from profile {0}", value));
            lastScanTimestamp = null == value ? DateTime.Now : DateTime.Parse(value);
            profile["LastScannedTimeStamp"].Value = DateTime.Now.ToString();
        }

        public class IntervalSync { }
    }
}