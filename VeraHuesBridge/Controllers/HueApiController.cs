using System.Collections.Generic;
using System.Web.Http;
using System.Net.Http;
using NLog;
using System.Web.Http.Cors;
using EchoBridge.Devices;
using EchoBridge.Webserver;

namespace EchoBridge.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class HueApiController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public HttpResponseMessage Get(string userId)
        {
            Logger.Info("ApiController GET (api/userId) called with userid [{0}]", userId);
            var deviceResponseList = new Dictionary<string, DeviceResponse>();

            foreach (var device in Globals.DeviceList.List())
            {
                var newDr=DeviceResponse.CreateResponse(device.Name, device.Id);
                deviceResponseList.Add(device.Id, newDr);

            }

            var haResponse = new HueApiResponse {Lights = deviceResponseList};

            Logger.Info("ApiController GET (api/userId) returned HueApi device with [{0}] light(s)", deviceResponseList.Count);
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, haResponse);



        }
    }
}
