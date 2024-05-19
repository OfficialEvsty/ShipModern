

using ShipsForm.Logic.NodeSystem;
using ShipsForm.Logic.ShipSystem.Ships;
using ShipsModern.Logic.ShipSystem.ShipNavigation;
using System;

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
            var iceResistanceLevel = (sb.Ship as CargoShip).Shell.IceResistLevel;
            (GeneralNode mn1, GeneralNode mn2) splittedMNodes = Route.GetReachedNodes(mainRoute, iceResistanceLevel);
            if (mainRoute.IsIceZone())
            {                
                if (splittedMNodes.mn1 is MarineNode)
                    IsEskort = true;

                sb.Navigation.AddRoutes(mainRoute.SplitMainRoute(splittedMNodes, iceResistanceLevel));
            }
            else
            {
                IsEskort = false;
            }
            sb.Navigation.AddRoutes(new Route[] {mainRoute});

            sb.Navigation.SetToNode((IsEskort) ? splittedMNodes.mn1 : splittedMNodes.mn2);
            sb.Navigation.InstallRoute(iceResistanceLevel);
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
