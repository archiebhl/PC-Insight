using LibreHardwareMonitor.Hardware;
using System.Diagnostics;

namespace client
{
    public class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }
        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware) 
                subHardware.Accept(this);
        }
        public void VisitSensor(ISensor sensor) { }
        public void VisitParameter(IParameter parameter) { }
    }

    public class ResourceMonitor
    {
        public double CPU_TEMP { get; private set; }
        public double CPU_USAGE { get; private set; }
        public Dictionary<string, double> CPU_CLOCKS { get; private set; } = new Dictionary<string, double>();
        public double GPU_TEMP { get; private set; }
        public double GPU_USAGE { get; private set; }
        public double GPU_VRAM_MAX { get; private set; }
        public double GPU_VRAM_CURRENT { get; private set; }
        public List<double> gpuUsageData = new List<double>();
        public List<double> gpuTemperatureData = new List<double>();
        public List<double> gpuVramUsageData = new List<double>();
        public List<double> cpuUsageData = new List<double>();
        public List<double> cpuTemperatureData = new List<double>();
        public List<Dictionary<string, double>> cpuClockData = new List<Dictionary<string, double>>();
        public string? GPU_NAME;
        public string? CPU_NAME;
        private DataSender dataSender = new DataSender();
        private Guid clientId = Guid.NewGuid();
        public double GPU_CORE_CLOCK;
        public double GPU_MEMORY_CLOCK;

        public async Task SendToServer()
        {
            await dataSender.SendDataAsync(cpuUsageData, cpuTemperatureData, gpuUsageData, gpuTemperatureData, clientId);
        }

        public void Monitor()
        {
            Computer computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = false,
                IsMotherboardEnabled = false,
                IsControllerEnabled = false,
                IsNetworkEnabled = false,
                IsStorageEnabled = false
            };

            try
            {
                computer.Open();
            }
            catch (NullReferenceException)
            {
                Debug.WriteLine("An exception of type 'System.NullReferenceException' occurred in LibreHardwareMonitorLib.dll");
                Debug.WriteLine("'Object reference not set to an instance of an object.'");
            }

            Debug.Flush();
            computer.Accept(new UpdateVisitor());

            CollectCpuData(computer);
            CollectGpuData(computer);

            try
            {
                computer?.Close();
            }
            catch (NullReferenceException)
            {
                Debug.WriteLine("An exception of type 'System.NullReferenceException' occurred in LibreHardwareMonitorLib.dll");
                Debug.WriteLine("'Object reference not set to an instance of an object.'");
            }
            Debug.Flush();
        }

        private void CollectCpuData(Computer computer)
        {
            foreach (IHardware hardware in computer.Hardware)
            {
                if (hardware.HardwareType.ToString().Equals("Cpu"))
                {
                    CPU_NAME = hardware.Name;
                    var clockData = new Dictionary<string, double>();

                    foreach (ISensor sensor in hardware.Sensors)
                    {
                        double sensorValue = sensor.Value.HasValue ? (double)Math.Round(sensor.Value.Value, 0) : 0;

                        if (sensor.SensorType.ToString().Equals("Temperature") && sensor.Name.ToString().Equals("Core Average"))
                        {
                            CPU_TEMP = sensorValue;
                            cpuTemperatureData.Add(CPU_TEMP);
                        }
                        else if (sensor.SensorType.ToString().Equals("Load") && sensor.Name.ToString().Equals("CPU Total"))
                        {
                            CPU_USAGE = sensorValue;
                            cpuUsageData.Add(CPU_USAGE);
                        }
                        else if (sensor.SensorType.ToString().Equals("Load"))
                        {
                            clockData[sensor.Name] = sensorValue;
                        }
                    }

                    if (clockData.Count > 0)
                    {
                        CPU_CLOCKS = clockData;
                        cpuClockData.Add(clockData);
                    }
                }
            }
        }

        private void CollectGpuData(Computer computer)
        {
            foreach (IHardware hardware in computer.Hardware)
            {
                if (hardware.HardwareType.ToString().Contains("Gpu"))
                {
                    GPU_NAME = hardware.Name;
                    double gpuVramMax = 0;
                    double gpuVramCurrent = 0;

                    foreach (ISensor sensor in hardware.Sensors)
                    {
                        double sensorValue = sensor.Value.HasValue ? (double)Math.Round(sensor.Value.Value, 0) : 0;

                        if (sensor.SensorType.ToString().Equals("Temperature") && sensor.Name.ToString().Equals("GPU Core"))
                        {
                            GPU_TEMP = sensorValue;
                            gpuTemperatureData.Add(GPU_TEMP);
                        }
                        else if (sensor.SensorType.ToString().Equals("Load") && sensor.Name.ToString().Equals("GPU Core"))
                        {
                            GPU_USAGE = sensorValue;
                            gpuUsageData.Add(GPU_USAGE);
                        }
                        else if (sensor.SensorType.ToString().Equals("SmallData") && sensor.Name.ToString().Equals("GPU Memory Used"))
                        {
                            GPU_VRAM_CURRENT = sensorValue;
                            gpuVramUsageData.Add(GPU_VRAM_CURRENT);
                        }
                        else if (sensor.SensorType.ToString().Equals("SmallData") && sensor.Name.ToString().Equals("GPU Memory Total"))
                        {
                            GPU_VRAM_MAX = sensorValue;
                        }
                        else if (sensor.SensorType.ToString().Equals("Clock") && sensor.Name.ToString().Equals("GPU Core"))
                        {
                            GPU_CORE_CLOCK = sensorValue;
                        }
                        else if (sensor.SensorType.ToString().Equals("Clock") && sensor.Name.ToString().Equals("GPU Memory"))
                        {
                            GPU_MEMORY_CLOCK = sensorValue;
                        }
                    }
                }
            }
        }
    }
}
