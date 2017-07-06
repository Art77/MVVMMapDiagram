using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiagramDesigner;
using System.Windows.Input;

namespace MapDiagram
{
    public class ODconnectorItemViewModel : OneDesignerItemViewModel, ISupportDataChanges
    {
        private IUIVisualizerService visualiserService;

        public ODconnectorItemViewModel(int id, DiagramViewModel parent, double left, double top,
           double itemHeight, double itemWidth, double angle, string hostUrl) : base(id, parent, left, top, itemHeight, itemWidth, angle, hostUrl)
        {
            this.HostUrl = hostUrl;
            Init_();
        }


        public ODconnectorItemViewModel() : base()
        {
            Init_();
        }


        public String HostUrl { get; set; }
        public ICommand ShowDataChangeWindowCommand { get; private set; }

        public void ExecuteShowDataChangeWindowCommand(object parameter)
        {
            ODConnectorItemData data = new ODConnectorItemData(HostUrl);
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
