using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Common.Debugs.Utility
{
    public class GuiV3TextField
    {
        public Vector3 Value => this.value;
        private Vector3 value = Vector3.zero;
        private string[] valueStrings = Enumerable.Range(0, 3).Select(v => "0").ToArray();

        public GuiV3TextField(Vector3 value)
            => this.Set(value);

        public void Set(Vector3 value)
        {
            this.valueStrings[0] = value.x.ToString();
            this.valueStrings[1] = value.y.ToString();
            this.valueStrings[2] = value.z.ToString();
            this.value = value;
        }
        public void OnGUI()
        {
            var temp = 0f;
            GUILayout.BeginHorizontal();
            this.valueStrings[0] = GUILayout.TextField(this.valueStrings[0]);
            if(float.TryParse(this.valueStrings[0], out temp))
            {
                this.value.x = temp;
            }
            this.valueStrings[1] = GUILayout.TextField(this.valueStrings[1]);
            if(float.TryParse(this.valueStrings[1], out temp))
            {
                this.value.y = temp;
            }
            this.valueStrings[2] = GUILayout.TextField(this.valueStrings[2]);
            if(float.TryParse(this.valueStrings[2], out temp))
            {
                this.value.z = temp;
            }
            GUILayout.EndHorizontal();
        }

        public void OnGUI(string label)
        {
            using(var v = new GUILayout.VerticalScope("box"))
            {
                GUILayout.Label(label);
                this.OnGUI();
            }
        }
    }
}
