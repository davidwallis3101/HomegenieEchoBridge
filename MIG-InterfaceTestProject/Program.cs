using System;

using MIG;
using System.IO;
using MIG.Config;
using System.Xml.Serialization;

namespace TestProject
{
    class MainClass
    {
        public static void Main (string[] args)
        {

            var migService = new MigService();

            // Construct an instance of the XmlSerializer with the type
            // of object that is being deserialized.
            var mySerializer = new XmlSerializer(typeof(MigServiceConfiguration));

            // To read the file, create a FileStream.
            var myFileStream = new FileStream("systemconfig.xml", FileMode.Open);

            // Call the Deserialize method and cast to the object type.
            var configuration = (MigServiceConfiguration)mySerializer.Deserialize(myFileStream);

            // Load the configuration Set the configuration and start MIG Service
            migService.Configuration = configuration;
            migService.StartService();


            Console.WriteLine("\n[Press Enter to Quit]\n");
            Console.ReadLine();
        }
    }
}
