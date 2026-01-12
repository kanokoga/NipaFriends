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

        public void SetFloat(int shaderPropId, float value)
        {
            this.mpb.SetFloat(shaderPropId, value);
            this.renderer.SetPropertyBlock(this.mpb);
        }

        public void SetColor(int shaderPropId, Color value)
        {
            this.mpb.SetColor(shaderPropId, value);
            this.renderer.SetPropertyBlock(this.mpb);
        }

        public void SetVector(int shaderPropId, Vector4 value)
        {
            this.mpb.SetVector(shaderPropId, value);
            this.renderer.SetPropertyBlock(this.mpb);
        }

        public void SetInt(int shaderPropId, int value)
        {
            this.mpb.SetInt(shaderPropId, value);
            this.renderer.SetPropertyBlock(this.mpb);
        }


        public void Dispose()
        {
        }
    }
}
