using System;

namespace EchoBridge.Devices
{
    public class Device
    {

        public Device()
        {
            _id = Guid.NewGuid();
        }

        private Guid _id; 
        
        public string Id
        {
            get
            {
                return _id.ToString();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _id = Guid.NewGuid();
                }
                else { 
                    _id = Guid.Parse(value);
                }
            }
        }
        public string Name {get; set;}
        public string DeviceType {get; set;}
        public string OffUrl {get; set;}
        public string OnUrl {get; set;}
        public string HttpVerb {get; set;}
        public string ContentType {get; set;}
        public string ContentBody {get; set;}

    
    

    }
}
