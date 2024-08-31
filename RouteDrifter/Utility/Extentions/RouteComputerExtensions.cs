using System.Collections.Generic;
using RouteDrifter.Computer;
using RouteDrifter.Models;
using UnityEngine;
using UnityEngine.Pool;

namespace RouteDrifter.Utility.Extensions
{
    public static class RouteComputerExtensions
    {
        public static bool TryGetClosestWorldPointOnComputer(this RouteComputer routeComputer, Vector3 worldPoint, out Vector3 closestWorldPointOnComputer)
        {
            closestWorldPointOnComputer = default;
            
            if (!routeComputer.TryGetClosestSamplePoint(worldPoint, out var closestSamplePoint))
            {
                return false;
            }

            closestWorldPointOnComputer = routeComputer.TransformLocalPointToWorldPoint(closestSamplePoint.LocalPosition);
            
            return true;
        }

        public static bool TryGetClosestSamplePoint(this RouteComputer routeComputer, Vector3 worldPoint, out SamplePoint closestSamplePoint)
        {
            closestSamplePoint = default;

            var samplePoints = routeComputer.SamplePoints;
            var samplePointCount = samplePoints.Count;
            
            if (samplePointCount <= 0)
            {
                return false;
            }

            var localPoint = routeComputer.TransformWorldPointToLocalPoint(worldPoint);
            
            var nearestSamplePoint = routeComputer.SamplePoints[0];
            var nearestSamplePointDistance = Vector3.Distance(localPoint, nearestSamplePoint.LocalPosition);

            for (var i = 1; i < samplePointCount; i++)
            {
                var samplePoint = routeComputer.SamplePoints[i];
                var distance = Vector3.Distance(localPoint, samplePoint.LocalPosition);

                if (distance <= nearestSamplePointDistance)
                {
                    nearestSamplePoint = samplePoint;
                    nearestSamplePointDistance = distance;
                }
            }

            closestSamplePoint = nearestSamplePoint;

            return true;
        }

        public static float GetPercentageTraveledByDistanceTraveled(this RouteComputer routeComputer, float distanceTraveled)
        {
            return Mathf.Clamp01(distanceTraveled / routeComputer.Length);
        }
        
        public static Vector3 GetWorldPositionByPercentage(this RouteComputer routeComputer, float percentage)
        {
            percentage = Mathf.Clamp01(percentage);
            List<SamplePoint> samplePointsAtPercentage = routeComputer.GetSamplePointsAtPercentage(percentage);
            
            var fractionalSamplePoint = percentage * (routeComputer.SamplePoints.Count - 1);
            var min = Mathf.FloorToInt(fractionalSamplePoint);

            var percentageBetweenSamplePoints = fractionalSamplePoint - min;

            var localPosition = Vector3.Lerp(samplePointsAtPercentage[0].LocalPosition, samplePointsAtPercentage[1].LocalPosition, percentageBetweenSamplePoints);

            var worldPositionByPercent = routeComputer.Transform.TransformPoint(localPosition);
            
            return worldPositionByPercent;
        }
        
        public static float GetDistanceTraveledByPercentage(this RouteComputer routeComputer, float percentage)
        {
            return percentage * routeComputer.Length;
        }
        
        public static List<SamplePoint> GetSamplePointsAtPercentage(this RouteComputer routeComputer, float percentage)
        {
            percentage = Mathf.Clamp01(percentage);
            
            var evenlySpacedPoint = percentage * (routeComputer.SamplePoints.Count - 1);
            var min = Mathf.FloorToInt(evenlySpacedPoint);
            var max = Mathf.CeilToInt(evenlySpacedPoint);
            
            var minSamplePoint = routeComputer.SamplePoints[min];
            var maxSamplePoint = routeComputer.SamplePoints[max];
            
            return new List<SamplePoint>() {minSamplePoint, maxSamplePoint};
        }

        public static SamplePoint GetClosestSamplePointAtPercentage(this RouteComputer routeComputer, float percentage)
        {
            var samplePointCount = routeComputer.SamplePoints.Count;
        
            if (Mathf.Approximately(percentage, 1f))
            {
                return routeComputer.SamplePoints[samplePointCount - 1];
            }
            
            if (Mathf.Approximately(percentage, 0f))
            {
                return routeComputer.SamplePoints[0];
            }
            
        
            var percentagePointIndex = (int) (percentage * samplePointCount);
            percentagePointIndex = Mathf.Clamp(percentagePointIndex, 0, samplePointCount - 1);
            
            var lowerPointIndex = Mathf.Clamp(percentagePointIndex - 1, 0, samplePointCount - 1);
            var upperPointIndex = Mathf.Clamp(percentagePointIndex + 1, 0, samplePointCount - 1);
            
            var nearestSamplePoint = routeComputer.SamplePoints[percentagePointIndex];

            var lowerPointSamplePoint = routeComputer.SamplePoints[lowerPointIndex];

            if (Mathf.Abs(lowerPointSamplePoint.Percentage - percentage) <= 
                Mathf.Abs(nearestSamplePoint.Percentage - percentage))
            {
                nearestSamplePoint = lowerPointSamplePoint;
            }
            
            var upperPointSamplePoint = routeComputer.SamplePoints[upperPointIndex];
            
            if (Mathf.Abs(upperPointSamplePoint.Percentage - percentage) <= 
                Mathf.Abs(nearestSamplePoint.Percentage - percentage))
            {
                nearestSamplePoint = upperPointSamplePoint;
            }
            
            return nearestSamplePoint;
        }
        
        public static SamplePoint GetLerpedSamplePointAtPercentage(this RouteComputer routeComputer, float percentage)
        {
            var samplePointCount = routeComputer.SamplePoints.Count - 1;
            var percentagePointIndex = percentage * samplePointCount;
        
            if (Mathf.Approximately(percentage, 1f))
            {
                return routeComputer.SamplePoints[samplePointCount];
            }
            
            var lowerPointIndex = Mathf.FloorToInt(percentagePointIndex);
            var upperPointIndex = Mathf.Clamp(lowerPointIndex + 1, lowerPointIndex, samplePointCount);
        
            var lowerPointPercentage = (float) lowerPointIndex / (float) samplePointCount;
            var upperPointPercentage = (float) upperPointIndex / (float) samplePointCount;
        
            var samplePointPercentage = (percentage - lowerPointPercentage) / (upperPointPercentage - lowerPointPercentage);
        
            var samplePoint = new SamplePoint()
            {
                Forward = Vector3.Lerp(
                    routeComputer.SamplePoints[lowerPointIndex].Forward,
                    routeComputer.SamplePoints[upperPointIndex].Forward,
                    samplePointPercentage),
        
                LocalPosition = Vector3.Lerp(
                    routeComputer.SamplePoints[lowerPointIndex].LocalPosition,
                    routeComputer.SamplePoints[upperPointIndex].LocalPosition,
                    samplePointPercentage)
            };
            
            return samplePoint;
        }

        public static Vector3 GetSamplePointRotatedLeftLocal(this RouteComputer routeComputer, SamplePoint samplePoint)
        {
            var rotatedLeft = (Quaternion.AngleAxis(-90, Vector3.up) * samplePoint.Forward).normalized;
            rotatedLeft *= routeComputer.Width / 2f;

            return samplePoint.LocalPosition + rotatedLeft;
        }
        
        public static Vector3 GetSamplePointRotatedRightLocal(this RouteComputer routeComputer, SamplePoint samplePoint)
        {
            var rotatedRight = (Quaternion.AngleAxis(90, Vector3.up) * samplePoint.Forward).normalized;
            rotatedRight *= routeComputer.Width / 2f;

            return samplePoint.LocalPosition + rotatedRight;
        }
        
    }
}