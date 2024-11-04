using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NipaFriends.WorldSpaces
{
    // todo
    // quad作成時にquadが属するモートン番号を計算する
    //　グリッドのステータスが変更されたらそのグリッドのモートン番号を計算する
    //　モートン番号を元にquadを取得する
    public class TileQuadTree
    {
        private QuadTree quadTree;
        private bool[,] grid;
        public List<QuadTree> quadTrees;

        public TileQuadTree(int size)
        {
            this.grid = new bool[size, size];
        }

        public void SetBool(Vector2Int pos, bool value)
        {
            this.grid[pos.x, pos.y] = value;
            this.quadTrees = new List<QuadTree>();
            this.AddQT(0, 0, this.grid.GetLength(0), this.grid.GetLength(1), this.grid, this.quadTrees);
        }

        private bool ContainsObstacle(int x, int y, int width, int height, bool[,] grid)
        {
            for(var i = x; i < x + width; i++)
            {
                for(var j = y; j < y + height; j++)
                {
                    if(grid[i, j])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void AddQT(int x, int y, int width, int height, bool[,] grid, List<QuadTree> regions)
        {
            if(this.ContainsObstacle(x, y, width, height, grid) == true && (width > 1 && height > 1))
            {
                var halfWidth = width / 2;
                var halfHeight = height / 2;
                this.AddQT(x, y, halfWidth, halfHeight, grid, regions);
                this.AddQT(x + halfWidth, y, width - halfWidth, halfHeight, grid, regions);
                this.AddQT(x, y + halfHeight, halfWidth, height - halfHeight, grid, regions);
                this.AddQT(x + halfWidth, y + halfHeight, width - halfWidth, height - halfHeight, grid, regions);
            }
            else
            {
                regions.Add(new QuadTree(x, y, width, height));
            }
        }

        public struct QuadTree
        {
            public int X { get; private set; }
            public int Y { get; private set; }
            public int Width { get; private set; }
            public int Height { get; private set; }

            public QuadTree(int x, int y, int width, int height)
            {
                this.X = x;
                this.Y = y;
                this.Width = width;
                this.Height = height;
            }
        }
    }
}
