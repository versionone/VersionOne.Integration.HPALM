using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;

namespace VersionOne.ServiceHost.HPALMConnector
{
    public class HPALMConnector : IDisposable
    {
        //private readonly string _url;
        //private readonly string _userName;
        //private readonly string _password;
        private readonly HttpClient _client;
        private HttpClientHandler _handler;

        private HPALMConnector() { }

        public HPALMConnector(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url");

            _handler = new HttpClientHandler();
            _client = new HttpClient(_handler) {BaseAddress = new Uri(url)};
        }

        public bool IsAuthenticated
        {
            get
            {
                var respMessage = SendGet("/qcbin/rest/is-authenticated");

                return respMessage.IsSuccessStatusCode;
            }
        }

        public void Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException("username");
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException("password");

            SendPost("/qcbin/authentication-point/alm-authenticate", CreateAlmAuthenticationPayload(username, password));
        }

        public void Logout()
        {
            SendPost("/qcbin/authentication-point/logout");
        }

        #region HTTP VERBS

        public XDocument Get(string resource)
        {
            return XDocument.Parse(SendGet(resource).Content.ReadAsStringAsync().Result);
        }

        public XDocument Post(string resource, XDocument data = null)
        {
            return XDocument.Parse(SendPost(resource, data).Content.ReadAsStringAsync().Result);
        }


        private HttpResponseMessage SendGet(string resource)
        {
            var reqMessage = new HttpRequestMessage(HttpMethod.Get, resource);
            var respMessage = _client.SendAsync(reqMessage).Result;

            return respMessage;
        }

        private HttpResponseMessage SendPost(string resource, XDocument data = null)
        {
            var reqMessage = new HttpRequestMessage(HttpMethod.Post, resource);
            if (data != null)
            {
                reqMessage.Content = new StringContent(data.ToString(), Encoding.UTF8, "application/xml");
            }
            
            var respMessage = _client.SendAsync(reqMessage).Result;

            return respMessage;
        }
        #endregion

        private XDocument CreateAlmAuthenticationPayload(string username, string password)
        {
           return new XDocument(new XElement("alm-authentication", new XElement("user", username),
                    new XElement("password", password)));
        }

        public void Dispose()
        {
            _client.Dispose();
            _handler.Dispose();
        }
    }
}