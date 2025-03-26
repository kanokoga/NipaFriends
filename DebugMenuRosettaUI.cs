using UnityEngine;
#if ROSETTA_UI
using PrefsGUI;
using RosettaUI;
#endif
namespace NipaFriends
{
    public abstract class DebugMenuRosettaUI : MonoBehaviour
    {
#if ROSETTA_UI
        public KeyCode toggleKeyCode = KeyCode.D;
        public char toggleKey = 'd';
        private RosettaUIRoot _root;
        private Element _element;


        protected virtual void Start()
        {
            this._root = FindObjectOfType<RosettaUIRoot>();
            this._root.enabled = false;
            this._root.BuildOnEnable(() => this._element = CreateElement());

            //  Keyboard.current.onTextInput += OnTextInput;
        }

        protected virtual void OnDestroy()
        {
            if(this._element != null)
            {
                this._element.DetachView();
                this._element = null;
            }

            //  Keyboard.current.onTextInput -= OnTextInput;
        }


        private void OnTextInput(char text)
        {
            if(text == this.toggleKey)
            {
                this._root.enabled = !this._root.enabled;
                if(!this._root.enabled)
                {
                    Prefs.Save();
                }
            }
        }

        protected virtual void Update()
        {
            if(Input.GetKeyDown(this.toggleKeyCode))
            {
                this._root.enabled = !this._root.enabled;
                if(!this._root.enabled)
                {
                    Prefs.Save();
                }
            }
        }

        protected virtual Element CreateElement()
        {
            return UI.Window($"ver[{Application.version}]",
                this.CreateMenuContents()
            );
        }

        protected abstract Element CreateMenuContents();
        // UI.WindowLauncher("dummy", typeof(dummy))
#endif
    }
}
