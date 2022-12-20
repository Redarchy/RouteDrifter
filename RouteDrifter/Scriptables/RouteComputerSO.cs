// using RouteDrifter.Computer;
// using UnityEditor;
// using UnityEngine;
//
// namespace RouteDrifter.Scriptables
// {
//     [CreateAssetMenu(fileName = "RouteComputerSO", menuName = "ScriptableObject/RouteDrifter/RouteComputer", order = 0)]
//     public class RouteComputerSO : ScriptableObject
//     {
//         [SerializeField]
//         private RouteComputerDataContainer routeComputerDataContainer;
//
//         public RouteComputerDataContainer RouteComputerDataContainer => routeComputerDataContainer;
//
//         
// #if UNITY_EDITOR
//
//         #region Editor Functionalities
//         
//         [Button("Copy From RouteComputer in Scene"), FoldoutGroup("Editor Functionalities")]
//         private void Editor_CopyFromRouteComputerInScene(RouteComputer routeComputer)
//         {
//             routeComputerDataContainer = routeComputer.GetRouteComputerDataContainer();
//             EditorUtility.SetDirty(this);
//         }
//
//         #endregion
// #endif
//     }
// }