using LibreHardwareMonitor.Hardware;
using System.Diagnostics;
namespace PCInsight.Client
{
    public class UpdateVisitor : IVisitor{
    public void VisitComputer(IComputer computer)
    {
        computer.Traverse(this);
    }
    public void VisitHardware(IHardware hardware)
    {
        hardware.Update();
        foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
    }
    public void VisitSensor(ISensor sensor) { }
    public void VisitParameter(IParameter parameter) { }
    }

    public class ResourceMonitor{

        public double CPU_TEMP {get; private set; }
        public double CPU_USAGE {get; private set; }
        public double GPU_TEMP {get; private set; }
        public double GPU_USAGE {get; private set; }
        public List<double> gpuUsageData = new List<double>();
        public List<double> gpuTemperatureData = new List<double>();
        public List<double> cpuUsageData = new List<double>();
        public List<double> cpuTemperatureData = new List<double>();
        public string? GPU_NAME;
        public string? CPU_NAME;
        
        public void Monitor(){
            
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

            foreach (IHardware hardware in computer.Hardware)
            {
                
                foreach (ISensor sensor in hardware.Sensors){

                    double sensorValue = 0;
                    if (sensor.Value.HasValue){
                        sensorValue = (double)Math.Round(sensor.Value.Value, 0);
                    }

                    if (hardware.HardwareType.ToString().Equals("Cpu")){
                        CPU_NAME = hardware.Name;

                        if (sensor.SensorType.ToString().Equals("Temperature") && sensor.Name.ToString().Equals("Core Average")){
                            CPU_TEMP = sensorValue;
                            cpuTemperatureData.Add(CPU_TEMP);
                            } 
                            
                            else if (sensor.SensorType.ToString().Equals("Load") && sensor.Name.ToString().Equals("CPU Total")){
                            CPU_USAGE = sensorValue;
                            cpuUsageData.Add(CPU_USAGE);
                        }

                    } else if (hardware.HardwareType.ToString().Contains("Gpu")){
                        GPU_NAME = hardware.Name;

                        if (sensor.SensorType.ToString().Equals("Temperature") && sensor.Name.ToString().Equals("GPU Core")){
                            GPU_TEMP = sensorValue;
                            gpuTemperatureData.Add(GPU_TEMP);
                        }

                        else if (sensor.SensorType.ToString().Equals("Load") && sensor.Name.ToString().Equals("GPU Core")){
                            GPU_USAGE = sensorValue;
                            gpuUsageData.Add(GPU_USAGE);
                        }
                    }
                }
            }
            
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
    }
}
