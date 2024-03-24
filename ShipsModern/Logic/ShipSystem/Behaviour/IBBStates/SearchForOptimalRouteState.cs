
using ShipsForm.Logic.NodeSystem;
using ShipsForm.Logic.ShipSystem.Behaviour.ShipStates;
using System;

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
                MarineNode mn = NetworkNodes.Network.GetNearMarineNode(ibb.GetFraghts()[0].ToNode, ibb.GetFraghts()[0].FromNode);
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
        }
    }
}
