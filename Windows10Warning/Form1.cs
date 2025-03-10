using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;

namespace Windows10Warning
{
    public partial class Form1 : Form
    {
        // Paths for dismiss file and config file
        private string dismissedFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Win10NotificationDismissed.txt");

        private string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        private bool ShouldDismiss()
        {
            try
            {
                Debug.WriteLine("Checking if dismissal file exists...");

                if (File.Exists(dismissedFilePath))
                {
                    Debug.WriteLine($"Dismissal file found: {dismissedFilePath}");

                    string savedDate = File.ReadAllText(dismissedFilePath).Trim();
                    Debug.WriteLine($"Dismissal File Content: '{savedDate}'");

                    if (DateTime.TryParseExact(savedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dismissedDate))
                    {
                        Debug.WriteLine($"Parsed Dismissal Date: {dismissedDate:yyyy-MM-dd} - Today's Date: {DateTime.Today:yyyy-MM-dd}");

                        if (dismissedDate.Date == DateTime.Today)
                        {
                            Debug.WriteLine("Dismissal honored. Closing app.");
                            Environment.Exit(0); // ✅ Immediate exit without requiring a form
                            return true;
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Dismissal file has an invalid format. Deleting.");
                        File.Delete(dismissedFilePath);
                    }
                }
                else
                {
                    Debug.WriteLine("No dismissal file found.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking dismissal file: {ex.Message}");
            }

            return false;
        }
        // Config settings structure
        private class ConfigSettings
        {
            public string FormTitle { get; set; }
            public string RedirectUrl { get; set; }
            public string LogoPath { get; set; }
        }

        private ConfigSettings settings;

        public Form1()
        {
            if (ShouldDismiss())
            {
                this.Close();
                Application.Exit();
                return;
            }

            InitializeComponent();
            LoadConfig();
            this.Text = settings.FormTitle; // Set form title from config

            // Ensure the window appears on top
            this.TopMost = true;
            this.Activate();
            BringToFront();
            ForceFocus();
        }
        // Ensure the window gains focus
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        private void NotificationForm_Load(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine($"Checking dismissal file at: {dismissedFilePath}");

                if (File.Exists(dismissedFilePath))
                {
                    string savedDate = File.ReadAllText(dismissedFilePath).Trim();

                    Debug.WriteLine($"Dismissal File Content: '{savedDate}'");

                    if (DateTime.TryParseExact(savedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dismissedDate))
                    {
                        Debug.WriteLine($"Parsed Dismissal Date: {dismissedDate:yyyy-MM-dd} - Today's Date: {DateTime.Today:yyyy-MM-dd}");

                        if (dismissedDate.Date == DateTime.Today)
                        {
                            Debug.WriteLine("Dismissal Date Matches Today. Closing the app.");
                            this.Close();
                            Application.Exit();
                            return;
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Dismissal file format incorrect. Resetting...");
                        File.Delete(dismissedFilePath);
                    }
                }
                else
                {
                    Debug.WriteLine("No dismissal file found. Notification will be shown.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking dismissal file: {ex.Message}");
            }
        }
        private void LoadConfig()
        {
            try
            {
                if (File.Exists(configFilePath))
                {
                    string json = File.ReadAllText(configFilePath);
                    settings = JsonSerializer.Deserialize<ConfigSettings>(json);
                }
                else
                {
                    MessageBox.Show("Config file not found: " + configFilePath);
                    settings = new ConfigSettings
                    {
                        FormTitle = "Default Windows 10 Warning",
                        RedirectUrl = "https://default.url",
                        LogoPath = "logo.png" // Default to a relative file in the same folder
                    };
                }

                // Apply Form Title from config
                this.Text = settings.FormTitle;

                // Resolve full path for the logo if using a relative path
                string logoPath = settings.LogoPath;
                if (!Path.IsPathRooted(logoPath)) // Convert to absolute path if needed
                {
                    logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logoPath);
                }

                Debug.WriteLine($"Resolved Logo Path: {logoPath}");

                // Load Logo if the file exists
                if (File.Exists(logoPath))
                {
                    try
                    {
                        pictureBox1.Load(logoPath); // Use Load() instead of Image.FromFile()
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to load logo: " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show($"Logo file not found at: {logoPath}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading config: " + ex.Message);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Open only the URL from the config file
                if (!string.IsNullOrEmpty(settings.RedirectUrl))
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = settings.RedirectUrl,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
                else
                {
                    MessageBox.Show("No URL defined in config file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open the link. " + ex.Message);
            }
        }
        private void ForceFocus()
        {
            this.WindowState = FormWindowState.Minimized;
            this.WindowState = FormWindowState.Normal;
            SetForegroundWindow(this.Handle);
        }
        private void Button3_Click(object sender, EventArgs e)
        {
            try
            {
                string todayDate = DateTime.Today.ToString("yyyy-MM-dd");
                // Write the dismissal date to the file, ensuring a clean write
                File.WriteAllText(dismissedFilePath, todayDate);
                Debug.WriteLine($"Dismissal file written: {todayDate}");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error dismissing notification: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"Error writing dismissal file: {ex.Message}");
            }
        }
        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}