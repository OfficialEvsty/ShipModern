

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
            sb.Navigation.OnEndRoute += (sb as CargoShipBehavior).OnArrived;
        }

        public override void OnExit(ShipBehavior sb)
        {
            sb.Navigation.OnEndRoute -= (sb as CargoShipBehavior).OnArrived;
            Console.WriteLine($"Ship-[id: {sb.Ship.Id}] leaves convoy.");
        }
    }
}
