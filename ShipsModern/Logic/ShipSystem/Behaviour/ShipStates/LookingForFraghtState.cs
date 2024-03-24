using ShipsForm.Logic.FraghtSystem;
using ShipsForm.Logic.NodeSystem;
using ShipsForm.Logic.ShipSystem.Behaviour;
using System;

namespace ShipsForm.Logic.ShipSystem.Behaviour.ShipStates
{
    class LookingForFraghtState : State
    {
        public override void OnEntry(ShipBehavior sb)
        {
            if(sb is CargoShipBehavior cargoSb)
            {
                Console.WriteLine($"Корабль {sb.Ship.Id} успешно поменял свое состояние на " + sb.State);
                //cargoSb.LookingForFraght(sb.Navigation.FromNode);
            }
        }

        public override void OnExit(ShipBehavior sb)
        {
            if (sb is CargoShipBehavior cargoSb)
            {
                CargoFraght cargoFraght = cargoSb.GetFraghtInfo();
                Console.WriteLine($"Корабль {cargoSb.Ship.Id} успешно вышел из состояния" + cargoSb.State);
                    if (cargoSb.Navigation.FromNode != null && cargoFraght != null)
                        if (cargoSb.GetFraghtInfo()?.FromNode is Node fromNode)
                            fromNode.LoadingSection.ContractAvailableCargo(cargoFraght);
            }            
        }

        public override State NextState()
        {
            return new ShipWaitingForLoadingState();
        }
    }
}
