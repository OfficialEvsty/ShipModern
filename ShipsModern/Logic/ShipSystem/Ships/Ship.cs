using ShipsForm.Logic.ShipSystem.ShipNavigation;
using ShipsForm.Logic.ShipSystem.ShipEngine;
using ShipsForm.Logic.ShipSystem.Behaviour;
using ShipsForm.Logic.FraghtSystem;
using ShipsForm.Graphic;
using ShipsForm.Logic.TilesSystem;
using ShipsForm.Timers.PerformLogic;
using ShipsForm.Timers;
using System.Windows.Media;

namespace ShipsForm.Logic.ShipSystem.Ships
{
    abstract class Ship : IDrawable
    {
        protected static int id_counter = 0;
        public int Id { get; protected set; }
        public IPathDrawable PathObserver { get { return m_navigation; } }
        protected Navigation m_navigation;
        protected Engine m_engine;

        public Performer Performer { get; } = new Performer(TimerData.Timer);

        abstract public void Update();

        public abstract ImageSource GetSkin(int size);
        public abstract SupportEntities.Point? GetCurrentPoint();
        public abstract double GetRotation();
        public abstract int GetSize();

    }
}
