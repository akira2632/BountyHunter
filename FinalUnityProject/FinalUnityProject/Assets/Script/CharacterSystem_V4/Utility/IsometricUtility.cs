using UnityEngine;

public class IsometricUtility
{
    public static Vector2 ToIsometricDirection(Vector2 vector)
    {
        float angle = Mathf.Atan2(vector.y, vector.x);
        return new Vector2(0.5f * Mathf.Cos(angle), 0.3f * Mathf.Sin(angle));
    }

    public static float ToIsometricDistance(Vector3 from, Vector3 to)
    {
        float angle = Mathf.Atan2(to.y - from.y, to.x - from.x);
        float orignalDistance = Vector3.Distance(from, to);

        return new Vector2(orignalDistance * Mathf.Cos(angle),
            0.6f * orignalDistance * Mathf.Sin(angle)).magnitude;
    }
}
