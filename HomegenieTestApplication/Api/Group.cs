using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomegenieTestApplication.Api
{
    class Group
    {
        public string Name { get; set; }
        public string Wallpaper { get; set; }
        public List<Module> Modules { get; set; }
    }
}
