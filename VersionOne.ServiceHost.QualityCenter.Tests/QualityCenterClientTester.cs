using System;
using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;
using Rhino.Mocks;
using TDAPIOLELib;
using VersionOne.ServiceHost.Eventing;
using VersionOne.ServiceHost.QualityCenterServices;
using IList = System.Collections.IList;
using VersionOne.ServiceHost.Core.Logging;

namespace VersionOne.ServiceHost.QualityCenter.Tests {
	[TestFixture]
	public abstract class QualityCenterClientContext {
		private readonly IDictionary<string, QualityCenterClient> _projects = new Dictionary<string, QualityCenterClient>();
		protected MockRepository repo;
		protected IEventManager mockEventManager;
		
        private ILogger logger;

		[TestFixtureSetUp]
		public virtual void Context() {
			repo = new MockRepository();
            logger = repo.Stub<ILogger>();
			CreateMockEventManager();

			XmlElement config = GetConfigXml();
			foreach (XmlNode node in config["QCProjects"].SelectNodes("Project")) {
				QCProject oneProject = new QCProject(node);
				_projects.Add(node.Attributes["id"].InnerText, new QualityCenterClient(oneProject, config, logger));
			}
		}

		[TestFixtureTearDown]
		public virtual void Teardown()
		{
			foreach (KeyValuePair<string, QualityCenterClient> pair in _projects)
			{
				pair.Value.Logout();
				pair.Value.Dispose();
			}
		}

		protected virtual void CreateMockEventManager() {
			mockEventManager = (IEventManager)repo.StrictMock(typeof(IEventManager));
		}

		protected virtual XmlElement GetConfigXml() {
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
	
		protected QualityCenterClient CallCenterConnection
		{
			get
			{
				return _projects["CallCenter"];
			}
		}
	}

	public abstract class QualityCenterClientLoggedInContext : QualityCenterClientContext {
		public override void Context() {
			base.Context();
			CallCenterConnection.Login();
		}
	}

	public abstract class QualityCenterClientProjectConnectedContext : QualityCenterClientLoggedInContext {
		public override void Context() {
			base.Context();
			CallCenterConnection.ConnectToProject();
		}
	}

	public class when_quality_center_connection_is_initialized_but_not_logged_in : QualityCenterClientContext {

		[Test]
		public void should_be_connected() {
			Assert.IsTrue(CallCenterConnection.IsConnected);
		}

		[Test]
		public void should_not_be_logged_in() {
			Assert.IsFalse(CallCenterConnection.IsLoggedIn);
		}

		[Test]
		public void should_not_be_connected_to_the_project() {
			Assert.IsFalse(CallCenterConnection.IsProjectConnected);
		}
	}

	public class after_calling_login_on_quality_center_connection : QualityCenterClientLoggedInContext {

		[Test]
		public void should_be_connected() {
			Assert.IsTrue(CallCenterConnection.IsConnected);
		}

		[Test]
		public void should_be_logged_in() {
			Assert.IsTrue(CallCenterConnection.IsLoggedIn);
		}

		[Test]
		public void should_not_be_connected_to_project() {
			Assert.IsFalse(CallCenterConnection.IsProjectConnected);
		}
	}

	public class after_calling_connect_to_project_on_quality_center_connection : QualityCenterClientProjectConnectedContext
	{
		[Test]
		public void should_be_connected() {
			Assert.IsTrue(CallCenterConnection.IsConnected);
		}

		[Test]
		public void should_be_logged_in() {
			Assert.IsTrue(CallCenterConnection.IsLoggedIn);
		}

		[Test]
		public void should_be_connected_to_project() {
			Assert.IsTrue(CallCenterConnection.IsProjectConnected);
		}		
	}

	public class after_calling_logout_on_quality_center_connection : QualityCenterClientProjectConnectedContext {

		public override void Context() {
			base.Context();
			CallCenterConnection.Logout();
		}

		[Test]
		public void should_be_connected() {
			Assert.IsTrue(CallCenterConnection.IsConnected);
		}

		[Test]
		public void should_not_be_logged_in() {
			Assert.IsFalse(CallCenterConnection.IsLoggedIn);
		}

		[Test]
		public void should_not_be_project_connected() {
			Assert.IsFalse(CallCenterConnection.IsProjectConnected);
		}
	}

	public class when_creating_a_test_in_quality_center : QualityCenterClientContext
	{
		private int _beforeCreateTestCount;

		public override void Context() {
			base.Context();
			_beforeCreateTestCount = CallCenterConnection.GetTestCount();
			string testTitle = string.Format("Unit Test {0}", DateTime.Now.ToString("yyyyMMdd-HHmmss"));
			CallCenterConnection.CreateQCTest(testTitle, "This is the test description", "AT-00001");
		}

		[Test]
		public void should_increments_test_count_in_quality_center()
		{
			int afterCreateCount = CallCenterConnection.GetTestCount();
			Assert.AreEqual(_beforeCreateTestCount + 1, afterCreateCount);
		}		
	}

	/**
	 * this test creates a defect in QC, then updates that defect from pretend data that it received from VersionOne
	 */
	public class when_updating_a_defect_after_creation_in_v1 : QualityCenterClientContext {
		#region Attributes necessary for this test
		private Bug _qcBug;
		private string beforeStatus;
		private string beforeComments;
		private int beforeAttachmentCount;
		#endregion

		public override void Context()
		{
			base.Context();
			_qcBug = CallCenterConnection.CreateQCDefect();
			beforeStatus = _qcBug.Status;
			beforeComments = (string)_qcBug["BG_DEV_COMMENTS"] + "";
			beforeAttachmentCount = ((AttachmentFactory)_qcBug.Attachments).NewList("").Count;

			List<string> comments = new List<string>();
			comments.Add("We pretended to create this defect in VersionOne");
			comments.Add("This is the data returned by the V1 server");

			CallCenterConnection.OnDefectCreated(QualityCenterClient.DefectID(_qcBug), comments, "http://localhost/versionone/assetdetail.v1?oid=Defect%3a1410");
			_qcBug.Refresh();
		}

		[Test]
		public void should_change_status() {
			string afterStatus = _qcBug.Status;
			Assert.AreNotEqual(beforeStatus, afterStatus);
		}

		[Test]
		public void should_add_comments() {
			string afterComments = (string)_qcBug["BG_DEV_COMMENTS"] + "";
			Assert.Greater(afterComments.Length, beforeComments.Length);
		}

		[Test]
		public void should_add_link() {
			int afterAttachmentCount = ((AttachmentFactory)_qcBug.Attachments).NewList("").Count;
			Assert.Greater(afterAttachmentCount, beforeAttachmentCount);
		}
	}

	/**
	 * this test creates a defect in QC, then pretends it was closed in VersionOne
	 */
	public class when_updating_a_defect_after_it_is_closed_in_v1 : QualityCenterClientContext {
		#region Attributes necessary for this test
		private Bug _qcBug;
		private string beforeStatus;
		private string beforeComments;
		#endregion

		public override void Context() {
			base.Context();
			_qcBug = CallCenterConnection.CreateQCDefect();
			beforeStatus = _qcBug.Status;
			beforeComments = (string)_qcBug["BG_DEV_COMMENTS"] + "";

			List<string> comments = new List<string>();
			comments.Add("We pretended to close this defect in VersionOne");

			CallCenterConnection.OnDefectStateChange(QualityCenterClient.DefectID(_qcBug), comments);
			_qcBug.Refresh();
		}

		[Test]
		public void should_change_status() {
			string afterStatus = _qcBug.Status;
			Assert.AreNotEqual(beforeStatus, afterStatus);
		}

		[Test]
		public void should_add_comments() {
			string afterComments = (string)_qcBug["BG_DEV_COMMENTS"] + "";
			Assert.Greater(afterComments.Length, beforeComments.Length);
		}
	}

	public class when_checking_for_test_updates : QualityCenterClientContext
	{
		[Test]
		public void should_find_some_changes()
		{
			DateTime lastCheck = new DateTime(2009, 07, 23, 10, 20, 00);
			IList results = CallCenterConnection.GetLatestTestRuns(lastCheck);
			Assert.AreNotEqual(0, results.Count);
		}
	}

	public class when_checking_for_new_defects : QualityCenterClientContext {
		[Test]
		public void should_find_some_changes() {
			DateTime lastCheck = new DateTime(2009, 07, 23, 10, 20, 00);
			IList results = CallCenterConnection.GetLatestDefects(lastCheck);
			Assert.AreNotEqual(0, results.Count);
		}
	}
}