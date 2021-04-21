using HardwareSoftwareMonitor_Framework_.src;
using Microsoft.Win32;
using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Windows.Forms;
using System.Windows.Threading;
using Excel = Microsoft.Office.Interop.Excel;

namespace HardwareSoftwareMonitor_Framework_
{
    public partial class MainWindow : System.Windows.Window
    {
        string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        ManagementObjectSearcher searcher;
        List<InstalledApp> apps = new List<InstalledApp>();
        List<string> keys = new List<string>() {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
        };
        List<Cpu> cpus = new List<Cpu>();
        List<Gpu> gpus = new List<Gpu>();
        List<Ram> rams = new List<Ram>();
        List<Disk> disks = new List<Disk>();
        List<Drive> drives = new List<Drive>();
        MotherBoard mb;
        Computer computer;
        DispatcherTimer timer;
        StreamWriter sw;
        string savePath = AppDomain.CurrentDomain.BaseDirectory;
        
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            SavePath.Text = savePath;
            softDG.ItemsSource = apps;
            computer = new Computer() { CPUEnabled = true, GPUEnabled = true, MainboardEnabled = true, RAMEnabled = true, HDDEnabled = true };
            computer.Open();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
            timer.Start();
            GetInstalledApps();
            GetCpuInfos();
            GetGpuInfos();
            GetRamInfos();
            GetMbInfos();
            GetDiskInfos();
            GetDriveInfos();
            FillComboBoxes();
            FillHardwareData();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            GetCpuSensorValues();
            GetGpuSensorValues();
            GetMbSensorValues();
            GetRamSensorValues();
            GetHddSensorValues();
        }

        private void GetHddSensorValues()
        {
            string tempContent = "Temp:\n";
            foreach (var hardwareItem in computer.Hardware)
            {
                if (hardwareItem.HardwareType == HardwareType.HDD)
                {
                    hardwareItem.Update();
                    foreach (var sensor in hardwareItem.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            tempContent += $"{sensor.Name} = {sensor.Value.Value}°C\r\n";
                        }
                    }
                }
            }
            hddTemp.Content = tempContent;
        }

        private void GetRamSensorValues()
        {
            string loadContent = "Load:\n";
            foreach (var hardwareItem in computer.Hardware)
            {
                if (hardwareItem.HardwareType == HardwareType.RAM)
                {
                    hardwareItem.Update();
                    foreach (IHardware subHardware in hardwareItem.SubHardware)
                        subHardware.Update();

                    foreach (var sensor in hardwareItem.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load)
                        {
                            loadContent += $"{sensor.Name} = {Math.Round(sensor.Value.Value, 1)}%\r\n";
                        }
                    }
                }
            }
            ramLoad.Content = loadContent;
        }

        private void GetMbSensorValues()
        {
            string tempContent = "Temp:\n";
            string voltContent = "Voltage:\n";
            string fanContent = "Fan Speed:\n";
            foreach (var hardwareItem in computer.Hardware)
            {
                if (hardwareItem.HardwareType == HardwareType.Mainboard)
                {
                    hardwareItem.Update();
                    foreach (IHardware subHardware in hardwareItem.SubHardware)
                    {
                        subHardware.Update();

                        foreach (var sensor in subHardware.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Temperature)
                            {
                                tempContent += $"{sensor.Name} = {sensor.Value.Value}°C\r\n";
                            }
                            else if (sensor.SensorType == SensorType.Voltage)
                            {
                                voltContent += $"{sensor.Name} = {Math.Round(sensor.Value.Value, 2)}V\r\n";
                            }
                            else if (sensor.SensorType == SensorType.Fan)
                            {
                                fanContent += $"{sensor.Name} = {Math.Round(sensor.Value.Value, 0)}RPM\r\n";
                            }
                        }
                    }
                }
                mbTemp.Content = tempContent;
                mbVolt.Content = voltContent;
                mbFan.Content = fanContent;
            }
        }

        private void GetGpuSensorValues()
        {
            string tempContent = "Temp:\n";
            string loadContent = "Load:\n";
            string clockContent = "Speed:\n";
            foreach (var hardwareItem in computer.Hardware)
            {
                if (hardwareItem.HardwareType == HardwareType.GpuNvidia || hardwareItem.HardwareType == HardwareType.GpuAti)
                {
                    hardwareItem.Update();

                    foreach (var sensor in hardwareItem.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            tempContent += $"{sensor.Name} = {sensor.Value.Value}°C\r\n";
                        }
                        else if (sensor.SensorType == SensorType.Load)
                        {
                            loadContent += $"{sensor.Name} = {Math.Round(sensor.Value.Value, 1)}%\r\n";
                        }
                        else if (sensor.SensorType == SensorType.Clock)
                        {
                            clockContent += $"{sensor.Name} = {Math.Round(sensor.Value.Value, 1)}Mhz\r\n";
                        }
                    }
                }
                gpuTemp.Content = tempContent;
                gpuLoad.Content = loadContent;
                gpuClock.Content = clockContent;
            }
        }

        private void GetCpuSensorValues()
        {
            string tempContent = "Temp:\n";
            string loadContent = "Load:\n";
            string clockContent = "Speed:\n";
            foreach (var hardwareItem in computer.Hardware)
            {
                if (hardwareItem.HardwareType == HardwareType.CPU)
                {
                    hardwareItem.Update();

                    foreach (var sensor in hardwareItem.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            tempContent += $"{sensor.Name} = {sensor.Value.Value}°C\r\n";
                        }
                        else if (sensor.SensorType == SensorType.Load)
                        {
                            loadContent += $"{sensor.Name} = {Math.Round(sensor.Value.Value, 1)}%\r\n";
                        }
                        else if (sensor.SensorType == SensorType.Clock)
                        {
                            clockContent += $"{sensor.Name} = {Math.Round(sensor.Value.Value, 1)}Mhz\r\n";
                        }
                    }
                }
            }
            cpuTemp.Content = tempContent;
            cpuLoad.Content = loadContent;
            cpuClock.Content = clockContent;
        }

        private void FillHardwareData()
        {
            try
            {
                //CPU
                Cpu currCpu = cpus.Where(x => x.Name == cpuCb.SelectedItem.ToString()).First();
                cpuName.Content = $"Name: {currCpu.Name}";
                cpuManufacturer.Content = $"Manufacturer: {currCpu.Manufacturer}";
                cpuCores.Content = $"Cores: {currCpu.Cores}";
                cpuThreads.Content = $"Threads: {currCpu.Threads}";
                cpul2size.Content = $"L2 Chache: {currCpu.L2Size / 1024} MB";
                cpul3size.Content = $"L3 Chache: {currCpu.L3Size / 1024} MB";

                //GPU
                Gpu currGpu = gpus.Where(x => x.Name == gpuCb.SelectedItem.ToString()).First();
                gpuName.Content = $"Name: {currGpu.Name}";
                gpuVram.Content = $"VRAM: {SizeSuffix(currGpu.Vram)}";
                gpuResolution.Content = $"Resolution: {currGpu.HorizontalRes}x{currGpu.VerticalRes}";
                gpuRefreshRate.Content = $"Refresh Rate: {currGpu.RefreshRate}";

                //RAM
                Ram currRam = rams.Where(x => x.Tag == ramCb.SelectedItem.ToString()).First();
                ramName.Content = $"Name: {currRam.Name}";
                ramManufacturer.Content = $"Manufacturer: {currRam.Manufacturer}";
                ramTag.Content = $"Tag: {currRam.Tag}";
                ramCapacity.Content = $"Capacity: {SizeSuffix(currRam.Capacity)}";

                //MotherBoard
                mbManufacturer.Content = $"Manufacturer: {mb.Manufacturer}";
                mbProduct.Content = $"Product: {mb.Product}";

                //Disks
                Disk currDisk = disks.Where(x => x.Manufacturer == diskCb.SelectedItem.ToString()).First();
                diskManufacturer.Content = $"Manufacturer: {currDisk.Manufacturer}";
                diskInterface.Content = $"Interface: {currDisk.InterfaceType}";
                diskSize.Content = $"Size: {SizeSuffix(currDisk.Size)}";

                //Drives
                Drive currDrive = drives.Where(x => x.RootDir == driveCb.SelectedItem.ToString()).First();
                driveRootDir.Content = $"Root Directory: {currDrive.RootDir}";
                driveFileSystem.Content = $"File System: {currDrive.FileSystem}";
                driveTotalSize.Content = $"Total Size: {SizeSuffix(currDrive.TotalSize)}";
                driveAvailableSpace.Content = $"Available Space: {SizeSuffix(currDrive.AvailableSpace)}";
            }
            catch (Exception)
            {
                return;
            }
        }

        private void FillComboBoxes()
        {
            //CPU
            foreach (var item in cpus)
            {
                cpuCb.Items.Add(item.Name);
            }
            cpuCb.SelectedItem = cpuCb.Items[0];
            //GPU
            foreach (var item in gpus)
            {
                gpuCb.Items.Add(item.Name);
            }
            gpuCb.SelectedItem = gpuCb.Items[0];
            //RAM
            foreach (var item in rams)
            {
                ramCb.Items.Add(item.Tag);
            }
            ramCb.SelectedItem = ramCb.Items[0];
            //Disks
            foreach (var item in disks)
            {
                diskCb.Items.Add(item.Manufacturer);
            }
            diskCb.SelectedItem = diskCb.Items[0];
            //Drives
            foreach (var item in drives)
            {
                driveCb.Items.Add(item.RootDir);
            }
            driveCb.SelectedItem = driveCb.Items[0];
        }

        private void GetDriveInfos()
        {
            foreach (var item in DriveInfo.GetDrives())
            {
                drives.Add(new Drive(item.RootDirectory.ToString(), item.DriveFormat, Convert.ToInt64(item.TotalSize), Convert.ToInt64(item.AvailableFreeSpace)));
            }
        }

        private void GetDiskInfos()
        {
            searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            foreach (var item in searcher.Get())
            {
                disks.Add(new Disk(item["Model"].ToString(), item["InterfaceType"].ToString(), Convert.ToUInt64(item["Size"])));
            }
            searcher = null;
        }

        private void GetMbInfos()
        {
            searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            foreach (var item in searcher.Get())
            {
                mb = new MotherBoard(item["Manufacturer"].ToString(), item["Product"].ToString());
            }
            searcher = null;
        }

        private void GetRamInfos()
        {
            searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
            foreach (var item in searcher.Get())
            {
                if (item != null)
                {
                    rams.Add(new Ram(item["Name"].ToString(), item["Manufacturer"].ToString(), item["Tag"].ToString(), Convert.ToInt64(item["Capacity"])));
                }
            }
            searcher = null;
        }

        private void GetGpuInfos()
        {
            searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            foreach (var item in searcher.Get())
            {
                gpus.Add(new Gpu(item["Name"].ToString(), Convert.ToUInt32(item["AdapterRAM"]), Convert.ToInt32(item["CurrentVerticalResolution"]), Convert.ToInt32(item["CurrentHorizontalResolution"]), Convert.ToInt32(item["CurrentRefreshRate"])));
            }
            searcher = null;
        }

        private void GetCpuInfos()
        {
            searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            foreach (var item in searcher.Get())
            {
                cpus.Add(new Cpu(item["Name"].ToString(), item["Manufacturer"].ToString(), Convert.ToInt32(item["NumberOfCores"]), Convert.ToInt32(item["ThreadCount"]), Convert.ToInt32(item["L2CacheSize"]), Convert.ToInt32(item["L3CacheSize"])));
            }
            searcher = null;
        }

        private void GetInstalledApps()
        {
            FindInstalls(RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64), keys, apps);
            FindInstalls(RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64), keys, apps);

            apps.RemoveAll(x => x.Name == "");
            softDG.Items.Refresh();
        }

        private void FindInstalls(RegistryKey regKey, List<string> keys, List<InstalledApp> installs)
        {
            foreach (string key in keys)
            {
                using (RegistryKey rk = regKey.OpenSubKey(key))
                {
                    if (rk == null)
                    {
                        continue;
                    }
                    foreach (string skName in rk.GetSubKeyNames())
                    {
                        using (RegistryKey sk = rk.OpenSubKey(skName))
                        {
                            try
                            {
                                installs.Add(new InstalledApp(Convert.ToString(sk.GetValue("DisplayName")), Convert.ToString(sk.GetValue("DisplayVersion"))));
                            }
                            catch (Exception)
                            { }
                        }
                    }
                }
            }
        }

        private string SizeSuffix(Int64 value)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }
        private string SizeSuffix(UInt64 value)
        {
            if (value == 0) { return "0.0 bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }

        private void Cb_SelectionChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            FillHardwareData();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SavePrograms();
            SaveHardwareInfo();
            SaveToExcel();
        }

        private void SaveToExcel()
        {
            Excel.Application app = new Excel.Application();
            app.Visible = false;
            app.DisplayAlerts = false;
            Excel.Workbook wb = app.Workbooks.Add();
            wb.Worksheets.Add();

            //Installed Apps worksheet
            Excel._Worksheet installedApps = app.Worksheets[app.Worksheets.Count -1];
            installedApps.Name = "Installed Applications";
            installedApps.Cells[1, 1].Value = "Name:";
            installedApps.Cells[1, 2].Value = "Version:";
            for (int i = 0; i < apps.Count; i++)
            {
                installedApps.Cells[i+2, 1].Value = apps[i].Name;
                installedApps.Cells[i+2, 2].Value = $"{apps[i].Version}";
            }
            installedApps.Columns.AutoFit();

            //Hardware Infos worksheet
            Excel._Worksheet hardawreInfos = app.Worksheets[app.Worksheets.Count];
            hardawreInfos.Name = "Hardware Informations";
            hardawreInfos.Cells[1, 1] = "CPU:";
            hardawreInfos.Cells[2, 1] = "Name";
            hardawreInfos.Cells[3, 1] = "Manufacturer";
            hardawreInfos.Cells[4, 1] = "Cores";
            hardawreInfos.Cells[5, 1] = "Threads";
            hardawreInfos.Cells[6, 1] = "L2 Chache";
            hardawreInfos.Cells[7, 1] = "L3 Chache";

            for (int i = 0; i < cpus.Count; i++)
            {
                hardawreInfos.Cells[2, i + 2] = cpus[i].Name;
                hardawreInfos.Cells[3, i + 2] = cpus[i].Manufacturer;
                hardawreInfos.Cells[4, i + 2] = $"{cpus[i].Cores}";
                hardawreInfos.Cells[5, i + 2] = $"{cpus[i].Threads}";
                hardawreInfos.Cells[6, i + 2] = $"{cpus[i].L2Size /1024} MB";
                hardawreInfos.Cells[7, i + 2] = $"{cpus[i].L3Size /1024} MB";
            }

            hardawreInfos.Cells[9, 1] = "GPU:";
            hardawreInfos.Cells[10, 1] = "Name";
            hardawreInfos.Cells[11, 1] = "Vram";
            hardawreInfos.Cells[12, 1] = "Resolution";
            hardawreInfos.Cells[13, 1] = "Refresh Rate";

            for (int i = 0; i < gpus.Count; i++)
            {
                hardawreInfos.Cells[10, i + 2] = gpus[i].Name;
                hardawreInfos.Cells[11, i + 2] = SizeSuffix(gpus[i].Vram);
                hardawreInfos.Cells[12, i + 2] = $"{ gpus[i].HorizontalRes}x{gpus[i].VerticalRes}";
                hardawreInfos.Cells[13, i + 2] = $"{gpus[i].RefreshRate} Hz";
            }

            hardawreInfos.Cells[15, 1] = "RAM:";
            hardawreInfos.Cells[16, 1] = "Name";
            hardawreInfos.Cells[17, 1] = "Manufacturer";
            hardawreInfos.Cells[18, 1] = "Tag";
            hardawreInfos.Cells[19, 1] = "Capacity";

            for (int i = 0; i < rams.Count; i++)
            {
                hardawreInfos.Cells[16, i + 2] = rams[i].Name;
                hardawreInfos.Cells[17, i + 2] = rams[i].Manufacturer;
                hardawreInfos.Cells[18, i + 2] = rams[i].Tag;
                hardawreInfos.Cells[19, i + 2] = SizeSuffix(rams[i].Capacity);
            }

            hardawreInfos.Cells[21, 1] = "Motherboard:";
            hardawreInfos.Cells[22, 1] = "Manufacturer:";
            hardawreInfos.Cells[23, 1] = "Product:";
            hardawreInfos.Cells[22, 2] = mb.Manufacturer;
            hardawreInfos.Cells[23, 2] = mb.Product;

            hardawreInfos.Cells[25, 1] = "Disks:";
            hardawreInfos.Cells[26, 1] = "Manufacturer";
            hardawreInfos.Cells[27, 1] = "Interface";
            hardawreInfos.Cells[28, 1] = "Size";

            for (int i = 0; i < disks.Count; i++)
            {
                hardawreInfos.Cells[26, i+2] = disks[i].Manufacturer;
                hardawreInfos.Cells[27, i+2] = disks[i].InterfaceType;
                hardawreInfos.Cells[28, i+2] = SizeSuffix(disks[i].Size);
            }

            hardawreInfos.Cells[30, 1] = "Drives:";
            hardawreInfos.Cells[31, 1] = "Root Dir";
            hardawreInfos.Cells[32, 1] = "File System";
            hardawreInfos.Cells[33, 1] = "Total Size";
            hardawreInfos.Cells[34, 1] = "Available Space";

            for (int i = 0; i < drives.Count; i++)
            {
                hardawreInfos.Cells[31, i + 2] = drives[i].RootDir;
                hardawreInfos.Cells[32, i + 2] = drives[i].FileSystem;
                hardawreInfos.Cells[33, i + 2] = SizeSuffix(drives[i].TotalSize);
                hardawreInfos.Cells[34, i + 2] = SizeSuffix(drives[i].AvailableSpace);
            }
            hardawreInfos.Columns.AutoFit();

            //Saving
            try
            {
                wb.SaveAs($"{savePath}\\infos", Excel.XlFileFormat.xlWorkbookDefault,
                Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive,
                Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing);
                wb.Close();
                app = null;
            }
            catch (Exception)
            {
                MessageBox.Show("Save was failed, because the save folder is readonly.");
            }

        }

        private void SaveHardwareInfo()
        {
            sw = new StreamWriter($"{savePath}\\hardware_save.csv");
            sw.WriteLine($"{mb.Manufacturer},{mb.Product}");
            foreach (var item in cpus)
            {
                sw.WriteLine($"{item.Name},{item.Manufacturer},{item.Cores},{item.Threads},{item.L2Size},{item.L3Size}");
            }
            foreach (var item in gpus)
            {
                sw.WriteLine($"{item.Name};{SizeSuffix(item.Vram)},{item.HorizontalRes},{item.VerticalRes},{item.RefreshRate}");
            }
            foreach (var item in rams)
            {
                sw.WriteLine($"{item.Name},{item.Manufacturer},{item.Tag},{SizeSuffix(item.Capacity)}");
            }
            foreach (var item in disks)
            {
                sw.WriteLine($"{item.Manufacturer},{item.InterfaceType},{SizeSuffix(item.Size)}");
            }
            foreach (var item in drives)
            {
                sw.WriteLine($"{item.RootDir},{item.FileSystem},{SizeSuffix(item.TotalSize)},{SizeSuffix(item.AvailableSpace)}");
            }
            sw.Close();
        }

        private void SavePrograms()
        {
            sw = new StreamWriter($"{savePath}\\app_save.csv");
            foreach (var item in apps)
            {
                sw.WriteLine($"{item.Name},{item.Version}");
            }
            sw.Close();
        }

        private void BrowsePath(object sender, System.Windows.RoutedEventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.ShowDialog();
            savePath = fd.SelectedPath;
            SavePath.Text = fd.SelectedPath;
        }
    }
}
