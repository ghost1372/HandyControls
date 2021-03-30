using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace HandyControl.Tools
{
    internal class WeakDependencyPropertyEventBus
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Dictionary<Subscription, WeakReference> subscriptions = new Dictionary<Subscription, WeakReference>();

        public IDisposable CreateSubscription(PropertyChangedCallback eventHandler)
        {
            Subscription subscription = new Subscription();

            subscription.Disposed += this.OnSubscriptionDisposed;

            this.subscriptions.Add(subscription, new WeakReference(eventHandler));

            return subscription;
        }

        public void NotifySubscribers(DependencyObject dp, DependencyPropertyChangedEventArgs args)
        {
            foreach (var subscriptionKV in this.subscriptions.ToArray())
            {
                PropertyChangedCallback target = subscriptionKV.Value.Target as PropertyChangedCallback;

                if (target != null)
                {
                    target.Invoke(dp, args);
                }
                else
                {
                    subscriptionKV.Key.Dispose();
                }
            }
        }

        private void OnSubscriptionDisposed(object sender, EventArgs e)
        {
            if (sender is Subscription subscription)
            {
                subscription.Disposed -= this.OnSubscriptionDisposed;

                this.subscriptions.Remove(subscription);
            }
        }
    }
}
