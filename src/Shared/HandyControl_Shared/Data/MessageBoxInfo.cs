using System.Windows;

namespace HandyControl.Data
{
    public class MessageBoxInfo
    {
        public string Message { get; set; }

        public string Caption { get; set; }

        public MessageBoxButton Button { get; set; } = MessageBoxButton.OK;

        public string IconKey { get; set; }

        public string IconBrushKey { get; set; }

        public MessageBoxResult DefaultResult { get; set; } = MessageBoxResult.None;

        public string StyleKey { get; set; }

        public string CancelContent { get; set; }
        
        public string ConfirmContent { get; set; }
        
        public string YesContent { get; set; }
        
        public string NoContent { get; set; }
    }
}