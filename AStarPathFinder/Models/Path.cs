using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AStarPathFinder.Models
{
    public class Path  : IComparable<Path>
    {  
        public IStarNode Parent { set; get; }
        public float Cost { set; get; }

        public int CompareTo(Path other)
        {
            if (other.Cost > Cost) return -1;
            if (other.Cost < Cost) return 1; 
            return 0; 
        }
    }

}
