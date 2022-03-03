Shader "Hidden/Metatex Test"
{
    Properties
    {
        _XRepeat("X Repeat", Float) = 1
        _YRepeat("Y Repeat", Float) = 1
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM

            #include "UnityCG.cginc"

            #pragma vertex vert_img
            #pragma fragment Fragment

            float _XRepeat, _YRepeat;

            float4 Fragment(float4 pos : SV_Position,
                            float2 uv : TEXCOORD0) : SV_Target
            {
                return float4(frac(uv.xy * float2(_XRepeat, _YRepeat)), 0, 1);
            }

            ENDCG
        }
    }
}
