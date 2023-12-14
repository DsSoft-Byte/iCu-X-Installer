using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;

namespace Liberty3Inst
{
    public partial class Form1 : Form
    {
        WebClient webClient = new WebClient();
        string zipFile = @"C:\iCuPlus.zip"; // Path to save the downloaded zip file
        string extractPath = @"C:\";
        public Form1()
        {
            InitializeComponent();
            webClient.DownloadProgressChanged += DownloadProgressChanged;
            webClient.DownloadFileCompleted += DownloadCompleted;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            // Update progress bar or any visual indicator of download progress
            progressBar1.Value = e.ProgressPercentage;
        }



        private void button1_Click(object sender, EventArgs e)
        {
            DownloadAndInstallProgram();
        }
        private void DownloadAndInstallProgram()
        {
            try
            {
                webClient.DownloadFileAsync(new Uri("https://raw.githubusercontent.com/DsSoft-Byte/iCu-X/main/iCures.zip"), zipFile);
                button1.Enabled = false; // Disable the button during download
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage; // Update progress bar with download progress
        }



        private void button2_Click(object sender, EventArgs e)
        {
            UninstallProgram();
        }

        private void DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            button1.Enabled = true; // Enable the button after download completes
            if (e.Error == null)
            {
                try
                {
                    // Extract the contents of the ZIP file directly to C:\
                    System.IO.Compression.ZipFile.ExtractToDirectory(zipFile, extractPath);

                    if (checkBox1.Checked)
                    {
                        // Add C:\iCures\MainProgramm\ to the system PATH environment variable
                        string newPath = Path.Combine(extractPath, "iCures\\MainProgramm");
                        string pathVariable = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);

                        if (!pathVariable.Contains(newPath))
                        {
                            pathVariable += ";" + newPath;
                            Environment.SetEnvironmentVariable("PATH", pathVariable, EnvironmentVariableTarget.Machine);
                        }
                    }

                    MessageBox.Show("Installation completed successfully!");
                    File.Delete(zipFile);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error installing: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Error downloading file: " + e.Error.Message);
            }
        }
        private void UninstallProgram()
        {
            string icuresFolder = @"C:\iCures";
            string zipFile = @"C:\iCuPlus.zip";

            try
            {
                // Check if the iCures folder exists before attempting deletion
                if (Directory.Exists(icuresFolder))
                {
                    // Delete the iCures folder and its contents recursively
                    Directory.Delete(icuresFolder, true);
                }

                // Check if the iCuPlus.zip file exists before attempting deletion
                if (File.Exists(zipFile))
                {
                    // Delete the iCuPlus.zip file
                    File.Delete(zipFile);
                }

                MessageBox.Show("Uninstallation completed successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during uninstallation: " + ex.Message);
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Installer Version V1.0.3, Built on our DsSoft Liberty3 Installer framework. Feel free to take this Installer" +
                "change it according to your needs and use it for your own projects. Credit me or DsSoft then though.");
        }
    }
}
