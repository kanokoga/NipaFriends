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
            this.targetPoint = this.transform.position + Vector3.forward * 10f;
        }

        // Update is called once per frame
        void Update()
        {
            var mouseX = Input.GetAxis("Mouse X");
            var mouseY = Input.GetAxis("Mouse Y");
            var mouseWheelScroll = Input.GetAxis("Mouse ScrollWheel");

            var isControlAndCommand = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);
            var isAlt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
            var isShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            // 平行移動
            if (Input.GetMouseButton(2) || (isAlt && isControlAndCommand))
            {
                var move = new Vector3(-mouseX, -mouseY, 0f) * this.translateSpeed;
                var moveWorld = this.transform.TransformVector(move);

                // World XZ平面平行移動
                if (isShift) moveWorld.y = 0f;

                this.targetPoint += moveWorld;

                this.transform.Translate(moveWorld, Space.World);
            }

            // ズーム
            if (mouseWheelScroll != 0)
            {
                var moveWorld = this.transform.forward * mouseWheelScroll * this.zoomSpeed;

                // World XZ平面平行移動
                if (isShift) moveWorld.y = 0f;

                this.transform.Translate(moveWorld, Space.World);

                var dist = Vector3.Distance(this.transform.position, this.targetPoint);
                if (dist <= 1f)
                {
                    this.targetPoint = this.transform.position + this.transform.forward * 1f;
                }
            }


            // 回転
            if (Input.GetMouseButton(1))
            {
                var dist = Vector3.Distance(this.transform.position, this.targetPoint);

                this.transform.rotation = Quaternion.AngleAxis(this.rotateSpeed * -mouseY, this.transform.right) * this.transform.rotation;
                this.transform.rotation = Quaternion.AngleAxis(this.rotateSpeed * mouseX, Vector3.up) * this.transform.rotation;

                this.targetPoint = this.transform.position + this.transform.forward * dist;
            }


            // 注視点の周りを回る
            if (Input.GetMouseButton(0) && !isControlAndCommand && isAlt)
            {
                this.transform.RotateAround(this.targetPoint, this.transform.right, -mouseY * this.rotateSpeed);
                this.transform.RotateAround(this.targetPoint, Vector3.up, mouseX * this.rotateSpeed);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.targetPoint, 0.1f);
        }
    }
}
