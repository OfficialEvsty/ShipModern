

using ShipsForm.Logic.NodeSystem;
using ShipsForm.Logic.ShipSystem.Ships;
using ShipsModern.Logic.ShipSystem.ShipNavigation;

namespace ShipsForm.Logic.ShipSystem.Behaviour.ShipStates
{
    class SearchProfitRouteState : State
    {
        private bool IsEskort;
        public override State NextState()
        {
            return (IsEskort) ? new ShipNeedsEskortState() : new ShipOnRouteState(); 
        }

        public override void OnEntry(ShipBehavior sb)
        {
            //Need some rules
            var mainRoute = Route.GetMainRoute(sb.Navigation.FromNode, (Node)sb.GetFraghtInfo().ToNode, sb.Navigation.AvailableRoutes);
            var destinationNode = Route.GetReachedNode(sb.Navigation.FromNode, (Node)sb.GetFraghtInfo().ToNode, (sb.Ship as CargoShip).Shell.IceResistLevel);
            if (destinationNode is null) { throw new System.Exception(); }
            if (destinationNode is MarineNode)
                IsEskort = true;

            sb.Navigation.SetToNode(destinationNode);
            sb.Navigation.InstallRoute((sb.Ship as CargoShip).Shell.IceResistLevel);
            if (sb.Navigation.ChosenRoute != null)
                sb.GoNextState();
        }

        public override void OnExit(ShipBehavior sb)
        {
            if (sb.Navigation.FromNode is Node fromNode && fromNode != null)
                fromNode.ShipLeaveNode(sb.Ship as CargoShip);
            else if (sb.Navigation.FromNode is MarineNode fromMNode && fromMNode != null)
                fromMNode.ShipLeaveMarineNode(sb.Ship as CargoShip);
        }
    }
}
