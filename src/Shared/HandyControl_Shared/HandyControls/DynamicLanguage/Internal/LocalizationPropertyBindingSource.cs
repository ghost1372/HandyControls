// https://github.com/MartinKuschnik/Goji

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;

namespace HandyControl.Tools
{
    internal class LocalizationPropertyBindingSource : INotifyPropertyChanged, IDisposable
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const string BoundPropertyName = "Value";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DependencyProperty targetProperty;

        private readonly IDisposable localizationPropertyChangedEventsSubscription;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DependencyObject targetObject;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly WeakDependencyPropertyEventBus localizationPropertyChangedEvents;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<Binding> bindng;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool disposed;

        public LocalizationPropertyBindingSource(DependencyProperty targetProperty, DependencyObject targetObject)
        {
            this.targetProperty = targetProperty;
            this.targetObject = targetObject;

            this.bindng = new Lazy<Binding>(this.CreateBinding);

            this.localizationPropertyChangedEvents = (WeakDependencyPropertyEventBus)targetObject.GetValue(LocalizationManager.LocalizationPropertyChangedEventsProperty);

            this.localizationPropertyChangedEventsSubscription = this.localizationPropertyChangedEvents.CreateSubscription(this.OnDependencyPropertyEvent);
        }

        ~LocalizationPropertyBindingSource()
        {
            this.Dispose();
            Debug.WriteLine(string.Format("~{0}", typeof(LocalizationPropertyBindingSource)));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public object Value
        {
            get
            {
                return this.targetObject.GetValue(this.targetProperty);
            }
        }

        public Binding Binding
        {
            get
            {
                return this.bindng.Value;
            }
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                using (this.localizationPropertyChangedEventsSubscription)
                {
                    this.disposed = true;
                    GC.SuppressFinalize(this);
                }
            }
        }

        private void OnDependencyPropertyEvent(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == this.targetProperty && e.NewValue != null)
            {
                PropertyChangedEventHandler propertyChanged = this.PropertyChanged;

                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs(BoundPropertyName));
                }
            }
        }

        private Binding CreateBinding()
        {
            return new Binding(BoundPropertyName) { Mode = BindingMode.OneWay, Source = this };
        }
    }
}
