using System;
using UnityEngine;

namespace NipaFriends
{
    public class MPB : IDisposable
    {
        private Renderer renderer;
        private MaterialPropertyBlock mpb;

        public MPB(Renderer renderer)
        {
            this.renderer = renderer;
            this.mpb = new MaterialPropertyBlock();
            this.renderer.SetPropertyBlock(this.mpb);
        }

        public void SetColor(string name, Color color)
        {
            this.mpb.SetColor(name, color);
            this.renderer.SetPropertyBlock(this.mpb);
        }

        public void SetFloat(string name, float value)
        {
            this.mpb.SetFloat(name, value);
            this.renderer.SetPropertyBlock(this.mpb);
        }

        public void SetInt(string name, int value)
        {
            this.mpb.SetInt(name, value);
            this.renderer.SetPropertyBlock(this.mpb);
        }

        public void SetVector(string name, Vector4 value)
        {
            this.mpb.SetVector(name, value);
            this.renderer.SetPropertyBlock(this.mpb);
        }

        public void Dispose()
        {
        }
    }
}
