namespace PCInsight;

using System.Windows.Forms;


public partial class LoadingForm : Form
{
    public LoadingForm()
    {
        InitializeComponent();
        // Set the properties for the loading screen here
        this.FormBorderStyle = FormBorderStyle.None;
        this.StartPosition = FormStartPosition.CenterScreen;
        Label loadingLabel = new Label
        {
            Text = "Loading...",
            AutoSize = true,
            Font = new Font("Arial", 16),
            Location = new Point(50, 50) // Adjust the position as needed
        };
        Controls.Add(loadingLabel);
    }
}
