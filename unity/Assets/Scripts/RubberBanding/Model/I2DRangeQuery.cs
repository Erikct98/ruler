﻿using System.Collections;
using System.Collections.Generic;
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
    }
}
