using System.Collections.Generic;
using RouteDrifter.Computer;
using RouteDrifter.Models;
using UnityEditor;
using UnityEngine;

namespace RouteDrifter.Editor
{
    [CustomEditor(typeof(RouteComputer))]
    public class RouteComputerEditor : UnityEditor.Editor
    {
        private RouteComputer _routeComputer;
        private RouteContainer _routeContainer;
        private List<RoutePoint> _routePoints;
        private List<RouteBezierCurve> _routeBezierCurves;
        
#if UNITY_EDITOR 
        
        private void OnEnable()
        {
            _routeComputer = (RouteComputer) target;
            _routeContainer = _routeComputer as RouteContainer;
            _routePoints = _routeContainer.RoutePoints;
            _routeBezierCurves = _routeContainer.BezierCurves;
        }

        private void OnSceneGUI()
        {
            Draw();
        }

#endif
        private void Draw()
        {
            DrawHandles();
            DrawRoutePointsAndTangents();
            DrawLine();
            DrawSamples();
        }
        
        private void DrawSamples()
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontStyle = FontStyle.Bold;
            
            float sampleLength = 0.15f;

            Vector3 previousLeft = new Vector3();
            Vector3 previousRight = new Vector3();
            
            
            List<SamplePoint> evenlySpacedSamplePoints = _routeContainer.SamplePoints;

            int samplePointsCount = evenlySpacedSamplePoints.Count;
            for (int samplePointIndex = 0; samplePointIndex < samplePointsCount; samplePointIndex++)
            {
                Handles.color = Color.white;
                SamplePoint samplePoint = evenlySpacedSamplePoints[samplePointIndex];

                Vector3 worldPoint = _routeContainer.TransformLocalPointToWorldPoint(samplePoint._LocalPosition);

                Vector3 rotatedLeft = (Quaternion.AngleAxis(-90, Vector3.up) * samplePoint._Forward).normalized;
                rotatedLeft = worldPoint + rotatedLeft * sampleLength;
                     
                Vector3 rotatedRight = (Quaternion.AngleAxis(90, Vector3.up) * samplePoint._Forward).normalized;
                rotatedRight = worldPoint + rotatedRight * sampleLength;
                    
                Handles.DrawLine(rotatedLeft, rotatedRight);

                if (samplePointIndex > 0)
                {
                    Handles.DrawLine(previousLeft, rotatedLeft);
                    Handles.DrawLine(previousRight, rotatedRight);
                }
                     
                previousLeft = rotatedLeft;
                previousRight = rotatedRight;
                     
                // Draw SamplePoint Forward
                float forwardLength = sampleLength - 0.12f;
                Handles.color = Color.blue;
                Handles.DrawLine(worldPoint, worldPoint + samplePoint._Forward * forwardLength, 3f);
            }
        }

        private void DrawHandles()
        {
            int routePointCount = _routePoints.Count;
            for (int i = 0; i < routePointCount; i++)
            {
                RoutePoint routePoint = _routeContainer.FromLocalToWorldRoutePointByIndex(i);
                
                // Route Point Position Handle
                Vector3 newPosition = Handles.PositionHandle(routePoint._Position, Quaternion.identity);
                
                if (routePoint._Position != newPosition)
                {
                    Undo.RecordObject(_routeComputer, "move point");
                    routePoint._Position = newPosition;
                    _routePoints[i] = _routeContainer.FromWorldToLocalRoutePoint(routePoint);
                    _routeComputer.Build();
                }
                
                // Route Point Tangent Handle
                Vector3 newTangent = Handles.PositionHandle(routePoint._Tangent, Quaternion.identity);
                
                if (routePoint._Tangent != newTangent)
                {
                    Undo.RecordObject(_routeComputer, "move tangent");
                    routePoint._Tangent = newTangent;
                    _routePoints[i] = _routeContainer.FromWorldToLocalRoutePoint(routePoint);
                    _routeComputer.Build();
                }
            }
        }

        private void DrawLine()
        {
            int bezierCurvesCount = _routeBezierCurves.Count;
            for (int i = 0; i < bezierCurvesCount; i++)
            {
                var curvePoint = _routeContainer.WorldSegmentByIndex(i);
                Handles.DrawBezier(curvePoint._StartPosition, curvePoint._EndPosition,
                    curvePoint._StartTangent, curvePoint._EndTangent,
                    Color.green, null, 3f);
            }
        }
        
        private void DrawRoutePointsAndTangents()
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontStyle = FontStyle.Bold;
        
            int routePointsCount = _routePoints.Count;
            for (int i = 0; i < routePointsCount; i++)
            {
                RoutePoint routePoint = _routeContainer.FromLocalToWorldRoutePointByIndex(i);
                
                // Draw Point Indicators
                Handles.color = Color.blue;
                Handles.SphereHandleCap(0, routePoint._Position, Quaternion.identity, 0.15f, EventType.Repaint);
                
                // Draw Labels
                style.normal.textColor = Color.white;
                Handles.Label(routePoint._Position, $"Point : {i}", style);
                
                style.normal.textColor = Color.white;
                Handles.Label(routePoint._Tangent, $"Tangent", style);
            }

            int bezierCurveCount = _routeBezierCurves.Count;
            for (int i = 0; i < bezierCurveCount; i++)
            {
                var curvePoint = _routeContainer.WorldSegmentByIndex(i);
                
                // Draw Tangent Lines
                Handles.color = Color.black;
                Handles.DrawLine(curvePoint._StartTangent, curvePoint._StartPosition, 2f);
                Handles.DrawLine(curvePoint._EndTangent, curvePoint._EndPosition, 2f); 
                
                // Draw Tangent Points
                Handles.color = Color.red;
                Handles.SphereHandleCap(0, curvePoint._StartTangent, Quaternion.identity, 0.08f, EventType.Repaint);
                Handles.SphereHandleCap(0, curvePoint._EndTangent, Quaternion.identity, 0.08f, EventType.Repaint);
            }

        }
    }
}