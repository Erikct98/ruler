namespace RubberBanding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RbRangeTree : I2DRangeQuery<rbPoint>
    {
        private readonly RTNode Root;

        public RbRangeTree(List<rbPoint> points)
        {
            // Presort list of points on x-coordinate
            points.Sort(delegate (rbPoint p1, rbPoint p2) { return p1.Pos.x.CompareTo(p2.Pos.x); });

            // Build tree
            Root = Build(points);
        }

        /// <summary>
        /// Constructs a balanced 2D range tree containing the specified set of points.
        /// </summary>
        /// <param name="xSorted">All points that need to be inserted, sorted on x-coordinate</param>
        /// <param name="ySorted">All points that need to be inserted, sorted on y-coordinate</param>
        /// <returns>The root of the 2D range tree</returns>
        private RTNode Build(List<rbPoint> points)
        {
            if (points.Count == 0)
            {
                return null;
            }

            // Create node
            RTNode v = new RTNode();

            // Create ADS
            List<rbPoint> ySorted = new List<rbPoint>(points);
            ySorted.Sort(delegate (rbPoint p1, rbPoint p2) { return p1.Pos.y.CompareTo(p2.Pos.y); });
            v.Ads = new RbBST(ySorted);

            if (points.Count == 1)
            {
                // Leaf node
                v.Point = points[0];
            }
            else
            {
                // Determine x_mid
                int medIdx = points.Count / 2;
                rbPoint x_mid = points[medIdx];

                // Store point in this node
                v.Point = x_mid;

                // Recursive build left subtree
                List<rbPoint> pointsLeft = new List<rbPoint>(points.Take(medIdx)); // Observe: excludes median
                List<rbPoint> pointsRight = new List<rbPoint>(points.Skip(medIdx + 1)); // Observe: excludes median
                v.Left = Build(pointsLeft);
                v.Right = Build(pointsRight);
            }

            return v;
        }

        /// <summary>
        /// Finds all rbPoints this tree contains in specified query range.
        /// </summary>
        /// <param name="topLeft">Top left corner of axis-aligned query rectangle</param>
        /// <param name="bottomRight">Bottom right corner of axis-aligned query rectangle</param>
        /// <returns>All points in this tree that lie inside indicated axis-aligned query rectangle</returns>
        override
        public List<rbPoint> FindInRange(AARect range2D)
        {
            List<rbPoint> points = new List<rbPoint>();

            float leftBound = range2D.Left;
            float rightBound = range2D.Right;

            // Find split node
            RTNode v_split = Root;
            while (v_split != null)
            {
                float x = v_split.Point.Pos.x;
                if (x < leftBound)
                {
                    v_split = v_split.Right;
                }
                else if (rightBound < x)
                {
                    v_split = v_split.Left;
                }
                else break;
            }

            if (v_split == null)
            {
                return points; // Returning and empty list.
            }
            else
            {
                if (range2D.Contains(v_split.Point))
                {
                    points.Add(v_split.Point);
                }                
            }

            // Walk through left subtree
            RTNode v = v_split.Left;
            while (v != null)
            {
                float x = v.Point.Pos.x;
                if (leftBound <= x)
                {
                    if (range2D.Contains(v.Point))
                    {
                        points.Add(v.Point);
                    }
                    if (v.Right != null)
                    {
                        points.AddRange(v.Right.Ads.FindInRange(range2D.Bottom, range2D.Top));
                    }                        
                    v = v.Left;
                }
                else
                {
                    v = v.Right;
                }
            }

            // Walk through right subtree
            v = v_split.Right;
            while (v != null && (v.Left != null || v.Right != null))
            {
                // v is not a leaf
                float x = v.Point.Pos.x;
                if (x <= rightBound)
                {
                    if (range2D.Contains(v.Point))
                    {
                        points.Add(v.Point);
                    }
                    if (v.Left != null)
                    {
                        points.AddRange(v.Left.Ads.FindInRange(range2D.Bottom, range2D.Top));
                    }                        
                    v = v.Right;
                }
                else
                {
                    v = v.Left;
                }
            }

            return points;
        }

        override
        public void RemovePoint(rbPoint point)
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// Node used in the creation of rbRangeTree
    /// </summary>
    public class RTNode
    {
        public RTNode Left { get; set; }
        public RTNode Right { get; set; }
        public rbPoint Point { get; set; }
        public RbBST Ads { get; set; }
    }
}