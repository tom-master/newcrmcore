using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

namespace NewCrmCore.Domain.Entitys
{
    public class PropertyArgs : EventArgs
    {
        public String PropertyName { get; }

        public Object PropertyValue { get; }

        public PropertyArgs(String propertyName, Object propertyValue)
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue;
        }
    }

    public abstract class PropertyMonitor
    {
        public delegate void PropertyChangeEventHandler(Object sender, PropertyArgs args);

        public event PropertyChangeEventHandler PropertyChanged;

        private IDictionary<String, Object> _propertyValues = new Dictionary<String, Object>();

        private readonly Object _sync = new Object();

        public PropertyMonitor()
        {
            if(PropertyChanged == null)
            {
                PropertyChanged += PropertyMonitor_PropertyChanged;
            }
        }

        private void PropertyMonitor_PropertyChanged(Object sender, PropertyArgs e)
        {
            lock(_sync)
            {
                var instanceName = $@"{e.PropertyName}";
                if(!_propertyValues.Keys.Contains(instanceName))
                {
                    _propertyValues.Add(new KeyValuePair<String, Object>(instanceName, e.PropertyValue));
                }
                else
                {
                    _propertyValues[instanceName] = e.PropertyValue;
                }
            }
        }

        public void OnPropertyChanged(params String[] propertyNames)
        {
            if(propertyNames.Length == 0)
            {
                return;
            }

            var temp = Interlocked.CompareExchange(ref PropertyChanged, null, null);

            for(int i = 0 ; i < propertyNames.Length ; i++)
            {
                var property = GetType().GetProperty(propertyNames[i], BindingFlags.Instance | BindingFlags.Public);
                temp?.Invoke(this, new PropertyArgs(propertyNames[i], property.GetValue(this)));
            }
        }

        public IDictionary<String, Object> GetPropertyValues()
        {
            return _propertyValues;
        }
    }
}
