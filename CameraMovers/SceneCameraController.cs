using UnityEngine;

namespace NipaFriends.CameraMovers
{

    [RequireComponent(typeof(Camera))]
    public class SceneCameraController : MonoBehaviour
    {
        public Vector3 targetPoint; // 注視点
        public float rotateSpeed = 10;
        public float translateSpeed = 1;
        public float zoomSpeed = 5;

        private void Start()
        {
            targetPoint = transform.position + Vector3.forward * 10f;
        }

        // Update is called once per frame
        void Update()
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            float mouseWheelScroll = Input.GetAxis("Mouse ScrollWheel");

            bool isControlAndCommand = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);
            bool isAlt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
            bool isShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            // 平行移動
            if (Input.GetMouseButton(2) || (isAlt && isControlAndCommand))
            {
                var move = new Vector3(-mouseX, -mouseY, 0f) * translateSpeed;
                var moveWorld = transform.TransformVector(move);

                // World XZ平面平行移動
                if (isShift) moveWorld.y = 0f;

                targetPoint += moveWorld;

                transform.Translate(moveWorld, Space.World);
            }

            // ズーム
            if (mouseWheelScroll != 0)
            {
                var moveWorld = transform.forward * mouseWheelScroll * zoomSpeed;

                // World XZ平面平行移動
                if (isShift) moveWorld.y = 0f;

                this.transform.Translate(moveWorld, Space.World);

                float dist = Vector3.Distance(this.transform.position, targetPoint);
                if (dist <= 1f)
                {
                    targetPoint = this.transform.position + this.transform.forward * 1f;
                }
            }


            // 回転
            if (Input.GetMouseButton(1))
            {
                float dist = Vector3.Distance(this.transform.position, targetPoint);

                this.transform.rotation = Quaternion.AngleAxis(rotateSpeed * -mouseY, transform.right) * transform.rotation;
                this.transform.rotation = Quaternion.AngleAxis(rotateSpeed * mouseX, Vector3.up) * transform.rotation;

                targetPoint = this.transform.position + this.transform.forward * dist;
            }


            // 注視点の周りを回る
            if (Input.GetMouseButton(0) && !isControlAndCommand && isAlt)
            {
                this.transform.RotateAround(targetPoint, transform.right, -mouseY * rotateSpeed);
                this.transform.RotateAround(targetPoint, Vector3.up, mouseX * rotateSpeed);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(targetPoint, 0.1f);
        }
    }
}
