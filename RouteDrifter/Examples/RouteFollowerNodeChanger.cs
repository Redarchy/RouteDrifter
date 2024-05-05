using System.Collections.Generic;
using RouteDrifter.Follower;
using RouteDrifter.Nodes;
using UnityEngine;

namespace RouteDrifter.Examples
{
    public class RouteFollowerNodeChanger : MonoBehaviour
    {
        [SerializeField] private RouteFollower _Follower;

        private void Start()
        {
            if (_Follower != null)
            {
                _Follower.OnNode += OnNode;
            }
        }

        private void OnNode(List<RouteNodeConnection> connections)
        {
            foreach (var connection in connections)
            {
                if (connection.Computer != _Follower._RouteComputer)
                {
                    _Follower.SetRouteComputer(connection.Computer);
                    _Follower.SetPercentage(connection.PercentageFractionOnComputer);
                    Debug.Log("Route Computer Changed");
                    break;
                }
            }
        }
    }
}