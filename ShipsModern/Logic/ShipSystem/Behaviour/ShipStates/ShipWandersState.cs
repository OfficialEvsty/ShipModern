

using ShipsForm.Logic.NodeSystem;
using ShipsForm.Logic.ShipSystem.Ships;
using System;

namespace ShipsForm.Logic.ShipSystem.Behaviour.ShipStates
{
    class ShipWandersState : State
    {
        public override State NextState()
        {
            return new ShipInNodeState();
        }

        public override void OnEntry(ShipBehavior sb)
        {
            Console.WriteLine("Корабль успешно поменял свое состояние на " + sb.State);
            if (sb.Navigation.CurrentNode is Node currentNode && currentNode != null)
                currentNode.ShipTryEnterInNode(sb.Ship as CargoShip);
            Console.WriteLine($"Текущий нод корабля {sb.Navigation.CurrentNode}");
        }

        public override void OnExit(ShipBehavior sb)
        {
            Console.WriteLine("Корабль успешно вышел из состояния" + sb.State);
        }
    }
}
