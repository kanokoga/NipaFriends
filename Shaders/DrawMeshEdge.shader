Shader "Custom/DrawMeshEdge"
{
    Properties
    {
        _EdgeColor("Edge Color", Color) = (1, 1, 1, 1)
        _EdgeThickness("Edge Thickness", Range(0.001, 0.1)) = 1.0
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2g
            {
                float4 vertex : POSITION;
            };

            struct g2f
            {
                float4 vertex : POSITION;
            };

            struct fragOut
            {
                float4 color : SV_Target;
            };

            float4 _EdgeColor;
            float _EdgeThickness;

            v2g vert(appdata v)
            {
                v2g o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            [maxvertexcount(18)]
            void geom(triangle v2g input[3], inout TriangleStream<g2f> triStream)
            {
                for (uint i = 0; i < 3; i++)
                {
                    float4 start = input[i].vertex;
                    float4 end = input[(i + 1) % 3].vertex;
                    float2 dir = normalize(end.xy - start.xy);
                    dir = float2(-dir.y, dir.x);

                    float4 startMoved = start + float4(dir, 0, 0) * start.w * _EdgeThickness;
                    float4 endMoved = end + float4(dir, 0, 0) * end.w * _EdgeThickness;

                    g2f oStart;
                    oStart.vertex = start;
                    g2f oEnd;
                    oEnd.vertex = end;
                    g2f oStartMoved;
                    oStartMoved.vertex = startMoved;
                    g2f oEndMoved;
                    oEndMoved.vertex = endMoved;

                    triStream.Append(oStart);
                    triStream.Append(oEnd);
                    triStream.Append(oStartMoved);

                    triStream.Append(oStartMoved);
                    triStream.Append(oEndMoved);
                    triStream.Append(oEnd);
                }
            }

            fragOut frag(g2f i)
            {
                fragOut o;
                o.color = _EdgeColor;
                return o;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}