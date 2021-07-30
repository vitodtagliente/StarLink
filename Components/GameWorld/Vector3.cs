using System;

namespace GameWorld
{
    [Serializable]
    public class Vector3
    {
        public static readonly Vector3 Zero = new Vector3();
        public static readonly Vector3 One = new Vector3(1.0f);

        public Vector3()
        {
            X = Y = Z = 0.0f;
        }

        public Vector3(float value)
        {
            X = Y = Z = value;
        }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Vector3 vector &&
                   X == vector.X &&
                   Y == vector.Y &&
                   Z == vector.Z;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public override string ToString()
        {
            return string.Format("x:{0},y:{1},z:{2}", X, Y, Z);
        }

        public static bool TryParse(string text, out Vector3 vector)
        {
            vector = new Vector3();

            return false;
        }
    }
}
