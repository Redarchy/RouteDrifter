using RouteDrifter.Follower;

namespace RouteDrifter.RouteEvents.Followers
{
    public class RouteFollowerReachedEndEvent : RouteDrifterEvent
    {
        public RouteFollower RouteFollower { get; private set; }

        public static RouteFollowerReachedEndEvent Craete(RouteFollower routeFollower)
        {
            RouteFollowerReachedEndEvent routeFollowerReachedEndEvent = new RouteFollowerReachedEndEvent();
            routeFollowerReachedEndEvent.RouteFollower = routeFollower;
            return routeFollowerReachedEndEvent;
        }
    }
}