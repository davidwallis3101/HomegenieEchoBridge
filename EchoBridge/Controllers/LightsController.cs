using System;
using System.Text;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using NLog;
using System.Web.Http.Cors;

namespace HGEchoBridge
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LightsController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // GET api/{userId}/lights
        public HttpResponseMessage Get()
        {
            logger.Info("LightsController called. (// GET api/{userId}/lights)...");
            string output = "{" ;

            foreach (Device d in Globals.DeviceList.List())
            {
                output += "\"" + d.id.ToString() + "\": \"" + d.name + "\",";
            }
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
            logger.Info("LightsController called (// GET  api/{userId}/lights/{id}) Retrieving light with id[" + id + "]...");
            Device device = Globals.DeviceList.FindById(id);
            if (device == null)
            {
                logger.Warn("LightsController GET: Could not locate a light with id [" + id + "].");
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    ReasonPhrase = "Could locate a device with that id."
                };
            }

            logger.Info("LightsController GET: Returned DeviceResponse for device named[{0}], with id [{1}]", device.name, device.id);
            DeviceResponse response = DeviceResponse.createResponse(device.name, device.id);
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, response);
        }


        // PUT api/{userId}/lights/5/ 
        public HttpResponseMessage Put(string id)
        {
            logger.Info("LightsController PUT called. Updating light with id{0}]...", id);
            var device = Globals.DeviceList.FindById(id);
            if (device == null)
            {
                logger.Warn("LightsController PUT: Could not locate a light with id [{0}].", id);
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    ReasonPhrase = "Could locate a device with that id."
                };
            }

            //the echo PUTs json data but does so in application/x-www-form-urlencoded instead of JSON
            //so we have to read the data from the body then convert it properly from a JSON object.
            var requestContent = Request.Content;
            var jsonContent = requestContent.ReadAsStringAsync().Result;
            logger.Info($"Alexa request: {jsonContent}");
            var deviceState=Newtonsoft.Json.JsonConvert.DeserializeObject<DeviceState>(jsonContent);

            var url = string.IsNullOrEmpty(device.DimUrl)
                ? deviceState.on
                    ? device.onUrl
                    : device.offUrl
                : device.DimUrl;

            url = ReplaceIntensityValue(url, deviceState);
            var body = ReplaceIntensityValue(device.contentBody, deviceState);
         
            if (Utilities.MakeHttpRequest(url, device.httpVerb, device.contentType, body))
            {

                logger.Info("LightsController PUT: Successfully updated state of device via HTTP request.");
                var responseString = "[{\"success\":{\"/lights/" + device.id + "/state/on\":" + deviceState.on.ToString().ToLower() + "}}]";
                return new HttpResponseMessage
                {
                    Content = new StringContent(responseString, Encoding.UTF8, "application/json"),
                    StatusCode = HttpStatusCode.OK
                };

            }
            else
            {
                logger.Warn("LightsController PUT: Failed to change state of device using HTTP request. URL:[{0}], Method: [{1}], ContentType: [{2}], Body: [{3}]", url, device.httpVerb, device.contentType, body);
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    ReasonPhrase = "Failed to change state of device."
                };
            }
        }


        private static string ReplaceIntensityValue(string request, DeviceState deviceState)
        {
            logger.Info($"Replacing IntensityValue for request \"{request}\"");
            /*  currently provides only two variables:
                        intensity.byte : 0-255 brightness.  this is raw from the echo
                        intensity.percent : 0-100, adjusted for the vera
            */
            var intensity = deviceState.bri;
            if (deviceState.on && intensity == 0)
            {
                logger.Info("DeviceState was on but brightness value was zero. Setting to default brightness.");
                intensity = Globals.DefaultIntensity; //205 is about 80% of 255
            }

            if (string.IsNullOrEmpty(request))
            {
                logger.Info("Empty request. No intensity value to replace.");
                return "";
            }
            if (request.Contains(Device.INTENSITY_BYTE))
            {
                logger.Info("Request contained [{0}] byte value. Replacing with intensity [{1}].", Device.INTENSITY_BYTE, intensity);
                request = request.Replace(Device.INTENSITY_BYTE, intensity.ToString());
            }
            else if (request.Contains(Device.INTENSITY_PERCENT))
            {
                var percentBrightness = (int)Math.Round(intensity / 255.0 * 100);
                logger.Info("Request contained [{0}] percent value. Scaling and Replacing with intensity [{1}].", Device.INTENSITY_PERCENT, percentBrightness);
                request = request.Replace(Device.INTENSITY_PERCENT, percentBrightness.ToString());
            }
            return request;
        }


    

    }
}
