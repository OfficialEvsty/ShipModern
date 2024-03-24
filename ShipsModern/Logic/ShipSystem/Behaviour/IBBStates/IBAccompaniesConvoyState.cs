
using ShipsForm.Logic.ShipSystem.Behaviour.ShipStates;
using System;

namespace ShipsForm.Logic.ShipSystem.Behaviour.IBBStates
{
    class IBAccompaniesConvoyState : State
    {
        public override State NextState()
        {
            throw new NotImplementedException();
        }

        public override void OnEntry(ShipBehavior sb)
        {
            sb.Engine.ChangeSpeed(sb.Engine.CaravanSpeedInKM);
            sb.Engine.StartEngine();
            sb.Navigation.OnEndRoute += sb.GoNextState;
            sb.Navigation.OnEndRoute += sb.Engine.StopEngine;
            if (sb is IBBehavior ibb)
                ibb.Convoy.Controller.StartEskorting();
        }

        public override void OnExit(ShipBehavior sb)
        {
            sb.Navigation.OnEndRoute -= sb.GoNextState;
            sb.Navigation.OnEndRoute -= sb.Engine.StopEngine;
        }
    }
}
