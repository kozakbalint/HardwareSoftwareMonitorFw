namespace HardwareSoftwareMonitor_Framework_.src
{
    class Ram
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string manufacturer;

        public string Manufacturer
        {
            get { return manufacturer; }
            set { manufacturer = value; }
        }

        private string tag;

        public string Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        private long capacity;

        public long Capacity
        {
            get { return capacity; }
            set { capacity = value; }
        }
        public Ram(string name, string manufacturer, string tag, long capacity)
        {
            this.name = name;
            this.manufacturer = manufacturer;
            this.tag = tag;
            this.capacity = capacity;
        }
    }
}
