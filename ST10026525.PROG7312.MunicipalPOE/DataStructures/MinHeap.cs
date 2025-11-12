using System;
using System.Collections.Generic;

namespace ST10026525.PROG7312.MunicipalPOE.DataStructures
{
    public class MinHeap<T>
    {
        private readonly List<T> _elements = new();
        private readonly Comparison<T> _comparer;

        public MinHeap(Comparison<T> comparer)
        {
            _comparer = comparer;
        }

        public int Count => _elements.Count;

        public void Insert(T item)
        {
            _elements.Add(item);
            HeapifyUp(_elements.Count - 1);
        }

        public T ExtractMin()
        {
            if (_elements.Count == 0) throw new InvalidOperationException("Heap is empty");
            var min = _elements[0];
            _elements[0] = _elements[^1];
            _elements.RemoveAt(_elements.Count - 1);
            HeapifyDown(0);
            return min;
        }

        // 👇 Add this method
        public T Peek()
        {
            if (_elements.Count == 0)
                throw new InvalidOperationException("Heap is empty");
            return _elements[0];
        }

        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (_comparer(_elements[index], _elements[parent]) >= 0) break;
                Swap(index, parent);
                index = parent;
            }
        }

        private void HeapifyDown(int index)
        {
            int lastIndex = _elements.Count - 1;
            while (true)
            {
                int left = 2 * index + 1;
                int right = 2 * index + 2;
                int smallest = index;

                if (left <= lastIndex && _comparer(_elements[left], _elements[smallest]) < 0) smallest = left;
                if (right <= lastIndex && _comparer(_elements[right], _elements[smallest]) < 0) smallest = right;

                if (smallest == index) break;
                Swap(index, smallest);
                index = smallest;
            }
        }

        private void Swap(int i, int j)
        {
            var tmp = _elements[i];
            _elements[i] = _elements[j];
            _elements[j] = tmp;
        }
    }
}
