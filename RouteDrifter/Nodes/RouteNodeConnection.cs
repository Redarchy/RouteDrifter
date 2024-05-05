using RouteDrifter.Computer;
using RouteDrifter.Models;

namespace RouteDrifter.Nodes
{
    [System.Serializable]
    public class RouteNodeConnection
    {
        public RouteComputer Computer;
        public RoutePoint Point;
        public RouteNode Node;
        public float PercentageFractionOnComputer;

        public RouteNodeConnection(RouteNode node, RouteComputer routeComputer, RoutePoint routePoint)
        {
            Node = node;
            Computer = routeComputer;
            Point = routePoint;
        }
    }
}