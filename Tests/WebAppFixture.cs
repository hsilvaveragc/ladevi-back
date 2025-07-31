using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LadeviVentasApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Tests
{
    public abstract class WebAppFixture : IDisposable
    {
        public CustomWebApplicationFactory<Startup> Factory { get; set; }
        public HttpClient Client { get; }
        public LinkGenerator LinkGenerator { get; }
        public string CurrentUser { get; set; }
        public Dictionary<string, string> Tokens = new Dictionary<string, string>();

        protected WebAppFixture()
        {
            Factory = new CustomWebApplicationFactory<Startup>();
            Client = Factory.CreateClient();
            LinkGenerator = ((LinkGenerator)Factory.Server.Host.Services.GetService(
                typeof(LinkGenerator)));
        }

        public void Dispose()
        {
            Factory.Dispose();
        }

        public async Task<JObject> Send<TControllerType>(string action, bool shouldSucceed = true,
            object routeValues = null, object bodyData = null, HttpMethod method = null)
            where TControllerType : ControllerBase
        {
            method = method ?? HttpMethod.Post;
            var url = LinkGenerator.GetPathByAction(action, typeof(TControllerType).Name.Replace("Controller", ""), routeValues);
            var m = new HttpRequestMessage(method, url);
            if (bodyData != null)
            {
                m.Content = new StringContent(JsonConvert.SerializeObject(bodyData, SerializerSettings()), Encoding.UTF8, "application/json");
            }
            if (!string.IsNullOrWhiteSpace(CurrentUser))
            {
                m.Headers.Add("Authorization", $"Bearer {Tokens[CurrentUser]}");
            }
            var httpResponse = await Client.SendAsync(m);
            var json = await httpResponse.Content.ReadAsStringAsync();
            if (shouldSucceed)
            {
                Debug.WriteLine(json);
                httpResponse.EnsureSuccessStatusCode();
            }
            else
            {
                var statusCode = (int)httpResponse.StatusCode;
                if (statusCode >= 200 && statusCode < 300)
                {
                    throw new InvalidOperationException("Should not be success!");
                }
            }
            return JObject.Parse(json);
        }

        public async Task<T> Send<T, TControllerType>(string action, bool shouldSucceed = true,
            object routeValues = null, object bodyData = null, HttpMethod method = null) where TControllerType : ControllerBase
        {
            var jobject = await Send<TControllerType>(action, shouldSucceed, routeValues, bodyData, method);
            return jobject.ToObject<T>(JsonSerializer.Create(SerializerSettings()));
        }

        private static JsonSerializerSettings SerializerSettings()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }
    }
}