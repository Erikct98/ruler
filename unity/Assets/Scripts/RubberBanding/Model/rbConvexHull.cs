namespace RubberBanding
{
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class rbConvexHull : MonoBehaviour {

    public List<rbPoint> convexHull;
    rbPoint pre;
    rbPoint post;
    rbPoint removed;

    float CrossProduct(rbPoint A, rbPoint B, rbPoint C)
    {
        return (B.Pos.x - A.Pos.x) * (C.Pos.y - A.Pos.y) - (B.Pos.y - A.Pos.y) * (C.Pos.x - A.Pos.x);
    }
    
	
    public List<rbPoint> BuildConvexHull(List<rbPoint> points)
    {
        if (points.Count <= 1)
        {
            return points;
        };

        int n = points.Count;
        List<rbPoint> hull = new List<rbPoint>(new rbPoint[n * 2]);


        //sort points from smallest to largest based on x then y
        points.Sort((a,b) => 
            a.Pos.x == b.Pos.x ? a.Pos.y.CompareTo(b.Pos.y) : a.Pos.x.CompareTo(b.Pos.x));

        //Create lower hull
        int k = 0;
        for (int i = 0; i < n; i++)
        {
            while(k >= 2 && CrossProduct(hull[k - 2], hull[k - 1], points[i]) <= 0)
            {
                k--;
            }
            hull[k++] = points[i];
        }

        //Add upper hull
        int l = k + 1;
        for (int i = n - 2; i >= 0; i--)
        {
            while (k >= l && CrossProduct(hull[k - 2], hull[k - 1], points[i]) <= 0)
            {
                k--;
            }
            hull[k++] = points[i];
        }

        this.convexHull = hull.Take(k - 1).ToList();

        return convexHull;
    }

    public bool RemovePoint(rbPoint p)
    {
        /*
         * TODO: remove p from tree
         * return if success
         */
        int n = this.convexHull.Count;
        for (int i = 0; i < n; i++)
        {
            if (this.convexHull[i].Equals(p))
            {
                removed = p;
                pre = this.convexHull[i - 1];
                post = this.convexHull[i + 1];
                break;
            }
        }
        return this.convexHull.Remove(p);
    }

    void UpdateConvexHull(){
        
    }
}
}
