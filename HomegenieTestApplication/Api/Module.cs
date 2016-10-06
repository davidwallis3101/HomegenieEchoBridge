namespace HomegenieTestApplication.Api
{
    class Module
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DeviceType { get; set; }
        public string Domain { get; set; }
        public string Address { get; set; }
        public ModuleProperties[] Properties { get; set; }
        public string RoutingNode { get; set; }
    }
}
