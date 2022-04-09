Shader "Metatex Test"
{
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM

            #include "UnityCG.cginc"

            #pragma vertex vert_img
            #pragma fragment Fragment

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
