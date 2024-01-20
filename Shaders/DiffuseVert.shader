Shader "Unlit/DiffuseVert"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;

            struct vertexInput
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct vertexOutput
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
                float power : COLOR0;
            };

            vertexOutput vert(vertexInput v)
            {
                vertexOutput o;

                o.position = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                float3 normal = normalize(mul(unity_WorldToObject, float4(v.normal, 0.0)).xyz);
                half power = max(0, dot(normal, _WorldSpaceLightPos0.xyz));
                o.power = saturate(power);

                return o;
            }

            float4 frag(vertexOutput i) : COLOR
            {
                float4 col = tex2D(_MainTex, i.uv);
                col *= i.power;
                return col;
            }
            ENDCG

        }
    }
}
