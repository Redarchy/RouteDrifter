using RouteDrifter.Computer;

namespace RouteDrifter.RouteEvents.Computers
{
    public class RouteComputerEnabledEvent : RouteDrifterEvent
    {
        public RouteComputer RouteComputer { get; private set; }
        
        public static RouteComputerEnabledEvent Create(RouteComputer routeComputer)
        {
            RouteComputerEnabledEvent routeComputerEnabledEvent = new RouteComputerEnabledEvent();
            routeComputerEnabledEvent.RouteComputer = routeComputer;
            return routeComputerEnabledEvent;
        }

    }
}