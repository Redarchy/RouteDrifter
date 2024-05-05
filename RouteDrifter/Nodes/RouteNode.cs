using System.Collections.Generic;
using RouteDrifter.Computer;
using RouteDrifter.Models;
using RouteDrifter.Utility.Extensions;
using UnityEngine;
using UnityEngine.Pool;

namespace RouteDrifter.Nodes
{
    [ExecuteInEditMode]
    public class RouteNode : MonoBehaviour
    {
        [SerializeField] private List<RouteNodeConnection> _Connections;

        public IReadOnlyList<RouteNodeConnection> Connections => _Connections;

        public bool TryConnectComputer(RouteComputer computer, RoutePoint routePoint)
        {
            foreach (var connection in _Connections)
            {
                if (connection.Computer == computer)
                {
                    return false;
                }
            }

            var nodeConnection = new RouteNodeConnection(this, computer, routePoint);

            if (!computer.TryConnectNode(nodeConnection))
            {
                return false;
            }

            _Connections.Add(nodeConnection);

            Rebuild();
            
            return true;
        }

        public void DisconnectComputer(RouteComputer connectedComputer)
        {
            RouteNodeConnection connectionToRemove = null;
            
            foreach (var connection in _Connections)
            {
                if (connection.Computer == connectedComputer)
                {
                    connectionToRemove = connection;
                    break;
                }
            }
            
            if (connectionToRemove == null)
            {
                return;
            }
            
            _Connections.Remove(connectionToRemove);
            connectedComputer.OnDisconnectedFromNode(connectionToRemove);
            
            Rebuild();
        }

        private void Rebuild()
        {
            if (_Connections.Count <= 0)
            {
                return;
            }

            for (var i = 0; i < _Connections.Count; i++)
            {
                var connection = _Connections[i];

                if (i == 0)
                {
                    transform.position = connection.Computer.TransformLocalPointToWorldPoint(connection.Point.Position);

                    var firstConnectionPointWorldPosition = connection.Computer.TransformLocalPointToWorldPoint(connection.Point.Position);
                
                    if (connection.Computer.TryGetClosestSamplePoint(firstConnectionPointWorldPosition, out var firstConnectionSamplePoint))
                    {
                        connection.PercentageFractionOnComputer = firstConnectionSamplePoint.Percentage;
                    }
                    
                    continue;
                }
                
                connection.Computer.SetConnectedPointWorldPosition(connection, transform.position);
                
                var connectedRoutePointWorldPosition = connection.Computer.TransformLocalPointToWorldPoint(connection.Point.Position);
                if (connection.Computer.TryGetClosestSamplePoint(connectedRoutePointWorldPosition, out var samplePoint))
                {
                    connection.PercentageFractionOnComputer = samplePoint.Percentage;
                }
                
            }
        }
        
        public bool IsConnectedWith(RouteNodeConnection connection)
        {
            foreach (var nodeConnection in _Connections)
            {
                if (nodeConnection.Computer == connection.Computer)
                {
                    if (nodeConnection.Point.Index == connection.Point.Index)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        private void OnDestroy()
        {
            var connections = ListPool<RouteNodeConnection>.Get();
        
            foreach (var connection in _Connections)
            {
                connections.Add(connection);
            }
            
            foreach (var connection in connections)
            {
                DisconnectComputer(connection.Computer);
            }
            
            connections.Clear();
            ListPool<RouteNodeConnection>.Release(connections);
        }
        
    }
}