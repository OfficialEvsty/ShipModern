using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipsForm.Logic.ShipSystem.Behaviour.ShipStates
{
    class ShipWaitingForLoadingState : State
    {
        public static event StateChangedEventHadler? CheckNodeForLoading;
        public override State NextState()
        {
            return new ShipOnLoadingState();
        }
        public override void OnEntry(ShipBehavior sb)
        {
            CheckNodeForLoading?.Invoke();
            Console.WriteLine($"Корабль {sb.Ship.Id} успешно поменял свое состояние на " + sb.State);

        }

        public override void OnExit(ShipBehavior sb)
        {
            Console.WriteLine($"Корабль {sb.Ship.Id} успешно вышел из состояния" + sb.State);
        }
    }
}
