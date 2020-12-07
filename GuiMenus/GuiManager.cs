using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NipaFriends.Guis
{
    public class GuiManager : SingletonMonoBehaviour<GuiManager>
    {
        [SerializeField] GUISkin skin;
        float scale = 1f;
        int fontDefaultSize;


        public GUISkin GetSkin()
        {
            return this.skin;
        }

        public void SetUiScale(float scale)
        {
            this.SetUiSIze(14f * scale);
            this.SetFontSize(Mathf.RoundToInt(this.fontDefaultSize * scale));
            this.scale = scale;
        }

        public float GetUiScale()
        {
            return this.scale;
        }

        void Awake()
        {
            this.skin.toggle.clipping = TextClipping.Overflow;
            this.fontDefaultSize = this.skin.font.fontSize;
        }

        void SetFontSize(int size)
        {
            this.skin.box.fontSize =
            this.skin.label.fontSize =
            this.skin.button.fontSize =
            this.skin.box.fontSize =
            this.skin.horizontalScrollbar.fontSize =
            this.skin.horizontalScrollbarLeftButton.fontSize =
            this.skin.horizontalScrollbarRightButton.fontSize =
            this.skin.horizontalScrollbarThumb.fontSize =
            this.skin.horizontalSlider.fontSize =
            this.skin.horizontalSliderThumb.fontSize =
            this.skin.label.fontSize =
            this.skin.textArea.fontSize =
            this.skin.textField.fontSize =
            this.skin.toggle.fontSize =
            this.skin.verticalScrollbar.fontSize =
            this.skin.verticalScrollbarDownButton.fontSize =
            this.skin.verticalScrollbarThumb.fontSize =
            this.skin.verticalScrollbarUpButton.fontSize =
            this.skin.verticalSlider.fontSize =
            this.skin.verticalSliderThumb.fontSize =
            size;
        }


        void SetUiSIze(float size = 0f)
        {
            this.skin.horizontalScrollbar.fixedHeight =
            this.skin.horizontalScrollbarThumb.fixedHeight =
            this.skin.horizontalSlider.fixedHeight =
            this.skin.horizontalSliderThumb.fixedHeight =
            this.skin.verticalScrollbar.fixedWidth =
            this.skin.verticalSlider.fixedWidth =
            this.skin.verticalSliderThumb.fixedWidth = Mathf.Max(size, 14f);
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }
}