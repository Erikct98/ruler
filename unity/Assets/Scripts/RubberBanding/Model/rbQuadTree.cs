using System.Collections.Generic;
using UnityEngine;

public class rbQuadTree{

    private Node root;

    public rbQuadTree(List<rbPoint> points)
    {
        // Create bounding box of all points
        float left = points[0].Pos.x;
        float right = points[0].Pos.x;
        float top = points[0].Pos.y;
        float bottom = points[0].Pos.y;
        foreach (rbPoint p in points)
        {
            left = Mathf.Min(p.Pos.x, left);
            right = Mathf.Max(p.Pos.x, right); // TODO: + eps
            bottom = Mathf.Min(p.Pos.y, bottom); // TODO: + eps
            top = Mathf.Max(p.Pos.y, top);
        }
        BoundingRect bound = new BoundingRect(top, right, bottom, left);

        // Build QuadTree
        this.root = BuildQuadTree(null, points, bound);
    }
	
    private Node BuildQuadTree(Node parent, List<rbPoint> points, BoundingRect bound)
    {
        Node n = new Node(parent, points.Capacity, bound);
        
        // Edge case: only one point has to fit in this box
        if (points.Capacity == 1)
        {
            n.point = points[0];
            return n;
        }

        // Split bounding rectangle into four pieces
        List<BoundingRect> boundingRects = bound.getSplit();

        // Split points among rectangle split
        List<List<rbPoint>> pointSplit = new List<List<rbPoint>>();
        for (int i = 0; i < points.Capacity; i++)
        {
            rbPoint p = points[i];
            for (int j = 0; j < boundingRects.Capacity; j++)
            {
                if (boundingRects[i].contains(p))
                {
                    pointSplit[j].Add(p);
                    break;
                }
            }
        }

        // Recursively build tree. Note: depth first
        for (int i = 0; i < boundingRects.Capacity; i++)
        {
            n.children.Add(BuildQuadTree(n, pointSplit[i], boundingRects[i]));
        }
        
        return n;
    }

    public void RemovePoint(rbPoint p)
    {
        // First, seach in which node this p belongs
        Node n = root;
        while (n.point == null)
        {
            for (int i = 0; i < n.children.Capacity; i++)
            {
                Node child = n.children[i];
                if (child.boundingRect.contains(p))
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
            if (n == root)
            {
                n.size = 0;
                return;
            }

            // Walk up in tree until you find a split in the tree
            while (n.parent.size == 1)
            {
                n = n.parent;
                if (n == root) break;
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
    public List<rbPoint> getPointsInRectangle(BoundingRect bound)
    {
        return getPointsInRectangle(root, bound);
    }

    private List<rbPoint> getPointsInRectangle(Node n, BoundingRect bound)
    {
        List<rbPoint> points = new List<rbPoint>();
        foreach (Node child in n.children)
        {
            if (child.boundingRect.overlaps(bound))
            {
                if (child.point != null)
                {
                    points.Add(child.point);
                }
                else
                {
                    points.AddRange(getPointsInRectangle(child, bound));
                }                    
            }
        }
        return points;
    }    
}

/*
 * Axis aligned rectangle used in the construction
 * and utilization of Quadtree
 */
public class BoundingRect
{
    private Vector2 topLeft;
    private Vector2 bottomRight;
    
    public BoundingRect(rbPoint tl, rbPoint br)
    {
        topLeft = tl.Pos;
        bottomRight = br.Pos;
    }

    public BoundingRect(Vector2 tl, Vector2 br)
    {
        topLeft = new Vector2(tl.x, tl.y);
        bottomRight = new Vector2(br.x, br.y);
    }

    public BoundingRect(float top, float right, float bottom, float left)
    {
        topLeft = new Vector2(left, top);
        bottomRight = new Vector2(right, bottom);
    }

    public float left()
    {
        return topLeft.x;

    }

    public float right()
    {
        return bottomRight.x;
    }

    public float top()
    {
        return topLeft.y;
    }

    public float bottom()
    {
        return bottomRight.y;
    }

    public BoundingRect clone()
    {
        return new BoundingRect(topLeft, bottomRight);
    }

    public bool contains(rbPoint p)
    {
        // NOTE: top and left inclusive, bottom and right exclusive.
        return left() <= p.Pos.x && p.Pos.x < right() && bottom() < p.Pos.y && p.Pos.y <= top();
    }

    public bool overlaps(BoundingRect o)
    {
        // TODO: I am not 100% sure about this formula.
        bool LR = left() < o.right() && o.left() < right();
        bool TB = bottom() < o.top() && o.bottom() < top();        
        return LR && TB;
    }

    public List<BoundingRect> getSplit()
    {
        float horCenter = (top() + bottom()) / 2;
        float vertCenter = (left() + right()) / 2;
        return new List<BoundingRect>{
            new BoundingRect(top(), vertCenter, horCenter, left()),     // TL
            new BoundingRect(top(), right(), horCenter, vertCenter),    // TR
            new BoundingRect(horCenter, vertCenter, bottom(), left()),  // BL
            new BoundingRect(horCenter, right(), bottom(), vertCenter)  // BR
        };
    }    
}

public class Node
{
    public Node parent;
    public int size;
    public BoundingRect boundingRect;
    public List<Node> children;
    public rbPoint point;

    public Node(Node parent, int size, BoundingRect boundingRect)
    {
        this.parent = parent;
        this.boundingRect = boundingRect;
        children = new List<Node>();
        point = null;
    }
}


