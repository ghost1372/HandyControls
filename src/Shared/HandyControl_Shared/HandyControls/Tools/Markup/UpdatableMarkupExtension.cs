// https://thomaslevesque.com/2009/08/23/wpf-markup-extensions-and-templates/
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Markup;
using System.Windows;

namespace HandyControl.Tools;

public abstract class UpdatableMarkupExtension : MarkupExtension
{
    private List<object> _targetObjects = new List<object>();
    private object _targetProperty;

    protected IEnumerable<object> TargetObjects
    {
        get { return _targetObjects; }
    }

    protected object TargetProperty
    {
        get { return _targetProperty; }
    }

    public sealed override object ProvideValue(IServiceProvider serviceProvider)
    {
        // Retrieve target information
        IProvideValueTarget target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

        if (target != null && target.TargetObject != null)
        {
            // In a template the TargetObject is a SharedDp (internal WPF class)
            // In that case, the markup extension itself is returned to be re-evaluated later
            if (target.TargetObject.GetType().FullName == "System.Windows.SharedDp")
                return this;

            // Save target information for later updates
            _targetObjects.Add(target.TargetObject);
            _targetProperty = target.TargetProperty;
        }

        // Delegate the work to the derived class
        return ProvideValueInternal(serviceProvider);
    }

    protected virtual void UpdateValue(object value)
    {
        if (_targetObjects.Count > 0)
        {
            // Update the target property of each target object
            foreach (var target in _targetObjects)
            {
                if (_targetProperty is DependencyProperty)
                {
                    DependencyObject obj = target as DependencyObject;
                    DependencyProperty prop = _targetProperty as DependencyProperty;

                    Action updateAction = () => obj.SetValue(prop, value);

                    // Check whether the target object can be accessed from the
                    // current thread, and use Dispatcher.Invoke if it can't

                    if (obj.CheckAccess())
                        updateAction();
                    else
                        obj.Dispatcher.Invoke(updateAction);
                }
                else // _targetProperty is PropertyInfo
                {
                    PropertyInfo prop = _targetProperty as PropertyInfo;
                    prop.SetValue(target, value, null);
                }
            }
        }
    }

    protected abstract object ProvideValueInternal(IServiceProvider serviceProvider);
}
