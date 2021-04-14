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

        private int l2size;

        public int L2Size
        {
            get { return l2size; }
            set { l2size = value; }
        }

        private int l3size;

        public int L3Size
        {
            get { return l3size; }
            set { l3size = value; }
        }



        public Cpu(string name, string manufacturer, int cores, int threads, int l2size, int l3size)
        {
            this.name = name;
            this.manufacturer = manufacturer;
            this.cores = cores;
            this.threads = threads;
            this.l2size = l2size;
            this.l3size = l3size;
        }
    }
}

