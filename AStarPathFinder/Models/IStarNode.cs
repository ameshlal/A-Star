using System;
using System.Collections.Generic;
using System.Text;

namespace AStarPathFinder.Models
{ 
    public  interface IStarNode: IEquatable<IStarNode>, IComparable<IStarNode> 
    { 
        IList<IStarEdge> GetEdges();

        float GetWeight();  

        void SetWeight(float weight);

        float GetScore();

        void SetSCore(float score); 

        IList<IStarNode> PathToMe { set; get; } 
         
    }

}
