Shader "Hidden/MinimalPost"
{
    Properties
    {
        // Blitterを使用する場合、メインテクスチャは内部的に _BlitTexture という名前で扱われます
        [HideInInspector] _MainTex("Base (RGB)", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline" "RenderType"="Opaque"
        }

        // ポストエフェクトなので深度テストやカリングは不要
        LOD 100
        ZTest Always
        ZWrite Off
        Cull Off

        Pass
        {
            Name "MinimalPostPass"

            HLSLPROGRAM
            // 頂点シェーダーとフラグメントシェーダーの定義
            #pragma vertex Vert
            #pragma fragment Frag

            // URPの標準ライブラリ
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // 重要：Blitterを使用するためのユーティリティ
            // これを読み込むことで、標準的な頂点シェーダー「Vert」が自動的に定義されます
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            // フラグメントシェーダーの入力（VaryingsはBlit.hlslで定義済み）
            float4 Frag(Varyings input) : SV_Target
            {
                // Unityの現在の描画ルール（VRのシングルパスレンダリング等）を考慮したUV座標を取得
                float2 uv = input.texcoord;

                // _BlitTexture は Blitter.BlitTexture によって自動的にバインドされる入力テクスチャ
                // SAMPLE_TEXTURE2D_X は、VR対応（XR）を容易にするためのマクロ
                float4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);

                color.rgb = 1 - color.rgb; // 色を反転

                return color;
            }
            ENDHLSL
        }
    }
    Fallback Off
}
