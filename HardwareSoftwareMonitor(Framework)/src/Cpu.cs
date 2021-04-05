namespace HardwareSoftwareMonitor_Framework_.src
{
    class Cpu
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

        private int cores;

        public int Cores
        {
            get { return cores; }
            set { cores = value; }
        }

        private int threads;

        public int Threads
        {
            get { return threads; }
            set { threads = value; }
        }

        public Cpu(string name, string manufacturer, int cores, int threads)
        {
            this.name = name;
            this.manufacturer = manufacturer;
            this.cores = cores;
            this.threads = threads;
        }
    }
}

