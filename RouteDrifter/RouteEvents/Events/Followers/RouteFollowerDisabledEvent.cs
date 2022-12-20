using RouteDrifter.Follower;

namespace RouteDrifter.RouteEvents.Followers
{
    public class RouteFollowerDisabledEvent : RouteDrifterEvent
    {
        public RouteFollower RouteFollower { get; private set; }

        public static RouteFollowerDisabledEvent Create(RouteFollower routeFollower)
        {
            RouteFollowerDisabledEvent routeFollowerDisabledEvent = new RouteFollowerDisabledEvent();
            routeFollowerDisabledEvent.RouteFollower = routeFollower;
            return routeFollowerDisabledEvent;
        }
    }
}