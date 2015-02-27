using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using VersionOne.Profile;
using VersionOne.SDK.APIClient;
using VersionOne.ServiceHost.Core.Logging;
using VersionOne.ServiceHost.Core.Services;
using VersionOne.ServiceHost.Eventing;

namespace VersionOne.ServiceHost.TestServices {
    public class TestWriterService : V1WriterServiceBase {
        private static readonly NeededAssetType[] neededAssetTypes = {
                new NeededAssetType("TestSuite", new[] {"Reference"}),
                new NeededAssetType("TestRun", new[] {"Name", "Description", "Date", "Passed", "Failed", "NotRun", "Elapsed"}),
                new NeededAssetType("Test",
                    new[]
                    {"Status", "Name", "AssetState", "Parent", "Parent.Number", "Scope", "Scope.Schedule", "Timebox"}),
                new NeededAssetType("TestStatus", new[] {"Key"}),
                new NeededAssetType("Defect", new[] {"AffectedPrimaryWorkitems", "Scope", "Timebox", "Name", "Description", "AssetState"}),
                new NeededAssetType("Timebox", new[] {"AssetState", "ID", "Schedule", "EndDate"}),
                new NeededAssetType("PrimaryWorkitem", new[] {"ID"}),
            };

        private readonly IDictionary<TestRun.TestRunState, Oid> states = new Dictionary<TestRun.TestRunState, Oid>();
        private readonly IDictionary timeboxes = new ListDictionary();
        private string changeComment;
        private CreateDefect createDefect;
        private string descriptionsuffix;
        private string testreferenceattributetoken;

        private IAssetType TestSuiteType {
            get { return Central.MetaModel.GetAssetType("TestSuite"); }
        }

        private IAssetType TestRunType {
            get { return Central.MetaModel.GetAssetType("TestRun"); }
        }

        private IAssetType TestType {
            get { return Central.MetaModel.GetAssetType("Test"); }
        }

        private IAssetType PrimaryWorkitem {
            get { return Central.MetaModel.GetAssetType("PrimaryWorkitem"); }
        }

        private IAssetType TestStatusType {
            get { return Central.MetaModel.GetAssetType("TestStatus"); }
        }

        private IAssetType TimeboxType {
            get { return Central.MetaModel.GetAssetType("Timebox"); }
        }

        private IAttributeDefinition TestReferenceDef {
            get { return TestType.GetAttributeDefinition(testreferenceattributetoken); }
        }

        private IAttributeDefinition TestStatusDef {
            get { return TestType.GetAttributeDefinition("Status"); }
        }

        private IAttributeDefinition TestNameDef {
            get { return TestType.GetAttributeDefinition("Name"); }
        }

        private IAttributeDefinition TestAssetStateDef {
            get { return TestType.GetAttributeDefinition("AssetState"); }
        }

        private IAttributeDefinition TestParentDef {
            get { return TestType.GetAttributeDefinition("Parent"); }
        }

        private IAttributeDefinition TestParentNumberDef {
            get { return TestType.GetAttributeDefinition("Parent.Number"); }
        }

        private IAttributeDefinition TestScopeDef {
            get { return TestType.GetAttributeDefinition("Scope"); }
        }

        private IAttributeDefinition TestScheduleDef {
            get { return TestType.GetAttributeDefinition("Scope.Schedule"); }
        }

        private IAttributeDefinition TestTimeboxDef {
            get { return TestType.GetAttributeDefinition("Timebox"); }
        }

        private IAttributeDefinition DefectAffectedWorkitemsDef {
            get { return DefectType.GetAttributeDefinition("AffectedPrimaryWorkitems"); }
        }

        private IAttributeDefinition DefectAssetStateDef {
            get { return DefectType.GetAttributeDefinition("AssetState"); }
        }

        private IAttributeDefinition DefectScopeDef {
            get { return DefectType.GetAttributeDefinition("Scope"); }
        }

        private IAttributeDefinition DefectTimeboxDef {
            get { return DefectType.GetAttributeDefinition("Timebox"); }
        }

        private IAttributeDefinition TimeboxAssetStateDef {
            get { return TimeboxType.GetAttributeDefinition("AssetState"); }
        }

        protected override IEnumerable<NeededAssetType> NeededAssetTypes {
            get { return neededAssetTypes; }
        }

        #region IHostedService Members

        public override void Initialize(XmlElement config, IEventManager eventManager, IProfile profile) {
            base.Initialize(config, eventManager, profile);
            testreferenceattributetoken = config["TestReferenceAttribute"].InnerText;

            VerifyMeta();

            LoadOid(TestRun.TestRunState.Passed, config, null);
            LoadOid(TestRun.TestRunState.Failed, config, null);
            LoadOid(TestRun.TestRunState.NotRun, config, Oid.Null);

            changeComment = config["ChangeComment"].InnerText;
            descriptionsuffix = config["DescriptionSuffix"].InnerText;

            if(!string.IsNullOrEmpty(config["CreateDefect"].InnerText)) {
                createDefect = (CreateDefect)Enum.Parse(typeof(CreateDefect), config["CreateDefect"].InnerText);
            } else {
                createDefect = CreateDefect.All;
            }

            eventManager.Subscribe(typeof(SuiteRun), SuiteRunSave);
            eventManager.Subscribe(typeof(TestRun), TestRunSave);
            eventManager.Subscribe(typeof(PartnerTestEvent), PartnerTestCreated);
        }

        #endregion

        private void LoadOid(TestRun.TestRunState state, XmlElement config, Oid def) {
            var configkey = state.ToString() + "Oid";
            var oidconfig = config[configkey];
            var oidtoken = oidconfig != null ? oidconfig.InnerText : null;
            var oid = def;

            try {
                oid = Central.Services.GetOid(oidtoken);
            } catch(OidException) {
            }

            //the oid is null-null or its not oid-null and we can't find it in the V1 system. (Oid.Null is ok!)
            if(oid == null) {
                throw new InvalidOperationException(string.Format("Invalid Oid Token for {0}: {1}", configkey, oidtoken));
            }
 
            if(oid != Oid.Null) {
                if(oid.AssetType != TestStatusType) {
                    throw new InvalidOperationException(string.Format("Oid for {0} is not a TestStatus Type: {1}", configkey, oidtoken));
                }

                var q = new Query(oid.AssetType);
                var term = new FilterTerm(oid.AssetType.GetAttributeDefinition("Key"));
                term.Equal(oid.Key);
                q.Filter = term;
                var assetlist = Central.Services.Retrieve(q).Assets;
                
                if(assetlist.Count == 0) {
                    throw new InvalidOperationException(string.Format("TestStatus for {0} does not exist: {1}", configkey, oidtoken));
                }
            }

            states.Add(state, oid);
        }

        private void PartnerTestCreated(object pubobj) {
            var partnerEvent = (PartnerTestEvent)pubobj;
            Logger.Log(LogMessage.SeverityType.Debug,
                string.Format("Update V1 Test {0}: set {1}={2}", partnerEvent.Oid, testreferenceattributetoken, partnerEvent.Reference));

            var query = new Query(Oid.FromToken(partnerEvent.Oid, Central.MetaModel));
            var result = Central.Services.Retrieve(query);
            result.Assets[0].SetAttributeValue(TestReferenceDef, partnerEvent.Reference);
            
            if(!partnerEvent.Successful) {
                result.Assets[0].SetAttributeValue(TestStatusDef, states[TestRun.TestRunState.Failed]);
            }
            
            Central.Services.Save(result.Assets[0]);
        }

        private void SuiteRunSave(object pubobj) {
            var run = (SuiteRun)pubobj;
            Logger.Log(LogMessage.SeverityType.Debug, run.ToString());

            if(string.IsNullOrEmpty(run.SuiteRef)) {
                Logger.Log(LogMessage.SeverityType.Debug, "Suite Reference is null or empty. Skipping...");
                return;
            }

            var q = new Query(TestSuiteType);
            var term = new FilterTerm(TestSuiteType.GetAttributeDefinition("Reference"));
            term.Equal(run.SuiteRef);
            q.Filter = term;
            var r = Central.Services.Retrieve(q);

            if(r.Assets.Count == 0) {
                Logger.Log(LogMessage.SeverityType.Debug, "No TestSuite found by reference: " + run.SuiteRef);
                return;
            }

            var save = new AssetList();

            foreach(var testsuite in r.Assets) {
                var testrun = Central.Services.New(TestRunType, testsuite.Oid);

                testrun.SetAttributeValue(TestRunType.GetAttributeDefinition("Name"), run.Name);
                testrun.SetAttributeValue(TestRunType.GetAttributeDefinition("Description"), run.Description);
                testrun.SetAttributeValue(TestRunType.GetAttributeDefinition("Date"), run.Stamp);

                testrun.SetAttributeValue(TestRunType.GetAttributeDefinition("Passed"), run.Passed);
                testrun.SetAttributeValue(TestRunType.GetAttributeDefinition("Failed"), run.Failed);
                testrun.SetAttributeValue(TestRunType.GetAttributeDefinition("NotRun"), run.NotRun);

                testrun.SetAttributeValue(TestRunType.GetAttributeDefinition("Elapsed"), run.Elapsed);

                LogSuiteRun(testrun);

                save.Add(testrun);
            }

            Central.Services.Save(save);
        }

        private void LogSuiteRun(Asset suiterun) {
            Logger.Log("Suite:\r\n" + GetAssetText(suiterun));
        }

        private static string GetAssetText(Asset testrun) {
            var sb = new StringBuilder();

            foreach(var entry in testrun.Attributes) {
                var attrib = entry.Value;
                if(attrib != null) {
                    sb.Append(new string('\t', 2) + attrib.Definition.Token + " = " + attrib.Value + "\r\n");
                }
            }

            return sb.ToString();
        }

        private void TestRunSave(object pubobj) {
            var run = (TestRun)pubobj;
            Logger.Log(LogMessage.SeverityType.Debug, run.ToString());

            if(string.IsNullOrEmpty(run.TestRef)) {
                Logger.Log(LogMessage.SeverityType.Debug, "Test Reference is null or empty. Skipping...");
                return;
            }

            var newStatus = states[run.State];
            var tests = GetRelatedTests(run);

            if(tests.Assets.Count == 0) {
                Logger.Log(LogMessage.SeverityType.Debug, "No Tests found by reference: " + run.TestRef);
            }

            foreach(var test in tests.Assets) {
                var stateAttribute = test.GetAttribute(TestAssetStateDef);
                
                if(((AssetState)stateAttribute.Value) == AssetState.Active) {
                    UpdateOpenTest(newStatus, test);
                } else if(run.State == TestRun.TestRunState.Failed) {
                    DefectForClosedTest(run, test);
                }
            }
        }

        private void DefectForClosedTest(TestRun run, Asset test) {
            var newDescription = string.Format(
                "One or more acceptance tests failed at \"{0}\".<BR />{1}",
                run.Stamp, descriptionsuffix);

            if(ShouldCreateDefect(test)) {
                var defect = CreateRelatedDefect(newDescription, test);
                Central.Services.Save(defect, changeComment);
                Logger.Log(string.Format("Saving defect for test \"{0}\".", run.TestRef));
            }
        }

        private bool ShouldCreateDefect(Asset test) {
            if(RelatedDefectExists(test)) {
                return false;
            }

            switch(createDefect) {
                case CreateDefect.None:
                    return false;
                case CreateDefect.All:
                    return true;
                case CreateDefect.CurrentIteration:
                    return TimeboxIsCurrent(test);
            }

            return false;
        }

        private bool TimeboxIsCurrent(Asset test) {
            var timeboxOid = test.GetAttribute(TestTimeboxDef).Value as Oid;

            if((timeboxOid == null) || timeboxOid.IsNull) {
                return false;
            }

            var timeboxStateQuery = new Query(TimeboxType);
            timeboxStateQuery.Selection.Add(TimeboxAssetStateDef);
            var term = new FilterTerm(TimeboxType.GetAttributeDefinition("ID"));
            term.Equal(timeboxOid.Token);
            timeboxStateQuery.Filter = term;

            var result = Central.Services.Retrieve(timeboxStateQuery);

            if(result.Assets.Count == 0) {
                return false;
            }

            var timebox = result.Assets[0];

            return ((AssetState)timebox.GetAttribute(TimeboxAssetStateDef).Value == AssetState.Active);
        }

        private bool RelatedDefectExists(Asset test) {
            var primaryWorkitemQuery = new Query(PrimaryWorkitem);

            var affectedTerm = new FilterTerm(DefectAffectedWorkitemsDef);
            affectedTerm.Equal(test.GetAttribute(TestParentDef).Value);
            var statusTerm = new FilterTerm(DefectAssetStateDef);
            statusTerm.Equal(AssetState.Active);
            primaryWorkitemQuery.Filter = new AndFilterTerm(affectedTerm, statusTerm);

            return Central.Services.Retrieve(primaryWorkitemQuery).Assets.Count > 0;
        }


        private Asset CreateRelatedDefect(string newDescription, Asset test) {
            var defect = Central.Services.New(DefectType, Oid.Null);
            defect.AddAttributeValue(DefectAffectedWorkitemsDef, test.GetAttribute(TestParentDef).Value);
            defect.SetAttributeValue(DefectScopeDef, test.GetAttribute(TestScopeDef).Value);

            var parent = (Oid)test.GetAttribute(TestParentDef).Value;
            
            defect.SetAttributeValue(DefectType.GetAttributeDefinition("Name"),
                string.Format(
                    "{0} \"{1}\" has failing Acceptance Test(s)",
                    Central.Loc.Resolve(parent.AssetType.DisplayName),
                    test.GetAttribute(TestParentNumberDef).Value));

            defect.SetAttributeValue(DefectType.GetAttributeDefinition("Description"), newDescription);

            var timeboxOid = FindTimebox(test);

            if(!timeboxOid.IsNull) {
                defect.SetAttributeValue(DefectTimeboxDef, timeboxOid);
            }

            return defect;
        }

        private void UpdateOpenTest(Oid newStatus, Asset test) {
            var statusAttribute = test.GetAttribute(TestStatusDef);
            var statusOid = (Oid)statusAttribute.Value;

            if(newStatus != statusOid) {
                test.SetAttributeValue(TestStatusDef, newStatus);
                Central.Services.Save(test, changeComment);
                Logger.Log(string.Format("Updating status of Acceptance Test \"{0}\".", test.Oid.Token));
            }
        }

        private QueryResult GetRelatedTests(TestRun run) {
            var testQuery = new Query(TestType);
            testQuery.Selection.Add(TestStatusDef);
            testQuery.Selection.Add(TestNameDef);
            testQuery.Selection.Add(TestAssetStateDef);
            testQuery.Selection.Add(TestParentDef);
            testQuery.Selection.Add(TestScopeDef);
            testQuery.Selection.Add(TestParentNumberDef);
            testQuery.Selection.Add(TestScheduleDef);
            testQuery.Selection.Add(TestTimeboxDef);

            var term = new FilterTerm(TestReferenceDef);
            term.Equal(run.TestRef);
            testQuery.Filter = term;

            return Central.Services.Retrieve(testQuery);
        }

        private Oid FindTimebox(Asset test) {
            if(createDefect == CreateDefect.CurrentIteration) {
                return test.GetAttribute(TestTimeboxDef).Value as Oid;
            }

            return FindTimebox(test.GetAttribute(TestScheduleDef).Value as Oid);
        }

        private Oid FindTimebox(Oid scheduleOid) {
            if((scheduleOid == null) || (scheduleOid.IsNull)) {
                return Oid.Null;
            }

            var timebox = (Oid)timeboxes[scheduleOid];
            if(timebox == null) {
                timebox = Oid.Null;

                var q = new Query(TimeboxType);
                var scheduleTerm = new FilterTerm(TimeboxType.GetAttributeDefinition("Schedule"));
                scheduleTerm.Equal(scheduleOid);
                var assetStateTerm = new FilterTerm(TimeboxType.GetAttributeDefinition("AssetState"));
                assetStateTerm.Equal(AssetState.Active);
                q.Filter = new AndFilterTerm(scheduleTerm, assetStateTerm);
                q.OrderBy.MajorSort(TimeboxType.GetAttributeDefinition("EndDate"), OrderBy.Order.Ascending);
                q.Paging = new Paging(0, 1);

                var r = Central.Services.Retrieve(q);

                if(r.Assets.Count != 0) {
                    timebox = r.Assets[0].Oid;
                }

                timeboxes[scheduleOid] = timebox;
            }


            return timebox;
        }

        private enum CreateDefect {
            All,
            CurrentIteration,
            None
        }
    }
}