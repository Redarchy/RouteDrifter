using System;
using System.Collections.Generic;
using RouteDrifter.Nodes;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace RouteDrifter.Models
{
    [ExecuteInEditMode]
    public class RouteContainer : MonoBehaviour
    {
        #region Inspector
        
        [SerializeField] protected List<RoutePoint> _RoutePoints;
        [SerializeField] protected bool _BuildOnAwake;
        [SerializeField, Range(0.005f, 1)] private float _SampleSpacing = 1f;
        [SerializeField, Range(0.005f, 1)] private float _SampleResolution = 1f;
        [SerializeField, ReadOnly] private float _Length;
        [SerializeField, ReadOnly] private Transform _ThisTransform;

        #endregion

        [SerializeField, HideInInspector] private List<RouteBezierCurve> _BezierCurves;
        [SerializeField] private List<SamplePoint> _SamplePoints;
        [SerializeField] private List<RouteNodeConnection> _NodeConnections;
        
        public List<SamplePoint> SamplePoints => _SamplePoints;
        public List<RoutePoint> RoutePoints => _RoutePoints;
        public List<RouteBezierCurve> BezierCurves => _BezierCurves;
        public float Length => _Length;
        public Transform Transform => _ThisTransform;

        public Action<List<SamplePoint>> OnBuild;

        protected virtual void Awake()
        {
            InternalInitialize();
            
            if (_BuildOnAwake)
            {
                Build();
            }
        }

        protected virtual void InternalInitialize()
        {
            _ThisTransform = GetComponent<Transform>();
        }

        public virtual void Set(bool buildOnAwake, float sampleSpacing, float sampleResolution, List<RoutePoint> routePoints)
        {
            _BuildOnAwake = buildOnAwake;
            _SampleSpacing = sampleSpacing;
            _SampleResolution = sampleResolution;
            _RoutePoints = routePoints;
            
            // Needed to call it again for instantiating at runtime, not as a scene root object.
            // Because Awake is called as soon as the object gets instantiated.
            if (buildOnAwake)
            {
                Build();
            }
        }
        
        #region Node Connections

        public bool TryConnectNode(RouteNodeConnection connection)
        {
            if (_RoutePoints.Count <= connection.Point.Index)
            {
                return false;
            }

            foreach (var nodeConnection in _NodeConnections)
            {
                if (nodeConnection.Node == connection.Node)
                {
                    return false;
                }

                if (nodeConnection.Point.Index == connection.Point.Index)
                {
                    return false;
                }
            }
            
            _NodeConnections.Add(connection);
            
            return true;
        }
        
        public void OnDisconnectedFromNode(RouteNodeConnection connection)
        {
            RouteNodeConnection removedConnection = null;

            foreach (var nodeConnection in _NodeConnections)
            {
                if (nodeConnection.Computer == connection.Computer)
                {
                    if (nodeConnection.Point.Index == connection.Point.Index)
                    {
                        removedConnection = nodeConnection;
                        break;
                    }
                }
            }
            
            _NodeConnections.Remove(removedConnection);
        }
        
        #endregion

        #region Build

        public void Build()
        {
            UpdateBezierCurves();
            UpdateSamplePoints();
            UpdateLength();
            UpdateSamplePointPercentages();
            ValidateNodeConnections();
            
            OnBuild?.Invoke(new List<SamplePoint>(_SamplePoints));
        }

        private void ValidateNodeConnections()
        {
            var invalidConnections = ListPool<RouteNodeConnection>.Get();
            
            foreach (var connection in _NodeConnections)
            {
                var isConnectionNull = connection == null || connection.Node == null;
                var isPointMissing = _RoutePoints.Count <= connection.Point.Index;
                var isConnectedWith = connection.Node.IsConnectedWith(connection);
                
                if (isConnectionNull || isPointMissing || !isConnectedWith)
                {
                    invalidConnections.Add(connection);
                }
            }

            foreach (var connection in invalidConnections)
            {
                if (connection.Node != null)
                {
                    connection.Node.DisconnectComputer(connection.Computer);
                }
                else
                {
                    OnDisconnectedFromNode(connection);
                }
            }
            
            invalidConnections.Clear();
            ListPool<RouteNodeConnection>.Release(invalidConnections);
        }

        private void UpdateBezierCurves()
        {
            _BezierCurves.Clear();
            
            var curveCount = _RoutePoints.Count - 1;

            if (curveCount <= 0)
            {
                return;
            }
            
            for (var i = 0; i < curveCount; i++)
            {
                var startRoutePoint = _RoutePoints[i];
                var endRoutePoint = _RoutePoints[i + 1];

                if (i > 0)
                {
                    startRoutePoint.Tangent *= -1;
                }

                var routeBezierCurve = new RouteBezierCurve(startRoutePoint, endRoutePoint);
                _BezierCurves.Add(routeBezierCurve);
            }
        }

        private void UpdateSamplePoints()
        {
            if (_BezierCurves.Count <= 0)
            {
                return;
            }

            var samplePoints = _SamplePoints;
            samplePoints.Clear();

            var firstSamplePoint = new SamplePoint
            {
                LocalPosition = _RoutePoints[0].Position
            };
            
            samplePoints.Add(firstSamplePoint);
            
            var previousSamplePoint = samplePoints[0];

            #region Position Calculations

            var bezierCurvesCount = _BezierCurves.Count;
            for (var i = 0; i < bezierCurvesCount; i++)
            {
                var fraction = 0f;

                while (fraction <= 1f)
                {
                    fraction += _SampleResolution;

                    var currentPoint = _BezierCurves[i].GetPointInCurveWithFraction(Mathf.Clamp01(fraction));
                    var distanceSinceLastPoint = Vector3.Distance(previousSamplePoint.LocalPosition, currentPoint);

                    while (distanceSinceLastPoint >= _SampleSpacing)
                    {
                        var overshootDistance = distanceSinceLastPoint - _SampleSpacing;
                        var newEvenlySpacedPoint = currentPoint + (previousSamplePoint.LocalPosition - currentPoint).normalized * overshootDistance;
                    
                        var newSamplePoint = new SamplePoint(newEvenlySpacedPoint, Vector3.zero);
                        samplePoints.Add(newSamplePoint);

                        distanceSinceLastPoint = overshootDistance;
                        previousSamplePoint = newSamplePoint;
                    }
                }
            }

            #endregion

            #region Forward Direction Calculations

            var pointCount = samplePoints.Count;

            if (pointCount < 2)
            {
                return;
            }
            
            for (var i = 0; i < pointCount; i++)
            {
                var samplePoint = samplePoints[i];
                
                var forward = (i == pointCount - 1)
                    ? samplePoints[i - 1].Forward
                    : (samplePoints[i + 1].LocalPosition - samplePoint.LocalPosition).normalized;
                
                samplePoint.Forward = forward;
                samplePoints[i] = samplePoint;
            }

            #endregion
        }

        private void UpdateSamplePointPercentages()
        {
            var samplePointCount = _SamplePoints.Count;

            if (samplePointCount <= 0)
            {
                return;
            }

            var previousSamplePoint = _SamplePoints[0];
            previousSamplePoint.Percentage = 0f;
            _SamplePoints[0] = previousSamplePoint;

            var length = _Length;
            
            for (var i = 1; i < samplePointCount; i++)
            {
                var samplePoint = _SamplePoints[i];
                
                var distanceToPrevious = Vector3.Distance(previousSamplePoint.LocalPosition, samplePoint.LocalPosition);
                var lengthPercentage = distanceToPrevious / length;
                samplePoint.Percentage = Mathf.Clamp01(previousSamplePoint.Percentage + lengthPercentage);

                _SamplePoints[i] = samplePoint;
                previousSamplePoint = samplePoint;
            }
        }
        
        private void UpdateLength()
        {
            _Length = 0;

            var sampleCount = _SamplePoints.Count;
            for (var i = 0; i < sampleCount - 1; i++)
            {
                _Length += Vector3.Distance(_SamplePoints[i].LocalPosition, _SamplePoints[i + 1].LocalPosition);
            }
        }

        #endregion

        #region Helper Methods

        public Vector3 TransformLocalPointToWorldPoint(Vector3 localPoint)
        {
            return _ThisTransform.TransformPoint(localPoint);
        }

        public Vector3 TransformWorldPointToLocalPoint(Vector3 worldPoint)
        {
            return _ThisTransform.InverseTransformPoint(worldPoint);
        }
        
        public void SetConnectedPointWorldPosition(RouteNodeConnection connection, Vector3 worldPosition)
        {
            if (!_NodeConnections.Contains(connection))
            {
                return;
            }

            var connectionPoint = connection.Point;
            if (connectionPoint.Index >= _RoutePoints.Count)
            {
                return;
            }

            var routePoint = RoutePoints[connectionPoint.Index];
            routePoint.Position = TransformWorldPointToLocalPoint(worldPosition);
            _RoutePoints[connectionPoint.Index] = routePoint;
            
            Build();
        }
        
        public bool TryGetNodeConnectionsNonAlloc(float startPercentage, float endPercentage, List<RouteNodeConnection> nodeConnections)
        {
            foreach (var connection in _NodeConnections)
            {
                if (startPercentage < connection.PercentageFractionOnComputer && connection.PercentageFractionOnComputer < endPercentage)
                {
                    nodeConnections.AddRange(connection.Node.Connections);
                }
            }

            return nodeConnections.Count > 0;
        }
        
        public RoutePoint FromLocalToWorldRoutePointByIndex(int routePointIndex)
        {
            var routePoint = _RoutePoints[routePointIndex];
            routePoint.Position = _ThisTransform.TransformPoint(routePoint.Position);
            routePoint.Tangent += routePoint.Position;
            
            return routePoint;
        }
        
        public RoutePoint FromWorldToLocalRoutePoint(RoutePoint worldRoutePoint)
        {
            var localRoutePoint = new RoutePoint
            {
                Position = _ThisTransform.InverseTransformPoint(worldRoutePoint.Position),
                Tangent = worldRoutePoint.Tangent - worldRoutePoint.Position
            };
            
            return localRoutePoint;
        }
        
        public RouteSegment WorldSegmentByIndex(int segmentIndex)
        {
            var routeSegment = _BezierCurves[segmentIndex].RouteSegment;
            
            routeSegment._StartPosition = _ThisTransform.TransformPoint(routeSegment._StartPosition);
            routeSegment._EndPosition = _ThisTransform.TransformPoint(routeSegment._EndPosition);
            
            routeSegment._StartTangent += routeSegment._StartPosition;
            routeSegment._EndTangent += routeSegment._EndPosition;

            return routeSegment;
        }

        #endregion

        private void OnDestroy()
        {
            #region Remove Node Connections
        
            // Use copy for not to modify the _NodeConnections list directly, to prevent IEnumeration error..
            var connectionsCopy = ListPool<RouteNodeConnection>.Get();
            
            foreach (var connection in _NodeConnections)
            {
                connectionsCopy.Add(connection);
            }
        
            foreach (var connection in connectionsCopy)
            {
                connection.Node.DisconnectComputer(connection.Computer);
            }
            
            connectionsCopy.Clear();
            ListPool<RouteNodeConnection>.Release(connectionsCopy);
        
            _NodeConnections.Clear();
            
            #endregion
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (_RoutePoints == null)
            {
                _RoutePoints = new List<RoutePoint>();
            }
            
            _ThisTransform = GetComponent<Transform>();
            SetRoutePointIndices();
            Build();
        }

        private void SetRoutePointIndices()
        {
            for (var index = 0; index < _RoutePoints.Count; index++)
            {
                var routePoint = _RoutePoints[index];
                routePoint.Index = index;
                _RoutePoints[index] = routePoint;
            }
        }
#endif


    }
}