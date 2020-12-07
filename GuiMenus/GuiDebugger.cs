using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace NipaFriends.Guis
{
    public abstract class GuiDebugger : IGuiDebugger
    {
        public event Action OnClose = delegate { };
        public string Title => this.debugTitle;
        protected bool DebugMode => this.debug;
        private string debugTitle = "No Name Debug";
        private bool debug = false;

        public void ShowGuiContent()
        {
            if (GUILayout.Button("<<", GUILayout.ExpandWidth(false)))
            {
                this.OnClosing();
                this.OnClose.Invoke();
            }
            using (var v = new GUILayout.VerticalScope("box"))
            {
                GUILayout.Label(this.debugTitle);
                this.GuiContent();
            }
        }

        protected abstract void GuiContent();

        public void Open()
            => this.OnOpening();
        public void Close()
            => this.OnClosing();

        public virtual void OnOpening()
        {
            this.debug = true;
        }

        protected virtual void OnClosing()
        {
            this.debug = false;
        }

        protected void Register(string category, string title)
        {
            this.debugTitle = title;
            DebugMenu.Instance.AddContent(category, this);
        }

        public virtual void Dispose()
        {
            this.OnClose = null;
        }

        protected string Now => DateTime.Now.ToString("HH:mm:ss");
    }
}