using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace HandyControl.Tools
{
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ApplicationExtensions
    {
        public static void SetCurrentUICulture(this Application application, CultureInfo value)
        {
            application.VerifyAccess();

            if (application == null)
            {
                throw new ArgumentNullException("application");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            CultureInfo oldValue = application.GetCurrentUICulture();

            application.Properties[typeof(CultureInfo)] = value;

            foreach (EventHandler<CultureChangedEventArgs> listener in application.GetListenerCollectionForCurrentUICultureChangedEvent().ToArray().Select(x => x.Key))
            {
                listener(application, new CultureChangedEventArgs(oldValue, value));
            }
        }

        public static CultureInfo GetCurrentUICulture(this Application application)
        {
            application.VerifyAccess();

            if (application == null)
            {
                throw new ArgumentNullException("application");
            }

            return application.Properties[typeof(CultureInfo)] as CultureInfo ?? CultureInfo.InstalledUICulture;
        }

        public static IDisposable RegisterForCurrentUICultureChangedEvent(this Application application, EventHandler<CultureChangedEventArgs> eventhandler)
        {
            application.VerifyAccess();

            if (application == null)
            {
                throw new ArgumentNullException("application");
            }

            if (eventhandler == null)
            {
                throw new ArgumentNullException("eventhandler");
            }

            Subscription<EventHandler<CultureChangedEventArgs>> subscription;

            IDictionary<EventHandler<CultureChangedEventArgs>, Subscription<EventHandler<CultureChangedEventArgs>>> listener = application.GetListenerCollectionForCurrentUICultureChangedEvent();

            if (!listener.TryGetValue(eventhandler, out subscription))
            {
                SubscriptionDisposedHandler subscriptionDisposedHandler = new SubscriptionDisposedHandler(application);

                subscription = new Subscription<EventHandler<CultureChangedEventArgs>>(eventhandler);
                subscription.Disposed += subscriptionDisposedHandler.OnCurrentUICultureChangedEventSubscriptionDestroyed;
                listener.Add(eventhandler, subscription);
            }

            return subscription;
        }

        private static IDictionary<EventHandler<CultureChangedEventArgs>, Subscription<EventHandler<CultureChangedEventArgs>>> GetListenerCollectionForCurrentUICultureChangedEvent(this Application application)
        {
            const string PROPERTY_KEY = "ListenerCollectionForCurrentUICultureChangedEvent";

            IDictionary<EventHandler<CultureChangedEventArgs>, Subscription<EventHandler<CultureChangedEventArgs>>> listeners = application.Properties[PROPERTY_KEY] as IDictionary<EventHandler<CultureChangedEventArgs>, Subscription<EventHandler<CultureChangedEventArgs>>>;

            if (listeners == null)
            {
                listeners = new Dictionary<EventHandler<CultureChangedEventArgs>, Subscription<EventHandler<CultureChangedEventArgs>>>();
                application.Properties[PROPERTY_KEY] = listeners;
            }

            return listeners;
        }

        private class SubscriptionDisposedHandler
        {
            private readonly Application application;

            public SubscriptionDisposedHandler(Application application)
            {
                this.application = application;
            }

            public void OnCurrentUICultureChangedEventSubscriptionDestroyed(object sender, EventArgs e)
            {
                Subscription<EventHandler<CultureChangedEventArgs>> subscription = sender as Subscription<EventHandler<CultureChangedEventArgs>>;

                if (subscription != null)
                {
                    subscription.Disposed -= this.OnCurrentUICultureChangedEventSubscriptionDestroyed;

                    this.application.GetListenerCollectionForCurrentUICultureChangedEvent().Remove(subscription.EventHandler);
                }
            }
        }
    }
}
