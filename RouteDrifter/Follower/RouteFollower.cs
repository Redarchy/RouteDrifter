using RouteDrifter.Gizmos;
using Gizmos.Events;
using RouteDrifter.RouteEvents;
using RouteDrifter.RouteEvents.Followers;
using RouteDrifter.Utility.Attributes;
using UnityEngine;

namespace RouteDrifter.Follower
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(-98)]
    public class RouteFollower : RouteFollowerContainer, IRouteGizmosDrawable
    {
        public bool Follow { get => _Follow; set => _Follow = value; }
        
        private void Awake()
        {
            Initialize();
        }
        
        private void OnEnable()
        {
            RouteDrifterEventManager.Send(AddRouteGizmosDrawableEvent.Create(this));
            RouteDrifterEventManager.Send(RouteFollowerEnabledEvent.Create(this));
        }

        private void OnDisable()
        {
            RouteDrifterEventManager.Send(RemoveRouteGizmosDrawableEvent.Create(this));
            RouteDrifterEventManager.Send(RouteFollowerDisabledEvent.Create(this));
        }
        
        #region IRouteGizmosDrawable

        public void DrawGizmos()
        {
            UnityEngine.Gizmos.color = Color.white;
            UnityEngine.Gizmos.DrawSphere(_Position, 0.05f);
        }

        #endregion
        
        #region Editor Functionalities
        
        [RouteInspectorButton("Set Percentage")]
        private void Editor_SetPercentage()
        {
            SetPercentage(_CurrentPercentage);
        }
                
        // [RouteInspectorButton("Find Closest Percentage To World Point")]
        // private void Editor_FindClosestPercentageToWorldPoint(Transform transform)
        // {
        //     Vector3 worldPoint = transform.position;
        //     transform.position = _RouteFollowerContainer.RouteComputer.GetWorldPositionByPercentage(RouteComputer.GetClosestPercentageToWorldPoint(worldPoint));
        // }
        
        #endregion

        public void SetLinearSpeed(float speed)
        {
            _LinearSpeed = speed;
        }

        public void SetFaceDirection(bool faceDirection)
        {
            _FaceDirection = faceDirection;
        }

        public void SetOffset(Vector3 offset)
        {
            _OffsetByLocalDirection = offset;
        }
    }
}