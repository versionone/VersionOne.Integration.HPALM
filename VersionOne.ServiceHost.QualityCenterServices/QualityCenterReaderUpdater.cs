using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using VersionOne.ServiceHost.TestServices;
using VersionOne.ServiceHost.WorkitemServices;
using QC = VersionOne.ServiceHost.QualityCenterServices.QualityCenterClient;
using VersionOne.ServiceHost.Core.Configuration;
using VersionOne.ServiceHost.Core.Logging;

namespace VersionOne.ServiceHost.QualityCenterServices {
    /// <summary>
    /// This class knows about WorkitemServices and TestServices event objects and encapsulates the QualityCenterClient
    /// It creates the necessary event objects and object collections that eventually get published
    /// It does *not* know about the EventManager or the QualityCenter COM library
    /// It does *not* publish events directly or subscribe to events directly
    /// </summary>
    public class QualityCenterReaderUpdater {
        private ILogger logger;
        private string versionOneSourceField;

        private readonly IDictionary<string, IQualityCenterClient> projects = new Dictionary<string, IQualityCenterClient>();

        private readonly IDictionary<MappingInfo, MappingInfo> priorityMappings;

        public QualityCenterReaderUpdater(IDictionary<MappingInfo, MappingInfo> priorityMappings) {
            this.priorityMappings = priorityMappings;
        }

        #region Public Methods

        public void Initialize(XmlElement config, ILogger log) {
            logger = log;
            versionOneSourceField = config["SourceFieldValue"].InnerText;

            foreach (XmlNode node in config["QCProjects"].SelectNodes("Project")) {
                var project = new QCProject(node);
                projects.Add(node.Attributes["id"].InnerText, new QualityCenterClient(project, config, log));
            }
        }

        public ClosedWorkitemsSource ClosedWorkitemsSource {
            get { return new ClosedWorkitemsSource(versionOneSourceField); }
        }

        public bool HandlesV1Project(string v1Project) {
            return projects.Values.Any(x => v1Project == x.ProjectInfo.V1Project);
        }

        // id should be {domain}.{project}.{qc id for asset}
        public bool HandlesQCProject(string id) {
            var idParts = id.Split(new[] {'.'});
            return idParts.Length >= 3 && HandlesQCProject(idParts[0], idParts[1]);
        }

        public bool HandlesQCProject(string domain, string project) {
            return projects.Values.Any(x => domain == x.ProjectInfo.Domain && x.ProjectInfo.Project == project);
        }

        public PartnerTestEvent CreateTest(V1TestReady testData) {
            if (!projects.ContainsKey(testData.Project)) {
                return null;
            }

            var qcEvent = new PartnerTestEvent(testData);
            
            // TODO see if it is reasonable to lock
            lock (this) {
                try {
                    qcEvent.Reference = projects[testData.Project].CreateQCTest(testData.Title, testData.Description, testData.DisplayId);
                    logger.Log(LogMessage.SeverityType.Debug,
                            string.Format("Creating QC Test from \"{0}\" ({1}) ", testData.Title, testData.Oid));
                } catch (Exception ex) {
                    logger.Log(LogMessage.SeverityType.Error, ex.ToString());
                    qcEvent.Successful = false;
                    qcEvent.Reference = "Quality Center Test Creation Failed.  See integration log for details.";
                }
            }

            return qcEvent;
        }

        public ICollection<TestRun> GetLatestTestRuns(DateTime lastCheck) {
            var result = new List<TestRun>();
            
            lock (this) {
                foreach (var project in projects.Values) {
                    var testList = project.GetLatestTestRuns(lastCheck);

                    foreach (var test in testList) {
                        // ignore the test that have not been executed
                        if (!QC.HasLastRun(test)) {
                            continue;
                        }

                        var externalId = project.GetFullyQualifiedQCId(QC.TestID(test));
                        result.Add(CreateV1TestRun(externalId, QC.TimeStamp(test), QC.LastRunStatus(test)));
                    }
                }
            }

            return result;
        }

        public IList<Defect> GetLatestDefects(DateTime lastCheck) {
            IList<Defect> defects = new List<Defect>();
            
            lock (this) {
                foreach (var project in projects.Values) {
                    var bugList = project.GetLatestDefects(lastCheck);
                    
                    foreach (var bug in bugList) {
                        var externalId = project.GetFullyQualifiedQCId(QC.DefectID(bug));
                        // TODO map priorities
                        var defect = CreateV1Defect(QC.DefectSummary(bug), externalId, QC.DefectDescription(bug),
                                                       QC.DefectPriority(bug), project.ProjectInfo.V1Project);
                        defects.Add(defect);
                    }
                }
            }
            return defects;
        }

        public void OnDefectCreated(WorkitemCreationResult createdResult) {
            IQualityCenterClient project = GetProjectFromExternalId(createdResult.Source.ExternalId);

            if (project == null) {
                return;
            }

            lock (this) {
                project.OnDefectCreated(createdResult.Source.ExternalId,
                                        Combine(createdResult.Messages, createdResult.Warnings),
                                        createdResult.Permalink);
            }
        }

        public bool OnDefectStateChange(WorkitemStateChangeResult stateChangeResult) {
            var project = GetProjectFromExternalId(stateChangeResult.ExternalId);

            if (project == null) {
                return false;
            }

            lock (this) {
                return project.OnDefectStateChange(stateChangeResult.ExternalId,
                                                   Combine(stateChangeResult.Messages, stateChangeResult.Warnings));
            }
        }

        // id should be {domain}.{project}.{qc id for asset}
        private IQualityCenterClient GetProjectFromExternalId(string externalId) {
            if (!HandlesQCProject(externalId)) {
                return null;
            }

            var idParts = externalId.Split(new[] {'.'});
            var domain = idParts[0];
            var project = idParts[1];

            return projects.Values.FirstOrDefault(value => domain == value.ProjectInfo.Domain && project == value.ProjectInfo.Project);
        }

        #endregion

        #region Private Methods

        private static TestRun CreateV1TestRun(string externalId, DateTime timestamp, string status) {
            var oneRun = new TestRun(timestamp, externalId);
            
            if (status == "Passed") {
                oneRun.State = TestRun.TestRunState.Passed;
            } else {
                oneRun.State = (status == "Failed")
                                   ? TestRun.TestRunState.Failed
                                   : TestRun.TestRunState.NotRun;
            }

            return oneRun;
        }

        private Defect CreateV1Defect(string title, string externalId, string description, string priority, string v1Project) {
            var defect = new Defect {
                                        Title = title,
                                        ExternalId = externalId,
                                        ExternalSystemName = versionOneSourceField,
                                        Project = v1Project,
                                        Description = description
                                    };

            var mappedVersionOnePriority = ResolvePriorityMapping(priority);

            if (mappedVersionOnePriority != null) {
                defect.Priority = mappedVersionOnePriority.Id;
            }

            return defect;
        }

        private MappingInfo ResolvePriorityMapping(string qcPriorityName) {
            foreach (var mappingPair in priorityMappings) {
                if (mappingPair.Value.Name.Equals(qcPriorityName)) {
                    return mappingPair.Key;
                }
            }

            var mapping = priorityMappings.FirstOrDefault(x => x.Value.Name.Equals(qcPriorityName));
            return MappingPairEmpty(mapping) ? mapping.Key : null;
        }

        private static bool MappingPairEmpty(KeyValuePair<MappingInfo, MappingInfo> pair) {
            return pair.Key == null && pair.Value == null;
        }

        private static ArrayList Combine(ICollection listOne, ICollection listTwo) {
            var combined = new ArrayList(listOne);
            combined.AddRange(listTwo);
            return combined;
        }

        #endregion

        public void Close() {
            foreach (var pair in projects) {
                try {
                    pair.Value.Dispose();
                } catch(Exception ex) {
                    logger.Log(LogMessage.SeverityType.Warning, "Failed to process a project when closing", ex);
                }
            }
        }
    }
}