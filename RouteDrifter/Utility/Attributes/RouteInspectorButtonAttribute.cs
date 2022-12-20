using System;

namespace RouteDrifter.Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RouteInspectorButtonAttribute : Attribute
    {
        public string MethodName { get; }

        public RouteInspectorButtonAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
}