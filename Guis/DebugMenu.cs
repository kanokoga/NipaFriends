using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NipaFriends.Guis
{
    public class DebugMenu : SingletonMonoBehaviour<DebugMenu>
    {
        [SerializeField] protected KeyCode toggleMenu = KeyCode.F1;
        protected bool showMenu = false;
        protected Dictionary<string, List<IGuiDebugger>> contents = new Dictionary<string, List<IGuiDebugger>>();
        protected IGuiDebugger focusedContent = null;
        protected Rect dubugWindow = new Rect(0, 0, 600, 500);
        protected Vector2 guiScrollPosition;

        private void Awake()
        {
            this.dubugWindow.width = Mathf.Max(400f, Screen.width * 0.25f);
            this.dubugWindow.height = Screen.height;
        }

        private void Update()
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



        private void OnGUI()
        {
            if (this.showMenu == false)
            {
                return;
            }
            GUI.skin = GuiUiController.Instance.GetSkin();
            this.dubugWindow = GUILayout.Window(100, this.dubugWindow, this.DebugWindow, "Debug Menu", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        }

        protected virtual void DebugWindow(int id)
        {
            this.guiScrollPosition = GUILayout.BeginScrollView(guiScrollPosition, GUILayout.MinHeight(Screen.height - 200f));
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