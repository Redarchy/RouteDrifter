using System;
using RouteDrifter.Computer;
using RouteDrifter.Models;
using RouteDrifter.Utility.Extensions;
using Unity.Collections;
using UnityEngine;

namespace RouteDrifter.Follower
{
    public class RouteFollowerContainer : MonoBehaviour
    {
        #region Inspector
        
        [SerializeField] public RouteComputer _RouteComputer;
        [SerializeField] [Range(0, 1f)] public float _CurrentPercentage;
        [SerializeField] protected bool _Follow;
        [SerializeField] public bool _FaceDirection;
        [SerializeField] public RouteFollowerMovementType _MovementType;
        [SerializeField][Range(0f, 50f)] public float _LinearSpeed;
        [SerializeField] public Vector3 _OffsetByLocalDirection;
        [SerializeField] [ReadOnly] public float _DistanceTraveled;

        #endregion
        
        #region Events
        
        public Action OnReachedEnd;
        public Action<float> OnPositionUpdate;

        #endregion

        protected Transform _thisTransform;
        
        protected Vector3 _Position;
        protected Quaternion _Rotation;

        protected virtual void Initialize()
        {
            _thisTransform = GetComponent<Transform>();
            UpdateTransform();
        }
        
        public void UpdateRouteFollower(float updateTime)
        {
            if (!_Follow)
            {
                return;
            }
            
            UpdateCurrentPercentage(updateTime);
            CalculateTransformValues();
            UpdateTransform();
            CheckForReachEnd();
            
            OnPositionUpdate?.Invoke(_CurrentPercentage);
        }

        private void UpdateCurrentPercentage(float updateTime)
        {
            var distanceTraveled = _DistanceTraveled + _LinearSpeed * updateTime;
            _CurrentPercentage = _RouteComputer.GetPercentageTraveledByDistanceTraveled(distanceTraveled);
            _DistanceTraveled = _RouteComputer.Length * _CurrentPercentage;
        }
        
        private void CalculateTransformValues()
        {
            var samplePoint = _RouteComputer.GetSamplePointAtPercentage(_CurrentPercentage);
            _Position = _RouteComputer.TransformLocalPointToWorldPoint(samplePoint._LocalPosition);
            _Rotation = Quaternion.LookRotation(samplePoint._Forward);
        }

        private void CheckForReachEnd()
        {
            if (_LinearSpeed > 0f && _CurrentPercentage >= 1f)
            {
                _CurrentPercentage = 1f;
                _Follow = false;
                OnReachedEnd?.Invoke();
            }
        }

        private void UpdateTransform()
        {
            if (_FaceDirection)
            {
                _thisTransform.rotation = _Rotation;
            }

            _thisTransform.position = _Position + GetOffsetAddition();
        }

        public void SetPercentage(float percentage)
        {
            _CurrentPercentage = Mathf.Clamp01(percentage);

            if (_RouteComputer != null)
            {
                _DistanceTraveled = _RouteComputer.GetDistanceTraveledByPercentage(_CurrentPercentage);
                CalculateTransformValues();
                UpdateTransform();
            }
        }

        private Vector3 GetOffsetAddition()
        {
            Vector3 offset = Vector3.zero;
            offset += _thisTransform.right * _OffsetByLocalDirection.x;
            offset += _thisTransform.up * _OffsetByLocalDirection.y;
            offset += _thisTransform.forward * _OffsetByLocalDirection.z;
            
            return offset;
        }
        
        public void SetRouteComputer(RouteComputer routeComputer)
        {
            _RouteComputer = routeComputer;
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            _thisTransform = GetComponent<Transform>();
            SetPercentage(_CurrentPercentage);
        }
#endif

    }
}