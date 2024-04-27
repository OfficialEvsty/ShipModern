
using ShipsForm.Logic.FraghtSystem;
using ShipsForm.Logic.ShipSystem.Behaviour.ShipStates;
using System;
using System.Linq;

namespace ShipsForm.Logic.ShipSystem.Behaviour.IBBStates
{
    class LookingForAnyShipWantsToJoinConvoy : State
    {
        public override State NextState()
        {
            return new WaitingForFulFillmentConvoyState();
        }

        public override void OnEntry(ShipBehavior sb)
        {
            if (sb is IBBehavior ibb)
                FraghtMarket.Fraghts.ToList().ForEach(fraght => ibb.Update(fraght));
            Console.WriteLine($"IceBreaker-[id: {sb.Ship.Id}] awaits additionally fraghts to appear.");
        }

        public override void OnExit(ShipBehavior sb)
        {
            Console.WriteLine($"IceBreaker-[id: {sb.Ship.Id}] found the eskort fraght-[id: {((IBBehavior)sb).GetFraghts()[0].Id}] for ice routing.");
        }
    }
}
