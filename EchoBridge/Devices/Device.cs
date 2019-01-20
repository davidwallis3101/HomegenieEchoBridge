using System;

namespace HGEchoBridge
{
    public class Device
    {
        public const string INTENSITY_PERCENT = "${intensity.percent}";
        public const string INTENSITY_BYTE = "${intensity.byte}";

        public Device()
        {
            _id = Guid.NewGuid();
        }

        private Guid _id; 
        
        public string id
        {
            get
            {
                return _id.ToString();
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    _id = Guid.NewGuid();
                }
                else { 
                    _id = Guid.Parse(value);
                }
            }
        }

        public String name {get; set;}
        public String deviceType {get; set;}
        public String offUrl {get; set;}
        public String onUrl {get; set;}

        public string DimUrl { get; set; }
        public String httpVerb {get; set;}
        public String contentType{get; set;}
        public String contentBody {get; set;}
    }
}
