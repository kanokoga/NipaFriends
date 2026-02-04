using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NipaFriends
{
    public class MeshNavigation
    {
        public struct Face
        {
            public Vector3 position;
            public Vector3 normal;
        }

        private NipaDijkstra dijkstra;
        private Face[] faces;

        public MeshNavigation(MeshFaceData faceData)
        {
            var nodes = new List<NipaDijkstra.Node>();
            for(var i = 0; i < faceData.faces.Length; i++)
            {
                var face = faceData.faces[i];
                var node = new NipaDijkstra.Node()
                {
                    id = i,
                    neighborIdAndCost = new Dictionary<int, float>()
                };

                for(var j = 0; j < face.neighborIndex.Count; j++)
                {
                    var neighborIndex = face.neighborIndex[j];
                    var neighborFace = faceData.faces[neighborIndex];
                    var distance = (face.position - neighborFace.position).sqrMagnitude;
                    node.neighborIdAndCost.Add(neighborIndex, distance);
                }

                nodes.Add(node);
            }

            this.dijkstra = new NipaDijkstra(nodes);
            this.faces = faceData.faces.ToArray().Select(v => new Face()
            {
                position = v.position,
                normal = v.normal
            }).ToArray();
        }

        public List<Face> GetRoute(Vector3 localStart, Vector3 localEnd)
        {
            //find nearest face index
            var startIndex = -1;
            var endIndex = -1;
            var minDistance = float.MaxValue;

            for(var i = 0; i < this.faces.Length; i++)
            {
                var face = this.faces[i];
                var distance = (face.position - localStart).sqrMagnitude;
                if(distance < minDistance)
                {
                    minDistance = distance;
                    startIndex = i;
                }
            }

            minDistance = float.MaxValue;
            for(var i = 0; i < this.faces.Length; i++)
            {
                var face = this.faces[i];
                var distance = (face.position - localEnd).sqrMagnitude;
                if(distance < minDistance)
                {
                    minDistance = distance;
                    endIndex = i;
                }
            }

            //find route
            var route = this.dijkstra.GetRoute(startIndex, endIndex);
            return route.Select(v => this.faces[v]).ToList();
        }
    }


    [Serializable]
    public class MeshFaceData
    {
        [Serializable]
        public struct Face
        {
            public Vector3 position;
            public Vector3 normal;
            public Vector3[] verts;
            public List<int> neighborIndex;
        }

        public Face[] faces;

        public static MeshFaceData GenerateData(Mesh mesh)
        {
            var faces = new List<Face>();


            for(var i = 0; i < mesh.triangles.Length; i += 3)
            {
                var vIndex0 = mesh.triangles[i];
                var vIndex1 = mesh.triangles[i + 1];
                var vIndex2 = mesh.triangles[i + 2];

                var v1 = mesh.vertices[vIndex0];
                var v2 = mesh.vertices[vIndex1];
                var v3 = mesh.vertices[vIndex2];

                var n1 = mesh.normals[vIndex0];
                var n2 = mesh.normals[vIndex1];
                var n3 = mesh.normals[vIndex2];

                var face = new Face()
                {
                    position = (v1 + v2 + v3) / 3f,
                    normal = (n1 + n2 + n3).normalized,
                    neighborIndex = new List<int>(),
                    verts = new Vector3[] { v1, v2, v3 }
                };

                faces.Add(face);
            }

            for(var i = 0; i < faces.Count; i++)
            {
                var face = faces[i];

                for(var j = 0; j < faces.Count; j++)
                {
                    if(i == j)
                    {
                        continue;
                    }

                    var otherFace = faces[j];

                    var nearCount = 0;
                    for(var k = 0; k < face.verts.Length; k++)
                    {
                        for(var l = 0; l < otherFace.verts.Length; l++)
                        {
                            if(face.verts[k] == otherFace.verts[l])
                            {
                                nearCount++;
                            }
                        }
                    }

                    if(nearCount == 2)
                    {
                        if(face.neighborIndex.Contains(j) == false)
                        {
                            face.neighborIndex.Add(j);
                        }

                        if(otherFace.neighborIndex.Contains(i) == false)
                        {
                            otherFace.neighborIndex.Add(i);
                        }
                    }
                }
            }

            var faceData = new MeshFaceData();
            faceData.faces = faces.ToArray();
            return faceData;
        }
    }
}
