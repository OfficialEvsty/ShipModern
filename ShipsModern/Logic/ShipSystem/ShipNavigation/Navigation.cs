using ShipsForm.Logic.TilesSystem;
using ShipsForm.Logic.NodeSystem;
using ShipsForm.Exceptions;
using System.Collections.Generic;
using System;
using System.Linq;
using ShipsForm.Graphic;
using System.Windows;
using System.Configuration;
using ShipsModern.Logic.ShipSystem.ShipNavigation;
using ShipsForm.SupportEntities.PatternObserver.Observers;
using ShipsForm.SupportEntities.PatternObserver;

namespace ShipsForm.Logic.ShipSystem.ShipNavigation
{
    interface INavigationController
    {
        public void ChooseRoute(GeneralNode gn, byte iceR);
    }
    class Navigation : IEventObserver<Route>, INavigationController, IPathDrawable
    {
        private List<Route> m_availableRoutes = new List<Route>();
        private List<string>? m_map;
        private float f_distanceTraveledOnCurrentTile;
        public List<Route> AvailableEdgesList { get { return m_availableRoutes; } }
        public Route ChosenRoute { get; private set; }
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
            EventObservable.AddEventObserver(this);
        }

        private void SetCurrentNode()
        {
            if (CurrentTile == null)
                throw new Exception("CurrentTile is not exist. Nullreference.");
            foreach (GeneralNode node in NetworkNodes.Network.Nodes)
            {
                if (Field.IsTilesEqual(Tile.GetTileCoords(node.GetCoords), CurrentTile))
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
                ChosenRoute = null;
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
            foreach (Tile tile in ChosenRoute.Tiles)
            {
                if (CurrentTile == tile)
                    if (ChosenRoute.Tiles.IndexOf(tile) + 1 < ChosenRoute.Tiles.Count)
                        return ChosenRoute.Tiles[ChosenRoute.Tiles.IndexOf(tile) + 1];
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
            CurrentTile = route.Tiles.First();
            Console.WriteLine($"Маршрут был выбран {ChosenRoute}");
        }
        
        private bool IsContainInRoutesList(Route newRoute)
        {
            foreach (var route in m_availableRoutes)
                if (Route.IsRoutesEqual(route, newRoute))
                    return true;
            return false;
        }
        public Route? GetRouteFromList(GeneralNode from, GeneralNode to)
        {
            Tile tileFrom = from.TileCoords;
            Tile tileTo = to.TileCoords;
            if (tileFrom == null || tileTo == null)
                return null;

            foreach (var route in m_availableRoutes)
            {

                if (route.Tiles.First().X == tileFrom.X && route.Tiles.First().Y == tileFrom.Y)
                {
                    if (route.Tiles.Last().X == tileTo.X && route.Tiles.Last().Y == tileTo.Y)
                        return route;
                }
            }
            return null;
        }
        private bool BuildNewRoute(byte iceResistLevel = 1)
        {
            if (FromNode == null || ToNode == null)
                return false;
            Route newRoute = new Route(FromNode, ToNode, m_map, iceResistLevel);            
            return true;
        }

        private void AddRoute(Route newRoute)
        {
            if (newRoute == null)
                return;
            if (!IsContainInRoutesList(newRoute))
            {
                m_availableRoutes.Add(newRoute);
                m_availableRoutes.Add(Route.GetReversedRoute(newRoute));
            }
        }

        public bool CheckRouteValid(List<Tile> route)
        {
            return false;
        }

        public bool IsPath()
        {
            return (ChosenRoute is null) ? false : true;
        }

        public Point[] GetPoints()
        {
            var points = new Point[ChosenRoute.Tiles.Count];
            var i = 0;
            var data = ShipsForm.Data.Configuration.Instance;
            if (data is null)
                throw new ConfigFileDoesntExistError();
            foreach(var tile in ChosenRoute.Tiles)
            {
                points[i++] = new Point(tile.X * data.TileWidth + data.TileWidth, tile.Y * data.TileWidth + data.TileWidth);
            }
            return points;
        }

        public void Update(Route ev)
        {
            throw new NotImplementedException();
        }
    }
}
