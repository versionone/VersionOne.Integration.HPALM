using System;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace VersionOne.ServiceHost.HPALMConnector
{
    public class HPALMConnector : IDisposable
    {
        private readonly HttpClient _client;
        private readonly HttpClientHandler _handler;

        private HPALMConnector() { }

        public HPALMConnector(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url");

            _handler = new HttpClientHandler();
            _client = new HttpClient(_handler) {BaseAddress = new Uri(url)};
        }

        #region AUTHENTICATION

        public bool IsAuthenticated
        {
            get
            {
                var respMessage = GetData("/qcbin/rest/is-authenticated");

                return respMessage.IsSuccessStatusCode;
            }
        }

        public bool Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException("username");
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException("password");

            var resp = SendData("/qcbin/authentication-point/alm-authenticate", HttpMethod.Post, CreateAlmAuthenticationPayload(username, password));

            return resp.IsSuccessStatusCode;
        }

        public void Logout()
        {
            GetData("/qcbin/authentication-point/logout");
        }

        #endregion

        #region HTTP VERBS

        public XDocument Get(string resource)
        {
            return XDocument.Parse(GetData(resource).Content.ReadAsStringAsync().Result);
        }

        public XDocument Post(string resource, object data = null)
        {
            return XDocument.Parse(SendData(resource, HttpMethod.Post, data).Content.ReadAsStringAsync().Result);
        }

        public XDocument Put(string resource, object data = null)
        {
            return XDocument.Parse(SendData(resource, HttpMethod.Put, data).Content.ReadAsStringAsync().Result);
        }

        #endregion

        private HttpResponseMessage GetData(string resource)
        {
            var reqMessage = new HttpRequestMessage(HttpMethod.Get, resource);
            var respMessage = _client.SendAsync(reqMessage).Result;

            return respMessage;
        }

        private HttpResponseMessage SendData(string resource, HttpMethod httpMethod, object data = null)
        {
            var reqMessage = new HttpRequestMessage(httpMethod, resource);
            if (data != null)
            {
                if (data.GetType() == typeof(XDocument))
                    reqMessage.Content = new StringContent(data.ToString(), Encoding.UTF8, "application/xml");
                if (data.GetType() == typeof(byte[]))
                    reqMessage.Content = new ByteArrayContent((byte[]) data);
            }
            
            var respMessage = _client.SendAsync(reqMessage).Result;

            return respMessage;
        }

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