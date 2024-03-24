

using ShipsForm.Logic.NodeSystem;
using ShipsForm.Logic.ShipSystem.Ships;

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
            IsEskort = true;
            GeneralNode destination;
            if (IsEskort)
                destination = NetworkNodes.Network.GetNearMarineNode(sb.Navigation.FromNode);
            else
                destination = sb.GetFraghtInfo().ToNode;
            sb.Navigation.SetToNode(destination);
            sb.Navigation.InstallRoute();
            if (sb.Navigation.ChosenRoute != null)
                sb.GoNextState();
        }

        public override void OnExit(ShipBehavior sb)
        {
            if (sb.Navigation.FromNode is Node fromNode && fromNode != null)
                fromNode.ShipLeaveNode(sb.Ship as CargoShip);
        }
    }
}
