using UnityEngine;

namespace RouteDrifter.Models
{
    [System.Serializable]
    public struct RoutePoint
    {
        [SerializeField] public Vector3 _Position;
        [SerializeField] public Vector3 _Tangent;
        
        public RoutePoint(Vector3 position, Vector3 tangent) 
        {
            _Position = position;
            _Tangent = tangent;
        }
        
        public RoutePoint SetTangentsDefault()
        {
            _Tangent = new Vector3(-1, 0, 1);
            return this;
        }
    }
}