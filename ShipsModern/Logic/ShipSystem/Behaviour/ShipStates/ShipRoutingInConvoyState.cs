

using System;

namespace ShipsForm.Logic.ShipSystem.Behaviour.ShipStates
{
    class ShipRoutingInConvoyState : State
    {
        public override State NextState()
        {
            return new SearchProfitRouteState();
        }

        public override void OnEntry(ShipBehavior sb)
        {
            if(sb is CargoShipBehavior csb)
            {
                csb.Navigation.OnEndRoute += csb.OnArrived;
            }
            
        }

        public override void OnExit(ShipBehavior sb)
        {
            if (sb is CargoShipBehavior csb)
            {
                sb.Navigation.OnEndRoute -= csb.OnArrived;
                csb.OnArrived = null;
            }
            
            Console.WriteLine($"Ship-[id: {sb.Ship.Id}] leaves convoy.");
        }
    }
}
