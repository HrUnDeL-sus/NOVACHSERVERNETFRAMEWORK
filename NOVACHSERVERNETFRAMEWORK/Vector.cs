using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOVACHSERVERNETFRAMEWORK
{
    internal class Vector3
    {
        public float X, Y, Z;
        public float LengthSquared { get => X * X + Y * Y + Z * Z; }
        public float Length { get => (float)Math.Sqrt(LengthSquared); }
        public Vector3(float x,float y,float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public static Vector3 operator + (Vector3 a,Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
        public static Vector3 operator *(Vector3 a, float n)
        {
            return new Vector3(a.X *n, a.Y * n, a.Z * n);
        }
        public static Vector3 operator -(Vector3 a, float n)
        {
            return new Vector3(a.X - n, a.Y - n, a.Z - n);
        }
        public static Vector3 operator +(Vector3 a, float n)
        {
            return new Vector3(a.X + n, a.Y + n, a.Z + n);
        }
        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }
        public static Vector3 operator /(Vector3 a, float n)
        {
            return new Vector3(a.X / n, a.Y / n, a.Z / n);
        }
     

        private static float Compare(Vector3 a, Vector3 b)
        {
            return a.LengthSquared - b.LengthSquared;
        }

        public static bool operator >(Vector3 a, Vector3 b)
        {
            return Compare(a, b) > 0;
        }

        public static bool operator <(Vector3 a, Vector3 b)
        {
            return Compare(a, b) < 0;
        }
        public static Vector3 One()
        {
            return new Vector3(1, 1, 1);
        }
        public static Vector3 Zero()
        {
            return new Vector3(0, 0, 0);
        }
    }
    
}
