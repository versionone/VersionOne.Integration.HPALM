using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace VersionOne.ServiceHost.HPALMConnector.Entities
{
    public class HPALMTest : IHPALMEntity
    {
        public string Id { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string ParentId { get; set; }
        public string Owner { get; set; }
        public string ExecutionStatus { get; private set; }

        public bool HaveBeenExecuted
        {
            get { return ExecutionStatus != null && !ExecutionStatus.Equals("No Run"); }
        }

        private readonly IDictionary<string, string> _customFields = new Dictionary<string, string>();

        public static HPALMTest FromXDocument(XDocument xDocument)
        {
            var instance = new HPALMTest();
            var fields = xDocument.Descendants("Field");
            instance.Name =
                fields.First(f => f.Attribute("Name").Value.Equals("name")).Value;
            instance.Description =
                fields.First(f => f.Attribute("Name").Value.Equals("description")).Value;
            instance.Status =
                fields.First(f => f.Attribute("Name").Value.Equals("status")).Value;
            instance.ExecutionStatus =
                fields.First(f => f.Attribute("Name").Value.Equals("exec-status")).Value;
            instance.ExecutionStatus =
                fields.First(f => f.Attribute("Name").Value.Equals("exec-status")).Value;
            instance.ParentId =
                fields.First(f => f.Attribute("Name").Value.Equals("parent-id")).Value;
            instance.Owner =
                fields.First(f => f.Attribute("Name").Value.Equals("owner")).Value;
            instance.Id =
                fields.First(f => f.Attribute("Name").Value.Equals("id")).Value;

            return instance;
        }

        public XDocument GetXmlPayload()
        {
            var payload = XDocument.Parse("<Entity Type=\"test\"></Entity>");

            var fields = new XElement("Fields");
            fields.Add(new XElement("Field", new XAttribute("Name", "name"), new XElement("Value", Name)));
            fields.Add(new XElement("Field", new XAttribute("Name", "description"), new XElement("Value", Description)));
            fields.Add(new XElement("Field", new XAttribute("Name", "status"), new XElement("Value", Status)));
            fields.Add(new XElement("Field", new XAttribute("Name", "parent-id"), new XElement("Value", ParentId)));
            fields.Add(new XElement("Field", new XAttribute("Name", "owner"), new XElement("Value", Owner)));
            fields.Add(new XElement("Field", new XAttribute("Name", "subtype-id"), new XElement("Value", "MANUAL")));

            foreach (var customField in CustomFields)
            {
                fields.Add(new XElement("Field", new XAttribute("Name", customField.Key),
                    new XElement("Value", customField.Value)));
            }

            if (payload.Root != null) payload.Root.Add(fields);

            return payload;
        }

        public IDictionary<string, string> CustomFields
        {
            get { return _customFields; }
        }
    }
}