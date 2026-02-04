using UnityEngine;
#if ROSETTA_UI
using RosettaUI;
#endif

namespace NipaFriends
{
    public abstract class RosettaUIMenuWindow : MonoBehaviour
    {
#if ROSETTA_UI
        [SerializeField] protected KeyCode toggleKeyCode = KeyCode.D;
        protected RosettaUIRoot rossetaUiRoot;
        protected Element element;

        protected virtual void Start()
        {
            this.rossetaUiRoot = FindFirstObjectByType<RosettaUIRoot>();
            this.rossetaUiRoot.enabled = false;
            this.rossetaUiRoot.BuildOnEnable(() => this.element = this.CreateElement());
        }

        protected virtual void Update()
        {
            if(Input.GetKeyDown(this.toggleKeyCode))
            {
                this.rossetaUiRoot.enabled = !this.rossetaUiRoot.enabled;
            }
        }

        protected virtual Element CreateElement()
        {
            return UI.Window($"{Application.productName}({Application.version})",
                this.CreateMenuContents()
            );
        }

        protected abstract Element CreateMenuContents();
        // UI.Column(
        // UI.WindowLauncher("dummy", typeof(dummy))

        protected virtual void OnDestroy()
        {
            if(this.element != null)
            {
                this.element.DetachView();
                this.element = null;
            }
        }
#endif
    }
}
