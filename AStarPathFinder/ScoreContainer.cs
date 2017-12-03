using AStarPathFinder.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AStarPathFinder
{
    public class ScoreContainer
    {
        private float DefaultValue;
        private Dictionary<ScoreType, Dictionary<IStarNode, float>> _Scores;

        public ScoreContainer(float defValue = float.MaxValue)
        {
            DefaultValue = defValue;
            _Scores = new Dictionary<ScoreType, Dictionary<IStarNode, float>>();
        }  

        public float this[ScoreType type,  IStarNode node]
        {
            get {
               
                if (_Scores.ContainsKey(type) &&  _Scores[type].ContainsKey(node))
                    return _Scores[type][node];
                else
                    return DefaultValue; 
            }
        }
         
        public void SetScore(ScoreType type, IStarNode node, float score)
        {
            if (!_Scores.ContainsKey(type))
                _Scores.Add(type, new Dictionary<IStarNode, float>()); 
            if (_Scores[type].ContainsKey(node))
                _Scores[type].Remove(node);   

            _Scores[type].Add(node, score); 
        } 


    }
}
