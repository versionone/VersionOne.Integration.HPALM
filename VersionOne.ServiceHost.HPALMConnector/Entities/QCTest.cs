using System;
using System.Linq;
using System.Xml.Linq;

namespace VersionOne.ServiceHost.HPALMConnector.Entities
{
    public class QCTest
    {
        private readonly XDocument _document;

        public QCTest(XDocument xDocument)
        {
            _document = xDocument;
        }

        public bool HaveBeenExecuted
        {
            get
            {
                var execStatusField = _document.Descendants("Field")
                    .First(f => f.Attribute("Name").Value.Equals("exec-status"))
                    .Value;

                return !execStatusField.Equals("No Run", StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}