using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

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

        public List<SamplePoint> SamplePoints => _samplePoints;
        public List<RoutePoint> RoutePoints => _RoutePoints;
        public List<RouteBezierCurve> BezierCurves => _bezierCurves;
        public float Length => _Length;

        private List<RouteBezierCurve> _bezierCurves = new List<RouteBezierCurve>();
        private List<SamplePoint> _samplePoints = new List<SamplePoint>();
        public Transform Transform => _ThisTransform;

        public Action<List<SamplePoint>> OnBuild;

        protected virtual void Awake()
        {
            Initialize();
            
            if (_BuildOnAwake)
            {
                Build();
            }
        }

        protected virtual void Initialize()
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
        
        public void Build()
        {
            UpdateBezierCurves();
            UpdateSamplePoints();
            UpdateLength();
            OnBuild?.Invoke(new List<SamplePoint>(_samplePoints));
        }
        
        private void UpdateBezierCurves()
        {
            _bezierCurves.Clear();
            
            var curveCount = _RoutePoints.Count - 1;

            if (curveCount <= 0)
            {
                return;
            }
            
            for (int i = 0; i < curveCount; i++)
            {
                var startRoutePoint = _RoutePoints[i];
                var endRoutePoint = _RoutePoints[i + 1];

                if (i > 0)
                {
                    startRoutePoint._Tangent *= -1;
                }

                var routeBezierCurve = new RouteBezierCurve(startRoutePoint, endRoutePoint);
                _bezierCurves.Add(routeBezierCurve);
            }
        }

        private void UpdateSamplePoints()
        {
            if (_bezierCurves.Count <= 0)
            {
                return;
            }
            
            var samplePoints = new List<SamplePoint>();

            var firstSamplePoint = new SamplePoint
            {
                _LocalPosition = _RoutePoints[0]._Position
            };
            
            samplePoints.Add(firstSamplePoint);
            
            var previousSamplePoint = samplePoints[0];

            #region Position Calculations

            int bezierCurvesCount = _bezierCurves.Count;
            for (int i = 0; i < bezierCurvesCount; i++)
            {
                float fraction = 0;

                while (fraction <= 1)
                {
                    fraction += _SampleResolution;

                    var currentPoint = _bezierCurves[i].GetPointInCurveWithFraction(Mathf.Clamp01(fraction));
                    var distanceSinceLastPoint = Vector3.Distance(previousSamplePoint._LocalPosition, currentPoint);

                    while (distanceSinceLastPoint >= _SampleSpacing)
                    {
                        var overshootDistance = distanceSinceLastPoint - _SampleSpacing;
                        var newEvenlySpacedPoint = currentPoint + (previousSamplePoint._LocalPosition - currentPoint).normalized * overshootDistance;
                    
                        var newSamplePoint = new SamplePoint(newEvenlySpacedPoint, Vector3.zero);
                        samplePoints.Add(newSamplePoint);

                        distanceSinceLastPoint = overshootDistance;
                        previousSamplePoint = newSamplePoint;
                    }
                }
            }

            #endregion

            #region Forward Direction Calculations

            int pointCount = samplePoints.Count;

            if (pointCount < 2)
            {
                return;
            }
            
            for (int i = 0; i < pointCount; i++)
            {
                var samplePoint = samplePoints[i];
                
                var forward = (i == pointCount - 1)
                    ? samplePoints[i - 1]._Forward
                    : (samplePoints[i + 1]._LocalPosition - samplePoint._LocalPosition).normalized;
                
                samplePoint._Forward = forward;
                samplePoints[i] = samplePoint;
            }

            #endregion
            
            _samplePoints = new List<SamplePoint>(samplePoints);
        }

        private void UpdateLength()
        {
            _Length = 0;

            var sampleCount = _samplePoints.Count;
            for (int i = 0; i < sampleCount - 1; i++)
            {
                _Length += Vector3.Distance(_samplePoints[i]._LocalPosition, _samplePoints[i + 1]._LocalPosition);
            }
        }
        
        public Vector3 TransformLocalPointToWorldPoint(Vector3 localPoint)
        {
            return _ThisTransform.TransformPoint(localPoint);
        }
        
        public RoutePoint FromLocalToWorldRoutePointByIndex(int routePointIndex)
        {
            var routePoint = _RoutePoints[routePointIndex];
            routePoint._Position = _ThisTransform.TransformPoint(routePoint._Position);
            routePoint._Tangent += routePoint._Position;
            
            return routePoint;
        }
        
        public RoutePoint FromWorldToLocalRoutePoint(RoutePoint worldRoutePoint)
        {
            var localRoutePoint = new RoutePoint
            {
                _Position = _ThisTransform.InverseTransformPoint(worldRoutePoint._Position),
                _Tangent = worldRoutePoint._Tangent - worldRoutePoint._Position
            };
            
            return localRoutePoint;
        }
        
        public RouteSegment WorldSegmentByIndex(int segmentIndex)
        {
            var routeSegment = _bezierCurves[segmentIndex].RouteSegment;
            
            routeSegment._StartPosition = _ThisTransform.TransformPoint(routeSegment._StartPosition);
            routeSegment._EndPosition = _ThisTransform.TransformPoint(routeSegment._EndPosition);
            
            routeSegment._StartTangent += routeSegment._StartPosition;
            routeSegment._EndTangent += routeSegment._EndPosition;

            return routeSegment;
        }
        
#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (_RoutePoints == null)
            {
                _RoutePoints = new List<RoutePoint>();
            }
            
            _ThisTransform = GetComponent<Transform>();
            Build();
        }
#endif


    }
}