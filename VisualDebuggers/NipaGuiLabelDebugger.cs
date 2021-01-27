using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NipaFriends
{
    public class NipaGuiLabelDebugger : SingletonMonoBehaviour<NipaGuiLabelDebugger>
    {
       [SerializeField] private Camera targetCamera;
        private Dictionary<Transform, string> labels = new Dictionary<Transform, string>();

        public void SetLabel(Transform trans, string label)
            => this.labels[trans] = label;

        public void Release(Transform trans)
        {
            if (this.labels.ContainsKey(trans) == true)
            {
                this.labels.Remove(trans);
            }
        }

        private void OnGUI()
        {
            foreach (var item in this.labels)
            {
                var screenPos = NipaUtility.GetScreenPosition(this.targetCamera, item.Key.position);
                GUI.Label(new Rect(screenPos.x, screenPos.y, 500f, 30f), item.Value);
            }
        }
    }
}