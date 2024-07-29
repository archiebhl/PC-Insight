using System.Diagnostics.Tracing;

namespace client
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
        private Button startButton;
        private Button pauseButton;
        private MenuStrip menuStrip;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.table = new System.Windows.Forms.TableLayoutPanel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.SuspendLayout();
            
            InitializeMenuStrip();
            InitializeTableLayoutPanel();

            GroupBox gpuGroupBox = CreateGpuGroupBox();
            GroupBox cpuGroupBox = CreateCpuGroupBox();

            this.table.Controls.Add(gpuGroupBox, 0, 0);
            this.table.Controls.Add(cpuGroupBox, 0, 1);

            // add buttons
            CreateButtonsGroupBox();

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Text = "PCInsight";
            this.Icon = new Icon("assets/logo.ico");
            this.Controls.Add(this.menuStrip);
            this.ResumeLayout(false);
            this.PerformLayout();
            dataGrid.SelectionChanged += datagridview_UserInteraction; // removing selection highlights from datagridview
        }

        private void InitializeMenuStrip()
        {
            this.menuStrip.Items.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem("File", null, new ToolStripMenuItem[]
                {
                    new ToolStripMenuItem("Save", null, this.SaveMenuItem_Click),
                    new ToolStripMenuItem("Upload", null, this.UploadMenuItem_Click),
                    new ToolStripMenuItem("Exit", null, this.ExitMenuItem_Click)

                }),
                new ToolStripMenuItem("Help", null, new ToolStripMenuItem[]
                {
                    new ToolStripMenuItem("About", null, this.AboutMenuItem_Click)
                })
            });
            this.menuStrip.Dock = DockStyle.Top;
        }
        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("PCInsight v1.0", "About PCInsight", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            
            MessageBox.Show("Pending Feature", "Save data locally", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private async void UploadMenuItem_Click(object sender, EventArgs e)
        {
            await rm.SendToServer();
            MessageBox.Show("Data uploaded to the server", "Upload data", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void InitializeTableLayoutPanel()
        {
            this.table.AutoSize = false;
            this.table.ColumnCount = 1;
            this.table.RowCount = 4;
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.table.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Controls.Add(this.table);
        }

        private void CreateButtonsGroupBox() // methods in form1.cs
        {
            this.startButton = new System.Windows.Forms.Button();
            this.startButton.Text = "Start";
            this.startButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.startButton.Click += new System.EventHandler(this.StartButton_Click);

            this.pauseButton = new System.Windows.Forms.Button();
            this.pauseButton.Text = "Pause";
            this.pauseButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pauseButton.Click += new System.EventHandler(PauseButton_Click);

            TableLayoutPanel buttonsTable = new TableLayoutPanel
            {
                AutoSize = false,
                ColumnCount = 2,
                RowCount = 1,
                Dock = DockStyle.Fill
            };
            buttonsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            buttonsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            buttonsTable.Controls.Add(this.startButton, 0, 0);
            buttonsTable.Controls.Add(this.pauseButton, 1, 0);

            buttonsGroupBox.Controls.Add(buttonsTable);
            this.table.Controls.Add(buttonsGroupBox, 0, 2);
        }
        private GroupBox buttonsGroupBox = new GroupBox
        {
            Text = "Monitoring options",
            Dock = DockStyle.Fill
        };

        private GroupBox CreateGpuGroupBox()
        {

            TableLayoutPanel gpuTable = new TableLayoutPanel
            {
                AutoSize = false,
                ColumnCount = 3,
                RowCount = 1,
                Dock = DockStyle.Fill
            };
            gpuTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            gpuTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            gpuTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            
            this.gpuUsageCanvas = new SkiaSharp.Views.Desktop.SKControl { Dock = DockStyle.Fill };
            this.gpuTemperatureCanvas = new SkiaSharp.Views.Desktop.SKControl { Dock = DockStyle.Fill };

            dataGrid.Columns[0].Name = "Sensor";
            dataGrid.Columns[1].Name = "Value";
            dataGrid.Rows[0].Cells[0].Value = "VRAM Used";
            dataGrid.Rows[0].Cells[1].Value = "Value 1-2";
            dataGrid.Rows[1].Cells[0].Value = "Core Clock";
            dataGrid.Rows[1].Cells[1].Value = "Value 2-2";
            dataGrid.Rows[2].Cells[0].Value = "Memory Clock";
            dataGrid.Rows[2].Cells[1].Value = "Value 3-2";

            gpuTable.Controls.Add(dataGrid, 2, 0);
            //gpuTable.Controls.Add(gpuUsageHeading, 0, 0);
            //gpuTable.Controls.Add(gpuTempHeading, 1, 0);
            gpuTable.Controls.Add(this.gpuUsageCanvas, 0, 0);
            gpuTable.Controls.Add(this.gpuTemperatureCanvas, 1, 0);

            gpuGroupBox.Controls.Add(gpuTable);

            return gpuGroupBox;
        }
        Label overlayLabel = new Label
        {
            Text = "Overlay Label",
            BackColor = Color.FromArgb(0, Color.Yellow), // Semi-transparent yellow background
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
        };

        public DataGridView dataGrid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 3,
            ReadOnly = true, // Prevent editing
            AutoSize = true,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
            AllowUserToResizeColumns = false, 
            AllowUserToResizeRows = false, 
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
            CellBorderStyle = DataGridViewCellBorderStyle.Single, 
            BorderStyle = BorderStyle.None,
            BackgroundColor = Color.FromArgb(255, 240, 240, 240),
            DefaultCellStyle = { BackColor = Color.FromArgb(255, 240, 240, 240) },
            EnableHeadersVisualStyles = false,
            ColumnHeadersDefaultCellStyle = { BackColor = Color.FromArgb(255, 240, 240, 240) },
         };
        private void datagridview_UserInteraction(object sender, EventArgs e)
        {
            dataGrid.ClearSelection();
            for (int i = 0; i < dataGrid.ColumnCount; i++)
            {
                dataGrid.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
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
            cpuTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            cpuTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            cpuTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));

            this.cpuUsageCanvas = new SkiaSharp.Views.Desktop.SKControl { Dock = DockStyle.Fill };
            this.cpuTemperatureCanvas = new SkiaSharp.Views.Desktop.SKControl { Dock = DockStyle.Fill };
            
            //cpuTable.Controls.Add(cpuUsageHeading, 0, 0);
            //cpuTable.Controls.Add(cpuTempHeading, 1, 0);
            cpuTable.Controls.Add(this.cpuUsageCanvas, 0, 1);
            cpuTable.Controls.Add(this.cpuTemperatureCanvas, 1, 1);

            cpuGroupBox.Controls.Add(cpuTable);
            return cpuGroupBox;
        }

        private GroupBox gpuGroupBox = new GroupBox
        {
            Text = "Gathering GPU information....",
            Dock = DockStyle.Fill,
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
