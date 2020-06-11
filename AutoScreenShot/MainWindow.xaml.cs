using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;
using AutoUpdaterDotNET;
using System.Diagnostics;

namespace AutoScreenShot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            //Check for Update program
            string updaterPath = Path.Combine(Environment.CurrentDirectory, "AutoScreenshotUpdater.exe");
            if (File.Exists(updaterPath))
            {
                Console.WriteLine("Updater Found");
                Process.Start(updaterPath);
                this.Close();
            }

            //AutoUpdate Checker (https://github.com/ravibpatel/AutoUpdater.NET)
            AutoUpdater.ShowSkipButton = false;
            AutoUpdater.ShowRemindLaterButton = false;
            AutoUpdater.DownloadPath = Environment.CurrentDirectory;
            //Environment.SpecialFolder.
            //var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
            //if (currentDirectory.Parent != null)
            //{
            //    AutoUpdater.InstallationPath = currentDirectory.Parent.FullName;
            //}
            //AutoUpdater.ReportErrors = true;
            AutoUpdater.Start("https://raw.githubusercontent.com/who8e1/AutoScreenShot/master/LastestUpdate.xml");

            //Other
            ScreenShotAmount = 0;
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1); // Tick every 1 Second
        }

        public void ScreenShot(string fileName, string fileLocation)
        {
            double screenLeft = SystemParameters.VirtualScreenLeft;
            double screenTop = SystemParameters.VirtualScreenTop;
            double screenWidth = SystemParameters.VirtualScreenWidth;
            double screenHeight = SystemParameters.VirtualScreenHeight;
            double Opacity = 0.0;

            using (Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                Screen.PrimaryScreen.Bounds.Height))  //(int)screenWidth, (int)screenHeight
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    //String filename = fileName + "_" + DateTime.Now.ToString("dd.MM.yyyy-hh.mm.ss") + ".png";   //Flipping the date around so that win Explorer sorts images in order or time taken
                    string filename = DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss") + "_" + fileName + ".png";
                    Opacity = .0;
                    g.CopyFromScreen(0, 0, 0, 0, bmp.Size);//(int)screenLeft, (int)screenTop,
                    bmp.Save(Path.Combine(fileLocation, filename));
                    Opacity = 1;
                }

            }

            //String nme = "";
            //Bitmap printscreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
            //                                Screen.PrimaryScreen.Bounds.Height);
            //Graphics graphics = Graphics.FromImage(printscreen as System.Drawing.Image);
            //graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);
            //nme = DateTime.Now.ToString();
            //printscreen.Save(@"F:\Temp\printScre.jpg", ImageFormat.Jpeg);


        }

        private void ManualScreenShotBtn_Click(object sender, RoutedEventArgs e)
        {
            if (saveLocationTxtBx.Text != string.Empty)
            {
                if (Directory.Exists(saveLocationTxtBx.Text))
                {
                    if (fileNameTxt.Text != string.Empty)
                    {
                        ScreenShot(fileNameTxt.Text, saveLocationTxtBx.Text);
                        ScreenShotAmount += 1;
                        SsAmount.Text = ScreenShotAmount.ToString();
                    }
                    else
                    {
                        fileNameTxt.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Save Location Invalid", "Invalid Directory", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Save Location Invalid", "Invalid Directory", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }


        
        //DispatcherTimer UiTimer = new DispatcherTimer();
        bool Enabled = false;
        private void AutoScreenShotBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Enabled)
            {
                Enabled = false;
                //UiTimer.Stop();
                dispatcherTimer.Stop();
                timeMins.IsEnabled = true;
                timeSecs.IsEnabled = true;
                fileNameTxt.IsEnabled = true;
                saveLocationTxtBx.IsEnabled = true;
                Browse.IsEnabled = true;
                AutoScreenShotBtn.Content = "Start Auto Screenshots";
                AutoScreenShotBtn.Style = (Style)FindResource("StartButton");
            }
            else
            { 
                int resultOut;
                if (saveLocationTxtBx.Text != string.Empty)
                {
                    if (Directory.Exists(saveLocationTxtBx.Text))
                    {
                        if (fileNameTxt.Text != string.Empty)
                        {
                            if (int.TryParse(timeMins.Text, out resultOut))
                            {
                                //Worked
                                if (resultOut >= 0 && resultOut < 30)
                                {
                                    int mins = resultOut;
                                    if (int.TryParse(timeSecs.Text, out resultOut))
                                    {
                                        Console.WriteLine(resultOut.ToString());
                                        if(resultOut >= 0 )
                                        {
                                            //Worked
                                            int secs = resultOut;

                                            //UiTimer
                                            SetMins = mins;
                                            SetSecs = secs;
                                            //UiTimer.Tick += UiTimer_Tick;
                                            //UiTimer.Interval = new TimeSpan(0, 0, 1);
                                            TotalSecs = (SetMins * 60) + SetSecs;
                                            dispatcherTimer.Start();
                                            //UiTimer.Start();

                                            timeMins.IsEnabled = false;
                                            timeSecs.IsEnabled = false;
                                            fileNameTxt.IsEnabled = false;
                                            saveLocationTxtBx.IsEnabled = false;
                                            Browse.IsEnabled = false;

                                            AutoScreenShotBtn.Content = "Stop Auto ScreenShots";
                                            AutoScreenShotBtn.Style = (Style)FindResource("StopButton");
                                            Enabled = true;
                                        }
                                        else
                                        {
                                            timeSecs.Focus();
                                            timeSecs.SelectAll();
                                            MessageBox.Show("Second Value is Out of Range \n It needs to greater then 0", "Value Invalid", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                                        }

                                    }
                                    else
                                    {
                                        timeSecs.Focus();
                                        timeSecs.SelectAll();
                                    }
                                }
                                else
                                {
                                    timeMins.Focus();
                                    timeMins.SelectAll();
                                    MessageBox.Show("Minute Value is Out of Range \n It needs to between 0-30", "Value Invalid", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                }
                            }
                            else
                            {
                                timeMins.Focus();
                                timeMins.SelectAll();
                            }
                        }
                        else
                        {
                            fileNameTxt.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Save Location Invalid", "Invalid Directory", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
                else
                {
                    MessageBox.Show("Save Location Invalid", "Invalid Directory", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        private void ResetAllBtn_Click(object sender, RoutedEventArgs e)
        {
            //resets all Fields
            saveLocationTxtBx.Text = string.Empty;
            ScreenShotAmount = 0;
            SsAmount.Text = ScreenShotAmount.ToString();
            timeMins.Text = string.Empty;
            timeSecs.Text = string.Empty;
            fileNameTxt.Text = string.Empty;

            dispatcherTimer.Stop();
            timeMins.IsEnabled = true;
            timeSecs.IsEnabled = true;
            fileNameTxt.IsEnabled = true;
            saveLocationTxtBx.IsEnabled = true;
            Browse.IsEnabled = true;
            AutoScreenShotBtn.Content = "Start Auto Screenshots";
            AutoScreenShotBtn.Style = (Style)FindResource("StartButton");

        }

        private void ResetCountBtn_Click(object sender, RoutedEventArgs e)
        {
            ScreenShotAmount = 0;
            SsAmount.Text = ScreenShotAmount.ToString();
        }

        int TotalSecs = 0;
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //UI Time
            decimal x = TotalSecs / 60;
            SstMins = (int)Math.Truncate(x);
            SstSecs = TotalSecs - (SstMins * 60);
            SsTimer.Text = ScreenShotTime;

            //The check if at 0 secs
            if (TotalSecs == 0)
            {
                ScreenShot(fileNameTxt.Text, saveLocationTxtBx.Text);
                ScreenShotAmount += 1;
                SsAmount.Text = ScreenShotAmount.ToString();
                TotalSecs = (SetMins * 60) + SetSecs;
            }
            else
            {
                TotalSecs -= 1;
            }
        }
        int secsPassed = 0;
        private void UiTimer_Tick(object sender, EventArgs e)
        {
            secsPassed += 1;
            if (SstMins > 0 || SstSecs > 0)
            {
                //if ((secsPassed % 60) == 0)
                //{
                //    //Minute has Passed
                //    SstMins -= 1;
                //}
                //else
                //{
                //    SstSecs -= 1;
                //}

                if(SstSecs == 0 && SstMins > 0)
                {
                    //Min
                    SstMins -= 1;
                    SstSecs = 60;
                }
                else    // if(SstSecs > 0)
                {
                    SstSecs -= 1;
                }
                SsTimer.Text = ScreenShotTime;
            }
            else
            {
                //Reset Time
                SstMins = (secsPassed % 60);
                SstSecs = secsPassed - (SstMins * 60);
                secsPassed = 0;
                SsTimer.Text = ScreenShotTime;
            }
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string currentDirectory = Environment.CurrentDirectory;

                Ookii.Dialogs.Wpf.VistaFolderBrowserDialog openFolderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
                openFolderDialog.RootFolder = Environment.SpecialFolder.MyComputer;
                

                if (openFolderDialog.ShowDialog() == true)
                {
                    if (Directory.Exists(openFolderDialog.SelectedPath))
                    {
                        saveLocationTxtBx.Text = openFolderDialog.SelectedPath;
                    }
                    
                    //CsvFilePath = openFileDialog.FileName;
                    //csvFilePath.Text = CsvFilePath;
                    //Console.WriteLine("Selected Folder: {0} ", CsvFilePath);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error Loading CSV File", "Loading Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


            
        }

        private int _screenShotAmount;

        public int ScreenShotAmount
        {
            get { return _screenShotAmount; }
            set { _screenShotAmount = value; }
        }

        private string _screenShotTime;

        public string ScreenShotTime
        {
            get 
            {
                if (SstSecs < 10)
                {
                    return SstMins.ToString() + ":0" + SstSecs.ToString();
                }
                else
                {
                    return SstMins.ToString() + ":" + SstSecs.ToString();
                }
                
                //return _screenShotTime; 
            }
            set { _screenShotTime = value; }
        }

        private int _sstMins;

        public int SstMins
        {
            get { return _sstMins; }
            set 
            { 
                _sstMins = value;
            }
        }

        private int _sstSecs;

        public int SstSecs
        {
            get { return _sstSecs; }
            set { _sstSecs = value; }
        }


        private int _setMins;

        public int SetMins
        {
            get { return _setMins; }
            set
            {
                _setMins = value;
            }
        }

        private int _setSecs;

        public int SetSecs
        {
            get { return _setSecs; }
            set { _setSecs = value; }
        }
        DispatcherTimer ColourTimer = new DispatcherTimer();
        private void Credits_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ColourTimer.Tick += colourTimer_Tick;
            ColourTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            ColourTimer.Start();
        }
        int R, G, B = 0;
        bool Rev = false;
        int add = 1;

        private void colourTimer_Tick(object sender, EventArgs e)
        {
            
            if (Rev) //Reverse
            {
                if (R > 0)
                {
                    R -= add;
                }
                else if (G > 0)
                {
                    G -= add;
                }
                else if (B > 0)
                {
                    B -= add;
                }
                else
                {
                    Rev = false;
                }
            }
            else
            {
                if (R < 255)
                {
                    R += add;
                }
                else if (G < 255)
                {
                    G += add;
                }
                else if (B < 255)
                {
                    B += add;
                }
                else
                {
                    Rev = true;
                }
            }
            Credits.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)R, (byte)G, (byte)B));
        }

        private void Credits_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ColourTimer.Stop();
            Rev = false;
            R = 0;
            G = 0;
            B = 0;
            Credits.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        }
    }
}
