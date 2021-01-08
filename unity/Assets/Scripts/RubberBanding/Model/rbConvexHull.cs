using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace RubberBanding {
    public class rbConvexHull {
        I2DRangeQuery<rbPoint> RangeQuery = null;
        public List<rbPoint> convexHull = null;
        rbPoint pre = null;
        rbPoint post = null;
        rbPoint removed = null;

        public struct Result
        {
            public int areaScore;
            public List<rbPoint> area;
        }

        float CrossProduct(rbPoint A, rbPoint B, rbPoint C)
        {
            return (B.Pos.x - A.Pos.x) * (C.Pos.y - A.Pos.y) - (B.Pos.y - A.Pos.y) * (C.Pos.x - A.Pos.x);
        }
        
        
        public List<rbPoint> BuildConvexHull(List<rbPoint> points)
        {
            RangeQuery = new RbQuadTree(points);
            // RangeQuery = new RbRangeTree(points);
            
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

        private void RemovePoint(rbPoint p, bool remove)
        {
            /*
            * TODO: remove p from tree
            * return if success
            */
            int n = this.convexHull.Count;
            int index = this.convexHull.IndexOf(p);

            this.removed = p;
            this.pre = this.convexHull[(index - 1 + n) % n];
            this.post = this.convexHull[(index + 1) % n];

            if (remove)
            {
                p.Removed = true;
                this.convexHull.Remove(p);
            }
        }

        public void UpdateConvexHull(rbPoint p, bool remove, out int areaScore, out List<rbPoint> area) {
            var isPointOnHull = convexHull.IndexOf(p) != -1;
            //Debug.Log( "Before remove point");
            //Debug.Log(p);
            RemovePoint(p, remove);
            List<rbPoint> queryPoints = new List<rbPoint>();
            queryPoints.Add(this.pre);
            queryPoints.Add(this.post);
            queryPoints.Add(p);

            AARect rect = AARect.GetBoundingRectangle(queryPoints);
            List<rbPoint> points = this.RangeQuery.FindInRange(rect);
            //Debug.Log("RangeQuery points");
            //Debug.Log(points.Count());
            // Debug.Log("pre");
            // Debug.Log(this.pre.Pos);
            // Debug.Log("post");
            // Debug.Log(this.post.Pos);


            points.Add(this.pre);
            points.Add(this.post);
            points.Remove(p);
            
            points.Sort((a, b) =>
            a.Pos.x == b.Pos.x ? a.Pos.y.CompareTo(b.Pos.y) : a.Pos.x.CompareTo(b.Pos.x));

            int n = points.Count();
            List<rbPoint> patch = new List<rbPoint>(new rbPoint[n]);
            List<rbPoint> temp;

            if (CrossProduct(this.pre, this.removed, this.post) == 0)
            {
                //return new Result{areaScore = 0, area = null};
                areaScore = 0;
                area = null;
            } else if (points[0] != this.pre && points[0] != this.post)
            {
                
                if (points[n - 1] == this.pre)
                {
                    if (this.pre.Pos.y > this.post.Pos.y)
                    {
                        patch = upper(points);
                        temp = lower(points.Take(points.IndexOf(this.post) + 1).ToList());
                    } else
                    {
                        temp = lower(points);
                        patch = upper(points.Take(points.IndexOf(this.post) + 1).ToList());
                    }
                } else {
                    if (this.post.Pos.y > this.pre.Pos.y)
                    {
                        patch = upper(points);
                        temp = lower(points.Take(points.IndexOf(this.pre) + 1).ToList());
                    } else
                    {
                        temp = lower(points);
                        patch = upper(points.Take(points.IndexOf(this.pre) + 1).ToList());
                    }
                }
                temp.Remove(points[0]);
                temp.Reverse();
                patch.AddRange(temp);
            } else if (points[n - 1] != this.pre && points[n - 1] != this.post)
            {
                if (points[0] == this.pre)
                {
                    if (this.pre.Pos.y > this.post.Pos.y)
                    {
                        temp = upper(points);
                        patch = lower(points.Skip(points.IndexOf(this.post)).ToList());
                    } else
                    {
                        patch = lower(points);
                        temp = upper(points.Skip(points.IndexOf(this.post)).ToList());
                    }
                } else {
                    if (this.post.Pos.y > this.pre.Pos.y)
                    {
                        temp = upper(points);
                        patch = lower(points.Skip(points.IndexOf(this.pre)).ToList());
                    } else
                    {
                        patch = lower(points);
                        temp = upper(points.Skip(points.IndexOf(this.pre)).ToList());
                    }
                }
                temp.Remove(points[n - 1]);
                patch.AddRange(temp);
            } else if (CrossProduct(points[0], this.removed, points[n - 1]) > 0)
            {
                patch = lower(points);
            } else if (CrossProduct(points[0], this.removed, points[n - 1]) < 0)
            {
                patch = upper(points);
            }


            // Debug.Log("patch");
            // Debug.Log(patch.Count());
            n = patch.Count();
            // Only update the convex hull if we actually remove this point
            if (remove && isPointOnHull) {
                int index = this.convexHull.IndexOf(patch[n - 1]);

                for (int i = n - 2; i > 0; i--)
                {
                    this.convexHull.Insert(index, patch[i]);
                }
            }
            
            int score = 0;
            if (isPointOnHull) {
                for (int i = 0; i < n - 1; i++)
                {
                    score += (int) Math.Abs(patch[i].Pos.x * (patch[i + 1].Pos.y - this.removed.Pos.y) + patch[i + 1].Pos.x * (this.removed.Pos.y - patch[i].Pos.y) + this.removed.Pos.x * (patch[i].Pos.y - patch[i + 1].Pos.y));
                }
            } else {
                score += 5000;
            }
            

            //return new Result{areaScore = score, area = patch};
            areaScore = score;
            patch.Add(p);
            area = patch;
        }

        private List<rbPoint> lower(List<rbPoint> points)
        {
            int n = points.Count();
            List<rbPoint> lower = new List<rbPoint>(new rbPoint[n]);

            int k = 0;
            for (int i = 0; i < n; i++)
            {
                while (k >= 2 && CrossProduct(lower[k - 2], lower[k - 1], points[i]) <= 0)
                {
                    k--;
                }
                lower[k++] = points[i];
            }
            return lower.Take(k).ToList();
        }

        private List<rbPoint> upper(List<rbPoint> points)
        {
            int n = points.Count();
            List<rbPoint> upper = new List<rbPoint>(new rbPoint[n]);

            int k = 0;
            for (int i = n - 1; i >= 0; i--)
            {
                while (k >= 2 && CrossProduct(upper[k - 2], upper[k - 1], points[i]) <= 0)
                {
                    k--;
                }
                upper[k++] = points[i];
            }
            return upper.Take(k).ToList();
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

