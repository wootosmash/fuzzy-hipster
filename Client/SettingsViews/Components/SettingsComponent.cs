using FuzzyHipster;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Client.Components
{
    public class SettingsComponent<T> : UserControl
    {

        public string Label
        {
            get;
            set;
        }

        public string SetProperty
        {
            get;
            set;
        }

        public T TextArea
        {
            get
            {
                PropertyInfo _setting = MoustacheLayer.Singleton.Settings.GetType().GetProperty(SetProperty);
                T value = (T)_setting.GetValue(MoustacheLayer.Singleton.Settings);
                return value;
            }

            set
            {
                PropertyInfo _setting = MoustacheLayer.Singleton.Settings.GetType().GetProperty(SetProperty);
                _setting.SetValue(MoustacheLayer.Singleton.Settings, value, null);
                MoustacheLayer.Singleton.Settings.Save();

            }
        }


        public string Desc
        {
            get;
            set;
        }



        public void LoadValue(object sender, RoutedEventArgs e)
        {

            OnPropertyChanged(SetProperty);
        
        }
        
        public virtual void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);


            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }
    }
}
