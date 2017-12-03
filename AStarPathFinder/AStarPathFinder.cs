using AStarPathFinder.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AStarPathFinder
{ 

  
    public class AStarPathFinder
    {
        public delegate float Heuristic(IStarNode from, IStarNode to);
        public delegate float DistanceBetweenEvaluater(IStarEdge edge);
        private Heuristic _HDelegate;
        private DistanceBetweenEvaluater _DistanceEvaluator;
        private ScoreContainer _Scores;

        public AStarPathFinder(Heuristic del, DistanceBetweenEvaluater evaluator)
        {
            this._HDelegate = del;
            _DistanceEvaluator = evaluator;
            _Scores = new ScoreContainer();
        } 

        public IStarNode Search(IStarNode start, IStarNode goal)
        {
            HashSet<IStarNode> closedSet = new HashSet<IStarNode>();
            HashSet<IStarNode> openSet = new HashSet<IStarNode>();
           
            openSet.Add(start);
            Dictionary<IStarNode, List<IStarNode>> cameFrom = new Dictionary<IStarNode, List<IStarNode>>();
            _Scores.SetScore(ScoreType.SCORE_OF_DISTANCE, start, 0);
            _Scores.SetScore(ScoreType.SCORE_OF_TOTAL, start, _HDelegate(start, goal));
           

            
            while (openSet.Count > 0)
            { 
                var current = openSet.OrderBy(o => _Scores[ScoreType.SCORE_OF_TOTAL, o]).FirstOrDefault();

                if (current.Equals(goal))
                {
                    current.PathToMe.Add(current);
                    return current;
                }

                if (closedSet.Contains(current))
                    continue;   

                openSet.Remove(current);
                closedSet.Add(current); 
                foreach (var edges in current.GetEdges())
                {
                    if (closedSet.Contains(edges.Child))
                        continue;

                    var tgSCore = _Scores[ScoreType.SCORE_OF_DISTANCE, current] + _DistanceEvaluator(edges);
                    if (tgSCore >= _Scores[ScoreType.SCORE_OF_DISTANCE, edges.Child])
                        continue; 
                    
                    // edges  
                    edges.Child.SetSCore(_Scores[ScoreType.SCORE_OF_DISTANCE, edges.Child]
                                        + _HDelegate(edges.Child, goal));  
                    edges.Child.PathToMe = (current.PathToMe.ToList());
                    edges.Child.PathToMe.Add(current);
                    openSet.Add(edges.Child); 
                    
                    // _Scores 
                    _Scores.SetScore(ScoreType.SCORE_OF_DISTANCE, edges.Child, tgSCore);
                    _Scores.SetScore(ScoreType.SCORE_OF_TOTAL, edges.Child, _Scores[ScoreType.SCORE_OF_DISTANCE, edges.Child]
                                        + _HDelegate(edges.Child, goal));

                }
            }
            return null;
        }

    }
}
