using System.Collections.Generic;
using VersionOne.ServiceHost.Core.Configuration;

namespace VersionOne.ServiceHost.QualityCenterServices {
    // TODO use ConfigurationReader or XML deserialization and store complete service configuration here
    public class QcConfiguration {
        public string Url;
        public string Username;
        public string Password;

        public readonly IDictionary<MappingInfo, MappingInfo> PriorityMappings = new Dictionary<MappingInfo, MappingInfo>();
    }
}