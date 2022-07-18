using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Collections.Concurrent;

namespace CommonLib
{
    public class ObservableObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Here we store all the property values when properties do not have a backing field
        /// </summary>
        private ConcurrentDictionary<string, object> _valueStore = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// An event that fires when any of the property values change
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise Property Changed
        /// </summary>
        /// <param name="propertyName"></param>
        protected void raisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        /// <summary>
        /// Set the property value using the backing field. Should be executed from withing the getter: eg set => setter(ref _myProp, value);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storageField">The backing field of the property</param>
        /// <param name="newValue">The new value to set</param>
        /// <param name="propertyName">The property name - Can be ommited when executed from within the setter</param>
        /// <returns></returns>
        protected bool setter<T>(ref T storageField, T newValue, [CallerMemberName] string propertyName = "")
        {
            // check for null property name
            if (String.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException("Property name cannot be empty");

            // if equals return false
            if (Equals(storageField, newValue))
                return false;

            // set the new value to the backing field
            storageField = newValue;

            // raise property changed
            raisePropertyChanged(propertyName);

            // return true - meaning the property has changed
            return true;
        }


        /// <summary>
        /// Set the property value using the dictionary store. Should be executed from withing the getter: eg set => setter(value);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newValue">The new value to set</param>
        /// <param name="propertyName">The property name - Can be ommited when executed from within the setter</param>
        /// <returns></returns>
        protected bool setter<T>(T newValue, [CallerMemberName] string propertyName = "")
        {
            // check for null property name
            if (String.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException("Property name cannot be empty");

            // try get current value from value store
            object curValue;
            if (_valueStore.TryGetValue(propertyName, out curValue))
            {
                // if equals return false
                if (Equals((T)curValue, newValue))
                    return false;
            }
            else if (!isPropertyNameValid(this.GetType(), propertyName)) // check if type has this property
            {
                // throw
                throw new ArgumentException($"Property name {propertyName} does not exist in type {this.GetType()}.");
            }

            // set the new value to store
            _valueStore[propertyName] = newValue;

            // raise property changed
            raisePropertyChanged(propertyName);

            // return true - meaning the property has changed
            return true;
        }

        /// <summary>
        /// Gets the property value using the dictionary store. Should be executed from withing the getter: eg get => getter<int>();
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName">The property name - Can be ommited when executed from within the getter</param>
        /// <returns></returns>
        protected T getter<T>([CallerMemberName] string propertyName = "")
        {
            // check for null
            if (String.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException("Property name cannot be empty");

            // try get current value from value store
            object curValue;
            if (_valueStore.TryGetValue(propertyName, out curValue))
            {
                // return current value if exists
                return (T)curValue;
            }
            else if (!isPropertyNameValid(this.GetType(), propertyName)) // check if type has this property
            {
                // throw
                throw new ArgumentException($"Property name {propertyName} does not exist in type {this.GetType()}.");
            }

            // get the default value for the type
            var defaultValue = default(T);

            // set the default value to store for next time
            _valueStore[propertyName] = defaultValue;

            // return 
            return defaultValue;
        }

        /// <summary>
        /// This is to check that the property name is valid
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private bool isPropertyNameValid(Type type, string propertyName)
        {
            return type.GetProperties().Any(o => o.Name == propertyName);
        }
    }
}
