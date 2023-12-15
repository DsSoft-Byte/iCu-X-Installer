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
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
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

            string icuresFolder = @"C:\iCures";
            if (Directory.Exists(icuresFolder))
            {
                button1.Enabled = false; // Disable the install button
                //button1.Text = ("iCu X already installed");
            }
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
            button1.Enabled = true;
        }

        private void DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //button1.Enabled = true; // Enable the button after download completes
            if (e.Error == null)
            {
                try
                {
                    // Extract the contents of the ZIP file directly to C:\
                    System.IO.Compression.ZipFile.ExtractToDirectory(zipFile, extractPath);

                    if (checkBox2.Checked)
                    {
                        CreateDesktopShortcut();
                    }

                    if (checkBox1.Checked)
                    {
                        // Add C:\iCures\MainProgramm\ to the system PATH environment variable
                        string newPath = Path.Combine(extractPath, "iCures\\MainProgramm\\");
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

                DeleteDesktopShortcut();

                MessageBox.Show("Uninstallation completed successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during uninstallation: " + ex.Message);
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Installer Version V1.0.5, Built on our DsSoft Liberty3 Installer framework. Feel free to take this Installer," +
                " change it according to your needs and use it for your own projects. Credit me or DsSoft then though.");
        }
        private void CreateDesktopShortcut()
        {
            IShellLink link = (IShellLink)new ShellLink();

            // setup shortcut information
            link.SetDescription("This is the description when hovered over.");
            link.SetPath(@"C:\iCures\MainProgramm\iCu X.exe");

            // save it
            IPersistFile file = (IPersistFile)link;
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            file.Save(Path.Combine(desktopPath, "iCu X.lnk"), false);
        }
        //This could always be better adjusted to take more input instead of just being set.
        //Then outside of your class but in your namespace
        [ComImport]
        [Guid("00021401-0000-0000-C000-000000000046")]
        internal class ShellLink
        {
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        internal interface IShellLink
        {
            void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);
            void GetIDList(out IntPtr ppidl);
            void SetIDList(IntPtr pidl);
            void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
            void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
            void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
            void GetHotkey(out short pwHotkey);
            void SetHotkey(short wHotkey);
            void GetShowCmd(out int piShowCmd);
            void SetShowCmd(int iShowCmd);
            void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
            void Resolve(IntPtr hwnd, int fFlags);
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }

        private void DeleteDesktopShortcut()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string shortcutPath = Path.Combine(desktopPath, "iCu X.lnk");

            try
            {
                if (File.Exists(shortcutPath))
                {
                    File.Delete(shortcutPath);
                    //MessageBox.Show("Desktop shortcut deleted successfully!");
                }
                else
                {
                    //MessageBox.Show("Desktop shortcut does not exist.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting desktop shortcut: " + ex.Message);
            }
        }
    }
}
