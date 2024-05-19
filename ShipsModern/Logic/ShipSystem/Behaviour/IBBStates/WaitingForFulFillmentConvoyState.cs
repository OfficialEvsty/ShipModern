
using ShipsForm.Logic.ShipSystem.Behaviour.ShipStates;
using ShipsForm.Timers;
using System;
using System.Linq;

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
                void Interrupt(object[]? args) { ibb.IsWaitingTimeUp = true; if(isConvoyReady
                        && ibb.IsLastFraghtShipArrived 
                        && ibb.State is WaitingForFulFillmentConvoyState) ibb.GoNextState(); }
                ibb.Ship.Performer.AddPerformance(new Timers.Time(0, 0, 1, 0), Interrupt);
            }
            
        }

        public override void OnExit(ShipBehavior sb)
        {
            if (sb is IBBehavior ibb)
            {
                ibb.Convoy.EstablishConvoy(ibb.GetFraghts());             
                ibb.Convoy.ShipBehaviors.Where(x => x is not null).ToList().ForEach(x => x.OnArrived += ibb.ChecksIsConvoyCompleteRoute);
            }
                
            Console.WriteLine($"Icebreaker-[id: {sb.Ship.Id}] ends waiting of fraghts.");
        }
    }
}
