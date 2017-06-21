using GCityDoor.MapControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiagramDesigner
{
    /// <summary>
    /// Interaction logic for DiagramControl.xaml
    /// </summary>
    public partial class DiagramControl : UserControl
    {
        private int _maxDownload = -1;

        public DiagramControl()
        {
            TileGenerator.CacheFolder = @"ImageCache";
            TileGenerator.DownloadCountChanged += this.OnDownloadCountChanged;
            TileGenerator.DownloadError += this.OnDownloadError;
            InitializeComponent();

        }

        private void OnDownloadError(object sender, EventArgs e)
        {
            if (this.Dispatcher.Thread != Thread.CurrentThread)
            {
                this.Dispatcher.BeginInvoke(new Action(() => this.OnDownloadError(sender, e)), null);
                return;
            }
        }

        private void OnDownloadCountChanged(object sender, EventArgs e)
        {
            if (this.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
            {
                this.Dispatcher.BeginInvoke(new Action(() => this.OnDownloadCountChanged(sender, e)), null);
                return;
            }
            if (TileGenerator.DownloadCount == 0)
            {
                _maxDownload = -1;
            }
            else
            {
                if (_maxDownload < TileGenerator.DownloadCount)
                {
                    _maxDownload = TileGenerator.DownloadCount;
                }
            }
        }

        //<!-- Широта: 53.2006600°	
        //Долгота: 45.0046400°-->

        private void DesignerCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            map.Center(53.2006600, 45.0046400, 16);
        }

        private void zoom_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            map.RaiseEvent(e);
        }

        private void MouseMoveAll(object sender, MouseEventArgs e)
        {
            map.RaiseEvent(e);
        }

        private void zoomBorder_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            map.RaiseEvent(e);
        }

        private void zoomBorder_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            map.RaiseEvent(e);
        }
    }
 }
