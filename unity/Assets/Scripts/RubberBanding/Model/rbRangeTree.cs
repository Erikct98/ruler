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
            if (xSorted.Capacity == 0) return null;

            // Build BST on y-coordinates of points
            RTNode v = new RTNode();
            v.Ads = new RbBST(ySorted);

            // Determine x-median point in the set
            int medIdx = xSorted.Capacity / 2;
            rbPoint med = xSorted[medIdx];
            v.Point = med;

            // Split xSorted on median
            List<rbPoint> xSortedLeft = new List<rbPoint>(xSorted.Take(medIdx));
            List<rbPoint> xSortedRight = new List<rbPoint>(xSorted.Skip(medIdx + 1)); // Skipping the median

            // Split ySorted on median
            ySorted.Remove(med);
            List<rbPoint> ySortedLeft = ySorted.FindAll(p => p.Pos.x <= med.Pos.x); // WARNING: it is assumed this function preserves order.
            List<rbPoint> ySortedRight = ySorted.FindAll(p => p.Pos.x > med.Pos.x);

            // Recurse
            v.Left = Build(xSortedLeft, ySortedLeft);
            v.Right = Build(xSortedRight, ySortedRight);

            return v;
        }

        /// <summary>
        /// Finds all rbPoints this tree contains in specified query range.
        /// </summary>
        /// <param name="topLeft">Top left corner of axis-aligned query rectangle</param>
        /// <param name="bottomRight">Bottom right corner of axis-aligned query rectangle</param>
        /// <returns>All points in this tree that lie inside indicated axis-aligned query rectangle</returns>
        override
        public List<rbPoint> FindInRange(AARect range)
        {
            List<rbPoint> points = new List<rbPoint>();

            float leftBound = range.Left;
            float rightBound = range.Right;

            // Find split node
            RTNode split = Root;
            while (split != null)
            {
                float x = split.Point.Pos.x;
                if (x < leftBound)
                {
                    split = split.Right;
                }
                else if (rightBound < x)
                {
                    split = split.Left;
                }
                else break;
            }
            if (split != null && split.Point != null)
            {
                points.Add(split.Point);
            }

            // Walk through left subtree
            RTNode elt = split;
            while (elt != null)
            {
                float x = elt.Point.Pos.x;
                if (leftBound <= x)
                {
                    points.Add(elt.Point);
                    if (elt.Right != null)
                    {
                        points.AddRange(elt.Right.Ads.FindInRange(range.Bottom, range.Top));
                    }
                    elt = elt.Left;
                }
                else
                {
                    elt = elt.Right;
                }
            }

            // Walk through right subtree
            elt = split;
            while (elt != null)
            {
                float x = elt.Point.Pos.x;
                if (x <= rightBound)
                {
                    points.Add(elt.Point);
                    if (elt.Left != null)
                    {
                        points.AddRange(elt.Left.Ads.FindInRange(range.Bottom, range.Top));
                    }
                    elt = elt.Right;
                }
                else
                {
                    elt = elt.Left;
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