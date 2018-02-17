using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MIG.Interfaces.HomeAutomation.Api
{
    class HgApiHelper
    {
        private readonly string _hgApiEndpoint;
        private static readonly HttpClient HttpClient = new HttpClient();

        public HgApiHelper(string apiEndpoint)
        {
            _hgApiEndpoint = apiEndpoint;
        }

        public bool UpdateModule(Module module)
        {
            var url = $"http://{_hgApiEndpoint}/api/HomeAutomation.HomeGenie/Config/Modules.Update";
            Post(url, JsonConvert.SerializeObject(module));
            return true;
        }

        public List<Module> GetModules()
        {
            var url = $"http://{_hgApiEndpoint}/api/HomeAutomation.HomeGenie/Config/Modules.List";
            return JsonConvert.DeserializeObject<List<Module>>(Get(url));
        }

        public List<Group> GetGroups()
        {
            var url = $"http://{_hgApiEndpoint}/api/HomeAutomation.HomeGenie/Config/Groups.List";
            return JsonConvert.DeserializeObject<List<Group>>(Get(url));
        }

        private string Get(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                var response = request.GetResponse();
                using (var responseStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                var errorResponse = ex.Response;
                using (var responseStream = errorResponse.GetResponseStream())
                {
                    var reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    var errorText = reader.ReadToEnd();
                }
                throw;
            }
        }

        private static void Post(string url,string payload)
        {
            Task.Run(() => HttpClient.PostAsync(url, new StringContent(payload, Encoding.UTF8, "application/json")))
                .Wait();
        }
    }
}
