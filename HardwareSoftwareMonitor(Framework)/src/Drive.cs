namespace HardwareSoftwareMonitor_Framework_.src
{
    class Drive
    {
        private string rootDir;

        public string RootDir
        {
            get { return rootDir; }
            set { rootDir = value; }
        }

        private string fileSystem;

        public string FileSystem
        {
            get { return fileSystem; }
            set { fileSystem = value; }
        }

        private long totalSize;

        public long TotalSize
        {
            get { return totalSize; }
            set { totalSize = value; }
        }

        private long availableSpace;

        public long AvailableSpace
        {
            get { return availableSpace; }
            set { availableSpace = value; }
        }

        public Drive(string rootDir, string fileSystem, long totalSize, long availableSpace)
        {
            this.rootDir = rootDir;
            this.fileSystem = fileSystem;
            this.totalSize = totalSize;
            this.availableSpace = availableSpace;
        }
    }
}

