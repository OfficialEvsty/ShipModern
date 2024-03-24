

using System;

namespace ShipsForm.Logic.ShipSystem.Behaviour.ShipStates
{
    class ShipInNodeState : State
    {
        public override State NextState()
        {
            return new LookingForFraghtState();
        }

        public override void OnEntry(ShipBehavior sb)
        {
            CargoShipBehavior? csb = sb as CargoShipBehavior;
            if (csb == null)
                return;

            Console.WriteLine($"Корабль {sb.Ship.Id} успешно поменял свое состояние на " + sb.State);
            //временно
            if (sb.GetFraghtInfo() != null && sb.Navigation.ToNode == sb.Navigation.CurrentNode)
            {
                Console.WriteLine("Груз доставлен!!!");
                csb.BoardCargo.Unloading();
            }

            sb.GoNextState();
        }

        public override void OnExit(ShipBehavior sb)
        {
            CargoShipBehavior? csb = sb as CargoShipBehavior;
            if (csb == null)
                return;
            Console.WriteLine($"Корабль {csb.Ship.Id} успешно вышел из состояния" + csb.State);
        }
    }
}
