using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NipaDijkstra
{

    public Dictionary<int, NodeInfo> nodes { get; private set; }
	Dictionary<int, Record> records;

	public NipaDijkstra(NodeInfo[] ns)
	{
		nodes = new Dictionary<int, NodeInfo>();
		foreach (var n in ns)
		{
			nodes.Add(n.nodeId, n);
			//Debug.Log(n.neighborIdAndCost.Count);
		}
		foreach (var n in nodes)
		{
			foreach (var nn in n.Value.neighborIdAndCost)
			{
				var nNode = nodes[nn.Key];
				if (!nNode.neighborIdAndCost.ContainsKey(n.Key))
				{
					nNode.neighborIdAndCost.Add(n.Key, nn.Value);
				}
			}
		}

		records = new Dictionary<int, Record>();
		foreach (var item in nodes)
		{
			records.Add(item.Value.nodeId, new Record() { previousNodeId = -1, costAmount = Mathf.Infinity });
		}
	}

	public Stack<int> Find(int start, int goal)
	{
		if (start == goal)
			return new Stack<int>();

		foreach (var item in nodes)
		{
			var r = records[item.Key];
			r.costAmount = Mathf.Infinity;
			r.previousNodeId = -1;
			records[item.Key] = r;
		}

		var s = records[start];
		s.costAmount = 0f;
		records[start] = s;

		CheckRoute(nodes[start]);

		var result = new Stack<int>();
		var id = goal;
		if (id != -1)
		{
			result.Push(id);
			while (id != -1)
			{
				id = records[id].previousNodeId;
				result.Push(id);
				if (id == start)
					break;
			}
		}

		return result;
	}

	void CheckRoute(NodeInfo checkFrom)
	{
		foreach (var n in nodes[checkFrom.nodeId].neighborIdAndCost)
		{
			var dist = records[checkFrom.nodeId].costAmount + checkFrom.neighborIdAndCost[n.Key];
			var r = records[n.Key];
			if (dist < r.costAmount)
			{
				r.previousNodeId = checkFrom.nodeId;
				r.costAmount = dist;
				records[n.Key] = r;
				CheckRoute(nodes[n.Key]);
			}
		}
	}

	struct Record
	{
		public int previousNodeId;
		public float costAmount;
	}

	public class NodeInfo
	{
		public int nodeId;
		public Dictionary<int, float> neighborIdAndCost;
	}
}
