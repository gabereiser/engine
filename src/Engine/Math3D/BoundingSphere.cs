﻿// Author:
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Red.Math3D
{
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    [StructLayout(LayoutKind.Sequential)]
    public struct BoundingSphere : IEquatable<BoundingSphere>
    {
        #region Public Fields

        public Vector3 Center;


        public float Radius;

        #endregion Public Fields


        #region Constructors

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BoundingSphere(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        #endregion Constructors


        #region Public Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BoundingSphere Transform(Matrix matrix)
        {
            var sphere = new BoundingSphere();
            sphere.Center = Vector3.Transform(Center, matrix);
            sphere.Radius = Radius * (float)System.Math.Sqrt(System.Math.Max(
                matrix.M11 * matrix.M11 + matrix.M12 * matrix.M12 + matrix.M13 * matrix.M13,
                System.Math.Max(matrix.M21 * matrix.M21 + matrix.M22 * matrix.M22 + matrix.M23 * matrix.M23,
                    matrix.M31 * matrix.M31 + matrix.M32 * matrix.M32 + matrix.M33 * matrix.M33)));
            return sphere;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Transform(ref Matrix matrix, out BoundingSphere result)
        {
            result.Center = Vector3.Transform(Center, matrix);
            result.Radius = Radius * (float)System.Math.Sqrt(System.Math.Max(
                matrix.M11 * matrix.M11 + matrix.M12 * matrix.M12 + matrix.M13 * matrix.M13,
                System.Math.Max(matrix.M21 * matrix.M21 + matrix.M22 * matrix.M22 + matrix.M23 * matrix.M23,
                    matrix.M31 * matrix.M31 + matrix.M32 * matrix.M32 + matrix.M33 * matrix.M33)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ContainmentType Contains(BoundingBox box)
        {
            //check if all corner is in sphere
            var inside = true;
            foreach (var corner in box.GetCorners())
                if (Contains(corner) == ContainmentType.Disjoint)
                {
                    inside = false;
                    break;
                }

            if (inside)
                return ContainmentType.Contains;

            //check if the distance from sphere center to cube face < radius
            double dmin = 0;

            if (Center.X < box.Min.X)
                dmin += (Center.X - box.Min.X) * (Center.X - box.Min.X);

            else if (Center.X > box.Max.X)
                dmin += (Center.X - box.Max.X) * (Center.X - box.Max.X);

            if (Center.Y < box.Min.Y)
                dmin += (Center.Y - box.Min.Y) * (Center.Y - box.Min.Y);

            else if (Center.Y > box.Max.Y)
                dmin += (Center.Y - box.Max.Y) * (Center.Y - box.Max.Y);

            if (Center.Z < box.Min.Z)
                dmin += (Center.Z - box.Min.Z) * (Center.Z - box.Min.Z);

            else if (Center.Z > box.Max.Z)
                dmin += (Center.Z - box.Max.Z) * (Center.Z - box.Max.Z);

            if (dmin <= Radius * Radius)
                return ContainmentType.Intersects;

            //else disjoint
            return ContainmentType.Disjoint;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Contains(ref BoundingBox box, out ContainmentType result)
        {
            result = Contains(box);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ContainmentType Contains(BoundingFrustum frustum)
        {
            //check if all corner is in sphere
            var inside = true;

            var corners = frustum.GetCorners();
            foreach (var corner in corners)
                if (Contains(corner) == ContainmentType.Disjoint)
                {
                    inside = false;
                    break;
                }

            if (inside)
                return ContainmentType.Contains;

            //check if the distance from sphere center to frustrum face < radius
            double dmin = 0;
            //TODO : calcul dmin

            if (dmin <= Radius * Radius)
                return ContainmentType.Intersects;

            //else disjoint
            return ContainmentType.Disjoint;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ContainmentType Contains(BoundingSphere sphere)
        {
            var val = MathHelper.Distance(sphere.Center, Center);

            if (val > sphere.Radius + Radius)
                return ContainmentType.Disjoint;

            if (val <= Radius - sphere.Radius)
                return ContainmentType.Contains;

            return ContainmentType.Intersects;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Contains(ref BoundingSphere sphere, out ContainmentType result)
        {
            result = Contains(sphere);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ContainmentType Contains(Vector3 point)
        {
            var distance = MathHelper.Distance(point, Center);

            if (distance > Radius)
                return ContainmentType.Disjoint;

            if (distance < Radius)
                return ContainmentType.Contains;

            return ContainmentType.Intersects;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Contains(ref Vector3 point, out ContainmentType result)
        {
            result = Contains(point);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoundingSphere CreateFromBoundingBox(BoundingBox box)
        {
            // Find the center of the box.
            var center = new Vector3((box.Min.X + box.Max.X) / 2.0f,
                (box.Min.Y + box.Max.Y) / 2.0f,
                (box.Min.Z + box.Max.Z) / 2.0f);

            // Find the distance between the center and one of the corners of the box.
            var radius = MathHelper.Distance(center, box.Max);

            return new BoundingSphere(center, radius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateFromBoundingBox(ref BoundingBox box, out BoundingSphere result)
        {
            result = CreateFromBoundingBox(box);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoundingSphere CreateFromFrustum(BoundingFrustum frustum)
        {
            return CreateFromPoints(frustum.GetCorners());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoundingSphere CreateFromPoints(IEnumerable<Vector3> points)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            // From "Real-Time Collision Detection" (Page 89)

            var minx = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var maxx = -minx;
            var miny = minx;
            var maxy = -minx;
            var minz = minx;
            var maxz = -minx;

            // Find the most extreme points along the principle axis.
            var numPoints = 0;
            foreach (var pt in points)
            {
                ++numPoints;

                if (pt.X < minx.X)
                    minx = pt;
                if (pt.X > maxx.X)
                    maxx = pt;
                if (pt.Y < miny.Y)
                    miny = pt;
                if (pt.Y > maxy.Y)
                    maxy = pt;
                if (pt.Z < minz.Z)
                    minz = pt;
                if (pt.Z > maxz.Z)
                    maxz = pt;
            }

            if (numPoints == 0)
                throw new ArgumentException("You should have at least one point in points.");

            var sqDistX = MathHelper.DistanceSquared(maxx, minx);
            var sqDistY = MathHelper.DistanceSquared(maxy, miny);
            var sqDistZ = MathHelper.DistanceSquared(maxz, minz);

            // Pick the pair of most distant points.
            var min = minx;
            var max = maxx;
            if (sqDistY > sqDistX && sqDistY > sqDistZ)
            {
                max = maxy;
                min = miny;
            }

            if (sqDistZ > sqDistX && sqDistZ > sqDistY)
            {
                max = maxz;
                min = minz;
            }

            var center = (min + max) * 0.5f;
            var radius = MathHelper.Distance(max, center);
            if (float.IsInfinity(radius))
                radius = 0;
            return new BoundingSphere(center, radius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoundingSphere CreateMerged(BoundingSphere original, BoundingSphere additional)
        {
            var ocenterToaCenter = Vector3.Subtract(additional.Center, original.Center);
            var distance = ocenterToaCenter.Length();
            if (distance <= original.Radius + additional.Radius) //intersect
            {
                if (distance <= original.Radius - additional.Radius) //original contain additional
                    return original;
                if (distance <= additional.Radius - original.Radius) //additional contain original
                    return additional;
            }

            //else find center of new sphere and radius
            var leftRadius = System.Math.Max(original.Radius - distance, additional.Radius);
            var Rightradius = System.Math.Max(original.Radius + distance, additional.Radius);
            ocenterToaCenter = ocenterToaCenter +
                               (leftRadius - Rightradius) / (2 * ocenterToaCenter.Length()) *
                               ocenterToaCenter; //oCenterToResultCenter

            var result = new BoundingSphere();
            result.Center = original.Center + ocenterToaCenter;
            result.Radius = (leftRadius + Rightradius) / 2;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateMerged(ref BoundingSphere original, ref BoundingSphere additional,
            out BoundingSphere result)
        {
            result = CreateMerged(original, additional);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(BoundingSphere other)
        {
            return Center == other.Center && Radius == other.Radius;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (obj is BoundingSphere)
                return Equals((BoundingSphere)obj);

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return Center.GetHashCode() + Radius.GetHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects(BoundingBox box)
        {
            return box.Intersects(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Intersects(ref BoundingBox box, out bool result)
        {
            result = Intersects(box);
        }

        /*
        public bool Intersects(BoundingFrustum frustum)
        {
            if (frustum == null)
                throw new NullReferenceException();

            throw new NotImplementedException();
        }
        */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects(BoundingSphere sphere)
        {
            var val = MathHelper.Distance(sphere.Center, Center);
            if (val > sphere.Radius + Radius)
                return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Intersects(ref BoundingSphere sphere, out bool result)
        {
            result = Intersects(sphere);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PlaneIntersectionType Intersects(Plane plane)
        {
            var result = default(PlaneIntersectionType);
            // TODO: we might want to inline this for performance reasons
            Intersects(ref plane, out result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Intersects(ref Plane plane, out PlaneIntersectionType result)
        {
            var distance = default(float);
            // TODO: we might want to inline this for performance reasons
            Vector3.Dot(ref plane.Normal, ref Center, out distance);
            distance += plane.D;
            if (distance > Radius)
                result = PlaneIntersectionType.Front;
            else if (distance < -Radius)
                result = PlaneIntersectionType.Back;
            else
                result = PlaneIntersectionType.Intersecting;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float? Intersects(Ray ray)
        {
            return ray.Intersects(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Intersects(ref Ray ray, out float? result)
        {
            result = Intersects(ray);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(BoundingSphere a, BoundingSphere b)
        {
            return a.Equals(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(BoundingSphere a, BoundingSphere b)
        {
            return !a.Equals(b);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return "{{Center:" + Center + " Radius:" + Radius + "}}";
        }

        #endregion Public Methods
    }
}