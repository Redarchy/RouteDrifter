using RouteDrifter.Follower;

namespace RouteDrifter.RouteEvents.Followers
{
    public class RouteFollowerEnabledEvent : RouteDrifterEvent
    {
        public RouteFollower RouteFollower { get; private set; }

        public static RouteFollowerEnabledEvent Create(RouteFollower routeFollower)
        {
            RouteFollowerEnabledEvent routeFollowerEnabledEvent = new RouteFollowerEnabledEvent();
            routeFollowerEnabledEvent.RouteFollower = routeFollower;
            return routeFollowerEnabledEvent;
        }
    }
}