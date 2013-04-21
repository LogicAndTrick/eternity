using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eternity.Algorithms
{
    public static class Search
    {
        private class Node<T> : IEquatable<Node<T>>, IComparable<Node<T>> where T : IEquatable<T>
        {
            public List<T> Path { get; private set; }
            public T Item { get; private set; }
            public double HeuristicValue { get; private set; }
            public int Depth { get; private set; }

            public Node(T item, int depth, double heuristicValue, List<T> path)
            {
                Item = item;
                Depth = depth;
                HeuristicValue = heuristicValue;
                Path = path;
            }

            public bool Equals(Node<T> other)
            {
                return Item.Equals(other.Item);
            }

            public int CompareTo(Node<T> other)
            {
                return (Depth + HeuristicValue).CompareTo(other.Depth + other.HeuristicValue);
            }
        }

        public static List<T> InformedSearch<T>(T initialState, Func<T, List<T>> expand, Func<T,double> heuristic, Func<T, bool> goal) where T : IEquatable<T>
        {
            var repeated = new List<Node<T>>();
            var fringe = new List<Node<T>>
            {
                new Node<T>(initialState, 0, heuristic(initialState), new List<T> { initialState })
            };

            while (fringe.Count > 0)
            {
                fringe.Sort();
                var next = fringe[0];
                fringe.RemoveAt(0);
                repeated.Add(next);
                if (goal(next.Item))
                {
                    return next.Path;
                }

                var more = expand(next.Item);
                foreach (var m in more)
                {
                    var nd = new Node<T>(m, next.Depth + 1, heuristic(m), next.Path.Concat(new[] { m }).ToList());
                    if (repeated.Contains(nd)) continue;
                    var ind = fringe.IndexOf(nd);
                    if (ind >= 0)
                    {
                        var match = fringe[ind];
                        if (nd.HeuristicValue + nd.Depth < match.HeuristicValue + match.Depth)
                        {
                            fringe.Remove(match);
                            fringe.Add(nd);
                        }
                    }
                    else
                    {
                        fringe.Add(nd);
                    }
                }
            }
            return new List<T>();
        }

        private class Node2<T> : IComparable<Node2<T>>
        {
            public List<T> Path { get; private set; }
            public T Item { get; private set; }
            public int Cost { get; private set; }

            public Node2(T item, int cost, List<T> path)
            {
                Item = item;
                Cost = cost;
                Path = path;
            }

            public int CompareTo(Node2<T> other)
            {
                return Cost.CompareTo(other.Cost);
            }
        }

        public static List<T> GetAllStates<T>(T initialState, Func<T, int, IEnumerable<T>> expand, Func<T, T, bool> equator, Func<T, int> cost)
        {
            var fringe = new List<Node2<T>>
                             {
                                 new Node2<T>(initialState, 0, new List<T> {initialState})
                             };
            var resultList = new List<T>();

            while (fringe.Any())
            {
                // Start from the lowest cost item
                fringe.Sort();
                var e = fringe[0];

                // Remove all equal items (as the remainder have higher cost)
                fringe.RemoveAll(x => equator(e.Item, x.Item));

                // Add item into list of result states if it's not there already
                if (!resultList.Any(x => equator(x, e.Item))) resultList.Add(e.Item);

                // Expand node
                var items = expand(e.Item, e.Cost);
                if (items != null) fringe.AddRange(items.Select(x => new Node2<T>(x, e.Cost + cost(x), new List<T>(e.Path) { x })));
            }
            return resultList;
        }

        public static List<T> InformedSearch<T>(T initialState, int initialCost, Func<T, int, IEnumerable<T>> expand, Func<T, T, bool> equator, Func<T, int> cost, Func<T, bool> goal)
        {
            var fringe = new List<Node2<T>>
            {
                new Node2<T>(initialState, initialCost, new List<T> { initialState })
            };

            while (fringe.Any())
            {
                // Start from the lowest cost item
                fringe.Sort();
                var e = fringe[0];

                // Remove all equal items (as the remainder have higher cost)
                fringe.RemoveAll(x => equator(e.Item, x.Item));

                // If we are at the goal, return the path
                if (goal(e.Item)) return e.Path;

                // Expand node
                var items = expand(e.Item, e.Cost);
                if (items != null) fringe.AddRange(items.Select(x => new Node2<T>(x, e.Cost + cost(x), new List<T>(e.Path) { x })));
            }
            return new List<T>();
        }
    }
}
