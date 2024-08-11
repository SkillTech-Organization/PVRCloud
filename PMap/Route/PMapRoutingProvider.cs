using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMap.NET.MapProviders;
using GMap.NET;
using PMapCore.DB.Base;
using System.Data;
using PMapCore.Route;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data.Odbc;
using PMapCore.DB;
using PMapCore.BO;
using PMapCore.Common;
using System.Runtime.ExceptionServices;

namespace PMapCore.MapProvider
{

    //Üzemmódok
    //
    public enum ECalcMode
    {
        ShortestPath,
        FastestPath
    }



    /// <summary>
    /// A multhread miatt példányosíthatónak kell lennie az osztálynak!!!
    /// </summary>
    public class PMapRoutingProvider
    {


        private List<int>[] m_computedNeighborsArr = null;              //A sebességprofilhoz tartozó korlátozásokkal figyelembe vett csomópontkapcsolatok


        private HashSet<int> m_targetNodes;
        public PMapRoutingProvider()
        {
        }

        ///
        /// Egy p_NOD_ID_FROM pontból az összes p_ListNOD_ID_TO pontba számított legrövidebb út számítása
        //
        public List<boRoute> GetAllRoutes(CRoutePars p_RoutePars, int p_NOD_ID_FROM, List<int> p_ListNOD_ID_TO, List<int>[] p_neighborsArrFull, List<int>[] p_neighborsArrCut, ECalcMode p_calcMode)
        {

 
            List<boRoute> result = new List<boRoute>();         //útvonal leíró az összes target-re
            DateTime dtStart = DateTime.Now;
            RouteCalculator calcEngine;

            if (p_calcMode == ECalcMode.ShortestPath)
                calcEngine = new RouteCalculator(RouteData.Instance.NodeCount,
                                                 new RouteCalculator.GetInternodeCost(getShortestPath),
                                                 new RouteCalculator.GetNodeNeigbors(getNeigbors));
            else
                calcEngine = new RouteCalculator(RouteData.Instance.NodeCount,
                                                 new RouteCalculator.GetInternodeCost(getFastestPath),
                                                 new RouteCalculator.GetNodeNeigbors(getNeigbors));




            m_targetNodes = new HashSet<int>(p_ListNOD_ID_TO);
            RouteCalculator.RouteCalcResult optimizedPathsForAllDest = null;
            RouteCalculator.RouteCalcResult optimizedPathsForAllDestNOCUT = null;

            if (PMapIniParams.Instance.CutMapForRouting && p_neighborsArrCut  != null && p_neighborsArrCut.Length > 0)
                m_computedNeighborsArr = p_neighborsArrCut;
            else
                m_computedNeighborsArr = p_neighborsArrFull;
            optimizedPathsForAllDest = calcEngine.CalcAllOptimizedPaths(p_NOD_ID_FROM, IsComputeAllRoutesFinish);


            foreach (int NOD_ID_TO in p_ListNOD_ID_TO)
            {

                int[] optimizedPath = new int[0];
                if (optimizedPathsForAllDest != null)
                {
                    optimizedPath = calcEngine.GetOptimizedPath(p_NOD_ID_FROM, NOD_ID_TO, optimizedPathsForAllDest.MinimumPath);
                }

                //kivágott térképen nem találtunk útvonalat, próbálkozunk a teljessel
                if (optimizedPath.Count() <= 1 && PMapIniParams.Instance.CutMapForRouting)
                {
                    if (optimizedPathsForAllDestNOCUT == null)      //teljes térkép path-ok legenerálása, amennyiben szükésges
                    {
                        m_computedNeighborsArr = p_neighborsArrFull;
                        m_targetNodes = new HashSet<int>(p_ListNOD_ID_TO);
                        optimizedPathsForAllDestNOCUT = calcEngine.CalcAllOptimizedPaths(p_NOD_ID_FROM, IsComputeAllRoutesFinish);
                    }
                    if (optimizedPathsForAllDestNOCUT != null)
                    {
                        //lekérdezzük 
                        optimizedPath = calcEngine.GetOptimizedPath(p_NOD_ID_FROM, NOD_ID_TO, optimizedPathsForAllDestNOCUT.MinimumPath);
                    }
                }

                result.Add(getRouteInfo(p_NOD_ID_FROM, NOD_ID_TO, p_RoutePars, optimizedPath));
            }
         //   Console.WriteLine("GetAllRoutes " + Util.GetSysInfo() + " Időtartam:" + (DateTime.Now - dtStart).ToString());
            return result;
        }


        private bool IsComputeAllRoutesFinish(int p_computedNode)
        {
            if (m_targetNodes.Contains(p_computedNode))
            {
                m_targetNodes.Remove(p_computedNode);
            }
            return m_targetNodes.Count == 0;
        }

        /// <summary>
        /// Két node közötti legrövidebb útvonal. 
        /// Részletesebb lekérdezéshez a GetRouteDetails() metódus használandó
        /// </summary>
        /// <param name="p_NOD_ID_FROM"></param>
        /// <param name="p_NOD_ID_TO"></param>
        /// <returns></returns>
        public boRoute GetRoute( int p_NOD_ID_FROM, int p_NOD_ID_TO, CRoutePars p_routePar, List<int>[] p_neighborsArrFull, List<int>[] p_neighborsArrCut, ECalcMode p_calcMode)
        {
  
            DateTime dtStart = DateTime.Now;
            RouteCalculator calcEngine;

            if (p_calcMode == ECalcMode.ShortestPath)
                calcEngine = new RouteCalculator(RouteData.Instance.NodeCount,
                                                 new RouteCalculator.GetInternodeCost(getShortestPath),
                                                 new RouteCalculator.GetNodeNeigbors(getNeigbors));
            else
                calcEngine = new RouteCalculator(RouteData.Instance.NodeCount,
                                                 new RouteCalculator.GetInternodeCost(getFastestPath),
                                                 new RouteCalculator.GetNodeNeigbors(getNeigbors));


            int[] optimizedPath = new int[0];

            if (PMapIniParams.Instance.CutMapForRouting && p_neighborsArrCut != null && p_neighborsArrCut.Length > 0)
            {
                m_computedNeighborsArr = p_neighborsArrCut;
                optimizedPath = calcEngine.CalcOneOptimizedPath(p_NOD_ID_FROM, p_NOD_ID_TO);
            }
            //ha kivágott térképen nem találunk útvonalat, megpróbáljuk a teljes térképpel
            if (optimizedPath.Count() <= 1)
            {

                m_computedNeighborsArr = p_neighborsArrFull;
                optimizedPath = calcEngine.CalcOneOptimizedPath(p_NOD_ID_FROM, p_NOD_ID_TO);
            }
            var result = getRouteInfo(p_NOD_ID_FROM, p_NOD_ID_TO, p_routePar, optimizedPath);
            Console.WriteLine("GetRoute " + Util.GetSysInfo() + " Időtartam:" + (DateTime.Now - dtStart).ToString());


            return result;
        }


        /// <summary>
        /// Útvonal koordináták összeállítása az eredménytömbből
        /// </summary>
        /// <param name="p_optimizedPath"></param>
        /// <returns></returns>
        private MapRoute getMapRoute(int[] p_optimizedPath, int p_NOD_ID_FROM)
        {

            List<PointLatLng> pathPoints = new List<PointLatLng>();

            pathPoints.Add(RouteData.Instance.NodePositions[p_NOD_ID_FROM]);                        //legelső pont nincs a p_shortestPath-ban!
            pathPoints.AddRange(p_optimizedPath.Select(x => RouteData.Instance.NodePositions[x]));


            return new MapRoute(pathPoints, "");
        }

        private boRoute getRouteInfo( int p_NOD_ID_FROM, int p_NOD_ID_TO, CRoutePars p_routePar, int[] p_optimizedPath)
        {
   
            boRoute routeInfo = new boRoute()
            {
                NOD_ID_FROM = p_NOD_ID_FROM,
                NOD_ID_TO = p_NOD_ID_TO,
                RZN_ID_LIST = p_routePar.RZN_ID_LIST,
                DST_MAXWEIGHT = p_routePar.Weight,
                DST_MAXHEIGHT = p_routePar.Height,
                DST_MAXWIDTH = p_routePar.Width,
                Route = getMapRoute(p_optimizedPath, p_NOD_ID_FROM),
                Edges = new List<boEdge>()
            };
            
            try
            {
                // edge-k összevadászása
                if (p_optimizedPath.Count() > 0)
                {
                    //Megj.: p_shortestPath nem tartalmazza a kiindulási node-ot. Emiatt van a variálás
                    int NodeFrom = p_NOD_ID_FROM;               //Legelső elem
                    int NodeTo = p_optimizedPath[0];            //A végelemet mindig tartalmazza, azaaz p_optimizedPath.Count()==1  esetén nincs eredmény
                    boEdge edge = null;

                    if (RouteData.Instance.Edges.TryGetValue(NodeFrom.ToString() + "," + NodeTo.ToString(), out edge))
                    {
                        routeInfo.Edges.Add(edge);

                        //A többi elem
                        for (int i = 0; i < p_optimizedPath.Count() - 1; i++)
                        {
                            NodeFrom = p_optimizedPath[i];
                            NodeTo = p_optimizedPath[i + 1];
                            if (!RouteData.Instance.Edges.TryGetValue(NodeFrom.ToString() + "," + NodeTo.ToString(), out edge))
                                throw new NullReferenceException("null edge:" + NodeFrom.ToString() + "," + NodeTo.ToString());

                            routeInfo.Edges.Add(edge);
                        }

                    }
                    else
                    {
                        //az p_optimizedPath mindig tartalmazza a végelemet. 
                        //Ha nincs útvonal a két pont között, akkor p_optimizedPath értéke 1,
                        //Ha 1-nél nagyobb a p_optimizedPath mérete és nem találunk edge-t, gebasz van
                        if (p_optimizedPath.Count() > 1)
                            throw new NullReferenceException("null edge!");
                    }


                }

                //Távolság kiszámolása
                routeInfo.DST_DISTANCE= routeInfo.Edges.Sum(e => (int)e.EDG_LENGTH);
    
                return routeInfo;
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
                throw;
            }
            finally
            {
            }

        }

        //node-k közötti távolságok lekérdezése
        /// Külső változók (gyorsítás miatt vannak külön tárolva)
        //      m_computedEdges       :A sebességprofilhoz tartozó korlátozásokkal figyelembe vett élek
        private float getShortestPath(int nodeFrom, int nodeTo)
        {
            string sKey = nodeFrom.ToString() + "," + nodeTo.ToString();
            boEdge retval;
            if (RouteData.Instance.Edges.TryGetValue(sKey, out retval))
            {
                return retval.EDG_LENGTH;
            }
            return float.MaxValue;
        }

        /// Külső változók (gyorsítás miatt vannak külön tárolva)
        //      m_computedEdges       :A sebességprofilhoz tartozó korlátozásokkal figyelembe vett élek
        private float getFastestPath(int nodeFrom, int nodeTo)
        {
            string sKey = nodeFrom.ToString() + "," + nodeTo.ToString();
            try
            {
                boEdge retval;
                if (RouteData.Instance.Edges.TryGetValue(sKey, out retval))
                {
                    return retval.CalcDuration;
                }
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
                throw;
            }
            return float.MaxValue;
        }

        //Egy node-ból elérhető node-ok visszaadása
        /// <summary>
        // Külső változók (gyorsítás miatt vannak külön tárolva)
        //     m_computedNeighborsArr:A sebességprofilhoz tartozó korlátozásokkal figyelembe vett csomópontkapcsolatok
        /// </summary>
        /// <param name="nodeFrom"></param>
        /// <returns></returns>
        private List<int> getNeigbors(int nodeFrom)
        {
            return m_computedNeighborsArr[nodeFrom];
        }


    }
}