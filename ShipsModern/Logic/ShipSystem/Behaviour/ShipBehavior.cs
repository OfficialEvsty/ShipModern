using ShipsForm.SupportEntities.PatternObserver.Observers;
using ShipsForm.Logic.ShipSystem.Behaviour.ShipStates;
using ShipsForm.Logic.ShipSystem.ShipCargoCompartment;
using ShipsForm.Logic.ShipSystem.ShipNavigation;
using ShipsForm.SupportEntities.PatternObserver;
using ShipsForm.Logic.ShipSystem.ShipEngine;
using ShipsForm.Logic.ShipSystem.Ships;
using ShipsForm.Logic.FraghtSystem;
using ShipsForm.Logic.NodeSystem;

namespace ShipsForm.Logic.ShipSystem.Behaviour
{
    abstract class ShipBehavior : IEventObserver<Fraght>
    {
        protected Ship m_ship;
        protected Engine m_engine;
        protected Navigation m_navigation;


        public Ship Ship { get { return m_ship; } }
        public State State { get; set; }

        public Navigation Navigation { get { return m_navigation; } }
        public Engine Engine { get { return m_engine; } }


        public abstract Fraght? GetFraghtInfo();

        public abstract void SetStartNode(GeneralNode nd);

        /// <summary>
        /// Switching states logic.
        /// </summary>
        public void GoNextState()
        {
            State.OnExit(this);
            State = State.NextState();
            State.OnEntry(this);
        }


        public abstract void Update(Fraght ev);
    }
}