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

        #region ToIsometricVector2

        #endregion

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