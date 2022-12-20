using RouteDrifter.Gizmos;
using RouteDrifter.Models;
using RouteDrifter.Utility.Attributes;
using UnityEngine;

namespace RouteDrifter.Computer
{
    [DefaultExecutionOrder(-99)]
    public class RouteComputer : RouteContainer, IRouteGizmosDrawable
    {
        public void DrawGizmos()
        {
            
        }
        

        #region Editor Functionalities

        [RouteInspectorButton("Set Tangents Default")]
        private void Editor_SetTangentsDefault()
        {
            int count = _RoutePoints.Count;
            for (int i = 0; i < count; i++)
            {
                _RoutePoints[i] = _RoutePoints[i].SetTangentsDefault();
            }
        }

        #endregion

    }
}