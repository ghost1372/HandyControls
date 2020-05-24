using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Tools;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using MessageBox = HandyControl.Controls.MessageBox;

namespace HandyControlDemo.UserControl
{
    public sealed partial class PersianToolkitDemoCtl
    {
        public PersianToolkitDemoCtl()
        {
            this.InitializeComponent();

            LoadGithubTimeLineItem();
            initDate();
            initCheckTreeView();
        }

        #region GithubTimeLine
        ObservableCollection<GithubTimeLine> data = new ObservableCollection<GithubTimeLine>();

        private void LoadGithubTimeLineItem()
        {
            var item = new GithubTimeLine() { TitleLabel = "2.2.4", TitleInfo = "December 11th 2019", TitleStyle = ResourceHelper.GetResource<Style>(ResourceToken.LabelViolet) };
            item.Members.Add(new ContentMember() { ContentTitle = "FIXED", ContentInfo = "Warn when committing to a protected branch", ContentStyle = ResourceHelper.GetResource<Style>(ResourceToken.LabelSuccess) });
            item.Members.Add(new ContentMember() { ContentTitle = "ADDED", ContentInfo = "Warn when committing to a repository you don't have write access to", ContentStyle = ResourceHelper.GetResource<Style>(ResourceToken.LabelSuccess) });
            item.Members.Add(new ContentMember() { ContentTitle = "IMPROVED", ContentInfo = "Adding integration for Xcode as external editor", ContentStyle = ResourceHelper.GetResource<Style>(ResourceToken.LabelPrimary) });
            data.Add(item);

            var item2 = new GithubTimeLine() { TitleLabel = "2.2.3", TitleInfo = "November 7th 2019", TitleStyle = ResourceHelper.GetResource<Style>(ResourceToken.LabelInfo) };
            item2.Members.Add(new ContentMember() { ContentTitle = "IMPROVED", ContentInfo = "Update embedded Git to address security vulnerabilities", ContentStyle = ResourceHelper.GetResource<Style>(ResourceToken.LabelSuccess) });
            item2.Members.Add(new ContentMember() { ContentTitle = "IMPROVED", ContentInfo = "Changed wording to match git autocrlf behavior", ContentStyle = ResourceHelper.GetResource<Style>(ResourceToken.LabelSuccess) });
            data.Add(item2);
            gitTime.ItemsSource = data;
        }
        private void btnAddGit_Click(object sender, RoutedEventArgs e)
        {
            var item = new GithubTimeLine() { TitleLabel = "3.1.0", TitleInfo = "Aug 11th 2018", TitleStyle = ResourceHelper.GetResource<Style>(ResourceToken.LabelViolet) };
            item.Members.Add(new ContentMember() { ContentTitle = "FIXED", ContentInfo = "Warn when committing to a protected branch", ContentStyle = ResourceHelper.GetResource<Style>(ResourceToken.LabelSuccess) });
            item.Members.Add(new ContentMember() { ContentTitle = "ADDED", ContentInfo = "Warn when committing to a repository you don't have write access to", ContentStyle = ResourceHelper.GetResource<Style>(ResourceToken.LabelSuccess) });
            item.Members.Add(new ContentMember() { ContentTitle = "IMPROVED", ContentInfo = "Adding integration for Xcode as external editor", ContentStyle = ResourceHelper.GetResource<Style>(ResourceToken.LabelPrimary) });
            data.Add(item);
            gitTime.ItemsSource = data;
        }
        private void btnRemoveGit_Click(object sender, RoutedEventArgs e)
        {
            gitTime.ItemsSource = null;
            data.Clear();
        }
        private void TgSort_Unchecked(object sender, RoutedEventArgs e)
        {
            gitTime.OrderBy = GithubTimeLine.OrderType.DessendingTitleLabel;
            tgSort.Content = "DessendingTitleLabel";
        }

        private void TgSort_Checked(object sender, RoutedEventArgs e)
        {
            gitTime.OrderBy = GithubTimeLine.OrderType.AssendingTitleLabel;
            tgSort.Content = "AssendingTitleLabel";
        }

        private void TgSort_Unchecked2(object sender, RoutedEventArgs e)
        {
            gitTime.OrderBy = GithubTimeLine.OrderType.DessendingTitleInfo;
            tgSort2.Content = "DessendingTitleInfo";
        }

        private void TgSort_Checked2(object sender, RoutedEventArgs e)
        {
            gitTime.OrderBy = GithubTimeLine.OrderType.AssendingTitleInfo;
            tgSort2.Content = "AssendingTitleInfo";
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
#if NET40
                Action a1 = delegate()
                {
                    sp.Value = i;
                    sld.Value = i;
                };
                Dispatcher.Invoke(DispatcherPriority.Normal, a1);
#else
                Dispatcher.Invoke(() =>
                 {
                     sp.Value = i;
                     sld.Value = i;
                 });
#endif
                Thread.Sleep(30);

                if (i == 180)
                {
                    while (i > 0)
                    {
                        i--;
#if NET40
                Action a2 = delegate()
                {
                    sp.Value = i;
                    sld.Value = i;
                };
                Dispatcher.Invoke(DispatcherPriority.Normal, a2);
#else
                        Dispatcher.Invoke(() =>
                         {
                             sp.Value = i;
                             sld.Value = i;
                         });
#endif

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
#if NET40
                Action a1 = delegate()
                {
                    sp2.Value = i;
                    sld2.Value = i;
                };
                Dispatcher.Invoke(DispatcherPriority.Normal, a1);
#else
                Dispatcher.Invoke(() =>
                 {
                     sp2.Value = i;
                     sld2.Value = i;
                 });
#endif
                Thread.Sleep(30);

                if (i == 120)
                {
                    while (i > 0)
                    {
                        i--;
#if NET40
                Action a2 = delegate()
                {
                    sp2.Value = i;
                    sld2.Value = i;
                };
                Dispatcher.Invoke(DispatcherPriority.Normal, a2);
#else
                        Dispatcher.Invoke(() =>
                         {
                             sp2.Value = i;
                             sld2.Value = i;
                         });
#endif

                        Thread.Sleep(30);
                        if (i == 0)
                            return;
                    }

                }
            }
        }
        BackgroundWorker worker = new BackgroundWorker();
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Parallel.Invoke(startSpeedoMeter1, startSpeedoMeter2);
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync();
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

        #region Update Helper
        private void btnCheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            var ver = UpdateHelper.CheckForUpdate("https://raw.githubusercontent.com/ghost1372/HandyControls/develop/Updater.xml");
            if (ver.IsExistNewVersion)
            {
                Growl.InfoGlobal("New Version Found!");
                lblUrl.Text = ver.Url;
                txtChangelog.Text = ver.Changelog;
            }
            else
            {
                Growl.ErrorGlobal("you are using latest version");
                lblUrl.Text = ver.Url;
                txtChangelog.Text = ver.Changelog;
            }
        }

        private void btnCheckUpdate2_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtus.Text) && !string.IsNullOrEmpty(txtrp.Text))
            {
                var ver = UpdateHelper.CheckForUpdateGithubRelease(txtus.Text, txtrp.Text);
                if (ver.IsExistNewVersion)
                {
                    Growl.InfoGlobal("New Version Found!");
                    lblUrl2.Text = ver.ApiUrl;
                    lbl1.Text = ver.CreatedAt.ToString();
                    lbl2.Text = ver.PublishedAt.ToString();

                    //Asset is List so maybe there is more than one file just use forech or increase index
                    lbl3.Text = ver.Asset[0].browser_download_url;
                    lbl4.Text = ver.IsPreRelease.ToString();
                    lbl5.Text = ver.Asset[0].size.ToString();
                    lbl6.Text = ver.TagName;
                    txtChangelog2.Text = ver.Changelog;
                }
                else
                {
                    Growl.ErrorGlobal("you are using latest version");
                    lblUrl2.Text = ver.ApiUrl;
                    lbl1.Text = ver.CreatedAt.ToString();
                    lbl2.Text = ver.PublishedAt.ToString();
                    lbl3.Text = ver.Asset[0].browser_download_url;
                    lbl4.Text = ver.IsPreRelease.ToString();
                    lbl5.Text = ver.Asset[0].size.ToString();
                    lbl6.Text = ver.TagName;
                    txtChangelog2.Text = ver.Changelog;
                }
            }
            else
            {
                Growl.ErrorGlobal("please use correct username and repo");
            }

        }
        #endregion

        #region TabControl
        private void CmbBrush_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string resourceName = ((ComboBoxItem)cmbBrush.SelectedItem).Content.ToString();
            uwpTab.HeaderBrush = ResourceHelper.GetResource<Brush>(resourceName);
        }

        private void cmbAligment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cmbAligment.SelectedIndex)
            {
                case 0:
                    uwpTab.HeaderBrushAlignment = HandyControl.Controls.TabControl.BrushAlignment.Top;
                    break;
                case 1:
                    uwpTab.HeaderBrushAlignment = HandyControl.Controls.TabControl.BrushAlignment.Bottom;
                    break;
            }
        }
        #endregion

        #region PersianDate
        private void initDate()
        {
            PersianDate persian = new PersianDate();
            txtP1.Text = persian.GetShamsiYear(DateTime.Now).ToString();
            txtP2.Text = persian.GetShamsiMonthName(DateTime.Now).ToString();
            txtP3.Text = persian.GetShamsiDayString(DateTime.Now).ToString();
            txtP4.Text = persian.GetShamsiDayName(DateTime.Now).ToString();
            txtP5.Text = persian.GetShamsiDayShortName(DateTime.Now).ToString();
            txtP6.Text = DateTime.Now.ToShamsiDate();
            txtP7.Text = DateTime.Now.ToShortShamsiDate();
            txtP8.Text = DateTime.Now.ToLongShamsiDate();
        }
        #endregion

        #region CheckTreeView
        public ObservableCollection<CheckTreeView> TreeRoot { get; set; } = new ObservableCollection<CheckTreeView>();

        private void initCheckTreeView()
        {
            for (int i = 0; i < 10; i++)
            {
                CheckTreeView item = new CheckTreeView() { Text = $"Item {i}", IsChecked = false, CTag = "TAG" };
                CheckTreeView subItem1 = new CheckTreeView() { Text = "SubItem 1", IsChecked = false, CTag = "TAG" };
                CheckTreeView subItem2 = new CheckTreeView() { Text = "SubItem 2", IsChecked = false, CTag = "TAG" };
                item.Add(subItem1);
                item.Add(subItem2);
                TreeRoot.Add(item);
            }


            DataContext = this;
        }
        #endregion
    }
}