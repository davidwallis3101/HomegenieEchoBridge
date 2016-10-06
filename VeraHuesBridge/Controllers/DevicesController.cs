using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using NLog;
using VeraHuesBridge.Devices;
using VeraHuesBridge.Webserver;

namespace VeraHuesBridge.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DevicesController: ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //GET /api/devices/{idString}
        public HttpResponseMessage Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Logger.Warn("DeviceController Get called but Id was not supplied.");
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    ReasonPhrase = "Did not specify a device id."
                };
            }

            var device = Globals.DeviceList.FindById(id);
            if (device == null)
            {
                Logger.Warn("DeviceController Get called but Id did not match existing device.");
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    ReasonPhrase = "Could locate a device with that id." 
                };
            }
            else
            {
                Logger.Info("DeviceController Get returned device with name [{0}] for id[{1}].", device.Name, device.Id);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, device);
            }

        }

        //GET /api/devices
        public HttpResponseMessage Get()
        {
            Logger.Info("DeviceController called (//GET /api/devices)");
            var container = new DeviceContainer(Globals.DeviceList.List());
            var listContainer = new List<DeviceContainer> {container};
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, listContainer);            
        }

        // POST api/devices
        //public HttpResponseMessage Post([FromBody]Device newDevice)
        //{
        //    Logger.Info("DeviceController POST called (//POST /api/devices)...");
        //    if (newDevice==null)
        //    {
        //        Logger.Warn("DeviceController POST: ContentBody did not contain a device.");
        //        return new HttpResponseMessage()
        //        {
        //             StatusCode= System.Net.HttpStatusCode.BadRequest,
        //             ReasonPhrase="ContentBody did not contain a device"
                    
        //        };
        //    }
                
        //    //throw an error if they are posting the same device 
        //    Device device=null;
        //    if (!string.IsNullOrEmpty(newDevice.Id)) device =Globals.DeviceList.FindById(newDevice.Id);

        //    if (device!=null || Globals.DeviceList.Contains(newDevice))
        //    {
        //        Logger.Warn("DeviceController POST: Device already exists. Use PUT to update.");
        //        return new HttpResponseMessage()
        //        {
        //             StatusCode= System.Net.HttpStatusCode.BadRequest,
        //             ReasonPhrase="Device already exists. Use PUT to update."
        //        };
        //    }


        //    Globals.DeviceList.Add(newDevice);
        //    Logger.Info("Device created.");
        //    return new HttpResponseMessage()
        //    {
        //        StatusCode = System.Net.HttpStatusCode.OK
        //    };

            
        //}

        //// PUT api/devices
        //public HttpResponseMessage Put([FromBody]Device updatedDevice)
        //{
        //    // expecting to see something like (although may come without an id):
        //    // {"id":"26a09317-0605-4ef5-b749-b53f78fde428","name":"Test","deviceType":"switch","offUrl":"http://www.google.com","onUrl":"http://www.yahoo.com","httpVerb":null,"contentType":null,"contentBody":null}
        //    Logger.Info("DeviceController PUT called (//PUT /api/devices)...");
        //    if (updatedDevice == null)
        //    {
        //        Logger.Warn("DeviceController PUT: ContentBody did not contain a Device.");
        //        return new HttpResponseMessage()
        //        {
        //            StatusCode = System.Net.HttpStatusCode.BadRequest,
        //            ReasonPhrase = "ContentBody did not contain a Device"

        //        };
        //    }



        //    if (Globals.DeviceList.Update(updatedDevice))
        //    {
        //        Logger.Info("Device updated.");
        //        return new HttpResponseMessage()
        //        {
        //            StatusCode = System.Net.HttpStatusCode.OK
        //        };
        //    }
        //    else
        //    {
        //        Logger.Warn("DeviceController PUT: Failed to update device.  Unable to locate device by id.");
        //        return new HttpResponseMessage()
        //        {
        //            StatusCode = System.Net.HttpStatusCode.NotFound,
        //            ReasonPhrase = "Failed to update device.  Unable to locate device by id."

        //        };
        //    }
            


        //}

        // DELETE api/devices/{idstring}
        //public HttpResponseMessage Delete(string id)
        //{
        //    Logger.Info("DeviceController DELETE called (//PUT /api/devices) with id [{0}]...", id);
        //    if (string.IsNullOrEmpty(id))
        //    {
        //        Logger.Warn("DeviceController DELETE: Did not specify a device id.");
        //        return new HttpResponseMessage()
        //        {
        //            StatusCode = System.Net.HttpStatusCode.BadRequest,
        //            ReasonPhrase = "Did not specify a device id."

        //        };
        //    }

        

        //    if (Globals.DeviceList.RemoveById(id))
        //    {
        //        Logger.Info("Device deleted.");
        //        return new HttpResponseMessage()
        //        {
        //            StatusCode = System.Net.HttpStatusCode.OK
        //        };
        //    }
        //    else
        //    {
        //        Logger.Warn("DeviceController DELETE: Failed to delete device.  Unable to locate device by id.");
        //        return new HttpResponseMessage()
        //        {
        //            StatusCode = System.Net.HttpStatusCode.NotFound,
        //            ReasonPhrase = "Failed to delete device.  Unable to locate device by id."

        //        };
        //    }



        //}

    }
}
