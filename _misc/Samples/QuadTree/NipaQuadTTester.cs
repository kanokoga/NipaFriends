using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NipaFriends.WorldSpaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NipaFriends.Samples
{
    public class NipaQuadTTester : MonoBehaviour
    {
        private NipaQuadTree quadTree;
        public int maxTreeDepth = 3;
        public Vector2 normalizedPos = new Vector2(0.5f, 0.5f);
        public int depth;
        public int indexInDepth;
        private string debugText;
        private string debugText2;
        private List<(Vector2Int, Vector2Int)> rects = new List<(Vector2Int, Vector2Int)>();
        private List<Vector2> points = new List<Vector2>();

        // Start is called before the first frame update
        private void Start()
        {
            this.quadTree = new NipaQuadTree(this.maxTreeDepth);
            this.quadTree.DebugHasObjectPerGrid();
        }

        [ContextMenu("UpdateGridAndTree")]
        private void UpdateGridAndTree()
        {
            this.SetObject();
            this.ClearTree();
            this.GenerateTree();
            this.DebugIsTreeNodeExistPerLinerNodeIndex();
            var sweep = new List<(int, int)>();
            this.quadTree.Sweep(0, 0, sweep);
            this.rects = sweep.Select(v => this.quadTree.GetGridArea(v.Item1, v.Item2)).ToList();
        }

        [ContextMenu("UpdateGridAndTreeRnd")]
        private void UpdateGridAndTreeRnd()
        {
            this.ClearObject();
            var start = new Vector2(Random.value, Random.value);
            for(var i = 0; i < 10; i++)
            {
                start = start + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 0.5f;
                var clamped = new Vector2(Mathf.Clamp01(start.x), Mathf.Clamp01(start.y));
                this.quadTree.SetObject(clamped);
                this.points.Add(clamped);
            }
            this.ClearTree();
            this.GenerateTree();
            this.DebugIsTreeNodeExistPerLinerNodeIndex();
            var sweep = new List<(int, int)>();
            this.quadTree.Sweep(0, 0, sweep);
            this.rects = sweep.Select(v => this.quadTree.GetGridArea(v.Item1, v.Item2)).ToList();
        }

        [ContextMenu("DebugTree")]
        private void DebugTree()
        {
            this.debugText = this.quadTree.DebugHasObjectPerGrid();
        }

        [ContextMenu("SetObject")]
        private void SetObject()
        {
            this.quadTree.SetObject(this.normalizedPos);
            this.points.Add(this.normalizedPos);
            this.DebugTree();
        }

        [ContextMenu("ClearObject")]
        private void ClearObject()
        {
            this.points.Clear();
            this.quadTree.ClearObject();
            this.DebugTree();
        }

        [ContextMenu("Get2DMortonNumber")]
        private void Get2DMortonNumber()
        {
            Debug.Log(this.quadTree.Get2DMortonNumber(this.quadTree.GetCellPosition(this.normalizedPos)));
        }

        [ContextMenu("DebugIsTreeNodeExistPerLinerNodeIndex")]
        private void DebugIsTreeNodeExistPerLinerNodeIndex()
        {
            this.debugText2 = this.quadTree.DebugIsTreeNodeExistPerLinerNodeIndex();
            var nodes = new List<(int, int)>();
            this.quadTree.Sweep(0, 0, nodes);
            Debug.Log(string.Join(", ", nodes.Select(v => $"(d:{v.Item1}, idx:{v.Item2})")));
        }

        [ContextMenu("GenerateTree")]
        private void GenerateTree()
        {
            this.quadTree.GenerateTree();
            this.DebugIsTreeNodeExistPerLinerNodeIndex();
        }

        [ContextMenu("ClearTree")]
        private void ClearTree()
        {
            this.quadTree.ClearTree();
            this.DebugIsTreeNodeExistPerLinerNodeIndex();
        }

        [ContextMenu("CheckGridArea")]
        private void CheckGridArea()
        {
            var gridArea = this.quadTree.GetGridArea(this.depth, this.indexInDepth);
            Debug.Log($"GridArea: {gridArea.min} {gridArea.max}");
        }


        private void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 1000, 1000), this.debugText);
            GUI.Label(new Rect(200, 30, 1000, 1000), this.debugText2);
        }

        private void OnDrawGizmos()
        {
            foreach(var rect in this.rects)
            {
                var min = new Vector3(rect.Item1.x, 0, rect.Item1.y);
                var max = new Vector3(rect.Item2.x + 1, 0, rect.Item2.y + 1);
                Gizmos.DrawWireCube((min + max) * 0.5f, max - min);
            }

            Gizmos.color = Color.red;
            foreach(var point in this.points)
            {
                Gizmos.DrawSphere(new Vector3(point.x, 0, point.y) * this.quadTree.GridSize, 0.1f);
            }
        }
    }
}
