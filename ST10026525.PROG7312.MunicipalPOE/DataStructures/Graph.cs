using System;
using System.Collections.Generic;

namespace ST10026525.PROG7312.MunicipalPOE.DataStructures
{
    public class Graph<T> where T : class
    {
        private readonly Dictionary<T, List<T>> _adjList = new();

        public void AddNode(T node)
        {
            if (!_adjList.ContainsKey(node))
                _adjList[node] = new List<T>();
        }

        public Graph<T> AddEdge(T from, T to)
        {
            AddNode(from);
            AddNode(to);

            if (!_adjList[from].Contains(to))
                _adjList[from].Add(to);

            if (!_adjList[to].Contains(from))
                _adjList[to].Add(from);

            return this;
        }

        public List<T> GetNeighbors(T node)
        {
            return _adjList.ContainsKey(node) ? _adjList[node] : new List<T>();
        }

        public IEnumerable<T> Nodes => _adjList.Keys;
    }
}
