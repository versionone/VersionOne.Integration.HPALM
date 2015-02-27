using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Ninject;
using VersionOne.Profile;
using VersionOne.ServiceHost.Core;
using VersionOne.ServiceHost.Core.Logging;
using VersionOne.ServiceHost.Core.Services;
using VersionOne.ServiceHost.Eventing;
using VersionOne.ServiceHost.TestServices;
using VersionOne.ServiceHost.WorkitemServices;

namespace VersionOne.ServiceHost.QualityCenterServices {
    /// <summary>
    /// Handles events for:
    ///  creating tests int Quality Center
    ///  updating Quality Center after pushing a Defect to V1
    ///  updating Quality Center after a Defect is closed in V1
    /// 
    /// Publishes events for:
    ///  notifying V1 that a Test was (or wasn't) successfully created in Quality Center
    /// </summary>
    public class QualityCenterUpdaterService : IHostedService {
        private readonly QualityCenterReaderUpdater server;
        private IEventManager eventManager;
        private ILogger logger;

        public QualityCenterUpdaterService(QualityCenterReaderUpdater server) {
            this.server = server;
        }

        #region IHostedService Members

        public void Initialize(XmlElement config, IEventManager eventManager, IProfile profile) {
            this.eventManager = eventManager;
            this.eventManager.Subscribe(typeof(V1TestReady), CreateQCTest);
            this.eventManager.Subscribe(typeof(WorkitemCreationResult), OnDefectCreated);
            this.eventManager.Subscribe(typeof(WorkitemStateChangeCollection), OnDefectStateChange);
            this.eventManager.Subscribe(typeof(ServiceHostState), HostStateChanged);

            logger = new Logger(eventManager);
        }

        public void Start() {
            // TODO move subscriptions to timer events, etc. here
        }

        #endregion

        private void HostStateChanged(object pubobj) {
            var state = (ServiceHostState)pubobj;
            
            if(state == ServiceHostState.Shutdown) {
                server.Close();
            }
        }

        //all configured qc instances will get all events for all closed defects, 
        //and must determine if that instance should handle that defect
        public void OnDefectStateChange(object pubobj) {
            var workitemChanges = (WorkitemStateChangeCollection)pubobj;
            
            var defectsToHandle = workitemChanges.Where(defectChange => server.HandlesQCProject(defectChange.ExternalId)).ToList();

            if(defectsToHandle.Count > 0) {
                try {
                    foreach(var defectChange in defectsToHandle) {
                        logger.Log(LogMessage.SeverityType.Debug, string.Format("Updating QC Defect {0}", defectChange.ExternalId));
                        workitemChanges.ChangesProcessed = server.OnDefectStateChange(defectChange);
                        logger.Log(LogMessage.SeverityType.Debug, string.Format("\tErrors? {0}", defectChange.ChangesProcessed));
                    }
                } catch(Exception ex) {
                    logger.Log(LogMessage.SeverityType.Error, ex.Message);
                }
            }
        }

        //all configured qc instances will get all events for all created defects, 
        //and must determine if that instance should handle that defect
        public void OnDefectCreated(object pubobj) {
            var result = (WorkitemCreationResult)pubobj;

            if(server.HandlesV1Project(result.Source.Project)) {
                try {
                    server.OnDefectCreated(result);
                } catch(Exception ex) {
                    logger.Log(LogMessage.SeverityType.Error, ex.Message);
                }
            }
        }

        public void CreateQCTest(object pubobj) {
            var testReady = (V1TestReady)pubobj;

            try {
                logger.Log(LogMessage.SeverityType.Debug, string.Format("Create Test {0} in QC Project {1}", testReady.DisplayId, testReady.Project));
                var testEvent = server.CreateTest(testReady);

                if(testEvent != null) {
                    eventManager.Publish(testEvent);
                }
            } catch(Exception ex) {
                logger.Log(LogMessage.SeverityType.Error, ex.Message);
            }
        }
    }
}