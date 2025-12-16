using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NipaFriends.CameraMovers
{
    public class FPSController : MonoBehaviour
    {
        public bool IsActive => this.isActive;

        public KeyCode toggleKey = KeyCode.P;
        public float moveSpeed = 10f;
        public float lookSpeed = 2.0f;
        public float verticalLookLimit = 45.0f;

        [SerializeField]
        protected Transform body = null;
        [SerializeField]
        protected Transform face = null;
        [SerializeField]
        protected bool isActive = true;

        protected float verticalRotation = 0;

        protected virtual void Start()
        {
            Cursor.lockState = this.isActive == true ? CursorLockMode.Locked : CursorLockMode.None;
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(this.toggleKey) == true)
            {
                this.SetActive(!this.isActive);
            }

            if (this.isActive == false)
            {
                return;
            }

            // *** Move ***

            var dir = Vector3.zero;
            var moveAmount = 0;

            if (Input.GetKey(KeyCode.W))
            {
                moveAmount++;
                dir += Vector3.forward;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                moveAmount++;
                dir += Vector3.back;
            }
            if (Input.GetKey(KeyCode.A))
            {
                moveAmount++;
                dir += Vector3.left;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                moveAmount++;
                dir += Vector3.right;
            }

            if (moveAmount > 0)
            {
                this.body.Translate((moveAmount == 1 ? dir : dir * 1.4142f) * this.moveSpeed * Time.deltaTime);
            }

            // *** Look ***

            this.verticalRotation += -Input.GetAxis("Mouse Y") * this.lookSpeed;
            this.verticalRotation = Mathf.Clamp(this.verticalRotation, -this.verticalLookLimit, this.verticalLookLimit);
            this.face.localRotation = Quaternion.Euler(this.verticalRotation, 0, 0);
            this.body.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * this.lookSpeed, 0);
        }

        public void SetActive(bool active)
        {
            this.isActive = active;
            Cursor.lockState = this.isActive == true ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}