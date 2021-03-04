// http://github.com/kinnara/ModernWpf

using System;
using System.Windows;
using HandyControl.Tools;

namespace HandyControl.Themes
{
    public class ColorPresetResources : ResourceDictionary
    {
        private ApplicationTheme _targetTheme;

        public ColorPresetResources()
        {

#if !NET40
            WeakEventManager<PresetManager, EventArgs>.AddHandler(
                PresetManager.Current,
                nameof(PresetManager.ColorPresetChanged),
                OnCurrentPresetChanged);
#endif
            ApplyCurrentPreset();
        }

        public ApplicationTheme TargetTheme
        {
            get => _targetTheme;
            set
            {
                if (_targetTheme != value)
                {
                    _targetTheme = value;
                    ApplyCurrentPreset();
                }
            }
        }

        private void OnCurrentPresetChanged(object sender, EventArgs e)
        {
            ApplyCurrentPreset();
        }
        private void ApplyCurrentPreset()
        {
            if (MergedDictionaries.Count > 0)
            {
                MergedDictionaries.Clear();
            }

            var currentPreset = PresetManager.Current.ColorPreset;
            if (currentPreset !=null)
            {
                var source = ApplicationHelper.GetAbsoluteUri(currentPreset.AssemblyName, $"{currentPreset.ColorPreset}/{TargetTheme}.xaml");
                var rd = new ResourceDictionary { Source = source };
                MergedDictionaries.Add(rd);
            }
        }
    }
}
