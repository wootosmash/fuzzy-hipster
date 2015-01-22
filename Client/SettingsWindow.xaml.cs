﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {

        private SettingsWindowModel _model;
        public SettingsWindowModel Model
        {
            get { return _model; }
        }


        public SettingsWindow()
        {
            _model = new SettingsWindowModel();
            DataContext = Model;
            InitializeComponent();
            Model.Setup();

            //
        }
    }
}
