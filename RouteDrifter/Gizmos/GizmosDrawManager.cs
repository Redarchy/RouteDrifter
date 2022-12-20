using UnityEngine;

namespace RouteDrifter.Gizmos
{
    public class GizmosDrawManager : MonoBehaviour
    {
        // [Title("Options")]
        // [SerializeField]
        // private bool drawGizmos;
        //
        // private List<IRouteGizmosDrawable> gizmosDrawables = new List<IRouteGizmosDrawable>();
        //
        // private void OnEnable()
        // {
        //     EventManager.RegisterHandler<AddGizmosDrawableEvent>(OnAddGizmosDrawable);
        //     EventManager.RegisterHandler<RemoveGizmosDrawableEvent>(OnRemoveGizmosDrawable);
        // }
        //
        // private void OnDisable()
        // {
        //     EventManager.UnregisterHandler<AddGizmosDrawableEvent>(OnAddGizmosDrawable);
        //     EventManager.UnregisterHandler<RemoveGizmosDrawableEvent>(OnRemoveGizmosDrawable);
        // }
        //
        // private void OnAddGizmosDrawable(AddGizmosDrawableEvent obj)
        // {
        //     AddGizmosDrawable(obj.RouteGizmosDrawable);
        // }
        //
        // private void OnRemoveGizmosDrawable(RemoveGizmosDrawableEvent obj)
        // {
        //     RemoveGizmosDrawable(obj.RouteGizmosDrawable);
        // }
        //
        // private void AddGizmosDrawable(IRouteGizmosDrawable drawable)
        // {
        //     if (!gizmosDrawables.Contains(drawable))
        //     {
        //         gizmosDrawables.Add(drawable);
        //     }
        // }
        //
        // private void RemoveGizmosDrawable(IRouteGizmosDrawable drawable)
        // {
        //     if (gizmosDrawables.Contains(drawable))
        //     {
        //         gizmosDrawables.Remove(drawable);
        //     }
        // }
        //
        // private void OnDrawGizmos()
        // {
        //     if (!drawGizmos) return;
        //
        //     int count = gizmosDrawables.Count;
        //     for (int i = 0; i < count; i++)
        //     {
        //         gizmosDrawables[i].DrawGizmos();
        //     }
        // }
    }
}