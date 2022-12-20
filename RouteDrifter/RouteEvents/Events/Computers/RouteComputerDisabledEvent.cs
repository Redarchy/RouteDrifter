using RouteDrifter.Computer;

namespace RouteDrifter.RouteEvents.Computers
{
    public class RouteComputerDisabledEvent : RouteDrifterEvent
    {
        public RouteComputer RouteComputer { get; private set; }
        
        public static RouteComputerDisabledEvent Create(RouteComputer routeComputer)
        {
            RouteComputerDisabledEvent routeComputerDisabledEvent = new RouteComputerDisabledEvent();
            routeComputerDisabledEvent.RouteComputer = routeComputer;
            return routeComputerDisabledEvent;
        }
    }
}