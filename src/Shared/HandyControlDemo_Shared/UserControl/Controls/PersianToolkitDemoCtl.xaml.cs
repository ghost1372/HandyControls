using HandyControl.Controls;
using HandyControl.Data;
using System.Windows.Controls;

namespace HandyControlDemo.UserControl
{
    public sealed partial class PersianToolkitDemoCtl
    {
        public PersianToolkitDemoCtl()
        {
            this.InitializeComponent();
        }

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
    }
}
