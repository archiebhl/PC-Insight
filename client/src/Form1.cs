using ScottPlot;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Timer = System.Windows.Forms.Timer;

namespace client
{
    public partial class Form1 : Form
    {
        private ResourceMonitor rm = new ResourceMonitor();
        private Timer updateTimer = new Timer
        {
            Interval = 1000 // 2 second(s)
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
            UpdateData(); // must be run twice for the labels to appear?
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

        private async Task UpdateData()
        {
            await Task.Run(() =>
            {
                rm.Monitor();

                // UI Thread
                BeginInvoke((MethodInvoker)(() =>
                {
                    cpuGroupBox.Text = rm.CPU_NAME;
                    gpuGroupBox.Text = rm.GPU_NAME;
                    
                    gpuDataGrid.Rows.Clear();
                    foreach (var kvp in rm.gpuExtraInfo)
                    {
                        gpuDataGrid.Rows.Add(kvp.Key, kvp.Value);
                    }
                    cpuDataGrid.Rows.Clear();
                    foreach (var kvp in rm.cpuExtraInfo)
                    {
                        cpuDataGrid.Rows.Add(kvp.Key, kvp.Value);
                    }

                    UpdateGraph(gpuUsagePlot, rm.gpuUsageData, gpuUsageCanvas, rm.GPU_USAGE, "Usage");
                    UpdateGraph(gpuTemperaturePlot, rm.gpuTemperatureData, gpuTemperatureCanvas, rm.GPU_TEMP, "Temperature");
                    UpdateGraph(cpuUsagePlot, rm.cpuUsageData, cpuUsageCanvas, rm.CPU_USAGE, "Usage");
                    UpdateGraph(cpuTemperaturePlot, rm.cpuTemperatureData, cpuTemperatureCanvas, rm.CPU_TEMP, "Temperature");
                }));
            });
        }
        private void UpdateGraph(Plot plot, List<double> dataList, SKControl skControl, double value, string measurement)
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
                    plot.Axes.Title.Label.Text = string.Format("{0}: {1}%", measurement, value);
                    plot.Axes.Title.Label.FontSize = 13;
                    plot.Axes.Title.Label.OffsetY = -10;
                    plot.Axes.Title.Label.FontName = "MS Sans Serif";
                    plot.Axes.Top.MaximumSize = 0;
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
