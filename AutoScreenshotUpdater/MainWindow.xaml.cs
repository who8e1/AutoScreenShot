using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Data.SqlTypes;
using Path = System.IO.Path;
using System.Diagnostics;

namespace AutoScreenshotUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        bool finish = false;
        private void Update_Btn_Click(object sender, RoutedEventArgs e)
        {
            if(finish == true)
            {
                Console.WriteLine("Hello");
                Process.Start(new ProcessStartInfo()
                {
                    Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" + "AutoScreenshotUpdater.exe" + "\"",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    FileName = "cmd.exe"
                });
                Process.Start(Path.Combine(Environment.CurrentDirectory, "AutoScreenShot.exe"));
                this.Close();
            }
            else
            {
                string PatchZipName = "AutoScreenshot_v1.0.2-Patch.zip";
                log("Patch.Zip Name: " + PatchZipName);
                string PatchZipPath = Path.Combine(Environment.CurrentDirectory, PatchZipName);
                log("Patch.Zip Path: " + PatchZipPath);
                string PatchName = "Patch-1.0.2";
                log("Patch Name: " + PatchName);
                string PatchPath = Path.Combine(Environment.CurrentDirectory, PatchName);
                log("Patch Path: " + PatchPath);
                string fileName = string.Empty;
                string destFile = string.Empty;
                int fAmount = Directory.GetFiles(PatchPath).Length;
                log("Amount of Patch Files: " + fAmount.ToString());
                UpdateProgress.Maximum = fAmount;
                int fDone = 0;
                try
                {
                    log("Start File Copying");
                    foreach (string file in Directory.GetFiles(PatchPath))
                    {
                        log("Copying File: " + fDone.ToString());
                        // Use static Path methods to extract only the file name from the path.
                        fileName = Path.GetFileName(file);
                        log("File Name: " + fileName);
                        destFile = Path.Combine(Environment.CurrentDirectory, fileName);
                        log("Destination: " + destFile);
                        File.Copy(file, destFile, true);
                        log("Copy Complete");
                        fDone += 1;
                        UpdateProgress.Value = fDone;
                        log("-------------------------------");
                    }
                    log("Deleting: " + PatchPath);
                    Console.WriteLine(PatchPath);
                    Directory.Delete(PatchPath, true);
                    log("Deleted");

                    log("Deleting: " + PatchZipPath);
                    File.Delete(PatchZipPath);
                    log("Deleted");
                    Update_Btn.Content = "Close Updater";
                    Update_Btn.Foreground = Brushes.Green;
                    finish = true;
                    log("--- Update Patch Complete ---");
                }
                catch (Exception exc)
                {
                    MessageBox.Show("An Error has occured, Check the PatchFolder was Extracted Correctly \n " + exc, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    //throw;
                }
                
            }
            
        }

        public void log(string logText)
        {
            LogTxtBx.Text += string.Format("\n {0}", logText);
            LogTxtBx.ScrollToEnd();
        }
    }
}
