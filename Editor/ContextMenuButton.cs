using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SubjectNerd.Utilities
{
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
                menuItem = item;
                function = null;
                validate = null;
            }
        }

        private Dictionary<string, ContextMenuData> contextData = new Dictionary<string, ContextMenuData>();

        private void OnEnable()
        {
            FindContextMenu();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawContextMenuButtons();
        }

        private void FindContextMenu()
        {
            contextData.Clear();
            Type targetType = target.GetType();
            Type contextMenuType = typeof(ContextMenu);
            foreach (MethodInfo methodInfo in targetType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                foreach (ContextMenu contextMenu in methodInfo.GetCustomAttributes(contextMenuType, false))
                {
                    var data = new ContextMenuData(contextMenu.menuItem);
                    if (contextMenu.validate)
                        data.validate = methodInfo;
                    else
                        data.function = methodInfo;

                    contextData[contextMenu.menuItem] = data;
                }
            }
        }

        private void DrawContextMenuButtons()
        {
            if (contextData.Count == 0) return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Context Menu", EditorStyles.boldLabel);
            foreach (var kv in contextData)
            {
                bool enabledState = GUI.enabled;
                bool isEnabled = kv.Value.validate == null || (bool)kv.Value.validate.Invoke(target, null);

                GUI.enabled = isEnabled;
                if (GUILayout.Button(kv.Key) && kv.Value.function != null)
                {
                    kv.Value.function.Invoke(target, null);
                }
                GUI.enabled = enabledState;
            }
        }
    }
}
