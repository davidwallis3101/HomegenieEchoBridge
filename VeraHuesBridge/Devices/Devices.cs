using System;
using System.Collections.Generic;
using NLog;

namespace EchoBridge.Devices
{

    public class Devices
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private List<Device> _devices;
        //private string _loadedFromFileName;
        public Devices()
        {
            //null constructor
            Logger.Info("New Devices created.");
            _devices = new List<Device>();
        }

        //public Devices(string fileName)
        //{
        //    logger.Info("Created Devices from file [{0}]...", fileName);
        //    //null constructor
        //    _devices = new List<Device>();

        //    if (!Load(fileName))
        //    {

        //        string message = string.Format("Failed to load devices from file [{0}].", fileName);
        //        logger.Warn(message);
        //        throw new ApplicationException(message);
        //    };
        //    logger.Info("Created [{0}] Device(s) from file [{1}].", _devices.Count.ToString(), fileName);

        //}

        public Devices(List<Device> devices)
        {
            _devices = devices;
            Logger.Info("Created [{0}] Device(s) from object.", _devices.Count);
        }

        public Device FindById(string id)
        {
            Logger.Info("Finding device with id [{0}]...", id);
            var d = _devices.Find(x => x.Id == id);
            if (d==null)
            {
                Logger.Info("Could not find device with id [{0}].", id);
            }
            else
            {
                Logger.Info("Found device named [{0}] with id [{1}].", d.Name, d.Id);
            }

            return d;
        }

        //public bool RemoveById(string id)
        //{
        //    Logger.Info("Removing (by id) device with id [{0}]...", id);
        //    var d = _devices.Find(x => x.Id == id);
        //    if (d == null)
        //    {
        //        Logger.Info("Could not remove (by id) Device with id [{0}]. Device not found.", id);
        //        return false;
        //    }

        //    _devices.Remove(d);
        //    Save();
        //    Logger.Info("Removed (by id) device with id [{0}].", id);
        //    return true;
        //}

        //public bool Update(Device device)
        //{
        //    Logger.Info("Updating device with id [{0}]...", device.Id);
        //    var d = FindById(device.Id);
        //    if (d == null)
        //    {
        //        Logger.Info("Could not update Device with id [{0}]. Device not found.", device.Id);
        //        return false;
        //    }
        //    d.Name = device.Name;
        //    d.OffUrl = device.OffUrl;
        //    d.OnUrl = device.OnUrl;
        //    d.HttpVerb = device.HttpVerb;
        //    d.DeviceType = device.DeviceType;
        //    d.ContentBody = d.ContentBody;
        //    d.ContentType = d.ContentType;

        //    Save();
        //    Logger.Info("Updated device with id [{0}].", device.Id);
        //    return true;


        //}
        public void Add(Device device)
        {
            Logger.Info("Adding new device with id [{0}]...", device.Id);
            _devices.Add(device);
            //Save();
            //Logger.Info("Added new device with id [{0}].", device.Id);
        }

        //public void Remove(Device device)
        //{
        //    Logger.Info("Removing device with id [{0}]...", device.Id);
        //    _devices.Remove(device);
        //    Save();
        //    Logger.Info("Removed device with id [{0}].", device.Id);
        //}

        public List<Device> List()
        {
            return _devices;
        }

        public int Count()
        {
            return _devices == null ? 0 : _devices.Count;
        }


        //public bool Load(string fileName, bool createIfNotExists=true)
        //{
        //    Logger.Info("Loading devices from file [{0}], creating file if it does not exists [{1}]...", fileName, createIfNotExists);
        //    _devices= new List<Device>();

        //    if (!System.IO.File.Exists(fileName)) //if it doesnt exist, lets create it
        //    {
        //        Logger.Info("Creating file...");
        //        Utilities.WriteToJsonFile(fileName, _devices);
        //    }

        //    _loadedFromFileName=fileName;



        //    _devices = Utilities.ReadFromJsonFile<List<Device>>(fileName);
        //    Logger.Info("Loaded devices from file [{0}].", fileName);
        //    return true;
        //}

        //public bool Save(string fileName=null)
        //{
        //    Logger.Info("Saving devices to file [{0}]...", fileName);
        //    string file;
        //    if (string.IsNullOrEmpty(fileName))
        //    {
        //        file = _loadedFromFileName;
        //    }
        //    else
        //    {
        //        file = fileName;
        //    }

        //    if (string.IsNullOrEmpty(file))
        //    {
        //        Logger.Warn("Cannot save device configuration without providing filename first.");
        //        throw new ApplicationException("Cannot save device configuration without providing filename first.");
        //    }

        //    Utilities.WriteToJsonFile<List<Device>>(file, _devices);
        //    Logger.Info("Saved devices to file [{0}].", fileName);
        //    return true;
        //}

        public bool Contains(Device device)
        {
            return _devices.Contains(device);
        }
    }
}
