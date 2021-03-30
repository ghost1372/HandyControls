using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace HandyControl.Tools
{
    internal class ApplicationLanguageBindingSource : INotifyPropertyChanged
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const string ApplicationLanguagePropertyName = "ApplicationLanguage";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Application application;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<Binding> bindng;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IDisposable currentUICultureChangedEventSubscription;

        public ApplicationLanguageBindingSource(Application application)
        {
            this.application = application;

            this.bindng = new Lazy<Binding>(this.CreateBinding);
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (this.PropertyChangedInternal == null)
                {
                    this.currentUICultureChangedEventSubscription = this.application.RegisterForCurrentUICultureChangedEvent(this.OnCurrentUICultureChanged);
                }

                this.PropertyChangedInternal += value;
            }

            remove
            {
                this.PropertyChangedInternal -= value;

                if (this.PropertyChangedInternal == null)
                {
                    this.currentUICultureChangedEventSubscription.Dispose();
                    this.currentUICultureChangedEventSubscription = null;
                }
            }
        }

        private event PropertyChangedEventHandler PropertyChangedInternal;

        public XmlLanguage ApplicationLanguage
        {
            get
            {
                return XmlLanguage.GetLanguage(this.application.GetCurrentUICulture().IetfLanguageTag);
            }
        }

        public Binding Binding
        {
            get
            {
                return this.bindng.Value;
            }
        }

        private Binding CreateBinding()
        {
            return new Binding(ApplicationLanguagePropertyName) { Mode = BindingMode.OneWay, Source = this };
        }

        private void OnCurrentUICultureChanged(object sender, CultureChangedEventArgs e)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChangedInternal;

            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(ApplicationLanguagePropertyName));
            }
        }
    }
}
