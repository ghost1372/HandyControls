using System.ComponentModel;
using System.Reflection;

namespace HandyControl.Tools.DynamicLanguage
{
    /// <summary>
    /// Special class which use as value provider for bindings.
    /// Some properties are sealing (Setter.Value and Binding.Source) and cannot change, so
    /// if we use such provider, it stay same but it's value free to change.
    /// </summary>
    internal class BindingValueProvider : INotifyPropertyChanged
    {
        /// <summary>
        /// Property info of <see cref="Value" /> property.
        /// </summary>
        public static PropertyInfo ValueProperty = typeof(BindingValueProvider).GetProperty(nameof(Value));

        private object _value;

        /// <summary>
        /// Providing value.
        /// </summary>
        public object Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise <see cref="PropertyChanged" /> event.
        /// </summary>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
