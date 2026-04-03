Shader "PostProcess/ArrowView"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off ZTest Always Blend Off Cull Off

        Pass
        {
            Name "ArrowViewPass"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            struct ArrowData
            {
                float2 start;
                float2 end;
                float4 color;
                float thickness;
                float headSize;
            };

            StructuredBuffer<ArrowData> _ArrowDataBuffer;
            int _ArrowCount;



            // 線分への最短距離（アスペクト補正済み空間用）
            float distanceToSegment(float2 p, float2 a, float2 b)
            {
                float2 pa = p - a, ba = b - a;
                float h = saturate(dot(pa, ba) / dot(ba, ba));
                return length(pa - ba * h);
            }

            // 三角形判定（アスペクト補正済み空間用）
            float isInsideTriangle(float2 p, float2 p1, float2 p2, float2 p3)
            {
                float2 v0 = p3 - p1, v1 = p2 - p1, v2 = p - p1;
                float det = (v0.x * v1.y - v0.y * v1.x);
                float u = (v0.x * v2.y - v0.y * v2.x) / det;
                float v = (v2.x * v1.y - v2.y * v1.x) / det;
                return (u >= 0) && (v >= 0) && (u + v <= 1);
            }

            half4 Frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float4 sceneColor = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, input.texcoord);

                // アスペクト比を取得 (横 / 縦)
                float aspect = _ScreenSize.x / _ScreenSize.y;

                // 現在のピクセルUVをアスペクト補正（横を伸ばす）
                float2 uv = input.texcoord;
                float2 p = uv * float2(aspect, 1.0);

                float4 finalColor = sceneColor;

                for (int i = 0; i < _ArrowCount; i++)
                {
                    ArrowData arrow = _ArrowDataBuffer[i];

                    // 始点と終点も同じ空間に変換
                    float2 start = arrow.start * float2(aspect, 1.0);
                    float2 end = arrow.end * float2(aspect, 1.0);

                    float2 dir = end - start;
                    float len = length(dir);
                    if (len < 0.0001) continue;

                    float2 n_dir = dir / len;
                    // 正しい垂直ベクトル (-y, x)
                    float2 perp = float2(-n_dir.y, n_dir.x);

                    // 1. 線分の描画
                    float dist = distanceToSegment(p, start, end);
                    if (dist < arrow.thickness)
                    {
                        finalColor = lerp(finalColor, arrow.color, arrow.color.a);
                    }

                    // 2. 矢印の頭（三角形）
                    float2 p1 = end;
                    float2 p2 = end - n_dir * arrow.headSize + perp * (arrow.headSize * 0.5);
                    float2 p3 = end - n_dir * arrow.headSize - perp * (arrow.headSize * 0.5);

                    if (isInsideTriangle(p, p1, p2, p3) > 0.5)
                    {
                        finalColor = lerp(finalColor, arrow.color, arrow.color.a);
                    }
                }

                return finalColor;
            }
            ENDHLSL
        }
    }
}
