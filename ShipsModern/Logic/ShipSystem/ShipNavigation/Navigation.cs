using ShipsForm.Logic.TilesSystem;
using ShipsForm.Logic.NodeSystem;
using ShipsForm.Exceptions;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ShipsForm.Logic.ShipSystem.ShipNavigation
{
    interface INavigationController
    {
        public void ChooseRoute(GeneralNode gn, byte iceR);
    }
    class Navigation : INavigationController
    {
        private List<List<Tile>> m_availableRoutesList = new List<List<Tile>>();
        private List<string>? m_map;
        private float f_distanceTraveledOnCurrentTile;
        public List<List<Tile>> AvailableEdgesList { get { return m_availableRoutesList; } }
        public List<Tile> ChosenRoute { get; private set; }
        public Tile CurrentTile { get; private set; }
        public double CurrentRotation { get; private set; }
        public event Action OnEndRoute;

        public float DistanceTraveledOnCurrentTile
        {
            get { return f_distanceTraveledOnCurrentTile; }
            private set
            {
                f_distanceTraveledOnCurrentTile = value;
                ControlCurrentTileTraveledDistance();
            }
        }

        public GeneralNode? CurrentNode { get; private set; }
        public GeneralNode? FromNode { get; set; }
        public GeneralNode? ToNode { get; set; }

        public Navigation()
        {
            PaintMap();
        }

        private void SetCurrentNode()
        {
            if (CurrentTile == null)
                throw new Exception("CurrentTile is not exist. Nullreference.");
            foreach (GeneralNode node in NetworkNodes.Network.Nodes)
            {
                if (IsTilesEqual(Tile.GetTileCoords(node.GetCoords), CurrentTile))
                    CurrentNode = node;
            }
        }

        public void ChooseRoute(GeneralNode gn, byte iceResistLevel = 1)
        {
            SetToNode(gn);
            InstallRoute(iceResistLevel);
        }

        public void SetRotation()
        {
            if (ChosenRoute is not null)
            {
                Tile? directionTile = GetNextTile();
                if (directionTile == null)
                {
                    CurrentRotation = 0;
                }
                else
                {
                    SupportEntities.Point startP = new SupportEntities.Point(CurrentTile.X, CurrentTile.Y);
                    SupportEntities.Point endP = new SupportEntities.Point(directionTile.X, directionTile.Y);
                    CurrentRotation = SupportEntities.Math.GetRotation(CurrentRotation, startP,endP);
                }                
            }
        }

        public void ObserveMoving(float distanceTraveled)
        {
            DistanceTraveledOnCurrentTile += distanceTraveled;
        }

        private void ControlCurrentTileTraveledDistance()
        {
            if (CurrentTile == null)
                return;
            var data = Data.Configuration.Instance;
            if (data is null)
                throw new ConfigFileDoesntExistError();
            while (CurrentTile != null && f_distanceTraveledOnCurrentTile > data.TileDistance)
            {
                f_distanceTraveledOnCurrentTile -= data.TileDistance;
                SwitchCurrentTile();
            }
        }

        private void SwitchCurrentTile()
        {
            if (GetNextTile() == null && CurrentTile != null)
            {
                SetCurrentNode();
                FromNode = CurrentNode;
                CurrentTile = null;
                OnEndRoute?.Invoke();
                f_distanceTraveledOnCurrentTile = 0;
                Console.WriteLine("Путь пройден");
                return;
            }
            CurrentTile = GetNextTile();
            SetRotation();
        }
        private Tile? GetNextTile()
        {
            foreach (Tile tile in ChosenRoute)
            {
                if (CurrentTile == tile)
                    if (ChosenRoute.IndexOf(tile) + 1 < ChosenRoute.Count)
                        return ChosenRoute[ChosenRoute.IndexOf(tile) + 1];
            }
            return null;
        }
        private void PaintMap()
        {
            m_map = Field.PaintNavigationMap();
        }
        public void SetToNode(GeneralNode destinationNode)
        {
            ToNode = destinationNode;
        }
        public void InstallRoute(byte iceResistLevel = 1)
        {
            if (FromNode == null || ToNode == null)
                return;
            var route = GetRouteFromList(FromNode, ToNode);
            if (route == null)
            {
                BuildNewRoute(iceResistLevel);
                route = GetRouteFromList(FromNode, ToNode);
            }
            if (route == null)
                return;
            ChosenRoute = route;
            CurrentTile = route.First();
            Console.WriteLine($"Маршрут был выбран {ChosenRoute}");
        }
        private static bool IsRoutesEqual(List<Tile> roate1, List<Tile> roate2)
        {
            if (roate1.Count != roate2.Count)
                return false;
            for (int i = 0; i < roate1.Count; i++)
            {
                if (roate1[i].X != roate2[i].X && roate1[i].Y != roate2[i].Y)
                    break;
                if (i == roate1.Count - 1)
                    return true;
            }

            for (int i = roate1.Count-1; i >= 0; i--)
            {
                if (roate1[i].X != roate2[i].X && roate1[i].Y != roate2[i].Y)
                    break;
                if (i == 1)
                    return true;
            }
            return false;
        }
        private static bool IsTilesEqual(Tile t1, Tile t2)
        {
            if (t1 == null || t2 == null)
                return false;
            return t1.X == t2.X && t1.Y == t2.Y;
        }
        private bool IsContainInRoutesList(List<Tile> newRoute)
        {
            foreach (var route in m_availableRoutesList)
                if (IsRoutesEqual(route, newRoute))
                    return true;
            return false;
        }
        public List<Tile>? GetRouteFromList(GeneralNode from, GeneralNode to)
        {
            Tile tileFrom = from.TileCoords;
            Tile tileTo = to.TileCoords;
            if (tileFrom == null || tileTo == null)
                return null;

            foreach (var tileList in m_availableRoutesList)
            {

                if (tileList.First().X == tileFrom.X && tileList.First().Y == tileFrom.Y)
                {
                    if (tileList.Last().X == tileTo.X && tileList.Last().Y == tileTo.Y)
                        return tileList;
                }
                else if (tileList.First().X == tileTo.X && tileList.First().Y == tileTo.Y)
                {
                    var reverseTilesList = tileList;
                    reverseTilesList.Reverse();
                    if (reverseTilesList.First().X == tileFrom.X && reverseTilesList.First().Y == tileFrom.Y)
                        return reverseTilesList;
                }
            }
            return null;
        }
        private bool BuildNewRoute(byte iceResistLevel = 1)
        {
            if (FromNode == null || ToNode == null)
                return false;
            Tile fromNode = Tile.GetTileCoords(FromNode.GetCoords);
            Tile toNode = Tile.GetTileCoords(ToNode.GetCoords);

            if (fromNode == null || toNode == null)
                return false;
            if (m_map == null)
                return false;
            var newRoute = Field.BuildPath(m_map, fromNode, toNode, iceResistLevel);
            if (newRoute == null)
                return false;
            AddRoute(newRoute);
            return true;
        }

        private void AddRoute(List<Tile>? newRoute)
        {
            if (newRoute == null)
                return;
            if (!IsContainInRoutesList(newRoute))
                m_availableRoutesList.Add(newRoute);
        }

        public bool CheckRouteValid(List<Tile> route)
        {
            return false;
        }
    }
}
