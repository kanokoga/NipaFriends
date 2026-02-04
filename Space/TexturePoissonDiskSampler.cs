using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TexturePoissonDiskSampler
{
    private Vector2 fieldSize;
    private float minRadius_sampling;
    private float minRadius;
    private Texture2D sampleTex;
    private bool[][] isValid;
    private float grayThreshold;
    private int testCount = 30;
    private float gridSize;
    private int[][] grid;
    private List<Vector2> activeList;
    private List<Vector2> deployed;
    private float random = 1f;

    public Vector2[] GeneratePoint(Vector2 fieldSize, Texture2D sampleTex, float grayThreshold, float pitch_initial, float pitch, int samplingCount, float randomNess = 1f)
    {
        this.random = randomNess;
        this.fieldSize = fieldSize;
        this.minRadius = pitch;
        this.minRadius_sampling = pitch_initial;
        this.testCount = samplingCount;
        this.grayThreshold = grayThreshold;
        this.sampleTex = sampleTex;

        this.activeList = new List<Vector2>();
        this.deployed = new List<Vector2>();
        this.gridSize = this.minRadius / Mathf.Pow(2f, 0.5f);
        var gX = Mathf.RoundToInt(this.fieldSize.x / this.gridSize);
        var gY = Mathf.RoundToInt(this.fieldSize.y / this.gridSize);
        this.grid = new int[gX][];
        this.isValid = new bool[gX][];

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

        for (var i = 0; i < this.grid.Length; i++)
        {
            this.isValid[i] = new bool[gY];
        }

        var deltaX = this.sampleTex.width / (float)gX;
        var deltaY = this.sampleTex.height / (float)gY;

        for (var x = 0; x < this.grid.Length; x++)
        {
            for (var y = 0; y < this.grid[x].Length; y++)
            {
                var c = this.sampleTex.GetPixel(Mathf.FloorToInt(deltaX * x), Mathf.FloorToInt(deltaY * y));
                this.isValid[x][y] = (c.r + c.g + c.b) / 3f < this.grayThreshold;
            }
        }


        this.StartDraw_origines(this.GetRandomPoint(), this.minRadius_sampling);
        var origines = this.deployed.ToArray();

        this.deployed.Clear();
        for (var x = 0; x < this.grid.Length; x++)
        {
            for (var y = 0; y < this.grid[x].Length; y++)
            {
                this.grid[x][y] = -1;
            }
        }

        var tempX = 0;
        var tempY = 0;
        foreach (var item in origines)
        {
            this.GetGridIndex(item, ref tempX, ref tempY);
            if (!this.IsValidArea(tempX, tempY))
            {
                continue;
            }
            this.deployed.Add(item);
            this.grid[tempX][tempY] = this.deployed.Count - 1;
            this.StartDraw(item, this.minRadius);
        }

        return this.deployed.ToArray();
    }

    private void StartDraw_origines(Vector2 _initPoint, float _minRadius)
    {
        var gridX = 0;
        var gridY = 0;
        var point = Vector2.zero;
        var sqrRadius = Mathf.Pow(_minRadius, 2f);

        this.GetGridIndex(_initPoint, ref gridX, ref gridY);
        this.activeList.Add(_initPoint);
        this.deployed.Add(_initPoint);
        this.grid[gridX][gridY] = this.deployed.Count - 1;

        while (this.activeList.Count != 0)
        {
            var seedPoint = this.activeList.Last();
            var found = false;

            for (var i = 0; i < this.testCount; i++)
            {
                point = this.GenRandomPointOnRing(seedPoint, _minRadius, i / (float)this.testCount);

                if (this.GetGridIndex(point, ref gridX, ref gridY) && !this.IsNeareValidPointExist(point, gridX, gridY, sqrRadius))
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
    }

    private void StartDraw(Vector2 _initPoint, float _minRadius)
    {
        var gridX = 0;
        var gridY = 0;
        var point = Vector2.zero;
        var sqrRadius = Mathf.Pow(_minRadius, 2f);

        this.GetGridIndex(_initPoint, ref gridX, ref gridY);
        this.activeList.Add(_initPoint);
        this.deployed.Add(_initPoint);
        this.grid[gridX][gridY] = this.deployed.Count - 1;

        while (this.activeList.Count != 0)
        {
            var seedPoint = this.activeList.Last();
            var found = false;

            for (var i = 0; i < this.testCount; i++)
            {
                point = this.GenRandomPointOnRing(seedPoint, _minRadius, i / (float)this.testCount);

                if (this.GetGridIndex(point, ref gridX, ref gridY) && this.IsValidArea(gridX, gridY) && !this.IsNeareValidPointExist(point, gridX, gridY, sqrRadius))
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
    }

    private Vector2 GetRandomPoint()
    {
        return new Vector2(this.fieldSize.x * Random.value, this.fieldSize.y * Random.value);
    }

    private Vector2 GenRandomPointOnRing(Vector2 _center, float _radius, float _angWeight)
    {
        var ang = _angWeight * 2f * Mathf.PI;
        return _center + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * _radius * (Random.value * this.random + 1f);
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

    private bool IsValidArea(int x, int y)
    {
        return this.isValid[x][y];
    }

    private bool IsNeareValidPointExist(Vector2 _point, int x, int y, float _sqrRadius)
    {
        var validPointIndex = -1;
        var repeat = 1;
        // repeat = 1;
        if (this.GetValidPointIndexInCell(x, y, ref validPointIndex))
            return true;

        for (var i = x - repeat; i < x + repeat + 1; i++)
        {
            for (var v = y - repeat; v < y + repeat + 1; v++)
            {
                if (this.IsValidPointTooNear(i, v, _point, _sqrRadius))
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
