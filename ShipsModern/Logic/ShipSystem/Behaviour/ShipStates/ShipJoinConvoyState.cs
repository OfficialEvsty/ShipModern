

using ShipsForm.Logic.ShipSystem.Ships;

namespace ShipsForm.Logic.ShipSystem.Behaviour.ShipStates
{
    class ShipJoinConvoyState : State
    {
        public override State NextState()
        {
            return new ShipRoutingInConvoyState();
        }

        public override void OnEntry(ShipBehavior sb)
        {
            sb.Engine.StartEngine();
            sb.Navigation.OnEndRoute += sb.Engine.StopEngine;
            sb.Navigation.OnEndRoute += sb.GoNextState;
        }

        public override void OnExit(ShipBehavior sb)
        {
            sb.Navigation.OnEndRoute -= sb.Engine.StopEngine;
            sb.Navigation.OnEndRoute -= sb.GoNextState;
            if (sb is CargoShipBehavior csb)
            {
                csb.OnArrived?.Invoke();
            }              
        }
    }
}
