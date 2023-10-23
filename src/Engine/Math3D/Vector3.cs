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
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Reactor.Math3D
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit, Size = 12, Pack = 4)]
    public struct Vector3i : IEquatable<Vector3i>
    {
        [FieldOffset(0)] public int X;
        [FieldOffset(4)] public int Y;
        [FieldOffset(8)] public int Z;

        public override bool Equals(object obj)
        {
            if (!(obj is Vector3i))
                return false;

            var other = (Vector3i)obj;
            return X == other.X &&
                   Y == other.Y &&
                   Z == other.Z;
        }

        public bool Equals(Vector3i other)
        {
            return X == other.X &&
                   Y == other.Y &&
                   Z == other.Z;
        }

        public override int GetHashCode()
        {
            return X ^ Y ^ Z;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Explicit, Size = 12, Pack = 4)]
    public struct Vector3 : IEquatable<Vector3>
    {
        #region Private Fields

        private static Vector3 zero = new Vector3(0f, 0f, 0f);

        #endregion Private Fields


        #region Public Fields

        [FieldOffset(0)] public float X;

        [FieldOffset(4)] public float Y;

        [FieldOffset(8)] public float Z;

        #endregion Public Fields


        #region Properties

        public static Vector3 Zero => zero;

        public static Vector3 One { get; } = new Vector3(1f, 1f, 1f);

        public static Vector3 UnitX { get; } = new Vector3(1f, 0f, 0f);

        public static Vector3 UnitY { get; } = new Vector3(0f, 1f, 0f);

        public static Vector3 UnitZ { get; } = new Vector3(0f, 0f, 1f);

        public static Vector3 Up { get; } = new Vector3(0f, 1f, 0f);

        public static Vector3 Down { get; } = new Vector3(0f, -1f, 0f);

        public static Vector3 Right { get; } = new Vector3(1f, 0f, 0f);

        public static Vector3 Left { get; } = new Vector3(-1f, 0f, 0f);

        public static Vector3 Forward { get; } = new Vector3(0f, 0f, -1f);

        public static Vector3 Backward { get; } = new Vector3(0f, 0f, 1f);

        public float this[int index]
        {
            get
            {
                if (index > 2)
                    throw new IndexOutOfRangeException();
                if (index == 0)
                    return X;
                if (index == 1)
                    return Y;
                return Z;
            }
            set
            {
                if (index > 2)
                    throw new IndexOutOfRangeException();
                if (index == 0)
                    X = value;
                if (index == 1)
                    Y = value;
                Z = value;
            }
        }
        #endregion Properties


        #region Constructors

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }


        public Vector3(float value)
        {
            X = value;
            Y = value;
            Z = value;
        }


        public Vector3(Vector2 value, float z)
        {
            X = value.X;
            Y = value.Y;
            Z = z;
        }

        #endregion Constructors


        #region Public Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Add(Vector3 value1, Vector3 value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            value1.Z += value2.Z;
            return value1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
            result.Z = value1.Z + value2.Z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Barycentric(Vector3 value1, Vector3 value2, Vector3 value3, float amount1, float amount2)
        {
            return new Vector3(
                MathHelper.Barycentric(value1.X, value2.X, value3.X, amount1, amount2),
                MathHelper.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2),
                MathHelper.Barycentric(value1.Z, value2.Z, value3.Z, amount1, amount2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Barycentric(ref Vector3 value1, ref Vector3 value2, ref Vector3 value3, float amount1,
            float amount2, out Vector3 result)
        {
            result = new Vector3(
                MathHelper.Barycentric(value1.X, value2.X, value3.X, amount1, amount2),
                MathHelper.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2),
                MathHelper.Barycentric(value1.Z, value2.Z, value3.Z, amount1, amount2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 CatmullRom(Vector3 value1, Vector3 value2, Vector3 value3, Vector3 value4, float amount)
        {
            return new Vector3(
                MathHelper.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount),
                MathHelper.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount),
                MathHelper.CatmullRom(value1.Z, value2.Z, value3.Z, value4.Z, amount));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CatmullRom(ref Vector3 value1, ref Vector3 value2, ref Vector3 value3, ref Vector3 value4,
            float amount, out Vector3 result)
        {
            result = new Vector3(
                MathHelper.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount),
                MathHelper.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount),
                MathHelper.CatmullRom(value1.Z, value2.Z, value3.Z, value4.Z, amount));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Clamp(Vector3 value1, Vector3 min, Vector3 max)
        {
            return new Vector3(
                MathHelper.Clamp(value1.X, min.X, max.X),
                MathHelper.Clamp(value1.Y, min.Y, max.Y),
                MathHelper.Clamp(value1.Z, min.Z, max.Z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clamp(ref Vector3 value1, ref Vector3 min, ref Vector3 max, out Vector3 result)
        {
            result = new Vector3(
                MathHelper.Clamp(value1.X, min.X, max.X),
                MathHelper.Clamp(value1.Y, min.Y, max.Y),
                MathHelper.Clamp(value1.Z, min.Z, max.Z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Cross(Vector3 vector1, Vector3 vector2)
        {
            Cross(ref vector1, ref vector2, out vector1);
            return vector1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Cross(ref Vector3 vector1, ref Vector3 vector2, out Vector3 result)
        {
            result = new Vector3(vector1.Y * vector2.Z - vector2.Y * vector1.Z,
                -(vector1.X * vector2.Z - vector2.X * vector1.Z),
                vector1.X * vector2.Y - vector2.X * vector1.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(Vector3 vector1, Vector3 vector2)
        {
            float result;
            DistanceSquared(ref vector1, ref vector2, out result);
            return (float)System.Math.Sqrt(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Distance(ref Vector3 value1, ref Vector3 value2, out float result)
        {
            DistanceSquared(ref value1, ref value2, out result);
            result = (float)System.Math.Sqrt(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceSquared(Vector3 value1, Vector3 value2)
        {
            float result;
            DistanceSquared(ref value1, ref value2, out result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DistanceSquared(ref Vector3 value1, ref Vector3 value2, out float result)
        {
            result = (value1.X - value2.X) * (value1.X - value2.X) +
                     (value1.Y - value2.Y) * (value1.Y - value2.Y) +
                     (value1.Z - value2.Z) * (value1.Z - value2.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Divide(Vector3 value1, Vector3 value2)
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            value1.Z /= value2.Z;
            return value1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Divide(Vector3 value1, float value2)
        {
            var factor = 1 / value2;
            value1.X *= factor;
            value1.Y *= factor;
            value1.Z *= factor;
            return value1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Divide(ref Vector3 value1, float divisor, out Vector3 result)
        {
            var factor = 1 / divisor;
            result.X = value1.X * factor;
            result.Y = value1.Y * factor;
            result.Z = value1.Z * factor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Divide(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
            result.Z = value1.Z / value2.Z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(Vector3 vector1, Vector3 vector2)
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Dot(ref Vector3 vector1, ref Vector3 vector2, out float result)
        {
            result = vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (!(obj is Vector3))
                return false;

            var other = (Vector3)obj;
            return X == other.X &&
                   Y == other.Y &&
                   Z == other.Z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector3 other)
        {
            return X == other.X &&
                   Y == other.Y &&
                   Z == other.Z;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Hermite(Vector3 value1, Vector3 tangent1, Vector3 value2, Vector3 tangent2, float amount)
        {
            var result = new Vector3();
            Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount, out result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Hermite(ref Vector3 value1, ref Vector3 tangent1, ref Vector3 value2, ref Vector3 tangent2,
            float amount, out Vector3 result)
        {
            result.X = MathHelper.Hermite(value1.X, tangent1.X, value2.X, tangent2.X, amount);
            result.Y = MathHelper.Hermite(value1.Y, tangent1.Y, value2.Y, tangent2.Y, amount);
            result.Z = MathHelper.Hermite(value1.Z, tangent1.Z, value2.Z, tangent2.Z, amount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Length()
        {
            float result;
            DistanceSquared(ref this, ref zero, out result);
            return (float)System.Math.Sqrt(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float LengthSquared()
        {
            float result;
            DistanceSquared(ref this, ref zero, out result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Lerp(Vector3 value1, Vector3 value2, float amount)
        {
            return new Vector3(
                MathHelper.Lerp(value1.X, value2.X, amount),
                MathHelper.Lerp(value1.Y, value2.Y, amount),
                MathHelper.Lerp(value1.Z, value2.Z, amount));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Lerp(ref Vector3 value1, ref Vector3 value2, float amount, out Vector3 result)
        {
            result = new Vector3(
                MathHelper.Lerp(value1.X, value2.X, amount),
                MathHelper.Lerp(value1.Y, value2.Y, amount),
                MathHelper.Lerp(value1.Z, value2.Z, amount));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Max(Vector3 value1, Vector3 value2)
        {
            return new Vector3(
                MathHelper.Max(value1.X, value2.X),
                MathHelper.Max(value1.Y, value2.Y),
                MathHelper.Max(value1.Z, value2.Z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Max(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result = new Vector3(
                MathHelper.Max(value1.X, value2.X),
                MathHelper.Max(value1.Y, value2.Y),
                MathHelper.Max(value1.Z, value2.Z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Min(Vector3 value1, Vector3 value2)
        {
            return new Vector3(
                MathHelper.Min(value1.X, value2.X),
                MathHelper.Min(value1.Y, value2.Y),
                MathHelper.Min(value1.Z, value2.Z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Min(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result = new Vector3(
                MathHelper.Min(value1.X, value2.X),
                MathHelper.Min(value1.Y, value2.Y),
                MathHelper.Min(value1.Z, value2.Z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Multiply(Vector3 value1, Vector3 value2)
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            value1.Z *= value2.Z;
            return value1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Multiply(Vector3 value1, float scaleFactor)
        {
            value1.X *= scaleFactor;
            value1.Y *= scaleFactor;
            value1.Z *= scaleFactor;
            return value1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply(ref Vector3 value1, float scaleFactor, out Vector3 result)
        {
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
            result.Z = value1.Z * scaleFactor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
            result.Z = value1.Z * value2.Z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Negate(Vector3 value)
        {
            value = new Vector3(-value.X, -value.Y, -value.Z);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Negate(ref Vector3 value, out Vector3 result)
        {
            result = new Vector3(-value.X, -value.Y, -value.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize()
        {
            Normalize(ref this, out this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Normalize(Vector3 vector)
        {
            Normalize(ref vector, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Normalize(ref Vector3 value, out Vector3 result)
        {
            float factor;
            Distance(ref value, ref zero, out factor);
            factor = 1f / factor;
            result.X = value.X * factor;
            result.Y = value.Y * factor;
            result.Z = value.Z * factor;
            if (float.IsNaN(result.X)) result.X = 0;
            if (float.IsNaN(result.Y)) result.Y = 0;
            if (float.IsNaN(result.Z)) result.Z = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Reflect(Vector3 vector, Vector3 normal)
        {
            // I is the original array
            // N is the normal of the incident plane
            // R = I - (2 * N * ( DotProduct[ I,N] ))
            Vector3 reflectedVector;
            // inline the dotProduct here instead of calling method
            var dotProduct = vector.X * normal.X + vector.Y * normal.Y + vector.Z * normal.Z;
            reflectedVector.X = vector.X - 2.0f * normal.X * dotProduct;
            reflectedVector.Y = vector.Y - 2.0f * normal.Y * dotProduct;
            reflectedVector.Z = vector.Z - 2.0f * normal.Z * dotProduct;

            return reflectedVector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Reflect(ref Vector3 vector, ref Vector3 normal, out Vector3 result)
        {
            // I is the original array
            // N is the normal of the incident plane
            // R = I - (2 * N * ( DotProduct[ I,N] ))

            // inline the dotProduct here instead of calling method
            var dotProduct = vector.X * normal.X + vector.Y * normal.Y + vector.Z * normal.Z;
            result.X = vector.X - 2.0f * normal.X * dotProduct;
            result.Y = vector.Y - 2.0f * normal.Y * dotProduct;
            result.Z = vector.Z - 2.0f * normal.Z * dotProduct;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 SmoothStep(Vector3 value1, Vector3 value2, float amount)
        {
            return new Vector3(
                MathHelper.SmoothStep(value1.X, value2.X, amount),
                MathHelper.SmoothStep(value1.Y, value2.Y, amount),
                MathHelper.SmoothStep(value1.Z, value2.Z, amount));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SmoothStep(ref Vector3 value1, ref Vector3 value2, float amount, out Vector3 result)
        {
            result = new Vector3(
                MathHelper.SmoothStep(value1.X, value2.X, amount),
                MathHelper.SmoothStep(value1.Y, value2.Y, amount),
                MathHelper.SmoothStep(value1.Z, value2.Z, amount));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Subtract(Vector3 value1, Vector3 value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            value1.Z -= value2.Z;
            return value1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Subtract(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
            result.Z = value1.Z - value2.Z;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(32);
            sb.Append("{X:");
            sb.Append(X);
            sb.Append(" Y:");
            sb.Append(Y);
            sb.Append(" Z:");
            sb.Append(Z);
            sb.Append("}");
            return sb.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Transform(Vector3 position, Matrix matrix)
        {
            Transform(ref position, ref matrix, out position);
            return position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform(ref Vector3 position, ref Matrix matrix, out Vector3 result)
        {
            result = new Vector3(
                position.X * matrix.M11 + position.Y * matrix.M21 + position.Z * matrix.M31 + matrix.M41,
                position.X * matrix.M12 + position.Y * matrix.M22 + position.Z * matrix.M32 + matrix.M42,
                position.X * matrix.M13 + position.Y * matrix.M23 + position.Z * matrix.M33 + matrix.M43);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform(Vector3[] sourceArray, ref Matrix matrix, Vector3[] destinationArray)
        {
            Debug.Assert(destinationArray.Length >= sourceArray.Length,
                "The destination array is smaller than the source array.");

            // TODO: Are there options on some platforms to implement a vectorized version of this?

            for (var i = 0; i < sourceArray.Length; i++)
            {
                var position = sourceArray[i];
                destinationArray[i] =
                    new Vector3(
                        position.X * matrix.M11 + position.Y * matrix.M21 + position.Z * matrix.M31 + matrix.M41,
                        position.X * matrix.M12 + position.Y * matrix.M22 + position.Z * matrix.M32 + matrix.M42,
                        position.X * matrix.M13 + position.Y * matrix.M23 + position.Z * matrix.M33 + matrix.M43);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform(Vector3[] sourceArray, int sourceIndex, ref Matrix matrix,
            Vector3[] destinationArray, int destinationIndex, int length)
        {
            Debug.Assert(sourceArray.Length - sourceIndex >= length,
                "The source array is too small for the given sourceIndex and length.");
            Debug.Assert(destinationArray.Length - destinationIndex >= length,
                "The destination array is too small for the given destinationIndex and length.");

            // TODO: Are there options on some platforms to implement a vectorized version of this?

            for (var i = 0; i < length; i++)
            {
                var position = sourceArray[sourceIndex + i];
                destinationArray[destinationIndex + i] =
                    new Vector3(
                        position.X * matrix.M11 + position.Y * matrix.M21 + position.Z * matrix.M31 + matrix.M41,
                        position.X * matrix.M12 + position.Y * matrix.M22 + position.Z * matrix.M32 + matrix.M42,
                        position.X * matrix.M13 + position.Y * matrix.M23 + position.Z * matrix.M33 + matrix.M43);
            }
        }

        /// <summary>
        ///     Transforms a vector by a quaternion rotation.
        /// </summary>
        /// <param name="vec">The vector to transform.</param>
        /// <param name="quat">The quaternion to rotate the vector by.</param>
        /// <returns>The result of the operation.</returns>
        /// [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Transform(Vector3 vec, Quaternion quat)
        {
            Vector3 result;
            Transform(ref vec, ref quat, out result);
            return result;
        }

        /// <summary>
        ///     Transforms a vector by a quaternion rotation.
        /// </summary>
        /// <param name="value">The vector to transform.</param>
        /// <param name="rotation">The quaternion to rotate the vector by.</param>
        /// <param name="result">The result of the operation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform(ref Vector3 value, ref Quaternion rotation, out Vector3 result)
        {
            var x = 2 * (rotation.Y * value.Z - rotation.Z * value.Y);
            var y = 2 * (rotation.Z * value.X - rotation.X * value.Z);
            var z = 2 * (rotation.X * value.Y - rotation.Y * value.X);

            result.X = value.X + x * rotation.W + (rotation.Y * z - rotation.Z * y);
            result.Y = value.Y + y * rotation.W + (rotation.Z * x - rotation.X * z);
            result.Z = value.Z + z * rotation.W + (rotation.X * y - rotation.Y * x);
        }

        /// <summary>
        ///     Transforms an array of vectors by a quaternion rotation.
        /// </summary>
        /// <param name="sourceArray">The vectors to transform</param>
        /// <param name="rotation">The quaternion to rotate the vector by.</param>
        /// <param name="destinationArray">The result of the operation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform(Vector3[] sourceArray, ref Quaternion rotation, Vector3[] destinationArray)
        {
            Debug.Assert(destinationArray.Length >= sourceArray.Length,
                "The destination array is smaller than the source array.");

            // TODO: Are there options on some platforms to implement a vectorized version of this?

            for (var i = 0; i < sourceArray.Length; i++)
            {
                var position = sourceArray[i];

                var x = 2 * (rotation.Y * position.Z - rotation.Z * position.Y);
                var y = 2 * (rotation.Z * position.X - rotation.X * position.Z);
                var z = 2 * (rotation.X * position.Y - rotation.Y * position.X);

                destinationArray[i] =
                    new Vector3(
                        position.X + x * rotation.W + (rotation.Y * z - rotation.Z * y),
                        position.Y + y * rotation.W + (rotation.Z * x - rotation.X * z),
                        position.Z + z * rotation.W + (rotation.X * y - rotation.Y * x));
            }
        }

        /// <summary>
        ///     Transforms an array of vectors within a given range by a quaternion rotation.
        /// </summary>
        /// <param name="sourceArray">The vectors to transform.</param>
        /// <param name="sourceIndex">The starting index in the source array.</param>
        /// <param name="rotation">The quaternion to rotate the vector by.</param>
        /// <param name="destinationArray">The array to store the result of the operation.</param>
        /// <param name="destinationIndex">The starting index in the destination array.</param>
        /// <param name="length">The number of vectors to transform.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform(Vector3[] sourceArray, int sourceIndex, ref Quaternion rotation,
            Vector3[] destinationArray, int destinationIndex, int length)
        {
            Debug.Assert(sourceArray.Length - sourceIndex >= length,
                "The source array is too small for the given sourceIndex and length.");
            Debug.Assert(destinationArray.Length - destinationIndex >= length,
                "The destination array is too small for the given destinationIndex and length.");

            // TODO: Are there options on some platforms to implement a vectorized version of this?

            for (var i = 0; i < length; i++)
            {
                var position = sourceArray[sourceIndex + i];

                var x = 2 * (rotation.Y * position.Z - rotation.Z * position.Y);
                var y = 2 * (rotation.Z * position.X - rotation.X * position.Z);
                var z = 2 * (rotation.X * position.Y - rotation.Y * position.X);

                destinationArray[destinationIndex + i] =
                    new Vector3(
                        position.X + x * rotation.W + (rotation.Y * z - rotation.Z * y),
                        position.Y + y * rotation.W + (rotation.Z * x - rotation.X * z),
                        position.Z + z * rotation.W + (rotation.X * y - rotation.Y * x));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 TransformNormal(Vector3 normal, Matrix matrix)
        {
            TransformNormal(ref normal, ref matrix, out normal);
            return normal;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TransformNormal(ref Vector3 normal, ref Matrix matrix, out Vector3 result)
        {
            result = new Vector3(normal.X * matrix.M11 + normal.Y * matrix.M21 + normal.Z * matrix.M31,
                normal.X * matrix.M12 + normal.Y * matrix.M22 + normal.Z * matrix.M32,
                normal.X * matrix.M13 + normal.Y * matrix.M23 + normal.Z * matrix.M33);
        }

        #endregion Public methods


        #region Operators
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector3 value1, Vector3 value2)
        {
            return value1.X == value2.X
                   && value1.Y == value2.Y
                   && value1.Z == value2.Z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector3 value1, Vector3 value2)
        {
            return !(value1 == value2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator +(Vector3 value1, Vector3 value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            value1.Z += value2.Z;
            return value1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator -(Vector3 value)
        {
            value = new Vector3(-value.X, -value.Y, -value.Z);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator -(Vector3 value1, Vector3 value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            value1.Z -= value2.Z;
            return value1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(Vector3 value1, Vector3 value2)
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            value1.Z *= value2.Z;
            return value1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(Vector3 value, float scaleFactor)
        {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            value.Z *= scaleFactor;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(Vector3 value, double scaleFactor)
        {
            value.X *= (float)scaleFactor;
            value.Y *= (float)scaleFactor;
            value.Z *= (float)scaleFactor;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(float scaleFactor, Vector3 value)
        {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            value.Z *= scaleFactor;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(double scaleFactor, Vector3 value)
        {
            value.X *= (float)scaleFactor;
            value.Y *= (float)scaleFactor;
            value.Z *= (float)scaleFactor;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(Vector3 value1, Matrix value2)
        {
            var x = value1.X * value2.M11 + value1.Y * value2.M21 + value1.Z * value2.M31;
            var y = value1.X * value2.M12 + value1.Y * value2.M22 + value1.Z * value2.M32;
            var z = value1.X * value2.M13 + value1.Y * value2.M23 + value1.Z * value2.M33;
            return new Vector3(x, y, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator /(Vector3 value1, Vector3 value2)
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            value1.Z /= value2.Z;
            return value1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator /(Vector3 value, float divider)
        {
            var factor = 1.0f / divider;
            value.X *= factor;
            value.Y *= factor;
            value.Z *= factor;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator /(Vector3 value, double divider)
        {
            var factor = 1.0f / (float)divider;
            value.X *= factor;
            value.Y *= factor;
            value.Z *= factor;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator System.Numerics.Vector3(Vector3 value)
        {
            return new System.Numerics.Vector3(value.X, value.Y, value.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector3(System.Numerics.Vector3 value)
        {
            return new Vector3(value.X, value.Y, value.Z);
        }

        #endregion
    }
}