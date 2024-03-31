using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NipaFriends
{
    public class CameraMRTBlit : MonoBehaviour
    {
        private static readonly int PropDepthTexture = Shader.PropertyToID("_DepthTexture");
        [SerializeField] private Camera camera;
        [SerializeField] private RenderTexture renderTextureColor;
        [SerializeField] private RenderTexture renderTextureDepth;
        public RenderTexture final;
        [SerializeField] private Material outputMaterial;

        private void Awake()
        {
            var resolution = new Vector2Int(Screen.width, Screen.height);
            this.renderTextureColor = new RenderTexture(resolution.x, resolution.y, 0, RenderTextureFormat.ARGB32);
            this.renderTextureColor.Create();

            this.renderTextureDepth = new RenderTexture(resolution.x, resolution.y, 24, RenderTextureFormat.Depth);
            this.renderTextureDepth.Create();

            this.camera.SetTargetBuffers(this.renderTextureColor.colorBuffer, this.renderTextureDepth.depthBuffer);


            this.final = new RenderTexture(resolution.x, resolution.y, 0, RenderTextureFormat.ARGB32);
            this.final.Create();
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            this.outputMaterial.SetTexture(PropDepthTexture, this.renderTextureDepth);
            Graphics.Blit(this.renderTextureColor, this.final, this.outputMaterial);
            Graphics.Blit(source, destination);
            // * output as normal *
            //  Graphics.Blit(this.renderTextureColor, destination);
        }

        private void OnDestroy()
        {
            this.renderTextureColor.Release();
            this.renderTextureDepth.Release();
            Destroy(this.renderTextureColor);
            Destroy(this.renderTextureDepth);
        }
    }
}
