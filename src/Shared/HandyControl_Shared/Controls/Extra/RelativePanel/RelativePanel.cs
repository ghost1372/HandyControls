using HandyControl.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace HandyControl.Controls
{
    public class RelativePanel : Panel
    {
        public static readonly DependencyProperty AlignLeftWithPanelProperty =
            DependencyProperty.RegisterAttached("AlignLeftWithPanel", typeof(bool), typeof(RelativePanel),
                new FrameworkPropertyMetadata(ValueBoxes.FalseBox,
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange,
                    OnHorizontalDirectionPropertyChanged));

        public static bool GetAlignLeftWithPanel(UIElement element) =>
            (bool)element.GetValue(AlignLeftWithPanelProperty);
        public static void SetAlignLeftWithPanel(UIElement element, bool value) =>
            element.SetValue(AlignLeftWithPanelProperty, value);

        public static readonly DependencyProperty AlignTopWithPanelProperty =
            DependencyProperty.RegisterAttached("AlignTopWithPanel", typeof(bool), typeof(RelativePanel),
                new FrameworkPropertyMetadata(ValueBoxes.FalseBox,
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange,
                    OnVerticalDirectionPropertyChanged));

        public static bool GetAlignTopWithPanel(UIElement element) =>
            (bool)element.GetValue(AlignTopWithPanelProperty);
        public static void SetAlignTopWithPanel(UIElement element, bool value) =>
            element.SetValue(AlignTopWithPanelProperty, GetBoxed(value));

        public static readonly DependencyProperty AlignRightWithPanelProperty =
            DependencyProperty.RegisterAttached("AlignRightWithPanel", typeof(bool), typeof(RelativePanel),
                new FrameworkPropertyMetadata(ValueBoxes.FalseBox,
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange,
                    OnHorizontalDirectionPropertyChanged));

        public static bool GetAlignRightWithPanel(UIElement element) =>
            (bool)element.GetValue(AlignRightWithPanelProperty);
        public static void SetAlignRightWithPanel(UIElement element, bool value) =>
            element.SetValue(AlignRightWithPanelProperty, GetBoxed(value));

        public static readonly DependencyProperty AlignBottomWithPanelProperty =
            DependencyProperty.RegisterAttached("AlignBottomWithPanel", typeof(bool), typeof(RelativePanel),
                new FrameworkPropertyMetadata(ValueBoxes.FalseBox,
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange,
                    OnVerticalDirectionPropertyChanged));

        public static bool GetAlignBottomWithPanel(UIElement element) =>
            (bool)element.GetValue(AlignBottomWithPanelProperty);
        public static void SetAlignBottomWithPanel(UIElement element, bool value) =>
            element.SetValue(AlignBottomWithPanelProperty, GetBoxed(value));

        public static readonly DependencyProperty AlignLeftWithProperty =
            DependencyProperty.RegisterAttached("AlignLeftWith", typeof(UIElement), typeof(RelativePanel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange,
                    OnHorizontalDirectionPropertyChanged));

        [TypeConverter(typeof(NameReferenceConverter))]
        public static UIElement GetAlignLeftWith(UIElement element) =>
            (UIElement)element.GetValue(AlignLeftWithProperty);
        public static void SetAlignLeftWith(UIElement element, UIElement value) =>
            element.SetValue(AlignLeftWithProperty, value);

        public static readonly DependencyProperty AlignTopWithProperty =
            DependencyProperty.RegisterAttached("AlignTopWith", typeof(UIElement), typeof(RelativePanel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange,
                    OnVerticalDirectionPropertyChanged));

        [TypeConverter(typeof(NameReferenceConverter))]
        public static UIElement GetAlignTopWith(UIElement element) =>
            (UIElement)element.GetValue(AlignTopWithProperty);
        public static void SetAlignTopWith(UIElement element, UIElement value) =>
            element.SetValue(AlignTopWithProperty, value);

        public static readonly DependencyProperty AlignRightWithProperty =
            DependencyProperty.RegisterAttached("AlignRightWith", typeof(UIElement), typeof(RelativePanel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange,
                    OnHorizontalDirectionPropertyChanged));

        [TypeConverter(typeof(NameReferenceConverter))]
        public static UIElement GetAlignRightWith(UIElement element) =>
            (UIElement)element.GetValue(AlignRightWithProperty);
        public static void SetAlignRightWith(UIElement element, UIElement value) =>
            element.SetValue(AlignRightWithProperty, value);

        public static readonly DependencyProperty AlignBottomWithProperty =
            DependencyProperty.RegisterAttached("AlignBottomWith", typeof(UIElement), typeof(RelativePanel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange,
                    OnVerticalDirectionPropertyChanged));

        [TypeConverter(typeof(NameReferenceConverter))]
        public static UIElement GetAlignBottomWith(UIElement element) =>
            (UIElement)element.GetValue(AlignBottomWithProperty);
        public static void SetAlignBottomWith(UIElement element, UIElement value) =>
            element.SetValue(AlignBottomWithProperty, value);

        public static readonly DependencyProperty LeftOfProperty =
            DependencyProperty.RegisterAttached("LeftOf", typeof(UIElement), typeof(RelativePanel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange,
                    OnHorizontalDirectionPropertyChanged));

        [TypeConverter(typeof(NameReferenceConverter))]
        public static UIElement GetLeftOf(UIElement element) =>
            (UIElement)element.GetValue(LeftOfProperty);
        public static void SetLeftOf(UIElement element, UIElement value) =>
            element.SetValue(LeftOfProperty, value);

        public static readonly DependencyProperty AboveProperty =
            DependencyProperty.RegisterAttached("Above", typeof(UIElement), typeof(RelativePanel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange,
                    OnVerticalDirectionPropertyChanged));

        [TypeConverter(typeof(NameReferenceConverter))]
        public static UIElement GetAbove(UIElement element) =>
            (UIElement)element.GetValue(AboveProperty);
        public static void SetAbove(UIElement element, UIElement value) =>
            element.SetValue(AboveProperty, value);

        public static readonly DependencyProperty RightOfProperty =
            DependencyProperty.RegisterAttached("RightOf", typeof(UIElement), typeof(RelativePanel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange,
                    OnHorizontalDirectionPropertyChanged));

        [TypeConverter(typeof(NameReferenceConverter))]
        public static UIElement GetRightOf(UIElement element) =>
            (UIElement)element.GetValue(RightOfProperty);
        public static void SetRightOf(UIElement element, UIElement value) =>
            element.SetValue(RightOfProperty, value);

        public static readonly DependencyProperty BelowProperty =
            DependencyProperty.RegisterAttached("Below", typeof(UIElement), typeof(RelativePanel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange,
                    OnVerticalDirectionPropertyChanged));

        [TypeConverter(typeof(NameReferenceConverter))]
        public static UIElement GetBelow(UIElement element) =>
            (UIElement)element.GetValue(BelowProperty);
        public static void SetBelow(UIElement element, UIElement value) =>
            element.SetValue(BelowProperty, value);

        public static readonly DependencyProperty AlignHorizontalCenterWithPanelProperty =
            DependencyProperty.RegisterAttached("AlignHorizontalCenterWithPanel", typeof(bool), typeof(RelativePanel),
                new FrameworkPropertyMetadata(ValueBoxes.FalseBox,
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange,
                    OnHorizontalDirectionPropertyChanged));

        public static bool GetAlignHorizontalCenterWithPanel(UIElement element) =>
            (bool)element.GetValue(AlignHorizontalCenterWithPanelProperty);
        public static void SetAlignHorizontalCenterWithPanel(UIElement element, bool value) =>
            element.SetValue(AlignHorizontalCenterWithPanelProperty, GetBoxed(value));

        public static readonly DependencyProperty AlignHorizontalCenterWithProperty =
            DependencyProperty.RegisterAttached("AlignHorizontalCenterWith", typeof(UIElement), typeof(RelativePanel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange,
                    OnHorizontalDirectionPropertyChanged));

        [TypeConverter(typeof(NameReferenceConverter))]
        public static UIElement GetAlignHorizontalCenterWith(UIElement element) =>
            (UIElement)element.GetValue(AlignHorizontalCenterWithProperty);
        public static void SetAlignHorizontalCenterWith(UIElement element, UIElement value) =>
            element.SetValue(AlignHorizontalCenterWithProperty, value);

        public static readonly DependencyProperty AlignVerticalCenterWithPanelProperty =
            DependencyProperty.RegisterAttached("AlignVerticalCenterWithPanel", typeof(bool), typeof(RelativePanel),
                new FrameworkPropertyMetadata(ValueBoxes.FalseBox,
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange,
                    OnVerticalDirectionPropertyChanged));

        public static bool GetAlignVerticalCenterWithPanel(UIElement element) =>
            (bool)element.GetValue(AlignVerticalCenterWithPanelProperty);
        public static void SetAlignVerticalCenterWithPanel(UIElement element, bool value) =>
            element.SetValue(AlignVerticalCenterWithPanelProperty, GetBoxed(value));

        public static readonly DependencyProperty AlignVerticalCenterWithProperty =
            DependencyProperty.RegisterAttached("AlignVerticalCenterWith", typeof(UIElement), typeof(RelativePanel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange,
                    OnVerticalDirectionPropertyChanged));

        [TypeConverter(typeof(NameReferenceConverter))]
        public static UIElement GetAlignVerticalCenterWith(UIElement element) =>
            (UIElement)element.GetValue(AlignVerticalCenterWithProperty);
        public static void SetAlignVerticalCenterWith(UIElement element, UIElement value) =>
            element.SetValue(AlignVerticalCenterWithProperty, value);

        static void OnHorizontalDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((d as FrameworkElement)?.Parent is RelativePanel owner)
                owner._horizontalOrderedLayoutInfos = null;
        }
        static void OnVerticalDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((d as FrameworkElement)?.Parent is RelativePanel owner)
                owner._verticalOrderedLayoutInfos = null;
        }

        static readonly DependencyProperty[] _horizontalEdgeAProperties, _horizontalEdgeBProperties, _horizontalCenterAlignmentProperties;
        static readonly DependencyProperty[] _verticalEdgeAProperties, _verticalEdgeBProperties, _verticalCenterAlignmentProperties;

        static readonly DependencyProperty LayoutInfoProperty =
            DependencyProperty.RegisterAttached(nameof(LayoutInfo), typeof(LayoutInfo), typeof(RelativePanel));

        static LayoutInfo GetLayoutInfo(UIElement element) =>
            (LayoutInfo)element.GetValue(LayoutInfoProperty);
        static void SetLayoutInfo(UIElement element, LayoutInfo value) =>
            element.SetValue(LayoutInfoProperty, value);

        List<LayoutInfo> _layoutInfos = new List<LayoutInfo>();

        LayoutInfo[] _horizontalOrderedLayoutInfos;
        LayoutInfo[] _verticalOrderedLayoutInfos;

        static RelativePanel()
        {
            _horizontalEdgeAProperties = new[]
            {
                AlignLeftWithPanelProperty,
                AlignLeftWithProperty,
                RightOfProperty,
            };
            _horizontalEdgeBProperties = new[]
            {
                AlignRightWithPanelProperty,
                AlignRightWithProperty,
                LeftOfProperty,
            };
            _horizontalCenterAlignmentProperties = new[]
            {
                AlignHorizontalCenterWithPanelProperty,
                AlignHorizontalCenterWithProperty,
            };

            _verticalEdgeAProperties = new[]
            {
                AlignTopWithPanelProperty,
                AlignTopWithProperty,
                BelowProperty,
            };
            _verticalEdgeBProperties = new[]
            {
                AlignBottomWithPanelProperty,
                AlignBottomWithProperty,
                AboveProperty,
            };
            _verticalCenterAlignmentProperties = new[]
            {
                AlignVerticalCenterWithPanelProperty,
                AlignVerticalCenterWithProperty,
            };
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_horizontalOrderedLayoutInfos == null)
                _horizontalOrderedLayoutInfos = GenerateHorizontalOrderedLayoutInfos();

            if (_verticalOrderedLayoutInfos == null)
                _verticalOrderedLayoutInfos = GenerateVerticalOrderedLayoutInfos();

            var width = availableSize.Width;
            var height = availableSize.Height;

            MeasureHorizontalDirection(width, height);
            MeasureVerticalDirection(width, height);

            return availableSize;
        }

        LayoutInfo[] GenerateHorizontalOrderedLayoutInfos()
        {
            var queue = new Queue<LayoutInfo>(_layoutInfos.Count);

            foreach (var info in _layoutInfos)
            {
                info.GraphDependencies = new HashSet<LayoutInfo>();
                info.GraphDependents = new HashSet<LayoutInfo>();

                info.LeftProperty = null;
                info.LeftDependency = null;
                info.RightProperty = null;
                info.RightDependency = null;

                info.IsHorizontalDirectionCenterAlignment = false;
            }

            foreach (var info in _layoutInfos)
                if (!CheckDependenciesAndBuildGraph(info, _horizontalEdgeAProperties, _horizontalEdgeBProperties, _horizontalCenterAlignmentProperties))
                    queue.Enqueue(info);

            return GenerateSortedLayoutInfos(queue);
        }
        LayoutInfo[] GenerateVerticalOrderedLayoutInfos()
        {
            var queue = new Queue<LayoutInfo>(_layoutInfos.Count);

            foreach (var info in _layoutInfos)
            {
                info.GraphDependencies = new HashSet<LayoutInfo>();
                info.GraphDependents = new HashSet<LayoutInfo>();

                info.TopProperty = null;
                info.TopDependency = null;
                info.BottomProperty = null;
                info.BottomDependency = null;

                info.IsVerticalDirectionCenterAlignment = false;
            }

            foreach (var info in _layoutInfos)
                if (!CheckDependenciesAndBuildGraph(info, _verticalEdgeAProperties, _verticalEdgeBProperties, _verticalCenterAlignmentProperties))
                    queue.Enqueue(info);

            return GenerateSortedLayoutInfos(queue);
        }
        LayoutInfo[] GenerateSortedLayoutInfos(Queue<LayoutInfo> queue)
        {
            var sortedElementCount = 0;

            var result = new LayoutInfo[_layoutInfos.Count];

            while (queue.Count > 0)
            {
                var info = queue.Dequeue();

                result[sortedElementCount++] = info;

                foreach (var dependent in info.GraphDependents)
                {
                    var dependencies = dependent.GraphDependencies;

                    dependencies.Remove(info);

                    if (dependencies.Count == 0)
                        queue.Enqueue(dependent);
                }

                info.GraphDependencies = null;
                info.GraphDependents = null;
            }

            if (sortedElementCount < result.Length)
                throw new InvalidOperationException("RelativePanel error: Circular dependency detected. Layout could not complete.");

            return result;
        }

        bool CheckDependenciesAndBuildGraph(LayoutInfo info, DependencyProperty[] edgeAProperties, DependencyProperty[] edgeBProperties, DependencyProperty[] centerAlignmentProperties)
        {
            var hasEdgeADependency = CheckDependencyAndBuildGraphCore(info, edgeAProperties);
            var hasEdgeBDependency = CheckDependencyAndBuildGraphCore(info, edgeBProperties);

            var result = hasEdgeADependency || hasEdgeBDependency;

            if (!result)
            {
                var element = info.Element;

                var centerAlignmentProperty = centerAlignmentProperties[0];

                if ((bool)element.GetValue(centerAlignmentProperty))
                {
                    if (centerAlignmentProperties == _horizontalCenterAlignmentProperties)
                    {
                        info.LeftProperty = info.RightProperty = centerAlignmentProperty;
                        info.IsHorizontalDirectionCenterAlignment = true;
                    }
                    else
                    {
                        info.TopProperty = info.BottomProperty = centerAlignmentProperty;
                        info.IsVerticalDirectionCenterAlignment = true;
                    }

                    return false;
                }

                centerAlignmentProperty = centerAlignmentProperties[1];

                var dependency = GetDependency(element, centerAlignmentProperty);
                if (dependency == null)
                    return false;

                if (centerAlignmentProperties == _horizontalCenterAlignmentProperties)
                {
                    info.LeftProperty = info.RightProperty = centerAlignmentProperty;
                    info.LeftDependency = info.RightDependency = dependency;
                    info.IsHorizontalDirectionCenterAlignment = true;
                }
                else
                {
                    info.TopProperty = info.BottomProperty = centerAlignmentProperty;
                    info.TopDependency = info.BottomDependency = dependency;
                    info.IsVerticalDirectionCenterAlignment = true;
                }

                dependency.GraphDependents.Add(info);
                info.GraphDependencies.Add(dependency);

                return true;
            }

            return result;
        }
        bool CheckDependencyAndBuildGraphCore(LayoutInfo info, DependencyProperty[] properties)
        {
            var element = info.Element;

            var alignWithPanelProperty = properties[0];

            if ((bool)element.GetValue(alignWithPanelProperty))
            {
                if (alignWithPanelProperty == AlignLeftWithPanelProperty)
                    info.LeftProperty = alignWithPanelProperty;
                else if (alignWithPanelProperty == AlignTopWithPanelProperty)
                    info.TopProperty = alignWithPanelProperty;
                else if (alignWithPanelProperty == AlignRightWithPanelProperty)
                    info.RightProperty = alignWithPanelProperty;
                else if (alignWithPanelProperty == AlignBottomWithPanelProperty)
                    info.BottomProperty = alignWithPanelProperty;

                return false;
            }

            for (var i = 1; i < properties.Length; i++)
            {
                var property = properties[i];

                var dependency = GetDependency(element, property);
                if (dependency == null)
                    continue;

                if (property == AlignLeftWithProperty || property == RightOfProperty)
                {
                    info.LeftProperty = property;
                    info.LeftDependency = dependency;
                }
                else if (property == AlignTopWithProperty || property == BelowProperty)
                {
                    info.TopProperty = property;
                    info.TopDependency = dependency;
                }
                else if (property == AlignRightWithProperty || property == LeftOfProperty)
                {
                    info.RightProperty = property;
                    info.RightDependency = dependency;
                }
                else if (property == AlignBottomWithProperty || property == AboveProperty)
                {
                    info.BottomProperty = property;
                    info.BottomDependency = dependency;
                }

                dependency.GraphDependents.Add(info);
                info.GraphDependencies.Add(dependency);

                return true;
            }

            return false;
        }
        LayoutInfo GetDependency(DependencyObject element, DependencyProperty property)
        {
            var value = (UIElement)element.GetValue(property);
            if (value == null)
                return null;

            if (!InternalChildren.Contains(value))
                throw new ArgumentException("RelativePanel error: Element does not exist in the current context");

            return GetLayoutInfo(value);
        }

        void MeasureHorizontalDirection(double width, double height)
        {
            foreach (var info in _horizontalOrderedLayoutInfos)
            {
                var leftProperty = info.LeftProperty;

                if (leftProperty == null || info.IsHorizontalDirectionCenterAlignment)
                    info.Left = double.NaN;
                else if (leftProperty == AlignLeftWithPanelProperty)
                    info.Left = .0;
                else if (leftProperty == AlignLeftWithProperty)
                    info.Left = info.LeftDependency.Left;
                else if (leftProperty == RightOfProperty)
                    info.Left = width - info.LeftDependency.Right;

                var rightProperty = info.RightProperty;

                if (rightProperty == null || info.IsHorizontalDirectionCenterAlignment)
                    info.Right = double.NaN;
                else if (rightProperty == AlignRightWithPanelProperty)
                    info.Right = .0;
                else if (rightProperty == AlignRightWithProperty)
                    info.Right = info.RightDependency.Right;
                else if (rightProperty == LeftOfProperty)
                    info.Right = width - info.RightDependency.Left;

                if (!info.Left.IsNaN() && !info.Right.IsNaN())
                    continue;

                var availableWidth = width - info.Right - info.Left;

                if (availableWidth.IsNaN())
                {
                    availableWidth = width;

                    if (!info.Left.IsNaN() && info.Right.IsNaN())
                        availableWidth -= info.Left;
                    else if (info.Left.IsNaN() && !info.Right.IsNaN())
                        availableWidth -= info.Right;
                }

                var element = info.Element;

                element.Measure(new Size(Math.Max(availableWidth, 0), height));

                if (!info.IsHorizontalDirectionCenterAlignment)
                {
                    if (info.Left.IsNaN() && !info.Right.IsNaN())
                        info.Left = width - info.Right - element.DesiredSize.Width;
                    else if (info.Right.IsNaN())
                    {
                        if (info.Left.IsNaN())
                            info.Left = .0;

                        info.Right = width - info.Left - element.DesiredSize.Width;
                    }

                    continue;
                }

                if (leftProperty == AlignHorizontalCenterWithPanelProperty)
                    info.Left = info.Right = (width - element.DesiredSize.Width) * .5;
                else
                {
                    var widthOfDependency = width - info.LeftDependency.Right - info.LeftDependency.Left;
                    var halfWidthDifference = (element.DesiredSize.Width - widthOfDependency) * .5;

                    info.Left = info.LeftDependency.Left - halfWidthDifference;
                    info.Right = info.LeftDependency.Right - halfWidthDifference;
                }
            }
        }
        void MeasureVerticalDirection(double width, double height)
        {
            foreach (var info in _verticalOrderedLayoutInfos)
            {
                var topProperty = info.TopProperty;

                if (topProperty == null || info.IsVerticalDirectionCenterAlignment)
                    info.Top = double.NaN;
                else if (topProperty == AlignTopWithPanelProperty)
                    info.Top = .0;
                else if (topProperty == AlignTopWithProperty)
                    info.Top = info.TopDependency.Top;
                else if (topProperty == BelowProperty)
                    info.Top = height - info.TopDependency.Bottom;

                var bottomProperty = info.BottomProperty;

                if (bottomProperty == null || info.IsVerticalDirectionCenterAlignment)
                    info.Bottom = double.NaN;
                else if (bottomProperty == AlignBottomWithPanelProperty)
                    info.Bottom = .0;
                else if (bottomProperty == AlignBottomWithProperty)
                    info.Bottom = info.BottomDependency.Bottom;
                else if (bottomProperty == AboveProperty)
                    info.Bottom = height - info.BottomDependency.Top;

                var availableHeight = height - info.Bottom - info.Top;

                if (availableHeight.IsNaN())
                {
                    availableHeight = height;

                    if (!info.Top.IsNaN() && info.Bottom.IsNaN())
                        availableHeight -= info.Top;
                    else if (info.Top.IsNaN() && !info.Bottom.IsNaN())
                        availableHeight -= info.Bottom;
                }

                var element = info.Element;

                element.Measure(new Size(Math.Max(width - info.Left - info.Right, 0), Math.Max(availableHeight, 0)));

                if (!info.IsVerticalDirectionCenterAlignment)
                {
                    if (info.Top.IsNaN() && !info.Bottom.IsNaN())
                        info.Top = height - info.Bottom - element.DesiredSize.Height;
                    else if (info.Bottom.IsNaN())
                    {
                        if (info.Top.IsNaN())
                            info.Top = .0;

                        info.Bottom = height - info.Top - element.DesiredSize.Height;
                    }

                    continue;
                }

                if (topProperty == AlignVerticalCenterWithPanelProperty)
                    info.Top = info.Bottom = (height - element.DesiredSize.Height) * .5;
                else
                {
                    var heightOfDependency = height - info.TopDependency.Bottom - info.TopDependency.Top;
                    var halfHeightDifference = (element.DesiredSize.Height - heightOfDependency) * .5;

                    info.Top = info.TopDependency.Top - halfHeightDifference;
                    info.Bottom = info.TopDependency.Bottom - halfHeightDifference;
                }
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var width = finalSize.Width;
            var height = finalSize.Height;

            foreach (var info in _layoutInfos)
                info.Element.Arrange(new Rect(info.Left, info.Top, Math.Max(width - info.Left - info.Right, 0), Math.Max(height - info.Top - info.Bottom, 0)));

            return finalSize;
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            _horizontalOrderedLayoutInfos = _verticalOrderedLayoutInfos = null;

            UIElement element;
            LayoutInfo layoutInfo;

            if (visualAdded != null)
            {
                element = (UIElement)visualAdded;
                layoutInfo = new LayoutInfo(element);

                _layoutInfos.Add(layoutInfo);

                SetLayoutInfo(element, layoutInfo);
            }
            else if (visualRemoved != null)
            {
                element = (UIElement)visualRemoved;
                layoutInfo = GetLayoutInfo(element);

                _layoutInfos.Remove(layoutInfo);

                element.ClearValue(LayoutInfoProperty);
            }

            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        [DebuggerDisplay("Element = {Element}, Name = {Name}, Placement = [{Left}, {Top}, {Right}, {Bottom}]")]
        class LayoutInfo
        {
            public readonly UIElement Element;

            public string Name => (Element as FrameworkElement)?.Name;

            public ISet<LayoutInfo> GraphDependencies;
            public ISet<LayoutInfo> GraphDependents;

            public DependencyProperty LeftProperty, TopProperty, RightProperty, BottomProperty;
            public LayoutInfo LeftDependency, TopDependency, RightDependency, BottomDependency;

            public bool IsHorizontalDirectionCenterAlignment, IsVerticalDirectionCenterAlignment;

            public double Left, Top, Right, Bottom;

            public LayoutInfo(UIElement element)
            {
                Element = element;
            }
        }

#if netle40
        [MethodImpl(256)]
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object GetBoxed(bool value) => value ? ValueBoxes.TrueBox : ValueBoxes.FalseBox;
    }
}
