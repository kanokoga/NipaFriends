Shader "Custom/DrawMeshEdgeURP"
{
    Properties
    {
        _EdgeColor("Edge Color", Color) = (1, 1, 1, 1)
        _EdgeThickness("Edge Thickness", Range(0.001, 0.1)) = 0.01
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }
        LOD 200

        Pass
        {
            Name "EdgePass"

            // URPではHLSLPROGRAMを使用します
            HLSLPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            // URPの標準ライブラリをインクルード
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            // CBUFFERで定義することでSRP Batcherに対応させます
            CBUFFER_START(UnityPerMaterial)
                half4 _EdgeColor;
                float _EdgeThickness;
            CBUFFER_END

            Varyings vert(Attributes v)
            {
                Varyings o;
                // オブジェクト空間からクリップ空間へ変換
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                return o;
            }

            [maxvertexcount(18)]
            void geom(triangle Varyings input[3], inout TriangleStream<Varyings> triStream)
            {
                for (uint i = 0; i < 3; i++)
                {
                    float4 start = input[i].positionHCS;
                    float4 end = input[(i + 1) % 3].positionHCS;

                    // クリップ空間での方向を計算（w除算によるパースペクティブ補正を考慮）
                    float2 dir = normalize((end.xy / end.w) - (start.xy / start.w));
                    // 法線方向（90度回転）
                    float2 normal = float2(-dir.y, dir.x);

                    // 厚みのオフセット計算
                    float4 offset = float4(normal * _EdgeThickness, 0, 0);

                    Varyings oStart, oEnd, oStartMoved, oEndMoved;

                    oStart.positionHCS = start;
                    oEnd.positionHCS = end;
                    // start.w / end.w を掛けることで、遠近感に合わせた太さを維持します
                    oStartMoved.positionHCS = start + offset * start.w;
                    oEndMoved.positionHCS = end + offset * end.w;

                    // 三角形1枚目
                    triStream.Append(oStart);
                    triStream.Append(oEnd);
                    triStream.Append(oStartMoved);

                    // 三角形2枚目
                    triStream.Append(oStartMoved);
                    triStream.Append(oEnd); // 頂点順序を最適化（元コードの順序を維持）
                    triStream.Append(oEndMoved);

                    triStream.RestartStrip();
                }
            }

            half4 frag(Varyings i) : SV_Target
            {
                return _EdgeColor;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
