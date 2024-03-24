using ShipsForm.Logic.NodeSystem;
using ShipsForm.Logic.ShipSystem.Ships;
using ShipsForm.SupportEntities;
using System.Collections.Generic;

namespace ShipsForm.Logic.CargoSystem
{
    class Cargo
    {
        private SupportEntities.Point? m_crds;

        public SupportEntities.Point Coords { get { return m_crds; } }

        public void ConnectToShip(Ship ship)
        {

        }

        public Cargo(Node nodeToSpawn)
        {
            m_crds = nodeToSpawn.GetCoords;
        }

        public Cargo(SupportEntities.Point pointToSpawn)
        {
            m_crds = pointToSpawn;
        }
    }

    class CargoContainer : Cargo
    {
        public CargoContainer(Node node) : base(node) { }
        public CargoContainer(SupportEntities.Point point) : base(point) { }
    }

    class CargoTanker : Cargo
    {
        public CargoTanker(Node node) : base(node) { }
        public CargoTanker(SupportEntities.Point point) : base(point) { }
    }

    interface ICargoFactory
    {
        CargoContainer CreateContainer(Node attachedToNode);
        CargoTanker CreateTanker(Node attachedToNode);
    }

    class CargoFactory : ICargoFactory
    {
        public CargoContainer CreateContainer(Node attachedToNode)
        {
            return new CargoContainer(attachedToNode);
        }
        public CargoTanker CreateTanker(Node attachedToNode)
        {
            return new CargoTanker(attachedToNode);
        }
    }

    static class CargoConstructor
    {
        private static CargoFactory m_factory = new CargoFactory();
        public static CargoFactory Factory { get { return m_factory; } }

        public static Dictionary<Cargo, int> AssignNewCargo(int quantity)
        {
            Dictionary<Cargo, int> newCargo = new Dictionary<Cargo, int>();
            Node emptyNode = new Node(new SupportEntities.Point(0, 0), 5);
            var cargoType = m_factory.CreateContainer(emptyNode);
            newCargo.Add(cargoType, quantity);
            return newCargo;
        }
    }
}
