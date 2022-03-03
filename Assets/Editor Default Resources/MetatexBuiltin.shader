Shader "Hidden/Metatex/Builtin"
{
    Properties
    {
        _Color("Color", Color) = (0, 0, 0, 0)
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

            float4 _Color;

            float4 Fragment(float4 pos : SV_Position,
                            float2 uv : TEXCOORD0) : SV_Target
            {
                return _Color;
            }

            ENDCG
        }
    }
}
