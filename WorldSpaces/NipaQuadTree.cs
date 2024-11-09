using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NipaFriends.WorldSpaces
{
    public class NipaQuadTree
    {
        public int GridSize => this.gridSize;
        private int[] hasObjectPerGrid;
        private int[] isTreeNodeExistPerLinerNodeIndex;
        private int maxTreeDepth;
        private int gridSize;


        public NipaQuadTree(int maxTreeDepth)
        {
            this.maxTreeDepth = maxTreeDepth;
            this.gridSize = (int)Mathf.Pow(2, maxTreeDepth);
            this.hasObjectPerGrid = new int[(1 << (maxTreeDepth * 2)) >> 3];
            this.isTreeNodeExistPerLinerNodeIndex = new int[this.GetFirstLinerNodeIndex(maxTreeDepth)
                                                            + (this.gridSize * this.gridSize)];
        }


        public bool Sweep(int depth, int nodeIndexInDepth, List<(int depth, int indexInDepth)> nodes)
        {
            var linerIndex = this.GetLinerNodeIndex(depth, nodeIndexInDepth);
            if(this.HasNode(linerIndex) == false)
            {
                return false;
            }
            else
            {
                nodes.Add((depth, nodeIndexInDepth));
            }

            var childLinerIndexStart = nodeIndexInDepth * 4;
            var childDepth = depth + 1;
            return this.Sweep(childDepth, childLinerIndexStart, nodes)
                   || this.Sweep(childDepth, childLinerIndexStart + 1, nodes)
                   || this.Sweep(childDepth, childLinerIndexStart + 2, nodes)
                   || this.Sweep(childDepth, childLinerIndexStart + 3, nodes);
        }

        public (Vector2Int min, Vector2Int max) GetGridArea(int depth, int indexInDepth)
        {
            var mortonStart = (int)Mathf.Pow(4, this.maxTreeDepth - depth) * indexInDepth;
            var mortonEnd = mortonStart + (int)Mathf.Pow(4, this.maxTreeDepth - depth) - 1;
            Debug.Log($"mortonStart: {mortonStart}, mortonEnd: {mortonEnd}");
            return (this.GetCellPos(mortonStart), this.GetCellPos(mortonEnd));
        }

        public void GenerateTree()
        {
            for(int i = 0; i < this.gridSize * this.gridSize; i++)
            {
                var hasObject = this.HasObject(i);
                if(hasObject == true)
                {
                    var cellPos = this.GetCellPosition(i);
                    var mortonNumber = this.Get2DMortonNumber(cellPos);
                    var nodeIndexInDepth = 0;
                    for(int d = 0; d <= this.maxTreeDepth; d++)
                    {
                        var parentNodeIndexInDepth = nodeIndexInDepth;
                        nodeIndexInDepth = mortonNumber >> (2 * (this.maxTreeDepth - d));
                        var linerIndex = this.GetLinerNodeIndex(d,
                            nodeIndexInDepth);
                        Debug.Log(
                            $"mortonNumber:{mortonNumber} depth:{d} nodeIndexInDepth: {nodeIndexInDepth} ({nodeIndexInDepth & 0b01}, {nodeIndexInDepth & 0b10 >> 1}), linerIndex: {linerIndex}");
                        this.SetNode(linerIndex);
                    }
                }
            }
        }

        public void ClearTree()
        {
            for(int i = 0; i < this.isTreeNodeExistPerLinerNodeIndex.Length; i++)
            {
                this.isTreeNodeExistPerLinerNodeIndex[i] = 0;
            }
        }

        public void SetObject(Vector2 normalizedPos)
            => this.SetObject(this.GetGridIndex(this.GetCellPosition(normalizedPos)));

        private void SetObject(int gridIndex)
        {
            var index = gridIndex >> 5;
            var bitIndex = gridIndex & 31;
            this.hasObjectPerGrid[index] |= 1 << bitIndex;
        }

        private bool HasObject(int gridIndex)
        {
            var index = gridIndex >> 5;
            var bitIndex = gridIndex & 31;
            return (this.hasObjectPerGrid[index] & (1 << bitIndex)) != 0;
        }

        public void ClearObject()
        {
            for(int i = 0; i < this.hasObjectPerGrid.Length; i++)
            {
                this.hasObjectPerGrid[i] = 0;
            }
        }

        private bool HasNode(int linderNodeIndex)
        {
            var index = linderNodeIndex >> 5;
            var bitIndex = linderNodeIndex & 31;
            return (this.isTreeNodeExistPerLinerNodeIndex[index] & (1 << bitIndex)) != 0;
        }

        private void SetNode(int linderNodeIndex)
        {
            var index = linderNodeIndex >> 5;
            var bitIndex = linderNodeIndex & 31;
            this.isTreeNodeExistPerLinerNodeIndex[index] |= 1 << bitIndex;
        }

        public Vector2Int GetCellPosition(Vector2 normalizedPos)
        {
            var x = Mathf.Min(this.gridSize - 1, Mathf.FloorToInt(normalizedPos.x * this.gridSize));
            var y = Mathf.Min(this.gridSize - 1, Mathf.FloorToInt(normalizedPos.y * this.gridSize));
            return new Vector2Int(x, y);
        }

        private int GetGridIndex(Vector2Int cellPos)
            => this.gridSize * cellPos.y + cellPos.x;

        public int Get2DMortonNumber(Vector2Int cellPos)
        {
            return (this.BitSeparate32(cellPos.x) | (this.BitSeparate32(cellPos.y) << 1));
        }

        public Vector2Int GetCellPos(int mortonNumber)
        {
            var x = this.InverseSeparate32(mortonNumber);
            var y = this.InverseSeparate32(mortonNumber >> 1);
            return new Vector2Int(x, y);
        }

        private int BitSeparate32(int n)
        {
            n = (n | (n << 8)) & 0x00ff00ff;
            n = (n | (n << 4)) & 0x0f0f0f0f;
            n = (n | (n << 2)) & 0x33333333;
            return (n | (n << 1)) & 0x55555555;
        }

        private int InverseSeparate32(int n)
        {
            n &= 0x55555555;
            n = (n ^ (n >> 1)) & 0x33333333;
            n = (n ^ (n >> 2)) & 0x0f0f0f0f;
            n = (n ^ (n >> 4)) & 0x00ff00ff;
            n = (n ^ (n >> 8)) & 0x0000ffff;
            return n;
        }


        private Vector2Int GetCellPosition(int gridIndex)
        {
            var x = gridIndex % this.gridSize;
            var y = gridIndex / this.gridSize;
            return new Vector2Int(x, y);
        }

        private int GetFirstLinerNodeIndex(int depth)
            => ((int)Mathf.Pow(4, depth) - 1) / 3;

        private int GetLinerNodeIndex(int depth, int indexInDepth)
            => ((int)Mathf.Pow(4, depth) - 1) / 3 + indexInDepth;

        public string DebugHasObjectPerGrid()
        {
            var str = "";
            for(int y = 0; y < this.gridSize; y++)
            {
                for(int x = 0; x < this.gridSize; x++)
                {
                    str += this.HasObject(this.GetGridIndex(new Vector2Int(x, y))) ? "1" : "0";
                }

                str += "\n";
            }

            return str;
        }

        public string DebugIsTreeNodeExistPerLinerNodeIndex()
        {
            var str = "";
            for(int d = 0; d <= this.maxTreeDepth; d++)
            {
                var startLinderIndex = this.GetLinerNodeIndex(d, 0);
                var endLinderIndex = this.GetLinerNodeIndex(d, (int)Mathf.Pow(4, d) - 1);

                for(int i = startLinderIndex; i <= endLinderIndex; i++)
                {
                    str += this.HasNode(i) ? "1" : "0";
                    if(i > 0 && i % 4 == 0)
                    {
                        str += "|";
                    }
                }

                str += "\n";
            }

            return str;
        }
    }
}
