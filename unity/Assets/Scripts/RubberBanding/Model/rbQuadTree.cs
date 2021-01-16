﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RubberBanding {

    public class RbQuadTree : I2DRangeQuery<rbPoint> {

        private readonly QNode Root;

        public RbQuadTree(List<rbPoint> points)
        {
            // Create bounding box of all points
            var boundingRectangle = AARect.GetBoundingRectangle(points);
            boundingRectangle.Bottom += 0.000001f;
            boundingRectangle.Right += 0.000001f;

            // Build QuadTree
            Root = Build(null, points, boundingRectangle);
        }

        private QNode Build(QNode parent, List<rbPoint> points, AARect rectangle)
        {
            QNode n = new QNode(parent, points.Count, rectangle);

            // Edge case: only one point has to fit in this box
            if (points.Count == 1)
            {
                n.point = points[0]; // Only leafs are assigned a point.
            }
            else
            {
                // Split bounding rectangle into four pieces
                List<AARect> boundingRects = rectangle.Split();

                // Recursively build tree. Note: depth first
                for (int i = 0; i < boundingRects.Count; i++)
                {
                    var foundPoints = points.FindAll(p => boundingRects[i].SplitContains(p));
                    if (foundPoints.Count > 0)
                    {
                        n.children.Add(Build(n, foundPoints, boundingRects[i]));
                    }
                }
            }

            return n;
        }

        override
        public void RemovePoint(rbPoint p)
        {
            // First, seach in which node this p belongs
            QNode n = Root;
            while (n.point == null)
            {
                for (int i = 0; i < n.children.Count; i++)
                {
                    QNode child = n.children[i];
                    if (child.boundingRect.Contains(p))
                    {
                        n = child;
                        break;
                    }
                }
            }

            // Next, remove node n and shrink tree appropriately.
            if (n.point == p)
            {
                n.point = null;

                // Edge case: the point was part of the root 
                // i.e. tree is empty now
                if (n == Root)
                {
                    n.size = 0;
                    return;
                }

                // Walk up in tree until you find a split in the tree
                while (n.parent.size == 1)
                {
                    n = n.parent;
                    if (n == Root) break;
                }
                n.children = null;
                while (n != null)
                {
                    n.size -= 1;
                    n = n.parent;
                }
            }
        }

        /*
         * Recursively retrieve all points from tree that lie inside BoundingRect bound.
         * Note: this action does not remove the found points.
         */
        override
        public List<rbPoint> FindInRange(AARect bound)
        {
            return FindInRange(Root, bound);
        }

        private List<rbPoint> FindInRange(QNode n, AARect bound)
        {
            List<rbPoint> points = new List<rbPoint>();
            if (n.children.Count == 0)
            {
                // Leaf node
                if (bound.Contains(n.point))
                {
                    points.Add(n.point);
                }                
            }
            else
            {
                // Internal node in tree
                foreach (QNode child in n.children)
                {
                    if (child.boundingRect.Intersects(bound)) {
                        points.AddRange(FindInRange(child, bound));
                    }                    
                }
            }

            return points;
        }

        /// <summary>
        /// Node used in the creation of rbQuadTree.
        /// </summary>
        public class QNode
        {
            public QNode parent;
            public int size;
            public AARect boundingRect;
            public List<QNode> children;
            public rbPoint point;

            public QNode(QNode parent, int size, AARect boundingRect)
            {
                this.parent = parent;
                this.boundingRect = boundingRect;
                this.size = size;
                children = new List<QNode>();                
                point = null;
            }
        }
    }
}



