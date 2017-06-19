using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiagramDesigner;
using System.Windows.Input;

namespace DemoApp
{
    public class LineGItemViewModel : LineGroupItemViewModel, ISupportDataChanges
    {
        private IUIVisualizerService visualiserService;

        public LineGItemViewModel(int id, IDiagramViewModel parent, double left, double top, string hostUrl) : base(id, parent, left, top)
        {
            this.HostUrl = hostUrl;
            Init_();
        }

       
        public String HostUrl { get; set; }
        public ICommand ShowDataChangeWindowCommand { get; private set; }

        public void ExecuteShowDataChangeWindowCommand(object parameter)
        {
            InternalNodeItemData data = new InternalNodeItemData(HostUrl);
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
