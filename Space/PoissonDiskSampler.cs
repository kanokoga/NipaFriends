using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NipaFriends.WorldSpaces
{

    public class PoissonDiskSampler
    {
        private Vector2 fieldSize;
        private float minRadius;
        private int testCount = 30;
        private float sqrMinRadius;
        private int[][] grid;
        private List<Vector2> activeList;
        private List<Vector2> deployed;

        public Vector2[] GeneratePoint(Vector2 fieldSize, float pitch, int samplingCount)
        {
            this.fieldSize = fieldSize;
            this.minRadius = pitch;
            this.testCount = samplingCount;

            this.activeList = new List<Vector2>();
            this.deployed = new List<Vector2>();
            var gridSize = this.minRadius / Mathf.Pow(2f, 0.5f);
            var girdX = Mathf.RoundToInt(this.fieldSize.x / gridSize);
            var girdY = Mathf.RoundToInt(this.fieldSize.y / gridSize);
            this.grid = new int[girdX][];

            this.sqrMinRadius = this.minRadius * this.minRadius;

            for (var i = 0; i < this.grid.Length; i++)
            {
                this.grid[i] = new int[girdY];
            }

            for (var x = 0; x < this.grid.Length; x++)
            {
                for (var y = 0; y < this.grid[x].Length; y++)
                {
                    this.grid[x][y] = -1;
                }
            }

            var gridX = 0;
            var gridY = 0;


            var initPoint = this.GetRandomPoint();
            this.activeList.Add(initPoint);
            this.deployed.Add(initPoint);
            this.GetGridIndex(initPoint, ref gridX, ref gridY);
            this.grid[gridX][gridY] = this.deployed.Count - 1;

            while (this.activeList.Count != 0)
            {
                var seedPoint = this.activeList.Last();
                var found = false;

                for (var i = 0; i < this.testCount; i++)
                {
                    var point = this.GenRandomPointOnRing(seedPoint, i / (float)this.testCount);

                    if (this.GetGridIndex(point, ref gridX, ref gridY) && !this.IsNeareValidPointExist(point, gridX, gridY))
                    {
                        found = true;
                        this.activeList.Add(point);
                        this.deployed.Add(point);
                        this.GetGridIndex(point, ref gridX, ref gridY);
                        this.grid[gridX][gridY] = this.deployed.Count - 1;
                    }
                }

                if (!found)
                {
                    // ----- remove seed
                    this.activeList.RemoveAt(this.activeList.Count - 1);
                }
            }
            return this.deployed.ToArray();
        }

        private Vector2 GetRandomPoint()
        {
            return new Vector2(this.fieldSize.x * Random.Range(0.2f, 0.8f), this.fieldSize.y * Random.Range(0.2f, 0.8f));
        }

        private Vector2 GenRandomPointOnRing(Vector2 _center, float _factor)
        {
            var ang = _factor * 2f * Mathf.PI;
            return _center + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * this.minRadius * (Random.value + 1f);
        }

        private bool GetGridIndex(Vector2 _p, ref int _gridIndexX, ref int _gridIndexY)
        {
            var x = Mathf.FloorToInt(this.grid.Length * _p.x / this.fieldSize.x);
            var y = Mathf.FloorToInt(this.grid[0].Length * _p.y / this.fieldSize.y);

            if (this.IsValidIndex(x, y))
            {
                _gridIndexX = x;
                _gridIndexY = y;
                return true;
            }
            else
                return false;
        }

        private bool IsNeareValidPointExist(Vector2 _point, int x, int y)
        {
            var validPointIndex = -1;
            int X;
            int Y;

            if (this.GetValidPointIndexInCell(x, y, ref validPointIndex))
                return true;

            X = x + 1;
            Y = y;
            if (this.IsValidPointTooNear(X, Y, _point))
                return true;

            X = x - 1;
            Y = y;
            if (this.IsValidPointTooNear(X, Y, _point))
                return true;

            X = x;
            Y = y + 1;
            if (this.IsValidPointTooNear(X, Y, _point))
                return true;

            X = x;
            Y = y - 1;
            if (this.IsValidPointTooNear(X, Y, _point))
                return true;

            X = x + 1;
            Y = y + 1;
            if (this.IsValidPointTooNear(X, Y, _point))
                return true;

            X = x + 1;
            Y = y - 1;
            if (this.IsValidPointTooNear(X, Y, _point))
                return true;

            X = x - 1;
            Y = y + 1;
            if (this.IsValidPointTooNear(X, Y, _point))
                return true;

            X = x - 1;
            Y = y - 1;
            if (this.IsValidPointTooNear(X, Y, _point))
                return true;

            return false;
        }

        private bool IsValidPointTooNear(int X, int Y, Vector2 _point)
        {
            var validPointIndex = -1;
            if (this.IsValidIndex(X, Y) && this.GetValidPointIndexInCell(X, Y, ref validPointIndex))
            {
                if ((this.deployed[validPointIndex] - _point).sqrMagnitude < this.sqrMinRadius)
                    return true;
                else
                    return false;
                // return true;
            }
            else
                return false;
        }

        private bool GetValidPointIndexInCell(int _gridIndexX, int _gridIndexY, ref int _index)
        {
            _index = this.grid[_gridIndexX][_gridIndexY];

            return _index != -1;
        }

        private bool IsValidIndex(int x, int y)
        {
            //if (x >= 0 && x < grid.Length && y >= 0 && y < grid[0].Length)
            if (x > 0 && x < this.grid.Length - 1 && y > 0 && y < this.grid[0].Length - 1)
            {
                return true;
            }
            else
                return false;
        }

        private void OnDrawGizmos()
        {

            //Gizmos.color = Color.cyan;
            //if (deployed != null)
            //{
            //    foreach (var item in deployed)
            //    {
            //        Gizmos.DrawSphere(new Vector3(item.x, 0f, item.y), 0.25f);
            //    }
            //}
        }
    }
}