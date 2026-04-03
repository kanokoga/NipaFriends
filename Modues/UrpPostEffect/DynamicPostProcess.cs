using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

namespace NipaFriends.Modules.UrpPostProcess
{
    [RequireComponent(typeof(Camera))]
    public class DynamicPostProcess : MonoBehaviour
    {
        [SerializeField] protected Material material;
        private readonly MinimalPass _minimalPass = new();
        protected Camera _camera;

        protected virtual void OnEnable()
        {
            this._camera = this.GetComponent<Camera>();
            // カメラのレンダリング開始イベントに登録
            RenderPipelineManager.beginCameraRendering += this.OnBeginCamera;
        }

        protected virtual void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= this.OnBeginCamera;
        }

        protected virtual void OnBeginCamera(ScriptableRenderContext context, Camera cam)
        {
            // アタッチされているカメラ以外には適用しない
            if(cam != this._camera || this.material == null)
            {
                return;
            }

            // URPのレンダラーを取得
            var additionalData = cam.GetComponent<UniversalAdditionalCameraData>();
            if(additionalData != null && additionalData.scriptableRenderer != null)
            {
                this._minimalPass.Setup(this.material);
                // 動的にパスをキューに入れる
                additionalData.scriptableRenderer.EnqueuePass(this._minimalPass);
            }
        }

        // --- Render Graph パスの定義 ---
        private class MinimalPass : ScriptableRenderPass
        {
            private Material _material;

            // 実行フェーズに必要なデータを運ぶ箱
            private class PassData
            {
                public TextureHandle source;
                public Material material;
            }

            public void Setup(Material mat)
            {
                this._material = mat;
                // ポストプロセスの直後に実行
                this.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                var resourceData = frameData.Get<UniversalResourceData>();
                var sourceTexture = resourceData.activeColorTexture;

                if(!sourceTexture.IsValid() || this._material == null)
                {
                    return;
                }

                // 1. 一時的なテクスチャの作成
                var desc = renderGraph.GetTextureDesc(sourceTexture);
                desc.name = "_DynamicPostTemp";
                desc.depthBufferBits = 0;
                var tempTexture = renderGraph.CreateTexture(desc);

                // 2. 描画パスの構築
                using(var builder = renderGraph.AddRasterRenderPass<PassData>("DynamicPostPass", out var passData))
                {
                    passData.source = sourceTexture;
                    passData.material = this._material;

                    // 読み書きの宣言
                    builder.UseTexture(passData.source, AccessFlags.Read);
                    builder.SetRenderAttachment(tempTexture, 0);

                    // カリングされないように設定
                    builder.AllowPassCulling(false);

                    // 3. 実行（Static ラムダ）
                    builder.SetRenderFunc(static (PassData data, RasterGraphContext context) =>
                    {
                        // URP標準のBlit。_BlitTextureという名前でシェーダーに渡される
                        Blitter.BlitTexture(context.cmd, data.source, Vector2.one, data.material, 0);
                    });
                }

                // 4. 結果をメインテクスチャに書き戻す
                renderGraph.AddBlitPass(tempTexture, sourceTexture, Vector2.one, Vector2.zero);
            }
        }
    }
}
