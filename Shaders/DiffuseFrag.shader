Shader "Custom/DiffuseFrag"
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


            //structs
            struct vertexInput
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct vertexOutput
            {
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;

            vertexOutput vert(vertexInput v)
            {
                vertexOutput o;
                o.normal = normalize(mul(unity_ObjectToWorld, float4(v.normal, 0.0)).xyz);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(vertexOutput i) : COLOR
            {
                float4 col = tex2D(_MainTex, i.uv);
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float diffPower = saturate(dot(i.normal, lightDir));
                col.rgb *= diffPower;
                return col;
            }
            ENDCG
        }

    }
    //Falback "Diffuse"
}
