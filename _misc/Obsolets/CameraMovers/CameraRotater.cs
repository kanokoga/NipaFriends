using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NipaFriends.CameraMovers
{
    public class CameraRotater : MonoBehaviour
    {
        [SerializeField] private Transform lookTarget;
        [SerializeField] private Transform childInLookTarget;
        [SerializeField] private Transform camera;
        public float rotationSpeed = 1f;
        public float zoomSpeed = 1f;

        private void Awake()
        {
            this.camera.LookAt(this.lookTarget);
        }

        private void Update()
        {
            // if(Input.GetMouseButton(1) == true)
            // {
            //     var mousePositionDeltaNormalized = Input.mousePositionDelta;
            //     mousePositionDeltaNormalized.x /= Screen.width;
            //     mousePositionDeltaNormalized.y /= Screen.height;
            //     this.lookTarget.Rotate(0f, mousePositionDeltaNormalized.x * this.rotationSpeed, 0f);
            //     this.childInLookTarget.Rotate(-mousePositionDeltaNormalized.y * this.rotationSpeed, 0f, 0f);
            // }

            var mouseScrollDelta = Input.mouseScrollDelta;
            if(mouseScrollDelta.y != 0f)
            {
                var scrollDelta = mouseScrollDelta.y > 0f ? 1f : -1f;
                this.camera.Translate(0f, 0f, scrollDelta * Time.deltaTime * this.zoomSpeed);
            }
        }

        public void SetHoriaonalAngle(float angle)
        {
            this.lookTarget.localRotation = Quaternion.Euler(0f, angle, 0f);
        }
    }
}
