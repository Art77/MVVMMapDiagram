﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Documents;
using System.Linq;
using System.Collections.Generic;
using DiagramDesigner.Helpers;
using DiagramDesigner;

namespace DemoApp
{
    public partial class Window1 : Window
    {
        private Window1ViewModel window1ViewModel;

        public Window1()
        {
            InitializeComponent();

            window1ViewModel = new Window1ViewModel();
            this.DataContext = window1ViewModel;
            this.Loaded += new RoutedEventHandler(Window1_Loaded);
        }


        /// <summary>
        /// This shows you how you can create diagram items in code, which means you can 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            var parent = window1ViewModel.DiagramViewModel;
            SettingsDesignerItemViewModel item1 = new SettingsDesignerItemViewModel();
            item1.Parent = parent;
            item1.Angle = -90;
            item1.Left = 100;
            item1.Top = 100;
            parent.Items.Add(item1);

            InternalNodeItemViewModel item2 = new InternalNodeItemViewModel();
            item2.Parent = parent;
            item2.Left = 300;
            item2.Angle = 65;
            item2.Top = 300;
            parent.Items.Add(item2);

            ConnectorViewModel con1 = new ConnectorViewModel(item1.RightConnector, item2.LeftConnector);    
            con1.Parent = parent;      
            parent.Items.Add(con1);
        }
    }
}
