using AStarPathFinder.Models;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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

            _Scores.SetScore(ScoreType.SCORE_OF_DISTANCE, start, 0);
            _Scores.SetScore(ScoreType.SCORE_OF_TOTAL, start, _HDelegate(start, goal));

            while (openSet.Count > 0)
            {
                var current = openSet
                        .OrderBy(o => _Scores[ScoreType.SCORE_OF_TOTAL, o]).FirstOrDefault();

                if (current.Equals(goal))
                {
                    current.PathToMe.Add(current);
                    return current;
                }
                if (current.IsVisited())
                    continue;
                else
                    current.Visit();


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

     
        public IStarNode Search(IStarNode start,
                                IStarNode goal, int numberOfWorkers)
        {
            IStarNode best = null;
            ConcurrentDictionary<int, IStarNode> closedSet = new ConcurrentDictionary<int, IStarNode>();
            ConcurrentDictionary<int, IStarNode> openSet = new ConcurrentDictionary<int, IStarNode>();
            openSet.TryAdd(start.GetHashCode(), start); 
            _Scores.SetScore(ScoreType.SCORE_OF_DISTANCE, start, 0);
            _Scores.SetScore(ScoreType.SCORE_OF_TOTAL, start, _HDelegate(start, goal)); 
            List<Action> actions = new List<Action>(); 

            for (int i = 0; i < numberOfWorkers; i++)
            {
                actions.Add(() =>
                {

                    while (openSet.Count > 0)
                    {
                        lock (_Scores)
                        {
                            var current = openSet.Select(x => x.Value)
                                    .OrderBy(o => _Scores[ScoreType.SCORE_OF_TOTAL, o]).FirstOrDefault();

                            if (current == null)
                                break;

                            if (current.IsVisited())
                            {
                                openSet.TryRemove(current.GetHashCode(), out IStarNode removed);
                                if (!closedSet.ContainsKey(current.GetHashCode())) 
                                closedSet.TryAdd(current.GetHashCode(), current); 
                                continue;
                            }
                            else
                                current.Visit();

                            if (current.Equals(goal) && best == null)
                            {
                                current.PathToMe.Add(current);
                                best = current;
                                break;
                            }
                            if (closedSet.ContainsKey(current.GetHashCode()))
                                continue;
                            openSet.TryRemove(current.GetHashCode(), out IStarNode some);
                            closedSet.TryAdd(current.GetHashCode(), current);

                            foreach (var edges in current.GetEdges())
                            {
                                if (closedSet.ContainsKey(edges.Child.GetHashCode()))
                                    continue;
                                var tgSCore = _Scores[ScoreType.SCORE_OF_DISTANCE, current] + _DistanceEvaluator(edges);
                                if (tgSCore >= _Scores[ScoreType.SCORE_OF_DISTANCE, edges.Child])
                                    continue;
                                edges.Child.SetSCore(_Scores[ScoreType.SCORE_OF_DISTANCE, edges.Child]
                                                    + _HDelegate(edges.Child, goal));
                                edges.Child.PathToMe = (current.PathToMe.ToList());
                                edges.Child.PathToMe.Add(current);
                                openSet.TryAdd(edges.Child.GetHashCode(), edges.Child);
                                _Scores.SetScore(ScoreType.SCORE_OF_DISTANCE, edges.Child, tgSCore);
                                _Scores.SetScore(ScoreType.SCORE_OF_TOTAL, edges.Child, _Scores[ScoreType.SCORE_OF_DISTANCE, edges.Child]
                                                    + _HDelegate(edges.Child, goal));

                            }

                        }
                    }
                });

            }



            // 
            SpawnAndWait(actions);
            return best;
        }


        private void SpawnAndWait(IEnumerable<Action> actions)
        {
            var list = actions.ToList();
            var handles = new ManualResetEvent[actions.Count()];
            for (var i = 0; i < list.Count; i++)
            {
                handles[i] = new ManualResetEvent(false);
                var currentAction = list[i];
                var currentHandle = handles[i];
                var number = i;
                Action wrappedAction = () => { try {   currentAction(); } finally { currentHandle.Set(); } };
                ThreadPool.QueueUserWorkItem(x => wrappedAction());
            }
            WaitHandle.WaitAll(handles);
        }


    }
}
