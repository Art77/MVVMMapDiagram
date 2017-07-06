using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using DiagramDesigner.Helpers;
using DiagramDesigner;

namespace MapDiagram
{
    public class ToolBoxViewModel
    {
        private List<ToolBoxData> toolBoxItems = new List<ToolBoxData>();

        public ToolBoxViewModel()
        {

            toolBoxItems.Add(new ToolBoxData("../Images/lineGroupPanel.png", typeof(LineGrouItemViewModel)));
            toolBoxItems.Add(new ToolBoxData("../Images/ODConnectorPanel.png", typeof(ODconnectorItemViewModel)));
            toolBoxItems.Add(new ToolBoxData("../Images/internalNodePanel.png", typeof(InternalNodeItemViewModel)));
            toolBoxItems.Add(new ToolBoxData("../Images/nodePanel.png", typeof(TrafficLightItemViewModel)));
        }

        public List<ToolBoxData> ToolBoxItems
        {
            get { return toolBoxItems; }
        }
    }
}
