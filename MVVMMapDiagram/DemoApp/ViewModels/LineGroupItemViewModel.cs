using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiagramDesigner;
using System.Windows.Input;

namespace DemoApp
{
    public class LineGroupItemViewModel : LineGroupItemViewModelBase, ISupportDataChanges
    {
        private IUIVisualizerService visualiserService;

        public LineGroupItemViewModel(int id, IDiagramViewModel parent, string hostUrl) : base(id,parent)
        {
            this.HostUrl = hostUrl;
            Init();
        }

        public LineGroupItemViewModel() : base()
        {
            Init();
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


        private void Init()
        {
            visualiserService = ApplicationServicesProvider.Instance.Provider.VisualizerService;
            ShowDataChangeWindowCommand = new SimpleCommand(ExecuteShowDataChangeWindowCommand);
            this.ShowConnectors = false;

        }
    }
}
