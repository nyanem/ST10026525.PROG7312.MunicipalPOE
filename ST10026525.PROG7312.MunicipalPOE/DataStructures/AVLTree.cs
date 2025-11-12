using System;
using System.Collections.Generic;

namespace ST10026525.PROG7312.MunicipalPOE.DataStructures
{
    public class AVLNode<T>
    {
        public DateTime Key { get; set; }
        public List<T> Values { get; set; } = new List<T>();
        public AVLNode<T> Left { get; set; }
        public AVLNode<T> Right { get; set; }
        public int Height { get; set; } = 1;

        public AVLNode(DateTime key, T value)
        {
            Key = key;
            Values.Add(value);
        }
    }

    public class AVLTree<T> where T : class
    {
        private AVLNode<T> _root;

        public void Insert(DateTime key, T value)
        {
            _root = InsertRec(_root, key, value);
        }

        private AVLNode<T> InsertRec(AVLNode<T> node, DateTime key, T value)
        {
            if (node == null) return new AVLNode<T>(key, value);

            if (key < node.Key)
                node.Left = InsertRec(node.Left, key, value);
            else if (key > node.Key)
                node.Right = InsertRec(node.Right, key, value);
            else
                node.Values.Add(value);

            node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));

            int balance = GetBalance(node);

            // Left Left
            if (balance > 1 && key < node.Left.Key)
                return RightRotate(node);

            // Right Right
            if (balance < -1 && key > node.Right.Key)
                return LeftRotate(node);

            // Left Right
            if (balance > 1 && key > node.Left.Key)
            {
                node.Left = LeftRotate(node.Left);
                return RightRotate(node);
            }

            // Right Left
            if (balance < -1 && key < node.Right.Key)
            {
                node.Right = RightRotate(node.Right);
                return LeftRotate(node);
            }

            return node;
        }

        private int GetHeight(AVLNode<T> node) => node?.Height ?? 0;
        private int GetBalance(AVLNode<T> node) => node == null ? 0 : GetHeight(node.Left) - GetHeight(node.Right);

        private AVLNode<T> RightRotate(AVLNode<T> y)
        {
            var x = y.Left;
            var T2 = x.Right;

            x.Right = y;
            y.Left = T2;

            y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;
            x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;

            return x;
        }

        private AVLNode<T> LeftRotate(AVLNode<T> x)
        {
            var y = x.Right;
            var T2 = y.Left;

            y.Left = x;
            x.Right = T2;

            x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;
            y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;

            return y;
        }

        public List<T> RangeQuery(DateTime start, DateTime end)
        {
            var list = new List<T>();
            InOrderRange(_root, start, end, list);
            return list;
        }

        private void InOrderRange(AVLNode<T> node, DateTime start, DateTime end, List<T> list)
        {
            if (node == null) return;

            if (start < node.Key)
                InOrderRange(node.Left, start, end, list);

            if (start <= node.Key && node.Key <= end)
                list.AddRange(node.Values);

            if (end > node.Key)
                InOrderRange(node.Right, start, end, list);
        }
    }
}
