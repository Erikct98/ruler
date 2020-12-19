﻿namespace RubberBanding
{
    using System.Collections.Generic;
    using System.Linq;

    public class RbBST
    {
        private BstNode Root { get; set; }

        /// <param name="points">List of all rbPoints that should be put in the tree. 
        /// It is assumed that these are sorted on y-coordinate.</param>
        public RbBST(List<rbPoint> points)
        {
            Root = Build(points);
        }

        /// <summary>
        /// Build the Binary Search Tree
        /// </summary>
        private BstNode Build(IEnumerable<rbPoint> points)
        {
            int count = points.Count();
            if (count == 0) return null;

            BstNode root = new BstNode();
            int medianIdx = count / 2;
            root.Point = points.ElementAt(medianIdx);

            root.Left = Build(points.Take(medianIdx));
            root.Right = Build(points.Skip(medianIdx + 1));

            return root;
        }

        public List<rbPoint> FindInRange(float leftBound, float rightBound)
        {
            List<rbPoint> points = new List<rbPoint>();

            // Look for split node
            BstNode split = Root;
            while (split != null)
            {
                float y = split.Point.Pos.y;
                if (rightBound < y)
                {
                    split = split.Left;
                }
                else if (y < leftBound)
                {
                    split = split.Right;
                }
            }

            // Search left subtree
            BstNode elt = split;
            while (elt != null)
            {
                float y = elt.Point.Pos.y;
                if (leftBound <= y)
                {
                    points.Add(elt.Point);
                    if (elt.Right != null)
                    {
                        points.AddRange(elt.Right.AllElements());
                    }
                    elt = elt.Left;
                }
                else
                {
                    elt = elt.Right;
                }
            }


            // Search right subtree
            elt = split;
            while (elt != null)
            {
                float y = elt.Point.Pos.y;
                if (y <= rightBound)
                {
                    points.Add(elt.Point);
                    if (elt.Left != null)
                    {
                        points.AddRange(elt.Left.AllElements());
                    }
                    elt = elt.Right;
                }
                else
                {
                    elt = elt.Left;
                }
            }

            // Filter out all points that have already been removed
            return points.FindAll(e => !e.Removed);
        }

        /// <summary>
        /// Node used in the creation of RbBST.
        /// </summary>
        private class BstNode
        {
            public rbPoint Point { get; set; }
            public BstNode Left { get; set; }
            public BstNode Right { get; set; }

            /// <summary>
            /// Returns all elements contained in this subtree.
            /// </summary>
            public IEnumerable<rbPoint> AllElements()
            {
                List<rbPoint> elements = new List<rbPoint>();

                if (Point != null)
                {
                    elements.Add(Point);
                }
                if (Left != null)
                {
                    elements.AddRange(Left.AllElements());
                }
                if (Right != null)
                {
                    elements.AddRange(Right.AllElements());
                }
                return elements;
            }
        }
    }
}
