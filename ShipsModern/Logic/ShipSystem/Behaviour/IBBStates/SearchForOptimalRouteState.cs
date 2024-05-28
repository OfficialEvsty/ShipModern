
using ShipsForm.Data;
using ShipsForm.GUI;
using ShipsForm.Logic.NodeSystem;
using ShipsForm.Logic.ShipSystem.Behaviour.ShipStates;
using System;
using System.Linq;

namespace ShipsForm.Logic.ShipSystem.Behaviour.IBBStates
{
    class SearchForOptimalRouteState : State
    {
        public override State NextState()
        {
            return new IBAccompaniesConvoyState();
        }

        public override void OnEntry(ShipBehavior sb)
        {
            if(sb is IBBehavior ibb)
            {
                ibb.GetFraghtInfo();
                MarineNode mn = NetworkNodes.Network.Host.GetNearMarineNode(ibb.CurrentActiveFraght.ToNode, ibb.CurrentActiveFraght.FromNode);
                ibb.Navigation.ChooseRoute(mn, ibb.Shell.IceResistLevel);
                if (ibb.Navigation.ChosenRoute != null)
                {
                    ibb.Convoy.Controller.SetConvoyRoute(mn, ibb.Shell.IceResistLevel);
                    ibb.GoNextState();
                }
            }
        }
        
        public override void OnExit(ShipBehavior sb)
        {
            Console.WriteLine($"Icebreaker-[id: {sb.Ship.Id}] ends searching route and set uped ships.");
            Painter.UpdateCaravanShips?.Invoke((sb as IBBehavior).Convoy.ShipBehaviors.Where(x => x is not null).Select(x => x.Ship.Id).ToArray());
        }
    }
}
