using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using HandyControl.Data;
using HandyControl.Tools.Extension;

namespace HandyControl.Controls
{
    /// <summary>
    ///     Cover flow
    /// </summary>
    [TemplatePart(Name = ElementViewport3D, Type = typeof(Viewport3D))]
    [TemplatePart(Name = ElementCamera, Type = typeof(ProjectionCamera))]
    [TemplatePart(Name = ElementVisualParent, Type = typeof(ModelVisual3D))]
    public class CoverFlow : Control
    {
        private const string ElementViewport3D = "PART_Viewport3D";

        private const string ElementCamera = "PART_Camera";

        private const string ElementVisualParent = "PART_VisualParent";

        /// <summary>
        ///     Half of the maximum display quantity
        /// </summary>
        private const int MaxShowCountHalf = 3;

        /// <summary>
        ///     page number
        /// </summary>
        public static readonly DependencyProperty PageIndexProperty = DependencyProperty.Register(
            "PageIndex", typeof(int), typeof(CoverFlow),
            new PropertyMetadata(ValueBoxes.Int0Box, OnPageIndexChanged, CoercePageIndex));

        private static object CoercePageIndex(DependencyObject d, object baseValue)
        {
            var ctl = (CoverFlow) d;
            var v = (int) baseValue;

            if (v < 0)
            {
                return 0;
            }
            if (v >= ctl._contentDic.Count)
            {
                return ctl._contentDic.Count - 1;
            }
            return v;
        }

        private static void OnPageIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (CoverFlow) d;
            ctl.UpdateIndex((int) e.NewValue);
        }

        /// <summary>
        ///     Whether to loop
        /// </summary>
        public static readonly DependencyProperty LoopProperty = DependencyProperty.Register(
            "Loop", typeof(bool), typeof(CoverFlow), new PropertyMetadata(ValueBoxes.FalseBox));

        /// <summary>
        ///     Store all content
        /// </summary>
        private readonly Dictionary<int, object> _contentDic = new Dictionary<int, object>();

        /// <summary>
        ///     Items currently in the display range
        /// </summary>
        private readonly Dictionary<int, CoverFlowItem> _itemShowDic = new Dictionary<int, CoverFlowItem>();

        /// <summary>
        ///     camera
        /// </summary>
        private ProjectionCamera _camera;

        private Point3DAnimation _point3DAnimation;

        /// <summary>
        ///     3d canvas
        /// </summary>
        private Viewport3D _viewport3D;

        /// <summary>
        ///     Item container
        /// </summary>
        private ModelVisual3D _visualParent;

        /// <summary>
        ///     Display the number of the first item in the range
        /// </summary>
        private int _firstShowIndex;

        /// <summary>
        ///     Display the number of the last item in the range
        /// </summary>
        private int _lastShowIndex;

        /// <summary>
        ///     Jump number
        /// </summary>
        private int _jumpToIndex = -1;

        /// <summary>
        ///     page number
        /// </summary>
        public int PageIndex
        {
            get => (int) GetValue(PageIndexProperty);
            internal set => SetValue(PageIndexProperty, value);
        }

        /// <summary>
        ///     Whether to loop
        /// </summary>
        public bool Loop
        {
            get => (bool) GetValue(LoopProperty);
            set => SetValue(LoopProperty, ValueBoxes.BooleanBox(value));
        }

        public override void OnApplyTemplate()
        {
            if (_viewport3D != null)
            {
                _viewport3D.Children.Clear();
                _itemShowDic.Clear();
                _viewport3D.MouseLeftButtonDown -= Viewport3D_MouseLeftButtonDown;
            }

            base.OnApplyTemplate();

            _viewport3D = GetTemplateChild(ElementViewport3D) as Viewport3D;
            if (_viewport3D != null)
            {
                _viewport3D.MouseLeftButtonDown += Viewport3D_MouseLeftButtonDown;
            }

            _camera = GetTemplateChild(ElementCamera) as ProjectionCamera;
            _visualParent = GetTemplateChild(ElementVisualParent) as ModelVisual3D;

            UpdateShowRange();
            if (_jumpToIndex > 0)
            {
                PageIndex = _jumpToIndex;
                _jumpToIndex = -1;
            }

            _point3DAnimation = new Point3DAnimation(new Point3D(CoverFlowItem.Interval * PageIndex, _camera.Position.Y, _camera.Position.Z), new Duration(TimeSpan.FromMilliseconds(200)));
            _camera.BeginAnimation(ProjectionCamera.PositionProperty, _point3DAnimation);
        }

        /// <summary>
        ///     Add resources in bulk
        /// </summary>
        /// <param name="contentList"></param>
        public void AddRange(IEnumerable<object> contentList)
        {
            foreach (var content in contentList)
            {
                _contentDic.Add(_contentDic.Count, content);
            }
        }

        /// <summary>
        ///     Add a resource
        /// </summary>
        /// <param name="uriString"></param>
        public void Add(string uriString) => _contentDic.Add(_contentDic.Count, new Uri(uriString));

        /// <summary>
        ///     Add a resource
        /// </summary>
        /// <param name="uri"></param>
        public void Add(Uri uri) => _contentDic.Add(_contentDic.Count, uri);

        /// <summary>
        ///     Jump
        /// </summary>
        public void JumpTo(int index) => _jumpToIndex = index;

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (e.Delta < 0)
            {
                var index = PageIndex + 1;
                PageIndex = index >= _contentDic.Count ? Loop ? 0 : _contentDic.Count - 1 : index;
            }
            else
            {
                var index = PageIndex - 1;
                PageIndex = index < 0 ? Loop ? _contentDic.Count - 1 : 0 : index;
            }

            e.Handled = true;
        }

        /// <summary>
        ///     Delete the item at the specified location
        /// </summary>
        /// <param name="pos"></param>
        private void Remove(int pos)
        {
            if (!_itemShowDic.TryGetValue(pos, out var item)) return;
            _visualParent.Children.Remove(item);
            _itemShowDic.Remove(pos);
        }

        private void Viewport3D_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var result = (RayMeshGeometry3DHitTestResult) VisualTreeHelper.HitTest(_viewport3D, e.GetPosition(_viewport3D));
            if (result != null)
            {
                foreach (var item in _itemShowDic.Values)
                {
                    if (item.HitTest(result.MeshHit))
                    {
                        PageIndex = item.Index;
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///     Update item location
        /// </summary>
        /// <param name="newIndex"></param>
        private void UpdateIndex(int newIndex)
        {
            UpdateShowRange();
            _itemShowDic.Do(item =>
            {
                if (item.Value.Index < _firstShowIndex || item.Value.Index > _lastShowIndex)
                {
                    Remove(item.Value.Index);
                }
                else
                {
                    item.Value.Move(PageIndex);
                }
            });

            _point3DAnimation = new Point3DAnimation(new Point3D(CoverFlowItem.Interval * newIndex, _camera.Position.Y,
                _camera.Position.Z), new Duration(TimeSpan.FromMilliseconds(200)));

            _camera.BeginAnimation(ProjectionCamera.PositionProperty, _point3DAnimation);
        }

        /// <summary>
        ///     Update display range
        /// </summary>
        private void UpdateShowRange()
        {
            var newFirstShowIndex = Math.Max(PageIndex - MaxShowCountHalf, 0);
            var newLastShowIndex = Math.Min(PageIndex + MaxShowCountHalf, _contentDic.Count - 1);

            for (var i = newFirstShowIndex; i <= newLastShowIndex; i++)
            {
                if (!_itemShowDic.ContainsKey(i))
                {
                    var cover = CreateCoverFlowItem(i, _contentDic[i]);
                    _itemShowDic[i] = cover;
                    _visualParent.Children.Add(cover);
                }
            }

            _firstShowIndex = newFirstShowIndex;
            _lastShowIndex = newLastShowIndex;
        }

        private CoverFlowItem CreateCoverFlowItem(int index, object content)
        {
            if (content is Uri uri)
            {
                try
                {
                    return new CoverFlowItem(index, PageIndex, new Image
                    {
                        Source = BitmapFrame.Create(uri, BitmapCreateOptions.DelayCreation, BitmapCacheOption.OnDemand)
                    });
                }
                catch
                {
                    return new CoverFlowItem(index, PageIndex, new ContentControl());
                }
            }

            return new CoverFlowItem(index, PageIndex, new ContentControl
            {
                Content = content
            });
        }
    }
}
