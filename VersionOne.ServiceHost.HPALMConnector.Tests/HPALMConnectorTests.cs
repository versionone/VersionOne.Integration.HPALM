using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}
