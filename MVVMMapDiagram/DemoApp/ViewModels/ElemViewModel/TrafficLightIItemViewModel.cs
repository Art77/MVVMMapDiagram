using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiagramDesigner;
using System.Windows.Input;
using System.Windows;
using DiagramDesigner.Helpers;
using System.ComponentModel;
using GalaSoft.MvvmLight.CommandWpf;

namespace DemoApp
{
    public class TrafficLightIItemViewModel : DesignerItemViewModelBase, ISupportDataChanges
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

        public RelayCommand EditPolygon { get; private set; }

        public TrafficLightIItemViewModel(int id, DiagramViewModel parent, double left, double top, string hostUrl) : base(id,parent, left,top)
        {
            this.HostUrl = hostUrl;
            EditPolygon = new RelayCommand(test);
            Points = PointHelper.GeneralPointPolygon(this);
            Init_();
        }

        public TrafficLightIItemViewModel() : base()
        {
            Points = PointHelper.GeneralPointPolygon(this);
            EditPolygon = new RelayCommand(test);
            Init_();
        }

        public void test()
        {
            var f = 2;
        }

        public String HostUrl { get; set; }
        public ICommand ShowDataChangeWindowCommand { get; private set; }

        public void ExecuteShowDataChangeWindowCommand(object parameter)
        {
            PersistDesignerItemData data = new PersistDesignerItemData(HostUrl);
            if (visualiserService.ShowDialog(data) == true)
            {
                this.HostUrl = data.HostUrl;
            }
        }


        private void Init_()
        {
            visualiserService = ApplicationServicesProvider.Instance.Provider.VisualizerService;
            ShowDataChangeWindowCommand = new SimpleCommand(ExecuteShowDataChangeWindowCommand);
            this.ShowConnectors = false;

        }
    }
}
