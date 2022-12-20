using UnityEngine;

namespace RouteDrifter.Models
{
    public struct RouteSegment
    {
        public Vector3 _StartPosition;
        public Vector3 _EndPosition;
        public Vector3 _StartTangent;
        public Vector3 _EndTangent;
        
        public RouteSegment(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent)
        {
            _StartPosition = startPosition;
            _EndPosition = endPosition;
            _StartTangent = startTangent;
            _EndTangent = endTangent;
        }
        
        public void SetTangentsDefault()
        {
            _StartTangent = new Vector3(-1, 0, 1);
            _EndTangent = new Vector3(1, 0, 1);
        }
    }
}