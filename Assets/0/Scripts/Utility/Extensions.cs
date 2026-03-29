using UnityEngine;

namespace Bellepron.Utility
{
    public static class Extensions
    {
        public static void RotateToMouse(this Transform tr)
        {
            var cam = Camera.main;

            if (!cam) return;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, tr.position);

            if (plane.Raycast(ray, out float dist))
            {
                Vector3 point = ray.GetPoint(dist);
                Vector3 dir = point - tr.position;
                dir.y = 0f;
                if (dir != Vector3.zero)
                    tr.rotation = Quaternion.LookRotation(dir);
            }
        }

        public static bool Contains(this LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) != 0;
        }
    }
}