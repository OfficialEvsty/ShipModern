using ShipsForm.Logic.FraghtSystem;
using ShipsForm.Logic.NodeSystem;
using ShipsForm.Logic.ShipSystem.Behaviour;
using ShipsForm.Logic.ShipSystem.ShipEngine;
using ShipsForm.Logic.ShipSystem.ShipNavigation;
using ShipsForm.Logic.ShipSystem.Ships;
using System;
using System.Linq;

namespace ShipsForm.Logic.ShipSystem.IceBreakerSystem.ConvoySystem
{
    class Convoy
    {
        public class ConvoyController
        {
            private Convoy m_convoy;
            private bool AreShipsOnRoute = false;
            private INavigationController[] m_navs
            {
                get
                {
                    var ships = Array.FindAll(m_convoy.ShipBehaviors, x => x is ShipBehavior);
                    Navigation[] navs = new Navigation[ships.Length];
                    int i = 0;
                    Array.ForEach(ships, x => { navs[i] = x.Navigation; i++; });
                    return navs;
                }
            }
            private IEngineController[] m_engines 
            { 
                get 
                {
                    var ships = Array.FindAll(m_convoy.ShipBehaviors, x => x is ShipBehavior);
                    var engines = new IEngineController[ships.Length];
                    for (int i = 0; i < ships.Length; i++)
                    {
                        engines[i] = ships[i].Engine;
                    }
                    return engines;
                } 
            }
            
            public ConvoyController(Convoy con)
            {
                m_convoy = con;
            }
            public bool IsLastShipInConvoyEndRoute()
            {
                return !m_navs.Any(x => x.IsOnRoute());
            }
            public void SetConvoyRoute(GeneralNode gn, byte iceResistLevel = 1)
            {
                foreach (var nav in m_navs)
                    nav.ChooseRoute(gn, iceResistLevel);
            }
            public void SetConvoySpeed(float speed)
            {
                foreach (var engine in m_engines)
                    engine.ChangeSpeed(speed);    
            }
            public void StartEskorting()
            {
                var scheme = m_convoy.m_scheme;
                PassControllerPerformer();
                void ControlPassed(object[]? args)
                {
                    if (args is null || args.Length == 0)
                        throw new ArgumentNullException(nameof(args));
                    if (args[0] is int index)
                        m_engines[index].SwitchMode(true);
                    PassControllerPerformer();
                }
                void PassControllerPerformer()
                {
                    int index = scheme.Current++;
                    float interval = scheme.Interval;
                    if (index >= m_engines.Length)
                    {
                        scheme.Current = 0;
                        return;
                    }
                        
                    float avgSpeed = m_convoy.m_shipBehaviors[index].Engine.AverageSpeedInKM;
                    var data = Data.Configuration.Instance;
                    if (data is null)
                        throw new Exception("Config file doesn't exist in context.");
                    int tick = data.TimeTickMS;
                    int timeToPerform = (int)(interval / (avgSpeed * tick));
                    var shipBehavior = m_convoy.m_shipBehaviors[index];
                    shipBehavior.Ship.Performer.AddPerformance(new Timers.Time(timeToPerform), ControlPassed, index);
                }
            }
            public void EndEskorting(IEngineController control)
            {
                control.SwitchMode(false);
            }
        }
        public ConvoyController Controller { get; }
        private Scheme m_scheme;
        private CargoShipBehavior[]? m_shipBehaviors = null;
        private IBBehavior m_ibb; 
        private int i_maxConvoySize;

        public bool IsEskorting { get; set; }

        public CargoShipBehavior[] ShipBehaviors { get { return m_shipBehaviors; } }
        public Convoy(int maxConvoySize = 4) 
        {
            i_maxConvoySize = maxConvoySize;
            m_shipBehaviors = new CargoShipBehavior[i_maxConvoySize];
            m_scheme = new Scheme();
            Controller = new ConvoyController(this);
        }

        public void SetConvoyer(IBBehavior ibb)
        {
            m_ibb = ibb;
        }

        private int AddShip(CargoShipBehavior behavior)
        {
            int id = behavior.Ship.Id;
            int index = Array.IndexOf(m_shipBehaviors, null);
            if (index != -1)
            {
                m_shipBehaviors[index] = behavior;
                m_shipBehaviors[index].OnArrived -= (m_ibb.Ship as IceBreaker).ArrivedShipHandler;
                m_shipBehaviors[index].Navigation.OnEndRoute -= (m_ibb.Ship as IceBreaker).ArrivedShipHandler;

                return id;
            }
            throw new Exception("Can't add ship in Icebreaker's convoy.");
        }

        private bool RemoveShip(CargoShipBehavior behavior)
        {
            Controller.EndEskorting(behavior.Engine);
            int index = Array.IndexOf(m_shipBehaviors, behavior);
            if (index != -1)
            {
                m_shipBehaviors[index] = null;
                return true;
            }
            return false;
        }

        public int[] EstablishConvoy(EskortFraght[] fraghts)
        {
            int[] shipIDs = new int[fraghts.Length];
            for (int i = 0; i < fraghts.Length && fraghts[i] != null; i++)
            {
                var csb = fraghts[i].GetOrder() as CargoShipBehavior;
                shipIDs[i] = AddShip(csb);
            }
            IsEskorting = true;
            return shipIDs;
        }

        public int ReleaseShip(EskortFraght fraght)
        {
            if (fraght.GetOrder() is CargoShipBehavior csb)
            {
                var res = RemoveShip(csb);
                if (res)
                {
                    Console.WriteLine($"Ship-[id: {csb.Ship.Id}] leaves icebreaker-[id: {fraght.Fraghter.Id}] convoy.");
                    csb.LeaveConvoy();
                    return csb.Ship.Id;
                }                    
                else
                    throw new Exception($"This ship-[id: {csb.Ship.Id}] isn't member of convoy.");
            }
            throw new Exception($"This fraght-[id: {fraght.Id}] couldn't be completed.");
        }

        public void EndConvoy()
        {
            
            
        }
        public void FreeConvoy() { m_shipBehaviors = new CargoShipBehavior[i_maxConvoySize]; IsEskorting = false; }
    }
}
