using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NipaFriends
{
    public class MaterialController : MonoBehaviour
    {
        public bool HasInit { get; private set; } = false;
        public MaterialPropertyBlock Mpb => this.mpb;

        private Renderer renderer;
        private MaterialPropertyBlock mpb;

        private void Awake()
        {
            this.mpb = new MaterialPropertyBlock();
            this.renderer = this.GetComponent<Renderer>();
            this.HasInit = true;
        }

        public void SetColor(Color c)
        {
            if(this.HasInit == false)
            {
                this.mpb = new MaterialPropertyBlock();
                this.renderer = this.GetComponent<Renderer>();
                this.HasInit = true;
            }
            this.mpb.SetColor("_Color", c);
            this.renderer.SetPropertyBlock(this.mpb);
        }

        public void SetMPB()
        {
            this.renderer.SetPropertyBlock(this.mpb);
        }
    }

}
