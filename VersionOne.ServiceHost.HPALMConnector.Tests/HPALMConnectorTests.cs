using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VersionOne.ServiceHost.HPALMConnector.Entities;

namespace VersionOne.ServiceHost.HPALMConnector.Tests
{
    [TestClass]
    public class HPALMConnectorTests
    {
        [TestMethod]
        public void IsNotAuthenticated()
        {
            var connector = new HPALMConnector("http://hpalm115.cloudapp.net:8080/qcbin");

            Assert.IsFalse(connector.IsAuthenticated);
        }

        [TestMethod]
        public void IsAuthenticatedWhenLoggedIn()
        {
            var connector = new HPALMConnector("http://hpalm115.cloudapp.net:8080/qcbin");
            connector.Authenticate("admin", "admin");

            Assert.IsTrue(connector.IsAuthenticated);
        }

        [TestMethod]
        public void IsAuthenticatedWhenLoggedOut()
        {
            var connector = new HPALMConnector("http://hpalm115.cloudapp.net:8080/qcbin");
            connector.Authenticate("admin", "admin");

            Assert.IsTrue(connector.IsAuthenticated);

            connector.Logout();

            Assert.IsFalse(connector.IsAuthenticated);
        }

        [TestMethod]
        public void GetLatestTestRuns()
        {
            var connector = new HPALMConnector("http://hpalm115.cloudapp.net:8080/qcbin");
            connector.Authenticate("admin", "admin");

            var lastCheck = new DateTime(2015, 7, 30);
            var filterParam = "{last-modified[>=\"" + lastCheck.ToString("yyyy-MM-dd HH:mm:00") + "\"]; user-01[AT*]}";

            var doc = connector.Get("/qcbin/rest/domains/Default/projects/VersionOne/customization/entities?extended-mode=y&entity-types=test&query=" + filterParam);
            var testRuns = new List<HPALMTest>();
            doc.Descendants("Entity").ToList().ForEach(testEntity => testRuns.Add(HPALMTest.FromXDocument(testEntity.Document)));

            foreach (var testRun in testRuns)
            {
                var user01Field = testRun.CustomFields["user-01"];
                Assert.IsNotNull(user01Field);
                Assert.IsTrue(user01Field.StartsWith("AT"));
                Assert.IsTrue(testRun.LastModified >= lastCheck);
            }
        }

        [TestMethod]
        public void CreateTest()
        {
            var connector = new HPALMConnector("http://hpalm115.cloudapp.net:8080/qcbin");
            connector.Authenticate("admin", "admin");

            var test = new HPALMTest()
            {
                Name = "Test #1000",
                Description = "Test desc...",
                Status = "Imported",
                ParentId = "1002",
                Owner = "admin"
            };
            test.CustomFields.Add("user-01", "AT-99999");

            var resource = string.Format("/qcbin/rest/domains/{0}/projects/{1}/tests", "Default", "VersionOne");

            var createdTest = HPALMTest.FromXDocument(connector.Post(resource, test.GetXmlPayload()));

            Assert.IsNotNull(createdTest.Id);
        }

        [TestMethod]
        public void CreateDefectTest()
        {
            var connector = new HPALMConnector("http://hpalm115.cloudapp.net:8080/qcbin");
            connector.Authenticate("admin", "admin");

            var payload = XDocument.Parse("<Entity Type=\"defect\"></Entity>");
            var fields = new XElement("Fields");
            fields.Add(new XElement("Field", new XAttribute("Name", "name"), new XElement("Value", "A Test Defect " + Guid.NewGuid())));
            fields.Add(new XElement("Field", new XAttribute("Name", "status"), new XElement("Value", "New")));
            fields.Add(new XElement("Field", new XAttribute("Name", "owner"), new XElement("Value", "VersionOne")));
            fields.Add(new XElement("Field", new XAttribute("Name", "detected-by"), new XElement("Value", "VersionOne")));
            fields.Add(new XElement("Field", new XAttribute("Name", "creation-time"), new XElement("Value", DateTime.Now.ToString("yyyy-MM-dd HH:mm:00"))));
            fields.Add(new XElement("Field", new XAttribute("Name", "description"), new XElement("Value", "DESCRIPTION")));
            fields.Add(new XElement("Field", new XAttribute("Name", "severity"), new XElement("Value", "Low")));
            payload.Elements().First().Add(fields);

            var resource = string.Format("/qcbin/rest/domains/{0}/projects/{1}/defects", "Default", "VersionOne");
            var createdDefectDoc = connector.Post(resource, payload);
        }
    }
}
