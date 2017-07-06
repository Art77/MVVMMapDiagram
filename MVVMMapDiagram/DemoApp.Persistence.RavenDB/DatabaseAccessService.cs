using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Embedded;
using Raven.Client;
using MapDiagram.Persistence.Common;

namespace MapDiagram.Persistence.RavenDB
{
    /// <summary>
    /// I decided to use RavenDB instead of SQL, to save people having to have SQL Server, and also
    /// it just takes less time to do with Raven. This is ALL the CRUD code. Simple no?
    /// 
    /// Thing is the IDatabaseAccessService and the items it persists could easily be applied to helper methods that
    /// use StoredProcedures or ADO code, the data being stored would be exactly the same. You would just need to store
    /// the individual property values in tables rather than store objects.
    /// </summary>
    public class DatabaseAccessService : IDatabaseAccessService
    {
        EmbeddableDocumentStore documentStore = null;

        public DatabaseAccessService()
        {
            documentStore = new EmbeddableDocumentStore
            {
                DataDirectory = "Data"
            };
            documentStore.Initialize();
        }
   
        public void DeleteConnection(int connectionId)
        {
            using (IDocumentSession session = documentStore.OpenSession())
            {
                IEnumerable<Connection> conns = session.Query<Connection>().Where(x => x.Id == connectionId);
                foreach (var conn in conns)
                {
                    session.Delete<Connection>(conn);
                }
                session.SaveChanges();
            }
        }

        public void DeletePersistDesignerItem(int persistDesignerId)
        {
            using (IDocumentSession session = documentStore.OpenSession())
            {
                IEnumerable<PersistDesignerItem> persistItems = session.Query<PersistDesignerItem>().Where(x => x.Id == persistDesignerId);
                foreach (var persistItem in persistItems)
                {
                    session.Delete<PersistDesignerItem>(persistItem);
                }
                session.SaveChanges();
            }
        }

        #region Удаление из БД  (по типу компонента)
        public void DeleteInternalNodeItem(int internalNodeId)
        {
            using (IDocumentSession session = documentStore.OpenSession())
            {
                IEnumerable<InternalNodeItem> internalNodeItems = 
                    session.Query<InternalNodeItem>().Where(x => x.Id == internalNodeId);
                foreach (var internalNode in internalNodeItems)
                {
                    session.Delete<InternalNodeItem>(internalNode);
                }
                session.SaveChanges();
            }
        }



        public void DeleteLaneItem(int laneId)
        {
            using (IDocumentSession session = documentStore.OpenSession())
            {
                IEnumerable<LaneItem> laneItems = 
                    session.Query<LaneItem>().Where(x => x.Id == laneId);
                foreach (var laneItem in laneItems)
                {
                    session.Delete<LaneItem>(laneItem);
                }
                session.SaveChanges();
            }
        }


        public void DeleteLineGroupItem(int linegroupId)
        {
            using (IDocumentSession session = documentStore.OpenSession())
            {
                IEnumerable<LineGroupItem> lineGroupItems = 
                    session.Query<LineGroupItem>().Where(x => x.Id == linegroupId);
                foreach (var lineGroup in lineGroupItems)
                {
                    session.Delete<LineGroupItem>(lineGroup);
                }
                session.SaveChanges();
            }
        }


        public void DeleteODConnectorItem(int odConnectorId)
        {
            using (IDocumentSession session = documentStore.OpenSession())
            {
                IEnumerable<ODConnectorItem> odConnectorItems = 
                    session.Query<ODConnectorItem>().Where(x => x.Id == odConnectorId);
                foreach (var odConnector in odConnectorItems)
                {
                    session.Delete<ODConnectorItem>(odConnector);
                }
                session.SaveChanges();
            }
        }


        public void DeleteTrafficLingtItem(int trafficLingtId)
        {
            using (IDocumentSession session = documentStore.OpenSession())
            {
                IEnumerable<TrafficLightItem> trafficLingtItems = 
                    session.Query<TrafficLightItem>().Where(x => x.Id == trafficLingtId);
                foreach (var trafficLingt in trafficLingtItems)
                {
                    session.Delete<TrafficLightItem>(trafficLingt);
                }
                session.SaveChanges();
            }
        }
        #endregion

        public int SaveDiagram(DiagramItem diagram)
        {
            return SaveItem(diagram);
        }

        public int SavePersistDesignerItem(PersistDesignerItem persistDesignerItemToSave)
        {
            return SaveItem(persistDesignerItemToSave);
        }

        #region Сохранение в БД (по типу компонента)

        public int SaveInternalNodeItem(InternalNodeItem internalNodeItemToSave)
        {
            return SaveItem(internalNodeItemToSave);
        }

        public int SaveLaneItem(LaneItem laneItemToSave)
        {
            return SaveItem(laneItemToSave);
        }

        public int SaveLineGroupItem(LineGroupItem lineGroupItemToSave)
        {
            return SaveItem(lineGroupItemToSave);
        }

        public int SaveODConnectorItem(ODConnectorItem  odConnectorItemToSave)
        {
            return SaveItem(odConnectorItemToSave);
        }

        public int SaveTrafficLightItem(TrafficLightItem trafficLightItemToSave)
        {
            return SaveItem(trafficLightItemToSave);
        }
        #endregion

        public int SaveConnection(Connection connectionToSave)
        {
            return SaveItem(connectionToSave);
        }

        public IEnumerable<DiagramItem> FetchAllDiagram()
        {
            using (IDocumentSession session = documentStore.OpenSession())
            {
                return session.Query<DiagramItem>().ToList();
            }
        }

        public DiagramItem FetchDiagram(int diagramId)
        {
            using (IDocumentSession session = documentStore.OpenSession())
            {
                return session.Query<DiagramItem>().Single(x => x.Id == diagramId);
            }
        }

        public PersistDesignerItem FetchPersistDesignerItem(int settingsDesignerItemId)
        {
            using (IDocumentSession session = documentStore.OpenSession())
            {
                return session.Query<PersistDesignerItem>().Single(x => x.Id == settingsDesignerItemId);
            }
        }

        #region Запросы к БД (относительно типа компонента)
        public InternalNodeItem FetchInternaNodeItem(int internslNodeItemId)
        {
            using (IDocumentSession session = documentStore.OpenSession())
            {
                return session.Query<InternalNodeItem>().Single(x => x.Id == internslNodeItemId);
            }
        }

        public LaneItem FetchLaneItem(int laneItemId)
        {
            using (IDocumentSession session = documentStore.OpenSession())
            {
                return session.Query<LaneItem>().Single(x => x.Id == laneItemId);
            }
        }
        public LineGroupItem FetchLineGroupItem(int lineGroupItemId)
        {
            using (IDocumentSession session = documentStore.OpenSession())
            {
                return session.Query<LineGroupItem>().Single(x => x.Id == lineGroupItemId);
            }
        }
        public ODConnectorItem FetchODConnectorItem(int odConnectorItemId)
        {
            using (IDocumentSession session = documentStore.OpenSession())
            {
                return session.Query<ODConnectorItem>().Single(x => x.Id == odConnectorItemId);
            }
        }
        public TrafficLightItem FetchTrafficLightItem(int trafficLightItemId)
        {
            using (IDocumentSession session = documentStore.OpenSession())
            {
                return session.Query<TrafficLightItem>().Single(x => x.Id == trafficLightItemId);
            }
        }
        #endregion

        public Connection FetchConnection(int connectionId)
        {
            using (IDocumentSession session = documentStore.OpenSession())
            {
                return session.Query<Connection>().Single(x => x.Id == connectionId);
            }
        }

        private int SaveItem(PersistableItemBase item)
        {
            using (IDocumentSession session = documentStore.OpenSession())
            {
                session.Store(item);
                session.SaveChanges();
            }
            return item.Id;
        }
    }
}
