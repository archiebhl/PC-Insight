using ScottPlot;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Timer = System.Windows.Forms.Timer;

namespace PCInsight
{
    public partial class Form1 : Form
    {
        private ResourceMonitor rm = new ResourceMonitor();
        private Timer updateTimer = new Timer
        {
            Interval = 2000 // 2 second(s)
        };
        private Plot gpuUsagePlot = new();
        private Plot gpuTemperaturePlot = new();
        private Plot cpuUsagePlot = new();
        private Plot cpuTemperaturePlot = new();
        private bool pauseTimer = false;

        public Form1()
        {
            InitializeComponent();
            InitializePlot();
            UpdateData(); // first run to gather component info
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            StartUpdateTimer();
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            PauseUpdateTimer();
        }

        private void StartUpdateTimer()
        {
            if (pauseTimer == true)
            {
                pauseTimer = false;
            } else
            {
                updateTimer.Tick += UpdateTimerTick;
                updateTimer.Start();
            }
        }

        private void PauseUpdateTimer()
        {
            pauseTimer = true;
        }

        private void UpdateTimerTick(object? sender, EventArgs e)
        {   
            if (!pauseTimer)
            {
                UpdateData();
            }
        }

        private async void UpdateData()
        {
            await Task.Run(() =>
            {
                rm.Monitor();

                // UI Thread
                BeginInvoke((MethodInvoker)(() =>
                {
                    cpuGroupBox.Text = rm.CPU_NAME;
                    cpuUsageHeading.Text = string.Format("Usage: {0}%", rm.CPU_USAGE);
                    cpuTempHeading.Text = string.Format("Temperature: {0}°C", rm.CPU_TEMP);

                    gpuGroupBox.Text = rm.GPU_NAME;
                    gpuUsageHeading.Text = string.Format("Usage: {0}%", rm.GPU_USAGE);
                    gpuTempHeading.Text = string.Format("Temperature: {0}°C", rm.GPU_TEMP);

                    UpdateGraph(gpuUsagePlot, rm.gpuUsageData, gpuUsageCanvas);
                    UpdateGraph(gpuTemperaturePlot, rm.gpuTemperatureData, gpuTemperatureCanvas);
                    UpdateGraph(cpuUsagePlot, rm.cpuUsageData, cpuUsageCanvas);
                    UpdateGraph(cpuTemperaturePlot, rm.cpuTemperatureData, cpuTemperatureCanvas);
                }));
            });
        }

        private void UpdateGraph(Plot plot, List<double> dataList, SKControl skControl)
        {
            double[] dataArray = dataList.ToArray();
            plot.Clear();
            plot.Add.Signal(dataArray);

            BeginInvoke((MethodInvoker)(() =>
            {   
                // SKControl wrapper for ScottPlots
                skControl.PaintSurface += (s, e) =>
                {
                    SKSurface surface = e.Surface;
                    SKCanvas canvas = surface.Canvas;

                    plot.Render(canvas, skControl.Width, skControl.Height);
                    plot.Axes.AutoScaleExpandX();
                };

                skControl.Dock = DockStyle.Fill;
                skControl.Invalidate();
            }));
        }

        private void InitializePlot()
        {
            InitializePlot(gpuUsagePlot);
            InitializePlot(gpuTemperaturePlot);
            InitializePlot(cpuUsagePlot);
            InitializePlot(cpuTemperaturePlot);
        }

        private void InitializePlot(Plot plot)
        {
            plot.Axes.SetLimitsY(0, 100);
            plot.HideGrid();
            plot.Style.Background(figure: ScottPlot.Color.FromHex("f0f0f0"), data: ScottPlot.Color.FromHex("#f0f0f0"));
            plot.Axes.Title.Label.Bold = false;

            ScottPlot.TickGenerators.NumericManual ticks = new();
            plot.Axes.Bottom.TickGenerator = ticks;
        }
    }
}
