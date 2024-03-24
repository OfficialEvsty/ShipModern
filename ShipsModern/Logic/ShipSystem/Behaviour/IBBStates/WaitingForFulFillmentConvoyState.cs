
using ShipsForm.Logic.ShipSystem.Behaviour.ShipStates;
using ShipsForm.Timers;
using System;

namespace ShipsForm.Logic.ShipSystem.Behaviour.IBBStates
{
    class WaitingForFulFillmentConvoyState : State
    {
        
        public override State NextState()
        {
            return new SearchForOptimalRouteState();
        }

        public override void OnEntry(ShipBehavior sb)
        {
            if (sb is IBBehavior ibb)
            {
                bool isWaiting = sb.State is WaitingForFulFillmentConvoyState;
                bool isConvoyReady = isWaiting && sb is not null;
                void Interrupt(object[]? args) { ibb.IsWaitingTimeUp = true; if(isConvoyReady && ibb.IsLastFraghtShipArrived) ibb.GoNextState(); }
                ibb.Ship.Performer.AddPerformance(new Timers.Time(0, 0, 1, 0), Interrupt);
            }
            
        }

        public override void OnExit(ShipBehavior sb)
        {
            if (sb is IBBehavior ibb)
                ibb.Convoy.EstablishConvoy(ibb.GetFraghts());
            Console.WriteLine($"Icebreaker-[id: {sb.Ship.Id}] ends waiting of fraghts.");
        }
    }
}
