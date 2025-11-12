using System;
using System.Collections.Generic;

namespace ST10026525.PROG7312.MunicipalPOE.DataStructures
{
    public class BSTreeNode<T>
    {
        public Guid Key { get; set; }
        public T Value { get; set; }
        public BSTreeNode<T> Left { get; set; }
        public BSTreeNode<T> Right { get; set; }

        public BSTreeNode(Guid key, T value)
        {
            Key = key;
            Value = value;
        }
    }

    public class BSTree<T> where T : class
    {
        private BSTreeNode<T> _root;

        public void Insert(Guid key, T value)
        {
            _root = InsertRec(_root, key, value);
        }

        private BSTreeNode<T> InsertRec(BSTreeNode<T> node, Guid key, T value)
        {
            if (node == null)
                return new BSTreeNode<T>(key, value);

            if (key.CompareTo(node.Key) < 0)
                node.Left = InsertRec(node.Left, key, value);
            else if (key.CompareTo(node.Key) > 0)
                node.Right = InsertRec(node.Right, key, value);
            // If key already exists, overwrite
            else
                node.Value = value;

            return node;
        }

        public T Search(Guid key)
        {
            var node = _root;
            while (node != null)
            {
                if (key.CompareTo(node.Key) == 0) return node.Value;
                node = key.CompareTo(node.Key) < 0 ? node.Left : node.Right;
            }
            return null;
        }

        public List<T> InOrderTraversal()
        {
            var list = new List<T>();
            InOrder(_root, list);
            return list;
        }

        private void InOrder(BSTreeNode<T> node, List<T> list)
        {
            if (node == null) return;
            InOrder(node.Left, list);
            list.Add(node.Value);
            InOrder(node.Right, list);
        }
    }
}
