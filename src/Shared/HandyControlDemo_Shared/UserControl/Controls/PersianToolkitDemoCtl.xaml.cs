using HandyControl.Controls;
using Microsoft.Win32;
using System;
using System.IO;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MessageBox = HandyControl.Controls.MessageBox;

namespace HandyControlDemo.UserControl
{
    public sealed partial class PersianToolkitDemoCtl
    {
        public PersianToolkitDemoCtl()
        {
            this.InitializeComponent();

            #region NeonLabel
            _dispatcherTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
            _dispatcherTimer.Tick += DispatcherTimer_Tick;
            _dispatcherTimer.Start();
            #endregion
        }

        #region NeonLabel
        private DispatcherTimer _dispatcherTimer;
        private int count = 0;

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
           if(tab.SelectedIndex == 5)
            {
                count++;
                switch (count)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        break;
                    case 5:
                        count = 0;
                        break;
                }
                neon2.Next(NeonLabelType.FadeNext, Guid.NewGuid().ToString());
                neon3.Next(NeonLabelType.SlideNext, Guid.NewGuid().ToString());
                if (count == 1)
                    neon.Next(NeonLabelType.ScrollToEnd, null, 4);
            }
        }
        #endregion

        #region RegistryHelper
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var btn = sender as Button;
            switch (btn.Tag)
            {
                case "btnAddReg":
                    RegistryHelper.AddOrUpdateKey("myKey", "myFolder", false);
                    RegistryHelper.AddOrUpdateKey("myKey2", "myFolder", 1372);
                    RegistryHelper.AddOrUpdateKey("myKey3", "myFolder", "test");
                    //RegistryHelper.AddOrUpdateRegistryKey("myKey4", "myFolder", "test", HKEYType.LocalMachine);
                    break;
                case "btnGetReg":
                    MessageBox.Info(RegistryHelper.GetKey<bool>("myKey", "myFolder").ToString());
                    MessageBox.Info(RegistryHelper.GetKey<int>("myKey2", "myFolder").ToString());
                    MessageBox.Info(RegistryHelper.GetKey<string>("myKey3", "myFolder").ToString());
                    //MessageBox.Info(RegistryHelper.GetRegistryKey<string>("myKey4", "myFolder", HKEYType.LocalMachine).ToString());
                    break;
                case "btnDelReg":
                    RegistryHelper.DeleteKey("myKey", "myFolder", false);
                    RegistryHelper.DeleteKey("myKey2", "myFolder");
                    RegistryHelper.DeleteKey("myKey3", "myFolder");
                    //RegistryHelper.DeleteRegistryKey("myKey4", "myFolder",false, HKEYType.LocalMachine);
                    break;
            }
        }
        #endregion

        #region SpeedoMeter
        public void startSpeedoMeter1()
        {
            for (int i = 0; i < 181; i++)
            {
                sp.Value = i;
                sld.Value = i;
                Thread.Sleep(30);

                if (i == 180)
                {
                    while (i > 0)
                    {
                        i--;
                        sp.Value = i;
                        sld.Value = i;

                        Thread.Sleep(30);
                        if (i == 0)
                            return;
                    }

                }
            }
        }
        public void startSpeedoMeter2()
        {
            for (int i = 0; i < 121; i++)
            {
                sp2.Value = i;
                sld2.Value = i;
                Thread.Sleep(30);

                if (i == 120)
                {
                    while (i > 0)
                    {
                        i--;
                        sp2.Value = i;
                        sld2.Value = i;

                        Thread.Sleep(30);
                        if (i == 0)
                            return;
                    }

                }
            }
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            startSpeedoMeter1();
            startSpeedoMeter2();
        }
#endregion

#region Encryption
        private void BtnEnText_Click(object sender, RoutedEventArgs e)
        {
           txtEn.Text = CryptographyHelper.EncryptTextAES(txtEnText.Text, txtEnTextPass.Text);
        }

        private void BtnDeText_Click(object sender, RoutedEventArgs e)
        {
            txtDe.Text = CryptographyHelper.DecryptTextAES(txtDeText.Text, txtEnTextPass.Text);
        }

        string globalFileName = string.Empty;
        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var dialog = new OpenFileDialog();
            dialog.Title = "Open File";
            if (dialog.ShowDialog() == true)
            {
                if (btn.Tag.Equals("enc"))
                {
                    txtEnBrowse.Text = dialog.FileName;
                }
                else
                {
                    txtDeBrowse.Text = dialog.FileName;

                }
                globalFileName = Path.GetExtension(dialog.FileName); ;
            }
           
        }

        private void btnEnFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.Title = "Save File";
            if (dialog.ShowDialog() == true)
            {
                Console.WriteLine(dialog.SafeFileName);
                CryptographyHelper.EncryptFileAES(txtEnBrowse.Text, dialog.FileName + globalFileName, txtEnFilePass.Text);
            }

        }

        private void btnDeFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.Title = "Save File";
            if (dialog.ShowDialog() == true)
            {
                CryptographyHelper.DecryptFileAES(txtDeBrowse.Text, dialog.FileName + globalFileName, txtDeFilePass.Text);
            }
        }

        private void btnGenerateHash_Click(object sender, RoutedEventArgs e)
        {
            txtmd5.Text = CryptographyHelper.GenerateMD5(txtHash.Text);
            txtsha.Text = CryptographyHelper.GenerateSHA256(txtHash.Text);
        }
#endregion

#region InIHelper
        private void btnIniHelper_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            switch (btn.Tag)
            {
                case "add":
                    InIHelper.AddValue("key1", "test1");
                    InIHelper.AddValue("key2", "test2");
                    InIHelper.AddValue("key3", "test3", "mySection");
                    InIHelper.AddValue("file4", "test4", "mySection");
                    InIHelper.AddValue("file5", "test5", "mySection");
                    //InIHelper.AddValue("file6", "test6", "mySection2", @"D:\config.ini");
                    break;
                case "read":
                    MessageBox.Info(InIHelper.ReadValue("key1"));
                    MessageBox.Info(InIHelper.ReadValue("key3", "mySection"));
                    //MessageBox.Info(InIHelper.ReadValue("key3", "mySection", @"D:\config.ini"));
                    break;
                case "delete":
                    InIHelper.DeleteKey("key4", "mySection");
                    //InIHelper.DeleteSection("mySection");
                    //InIHelper.DeleteKey("key4", "mySection", @"D:\config.ini");
                    break;
                case "exist":
                    MessageBox.Info(InIHelper.IsKeyExists("key4", "mySection") + "");
                    break;
            }
        }
#endregion

        private void btnAppHost_Click(object sender, RoutedEventArgs e)
        {
            new AppHostWindow().ShowDialog();
        }

        private void btnCheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            var isExist = UpdateHelper.IsNewVersionExist("https://raw.githubusercontent.com/ghost1372/HandyControls/develop/Updater.xml");
            if(isExist)
            {
                Growl.InfoGlobal("New Version Found!");
                lblUrl.Content = UpdateHelper.URL;
                txtChangelog.Text = UpdateHelper.ChangeLog;
            }
            else
            {
                Growl.ErrorGlobal("you are using latest version");
                lblUrl.Content = string.Empty;
                txtChangelog.Text = string.Empty;
            }
        }
    }
}
