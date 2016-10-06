using System;
using System.Collections.Generic;
using NLog;

namespace VeraHuesBridge.Devices
{
    public class DeviceResponse
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DeviceResponse()
        {
            Pointsymbol = new Dictionary<string, string>
            {
                {"1", "none"},
                {"2", "none"},
                {"3", "none"},
                {"4", "none"},
                {"5", "none"},
                {"6", "none"},
                {"7", "none"},
                {"8", "none"}
            };
        }

        public DeviceState State {get;set;}
        public string Type { get; set; }
        public string Name { get; set; }
        public string Modelid { get; set; }
        public string Manufacturername { get; set; }
        public string Uniqueid { get; set; }
        public string Swversion { get; set; }
        public Dictionary<string, string> Pointsymbol { get; set; }
        
        public static DeviceResponse CreateResponse(string name, string id){
            Logger.Info("Creating a device response object with the name [{0}], id [{1}].", name, id);

            var deviceState = new DeviceState();
            var response = new DeviceResponse {State = deviceState};
            deviceState.On=false;
            deviceState.Reachable=true;
            deviceState.Effect="none";
            deviceState.Alert="none";
            deviceState.Bri=254;
            deviceState.Hue=15823;
            deviceState.Sat=88;
            deviceState.Ct=313;

            var xv = new List<Double> {0.4255, 0.3998};
            deviceState.Xy=xv;

            deviceState.Colormode="ct";
            response.Name=name;
            response.Uniqueid=id;
            response.Manufacturername="Philips";
            response.Type="Extended color light";
            response.Modelid="LCT001";
            response.Swversion="65003148";

            return response;
        }
    }
}
