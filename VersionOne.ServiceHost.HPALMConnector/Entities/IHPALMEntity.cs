using System.Collections.Generic;
using System.Xml.Linq;

namespace VersionOne.ServiceHost.HPALMConnector.Entities
{
    public interface IHPALMEntity
    {
        XDocument GetXmlPayload();

        IDictionary<string, string> CustomFields { get; }
    }
}