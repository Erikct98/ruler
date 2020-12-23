using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RubberBanding {
    public class rbConvexHull {
        I2DRangeQuery<rbPoint> RangeQuery = null;
        public List<rbPoint> convexHull = null;
        rbPoint pre = null;
        rbPoint post = null;
        rbPoint removed = null;

        float CrossProduct(rbPoint A, rbPoint B, rbPoint C)
        {
            return (B.Pos.x - A.Pos.x) * (C.Pos.y - A.Pos.y) - (B.Pos.y - A.Pos.y) * (C.Pos.x - A.Pos.x);
        }
        
        
        public List<rbPoint> BuildConvexHull(List<rbPoint> points)
        {
            RangeQuery = new RbQuadTree(points);
            //RangeQuery = new RbRangeTree(points);
            
            if (points.Count() <= 1)
            {
                return points;
            };

            int n = points.Count();
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
            int n = this.convexHull.Count();

            if (this.convexHull[0].Equals(p))
            {
                this.removed = p;
                this.pre = this.convexHull[n - 1];
                this.post = this.convexHull[1];
                // this.RangeQuery.RemovePoint(p);//remove if switching to range tree
            } else if (this.convexHull[n - 1].Equals(p))
            {
                this.removed = p;
                this.pre = this.convexHull[n - 2];
                this.post = this.convexHull[0];
                // this.RangeQuery.RemovePoint(p);//remove if switching to range tree
            } else
            {
                for (int i = 1; i < n - 1; i++)
                {
                    if (this.convexHull[i].Equals(p))
                    {
                        this.removed = p;
                        this.pre = this.convexHull[i - 1];
                        this.post = this.convexHull[i + 1];
                        // this.RangeQuery.RemovePoint(p);//remove if switching to range tree
                        break;
                    }
                }
            }

            p.Removed = true;
            return this.convexHull.Remove(p);
        }

        public int UpdateConvexHull(rbPoint p) {
            RemovePoint(p);
            List<rbPoint> queryPoints = new List<rbPoint>();
            queryPoints.Add(this.pre);
            queryPoints.Add(this.post);
            queryPoints.Add(p);

            AARect rect = AARect.GetBoundingRectangle(queryPoints);
            List<rbPoint> points = this.RangeQuery.FindInRange(rect);
            Debug.Log("RangeQuery points");
            Debug.Log(points.Count());

            points.Add(this.pre);
            points.Add(this.post);
            points.Remove(p);

            points.Sort((a, b) =>
            a.Pos.x == b.Pos.x ? a.Pos.y.CompareTo(b.Pos.y) : a.Pos.x.CompareTo(b.Pos.x));

            int n = points.Count();
            List<rbPoint> patch = new List<rbPoint>(new rbPoint[n]);

            int k = 0;
            if (CrossProduct(points[0], this.removed, points[n - 1]) == 0)
            {
                return 0;
            } else if (CrossProduct(points[0], this.removed, points[n - 1]) > 0)
            {
                k = 0;
                for (int i = 0; i < n; i++)
                {
                    while (k >= 2 && CrossProduct(patch[k - 2], patch[k - 1], points[i]) <= 0)
                    {
                        k--;
                    }
                    patch[k++] = points[i];
                }
            } else if (CrossProduct(points[0], this.removed, points[n - 1]) < 0)
            {
                k = 0;
                for (int i = n - 1; i >= 0; i--)
                {
                    while (k >= 2 && CrossProduct(patch[k - 2], patch[k - 1], points[i]) <= 0)
                    {
                        k--;
                    }
                    patch[k++] = points[i];
                }
            }

            patch = patch.Take(k).ToList();
            n = patch.Count();

            int index = this.convexHull.IndexOf(patch[n - 1]);

            for (int i = n - 2; i > 0; i--)
            {
                this.convexHull.Insert(index, patch[i]);
            }

            int score = 0;
            for (int i = 0; i < n - 1; i++)
            {
                score += (int) (patch[i].Pos.x * (patch[i + 1].Pos.y - this.removed.Pos.y) + patch[i + 1].Pos.x * (this.removed.Pos.y - patch[i].Pos.y) + this.removed.Pos.x * (patch[i].Pos.y - patch[i + 1].Pos.y));
            }
            return score;
        }

        public List<rbPoint> GetReplacements(rbPoint p)
        {
            List<rbPoint> points = new List<rbPoint>();
            int idx = convexHull.IndexOf(p);
            if (idx >= 0)
            {
                int n = convexHull.Count;
                rbPoint prev = convexHull[(idx - 1 + n) % n];
                rbPoint next = convexHull[(idx + 1) % n];
                AARect range = AARect.GetBoundingRectangle(new List<rbPoint>() { p, next, prev });
                points = RangeQuery.FindInRange(range);
            }
            return points;
        }
    }
}

