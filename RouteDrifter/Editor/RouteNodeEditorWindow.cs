using System;
using System.Collections.Generic;
using System.Linq;
using RouteDrifter.Computer;
using RouteDrifter.Models;
using RouteDrifter.Nodes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RouteDrifter.Editor
{
    public class RouteNodeEditorWindow : EditorWindow
    {
        public const string Title = "Route Node Connector";

        private static RouteNode _node;
        private static Action<RouteComputer, RoutePoint> _onConnectionSelected;
        private static List<RouteComputer> _activeRouteComputers = new ();
        private RouteComputer _selectedComputer;
        private ListView _routePointList;
        private RoutePoint _selectedRoutePoint;
        private Button _connectButton;
        

        public static void Initialize(RouteNode node, Action<RouteComputer, RoutePoint> onConnectionSelected)
        {
            _node = node;
            _onConnectionSelected = onConnectionSelected;
        }
        
        private void OnEnable()
        {
            _activeRouteComputers.Clear();
            
            _activeRouteComputers = FindObjectsOfType<RouteComputer>().ToList();
            
            foreach (var connection in _node.Connections)
            {
                if (_activeRouteComputers.Contains(connection.Computer))
                {
                    _activeRouteComputers.Remove(connection.Computer);
                }
            }

            var computersToRemove = new List<RouteComputer>(_activeRouteComputers);
            
            foreach (var routeComputer in computersToRemove)
            {
                if (routeComputer.RoutePoints.Count <= 0)
                {
                    _activeRouteComputers.Remove(routeComputer);
                }
            }
            
            computersToRemove.Clear();
        }

        private void OnDisable()
        {
            _node = null;
            _onConnectionSelected = null;
            _activeRouteComputers.Clear();
            _selectedComputer = null;
        }

        private void CreateGUI()
        {
            var leftPanel = new VisualElement();
            var rightPanel = new VisualElement();

            var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
            splitView.Add(leftPanel);
            splitView.Add(rightPanel);
            
            leftPanel.Add(CreateLabel("Select a Computer"));
            rightPanel.Add(CreateLabel("Select a Point Index"));
            
            var routeComputerList = new ListView();
            routeComputerList.style.marginTop = 25f;
            
            routeComputerList.makeItem = () => new Label();
            routeComputerList.bindItem = (item, index) => { (item as Label).text = $"{_activeRouteComputers[index].gameObject.name}"; };
            routeComputerList.itemsSource = _activeRouteComputers;

            routeComputerList.onSelectionChange += OnComputerSelectionChanged;
            
            leftPanel.Add(routeComputerList);
            
            _routePointList = new ListView();
            _routePointList.style.marginTop = 25f;
            
            rightPanel.Add(_routePointList);

            _connectButton = new Button(OnConnectButtonClicked);
            _connectButton.text = "Connect";
            _connectButton.visible = false;
            _connectButton.style.height = 50f;
            _connectButton.style.backgroundColor = Color.green;
            _connectButton.style.color = Color.black;
            _connectButton.style.fontSize = 25f;
            
            rootVisualElement.Add(splitView);
            rootVisualElement.Add(_connectButton);
        }

        private Label CreateLabel(string text)
        {
            var label = new Label(text);
            label.style.color = Color.red;
            
            return label;
        }

        private void OnComputerSelectionChanged(IEnumerable<object> selectedComputers)
        {
            _selectedComputer = null;
            
            foreach (var selectedComputer in selectedComputers)
            {
                if (selectedComputer is RouteComputer routeComputer)
                {
                    _selectedComputer = routeComputer;
                    break;
                }
            }

            if (_selectedComputer == null)
            {
                return;
            }

            Selection.activeObject = _selectedComputer.gameObject;
            
            _routePointList.makeItem = () => new Label();
            _routePointList.bindItem = (item, index) => { (item as Label).text = $"{_selectedComputer.RoutePoints[index].Index}"; };
            _routePointList.itemsSource = _selectedComputer.RoutePoints;
            _routePointList.onSelectionChange += OnRoutePointSelectionChanged;

            _routePointList.RefreshItems();
        }

        private void OnRoutePointSelectionChanged(IEnumerable<object> selectedRoutePoints)
        {
            var routePointSet = false;

            foreach (var selectedRoutePoint in selectedRoutePoints)
            {
                if (selectedRoutePoint is RoutePoint routePoint)
                {
                    _selectedRoutePoint = routePoint;
                    routePointSet = true;
                    break;
                }
            }

            SetAddButtonActivation(routePointSet);

            if (!routePointSet)
            {
                _selectedRoutePoint = default;
            }
        }

        private void SetAddButtonActivation(bool enableConnectButton)
        {
            _connectButton.visible = enableConnectButton;
        }
        
        private void OnConnectButtonClicked()
        {
            if (_node == null || _selectedComputer == null || !_selectedComputer.RoutePoints.Contains(_selectedRoutePoint))
            {
                return;
            }
            
            _onConnectionSelected?.Invoke(_selectedComputer, _selectedRoutePoint);
            GetWindow<RouteNodeEditorWindow>().Close();
        }
    }
}