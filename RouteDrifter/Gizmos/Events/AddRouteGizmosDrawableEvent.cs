using RouteDrifter.Gizmos;
using RouteDrifter.RouteEvents;

namespace Gizmos.Events
{
    public class AddRouteGizmosDrawableEvent : RouteDrifterEvent
    {
        public IRouteGizmosDrawable RouteGizmosDrawable { get; private set; }

        public static AddRouteGizmosDrawableEvent Create(IRouteGizmosDrawable routeGizmosDrawable)
        {
            AddRouteGizmosDrawableEvent addRouteGizmosDrawableEvent = new AddRouteGizmosDrawableEvent();
            addRouteGizmosDrawableEvent.RouteGizmosDrawable = routeGizmosDrawable;

            return addRouteGizmosDrawableEvent;
        }
    }
}