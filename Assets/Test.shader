Shader "Metatex Test"
{
    Properties
    {
        _XFreq("X Frequency", Vector) = (1, 1, 1, 1)
        _YFreq("Y Frequency", Vector) = (1, 1, 1, 1)
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

            float4 _XFreq, _YFreq;

            float4 Fragment(float4 pos : SV_Position,
                            float2 uv : TEXCOORD0) : SV_Target
            {
                float3 phi = _XFreq.xyz * uv.x + _YFreq.xyz * uv.y;
                phi += _XFreq.w + _YFreq.w;
                return float4(sin(phi) * 0.5 + 0.5, 1);
            }

            ENDCG
        }
    }
}
