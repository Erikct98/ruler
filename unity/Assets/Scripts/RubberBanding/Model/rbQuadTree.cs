using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RubberBanding {
    public class RbQuadTree {

    private Node Root;

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
	
    private Node Build(Node parent, List<rbPoint> points, AARect rectangle)
    {
        Node n = new Node(parent, points.Capacity, rectangle);
        
        // Edge case: only one point has to fit in this box
        if (points.Capacity == 1)
        {
            n.point = points[0];
            return n;
        }

        // Split bounding rectangle into four pieces
        List<AARect> boundingRects = rectangle.getSplit();

        // Recursively build tree. Note: depth first
        List<List<rbPoint>> pointSplit = new List<List<rbPoint>>();
        for (int i = 0; i < boundingRects.Capacity; i++)
        {
            n.children.Add(Build(n, points.FindAll(p => boundingRects[i].Contains(p)), boundingRects[i]));
        }

        
        return n;
    }

    public void RemovePoint(rbPoint p)
    {
        // First, seach in which node this p belongs
        Node n = Root;
        while (n.point == null)
        {
            for (int i = 0; i < n.children.Capacity; i++)
            {
                Node child = n.children[i];
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
    public List<rbPoint> getPointsInRectangle(AARect bound)
    {
        return FindInRange(Root, bound);
    }

    private List<rbPoint> FindInRange(Node n, AARect bound)
    {
        List<rbPoint> points = new List<rbPoint>();
        foreach (Node child in n.children)
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
}

/*
 * Axis aligned rectangle used in the construction
 * and utilization of Quadtree
 */
public class AARect
{
    public float Left { get; private set; }
    public float Right { get; private set; }
    public float Top { get; private set; }
    public float Bottom { get; private set; }

    public AARect(float top, float right, float bottom, float left)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public AARect(Vector2 tl, Vector2 br)
    {
        Left = tl.x;
        Right = br.x;
        Top = tl.y;
        Bottom = br.y;
    }

    public bool Contains(rbPoint p)
    {
        // NOTE: top and left inclusive, bottom and right exclusive.
        return Left <= p.Pos.x && p.Pos.x < Right && Bottom < p.Pos.y && p.Pos.y <= Top;
    }

    public bool Overlaps(AARect o)
    {
        // TODO: I am not 100% sure about this formula.
        bool LR = Left < o.Right && o.Left < Right;
        bool TB = Bottom < o.Top && o.Bottom < Top;
        return LR && TB;
    }

    public List<AARect> getSplit()
    {
        float horCenter = (Top + Bottom) / 2;
        float vertCenter = (Left + Right) / 2;
        return new List<AARect>{
            new AARect(Top, vertCenter, horCenter, Left),     // TL
            new AARect(Top, Right, horCenter, vertCenter),    // TR
            new AARect(horCenter, vertCenter, Bottom, Left),  // BL
            new AARect(horCenter, Right, Bottom, vertCenter)  // BR
        };
    }    
}

public class Node
{
    public Node parent;
    public int size;
    public AARect boundingRect;
    public List<Node> children;
    public rbPoint point;

    public Node(Node parent, int size, AARect boundingRect)
    {
        this.parent = parent;
        this.boundingRect = boundingRect;
        children = new List<Node>();
        point = null;
    }
}
}



