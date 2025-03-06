using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Windows10Warning
{
    public partial class Form1: Form
    {
        // File path in AppData (you can adjust the location as needed)
        private string dismissedFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Win10NotificationDismissed.txt");
        public Form1()
        {
            InitializeComponent();
        }
        private void NotificationForm_Load(object sender, EventArgs e)
        {
            if (File.Exists(dismissedFilePath))
            {
                string savedDate = File.ReadAllText(dismissedFilePath);
                DateTime dismissedDate;
                if (DateTime.TryParse(savedDate, out dismissedDate))
                {
                    if (dismissedDate.Date == DateTime.Today)
                    {
                        // The notification was dismissed today—do not show it.
                        this.Close();
                    }
                }
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            File.WriteAllText(dismissedFilePath, DateTime.Today.ToString("yyyy-MM-dd"));
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Opens the URL in the default browser
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "https://cnu.helpspot.com/index.php?pg=request&xCategory=65",
                    UseShellExecute = true
                };
                Process.Start(psi);
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open the link. " + ex.Message);
            }
        }
    }
}
