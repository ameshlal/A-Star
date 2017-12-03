using AStarPathFinder.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AstarAlgoritm.Models
{
    public class Edge : IStarEdge
    {
        private float _Weight;

        public Edge(IStarNode from, IStarNode to, float weight)
        {
            _Weight = weight;
            Source = from;
            Child = to;
        }

        public IStarNode Source { get; }

        public IStarNode Child { get; }

        public float GetWeight() => _Weight; 

        public void SetWeight(float weight) => _Weight = weight;
    }
}
