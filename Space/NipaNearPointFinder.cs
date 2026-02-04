using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NipaFriends.WorldSpaces
{
    public class NipaNearPointFinder
    {
        #region ***Define***

        public struct PointInfo
        {
            public int id;
            public Vector2 position;
        }

        #endregion

        private readonly Dictionary<Vector2Int, List<int>> pointIdsPerIndex;
        private readonly bool[,] isExist;
        private readonly Vector2Int cellSize;
        private readonly float cellLength;

        public NipaNearPointFinder(Vector2 size, float divideLength, List<PointInfo> points)
        {
            this.cellLength = divideLength;
            this.cellSize = new Vector2Int(Mathf.CeilToInt(size.x / this.cellLength), Mathf.CeilToInt(size.y / this.cellLength));
            this.isExist = new bool[this.cellSize.x + 1, this.cellSize.y + 1];
            this.pointIdsPerIndex = new Dictionary<Vector2Int, List<int>>();

            for (var i = 0; i < points.Count; i++)
            {
                var info = points[i];
                var cellIdex = this.GetCellIndex(info.position);
                if (this.pointIdsPerIndex.ContainsKey(cellIdex) == false)
                {
                    this.pointIdsPerIndex.Add(cellIdex, new List<int>());
                }
                this.isExist[cellIdex.x, cellIdex.y] = true;
                this.pointIdsPerIndex[cellIdex].Add(info.id);
            }
        }

        public List<int> GetNearPoints(Vector2 pos)
        {
            var searchLevelLeft = 2;
            var level = 1;
            var result = new List<int>();
            var targetIdex = this.GetCellIndex(pos);

            if (this.IsValid(targetIdex) && this.isExist[targetIdex.x, targetIdex.y])
            {
                result.AddRange(this.pointIdsPerIndex[targetIdex]);
            }

            var pointFound = false;

            while (searchLevelLeft > 0)
            {
                var validCellFound = false;

                var arounds = this.GetAroundCellIndex(targetIdex, level);
                foreach (var a in arounds)
                {
                    if (this.IsValid(a))
                    {
                        validCellFound = true;
                        if (this.isExist[a.x, a.y])
                        {
                            result.AddRange(this.pointIdsPerIndex[a]);
                            pointFound = true;
                        }
                    }
                }

                if (validCellFound == false)
                {
                    break;
                }

                if (pointFound == true)
                {
                    searchLevelLeft--;
                }

                level++;
            }

            return result;
        }

        private List<Vector2Int> GetAroundCellIndex(Vector2Int center, int distance)
        {
            var result = new List<Vector2Int>();
            result.Add(center + Vector2Int.up * distance);
            var index = 1;

            for (var i = 0; i < distance; i++)
            {
                result.Add(result[index - 1] - Vector2Int.one);
                index++;
            }
            for (var i = 0; i < distance; i++)
            {
                result.Add(result[index - 1] + Vector2Int.right + Vector2Int.down);
                index++;
            }
            for (var i = 0; i < distance; i++)
            {
                result.Add(result[index - 1] + Vector2Int.right + Vector2Int.up);
                index++;
            }
            for (var i = 0; i < distance - 1; i++)
            {
                result.Add(result[index - 1] + Vector2Int.left + Vector2Int.up);
                index++;
            }

            return result;
        }

        private bool IsValid(Vector2Int index)
            => index.x >= 0 && index.y >= 0 && index.x <= this.cellSize.x && index.y <= this.cellSize.y;

        private Vector2Int GetCellIndex(Vector2 p)
            => new Vector2Int(Mathf.CeilToInt(p.x / this.cellLength), Mathf.CeilToInt(p.y / this.cellLength));

    }
}