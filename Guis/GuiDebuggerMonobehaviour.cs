using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace NipaFriends.Guis
{
    public abstract class GuiDebuggerMonobehaviour : MonoBehaviour, IGuiDebugger
    {
        public event Action OnClose = delegate { };
        public string Title => this.debugTitle;
        private string debugTitle = "No Name Debug";

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

        public void ForceOpen()
        {
            DebugMenu.Instance.Open(this);
        }

        protected abstract void GuiContent();

        public void Open()
            => this.OnOpening();
        public void Close()
            => this.OnClosing();

        protected virtual void OnOpening()
        {

        }

        protected virtual void OnClosing()
        {

        }

        protected void Register(string category, string title)
        {
            this.debugTitle = title;
            DebugMenu.Instance.AddContent(category, this);
        }

        public void Dispose()
        {
            this.OnClose = null;
        }
    }
}