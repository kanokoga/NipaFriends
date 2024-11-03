using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NipaFriends.WorldSpaces
{
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
            this.quadTree = new QuadTree(0, 0, this.grid.GetLength(0), this.grid.GetLength(1), this.grid,
                true);
            this.quadTrees = this.quadTree.GetLargestObstacleFreeRegions();
            Debug.Log(this.quadTrees.Count);
        }
        
        public class QuadTree
        {
            public bool HasObstacle { get; private set; }
            public int X { get; private set; }
            public int Y { get; private set; }
            public int Width { get; private set; }
            public int Height { get; private set; }

            public QuadTree TopLeft, TopRight, BottomLeft, BottomRight;

            public QuadTree(int x, int y, int width, int height, bool[,] grid, bool isRoot = false)
            {
                this.X = x;
                this.Y = y;
                this.Width = width;
                this.Height = height;
                this.HasObstacle = this.ContainsObstacle(x, y, width, height, grid);

                if(this.HasObstacle == true && (width > 1 || height > 1))
                {
                    int halfWidth = width / 2;
                    int halfHeight = height / 2;

                    this.TopLeft = new QuadTree(x, y, halfWidth, halfHeight, grid);
                    this.TopRight = new QuadTree(x + halfWidth, y, width - halfWidth, halfHeight, grid);
                    this.BottomLeft = new QuadTree(x, y + halfHeight, halfWidth, height - halfHeight, grid);
                    this.BottomRight = new QuadTree(x + halfWidth, y + halfHeight, width - halfWidth,
                        height - halfHeight, grid);
                }
            }

            private bool ContainsObstacle(int x, int y, int width, int height, bool[,] grid)
            {
                for(int i = x; i < x + width; i++)
                {
                    for(int j = y; j < y + height; j++)
                    {
                        if(grid[i, j])
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            public List<QuadTree> GetLargestObstacleFreeRegions()
            {
                List<QuadTree> regions = new List<QuadTree>();


                if(this.TopLeft == null)
                {
                    regions.Add(this);
                }
                else
                {
                    regions.AddRange(this.TopLeft.GetLargestObstacleFreeRegions());
                    regions.AddRange(this.TopRight.GetLargestObstacleFreeRegions());
                    regions.AddRange(this.BottomLeft.GetLargestObstacleFreeRegions());
                    regions.AddRange(this.BottomRight.GetLargestObstacleFreeRegions());
                }

                return regions;
            }
        }
    }
}
