namespace Util.DataStructures.BST
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BST<T> : IBST<T>
        where T : IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Root of the tree
        /// </summary>
        public Node Root;

        /// <summary>
        /// Number of nodes of the tree.
        /// </summary>
        public int Count { get; private set; }

        public BST(IEnumerable<T> elements)
        {
            Root = null;

            // elements.OrderBy(
            //     delegate (T e1, T e2) { return e1.CompareTo(e2); }
            // );
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(T data)
        {
            throw new System.NotImplementedException();
        }

        public bool Delete(T data)
        {
            throw new System.NotImplementedException();
        }

        public T DeleteMax()
        {
            throw new System.NotImplementedException();
        }

        public T DeleteMin()
        {
            throw new System.NotImplementedException();
        }

        public bool FindMax(out T out_MaxValue)
        {
            throw new System.NotImplementedException();
        }

        public bool FindMin(out T out_MinValue)
        {
            throw new System.NotImplementedException();
        }

        public bool FindNextBiggest(T data, out T out_NextBiggest)
        {
            throw new System.NotImplementedException();
        }

        public bool FindNextSmallest(T data, out T out_NextSmallest)
        {
            throw new System.NotImplementedException();
        }

        public bool Insert(T data)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Class for all nodes in the tree.
        /// </summary>
        public class Node
        {
            public Node Left { get; set; }

            public Node Right { get; set; }

            public T Data { get; set; }
            public int Level { get; set; }

            public Node(T a_Data, Node a_Left, Node a_Right, int a_Level)
            {
                Data = a_Data;
                Left = a_Left;
                Right = a_Right;
                Level = a_Level;
            }
        }
    }
}