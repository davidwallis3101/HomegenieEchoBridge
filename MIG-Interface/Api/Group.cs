using System.Collections.Generic;

namespace MIG.Interfaces.HomeAutomation.Api
{
    class Group
    {
        public string Name { get; set; }
        public string Wallpaper { get; set; }
        public List<Module> Modules { get; set; }
    }
}
