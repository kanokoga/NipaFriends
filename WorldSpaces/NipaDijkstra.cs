using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NipaDijkstra
{
    #region Define

    private struct Record
    {
        public int previousNodeId;
        public float costAmount;
    }

    public class Node
    {
        public int id;
        public Dictionary<int, float> neighborIdAndCost;
    }

    #endregion

    private Dictionary<int, Node> nodes;
    private Dictionary<int, Record> records;


    public NipaDijkstra(IEnumerable<Node> nodes)
    {
        this.nodes = new Dictionary<int, Node>();
        foreach(var n in nodes)
        {
            this.nodes.Add(n.id, n);
        }

        foreach(var n in this.nodes)
        {
            foreach(var nn in n.Value.neighborIdAndCost)
            {
                var nNode = this.nodes[nn.Key];
                if(!nNode.neighborIdAndCost.ContainsKey(n.Key))
                {
                    nNode.neighborIdAndCost.Add(n.Key, nn.Value);
                }
            }
        }

        this.records = new Dictionary<int, Record>();
        foreach(var item in this.nodes)
        {
            this.records.Add(item.Value.id, new Record() { previousNodeId = -1, costAmount = Mathf.Infinity });
        }
    }

    public List<int> GetRoute(int start, int goal)
    {
        var result = new List<int>();
        if(start == goal)
        {
            return result;
        }

        foreach(var item in this.nodes)
        {
            var r = this.records[item.Key];
            r.costAmount = Mathf.Infinity;
            r.previousNodeId = -1;
            this.records[item.Key] = r;
        }

        var s = this.records[start];
        s.costAmount = 0f;
        this.records[start] = s;

        this.CheckRoute(this.nodes[start]);

        var id = goal;
        if(id != -1)
        {
            result.Add(id);
            while(id != -1)
            {
                id = this.records[id].previousNodeId;
                result.Add(id);
                if(id == start)
                {
                    break;
                }
            }
        }

        result.Reverse();
        return result;
    }

    private void CheckRoute(Node checkFrom)
    {
        foreach(var n in this.nodes[checkFrom.id].neighborIdAndCost)
        {
            var dist = this.records[checkFrom.id].costAmount + checkFrom.neighborIdAndCost[n.Key];
            var r = this.records[n.Key];
            if(dist < r.costAmount)
            {
                r.previousNodeId = checkFrom.id;
                r.costAmount = dist;
                this.records[n.Key] = r;
                this.CheckRoute(this.nodes[n.Key]);
            }
        }
    }
}
