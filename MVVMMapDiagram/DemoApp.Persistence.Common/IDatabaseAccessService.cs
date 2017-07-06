using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapDiagram.Persistence.Common
{
    public interface IDatabaseAccessService
    {

        //void DeletePersistDesignerItem(int persistDesignerId);
        //int SavePersistDesignerItem(PersistDesignerItem persistDesignerItemToSave);
        //PersistDesignerItem FetchPersistDesignerItem(int settingsDesignerItemId);

        //delete methods
        void DeleteConnection(int connectionId);     
        void DeleteInternalNodeItem(int internalNodeId);
        void DeleteLaneItem(int laneId);
        void DeleteLineGroupItem(int linegroupId);
        void DeleteODConnectorItem(int odConnectorId);
        void DeleteTrafficLingtItem(int trafficLingtId);

        //save methods
        int SaveDiagram(DiagramItem diagram);
        int SaveInternalNodeItem(InternalNodeItem internalNodeItemToSave);
        int SaveLaneItem(LaneItem laneItemToSave);
        int SaveLineGroupItem(LineGroupItem lineGroupItemToSave);
        int SaveODConnectorItem(ODConnectorItem odConnectorItemToSave);
        int SaveTrafficLightItem(TrafficLightItem trafficLightItemToSave);
        int SaveConnection(Connection connectionToSave);

        //Fetch methods
        IEnumerable<DiagramItem> FetchAllDiagram();
        DiagramItem FetchDiagram(int diagramId);
        InternalNodeItem FetchInternaNodeItem(int internslNodeItemId);
        LaneItem FetchLaneItem(int laneItemId);
        LineGroupItem FetchLineGroupItem(int lineGroupItemId);
        ODConnectorItem FetchODConnectorItem(int odConnectorItemId);
        TrafficLightItem FetchTrafficLightItem(int trafficLightItemId);
        Connection FetchConnection(int connectionId);
    }
}
