using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NipaFriends.Samples
{
    public class MeshNavigationTest : MonoBehaviour
    {
        public MeshFaceData meshFaceData;
        public float gizmoOffset = 0.1f;
        private MeshNavigation meshNavigation;
        public Transform startPoint;
        public Transform endPoint;
        private List<MeshNavigation.Face> faces;

        [ContextMenu("Generate Face Data")]
        private void GenerateFaceData()
        {
            this.meshFaceData = MeshFaceData.GenerateData(this.gameObject.GetComponent<MeshFilter>().sharedMesh);
        }

        private void Awake()
        {
            this.meshNavigation = new MeshNavigation(this.meshFaceData);
        }

        [ContextMenu("Find Route")]
        private void FindRoute()
        {
            var startPoint = this.transform.InverseTransformPoint(this.startPoint.position);
            var endPoint = this.transform.InverseTransformPoint(this.endPoint.position);
            NipaThread.RunTask(() =>
            {
                this.faces = this.meshNavigation.GetRoute(
                    startPoint,
                    endPoint);
            }, () =>
            {
                this.faces = this.faces.Select(v => new MeshNavigation.Face()
                {
                    position = this.transform.TransformPoint(v.position),
                    normal = this.transform.TransformDirection(v.normal)
                }).ToList();
            });
        }

        private void OnDrawGizmos()
        {
            if(this.meshFaceData != null)
            {
                Gizmos.color = Color.red;
                for(var i = 0; i < this.meshFaceData.faces.Length; i++)
                {
                    var face = this.meshFaceData.faces[i];
                    foreach(var n in face.neighborIndex)
                    {
                        var neighborFace = this.meshFaceData.faces[n];

                        Gizmos.DrawLine(face.position + face.normal * this.gizmoOffset,
                            neighborFace.position + neighborFace.normal * this.gizmoOffset);
                    }
                }

                for(var i = 0; i < this.meshFaceData.faces.Length; i++)
                {
                    var face = this.meshFaceData.faces[i];
                    Gizmos.DrawSphere(face.position + face.normal * this.gizmoOffset, 0.02f);
                }
            }

            if(this.faces != null)
            {
                Gizmos.color = Color.green;
                for(var i = 0; i < this.faces.Count; i++)
                {
                    var face = this.faces[i];
                    Gizmos.DrawWireSphere(face.position + face.normal * this.gizmoOffset, 0.02f);
                    if(i < this.faces.Count - 1)
                    {
                        var nextFace = this.faces[i + 1];
                        Gizmos.DrawLine(face.position + face.normal * this.gizmoOffset,
                            nextFace.position + nextFace.normal * this.gizmoOffset);
                    }
                }
            }
        }
    }
}
