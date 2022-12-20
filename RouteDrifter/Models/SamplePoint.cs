using UnityEngine;

namespace RouteDrifter.Models
{
    public struct SamplePoint
    {
        public Vector3 _LocalPosition;
        public Vector3 _Forward;
        
        public SamplePoint(Vector3 localPosition, Vector3 forward)
        {
            _LocalPosition = localPosition;
            _Forward = forward;
        }
    }

}