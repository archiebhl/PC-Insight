namespace PCInsight
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        
        // UI Controls
        private System.Windows.Forms.TableLayoutPanel table;
        private SkiaSharp.Views.Desktop.SKControl gpuUsageCanvas;
        private SkiaSharp.Views.Desktop.SKControl gpuTemperatureCanvas;
        private SkiaSharp.Views.Desktop.SKControl cpuUsageCanvas;
        private SkiaSharp.Views.Desktop.SKControl cpuTemperatureCanvas;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.table = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();

            InitializeTableLayoutPanel();

            GroupBox gpuGroupBox = CreateGpuGroupBox();
            GroupBox cpuGroupBox = CreateCpuGroupBox();

            this.table.Controls.Add(gpuGroupBox, 0, 0);
            this.table.Controls.Add(cpuGroupBox, 0, 1);

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 365);
            this.Text = "PCInsight";
            this.Icon = new Icon("logo.ico");
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void InitializeTableLayoutPanel()
        {
            this.table.AutoSize = false;
            this.table.ColumnCount = 1;
            this.table.RowCount = 2;
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.table.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Controls.Add(this.table);
        }

        private GroupBox CreateGpuGroupBox()
        {

            TableLayoutPanel gpuTable = new TableLayoutPanel
            {
                AutoSize = false,
                ColumnCount = 2,
                RowCount = 2,
                Dock = DockStyle.Fill
            };
            gpuTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            gpuTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            

            this.gpuUsageCanvas = new SkiaSharp.Views.Desktop.SKControl { Dock = DockStyle.Fill };
            this.gpuTemperatureCanvas = new SkiaSharp.Views.Desktop.SKControl { Dock = DockStyle.Fill };

            gpuTable.Controls.Add(gpuUsageHeading, 0, 0);
            gpuTable.Controls.Add(gpuTempHeading, 1, 0);
            gpuTable.Controls.Add(this.gpuUsageCanvas, 0, 1);
            gpuTable.Controls.Add(this.gpuTemperatureCanvas, 1, 1);

            gpuGroupBox.Controls.Add(gpuTable);
            return gpuGroupBox;
        }

        private GroupBox CreateCpuGroupBox()
        {
            TableLayoutPanel cpuTable = new TableLayoutPanel
            {
                AutoSize = false,
                ColumnCount = 2,
                RowCount = 2,
                Dock = DockStyle.Fill,
            };
            cpuTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            cpuTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));

            this.cpuUsageCanvas = new SkiaSharp.Views.Desktop.SKControl { Dock = DockStyle.Fill };
            this.cpuTemperatureCanvas = new SkiaSharp.Views.Desktop.SKControl { Dock = DockStyle.Fill };

            cpuTable.Controls.Add(cpuUsageHeading, 0, 0);
            cpuTable.Controls.Add(cpuTempHeading, 1, 0);
            cpuTable.Controls.Add(this.cpuUsageCanvas, 0, 1);
            cpuTable.Controls.Add(this.cpuTemperatureCanvas, 1, 1);

            cpuGroupBox.Controls.Add(cpuTable);
            return cpuGroupBox;
        }

        private GroupBox gpuGroupBox = new GroupBox
        {
            Text = "Gathering GPU information....",
            Dock = DockStyle.Fill
        };
        private GroupBox cpuGroupBox = new GroupBox
        {
            Text = "Gathering CPU information....",
            Dock = DockStyle.Fill
        };

        private System.Windows.Forms.Label cpuUsageHeading = new System.Windows.Forms.Label()
        {
            Text = "CPU Usage",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };
        private System.Windows.Forms.Label cpuTempHeading = new System.Windows.Forms.Label()
        {
            Text = "CPU Temperature",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
        };
        private System.Windows.Forms.Label gpuUsageHeading = new System.Windows.Forms.Label()
        {
            Text = "GPU Usage",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };
        private System.Windows.Forms.Label gpuTempHeading = new System.Windows.Forms.Label()
        {
            Text = "GPU Temperature",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };

    }
}
