using UnityEngine;

namespace RouteDrifter.Utility.Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 TurnRightAroundY(this Vector3 vector)
        {
            return new Vector3(vector.z, vector.y, -vector.x);
        }
        
        public static Vector3 TurnLeftAroundY(this Vector3 vector)
        {
            return new Vector3(-vector.z, vector.y, vector.x);
        }
    }
}