using System;
using System.Collections.Generic;
using RouteDrifter.Computer;
using RouteDrifter.Models;
using RouteDrifter.Nodes;
using RouteDrifter.Utility.Extensions;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Pool;

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
        [SerializeField] public RouteFollowerMovementDirection _Direction;
        
        public bool ReversedMovement => _Direction == RouteFollowerMovementDirection.Reversed;

        #endregion
        
        #region Events
        
        public Action OnReachedEnd;
        public Action<float> OnPositionUpdate;
        public Action<List<RouteNodeConnection>> OnNode;

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

            var latestPercentage = _CurrentPercentage;
            
            UpdateCurrentPercentage(updateTime);
            CalculateTransformValues();
            UpdateTransform();
            CheckForReachEnd();
            CheckForNode(latestPercentage);
            
            OnPositionUpdate?.Invoke(_CurrentPercentage);
        }

        private void CheckForNode(float percentageBeforeUpdate)
        {
            var nodeConnections = ListPool<RouteNodeConnection>.Get();
            nodeConnections.Clear();
            
            var forwardPercentageOnComputer = ReversedMovement ? percentageBeforeUpdate : _CurrentPercentage;
            var rearPercentageOnComputer = ReversedMovement ? _CurrentPercentage : percentageBeforeUpdate;
            
            if (_RouteComputer.TryGetNodeConnectionsNonAlloc(rearPercentageOnComputer, forwardPercentageOnComputer, nodeConnections))
            {
                OnNode?.Invoke(nodeConnections);
            }
            
            nodeConnections.Clear();
            ListPool<RouteNodeConnection>.Release(nodeConnections);
        }

        private void UpdateCurrentPercentage(float updateTime)
        {
            var distanceChange = _LinearSpeed * updateTime;
            distanceChange *= ReversedMovement ? -1 : 1;
            
            var distanceTraveled = _DistanceTraveled + distanceChange;
            _CurrentPercentage = _RouteComputer.GetPercentageTraveledByDistanceTraveled(distanceTraveled);
            _DistanceTraveled = _RouteComputer.Length * _CurrentPercentage;
        }
        
        private void CalculateTransformValues()
        {
            var samplePoint = _RouteComputer.GetLerpedSamplePointAtPercentage(_CurrentPercentage);
            _Position = _RouteComputer.TransformLocalPointToWorldPoint(samplePoint.LocalPosition);
            
            var forwardDirection = samplePoint.Forward;
            forwardDirection *= ReversedMovement ? -1 : 1;
            _Rotation = Quaternion.LookRotation(forwardDirection);
        }

        private void CheckForReachEnd()
        {
            var endPercentage = ReversedMovement ? 0f : 1f;
            
            var isReachedEndPercentage = ReversedMovement
                ? _CurrentPercentage <= endPercentage
                : _CurrentPercentage >= endPercentage;
            
            if (_LinearSpeed > 0f && isReachedEndPercentage)
            {
                _CurrentPercentage = endPercentage;
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

        public void SetPercentage(float percentage, bool updateTransform = true)
        {
            _CurrentPercentage = Mathf.Clamp01(percentage);

            if (_RouteComputer != null && updateTransform)
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

            offset *= _Direction == RouteFollowerMovementDirection.Straight ? 1f : -1f;
            
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