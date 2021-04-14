﻿using HardwareSoftwareMonitor_Framework_.src;
using Microsoft.Win32;
using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Windows;
using System.Windows.Threading;

namespace HardwareSoftwareMonitor_Framework_
{
    public partial class MainWindow : Window
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

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
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
                cpus.Add(new Cpu(item["Name"].ToString(), item["Manufacturer"].ToString(), Convert.ToInt32(item["NumberOfCores"]), Convert.ToInt32(item["ThreadCount"])));
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
        }

        private void SaveHardwareInfo()
        {
            sw = new StreamWriter("../../save/hardware_save.csv");
            sw.WriteLine($"{mb.Manufacturer},{mb.Product}");
            foreach (var item in cpus)
            {
                sw.WriteLine($"{item.Name},{item.Manufacturer},{item.Cores},{item.Threads}");
            }
            foreach (var item in gpus)
            {
                sw.WriteLine($"{item.Name};{item.Vram},{item.HorizontalRes},{item.VerticalRes},{item.RefreshRate}");
            }
            foreach (var item in rams)
            {
                sw.WriteLine($"{item.Name},{item.Manufacturer},{item.Tag},{item.Capacity}");
            }
            foreach (var item in disks)
            {
                sw.WriteLine($"{item.Manufacturer},{item.InterfaceType},{item.Size}");
            }
            foreach (var item in drives)
            {
                sw.WriteLine($"{item.RootDir},{item.FileSystem},{item.TotalSize},{item.AvailableSpace}");
            }
            sw.Close();
        }

        private void SavePrograms()
        {
            sw = new StreamWriter("../../save/app_save.csv");
            foreach (var item in apps)
            {
                sw.WriteLine($"{item.Name},{item.Version}");
            }
            sw.Close();
        }
    }
}
