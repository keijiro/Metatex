#ifndef _METATEX_HSLUV_HLSL_
#define _METATEX_HSLUV_HLSL_

static const float3x3 Hsluv_M = float3x3
  ( 3.240969941904521, -1.537383177570093,   -0.498610760293,
    -0.96924363628087,   1.87596750150772, 0.041555057407175,
    0.055630079696993,  -0.20397695888897, 1.056971514242878);

static const float2 Hsluv_RefUV =
  float2(0.19783000664283, 0.46831999493879);

// CIE LUV constants
static const float Hsluv_Kappa = 903.2962962;
static const float Hsluv_Epsilon = 0.0088564516;

float Hsluv_MaxChromaForLH(float2 lh)
{
    float sub1 = pow(lh.x + 16, 3) / 1560896;
    float sub2 = sub1 > Hsluv_Epsilon ? sub1 : lh.x / Hsluv_Kappa;

    float3 top1   = sub2 * mul(Hsluv_M, float3(284517,       0, -94839));
    float3 top2   = sub2 * mul(Hsluv_M, float3(731718,  769860, 838422));
    float3 bottom = sub2 * mul(Hsluv_M, float3(     0, -126452, 632260));

    float3 mn = 1e+22;

    for (uint t = 0; t < 2; ++t)
    {
        float3 div = rcp(bottom + 126452 * t);
        float3 slope = div * top1;
        float3 inter = div * lh.x * (top2 - 769860 * t);
        float3 len = inter / (sin(lh.y) - slope * cos(lh.y));
        mn = (len < mn && len >= 0) ? len : mn;
    }

    return mn.x < mn.y ? (mn.x < mn.z ? mn.x : mn.z) : 
                         (mn.y < mn.z ? mn.y : mn.z);
}

float3 Hsluv_XyzToRgb(float3 xyz)
{
    float3 rgb = mul(Hsluv_M, xyz);
    #ifdef UNITY_COLORSPACE_GAMMA
    rgb = LinearToGammaSpace(rgb);
    #endif
    return rgb;
}

float Hsluv_LToY(float L) 
{
    return L <= 8 ? L / Hsluv_Kappa : pow((L + 16) / 116, 3);
}

float3 Hsluv_LuvToXyz(float3 luv)
{
    if (luv.x == 0) return 0;
    float2 UV = luv.yz / (13 * luv.x) + Hsluv_RefUV;
    float Y = Hsluv_LToY(luv.x);
    float X = 9 * Y * UV.x / (4 * UV.y);
    float Z = (9 * Y - (X + 15 * Y) * UV.y) / (3 * UV.y);
    return float3(X, Y, Z);
}

float3 Hsluv_LchToLuv(float3 lch)
{
    return lch.xyy * float3(1, cos(lch.z), sin(lch.z));
}

float3 Hsluv_HsluvToLch(float3 hsl)
{
    return hsl.zyx * float3(1, Hsluv_MaxChromaForLH(hsl.zx) / 100, 1);
}

float3 HsluvToRgb(float3 hsl)
{
    float3 lch = Hsluv_HsluvToLch(hsl);
    float3 luv = Hsluv_LchToLuv(lch);
    float3 xyz = Hsluv_LuvToXyz(luv);
    return Hsluv_XyzToRgb(xyz);
}

#endif // _METATEX_HSLUV_HLSL_
