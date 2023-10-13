using System;
using System.Diagnostics;
using System.Management;
using System.Windows;
using System.Windows.Threading;
namespace qwe
{
    public partial class MainWindow : Window
    {
        private PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        private double totalPhysicalMemory = GetTotalPhysicalMemory(); 
        private DispatcherTimer updateTimer;

        public MainWindow()
        {
            InitializeComponent();

            updateTimer = new DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromSeconds(1); 
            updateTimer.Tick += UpdateRamUsage;
            updateTimer.Start();
        }

        static private double GetTotalPhysicalMemory()
        {
            ObjectQuery wmiQuery = new ObjectQuery("SELECT * FROM Win32_ComputerSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiQuery);
            ManagementObjectCollection results = searcher.Get();

            foreach (ManagementObject queryObj in results)
            {
                return Convert.ToDouble(queryObj["TotalPhysicalMemory"]);
            }

            return 0; 
        }

        private void UpdateRamUsage(object sender, EventArgs e)
        {
            float availableMemory = ramCounter.NextValue();
            double usedMemory = totalPhysicalMemory - availableMemory;
            double ramUsagePercentage = (usedMemory / totalPhysicalMemory) * 100;

            ramProgressBar.Value = ramUsagePercentage;
            ramPercentage.Text = $"RAM Usage: {ramUsagePercentage:F2}%";
        }
    }
}
