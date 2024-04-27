using ShipsForm.Logic.ShipSystem.Ships;
using ShipsForm.Logic.CargoSystem;
using ShipsForm.Graphic;
using ShipsForm.Logic.TilesSystem;
using ShipsForm.Exceptions;
using System.Collections.Generic;
using System;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using ShipsModern.SupportEntities;
using ShipsForm.SupportEntities.PatternObserver;

namespace ShipsForm.Logic.NodeSystem
{
    sealed class Node : GeneralNode
    {
        private LoadingSection m_loadingSection;
        private List<CargoShip> m_ships = new List<CargoShip>();
        private Dictionary<Cargo, decimal> m_priceCargo = new Dictionary<Cargo, decimal>();
        private int i_maxNodeSize;

        public LoadingSection LoadingSection { get { return m_loadingSection; } }
        public List<CargoShip> Ships { get { return m_ships; } }

        public Dictionary<Cargo, decimal> GetPriceCargo { get { return m_priceCargo; } }
        public int MaxNodeSize { get { return i_maxNodeSize; } }




        public Node(SupportEntities.Point relatedPointToSetNode, int maxSize)
        {
            Id = Manager.GetGuiElementID();
            m_loadingSection = new LoadingSection(this, maxSize / 3);
            m_relatedPoint = relatedPointToSetNode;
            i_maxNodeSize = maxSize;
            EventObservable.NotifyObservers((IDrawable)this);
        }


        public bool ShipTryEnterInNode(CargoShip enteredShip)
        {
            if (m_ships.Count < MaxNodeSize)
            {
                m_ships.Add(enteredShip);
                enteredShip.Behavior.SetStartNode(this);
                enteredShip.Behavior.GoNextState();
                Console.WriteLine($"{enteredShip} вошёл в порт {this}");
                return true;
            }
            Console.WriteLine($"{enteredShip} не может зайти в порт {this}. Причина: Порт заполнен");
            return false;
        }

        public void ShipLeaveNode(CargoShip leavedShip)
        {
            for (int i = 0; i < m_ships.Count; i++)
            {
                if (m_ships[i] == leavedShip)
                {
                    Console.WriteLine($"Корабль {m_ships[i]} покинул порт {this}");
                    m_ships.RemoveAt(i);
                }
            }
        }

        public override SupportEntities.Point? GetCurrentPoint()
        {
            var tile = TileCoords;
            return new SupportEntities.Point(tile.X, tile.Y);
        }

        public override ImageSource GetSkin(int size)
        {
            var modelName = "Node";
            return ((IDrawable)this).DownloadImage(modelName, size);
        }

        public override int GetSize()
        {
            var data = Data.Configuration.Instance;
            if (data is null)
                throw new ConfigFileDoesntExistError();
            return data.NodeImageSize;
        }
    }
}

