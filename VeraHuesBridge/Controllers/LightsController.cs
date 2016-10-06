using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using NLog;
using VeraHuesBridge.Devices;
using VeraHuesBridge.Webserver;

namespace VeraHuesBridge.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LightsController : ApiController
    {
        private static readonly string IntensityPercent = "${intensity.percent}";
        private static readonly string IntensityByte = "${intensity.byte}";
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        // GET api/{userId}/lights
        public HttpResponseMessage Get()
        {
            Logger.Info("LightsController called. (// GET api/{userId}/lights)...");
            var output = Globals.DeviceList.List().Aggregate("{", (current, d) => current + ("\"" + d.Id.ToString() + "\": \"" + d.Name + "\","));

            //remove trailing comma
            if (output.Length > 1) output = output.Substring(0, output.Length - 1);

            output += "}";

            
            return new HttpResponseMessage()
            {
                Content = new StringContent(output, Encoding.UTF8, "application/json")
            };

        }

        // GET  api/{userId}/lights/5 
        public HttpResponseMessage Get(string id)
        {
            Logger.Info("LightsController called (// GET  api/{userId}/lights/{id}) Retrieving light with id[" + id + "]...");
            var device = Globals.DeviceList.FindById(id);
            if (device == null)
            {
                Logger.Warn("LightsController GET: Could not locate a light with id [" + id + "].");
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    ReasonPhrase = "Could locate a device with that id."
                };
            }

            Logger.Info("LightsController GET: Returned DeviceResponse for device named[{0}], with id [{1}]", device.Name, device.Id);
            var response = DeviceResponse.CreateResponse(device.Name, device.Id);
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, response);
            

        }


        // PUT api/{userId}/lights/5/ 
        public HttpResponseMessage Put(string id)
        {
            Logger.Info("LightsController PUT called. Updating light with id{0}]...", id);
            var device = Globals.DeviceList.FindById(id);
            if (device == null)
            {
                Logger.Warn("LightsController PUT: Could not locate a light with id [{0}].", id);
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    ReasonPhrase = "Could locate a device with that id."
                };
            }
            //the echo PUTs json data but does so in application/x-www-form-urlencoded instead of JSON
            //so we have to read the data from the body then convert it properly from a JSON object.
            var requestContent = Request.Content;
            var jsonContent = requestContent.ReadAsStringAsync().Result;
            var deviceState=Newtonsoft.Json.JsonConvert.DeserializeObject<DeviceState>(jsonContent);

            
            var url = deviceState.On ? device.OnUrl: device.OffUrl;
            url = ReplaceIntensityValue(url, deviceState);

            var body = ReplaceIntensityValue(device.ContentBody, deviceState);
         
            if (Utilities.MakeHttpRequest(url, device.HttpVerb, device.ContentType, body))
            {

                Logger.Info("LightsController PUT: Successfully updated state of device via HTTP request.");
                var responseString = "[{\"success\":{\"/lights/" + device.Id + "/state/on\":" + deviceState.On.ToString().ToLower() + "}}]";
                return new HttpResponseMessage
                {
                    Content = new StringContent(responseString, Encoding.UTF8, "application/json"),
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            }

            Logger.Warn("LightsController PUT: Failed to change state of device using HTTP request. URL:[{0}], Method: [{1}], ContentType: [{2}], Body: [{3}]", url, device.HttpVerb, device.ContentType, body);
            return new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.NotFound,
                ReasonPhrase = "Failed to change state of device."
            };
        }


        protected string ReplaceIntensityValue(string request, DeviceState deviceState)
        {
            Logger.Info("Replacing IntensityValue...");
            /*  currently provides only two variables:
                        intensity.byte : 0-255 brightness.  this is raw from the echo
                        intensity.percent : 0-100
            */
            var intensity = deviceState.Bri;
            if (deviceState.On && intensity == 0)
            {
                Logger.Info("DeviceState was on but brightness value was zero. Setting to default brightness.");
                intensity = Globals.DefaultIntensity; //205 is about 80% of 255
            }
            
            if (string.IsNullOrEmpty(request))
            {
                Logger.Info("Empty request. No intensity value to replace.");
                return "";
            }

            if (request.Contains(IntensityByte))
            {
                Logger.Info("Request contained [{0}] byte value. Replacing with intensity [{1}].", IntensityByte, intensity);
                request = request.Replace(IntensityByte, intensity.ToString());
            }
            else if (request.Contains(IntensityPercent))
            {
                var percentBrightness = (int)Math.Round(intensity / 255.0 * 100);
                Logger.Info("Request contained [{0}] percent value. Scaling and Replacing with intensity [{1}].", IntensityPercent, percentBrightness);
                request = request.Replace(IntensityPercent, percentBrightness.ToString());
            }
            return request;
        }


    

    }
}
