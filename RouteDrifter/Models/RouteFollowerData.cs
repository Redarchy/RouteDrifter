using Unity.Collections;
using UnityEngine;

namespace RouteDrifter.Models
{
    [System.Serializable]
    public struct RouteFollowerData
    {
        [SerializeField, ReadOnly] public string _ID;
        [Range(0f, 1f)] public float CurrentPercentage;
        
        public bool Follow;
        public bool FaceDirection;
        public bool FollowBackwards;
        
        public Vector3 OffsetByLocalDirection;

        public RouteFollowerMovementType MovementType; 
        
        [Range(0f, 100f)]
        public float LinearSpeed;
        
        [Range(0f, 90f)]
        public float AngularSpeed;

        
        public RouteFollowerData(
            string id,
            Vector3 offsetByLocalDirection,
            RouteFollowerMovementType movementType,
            float currentPercentage = 0f, 
            bool follow = true, 
            bool faceDirection = true, 
            bool followBackwards = false,
            float linearSpeed = 1f, 
            float angularSpeed = 1f
        )
        {
            this._ID = id;
            this.OffsetByLocalDirection = offsetByLocalDirection;
            this.CurrentPercentage = currentPercentage;
            this.Follow = follow;
            this.FaceDirection = faceDirection;
            this.FollowBackwards = followBackwards;
            this.MovementType = movementType;
            this.LinearSpeed = linearSpeed;
            this.AngularSpeed = angularSpeed;
            
        }
    }
}