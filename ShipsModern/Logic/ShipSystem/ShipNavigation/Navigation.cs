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
using ShipsForm.Logic.ShipSystem.ShipEngine;

namespace ShipsForm.Logic.ShipSystem.ShipNavigation
{
    interface INavigationController
    {
        public void ChooseRoute(GeneralNode gn, byte iceR);
        public bool IsOnRoute();
    }
    class Navigation : INavigationController, IPathDrawable
    {
        private static List<Route> m_availableRoutes = new List<Route>();
        private List<string>? m_map;
        private float f_distanceTraveledOnCurrentTile;
        public List<Route> AvailableRoutes { get { return m_availableRoutes; } }
        public Route ChosenRoute { get; private set; }
        public Tile CurrentTile { get; private set; }
        public double CurrentRotation { get; private set; }
        public event Action OnEndRoute;
        public event Action<int> OnSwitchTile;

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

        public Navigation(Engine shipEngine)
        {
            PaintMap();
            OnSwitchTile += shipEngine.OnSwitchTile;
            //OnEndRoute += 
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
        public bool IsOnRoute()
        {
            return (ChosenRoute != null) ? true : false;
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
                //OnEndRoute?.Invoke();
                var copiedActionOnEndRoute = OnEndRoute;
                copiedActionOnEndRoute?.Invoke();
                f_distanceTraveledOnCurrentTile = 0;
                Console.WriteLine("Путь пройден");
                return;
            }
            var nextTile = GetNextTile();
            if (nextTile is not null && nextTile.Id != CurrentTile.Id)
                OnSwitchTile(nextTile.Id);
            CurrentTile = nextTile;
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
            var route = Route.GetRouteFromList(FromNode, ToNode, m_availableRoutes);
            if (route == null)
            {
                BuildNewRoute(iceResistLevel);
                route = Route.GetRouteFromList(FromNode, ToNode, m_availableRoutes);
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
        
        private bool BuildNewRoute(byte iceResistLevel = 1)
        {
            if (FromNode == null || ToNode == null)
                return false;
            Route newRoute = new Route(FromNode, ToNode, iceResistLevel);
            AddRoute(newRoute);
            return true;
        }

        private void AddRoute(Route newRoute)
        {
            if (newRoute == null)
                return;
            if (!IsContainInRoutesList(newRoute))
            {
                m_availableRoutes.Add(newRoute);
                RoutesPreloader.Save(newRoute);
                var reversedRoute = Route.GetReversedRoute(newRoute);
                m_availableRoutes.Add(reversedRoute);
                RoutesPreloader.Save(reversedRoute);
            }
        }

        public void AddRoutes(Route[] routes)
        {
            foreach(var route in routes)
                AddRoute(route);
        }

        public static void AddOpenRoutes(Route[] routes)
        {
            m_availableRoutes.AddRange(routes);
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
    }
}
