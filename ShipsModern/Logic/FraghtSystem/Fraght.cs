using ShipsForm.Logic.ShipSystem.Ships;
using ShipsForm.Logic.CargoSystem;
using ShipsForm.Logic.NodeSystem;
using ShipsForm.Logic.ShipSystem.Behaviour;
using System;
using System.Collections.Generic;

namespace ShipsForm.Logic.FraghtSystem
{
    /// <summary>
    /// Protected base Fraght class holds voyage information and his route.
    /// </summary>
    class Fraght : IComparable<Fraght>
    {
        protected static int i_counter = 0;
        protected Ship? m_fraghter;
        protected GeneralNode m_fromNode;
        protected GeneralNode m_toNode;

        /// <summary>
        /// Protected ctor triggers by subclasses because they have general interfaces.
        /// </summary>
        /// <param name="from">GeneralNode where fraghted ship starts route.</param>
        /// <param name="to">GeneralNode where fraghted ship ends route.</param>
        protected Fraght(GeneralNode from, GeneralNode to) 
        {
            Id = ++i_counter;
            m_fromNode = from;
            m_toNode = to;
        }

        public int Id { get; protected set; }
        /// <summary>
        /// Ship who performing route.
        /// </summary>
        public Ship Fraghter 
        { 
            get 
            { 
                if (m_fraghter is null) throw new Exception("This fraght still has not charter ship."); 
                return m_fraghter; 
            } 
            protected set 
            { 
                m_fraghter = value; 
            } 
        }
        public GeneralNode FromNode { get => m_fromNode;}
        public GeneralNode ToNode { get => m_toNode; }

        /// <summary>
        /// Adds concrete charter ship who will fulfill route.
        /// </summary>
        /// <param name="charterShip">Chartering ship</param>
        /// <returns>Returns true if corresponding requirements, in other cases returns false.</returns>
        public virtual bool Charter(Ship charterShip)
        {
            if (charterShip is null)
                return false;
            m_fraghter = charterShip;
            FraghtMarket.Fraghts.Remove(this);
            return true;
        }

        public int CompareTo(Fraght? other)
        {
            if (this is null && other is null)
                return 0;
            if (other is null)
                return 1;
            if (this is null)
                return -1;
            SupportEntities.Point p1 = this.FromNode.GetCoords;
            float dist1 = p1.GetDistance(this.ToNode.GetCoords);
            float dist2 = p1.GetDistance(other.ToNode.GetCoords);
            var result = -dist1.CompareTo(dist2);
            return result;
        }
    }

    /// <summary>
    /// CargoFraght class due to cargo ships. There is information about which types of cargos should be on boarding.
    /// </summary>
    sealed class CargoFraght : Fraght
    {
        private Dictionary<Cargo, int> m_requiredCargo;
        public Dictionary<Cargo, int> RequiredCargo { get { return m_requiredCargo; } }

        /// <summary>
        /// Extanded fraght information for cargo ships who onboarded conctracted cargo.
        /// </summary>
        /// <param name="reqCargo">Contracted cargo.</param>
        /// <param name="from">GeneralNode where chartered ship starts route.</param>
        /// <param name="to">GeneralNode where chartered ship ends route.</param>
        public CargoFraght(Dictionary<Cargo, int> reqCargo, Node from, Node to) : base(from, to)
        {
            m_requiredCargo = reqCargo;
            FraghtMarket.AddFraght(this);
            Console.WriteLine("CargoFraght created.");
        }
    }

    /// <summary>
    /// EskortFraght is IceBreaker's fraght for accompaniment cargo ships through ice fields.
    /// </summary>
    sealed class EskortFraght :  Fraght 
    {
        private ShipBehavior m_order;
        private byte i_requiredIceResistLevel;
        /// <summary>
        /// Ice resistance indicates requirements to Icebreaker's shell which allows providing routes through ice.
        /// </summary>
        public byte RequiredIceResistLevel { get {  return i_requiredIceResistLevel; } }

        public EskortFraght(byte iceR, ShipBehavior owner, GeneralNode from, GeneralNode to) : base(from, to)
        {
            i_requiredIceResistLevel = iceR;
            m_order = owner;
            Console.WriteLine("Eskort Fraght successfully created.");
        }

        public override bool Charter(Ship charterShip)
        {
            if (charterShip is null)
                return false;
            m_fraghter = charterShip;
            if(m_order is CargoShipBehavior csb)
            {
                csb.OnArrived += ((IceBreaker)charterShip).ArrivedShipHandler;
                csb.GoNextState();
            }
                
            FraghtMarket.Fraghts.Remove(this);

            return true;
        }
        public ShipBehavior GetOrder()
        {
            return m_order;
        }
    }
}
