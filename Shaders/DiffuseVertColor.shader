Shader "Unlit/DiffuseVertColor"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            

            struct vertexInput
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct vertexOutput
            {
                float4 position : POSITION;
                float power : COLOR0;
            };

            float4 _Color;

            vertexOutput vert(vertexInput v)
            {
                vertexOutput o;

                o.position = UnityObjectToClipPos(v.vertex);

                float3 normal = normalize(mul(unity_ObjectToWorld, float4(v.normal, 0.0)).xyz);
                half power = max(0, dot(normal, _WorldSpaceLightPos0.xyz));
                o.power = saturate(power);

                return o;
            }

            float4 frag(vertexOutput i) : COLOR
            {
                float4 col = _Color;
                col *= i.power;
                return col;
            }
            ENDCG

        }
    }
}
