﻿using UnityEngine;

public class IsometricUtility
{
    const float
        VerticalMin = Mathf.PI / 8,
        VerticalMax = Mathf.PI * 7 / 8,
        HorizontalMin = Mathf.PI * 3 / 8,
        HorizontalMax = Mathf.PI * 5 / 8;

    public static Vector2 ToIsometricDirection(Vector2 vector)
    {
        float angle = Mathf.Atan2(vector.y, vector.x);
        return new Vector2(0.5f * Mathf.Cos(angle), 0.3f * Mathf.Sin(angle));
    }

    public static Vector3 ToIsometricVector(Vector3 vector)
    {
        float angle = Mathf.Atan2(vector.y, vector.x);

        return new Vector3(vector.magnitude * Mathf.Cos(angle),
            0.6f * vector.magnitude * Mathf.Sin(angle));
    }

    public static float ToIsometricDistance(Vector3 from, Vector3 to)
    {
        float angle = Mathf.Atan2(to.y - from.y, to.x - from.x);
        float orignalDistance = Vector3.Distance(from, to);

        return new Vector2(orignalDistance * Mathf.Cos(angle),
            0.6f * orignalDistance * Mathf.Sin(angle)).magnitude;
    }

    public static void GetVerticalAndHorizontal(Vector2 direction, out float vertical, out float horizontal)
    {
        var verticalAngle = Mathf.Atan2(direction.y, direction.x);
        var horizontalAngle = Mathf.Abs(verticalAngle);

        if (verticalAngle > VerticalMin && verticalAngle <= VerticalMax)
            vertical = 1;
        else if (verticalAngle <= -VerticalMin && verticalAngle > -VerticalMax)
            vertical = -1;
        else
            vertical = 0;

        if (horizontalAngle > HorizontalMax)
            horizontal = -1;
        else if (horizontalAngle < HorizontalMin)
            horizontal = 1;
        else
            horizontal = 0;
    }

    public static float GetVertical(Vector2 direction)
    {
        var angle = Mathf.Atan2(direction.y, direction.x);
        if (angle > VerticalMin && angle <= VerticalMax)
            return 1;
        else if (angle <= -VerticalMin && angle > -VerticalMax)
            return -1;
        else
            return 0;
    }

    public static float GetHorizontal(Vector2 direction)
    {
        var angle = Mathf.Abs(Mathf.Atan2(direction.y, direction.x));
        if (angle > HorizontalMax)
            return -1;
        else if (angle < HorizontalMin)
            return 1;
        else
            return 0;
    }
}
