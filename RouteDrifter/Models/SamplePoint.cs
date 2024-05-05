using Unity.Collections;
using UnityEngine;

namespace RouteDrifter.Models
{
    [System.Serializable]
    public struct SamplePoint
    {
        [ReadOnly] public Vector3 LocalPosition;
        [ReadOnly] public Vector3 Forward;
        [ReadOnly] public float Percentage;
        
        public SamplePoint(Vector3 localPosition, Vector3 forward)
        {
            LocalPosition = localPosition;
            Forward = forward;
            Percentage = 0f;
        }
    }

}