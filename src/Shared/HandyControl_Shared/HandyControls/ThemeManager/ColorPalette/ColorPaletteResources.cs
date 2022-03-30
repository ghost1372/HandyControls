// http://github.com/kinnara/ModernWpf

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace HandyControl.Themes
{
    /// <summary>
    /// Represents a specialized resource dictionary that contains color resources used
    /// by XAML elements.
    /// </summary>
    public class ColorPaletteResources : ResourceDictionary, ISupportInitialize
    {
        /// <summary>
        /// Initializes a new instance of the ColorPaletteResources class.
        /// </summary>
        public ColorPaletteResources()
        {
        }

        private ApplicationTheme? _targetTheme;
        public ApplicationTheme? TargetTheme
        {
            get => _targetTheme;
            set
            {
                if (_targetTheme.HasValue)
                {
                    throw new InvalidOperationException(nameof(TargetTheme) + " cannot be changed after it's set.");
                }

                if (_targetTheme != value)
                {
                    _targetTheme = value;
                    UpdateBrushes();
                }
            }
        }

        #region Brush
        private Brush? _regionBrush;
        public Brush? RegionBrush
        {
            get => _regionBrush;
            set
            {
                if (Set(ref _regionBrush, value, updateBrushes: false))
                {
                    if (TargetTheme.HasValue)
                    {
                        UpdateBrushes();
                    }
                };
            }
        }

        private Brush? _LightPrimaryBrush;
        public Brush? LightPrimaryBrush
        {
            get => _LightPrimaryBrush;
            set => Set(ref _LightPrimaryBrush, value);
        }

        private Brush? _PrimaryBrush;
        public Brush? PrimaryBrush
        {
            get => _PrimaryBrush;
            set => Set(ref _PrimaryBrush, value);
        }

        private Brush? _DarkPrimaryBrush;
        public Brush? DarkPrimaryBrush
        {
            get => _DarkPrimaryBrush;
            set => Set(ref _DarkPrimaryBrush, value);
        }

        private Brush? _PrimaryTextBrush;
        public Brush? PrimaryTextBrush
        {
            get => _PrimaryTextBrush;
            set => Set(ref _PrimaryTextBrush, value);
        }

        private Brush? _SecondaryTextBrush;
        public Brush? SecondaryTextBrush
        {
            get => _SecondaryTextBrush;
            set => Set(ref _SecondaryTextBrush, value);
        }
        private Brush? _ThirdlyTextBrush;
        public Brush? ThirdlyTextBrush
        {
            get => _ThirdlyTextBrush;
            set => Set(ref _ThirdlyTextBrush, value);
        }

        private Brush? _ReverseTextBrush;
        public Brush? ReverseTextBrush
        {
            get => _ReverseTextBrush;
            set => Set(ref _ReverseTextBrush, value);
        }

        private Brush? _TextIconBrush;
        public Brush? TextIconBrush
        {
            get => _TextIconBrush;
            set => Set(ref _TextIconBrush, value);
        }

        private Brush? _BorderBrush;
        public Brush? BorderBrush
        {
            get => _BorderBrush;
            set => Set(ref _BorderBrush, value);
        }

        private Brush? _SecondaryBorderBrush;
        public Brush? SecondaryBorderBrush
        {
            get => _SecondaryBorderBrush;
            set => Set(ref _SecondaryBorderBrush, value);
        }

        private Brush? _BackgroundBrush;
        public Brush? BackgroundBrush
        {
            get => _BackgroundBrush;
            set => Set(ref _BackgroundBrush, value);
        }

        private Brush? _SecondaryRegionBrush;
        public Brush? SecondaryRegionBrush
        {
            get => _SecondaryRegionBrush;
            set => Set(ref _SecondaryRegionBrush, value);
        }

        private Brush? _ThirdlyRegionBrush;
        public Brush? ThirdlyRegionBrush
        {
            get => _ThirdlyRegionBrush;
            set => Set(ref _ThirdlyRegionBrush, value);
        }

        private Brush? _OddEvenRegionBrush;
        public Brush? OddEvenRegionBrush
        {
            get => _OddEvenRegionBrush;
            set => Set(ref _OddEvenRegionBrush, value);
        }

        private Brush? _TitleBrush;
        public Brush? TitleBrush
        {
            get => _TitleBrush;
            set => Set(ref _TitleBrush, value);
        }

        private Brush? _DefaultBrush;
        public Brush? DefaultBrush
        {
            get => _DefaultBrush;
            set => Set(ref _DefaultBrush, value);
        }

        private Brush? _DarkDefaultBrush;
        public Brush? DarkDefaultBrush
        {
            get => _DarkDefaultBrush;
            set => Set(ref _DarkDefaultBrush, value);
        }

        private Brush? _LightDangerBrush;
        public Brush? LightDangerBrush
        {
            get => _LightDangerBrush;
            set => Set(ref _LightDangerBrush, value);
        }

        private Brush? _DangerBrush;
        public Brush? DangerBrush
        {
            get => _DangerBrush;
            set => Set(ref _DangerBrush, value);
        }

        private Brush? _DarkDangerBrush;
        public Brush? DarkDangerBrush
        {
            get => _DarkDangerBrush;
            set => Set(ref _DarkDangerBrush, value);
        }

        private Brush? _LightWarningBrush;
        public Brush? LightWarningBrush
        {
            get => _LightWarningBrush;
            set => Set(ref _LightWarningBrush, value);
        }

        private Brush? _WarningBrush;
        public Brush? WarningBrush
        {
            get => _WarningBrush;
            set => Set(ref _WarningBrush, value);
        }

        private Brush? _DarkWarningBrush;
        public Brush? DarkWarningBrush
        {
            get => _DarkWarningBrush;
            set => Set(ref _DarkWarningBrush, value);
        }

        private Brush? _LightInfoBrush;
        public Brush? LightInfoBrush
        {
            get => _LightInfoBrush;
            set => Set(ref _LightInfoBrush, value);
        }

        private Brush? _InfoBrush;
        public Brush? InfoBrush
        {
            get => _InfoBrush;
            set => Set(ref _InfoBrush, value);
        }

        private Brush? _DarkInfoBrush;
        public Brush? DarkInfoBrush
        {
            get => _DarkInfoBrush;
            set => Set(ref _DarkInfoBrush, value);
        }

        private Brush? _LightSuccessBrush;
        public Brush? LightSuccessBrush
        {
            get => _LightSuccessBrush;
            set => Set(ref _LightSuccessBrush, value);
        }

        private Brush? _SuccessBrush;
        public Brush? SuccessBrush
        {
            get => _SuccessBrush;
            set => Set(ref _SuccessBrush, value);
        }

        private Brush? _DarkSuccessBrush;
        public Brush? DarkSuccessBrush
        {
            get => _DarkSuccessBrush;
            set => Set(ref _DarkSuccessBrush, value);
        }

        private Brush? _VioletBrush;
        public Brush? VioletBrush
        {
            get => _VioletBrush;
            set => Set(ref _VioletBrush, value);
        }

        private Brush? _DarkVioletBrush;
        public Brush? DarkVioletBrush
        {
            get => _DarkVioletBrush;
            set => Set(ref _DarkVioletBrush, value);
        }

        private Brush? _DarkMaskBrush;
        public Brush? DarkMaskBrush
        {
            get => _DarkMaskBrush;
            set => Set(ref _DarkMaskBrush, value);
        }

        private Brush? _DarkOpacityBrush;
        public Brush? DarkOpacityBrush
        {
            get => _DarkOpacityBrush;
            set => Set(ref _DarkOpacityBrush, value);
        }

        private Brush? _MainContentBackgroundBrush;
        public Brush? MainContentBackgroundBrush
        {
            get => _MainContentBackgroundBrush;
            set => Set(ref _MainContentBackgroundBrush, value);
        }

        private Brush? _MainContentForegroundBrush;
        public Brush? MainContentForegroundBrush
        {
            get => _MainContentForegroundBrush;
            set => Set(ref _MainContentForegroundBrush, value);
        }

        #endregion

        #region Colors
        private Color? _regionColor;
        public Color? RegionColor
        {
            get => _regionColor;
            set
            {
                if (Set(ref _regionColor, value, updateBrushes: false))
                {
                    if (TargetTheme.HasValue)
                    {
                        UpdateBrushes();
                    }
                };
            }
        }

        private Color? _LightPrimaryColor;
        public Color? LightPrimaryColor
        {
            get => _LightPrimaryColor;
            set => Set(ref _LightPrimaryColor, value);
        }

        private Color? _PrimaryColor;
        public Color? PrimaryColor
        {
            get => _PrimaryColor;
            set => Set(ref _PrimaryColor, value);
        }

        private Color? _DarkPrimaryColor;
        public Color? DarkPrimaryColor
        {
            get => _DarkPrimaryColor;
            set => Set(ref _DarkPrimaryColor, value);
        }

        private Color? _LightDangerColor;
        public Color? LightDangerColor
        {
            get => _LightDangerColor;
            set => Set(ref _LightDangerColor, value);
        }

        private Color? _DangerColor;
        public Color? DangerColor
        {
            get => _DangerColor;
            set => Set(ref _DangerColor, value);
        }

        private Color? _DarkDangerColor;
        public Color? DarkDangerColor
        {
            get => _DarkDangerColor;
            set => Set(ref _DarkDangerColor, value);
        }

        private Color? _LightWarningColor;
        public Color? LightWarningColor
        {
            get => _LightWarningColor;
            set => Set(ref _LightWarningColor, value);
        }

        private Color? _WarningColor;
        public Color? WarningColor
        {
            get => _WarningColor;
            set => Set(ref _WarningColor, value);
        }

        private Color? _DarkWarningColor;
        public Color? DarkWarningColor
        {
            get => _DarkWarningColor;
            set => Set(ref _DarkWarningColor, value);
        }

        private Color? _LightInfoColor;
        public Color? LightInfoColor
        {
            get => _LightInfoColor;
            set => Set(ref _LightInfoColor, value);
        }

        private Color? _InfoColor;
        public Color? InfoColor
        {
            get => _InfoColor;
            set => Set(ref _InfoColor, value);
        }

        private Color? _DarkInfoColor;
        public Color? DarkInfoColor
        {
            get => _DarkInfoColor;
            set => Set(ref _DarkInfoColor, value);
        }

        private Color? _LightSuccessColor;
        public Color? LightSuccessColor
        {
            get => _LightSuccessColor;
            set => Set(ref _LightSuccessColor, value);
        }

        private Color? _SuccessColor;
        public Color? SuccessColor
        {
            get => _SuccessColor;
            set => Set(ref _SuccessColor, value);
        }

        private Color? _DarkSuccessColor;
        public Color? DarkSuccessColor
        {
            get => _DarkSuccessColor;
            set => Set(ref _DarkSuccessColor, value);
        }

        private Color? _VioletColor;
        public Color? VioletColor
        {
            get => _VioletColor;
            set => Set(ref _VioletColor, value);
        }

        private Color? _DarkVioletColor;
        public Color? DarkVioletColor
        {
            get => _DarkVioletColor;
            set => Set(ref _DarkVioletColor, value);
        }

        private Color? _PrimaryTextColor;
        public Color? PrimaryTextColor
        {
            get => _PrimaryTextColor;
            set => Set(ref _PrimaryTextColor, value);
        }

        private Color? _SecondaryTextColor;
        public Color? SecondaryTextColor
        {
            get => _SecondaryTextColor;
            set => Set(ref _SecondaryTextColor, value);
        }
        private Color? _ThirdlyTextColor;
        public Color? ThirdlyTextColor
        {
            get => _ThirdlyTextColor;
            set => Set(ref _ThirdlyTextColor, value);
        }

        private Color? _ReverseTextColor;
        public Color? ReverseTextColor
        {
            get => _ReverseTextColor;
            set => Set(ref _ReverseTextColor, value);
        }

        private Color? _TextIconColor;
        public Color? TextIconColor
        {
            get => _TextIconColor;
            set => Set(ref _TextIconColor, value);
        }

        private Color? _BorderColor;
        public Color? BorderColor
        {
            get => _BorderColor;
            set => Set(ref _BorderColor, value);
        }

        private Color? _SecondaryBorderColor;
        public Color? SecondaryBorderColor
        {
            get => _SecondaryBorderColor;
            set => Set(ref _SecondaryBorderColor, value);
        }

        private Color? _BackgroundColor;
        public Color? BackgroundColor
        {
            get => _BackgroundColor;
            set => Set(ref _BackgroundColor, value);
        }

        private Color? _SecondaryRegionColor;
        public Color? SecondaryRegionColor
        {
            get => _SecondaryRegionColor;
            set => Set(ref _SecondaryRegionColor, value);
        }

        private Color? _ThirdlyRegionColor;
        public Color? ThirdlyRegionColor
        {
            get => _ThirdlyRegionColor;
            set => Set(ref _ThirdlyRegionColor, value);
        }

        private Color? _OddEvenRegionColor;
        public Color? OddEvenRegionColor
        {
            get => _OddEvenRegionColor;
            set => Set(ref _OddEvenRegionColor, value);
        }

        private Color? _TitleColor;
        public Color? TitleColor
        {
            get => _TitleColor;
            set => Set(ref _TitleColor, value);
        }

        private Color? _SecondaryTitleColor;
        public Color? SecondaryTitleColor
        {
            get => _SecondaryTitleColor;
            set => Set(ref _SecondaryTitleColor, value);
        }

        private Color? _DefaultColor;
        public Color? DefaultColor
        {
            get => _DefaultColor;
            set => Set(ref _DefaultColor, value);
        }

        private Color? _DarkDefaultColor;
        public Color? DarkDefaultColor
        {
            get => _DarkDefaultColor;
            set => Set(ref _DarkDefaultColor, value);
        }

        private Color? _DarkMaskColor;
        public Color? DarkMaskColor
        {
            get => _DarkMaskColor;
            set => Set(ref _DarkMaskColor, value);
        }

        private Color? _DarkOpacityColor;
        public Color? DarkOpacityColor
        {
            get => _DarkOpacityColor;
            set => Set(ref _DarkOpacityColor, value);
        }

        private Color? _MainContentBackgroundColor;
        public Color? MainContentBackgroundColor
        {
            get => _MainContentBackgroundColor;
            set => Set(ref _MainContentBackgroundColor, value);
        }

        private Color? _MainContentForegroundColor;
        public Color? MainContentForegroundColor
        {
            get => _MainContentForegroundColor;
            set => Set(ref _MainContentForegroundColor, value);
        }

        private Color? _ForthlyRegionColor;
        public Color? ForthlyRegionColor
        {
            get => _ForthlyRegionColor;
            set => Set(ref _ForthlyRegionColor, value);
        }

        #endregion
        private bool Set(ref Brush? storage, Brush? value, bool updateBrushes = true, [CallerMemberName]string propertyName = null)
        {
            if (storage != value)
            {
                Remove(propertyName);

                storage = value;

                Add(propertyName, storage);


                if (TargetTheme.HasValue && updateBrushes)
                {
                    UpdateBrushes();
                }

                return true;
            }

            return false;
        }
        private bool Set(ref Color? storage, Color? value, bool updateBrushes = true, [CallerMemberName] string propertyName = null)
        {
            if (storage != value)
            {
                if (storage.HasValue)
                {
                    Remove(propertyName);
                }

                storage = value;

                if (storage.HasValue)
                {
                    Add(propertyName, storage);
                }

                if (TargetTheme.HasValue && updateBrushes)
                {
                    UpdateBrushes();
                }

                return true;
            }

            return false;
        }
        private void UpdateBrushes()
        {
            if (IsInitializePending)
            {
                return;
            }

            if (MergedDictionaries.Count > 0)
            {
                MergedDictionaries.Clear();
            }

            if (TargetTheme == null || Count == 0)
            {
                return;
            }

            var originals = ThemeManager.GetDefaultThemeDictionary(TargetTheme.Value.ToString());
            var overrides = new ResourceDictionary();
            var originalsToOverrides = new Dictionary<SolidColorBrush, SolidColorBrush>();

            // TODO: recursive
            foreach (DictionaryEntry entry in originals)
            {
                if (entry.Value is SolidColorBrush originalBrush)
                {
                    object colorKey = ThemeResourceHelper.GetColorKey(originalBrush);
                    if (colorKey != null && Contains(colorKey))
                    {
                        if (!originalsToOverrides.TryGetValue(originalBrush, out SolidColorBrush overrideBrush))
                        {
                            overrideBrush = originalBrush.CloneCurrentValue();
                            overrideBrush.Color = (Color)this[colorKey];
                            originalsToOverrides[originalBrush] = overrideBrush;
                        }
                        overrides.Add(entry.Key, overrideBrush);
                    }
                }
            }

            MergedDictionaries.Add(overrides);
        }

        #region ISupportInitialize

        private bool IsInitializePending { get; set; }

        public new void BeginInit()
        {
            base.BeginInit();

            IsInitializePending = true;
        }

        public new void EndInit()
        {
            base.EndInit();

            IsInitializePending = false;

            UpdateBrushes();
        }

        void ISupportInitialize.BeginInit()
        {
            BeginInit();
        }

        void ISupportInitialize.EndInit()
        {
            EndInit();
        }

        #endregion
    }
}
