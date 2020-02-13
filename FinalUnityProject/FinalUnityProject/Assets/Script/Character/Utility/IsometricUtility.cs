using UnityEngine;

namespace CharacterSystem
{
    public class IsometricUtility
    {
        const float
            VerticalMin = Mathf.PI / 8,
            VerticalMax = Mathf.PI * 7 / 8,
            HorizontalMin = Mathf.PI * 3 / 8,
            HorizontalMax = Mathf.PI * 5 / 8;

        #region ToIsometricVector2
        public static Vector2 ToVector2(Vector2 vector)
        {
            float angle = Mathf.Atan2(vector.y, vector.x);
            return new Vector2(Mathf.Cos(angle), 0.6f * Mathf.Sin(angle));
        }

        public static Vector2 ToVector2(Vector3 vector)
        {
            float angle = Mathf.Atan2(vector.y, vector.x);
            return new Vector2(Mathf.Cos(angle), 0.6f * Mathf.Sin(angle));
        }

        public static Vector2 ToVector2(Vector2 from, Vector2 to)
        {
            float angle = Mathf.Atan2(to.y - from.y, to.x - from.x);
            float orignalDistance = Vector2.Distance(from, to);

            return new Vector2(orignalDistance * Mathf.Cos(angle),
                0.6f * orignalDistance * Mathf.Sin(angle));
        }

        public static Vector2 ToVector2(Vector3 from, Vector3 to)
        {
            float angle = Mathf.Atan2(to.y - from.y, to.x - from.x);
            float orignalDistance = Vector2.Distance(from, to);

            return new Vector2(orignalDistance * Mathf.Cos(angle),
                0.6f * orignalDistance * Mathf.Sin(angle));
        }
        #endregion

        #region ToIsometricVector3
        public static Vector3 ToVector3(Vector2 vector)
        {
            float angle = Mathf.Atan2(vector.y, vector.x);
            return new Vector3(Mathf.Cos(angle), 0.6f * Mathf.Sin(angle));
        }

        public static Vector3 ToVector3(Vector3 vector)
        {
            float angle = Mathf.Atan2(vector.y, vector.x);
            return new Vector3(Mathf.Cos(angle), 0.6f * Mathf.Sin(angle));
        }

        public static Vector3 ToVector3(Vector2 from, Vector2 to)
        {
            float angle = Mathf.Atan2(to.y - from.y, to.x - from.x);
            float orignalDistance = Vector2.Distance(from, to);

            return new Vector2(orignalDistance * Mathf.Cos(angle),
                0.6f * orignalDistance * Mathf.Sin(angle));
        }

        public static Vector3 ToVector3(Vector3 from, Vector3 to)
        {
            float angle = Mathf.Atan2(to.y - from.y, to.x - from.x);
            float orignalDistance = Vector2.Distance(from, to);

            return new Vector2(orignalDistance * Mathf.Cos(angle),
                0.6f * orignalDistance * Mathf.Sin(angle));
        }
        #endregion

        #region ToIsometricDistance
        public static float ToDistance(Vector2 from, Vector2 to)
            => ToVector2(from, to).magnitude;

        public static float ToDistance(Vector3 from, Vector3 to)
            => ToVector2(from, to).magnitude;
        #endregion

        #region VerticalAndHorizontal
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
        #endregion
    }
}