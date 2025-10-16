using System;
using System.Collections.Generic;
using Red.Common;
using Red.Math3D;

namespace Red.Entities
{
    public class SceneNode : IDisposable
    {
        public ulong ID { get; internal set; }
        public BoundingBox Bounds { get; protected set; }
        public SceneNode Parent { get; private set; }
        public List<SceneNode> Children { get; private set; } = new List<SceneNode>();


        // used for BVH splitting and calculation.
        private SceneNode Left { get; set; }
        private SceneNode Right { get; set; }

        public SceneNode() : this(new BoundingBox())
        {
        }

        public SceneNode(BoundingBox bounds)
        {
            Bounds = bounds;
        }

        private SceneNode(List<SceneNode> children)
        {
            Children = children;
            Bounds = new BoundingBox();
            Bounds = CalculateBounds();

            if (Children.Count > 1)
            {
                Split();
            }
        }

        public void Dispose()
        {
            Left.Dispose();
            Right.Dispose();
            foreach (var child in Children)
            {
                child.Dispose();
            }
        }
        public void AddChild(SceneNode node, bool splitBVH = false)
        {
            Children.Add(node);
            node.Parent = this;
            if (Children.Count > 1 && splitBVH)
            {
                Split();
            }
        }

        public void RemoveChild(SceneNode node)
        {
            Children.Remove(node);
        }

        public bool Contains(SceneNode node, bool recursive = false)
        {
            if (Children.Contains(node))
            {
                return true;
            }

            if (recursive)
            {
                foreach (var child in Children)
                {
                    if (Contains(node, recursive))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public BoundingBox CalculateBounds()
        {
            if (Children.Count == 0)
            {
                return Bounds;
            }

            var min = Bounds.Min;
            var max = Bounds.Max;

            for (int i = 1; i < Children.Count; i++)
            {
                var box = Children[i].CalculateBounds();
                min = Vector3.Min(min, box.Min);
                max = Vector3.Max(max, box.Max);
            }

            return new BoundingBox(min, max);
        }
        private void Split()
        {
            // Find the longest axis to split along
            int longestAxis = Bounds.GetLongestAxis();

            // Sort the boxes based on their center along the longest axis
            Children.Sort((a, b) => a.Bounds.Center[longestAxis].CompareTo(b.Bounds.Center[longestAxis]));

            // Divide the boxes into two subsets
            int mid = Children.Count / 2;
            List<SceneNode> leftBoxes = Children.GetRange(0, mid);
            List<SceneNode> rightBoxes = Children.GetRange(mid, Children.Count - mid);

            // Create child nodes for the left and right subsets
            Left = new SceneNode(leftBoxes);
            Right = new SceneNode(rightBoxes);
        }
        public bool Intersect(Ray ray, out BoundingBox hitBox)
        {
            // Start traversal from the root node
            return Traverse(this, ray, out hitBox);
        }

        private bool Traverse(SceneNode node, Ray ray, out BoundingBox hitBox)
        {
            hitBox = new BoundingBox();

            // Check if the ray intersects the node's bounding box
            if (!IntersectBox(node.Bounds, ray))
            {
                return false;
            }

            if (node.Children != null)
            {
                // This is a leaf node, so check for intersections with the objects inside
                foreach (var child in node.Children)
                {
                    // Implement ray-box intersection testing for each object
                    if (IntersectBox(child.Bounds, ray))
                    {
                        hitBox = child.Bounds;
                        return true;
                    }
                }

                return false;
            }

            // This is an internal node, so continue the traversal
            BoundingBox leftHit, rightHit;
            bool hitLeft = Traverse(node.Left, ray, out leftHit);
            bool hitRight = Traverse(node.Right, ray, out rightHit);

            if (hitLeft && hitRight)
            {
                // If the ray hits both subtrees, return the nearest hit
                if (Vector3.Distance(ray.Position, leftHit.Center) < Vector3.Distance(ray.Position, rightHit.Center))
                {
                    hitBox = leftHit;
                }
                else
                {
                    hitBox = rightHit;
                }

                return true;
            }
            else if (hitLeft)
            {
                hitBox = leftHit;
                return true;
            }
            else if (hitRight)
            {
                hitBox = rightHit;
                return true;
            }

            return false;
        }
        private bool IntersectBox(BoundingBox box, Ray ray)
        {
            float tNear = float.MinValue; // The intersection point entering the box
            float tFar = float.MaxValue;  // The intersection point exiting the box

            // Check each dimension (X, Y, Z) of the box
            for (int i = 0; i < 3; i++)
            {
                // Ray's origin and direction components
                float origin = ray.Position[i];
                float direction = ray.Direction[i];

                // Check if the ray is parallel to the box's plane
                if (Math.Abs(direction) < float.Epsilon)
                {
                    // Ray is parallel, and if outside the box, it won't hit
                    if (origin < box.Min[i] || origin > box.Max[i])
                    {
                        return false;
                    }
                }
                else
                {
                    // Calculate the intersection distances for entering and exiting the box
                    float t1 = (box.Min[i] - origin) / direction;
                    float t2 = (box.Max[i] - origin) / direction;

                    // Ensure t1 is the intersection with the near plane, t2 with the far plane
                    if (t1 > t2)
                    {
                        float temp = t1;
                        t1 = t2;
                        t2 = temp;
                    }

                    // Update tNear and tFar
                    tNear = Math.Max(tNear, t1);
                    tFar = Math.Min(tFar, t2);

                    // Check if the ray misses the box
                    if (tNear > tFar || tFar < 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}