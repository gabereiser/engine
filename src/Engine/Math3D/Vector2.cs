// Author:
//       Gabriel Reiser <gabe@reisergames.com>
//
// Copyright (c) 2010-2016 Reiser Games, LLC.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Reactor.Math3D
{
    [StructLayout(LayoutKind.Explicit, Size = 8, Pack = 4)]
    public struct Vector2 : IEquatable<Vector2>
    {
        #region Private Fields

        #endregion Private Fields


        #region Public Fields

        [FieldOffset(0)] public float X;

        [FieldOffset(4)] public float Y;

        #endregion Public Fields


        #region Properties

        public static Vector2 Zero { get; } = new Vector2(0f, 0f);

        public static Vector2 One { get; } = new Vector2(1f, 1f);

        public static Vector2 UnitX { get; } = new Vector2(1f, 0f);

        public static Vector2 UnitY { get; } = new Vector2(0f, 1f);

        public float this[int index]
        {
            get
            {
                if (index > 1)
                    throw new IndexOutOfRangeException();
                if (index == 0)
                    return X;
                return Y;
            }
            set
            {
                if (index > 1)
                    throw new IndexOutOfRangeException();
                if (index == 0)
                    X = value;
                Y = value;
            }
        }
        #endregion Properties


        #region Constructors

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2(float value)
        {
            X = value;
            Y = value;
        }

        #endregion Constructors


        #region Public Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Add(Vector2 value1, Vector2 value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            return value1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Barycentric(Vector2 value1, Vector2 value2, Vector2 value3, float amount1, float amount2)
        {
            return new Vector2(
                MathHelper.Barycentric(value1.X, value2.X, value3.X, amount1, amount2),
                MathHelper.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Barycentric(ref Vector2 value1, ref Vector2 value2, ref Vector2 value3, float amount1,
            float amount2, out Vector2 result)
        {
            result = new Vector2(
                MathHelper.Barycentric(value1.X, value2.X, value3.X, amount1, amount2),
                MathHelper.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 CatmullRom(Vector2 value1, Vector2 value2, Vector2 value3, Vector2 value4, float amount)
        {
            return new Vector2(
                MathHelper.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount),
                MathHelper.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CatmullRom(ref Vector2 value1, ref Vector2 value2, ref Vector2 value3, ref Vector2 value4,
            float amount, out Vector2 result)
        {
            result = new Vector2(
                MathHelper.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount),
                MathHelper.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Clamp(Vector2 value1, Vector2 min, Vector2 max)
        {
            return new Vector2(
                MathHelper.Clamp(value1.X, min.X, max.X),
                MathHelper.Clamp(value1.Y, min.Y, max.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clamp(ref Vector2 value1, ref Vector2 min, ref Vector2 max, out Vector2 result)
        {
            result = new Vector2(
                MathHelper.Clamp(value1.X, min.X, max.X),
                MathHelper.Clamp(value1.Y, min.Y, max.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(Vector2 value1, Vector2 value2)
        {
            float v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
            return (float)System.Math.Sqrt(v1 * v1 + v2 * v2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Distance(ref Vector2 value1, ref Vector2 value2, out float result)
        {
            float v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
            result = (float)System.Math.Sqrt(v1 * v1 + v2 * v2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceSquared(Vector2 value1, Vector2 value2)
        {
            float v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
            return v1 * v1 + v2 * v2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DistanceSquared(ref Vector2 value1, ref Vector2 value2, out float result)
        {
            float v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
            result = v1 * v1 + v2 * v2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Divide(Vector2 value1, Vector2 value2)
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            return value1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Divide(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Divide(Vector2 value1, float divider)
        {
            var factor = 1 / divider;
            value1.X *= factor;
            value1.Y *= factor;
            return value1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Divide(ref Vector2 value1, float divider, out Vector2 result)
        {
            var factor = 1 / divider;
            result.X = value1.X * factor;
            result.Y = value1.Y * factor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(Vector2 value1, Vector2 value2)
        {
            return value1.X * value2.X + value1.Y * value2.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Dot(ref Vector2 value1, ref Vector2 value2, out float result)
        {
            result = value1.X * value2.X + value1.Y * value2.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2) return Equals((Vector2)obj);

            return false;
        }

        public bool Equals(Vector2 other)
        {
            return X == other.X && Y == other.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Reflect(Vector2 vector, Vector2 normal)
        {
            Vector2 result;
            var val = 2.0f * (vector.X * normal.X + vector.Y * normal.Y);
            result.X = vector.X - normal.X * val;
            result.Y = vector.Y - normal.Y * val;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Reflect(ref Vector2 vector, ref Vector2 normal, out Vector2 result)
        {
            var val = 2.0f * (vector.X * normal.X + vector.Y * normal.Y);
            result.X = vector.X - normal.X * val;
            result.Y = vector.Y - normal.Y * val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Hermite(Vector2 value1, Vector2 tangent1, Vector2 value2, Vector2 tangent2, float amount)
        {
            var result = new Vector2();
            Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount, out result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Hermite(ref Vector2 value1, ref Vector2 tangent1, ref Vector2 value2, ref Vector2 tangent2,
            float amount, out Vector2 result)
        {
            result.X = MathHelper.Hermite(value1.X, tangent1.X, value2.X, tangent2.X, amount);
            result.Y = MathHelper.Hermite(value1.Y, tangent1.Y, value2.Y, tangent2.Y, amount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Length()
        {
            return (float)System.Math.Sqrt(X * X + Y * Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float LengthSquared()
        {
            return X * X + Y * Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Lerp(Vector2 value1, Vector2 value2, float amount)
        {
            return new Vector2(
                MathHelper.Lerp(value1.X, value2.X, amount),
                MathHelper.Lerp(value1.Y, value2.Y, amount));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Lerp(ref Vector2 value1, ref Vector2 value2, float amount, out Vector2 result)
        {
            result = new Vector2(
                MathHelper.Lerp(value1.X, value2.X, amount),
                MathHelper.Lerp(value1.Y, value2.Y, amount));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Max(Vector2 value1, Vector2 value2)
        {
            return new Vector2(value1.X > value2.X ? value1.X : value2.X,
                value1.Y > value2.Y ? value1.Y : value2.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Max(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result.X = value1.X > value2.X ? value1.X : value2.X;
            result.Y = value1.Y > value2.Y ? value1.Y : value2.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Min(Vector2 value1, Vector2 value2)
        {
            return new Vector2(value1.X < value2.X ? value1.X : value2.X,
                value1.Y < value2.Y ? value1.Y : value2.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Min(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result.X = value1.X < value2.X ? value1.X : value2.X;
            result.Y = value1.Y < value2.Y ? value1.Y : value2.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Multiply(Vector2 value1, Vector2 value2)
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            return value1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Multiply(Vector2 value1, float scaleFactor)
        {
            value1.X *= scaleFactor;
            value1.Y *= scaleFactor;
            return value1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply(ref Vector2 value1, float scaleFactor, out Vector2 result)
        {
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Negate(Vector2 value)
        {
            value.X = -value.X;
            value.Y = -value.Y;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Negate(ref Vector2 value, out Vector2 result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize()
        {
            var val = 1.0f / (float)System.Math.Sqrt(X * X + Y * Y);
            X *= val;
            Y *= val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Normalize(Vector2 value)
        {
            var val = 1.0f / (float)System.Math.Sqrt(value.X * value.X + value.Y * value.Y);
            value.X *= val;
            value.Y *= val;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Normalize(ref Vector2 value, out Vector2 result)
        {
            var val = 1.0f / (float)System.Math.Sqrt(value.X * value.X + value.Y * value.Y);
            result.X = value.X * val;
            result.Y = value.Y * val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 SmoothStep(Vector2 value1, Vector2 value2, float amount)
        {
            return new Vector2(
                MathHelper.SmoothStep(value1.X, value2.X, amount),
                MathHelper.SmoothStep(value1.Y, value2.Y, amount));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SmoothStep(ref Vector2 value1, ref Vector2 value2, float amount, out Vector2 result)
        {
            result = new Vector2(
                MathHelper.SmoothStep(value1.X, value2.X, amount),
                MathHelper.SmoothStep(value1.Y, value2.Y, amount));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Subtract(Vector2 value1, Vector2 value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            return value1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Subtract(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Transform(Vector2 position, Matrix matrix)
        {
            Transform(ref position, ref matrix, out position);
            return position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform(ref Vector2 position, ref Matrix matrix, out Vector2 result)
        {
            result = new Vector2(position.X * matrix.M11 + position.Y * matrix.M21 + matrix.M41,
                position.X * matrix.M12 + position.Y * matrix.M22 + matrix.M42);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Transform(Vector2 position, Quaternion quat)
        {
            Transform(ref position, ref quat, out position);
            return position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform(ref Vector2 position, ref Quaternion quat, out Vector2 result)
        {
            Quaternion v = new Quaternion(position.X, position.Y, 0, 0), i, t;
            Quaternion.Inverse(ref quat, out i);
            Quaternion.Multiply(ref quat, ref v, out t);
            Quaternion.Multiply(ref t, ref i, out v);

            result = new Vector2(v.X, v.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform(
            Vector2[] sourceArray,
            ref Matrix matrix,
            Vector2[] destinationArray)
        {
            Transform(sourceArray, 0, ref matrix, destinationArray, 0, sourceArray.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform(
            Vector2[] sourceArray,
            int sourceIndex,
            ref Matrix matrix,
            Vector2[] destinationArray,
            int destinationIndex,
            int length)
        {
            for (var x = 0; x < length; x++)
            {
                var position = sourceArray[sourceIndex + x];
                var destination = destinationArray[destinationIndex + x];
                destination.X = position.X * matrix.M11 + position.Y * matrix.M21 + matrix.M41;
                destination.Y = position.X * matrix.M12 + position.Y * matrix.M22 + matrix.M42;
                destinationArray[destinationIndex + x] = destination;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 TransformNormal(Vector2 normal, Matrix matrix)
        {
            TransformNormal(ref normal, ref matrix, out normal);
            return normal;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TransformNormal(ref Vector2 normal, ref Matrix matrix, out Vector2 result)
        {
            result = new Vector2(normal.X * matrix.M11 + normal.Y * matrix.M21,
                normal.X * matrix.M12 + normal.Y * matrix.M22);
        }

        public override string ToString()
        {
            var currentCulture = CultureInfo.CurrentCulture;
            return string.Format(currentCulture, "{{X:{0} Y:{1}}}", new object[]
            {
                X.ToString(currentCulture), Y.ToString(currentCulture)
            });
        }

        #endregion Public Methods


        #region Operators

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator -(Vector2 value)
        {
            value.X = -value.X;
            value.Y = -value.Y;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector2 value1, Vector2 value2)
        {
            return value1.X == value2.X && value1.Y == value2.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector2 value1, Vector2 value2)
        {
            return value1.X != value2.X || value1.Y != value2.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator +(Vector2 value1, Vector2 value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            return value1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator -(Vector2 value1, Vector2 value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            return value1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator *(Vector2 value1, Vector2 value2)
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            return value1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator *(Vector2 value, float scaleFactor)
        {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator *(float scaleFactor, Vector2 value)
        {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator /(Vector2 value1, Vector2 value2)
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            return value1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator /(Vector2 value1, float divider)
        {
            var factor = 1 / divider;
            value1.X *= factor;
            value1.Y *= factor;
            return value1;
        }

        /// <summary>
        ///     Converts a Reactor Vector2 to a System Numerics Vector2 (supposedly they are fast.)
        /// </summary>
        /// <param name="value">a Reactor Vector2</param>
        /// <returns>a System Numerics Vector2</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator System.Numerics.Vector2(Vector2 value)
        {
            return new System.Numerics.Vector2(value.X, value.Y);
        }

        /// <summary>
        ///     Converts a System Numerics Vector2 to a Reactor Vector2 (supposedly they are just as fast.)
        /// </summary>
        /// <param name="value">a System Numerics Vector2</param>
        /// <returns>a Reactor Vector2</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector2(System.Numerics.Vector2 value)
        {
            return new Vector2(value.X, value.Y);
        }

        #endregion Operators
    }
}