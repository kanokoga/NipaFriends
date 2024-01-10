using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace NipaFriends.Guis
{
    public class DebugMenu : SingletonMonoBehaviour<DebugMenu>
    {
        public bool IsMouseOvered => this.isMouseOvered;
        public event Action OnMouseOverUpdated = delegate { };

        [SerializeField] protected KeyCode toggleMenu = KeyCode.F1;
        protected bool showMenu = false;
        protected Dictionary<string, List<IGuiDebugger>> contents = new Dictionary<string, List<IGuiDebugger>>();
        protected IGuiDebugger focusedContent = null;
        protected Rect windowRect = new Rect(0, 0, 600, 500);
        protected Vector2 guiScrollPosition;
        protected bool isMouseOvered = false;

        protected virtual void Awake()
        {
            this.windowRect.width = Mathf.Max(400f, Screen.width * 0.25f);
            this.windowRect.height = Screen.height;
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(this.toggleMenu))
            {
                this.showMenu = !this.showMenu;
            }
        }

        public void AddContent(string category, IGuiDebugger content)
        {
            if (this.contents.ContainsKey(category) == false)
            {
                this.contents.Add(category, new List<IGuiDebugger>());
            }
            this.contents[category].Add(content);
            this.contents[category] = this.contents[category].OrderBy(v => v.Title).ToList();
            content.OnClose += () => this.focusedContent = null;
        }

        public void Open(IGuiDebugger target)
        {
            if (this.focusedContent != null)
            {
                this.focusedContent.Close();
            }
            target.Open();
            this.focusedContent = target;
        }


        protected virtual void OnGUI()
        {
            if (this.showMenu == false)
            {
                return;
            }
            var temp = this.windowRect.Contains(Event.current.mousePosition);
            if(temp != this.isMouseOvered)
            {
                this.isMouseOvered = temp;
                this.OnMouseOverUpdated.Invoke();
            }

            GUI.skin = GuiUiController.Instance.GetSkin();
            this.windowRect = GUILayout.Window(100, this.windowRect, this.DebugWindow, "Debug Menu", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        }

        protected virtual void DebugWindow(int id)
        {
            this.guiScrollPosition = GUILayout.BeginScrollView(this.guiScrollPosition, GUILayout.MinHeight(Screen.height - 100f));
            GUILayout.BeginVertical();

            if (this.focusedContent == null)
            {
                foreach (var item in this.contents)
                {

                    using (var v = new GUILayout.VerticalScope("box"))
                    {
                        GUILayout.Label(item.Key);
                        foreach (var content in item.Value)
                        {
                            if (GUILayout.Button(content.Title))
                            {
                                content.Open();

                                if (this.focusedContent != null && this.focusedContent != content)
                                {
                                    this.focusedContent.Close();
                                }

                                this.focusedContent = content;
                            }
                        }
                    }
                }
            }
            else
            {
                this.focusedContent.ShowGuiContent();
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            GUI.DragWindow();
        }

        private void OnDestroy()
        {
            foreach (var item in this.contents.Values)
            {
                foreach (var cont in item)
                {
                    cont.Dispose();
                }
            }
        }
    }
}