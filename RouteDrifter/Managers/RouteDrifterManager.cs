using System.Collections.Generic;
using RouteDrifter.Follower;
using RouteDrifter.Computer;
using RouteDrifter.Constants;
using RouteDrifter.RouteEvents;
using RouteDrifter.RouteEvents.Computers;
using RouteDrifter.RouteEvents.Followers;
using UnityEngine;

namespace RouteDrifter.Managers
{
    [DefaultExecutionOrder(-100)]
    public class RouteDrifterManager : MonoBehaviour
    {
        [SerializeField] private bool _Update;
        
        private List<RouteComputer> _routeComputers = new List<RouteComputer>();
        private List<RouteFollower> _routeFollowers = new List<RouteFollower>();

        public void EnableUpdate(bool update)
        {
            _Update = update;
        }

        public void UpdateImmediately()
        {
            UpdateRouteFollowers(Time.deltaTime);
        }

        private void Update()
        {
            if (_Update)
            {
                UpdateRouteFollowers(Time.deltaTime);
            }
        }

        private void OnEnable()
        {
            RouteDrifterEventManager.RegisterHandler<RouteFollowerEnabledEvent>(OnRouteFollowerEnabled);
            RouteDrifterEventManager.RegisterHandler<RouteFollowerDisabledEvent>(OnRouteFollowerDisabled);
            RouteDrifterEventManager.RegisterHandler<RouteComputerEnabledEvent>(OnRouteComputerEnabled);
            RouteDrifterEventManager.RegisterHandler<RouteComputerDisabledEvent>(OnRouteComputerDisabled);
        }

        private void OnDisable()
        {
            RouteDrifterEventManager.UnregisterHandler<RouteFollowerEnabledEvent>(OnRouteFollowerEnabled);
            RouteDrifterEventManager.UnregisterHandler<RouteFollowerDisabledEvent>(OnRouteFollowerDisabled);
            RouteDrifterEventManager.UnregisterHandler<RouteComputerEnabledEvent>(OnRouteComputerEnabled);
            RouteDrifterEventManager.UnregisterHandler<RouteComputerDisabledEvent>(OnRouteComputerDisabled);
        }

        private void UpdateRouteFollowers(float updateTime)
        {
            int routeFollowerCount = _routeFollowers.Count;
            for (int i = 0; i < routeFollowerCount; i++)
            {
                _routeFollowers[i].UpdateRouteFollower(updateTime);
            }
        }
        
        private void OnRouteFollowerEnabled(RouteFollowerEnabledEvent data)
        {
            var routeFollower = data.RouteFollower;
            if (!_routeFollowers.Contains(routeFollower))
            {
                _routeFollowers.Add(routeFollower);
            }
        }
        
        private void OnRouteFollowerDisabled(RouteFollowerDisabledEvent data)
        {
            var routeFollower = data.RouteFollower;
            if (_routeFollowers.Contains(routeFollower))
            {
                _routeFollowers.Remove(routeFollower);
            }
        }

        private void OnRouteComputerEnabled(RouteComputerEnabledEvent data)
        {
            var routeComputer = data.RouteComputer;
            if (!_routeComputers.Contains(routeComputer))
            {
                _routeComputers.Add(routeComputer);
            }

            // RespondWaitingRequests(routeComputer);
        }

        private void OnRouteComputerDisabled(RouteComputerDisabledEvent data)
        {
            var routeComputer = data.RouteComputer;
            if (_routeComputers.Contains(routeComputer))
            {
                _routeComputers.Remove(routeComputer);
            }
        }
    }
}