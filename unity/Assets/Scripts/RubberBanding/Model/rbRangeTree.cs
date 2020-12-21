namespace RubberBanding
{
    using System.Collections.Generic;
    using System.Linq;

    public class RbRangeTree : I2DRangeQuery<rbPoint>
    {
        private readonly RTNode Root;

        public RbRangeTree(List<rbPoint> points)
        {
            // Presort list of points on x-coordinate
            List<rbPoint> xSorted = new List<rbPoint>(points);
            xSorted.Sort(delegate (rbPoint p1, rbPoint p2) { return p1.Pos.x.CompareTo(p2.Pos.x); });

            // Presort list of points on y-coordinate
            List<rbPoint> ySorted = points;
            ySorted.Sort(delegate (rbPoint p1, rbPoint p2) { return p1.Pos.y.CompareTo(p2.Pos.y); });

            // Build tree
            Root = Build(xSorted, ySorted);
        }

        /// <summary>
        /// Constructs a balanced 2D range tree containing the specified set of points.
        /// </summary>
        /// <param name="xSorted">All points that need to be inserted, sorted on x-coordinate</param>
        /// <param name="ySorted">All points that need to be inserted, sorted on y-coordinate</param>
        /// <returns>The root of the 2D range tree</returns>
        private RTNode Build(List<rbPoint> xSorted, List<rbPoint> ySorted)
        {
            // Create node
            RTNode v = new RTNode();

            // Create ADS
            v.Ads = new RbBST(ySorted);

            if (ySorted.Count == 1)
            {
                // Leaf node
                v.Point = ySorted[0];
            }
            else
            {
                // Determine x_mid
                int medIdx = xSorted.Capacity / 2;
                rbPoint x_mid = xSorted[medIdx];

                // Store point in this node
                v.Point = x_mid;

                // Recursive build left subtree
                List<rbPoint> xSortedLeft = new List<rbPoint>(xSorted.Take(medIdx)); // Observe: excludes median
                ySorted.Remove(x_mid);
                List<rbPoint> ySortedLeft = ySorted.FindAll(p => p.Pos.x <= x_mid.Pos.x); // WARNING: it is assumed this function preserves order.
                v.Left = Build(xSortedLeft, ySortedLeft);

                // Recursively build right subtree
                List<rbPoint> xSortedRight = new List<rbPoint>(xSorted.Skip(medIdx + 1)); // Observe: skip the median
                List<rbPoint> ySortedRight = ySorted.FindAll(p => p.Pos.x > x_mid.Pos.x);
                v.Right = Build(xSortedRight, ySortedRight);
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
            else if (v_split.Left == null || v_split.Right == null)
            {
                // v_split is a leaf node
                float x = v_split.Point.Pos.x;
                if (leftBound <= x && x <= rightBound)
                {
                    points.Add(v_split.Point);
                }                
            }
            else
            {
                // Walk through left subtree
                RTNode v = v_split.Left;
                while (v != null && (v.Left != null || v.Right != null))
                {
                    // v is not a leaf
                    float x = v.Point.Pos.x;
                    if (leftBound <= x)
                    {
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

                // Check point at end

                // Walk through right subtree
                v = v_split.Right;
                while (v != null && (v.Left != null || v.Right != null))
                {
                    // v is not a leaf
                    float x = v.Point.Pos.x;
                    if (x <= rightBound)
                    {
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