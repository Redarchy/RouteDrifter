using RouteDrifter.Computer;
using RouteDrifter.Models;
using RouteDrifter.Nodes;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RouteDrifter.Editor
{
    [CustomEditor(typeof(RouteNode))]
    public class RouteNodeEditor : UnityEditor.Editor
    {
        private RouteNode _routeNode;
        private VisualElement _rootVisualElement;

        private void OnEnable()
        {
            _routeNode = (RouteNode) target;
            _rootVisualElement = new VisualElement();
        }

        private void OnSceneGUI()
        {
            var defaultHandleColor = Handles.color;
            Handles.color = _routeNode.Connections.Count >= 2 ? Color.green : Color.red;
            
            Handles.SphereHandleCap(0, _routeNode.transform.position, Quaternion.identity, 0.1f, EventType.Repaint);
            
            Handles.color = defaultHandleColor;
        }

        public override VisualElement CreateInspectorGUI()
        {
            DrawInspector();

            return _rootVisualElement;
        }

        private void DrawInspector()
        {
            _rootVisualElement.Clear();
            
            var connectionBox = new Box();
            connectionBox.style.flexDirection = FlexDirection.Column;

            connectionBox.Add(new Label($"Connections: {_routeNode.Connections.Count}"));

            foreach (var connection in _routeNode.Connections)
            {
                connectionBox.Add(CreateNewConnectionEntry(connection.Computer, connection.Point.Index));
            }

            _rootVisualElement.Add(connectionBox);

            var activationToggle = new Toggle("Active");
            activationToggle.value = _routeNode.IsActive;
            activationToggle.RegisterValueChangedCallback(toggleChange =>
            {
                _routeNode.SetActivation(toggleChange.newValue);
                SaveRouteNode();
            });
            
            _rootVisualElement.Add(activationToggle);
            
            _rootVisualElement.Add(CreateAddNewConnectionButton());
        }

        private VisualElement CreateAddNewConnectionButton()
        {
            var addButton = new Button(OnAdd);
            addButton.text = "Add New Connection";

            return addButton;

            void OnAdd()
            {
                RouteNodeEditorWindow.Initialize(_routeNode, OnConnectionAdded);
                EditorWindow.GetWindow<RouteNodeEditorWindow>(RouteNodeEditorWindow.Title).ShowPopup();
            }

        }

        private void OnConnectionAdded(RouteComputer selectedRouteComputer, RoutePoint routePoint)
        {
            if (_routeNode.TryConnectComputer(selectedRouteComputer, routePoint))
            {
                SaveRouteNode();
                DrawInspector();
            }
        }

        private VisualElement CreateNewConnectionEntry(RouteComputer connectedComputer, int connectedRoutePointIndex)
        {
            var horizontalBox = new Box();
            horizontalBox.style.flexDirection = FlexDirection.Row;
            
            var computerReference = new ObjectField(connectedComputer.name);
            computerReference.objectType = typeof(RouteComputer);
            computerReference.allowSceneObjects = false;
            computerReference.pickingMode = PickingMode.Ignore;
            computerReference.value = connectedComputer;
            
            var routePointReference = new Label($"Point Index: {connectedRoutePointIndex}");
            routePointReference.focusable = false;
            
            var removeButton = new Button(() =>
            {
                _routeNode.DisconnectComputer(connectedComputer);
                SaveRouteNode();
                DrawInspector();
            });
            removeButton.text = "Remove";
            
            horizontalBox.Add(computerReference);
            horizontalBox.Add(routePointReference);
            horizontalBox.Add(removeButton);
            
            return horizontalBox;
        }

        private void SaveRouteNode()
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }
}