using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RubberBanding {

    public class RbQuadTree : I2DRangeQuery<rbPoint> {

        private QNode Root;

        public RbQuadTree(List<rbPoint> points)
        {
            // Create bounding box of all points
            float left = points.Min(e => e.Pos.x);
            float right = points.Max(e => e.Pos.x) + 0.00001f;
            float top = points.Max(e => e.Pos.y) + 0.00001f;
            float bottom = points.Min(e => e.Pos.y);
            AARect rectangle = new AARect(top, right, bottom, left);

            // Build QuadTree
            this.Root = Build(null, points, rectangle);
        }

        private QNode Build(QNode parent, List<rbPoint> points, AARect rectangle)
        {
            QNode n = new QNode(parent, points.Count, rectangle);

            // Edge case: only one point has to fit in this box
            if (points.Count == 1)
            {
                n.point = points[0];
                return n;
            }

            // Split bounding rectangle into four pieces
            List<AARect> boundingRects = rectangle.Split();

            // Recursively build tree. Note: depth first
            for (int i = 0; i < boundingRects.Count; i++)
            {
                var foundPoints = points.FindAll(p => boundingRects[i].Contains(p));
                if (foundPoints.Count > 1) {
                    n.children.Add(Build(n, foundPoints, boundingRects[i]));
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
                for (int i = 0; i < n.children.Capacity; i++)
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
            foreach (QNode child in n.children)
            {
                if (child.boundingRect.Overlaps(bound))
                {
                    if (child.point != null)
                    {
                        points.Add(child.point);
                    }
                    else
                    {
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
                children = new List<QNode>();
                point = null;
            }
        }
    }
}



