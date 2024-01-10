using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NipaFriends.WorldSpaces
{
    public class TexturePitchPoissonDiskSampler
    {
        private float gridSize;
        private int[][] grid;
        private List<Vector2> activeList = new List<Vector2>();
        private List<Vector2> deployed = new List<Vector2>();
        private Vector2 fieldSize;
        private float pitchRandomScale;


        public List<Vector2> GeneratePoint(Vector2 fieldSize, Texture2D sampleTex, float pitch, float maxPitchScale, float pitchRandomScale, int samplingCount)
        {
            this.activeList.Clear();
            this.deployed.Clear();
            this.fieldSize = fieldSize;
            this.pitchRandomScale = pitchRandomScale;
            this.gridSize = pitch * 0.5f;

            var gX = Mathf.RoundToInt(fieldSize.x / this.gridSize);
            var gY = Mathf.RoundToInt(fieldSize.y / this.gridSize);
            this.grid = new int[gX][];
            for (var i = 0; i < this.grid.Length; i++)
            {
                this.grid[i] = new int[gY];
            }

            for (var x = 0; x < this.grid.Length; x++)
            {
                for (var y = 0; y < this.grid[x].Length; y++)
                {
                    this.grid[x][y] = -1;
                }
            }

            float GetPitchWeight(Vector2 pos)
            {
                var normalized = new Vector2(pos.x / this.fieldSize.x, pos.y / this.fieldSize.y);
                return Mathf.Lerp(maxPitchScale, 1f, sampleTex.GetPixelBilinear(Mathf.Clamp01(normalized.x), Mathf.Clamp01(normalized.y)).r);
            }

            var gridX = 0;
            var gridY = 0;
            var _initPoint = new Vector2(fieldSize.x, fieldSize.y) * 0.5f;
            this.GetGridIndex(_initPoint, ref gridX, ref gridY);
            this.activeList.Add(_initPoint);
            this.deployed.Add(_initPoint);
            this.grid[gridX][gridY] = this.deployed.Count - 1;

            while (this.activeList.Count != 0)
            {
                var seedPoint = this.activeList.Last();
                var found = false;

                for (var i = 0; i < samplingCount; i++)
                {
                    var maxWeight = GetPitchWeight(seedPoint);
                    var ratio = 0f;
                    while (ratio < 1f)
                    {
                        var neighborNear = this.GetPointOnRing(seedPoint, Mathf.Lerp(pitch, pitch * maxPitchScale, ratio), i / (float)samplingCount);
                        var neighborNearWeight = GetPitchWeight(neighborNear);
                        maxWeight = Mathf.Max(maxWeight, neighborNearWeight);
                        ratio += 0.25f;
                    }

                    var customPitch = maxWeight * pitch;
                    var point = this.GenRandomPointOnRing(seedPoint, customPitch, i / (float)samplingCount);

                    if (this.GetGridIndex(point, ref gridX, ref gridY)
                        && this.IsNeareValidPointExist(point, gridX, gridY, customPitch) == false)
                    {
                        found = true;
                        this.activeList.Add(point);
                        this.deployed.Add(point);
                        this.grid[gridX][gridY] = this.deployed.Count - 1;
                    }

                }

                if (!found)
                {
                    // ----- remove seed
                    this.activeList.RemoveAt(this.activeList.Count - 1);
                }
            }
            return this.deployed;
        }

        private Vector2 GenRandomPointOnRing(Vector2 _center, float _radius, float _angWeight)
        {
            var ang = _angWeight * 2f * Mathf.PI;
            return _center + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * _radius * (Random.value * this.pitchRandomScale + 1f);
        }



        private Vector2 GetPointOnRing(Vector2 _center, float _radius, float _angWeight)
        {
            var ang = _angWeight * 2f * Mathf.PI;
            return _center + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * _radius * 1f;
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

        private Vector2Int GetGridIndex(Vector2 _p)
        {
            var x = Mathf.FloorToInt(this.grid.Length * _p.x / this.fieldSize.x);
            var y = Mathf.FloorToInt(this.grid[0].Length * _p.y / this.fieldSize.y);

            return new Vector2Int(x, y);
        }

        private bool IsNeareValidPointExist(Vector2 _point, int x, int y, float radius)
        {
            var validPointIndex = -1;
            var range = 1;
            range = Mathf.CeilToInt(radius / this.gridSize);
            if (this.GetValidPointIndexInCell(x, y, ref validPointIndex))
                return true;

            var radiusSqr = radius * radius;
            for (var i = x - range; i < x + range + 1; i++)
            {
                for (var v = y - range; v < y + range + 1; v++)
                {
                    if (this.IsValidPointTooNear(i, v, _point, radiusSqr))
                        return true;
                }
            }

            return false;
        }

        private bool IsValidPointTooNear(int X, int Y, Vector2 _point, float _sqrRadius)
        {
            var validPointIndex = -1;
            if (this.IsValidIndex(X, Y) && this.GetValidPointIndexInCell(X, Y, ref validPointIndex))
            {
                if ((this.deployed[validPointIndex] - _point).sqrMagnitude < _sqrRadius)
                    return true;
                else
                    return false;
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
            if (x >= 0 && x < this.grid.Length && y >= 0 && y < this.grid[0].Length)
            {
                return true;
            }
            else
                return false;
        }

        //void OnDrawGizmos()
        //{
        //    //if (activeList != null)
        //    //{
        //    //    //foreach (var item in validPoints)
        //    //    //{
        //    //    //    if (item == null)
        //    //    //        continue;

        //    //    //    Gizmos.DrawSphere(new Vector3(item.x, 0f, item.y), 0.25f);
        //    //    //}
        //    //    foreach (var item in activeList)
        //    //    {


        //    //        Gizmos.DrawSphere(new Vector3(item.x, 0f, item.y), 0.25f);
        //    //    }
        //    //}
        //    Gizmos.color = Color.cyan;
        //    if (deployed != null)
        //    {
        //        foreach (var item in deployed)
        //        {
        //            Gizmos.DrawSphere(new Vector3(item.x, 0f, item.y), 0.25f);
        //        }
        //    }
        //}
    }
}