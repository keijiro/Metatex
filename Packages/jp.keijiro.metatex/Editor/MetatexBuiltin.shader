Shader "Hidden/Metatex/Builtin"
{
    Properties
    {
        _Color("", Color) = (0, 0, 0, 0)
        _Color2("", Color) = (0, 0, 0, 0)
    }

    CGINCLUDE

#pragma exclude_renderers gles

#include "UnityCG.cginc"
#include "Packages/jp.keijiro.klak.lineargradient/Shader/LinearGradient.hlsl"
#include "Hsluv.hlsl"
#include "Colormap.hlsl"

float4 _Color, _Color2;
LinearGradient _Gradient;
float2 _Scale, _Dimensions;
float _FloatParam;

// Hue to RGB convertion
half3 HueToRGB(half h)
{
    h = saturate(h);
    half r = abs(h * 6 - 3) - 1;
    half g = 2 - abs(h * 6 - 2);
    half b = 2 - abs(h * 6 - 4);
    half3 rgb = saturate(half3(r, g, b));
    return rgb;
}

// Common vertex shader
void Vertex
  (float4 inPos : POSITION, float2 inUV : TEXCOORD0,
   out float4 outPos : SV_Position, out float2 outUV : TEXCOORD0)
{
    outPos = UnityObjectToClipPos(inPos);
    outUV = inUV;
}

// Pass 0: Solid Color
float4 FragmentSolidColor
  (float4 pos : SV_Position, float2 uv : TEXCOORD0) : SV_Target
{
    return _Color;
}

// Pass 1: Linear Gradient
float4 FragmentLinearGradient
  (float4 pos : SV_Position, float2 uv : TEXCOORD0) : SV_Target
{
    float4 s = SampleLinearGradient(_Gradient, uv.x);
    #ifndef UNITY_COLORSPACE_GAMMA
    s.rgb = GammaToLinearSpace(s.rgb);
    #endif
    return s;
}

// Pass 2: Radial Gradient
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

// Pass 3: Colormap (HSV)
float4 FragmentSpectrum
  (float4 pos : SV_Position, float2 uv : TEXCOORD0) : SV_Target
{
    float3 rgb = HueToRGB(uv.x);
    #ifndef UNITY_COLORSPACE_GAMMA
    rgb = GammaToLinearSpace(rgb);
    #endif
    return float4(rgb, 1);
}

// Pass 4: Colormap (Hsluv)
float4 FragmentHsluv
  (float4 pos : SV_Position, float2 uv : TEXCOORD0) : SV_Target
{
    float3 rgb = HsluvToRgb(float3(uv.x * UNITY_PI * 2, 100, 50));
    return float4(rgb, 1);
}

// Pass 5: Colormap (Turbo)
float4 FragmentColormapTurbo
  (float4 pos : SV_Position, float2 uv : TEXCOORD0) : SV_Target
{
    float3 rgb = Colormap_Calculate(Colormap_Turbo1, Colormap_Turbo2, uv.x);
    return float4(rgb, 1);
}

// Pass 6: Colormap (Viridis)
float4 FragmentColormapViridis
  (float4 pos : SV_Position, float2 uv : TEXCOORD0) : SV_Target
{
    float3 rgb = Colormap_Calculate(Colormap_Viridis1, Colormap_Viridis2, uv.x);
    return float4(rgb, 1);
}

// Pass 7: Colormap (Plasma)
float4 FragmentColormapPlasma
  (float4 pos : SV_Position, float2 uv : TEXCOORD0) : SV_Target
{
    float3 rgb = Colormap_Calculate(Colormap_Plasma1, Colormap_Plasma2, uv.x);
    return float4(rgb, 1);
}

// Pass 8: Colormap (Magma)
float4 FragmentColormapMagma
  (float4 pos : SV_Position, float2 uv : TEXCOORD0) : SV_Target
{
    float3 rgb = Colormap_Calculate(Colormap_Magma1, Colormap_Magma2, uv.x);
    return float4(rgb, 1);
}

// Pass 9: Colormap (Inferno)
float4 FragmentColormapInferno
  (float4 pos : SV_Position, float2 uv : TEXCOORD0) : SV_Target
{
    float3 rgb = Colormap_Calculate(Colormap_Inferno1, Colormap_Inferno2, uv.x);
    return float4(rgb, 1);
}

// Pass 10: Checkerboard
float4 FragmentCheckerboard
  (float4 pos : SV_Position, float2 uv : TEXCOORD0) : SV_Target
{
    bool2 f = frac(uv * _Scale) > 0.5;
    return f.x ^ f.y ? _Color : _Color2;
}

// Pass 11: UV Checker
float4 FragmentUVChecker
  (float4 pos : SV_Position, float2 uv : TEXCOORD0) : SV_Target
{
    // Frequency
    float2 freq = _Scale * 8;

    // Cell color
    float p1 = frac(dot(floor(uv * freq) / freq, 1));
    float3 rgb = HueToRGB(frac(lerp(1.6, 0.7, p1)));    // H
    rgb = lerp(rgb, 1, 0.5);                            // S
    rgb *= 0.35 + sin(p1 * UNITY_PI) * 0.4;             // V

    // Grid lines
    float2 p2 = frac(uv * freq);
    p2 = min(p2, 1 - p2);
    p2 = p2 / freq * _Dimensions;
    rgb += any(p2 < 2) * _Color.rgb * _Color.a;

    // Crosshairs
    float2 p3 = abs(frac(uv * freq) - 0.5);
    bool mask = all(p3 < 0.2);
    p3 = p3 / freq * _Dimensions;
    rgb += mask * any(p3 < 2) * _Color.rgb * _Color.a;

    #ifndef UNITY_COLORSPACE_GAMMA
    rgb = GammaToLinearSpace(rgb);
    #endif

    return float4(rgb, 1);
}

// Pass 12: TV Test Card
float4 FragmentTestCard
  (float4 pos : SV_Position, float2 uv : TEXCOORD0) : SV_Target
{
    float scale = 27 / _Dimensions.y;           // Grid scale
    float2 p0 = (uv - 0.5) * _Dimensions.xy;    // Position (pixel)
    float2 p1 = p0 * scale;                     // Position (half grid)
    float2 p2 = p1 / 2 - 0.5;                   // Position (grid)

    // Size of inner area
    float aspect = _Dimensions.x / _Dimensions.y;
    float2 area = float2(floor(6.5 * aspect) * 2 + 1, 13);

    // Crosshair and grid lines
    float2 ch = abs(p0);
    float2 grid = (1 - abs(frac(p2) - 0.5) * 2) / scale;
    float c1 = min(min(ch.x, ch.y), min(grid.x, grid.y)) < 1 ? 1 : 0.5;

    // Outer area checker
    float2 checker = frac(floor(p2) / 2) * 2;
    if (any(abs(p1) > area)) c1 = abs(checker.x - checker.y);

    float corner = sqrt(8) - length(abs(p1) - area + 4); // Corner circles
    float circle = 12 - length(p1);                      // Big center circle
    float mask = saturate(circle / scale);               // Center circls mask

    // Grayscale bars
    float bar1 = saturate(p1.y < 5 ? floor(p1.x / 4 + 3) / 5 : p1.x / 16 + 0.5);
    if (abs(5 - p1.y) < 4 * mask) c1 = bar1;

    // Basic color bars
    float3 bar2 = HueToRGB((p1.y > -5 ? floor(p1.x / 4) / 6 : p1.x / 16) + 0.5);
    float3 rgb = abs(-5 - p1.y) < 4 * mask ? bar2 : saturate(c1);

    // Circle lines
    rgb = lerp(rgb, 1, saturate(1.5 - abs(max(circle, corner)) / scale));

    #ifndef UNITY_COLORSPACE_GAMMA
    rgb = GammaToLinearSpace(rgb);
    #endif

    return float4(rgb, 1);
}

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentSolidColor
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentLinearGradient
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentRadialGradient
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentSpectrum
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentHsluv
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentColormapTurbo
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentColormapViridis
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentColormapPlasma
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentColormapMagma
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentColormapInferno
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentCheckerboard
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentUVChecker
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentTestCard
            ENDCG
        }
    }
}
