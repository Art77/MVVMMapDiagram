using System;
using System.Collections.Generic;
using DiagramDesigner;
using System.Windows.Input;
using System.Windows;
using GalaSoft.MvvmLight.CommandWpf;

namespace MapDiagram
{
    public class TrafficLightItemViewModel : DesignerItemViewModelBase, ISupportDataChanges
    {
        private IUIVisualizerService visualiserService;

        private List<Point> lisrPoint = new List<Point>();

        public List<Point> Points
        {
            get { return lisrPoint; }
            set
            {
                if(lisrPoint != value)
                {
                    lisrPoint = value;
                    NotifyChanged("Points");
                }
            }
        }

        public TrafficLightItemViewModel(int id, DiagramViewModel parent, double left, double top,
           double itemHeight, double itemWidth, double angle, string hostUrl) : base(id, parent, left, top, itemHeight, itemWidth, angle)
        {
               
            Points = PointHelper.GeneralPointPolygon(this);
            this.HostUrl = hostUrl;
            Init_();
        }

        public TrafficLightItemViewModel() : base()
        {
            Points = PointHelper.GeneralPointPolygon(this);
            Init_();
        }


        public String HostUrl { get; set; }
        public ICommand ShowDataChangeWindowCommand { get; private set; }
        public RelayCommand<MouseButtonEventArgs> ShowDataChangeWindowRelayCommand { get; private set; }

        public void ExecuteShowDataChangeWindowCommand(object parameter)
        {
            TrafficLightIItemData data = new TrafficLightIItemData(HostUrl);
            if (visualiserService.ShowDialog(data) == true)
            {
                this.HostUrl = data.HostUrl;
            }
        }
        public void ExecuteShowDataChangeWindowCommand(MouseButtonEventArgs e)
        {
            if ((e.LeftButton == MouseButtonState.Pressed) && (Keyboard.IsKeyDown(Key.LeftAlt)))
            {
                TrafficLightIItemData data = new TrafficLightIItemData(HostUrl);
                if (visualiserService.ShowDialog(data) == true)
                {
                    this.HostUrl = data.HostUrl;
                }
            }
        }

        private void Init_()
        {
            visualiserService = ApplicationServicesProvider.Instance.Provider.VisualizerService;
            ShowDataChangeWindowCommand = new SimpleCommand(ExecuteShowDataChangeWindowCommand);
            ShowDataChangeWindowRelayCommand = new RelayCommand<MouseButtonEventArgs>(ExecuteShowDataChangeWindowCommand);
            this.ShowConnectors = false;
        }
    }
}
