namespace PCInsight;

using System.Diagnostics;
using System.Windows.Forms;
using ScottPlot;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

public partial class Form1 : Form
{

    private ResourceMonitor rm = new ResourceMonitor();
    private Timer updateTimer;
    private System.Windows.Forms.Label infoLabel = new System.Windows.Forms.Label();
    private TableLayoutPanel table = new TableLayoutPanel();

    
    private ScottPlot.Plot gpuUsagePlot = new();
    private ScottPlot.Plot gpuTemperaturePlot = new();
    private ScottPlot.Plot cpuUsagePlot = new();
    private ScottPlot.Plot cpuTemperaturePlot = new();

    private SKControl gpuUsageCanvas = new SKControl();
    private SKControl gpuTemperatureCanvas = new SKControl();
    private SKControl cpuUsageCanvas = new SKControl();
    private SKControl cpuTemperatureCanvas = new SKControl();

    public Form1()
    {
        InitializeComponent();
        InitializeLabel();
        InitializeTable();

        this.Width = 1000;
        this.Height = 730;
        
        InitializePlot(gpuUsagePlot, gpuUsageCanvas);
        InitializePlot(gpuTemperaturePlot, gpuTemperatureCanvas);
        InitializePlot(cpuUsagePlot, cpuUsageCanvas);
        InitializePlot(cpuTemperaturePlot, cpuTemperatureCanvas);

        updateTimer = new Timer();
        updateTimer.Interval = 2000; // 2 second(s) (minimum)
        updateTimer.Tick += UpdateTimerTick;
        updateTimer.Start();
        this.Icon = new Icon("logo.ico");
    }

    private void UpdateTimerTick(object? sender, EventArgs e){
        UpdateData();
    }
    private async void UpdateData()
    {

        await Task.Run(() =>
        {
            rm.Monitor();

            string labelText = string.Format("GPU Temperature: {0}°C", rm.GPU_TEMP) + Environment.NewLine +
                            string.Format("GPU Usage: {0}%", rm.GPU_USAGE) + Environment.NewLine +
                            string.Format("CPU Temperature: {0}°C", rm.CPU_TEMP) + Environment.NewLine +
                            string.Format("CPU Usage: {0}%", rm.CPU_USAGE);
            
            // UI Thread
            BeginInvoke((MethodInvoker)(() =>
            {
                table.Controls.Add(infoLabel, 0, 0);
                infoLabel.Text = labelText;
                infoLabel.AutoSize = true;

                UpdateGraph(gpuUsagePlot, rm.gpuUsageData, gpuUsageCanvas, "GPU", "usage");
                UpdateGraph(gpuTemperaturePlot, rm.gpuTemperatureData, gpuTemperatureCanvas, "GPU", "temperature");
                
                UpdateGraph(cpuUsagePlot, rm.cpuUsageData, cpuUsageCanvas, "CPU", "usage");
                UpdateGraph(cpuTemperaturePlot, rm.cpuTemperatureData, cpuTemperatureCanvas, "CPU", "temperature");

            }));
        });
    }

    private void UpdateGraph(ScottPlot.Plot plot, List<double> dataList, SKControl skControl, string component, string measurement)
    {
        double[] dataArray = dataList.ToArray();

        foreach (var plottable in plot.GetPlottables())
        {
            if (plottable is ScottPlot.Plottables.Signal){
                plot.Remove(plottable);
                break;
            }
        }
        
        plot.Add.Signal(dataArray);

        int row = (measurement.ToLower() == "temperature") ? 2 : 3;
        int column = (component.ToUpper() == "GPU") ? 0 : 1;
        table.Controls.Add(skControl, column, row);

        // SKControl wrapper for SigPlot
        skControl.PaintSurface += (s, e) =>
        {
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            plot.Render(canvas, skControl.Width, skControl.Height);
            plot.Axes.AutoScaleExpandX();
            plot.Axes.Title.Label.Text = $"{component} {measurement}";
            plot.Axes.Title.Label.FontName = "Courier New";
            plot.Axes.Title.Label.FontSize = 16;
            plot.Axes.Title.Label.OffsetY = 20;
        };

        skControl.Dock = DockStyle.Fill;

        skControl.Invalidate();
    }

    private void InitializeTable()
    {
        table.AutoSize = true;
        Controls.Add(table);
    }

    private void InitializeLabel()
    {
        Font LargeFont = new Font("Courier New", 12);
        
        infoLabel.Font = LargeFont;
        Controls.Add(infoLabel);

        AddComponentNames();
        UpdateData();
    }

    private async void AddComponentNames()
    {
        await Task.Run(() =>
        {
            rm.Monitor();
            string cpuName = rm.CPU_NAME ?? "ERR";
            string gpuName = rm.GPU_NAME ?? "ERR";

            // UI Thread
            BeginInvoke((MethodInvoker)(() =>
            {
                System.Windows.Forms.Label nameLabel = new System.Windows.Forms.Label();
                Font LargeFont = new Font("Courier New", 12);
                nameLabel.Font = LargeFont;
                nameLabel.Text = "GPU name: " + gpuName + "\r\n" + "CPU name: " + cpuName;
                nameLabel.AutoSize = true;
                Controls.Add(nameLabel);
                table.Controls.Add(nameLabel, 1, 0);
            }));
        });
    }

    private void InitializePlot(ScottPlot.Plot plot, SKControl skControl){
        
        skControl.AutoSize = true;
        skControl.Height = 300;
        skControl.Width = 500;
        Controls.Add(skControl);

        plot.Axes.SetLimitsY(0, 100);
        plot.HideGrid();
        plot.Style.Background(figure: ScottPlot.Color.FromHex("f0f0f0"), data: ScottPlot.Color.FromHex("#f0f0f0"));
        plot.Axes.Title.Label.Bold = false;
        
        ScottPlot.TickGenerators.NumericManual ticks = new();
        plot.Axes.Bottom.TickGenerator = ticks;

    }
}
