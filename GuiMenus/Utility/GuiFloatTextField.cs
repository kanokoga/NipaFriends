using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Debugs.Utility
{
    public class GuiFloatTextField
    {
        public float Value => this.value;
        private float value = 0f;
        private string valueString = "0";

        public GuiFloatTextField(float value)
            => this.Set(value);

        public void Set(float value)
        {
            this.valueString = value.ToString();
            this.value = value;
        }

        public void OnGUI()
        {
            var temp = 0f;
            this.valueString = GUILayout.TextField(this.valueString);
            if(float.TryParse(this.valueString, out temp))
            {
                this.value = temp;
            }
        }

        public void OnGUI(string label)
        {
            using(var v = new GUILayout.VerticalScope("box"))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(label);
                this.OnGUI();
                GUILayout.EndHorizontal();
            }
        }
    }
}
