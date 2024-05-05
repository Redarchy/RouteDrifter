using Unity.Collections;
using UnityEngine;

namespace RouteDrifter.Models
{
    [System.Serializable]
    public struct RoutePoint
    {
        [SerializeField, ReadOnly] public int Index;
        [SerializeField] public Vector3 Position;
        [SerializeField] public Vector3 Tangent;
        
        public RoutePoint(int index, Vector3 position, Vector3 tangent)
        {
            Index = index;
            Position = position;
            Tangent = tangent;
        }
        
        public RoutePoint SetTangentsDefault()
        {
            Tangent = new Vector3(-1, 0, 1);
            return this;
        }
    }
}