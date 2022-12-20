// Source : https://bitbucket.org/snippets/qzix13/deenBy/inject-buttons-for-any-monobehaviour
// Another link : https://forum.unity.com/threads/inject-buttons-for-any-monobehaviour-inheritance-to-inspector.507070/

using System.Linq;
using RouteDrifter.Utility.Attributes;
using UnityEditor;
using UnityEngine;

namespace RouteDrifter.Editor
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class RouteInspectorButtonEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawButton();
        }

        private void DrawButton()
        {
            var buttonMethods = AttributeReader.ReadMethods<RouteInspectorButtonAttribute>(target.GetType());
            var buttonCount = buttonMethods.Count();
            
            if (buttonCount > 0) 
            {
                var methodInfo = buttonMethods.ElementAt(0);
                var attribute = AttributeReader.GetMethodAttribute<RouteInspectorButtonAttribute>(methodInfo);
                var methodName = attribute.MethodName;
                
                if (GUILayout.Button(methodName)) {
                    methodInfo.Invoke(target, null);
                }
            }
        }
    }
}