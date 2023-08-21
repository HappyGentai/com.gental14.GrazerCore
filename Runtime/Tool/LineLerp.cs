using UnityEngine;

namespace GrazerCore.Tool {
    public class LineLerp
    {
        public static Vector3 Quadraticlerp(Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            Vector3 p1Top2 = Vector3.Lerp(p1, p2, t);
            Vector3 p2Top3 = Vector3.Lerp(p2, p3, t);
            return Vector3.Lerp(p1Top2, p2Top3, t);
        }

        public static Vector3 CubicLerp(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
        {
            Vector3 p1Tp2_p2Tp3 = Quadraticlerp(p1, p2, p3, t);
            Vector3 p2Tp3_p3Tp4 = Quadraticlerp(p2, p3, p4, t);
            return Vector3.Lerp(p1Tp2_p2Tp3, p2Tp3_p3Tp4, t);
        }

        public static Vector2 Quadraticlerp(Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            Vector2 p1Top2 = Vector2.Lerp(p1, p2, t);
            Vector2 p2Top3 = Vector2.Lerp(p2, p3, t);
            return Vector2.Lerp(p1Top2, p2Top3, t);
        }

        public static Vector2 CubicLerp(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, float t)
        {
            Vector2 p1Tp2_p2Tp3 = Quadraticlerp(p1, p2, p3, t);
            Vector2 p2Tp3_p3Tp4 = Quadraticlerp(p2, p3, p4, t);
            return Vector2.Lerp(p1Tp2_p2Tp3, p2Tp3_p3Tp4, t);
        }
    }
}
