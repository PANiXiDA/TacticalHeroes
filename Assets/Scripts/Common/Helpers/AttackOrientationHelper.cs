using Assets.Scripts.Common.Enumerations;

using UnityEngine;

namespace Assets.Scripts.Common.Helpers
{
    public readonly struct AttackOrientation
    {
        public AttackDirection Direction { get; }
        public Vector2Int CellOffset { get; }
        public float RotationZ { get; }

        public AttackOrientation(AttackDirection direction, Vector2Int cellOffset, float rotationZ)
        {
            Direction = direction;
            CellOffset = cellOffset;
            RotationZ = rotationZ;
        }
    }

    public static class AttackOrientationHelper
    {
        public static readonly (AttackDirection direction, float start, float end, Vector2Int offset)[] _map =
        {
            ( AttackDirection.Right,        337.5f, 360f, new( 1,  0) ),
            ( AttackDirection.Right,         0f,   22.5f, new( 1,  0) ),
            ( AttackDirection.BottomRight, 22.5f,  67.5f, new( 1, -1) ),
            ( AttackDirection.Bottom,      67.5f, 112.5f, new( 0, -1) ),
            ( AttackDirection.BottomLeft, 112.5f, 157.5f, new(-1, -1) ),
            ( AttackDirection.Left,       157.5f, 202.5f, new(-1,  0) ),
            ( AttackDirection.TopLeft,    202.5f, 247.5f, new(-1,  1) ),
            ( AttackDirection.Top,        247.5f, 292.5f, new( 0,  1) ),
            ( AttackDirection.TopRight,   292.5f, 337.5f, new( 1,  1) ),
        };

        public static AttackOrientation FromPositions(Vector2 from, Vector2 to)
        {
            var dirVec = (to - from).normalized;
            float angle = (Mathf.Atan2(dirVec.y, dirVec.x) * Mathf.Rad2Deg + 360f) % 360f;
            foreach (var (direction, start, end, offset) in _map)
            {
                if (angle >= start && angle < end)
                {
                    float rotationZ = Vector2.SignedAngle(Vector2.up, dirVec);
                    return new AttackOrientation(direction, offset, rotationZ);
                }
            }

            var fallback = _map[0];
            return new AttackOrientation(fallback.direction, fallback.offset, Vector2.SignedAngle(Vector2.up, (to - from).normalized));
        }
    }
}
