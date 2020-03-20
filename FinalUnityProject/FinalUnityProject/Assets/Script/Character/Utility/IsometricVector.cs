using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public static class IsometricVector
    {
        private const float
            VerticalMin = Mathf.PI / 8,
            VerticalMax = Mathf.PI * 7 / 8,
            HorizontalMin = Mathf.PI * 3 / 8,
            HorizontalMax = Mathf.PI * 5 / 8;

        public static Vector2 IsoNormalized(this Vector2 myVector2)
        {
            float angle = Mathf.Atan2(myVector2.y, myVector2.x);
            return new Vector2(Mathf.Cos(angle), 0.6f * Mathf.Sin(angle));
        }

        public static float IsoDistance(this Vector3 myVector3, Vector3 otherVector3)
        {
            float angle = Mathf.Atan2(otherVector3.y - myVector3.y, otherVector3.x - myVector3.x);
            Vector3 temp = new Vector3(Mathf.Cos(angle), 1.667f * Mathf.Sin(angle)) * Vector3.Distance(myVector3, otherVector3);
            return temp.magnitude;
        }

        #region VerticalAndHorizontal
        public static void GetVerticalAndHorizontal(
            this Vector2 myVector2, out float vertical, out float horizontal)
        {
            var verticalAngle = Mathf.Atan2(myVector2.y, myVector2.x);
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

        public static float GetVertical(this Vector2 myVector2)
        {
            var angle = Mathf.Atan2(myVector2.y, myVector2.x);
            if (angle > VerticalMin && angle <= VerticalMax)
                return 1;
            else if (angle <= -VerticalMin && angle > -VerticalMax)
                return -1;
            else
                return 0;
        }

        public static float GetHorizontal(this Vector2 myVector2)
        {
            var angle = Mathf.Abs(Mathf.Atan2(myVector2.y, myVector2.x));
            if (angle > HorizontalMax)
                return -1;
            else if (angle < HorizontalMin)
                return 1;
            else
                return 0;
        }
        #endregion
    }
}