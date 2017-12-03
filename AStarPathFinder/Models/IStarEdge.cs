using System;
using System.Collections.Generic;
using System.Text;

namespace AStarPathFinder.Models
{

    public interface IStarEdge 
    {  
        IStarNode Source { get; }  
        IStarNode Child { get; }  
        float GetWeight();
        void SetWeight(float weight); 


    }   



}
