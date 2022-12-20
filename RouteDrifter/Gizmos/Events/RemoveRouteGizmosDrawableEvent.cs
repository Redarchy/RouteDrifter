using RouteDrifter.Gizmos;
using RouteDrifter.RouteEvents;

namespace Gizmos.Events
{
    public class RemoveRouteGizmosDrawableEvent : RouteDrifterEvent
    {
        public IRouteGizmosDrawable RouteGizmosDrawable { get; private set; }

        public static RemoveRouteGizmosDrawableEvent Create(IRouteGizmosDrawable routeGizmosDrawable)
        {
            RemoveRouteGizmosDrawableEvent removeRouteGizmosDrawableEvent = new RemoveRouteGizmosDrawableEvent();
            removeRouteGizmosDrawableEvent.RouteGizmosDrawable = routeGizmosDrawable;

            return removeRouteGizmosDrawableEvent;
        }
    }
}