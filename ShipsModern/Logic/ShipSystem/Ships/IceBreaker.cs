using ShipsForm.Logic.ShipSystem.ShipNavigation;
using ShipsForm.Logic.ShipSystem.ShipEngine;
using ShipsForm.Logic.ShipSystem.IceBreakerSystem.ConvoySystem;
using ShipsForm.Logic.ShipSystem.IceBreakerSystem;
using ShipsForm.Logic.ShipSystem.Behaviour;
using ShipsForm.Logic.FraghtSystem;
using ShipsForm.Logic.NodeSystem;
using ShipsForm.Exceptions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ShipsModern.SupportEntities;
using ShipsForm.Graphic;
using ShipsForm.SupportEntities.PatternObserver;

namespace ShipsForm.Logic.ShipSystem.Ships
{
    class IceBreaker : Ship
    {
        private IBBehavior m_behavior;
        private Shell m_shell;        
        private Convoy m_convoy;
        private EskortFraght[] m_fraghts;

        public EskortFraght[] Fraghts { get {  return m_fraghts; } }
        public IceBreaker(MarineNode marineNode)
        {
            Id = Manager.GetGuiElementID();
            m_navigation = new Navigation();
            m_engine = new Engine();
            var data = Data.Configuration.Instance;
            if (data is null)
                throw new ConfigFileDoesntExistError();
            m_shell = new Shell(data.IBIceResistLevel);
            m_convoy = new Convoy();
            m_fraghts = new EskortFraght[4];
            m_behavior = new IBBehavior(this, m_navigation, m_engine, m_shell, m_convoy, m_fraghts);
            m_navigation.FromNode = marineNode;
            EventObservable.NotifyObservers((IDrawable)this);
        }

        public Convoy? Convoy { get { return m_convoy; } }

        public void ArrivedShipHandler()
        {
            m_behavior.FraghtShipArrived();
        }

        public override void Update()
        {
            if (m_navigation != null && m_engine != null)
            {
                if (m_navigation.ChosenRoute is null)
                    return;
                float passedDistance = m_engine.Running();
                m_navigation.ObserveMoving(passedDistance);
            }
        }


        public override ImageSource GetSkin(int size)
        {
            var modelName = "Icebreaker";
            return ((IDrawable)this).DownloadImage(modelName, size);
        }

        public override SupportEntities.Point? GetCurrentPoint()
        {
            var tile = m_navigation.CurrentTile;
            if (tile is null)
                return null;
            return new SupportEntities.Point(tile.X, tile.Y);
        }

        public override double GetRotation()
        {
            return m_navigation.CurrentRotation;
        }
        public override int GetSize()
        {
            return Data.Configuration.Instance.IcebreakerImageSize;
        }
    }
}
