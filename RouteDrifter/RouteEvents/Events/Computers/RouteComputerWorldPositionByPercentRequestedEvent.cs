using System;
using UnityEngine;

namespace RouteDrifter.RouteEvents.Computers
{
    public class RouteComputerWorldPositionByPercentRequestedEvent : RouteDrifterEvent
    {
        public float Percentage { get; private set; }
        public Action<Vector3> Callback { get; private set; }
        
        public static RouteComputerWorldPositionByPercentRequestedEvent Create(float percentage, Action<Vector3> callback)
        {
            RouteComputerWorldPositionByPercentRequestedEvent customEvent = new RouteComputerWorldPositionByPercentRequestedEvent();
            customEvent.Percentage = percentage;
            customEvent.Callback = callback;
            return customEvent;
        }

        
    }
}