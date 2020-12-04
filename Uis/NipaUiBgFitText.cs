using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NipaFriends
{

    ////////////////////////////////////////////////////////////////////////////////
    ///<summary>
    /// [ROLE] : 子のテキストの幅に合わせて自身の幅を合わせる
    /// [note] : -
    ///</summary> 
    public class NipaUiBgFitText : MonoBehaviour
    {
        [HideInInspector]
        public Text targetText;
        private RectTransform targetTextRect;
        private RectTransform myRect;
        private ContentSizeFitter contFitter;
        [SerializeField]
        private float margine;
        private void Awake()
        {
            this.myRect = this.GetComponent<RectTransform>();
            this.targetText = this.gameObject.GetComponentInChildren<Text>();
            this.targetTextRect = this.targetText.gameObject.GetComponent<RectTransform>();
            this.contFitter = this.targetText.gameObject.AddComponent<ContentSizeFitter>();
            this.contFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            this.contFitter.enabled = false;
        }

        public void SetText(string cont)
        {
            this.contFitter.enabled = true;
            this.targetText.text = cont;
            NipaCorountine.instance.StartProcess(this.UpdateTextLate());
        }

        private IEnumerator UpdateTextLate()
        {
            yield return null;
            this.myRect.sizeDelta = new Vector2(this.targetTextRect.sizeDelta.x + this.margine * 2, this.myRect.sizeDelta.y);
            this.contFitter.enabled = false;
        }
    }
}