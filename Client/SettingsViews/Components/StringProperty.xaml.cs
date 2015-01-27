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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.Components
{
    /// <summary>
    /// Interaction logic for StringProperty.xaml
    /// </summary>
    public partial class StringProperty : UserControl
    {
        public StringProperty()
        {
            InitializeComponent();
        }

        private string _label = "";
        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }

        private string _setProperty = "";
        public string SetProperty
        {
            get { return _setProperty; }
            set { _setProperty = value; }
        }

        public string TextArea
        {
            get
            {
                PropertyInfo _setting = MoustacheLayer.Singleton.Settings.GetType().GetProperty(SetProperty);
                string value = (string)_setting.GetValue(MoustacheLayer.Singleton.Settings);

                if (String.IsNullOrEmpty(value))
                {
                    return "";
                }

                return value;
            }

            set
            {
                PropertyInfo _setting = MoustacheLayer.Singleton.Settings.GetType().GetProperty(SetProperty);
                _setting.SetValue(MoustacheLayer.Singleton.Settings, value, null);
                MoustacheLayer.Singleton.Settings.Save();

            }
        }


        private string _desc = "";
        public string Desc
        {
            get { return _desc; }
            set { _desc = value; }
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
