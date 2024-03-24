using ShipsForm.SupportEntities.PatternObserver;
using System;

namespace ShipsForm.Logic.ShipSystem.Behaviour.ShipStates
{
    class ShipOnLoadingState : State
    {
        public event StateChangedEventHadler? ReadyLoad;

        public override State NextState()
        {
            return new SearchProfitRouteState();
        }

        public override void OnEntry(ShipBehavior sb)
        {
            Console.WriteLine($"Корабль {sb.Ship.Id} успешно поменял свое состояние на " + sb.State);
            EventObservable.NotifyObservers(this);
            ReadyLoad?.Invoke();
        }

        public override void OnExit(ShipBehavior sb)
        {

            Console.WriteLine($"Корабль {sb.Ship.Id} успешно вышел из состояния" + sb.State);
        }
    }
}
