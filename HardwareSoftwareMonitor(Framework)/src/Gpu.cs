namespace HardwareSoftwareMonitor_Framework_.src
{
    class Gpu
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private uint vram;

        public uint Vram
        {
            get { return vram; }
            set { vram = value; }
        }

        private int verticalRes;

        public int VerticalRes
        {
            get { return verticalRes; }
            set { verticalRes = value; }
        }

        private int horizontalRes;

        public int HorizontalRes
        {
            get { return horizontalRes; }
            set { horizontalRes = value; }
        }

        private int refreshRate;

        public int RefreshRate
        {
            get { return refreshRate; }
            set { refreshRate = value; }
        }

        public Gpu(string name, uint vram, int verticalRes, int horizontalRes, int refreshRate)
        {
            this.name = name;
            this.vram = vram;
            this.verticalRes = verticalRes;
            this.horizontalRes = horizontalRes;
            this.refreshRate = refreshRate;
        }
    }
}

