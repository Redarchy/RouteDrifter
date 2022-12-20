using UnityEngine;

namespace RouteDrifter.Models
{
    public class RouteBezierCurve
    {
        public RouteSegment RouteSegment => _routeSegment;
        
        private RouteSegment _routeSegment;

        public RouteBezierCurve(RoutePoint startPoint, RoutePoint endPoint)
        {
            _routeSegment = new RouteSegment(startPoint._Position, endPoint._Position, startPoint._Tangent, endPoint._Tangent);
        }
        
        // Equations from: https://en.wikipedia.org/wiki/B%C3%A9zier_curve
        public Vector3 GetPointInCurveWithFraction(float fraction)
        {
            fraction = Mathf.Clamp01(fraction);

            float reversedTerm = 1 - fraction;

            return (reversedTerm * reversedTerm * reversedTerm * _routeSegment._StartPosition)
                   + (3 * reversedTerm * reversedTerm * fraction * (_routeSegment._StartPosition + _routeSegment._StartTangent))
                   + (3 * reversedTerm * fraction * fraction * (_routeSegment._EndPosition + _routeSegment._EndTangent))
                   + (fraction * fraction * fraction * _routeSegment._EndPosition);
        }
        
    }

}