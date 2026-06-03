using UnityEngine;

namespace NipaFriends
{
    public static class DebugGizmoUtil
    {
        const int DefaultCircleSegments = 32;

        public static void DrawCircleXZ(Vector2 position, float radius, Color color, float y)
        {
            DrawCircleXZ(position, radius, color, y, DefaultCircleSegments);
        }

        public static void DrawCircleXZ(Vector2 position, float radius, Color color, float y, int segments)
        {
            if(radius <= 0f || segments < 1)
            {
                return;
            }

            var prevGizmoColor = Gizmos.color;
            Gizmos.color = color;

            var step = 2f * Mathf.PI / segments;
            var prev = new Vector3(position.x + radius, y, position.y);

            for(var i = 1; i <= segments; i++)
            {
                var ang = step * i;
                var next = new Vector3(
                    position.x + Mathf.Cos(ang) * radius,
                    y,
                    position.y + Mathf.Sin(ang) * radius);
                Gizmos.DrawLine(prev, next);
                prev = next;
            }

            Gizmos.color = prevGizmoColor;
        }
    }
}
