/*(c) Copyright 2008, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Xml;
using NUnit.Framework;
using VersionOne.ServiceHost.Eventing;
using VersionOne.ServiceHost.Core.Logging;
using VersionOne.ServiceHost.QualityCenterServices;
using Rhino.Mocks;

namespace VersionOne.ServiceHost.QualityCenter.Tests
{
    #region Abstract Context Base Classes

    [TestFixture]
    public abstract class QualityCenterReaderContext
    {
        protected QualityCenterReaderUpdater qcReaderUpdater;
        protected MockRepository repo;
        protected IEventManager mockEventManager;

        protected ILogger loggerStub;

        [TestFixtureSetUp]
        public virtual void Context()
        {
            repo = new MockRepository();
            loggerStub = repo.Stub<ILogger>();
            CreateMockEventManager();
            qcReaderUpdater = new QualityCenterReaderUpdater(null);
            qcReaderUpdater.Initialize(GetConfigXml(), loggerStub);
        }

        protected virtual void CreateMockEventManager()
        {
            mockEventManager = repo.StrictMock<IEventManager>();
        }

        protected virtual XmlElement GetConfigXml()
        {
            const string xml = @"<QualityCenterHostedService class=""VersionOne.ServiceHost.QualityCenterServices.QualityCenterHostedService, VersionOne.ServiceHost.QualityCenterServices"">
			<Connection>
				<ApplicationUrl>http://hpmercury/qcbin/</ApplicationUrl>
				<Username>alex_qc</Username>
				<Password></Password>
			</Connection>
			<QCProjects>
				<Project id=""CallCenter"">
					<Domain>Development</Domain>
					<Project>UnitTest</Project>
					<TestFolder>VersionOne</TestFolder>
					<TestStatus>Imported</TestStatus>
					<VersionOneIdField>TS_USER_01</VersionOneIdField>
					<VersionOneProject>Call Center</VersionOneProject>
				</Project>
			</QCProjects>

			<!-- Quality Center Search Criteria to find Defects to move into VersionOne -->
			<DefectFilters>
				<!-- User the Defect is Assigned To-->
				<DefectFilter>
					<FieldName>BG_RESPONSIBLE</FieldName>
					<FieldValue>VersionOne</FieldValue>
				</DefectFilter>
				
				<!-- Status of the Defect -->
				<DefectFilter>
					<FieldName>BG_STATUS</FieldName>
					<FieldValue>New OR Reopen</FieldValue>
				</DefectFilter>
			</DefectFilters>

			<!-- Quality Center change after a Defect is created in VersionOne -->
			<CreateStatusValue>Open</CreateStatusValue>

			<!-- Quality Center change after a VersionOne Defect is closed -->
			<CloseStatusValue>Fixed</CloseStatusValue>

			<!-- VersionOne 'Source' value to use when Defect was created from Quality Center artifact -->
			<SourceFieldValue>Quality Center</SourceFieldValue>
		</QualityCenterHostedService>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc.DocumentElement;
        }
    }

	public abstract class QualityCenterQueryContext : QualityCenterReaderContext
    {
        public override void Context()
        {
            base.Context();
            SetLastCheckDateTime();
        }

        protected DateTime lastCheck;
        protected abstract void SetLastCheckDateTime();

    }

    #endregion

	public class when_quality_center_reader_updater_is_initialized_but_not_logged_in : QualityCenterReaderContext {

		[Test]
		public void should_support_qc_call_center_project() {
			Assert.IsTrue(qcReaderUpdater.HandlesQCProject("Default", "CallCenter"));
		}

		[Test]
		public void should_handle_v1_call_center_project() {
			Assert.IsTrue(qcReaderUpdater.HandlesV1Project("Call Center"));
		}
	}
}