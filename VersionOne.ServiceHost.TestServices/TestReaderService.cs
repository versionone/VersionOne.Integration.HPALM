using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Ninject;
using VersionOne.Profile;
using VersionOne.SDK.APIClient;
using VersionOne.ServiceHost.Core.Configuration;
using VersionOne.ServiceHost.Core.Logging;
using VersionOne.ServiceHost.Core.Services;
using VersionOne.ServiceHost.Eventing;

namespace VersionOne.ServiceHost.TestServices
{
    /**
	 * This class is responsible for reading Tests from VersionOne based on a 
	 * filter defined in the configuration.  For each Test asset a "V1TestReady"
	 * message is published to the ServiceHost EventManager.
	 */

    public class TestReaderService : IHostedService
    {
        private readonly IList<V1ProjectMap> projectMaps = new List<V1ProjectMap>();
        protected IEventManager EventManager;
        private IServices services;
        protected ILogger Logger;

        public class IntervalSync { }

        public void Initialize(XmlElement config, IEventManager eventManager, IProfile profile)
        {
            EventManager = eventManager;
            Logger = new Logger(eventManager);

            var settings = VersionOneSettings.FromXmlElement(config["Settings"]);

            var connector = V1Connector
                .WithInstanceUrl(settings.Url)
                .WithUserAgentHeader("VersionOne.ServiceHost.TestServices", Assembly.GetEntryAssembly().GetName().Version.ToString());

            ICanSetProxyOrEndpointOrGetConnector connectorWithAuth;
            switch (settings.AuthenticationType)
            {
                case AuthenticationTypes.AccessToken:
                    connectorWithAuth = connector.WithAccessToken(settings.AccessToken);
                    break;
                case AuthenticationTypes.Basic:
                    connectorWithAuth = connector.WithUsernameAndPassword(settings.Username, settings.Password);
                    break;
                case AuthenticationTypes.Integrated:
                    connectorWithAuth = connector.WithWindowsIntegrated();
                    break;
                case AuthenticationTypes.IntegratedWithCredentials:
                    connectorWithAuth = connector.WithWindowsIntegrated(settings.Username, settings.Password);
                    break;
                default:
                    throw new Exception("Invalid authentication type");
            }

            if (settings.ProxySettings.Enabled)
                connectorWithAuth.WithProxy(
                    new ProxyProvider(
                        new Uri(settings.ProxySettings.Url), settings.ProxySettings.Username, settings.ProxySettings.Password, settings.ProxySettings.Domain));

            services = new Services(connectorWithAuth.Build());

            LoadProjectMap(config["TestPublishProjectMap"], config["BaseQueryFilter"]);

            EventManager.Subscribe(typeof(IntervalSync), OnInterval);
        }

        public void Start()
        {
            // TODO move subscriptions to timer events, etc. here
        }

        /**
		 * Load nodes defined the <TestPublishProjectMap> element
		 */

        private void LoadProjectMap(XmlNode mapElement, XmlNode filterElement)
        {
            var baseFilter = filterElement.InnerText;
            var v1ProjectNodes = mapElement.SelectNodes("V1Project");

            if (v1ProjectNodes != null)
            {
                foreach (XmlNode oneNode in v1ProjectNodes)
                {
                    var v1ProjectMap = new V1ProjectMap(oneNode);
                    Logger.Log(LogMessage.SeverityType.Debug,
                        string.Format("Maping V1Project {0} to HP-ALM Project {1}", v1ProjectMap.ProjectName, v1ProjectMap.DestinationProject));
                    SetQueryOnNode(v1ProjectMap, baseFilter);
                    Logger.Log(LogMessage.SeverityType.Debug, string.Format("\tQuery is {0}", v1ProjectMap.Query.Filter.Token));
                    projectMaps.Add(v1ProjectMap);
                }
            }
        }

        /**
		 * Set the query on each V1ProjectMap
		 */

        private void SetQueryOnNode(V1ProjectMap projectNode, string baseFilter)
        {
            var filter = projectNode.IncludeChildren
                ? string.Format("{0};Scope.ParentMeAndUp.Name='{1}'", baseFilter, projectNode.ProjectName)
                : string.Format("{0};Scope.Name='{1}'", baseFilter, projectNode.ProjectName);
            projectNode.Query = new Query(TestType);
            projectNode.Query.Selection.Add(TestNameDef);
            projectNode.Query.Selection.Add(TestDescriptionDef);
            projectNode.Query.Selection.Add(TestProjectDef);
            projectNode.Query.Selection.Add(TestDisplayIdDef);
            projectNode.Query.Filter = new QueryFilterTerm(filter);
        }

        private void OnInterval(object pubobj)
        {
            Logger.Log(LogMessage.SeverityType.Debug, string.Format("ProjectMap length is {0}", projectMaps.Count));

            foreach (var projectMap in projectMaps)
            {
                var result = services.Retrieve(projectMap.Query);
                Logger.Log(string.Format("Found {0} Test to send to HP-ALM in VersionOne Project \"{1}\"", result.TotalAvaliable, projectMap.ProjectName));

                foreach (var test in result.Assets)
                {
                    var displayName = GetAttributeValue(test, TestDisplayIdDef);
                    Logger.Log(LogMessage.SeverityType.Debug, string.Format("\tPublishing Test {0}", displayName));
                    EventManager.Publish(new V1TestReady(test.Oid.Momentless.ToString(),
                        displayName,
                        GetAttributeValue(test, TestNameDef),
                        GetAttributeValue(test, TestDescriptionDef),
                        projectMap.DestinationProject));
                }
            }
        }

        private IAssetType TestType
        {
            get { return services.Meta.GetAssetType("Test"); }
        }

        private IAttributeDefinition TestNameDef
        {
            get { return TestType.GetAttributeDefinition("Name"); }
        }

        private IAttributeDefinition TestDescriptionDef
        {
            get { return TestType.GetAttributeDefinition("Description"); }
        }

        private IAttributeDefinition TestProjectDef
        {
            get { return TestType.GetAttributeDefinition("Scope.Name"); }
        }

        private IAttributeDefinition TestDisplayIdDef
        {
            get { return TestType.GetAttributeDefinition("Number"); }
        }

        private static string GetAttributeValue(Asset asset, IAttributeDefinition attributeDef)
        {
            var value = asset.GetAttribute(attributeDef).Value;
            return (null != value) ? value.ToString() : "";
        }
    }

    /**
	 * Maps V1 projects to destination projects
	 */
    internal class V1ProjectMap
    {
        private readonly string destinationProject;
        private readonly bool includeChildren;
        private readonly string name;

        public V1ProjectMap(XmlNode node)
        {
            name = node.Attributes["Name"].InnerText;
            includeChildren = (node.Attributes["IncludeChildren"].InnerText.Contains("Y")) ? true : false;
            destinationProject = node.InnerText;
        }

        public string ProjectName
        {
            get { return name; }
        }

        public bool IncludeChildren
        {
            get { return includeChildren; }
        }

        public string DestinationProject
        {
            get { return destinationProject; }
        }

        public Query Query { get; set; }
    }

    /**
	 * This class holds the filter terms
	 */

    internal class QueryFilterTerm : IFilterTerm
    {
        private readonly string token;

        public QueryFilterTerm(string token)
        {
            this.token = token;
        }

        public string Token
        {
            get { return token; }
        }

        // TODO fix this if anyone really needs ShortToken
        public string ShortToken
        {
            get { return token; }
        }
    }
}