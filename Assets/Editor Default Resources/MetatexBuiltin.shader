Shader "Hidden/Metatex/Builtin"
{
    Properties
    {
        _Color("Color", Color) = (0, 0, 0, 0)
    }

    CGINCLUDE

#include "UnityCG.cginc"
#include "Packages/jp.keijiro.klak.lineargradient/Shader/LinearGradient.hlsl"

float4 _Color;
LinearGradient _Gradient;

float4 FragmentSolidColor
  (float4 pos : SV_Position, float2 uv : TEXCOORD0) : SV_Target
{
    return _Color;
}

float4 FragmentLinearGradient
  (float4 pos : SV_Position, float2 uv : TEXCOORD0) : SV_Target
{
    float4 s = SampleLinearGradient(_Gradient, uv.x);
#ifndef UNITY_COLORSPACE_GAMMA
    s.rgb = GammaToLinearSpace(s.rgb);
#endif
    return s;
}

float4 FragmentRadialGradient
  (float4 pos : SV_Position, float2 uv : TEXCOORD0) : SV_Target
{
    float l = length(uv - 0.5) / 0.5;
    float4 s = SampleLinearGradient(_Gradient, l);
#ifndef UNITY_COLORSPACE_GAMMA
    s.rgb = GammaToLinearSpace(s.rgb);
#endif
    return s;
}

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment FragmentSolidColor
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment FragmentLinearGradient
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment FragmentRadialGradient
            ENDCG
        }
    }
}
