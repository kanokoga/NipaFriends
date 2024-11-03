using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NipaFriends.WorldSpaces
{
    public class TileNavigation
    {
        private struct NeighborInfo
        {
            public Vector2Int pos;
            public int cost;
        }

        public static int NoPassCost = 9999;

        private int[] costPerTile;
        private int tileCountPerEdge;

        public TileNavigation(int tileCountPerEdge)
        {
            this.costPerTile = new int[tileCountPerEdge * tileCountPerEdge];
            this.tileCountPerEdge = tileCountPerEdge;
        }

        public void SetCost(Vector2Int pos, int cost)
        {
            var index = this.GetTileIndex(pos.x, pos.y);
            this.costPerTile[index] = cost;
        }

        /// <summary>
        /// Breadth-First Search (BFS) algorithm
        /// </summary>
        /// <param name="start"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        public List<Vector2Int> GetRoute(Vector2Int start, Vector2Int goal)
        {
            var queue = new Queue<Vector2Int>();
            var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            var costSoFar = new Dictionary<Vector2Int, int>();
            var visited = new HashSet<Vector2Int>();
            var neighborIndex = new Vector2Int[4];
            var neighborInfos = new NeighborInfo[4];

            queue.Enqueue(start);
            visited.Add(start);
            costSoFar[start] = 0;

            while(queue.Count > 0)
            {
                var current = queue.Dequeue();

                if(current == goal)
                {
                    var route = this.ReconstructPath(cameFrom, current);
                    route.Add(goal);
                    return route;
                }

                var relative = (goal - current);
                this.UpdateNeighborIndex(relative, neighborIndex);
                this.UpdateNeighborInfos(neighborInfos, current, neighborIndex);

                for(int i = 0; i < 4; i++)
                {
                    var neighbor = neighborInfos[i];
                    if(neighbor.cost != NoPassCost)
                    {
                        int newCost = costSoFar[current] + neighbor.cost;
                        if(!visited.Contains(neighbor.pos) || newCost < costSoFar[neighbor.pos])
                        {
                            costSoFar[neighbor.pos] = newCost;
                            queue.Enqueue(neighbor.pos);
                            visited.Add(neighbor.pos);
                            cameFrom[neighbor.pos] = current;
                        }
                    }
                }
            }

            return new List<Vector2Int>(); // Return an empty path if no route is found
        }

        private void UpdateNeighborIndex(Vector2Int relative, Vector2Int[] neighborIndex)
        {
            int GetIndex(int v)
                => v > 0 ? 1 : -1;

            var isXGreater = Mathf.Abs(relative.x) > Mathf.Abs(relative.y);

            if(isXGreater == true)
            {
                neighborIndex[0] = new Vector2Int(GetIndex(relative.x), 0);
                neighborIndex[1] = new Vector2Int(0, GetIndex(relative.y));
                neighborIndex[3] = new Vector2Int(0, -GetIndex(relative.y));
                neighborIndex[2] = new Vector2Int(-GetIndex(relative.x), 0);
            }
            else
            {
                neighborIndex[0] = new Vector2Int(0, GetIndex(relative.y));
                neighborIndex[1] = new Vector2Int(GetIndex(relative.x), 0);
                neighborIndex[3] = new Vector2Int(-GetIndex(relative.x), 0);
                neighborIndex[2] = new Vector2Int(0, -GetIndex(relative.y));
            }
        }

        private void UpdateNeighborInfos(NeighborInfo[] neighborInfos, Vector2Int pos, Vector2Int[] neighborIndexs)
        {
            for(int i = 0; i < 4; i++)
            {
                var neighborIndex = neighborIndexs[i];
                var neighborPos = pos + neighborIndex;
                var isValidPos = (neighborPos.x < 0 || neighborPos.x >= this.tileCountPerEdge ||
                                  neighborPos.y < 0 || neighborPos.y >= this.tileCountPerEdge) == false;
                var index = this.GetTileIndex(neighborPos.x, neighborPos.y);
                var cost = isValidPos == true ? this.costPerTile[index] : NoPassCost;
                neighborInfos[i] = new NeighborInfo { pos = neighborPos, cost = cost };
            }
        }

        private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
        {
            var totalPath = new List<Vector2Int> { current };
            while(cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Add(current);
            }

            totalPath.Reverse();
            return totalPath;
        }

        private int GetTileIndex(int x, int y)
        {
            return x + y * this.tileCountPerEdge;
        }
    }
}
