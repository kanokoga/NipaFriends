using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NipaFriends
{
    /// <summary>
    /// this code is originally from https://github.com/SubjectNerd-Unity/ReorderableInspector
    /// </summary>
    [CustomEditor(typeof(UnityEngine.Object), true)]
    [CanEditMultipleObjects]
    public class ContextMenuButton : Editor
    {
        private struct ContextMenuData
        {
            public string menuItem;
            public MethodInfo function;
            public MethodInfo validate;

            public ContextMenuData(string item)
            {
                this.menuItem = item;
                this.function = null;
                this.validate = null;
            }
        }

        private Dictionary<string, ContextMenuData> contextData = new Dictionary<string, ContextMenuData>();

        private void OnEnable()
        {
            this.FindContextMenu();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            this.DrawContextMenuButtons();
        }

        private void FindContextMenu()
        {
            this.contextData.Clear();
            var targetType = this.target.GetType();
            var contextMenuType = typeof(ContextMenu);
            foreach(var methodInfo in targetType.GetMethods(BindingFlags.Instance | BindingFlags.Public |
                                                            BindingFlags.NonPublic))
            {
                foreach(ContextMenu contextMenu in methodInfo.GetCustomAttributes(contextMenuType, false))
                {
                    var data = new ContextMenuData(contextMenu.menuItem);
                    if(contextMenu.validate)
                        data.validate = methodInfo;
                    else
                        data.function = methodInfo;

                    this.contextData[contextMenu.menuItem] = data;
                }
            }
        }

        private void DrawContextMenuButtons()
        {
            if(this.contextData.Count == 0) return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Context Menu", EditorStyles.boldLabel);
            foreach(var kv in this.contextData)
            {
                bool enabledState = GUI.enabled;
                bool isEnabled = kv.Value.validate == null || (bool)kv.Value.validate.Invoke(this.target, null);

                GUI.enabled = isEnabled;
                if(GUILayout.Button(kv.Key) && kv.Value.function != null)
                {
                    kv.Value.function.Invoke(this.target, null);
                }

                GUI.enabled = enabledState;
            }
        }
    }
}
