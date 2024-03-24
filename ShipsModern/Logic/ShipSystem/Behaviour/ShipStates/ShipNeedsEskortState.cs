

using ShipsForm.Exceptions;
using ShipsForm.Logic.FraghtSystem;
using System;

namespace ShipsForm.Logic.ShipSystem.Behaviour.ShipStates
{
    class ShipNeedsEskortState : State
    {
        public override State NextState()
        {
            return new ShipJoinConvoyState();
        }

        public override void OnEntry(ShipBehavior sb)
        {
            var data = Data.Configuration.Instance;
            if (data is null) throw new ConfigFileDoesntExistError();
            if(sb is CargoShipBehavior csb)
                FraghtMarket.AddFraght(new EskortFraght(data.IBIceResistLevel, csb, csb.Navigation.ToNode, csb.GetFraghtInfo().ToNode));
            Console.WriteLine($"Ship [id:{sb.Ship.Id} starts looking for Icebreaker to go through ice.");
        }

        public override void OnExit(ShipBehavior sb)
        {
            Console.WriteLine($"Ship [id:{sb.Ship.Id}] assigns in convoy on MarineNode [id:{sb.Navigation.ToNode}].");
        }
    }
}
