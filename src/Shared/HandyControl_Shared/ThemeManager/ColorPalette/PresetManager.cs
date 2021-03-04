// http://github.com/kinnara/ModernWpf

using System;
using HandyControl.Controls;
namespace HandyControl.Themes
{
    public class PresetManager : BindablePropertyBase
    {
        private Preset _colorPreset ;

        private PresetManager()
        {
        }

        public static PresetManager Current { get; } = new PresetManager();

        public Preset ColorPreset
        {
            get => _colorPreset;
            set
            {
                if (_colorPreset != value)
                {
                    _colorPreset = value;
                    RaisePropertyChanged();
                    ColorPresetChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public class Preset
        {
            public string AssemblyName { get; set; }
            public string ColorPreset { get; set; }
        }

        public event EventHandler ColorPresetChanged;
    }
}
