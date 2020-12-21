using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RubberBanding { 
    public abstract class I2DRangeQuery<T> {

        public abstract List<T> FindInRange(AARect range);

        public abstract void RemovePoint(T point);
    }

    /*
     * Axis aligned rectangle used in the construction
     * and utilization of I2DRangeQuery<T> objects.
     */
    public class AARect
    {
        public float Left { get; set; }
        public float Right { get; set; }
        public float Top { get; set; }
        public float Bottom { get; set; }

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

        /// <summary>
        /// Boundary inclusive contain
        /// </summary>
        public bool Contains(rbPoint p)
        {
            return Left < p.Pos.x && p.Pos.x < Right && Bottom <= p.Pos.y && p.Pos.y <= Top;
        }

        /// <summary>
        /// Top and left inclusive, bottom and right exclusive.
        /// </summary>
        public bool SplitContains(rbPoint p)
        {            
            return Left <= p.Pos.x && p.Pos.x < Right && Bottom < p.Pos.y && p.Pos.y <= Top;
        }

        public bool Intersects(AARect o)
        {
            // TODO: I am not 100% sure about this formula.
            bool LR = Left <= o.Right && o.Left <= Right;
            bool TB = Bottom <= o.Top && o.Bottom <= Top;
            return LR && TB;
        }

        public List<AARect> Split()
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

        public static AARect GetBoundingRectangle(List<rbPoint> points)
        {
            float left = points.Min(e => e.Pos.x);
            float right = points.Max(e => e.Pos.x);
            float top = points.Max(e => e.Pos.y);
            float bottom = points.Min(e => e.Pos.y);
            return new AARect(top, right, bottom, left);
        }
    }
}
