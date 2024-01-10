using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NipaFriends.WorldSpaces
{
    public class NipaStepPathFinder<T> where T : INipaStepPath<T>
    {
        public List<Queue<T>> Seach(T start, T goal, int maxStep = 100, bool includeStartInResult = false, bool searchSeveralPathInSameStep = true)
        {
            var parents = new List<INipaStepPath<T>>();
            var toCheck = new Queue<NodeInfo<T>>();
            int stepPathFound = 0;
            var pathFound = new List<NodeInfo<T>>();

            parents.Add(start);
            foreach (var n in start.GetNeighbors())
            {
                toCheck.Enqueue(new NodeInfo<T>(n, start, null));
                //  parents.Add(n);
            }

            NodeInfo<T> target = null;

            while (toCheck.Count > 0)
            {
                target = toCheck.Dequeue();

                var step = target.parents.Count;
                if (step > maxStep)
                    break;

                if (stepPathFound != 0 && step > stepPathFound)
                    break;

                if (target.self.Equals(goal))
                {
                    stepPathFound = step;
                    pathFound.Add(target);
                    if (!searchSeveralPathInSameStep)
                        break;
                }
                else
                {
                    var neighbors = target.self.GetNeighbors();
                    bool childAdded = false;
                    foreach (var n in neighbors)
                    {
                        if (parents.Contains(n))
                            continue;

                        toCheck.Enqueue(new NodeInfo<T>(n, target.self, target.parents));
                        childAdded = true;
                    }
                    if (childAdded && !parents.Contains(target.self))
                        parents.Add(target.self);
                }
            }

            if (pathFound.Count == 0)
                return null;

            var result = new List<Queue<T>>();

            foreach (var item in pathFound)
            {
                var ini = includeStartInResult ? item.parents.Count - 1 : item.parents.Count - 2;
                var path = new Queue<T>();
                for (int i = ini; i > -1; i--)
                {
                    path.Enqueue(item.parents[i]);
                }
                path.Enqueue(goal);
                result.Add(path);
            }
            return result;
        }


        public class NodeInfo<S> where S : INipaStepPath<S>
        {
            public NodeInfo(S me, S parent, List<S> parentsOfParent)
            {
                this.self = me;
                this.parents = new List<S>();
                this.parents.Add(parent);
                if (parentsOfParent != null)
                    this.parents.AddRange(parentsOfParent);

            }


            public S self;
            public List<S> parents;
        }
    }

    public interface INipaStepPath<T>
    {
        T Me();
        T[] GetNeighbors();
    }
}