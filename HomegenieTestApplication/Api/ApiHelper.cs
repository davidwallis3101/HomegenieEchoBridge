using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace HomegenieTestApplication.Api
{
    class ApiHelper
    {
        public List<Module> GetModules(string host)
        {
            var url = $"http://{host}/api/HomeAutomation.HomeGenie/Config/Modules.List";
            return JsonConvert.DeserializeObject<List<Module>>(this.Get(url));
        }

        public List<Group> GetGroups(string host)
        {
            var url = $"http://{host}/api/HomeAutomation.HomeGenie/Config/Groups.List";
            return JsonConvert.DeserializeObject<List<Group>>(this.Get(url));
        }

        private string Get(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                }
                throw;
            }
        }
    }
}
