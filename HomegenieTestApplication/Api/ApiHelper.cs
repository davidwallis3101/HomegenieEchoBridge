using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace HomegenieTestApplication.Api
{
    internal class ApiHelper
    {

        public bool UpdateModule(string host, Module module)
        {
            var url = $"http://{host}/api/HomeAutomation.HomeGenie/Config/Modules.Update";
            Post(url, JsonConvert.SerializeObject(module));
            return true;
        }

        public List<Module> GetModules(string host)
        {
            var url = $"http://{host}/api/HomeAutomation.HomeGenie/Config/Modules.List";
            return JsonConvert.DeserializeObject<List<Module>>(Get(url));
        }

        public List<Group> GetGroups(string host)
        {
            var url = $"http://{host}/api/HomeAutomation.HomeGenie/Config/Groups.List";
            return JsonConvert.DeserializeObject<List<Group>>(Get(url));
        }

        private static string Get(string url)
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
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {

                streamWriter.Write(payload);
                streamWriter.Flush();
                //streamWriter.Close(); // should not call dispose more than once
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                Console.WriteLine(result);

            }
        }
    }
}
