using System.Collections.Generic;
using RouteDrifter.Computer;
using RouteDrifter.Models;
using UnityEngine;

namespace RouteDrifter.Utility.Extensions
{
    public static class RouteComputerExtensions
    {
        public static float GetClosestPercentageToWorldPoint(this RouteComputer routeComputer, Vector3 worldPoint)
        {
            return 0;
        }

        public static float GetPercentageTraveledByDistanceTraveled(this RouteComputer routeComputer, float distanceTraveled)
        {
            return Mathf.Clamp01(distanceTraveled / routeComputer.Length);
        }
        
        public static Vector3 GetWorldPositionByPercentage(this RouteComputer routeComputer, float percentage)
        {
            percentage = Mathf.Clamp01(percentage);
            List<SamplePoint> samplePointsAtPercentage = routeComputer.GetSamplePointsAtPercentage(percentage);
            
            float fractionalSamplePoint = percentage * (routeComputer.SamplePoints.Count - 1);
            int min = Mathf.FloorToInt(fractionalSamplePoint);

            float percentageBetweenSamplePoints = fractionalSamplePoint - min;

            Vector3 localPosition = Vector3.Lerp(samplePointsAtPercentage[0]._LocalPosition, samplePointsAtPercentage[1]._LocalPosition, percentageBetweenSamplePoints);

            Vector3 worldPositionByPercent = routeComputer.Transform.TransformPoint(localPosition);
            return worldPositionByPercent;
        }
        
        public static float GetDistanceTraveledByPercentage(this RouteComputer routeComputer, float percentage)
        {
            return percentage * routeComputer.Length;
        }
        
        public static List<SamplePoint> GetSamplePointsAtPercentage(this RouteComputer routeComputer, float percentage)
        {
            percentage = Mathf.Clamp01(percentage);
            
            float evenlySpacedPoint = percentage * (routeComputer.SamplePoints.Count - 1);
            int min = Mathf.FloorToInt(evenlySpacedPoint);
            int max = Mathf.CeilToInt(evenlySpacedPoint);
            
            SamplePoint minSamplePoint = routeComputer.SamplePoints[min];
            SamplePoint maxSamplePoint = routeComputer.SamplePoints[max];
            
            return new List<SamplePoint>() {minSamplePoint, maxSamplePoint};
        }

        public static SamplePoint GetSamplePointAtPercentage(this RouteComputer routeComputer, float percentage)
        {
            var samplePointCount = routeComputer.SamplePoints.Count - 1;
            float percentagePointIndex = percentage * samplePointCount;

            if (percentage == 1.0f)
            {
                return routeComputer.SamplePoints[samplePointCount];
            }
            
            int lowerPointIndex = Mathf.FloorToInt(percentagePointIndex);
            int upperPointIndex = Mathf.Clamp(lowerPointIndex + 1, lowerPointIndex, samplePointCount);

            var lowerPointPercentage = (float) lowerPointIndex / (float) samplePointCount;
            var upperPointPercentage = (float) upperPointIndex / (float) samplePointCount;

            var samplePointPercentage = (percentage - lowerPointPercentage) / (upperPointPercentage - lowerPointPercentage);

            var samplePoint = new SamplePoint()
            {
                _Forward = Vector3.Lerp(
                    routeComputer.SamplePoints[lowerPointIndex]._Forward,
                    routeComputer.SamplePoints[upperPointIndex]._Forward,
                    samplePointPercentage),

                _LocalPosition = Vector3.Lerp(
                    routeComputer.SamplePoints[lowerPointIndex]._LocalPosition,
                    routeComputer.SamplePoints[upperPointIndex]._LocalPosition,
                    samplePointPercentage)
            };
            
            return samplePoint;
        }

    }
}