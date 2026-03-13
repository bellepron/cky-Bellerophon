using UnityEngine;

public class GizmoHelper
{
    public static void DrawWireSphere(Vector3 center, float radius, Color color, int segments = 12)
    {
        // Sphere approximation using circles on 3 planes
        DrawCircle(center, Vector3.up, radius, color, segments);
        DrawCircle(center, Vector3.right, radius, color, segments);
        DrawCircle(center, Vector3.forward, radius, color, segments);
    }

    public static void DrawCircle(Vector3 center, Vector3 normal, float radius, Color color, int segments)
    {
        Vector3 up = normal.normalized;
        Vector3 forward = Vector3.Slerp(up, -up, 0.5f); // arbitrary perpendicular vector
        Vector3 right = Vector3.Cross(up, forward).normalized;

        float step = 360f / segments;
        Vector3 prevPoint = center + right * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * step * Mathf.Deg2Rad;
            Vector3 nextPoint = center + (Mathf.Cos(angle) * right + Mathf.Sin(angle) * forward) * radius;
            Debug.DrawLine(prevPoint, nextPoint, color, 0f, false);
            prevPoint = nextPoint;
        }
    }
}