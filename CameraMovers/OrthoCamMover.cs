using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NipaFriends;
using PrefsGUI;
using PrefsGUI.RapidGUI;
using RapidGUI;
using UnityEngine;

namespace NipaFriends
{
    public class OrthoCamMover : SingletonMonoBehaviour<OrthoCamMover>, IDoGUI
    {
        public event Action<float> OnCameraZoomSizeChanged = delegate(float f) { };
        public float CamraSize => this.cam.orthographicSize;

        [SerializeField] private Camera cam;
        [SerializeField] private Rect cameraMovableArea = new Rect(-100f, -100f, 200f, 200f);
        [SerializeField] PrefsFloat camMoveSpeed = new PrefsFloat("CamMoveSpeed", 50f);
        [SerializeField] PrefsFloat camZoomSpeed = new PrefsFloat("CamZoomSpeed", 100f);
        [SerializeField] PrefsMinMaxFloat camZoomMinMax = new PrefsMinMaxFloat("CamZoomMinMax", 10f, 150f);


        private void Update()
        {
            //wasd key to move
            if(Input.GetKey(KeyCode.W))
            {
                this.transform.position += Vector3.forward * this.camMoveSpeed * Time.deltaTime;
            }

            if(Input.GetKey(KeyCode.S))
            {
                this.transform.position += Vector3.back * this.camMoveSpeed * Time.deltaTime;
            }

            if(Input.GetKey(KeyCode.A))
            {
                this.transform.position += Vector3.left * this.camMoveSpeed * Time.deltaTime;
            }

            if(Input.GetKey(KeyCode.D))
            {
                this.transform.position += Vector3.right * this.camMoveSpeed * Time.deltaTime;
            }

            //scroll wheel to zoom

            var scroll = Input.GetAxis("Mouse ScrollWheel");
            if(scroll != 0f)
            {
                scroll = scroll < 0f ? -1f : 1f;
                var size = this.cam.orthographicSize;
                size -= scroll * this.camZoomSpeed * Time.deltaTime;
                size = Mathf.Clamp(size, this.camZoomMinMax.min,
                    this.camZoomMinMax.max);

                this.OnCameraZoomSizeChanged(size);
                cam.orthographicSize = size;
            }

            //clamp camera position
            var camPos = this.transform.position;
            camPos.x = Mathf.Clamp(camPos.x, this.cameraMovableArea.xMin, this.cameraMovableArea.xMax);
            camPos.z = Mathf.Clamp(camPos.z, this.cameraMovableArea.yMin, this.cameraMovableArea.yMax);
            this.transform.position = camPos;
        }

        public void DoGUI()
        {
            this.camMoveSpeed.DoGUI();
            this.camZoomSpeed.DoGUI();
            this.camZoomMinMax.DoGUI();
        }

        private void OnDrawGizmos()
        {
            //draw camera movable area
            Gizmos.color = Color.green;
            var center = new Vector3(this.cameraMovableArea.center.x, 0f, this.cameraMovableArea.center.y);
            var size = new Vector3(this.cameraMovableArea.size.x, 0f, this.cameraMovableArea.size.y);
            Gizmos.DrawWireCube(center, size);
        }
    }
}
