Shader "Metatex Test"
{
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM

            #include "UnityCG.cginc"

            #pragma vertex Vertex
            #pragma fragment Fragment

            void Vertex
              (float4 inPos : POSITION, float2 inUV : TEXCOORD0,
               out float4 outPos : SV_Position, out float2 outUV : TEXCOORD0)
            {
                outPos = UnityObjectToClipPos(inPos);
                outUV = inUV;
            }

            float4 Fragment(float4 pos : SV_Position,
                            float2 uv : TEXCOORD0) : SV_Target
            {
                float z = cos(length(uv - 0.5) * 30) * 8;
                float2 xy = -float2(ddx(z), ddy(z));
                return float4((xy + 1) / 2, 0, 1);
            }

            ENDCG
        }
    }
}
