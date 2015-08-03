using System;
using System.Collections.Generic;
using System.Linq;
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

            var doc = connector.Get("/qcbin/rest/domains/Default/projects/VersionOne/tests?query=" + filterParam);
            var testRuns = new List<QCTest>();
            doc.Descendants("Entity").ToList().ForEach(testEntity => testRuns.Add(new QCTest(new XDocument(testEntity))));

            foreach (var testRun in testRuns)
            {
                var x = testRun.HaveBeenExecuted;
            }
        }
    }
}
