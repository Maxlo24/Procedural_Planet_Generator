//
// Noise Shader Library for Unity - https://github.com/keijiro/NoiseShader
//
// Original work (webgl-noise) Copyright (C) 2011 Stefan Gustavson
// Translation and modification was made by Keijiro Takahashi.
//
// This shader is based on the webgl-noise GLSL shader. For further details
// of the original shader, please see the following description from the
// original source code.
//

//
// GLSL textureless classic 3D noise "cnoise",
// with an RSL-style periodic variant "pnoise".
// Author:  Stefan Gustavson (stefan.gustavson@liu.se)
// Version: 2011-10-11
//
// Many thanks to Ian McEwan of Ashima Arts for the
// ideas for permutation and gradient selection.
//
// Copyright (c) 2011 Stefan Gustavson. All rights reserved.
// Distributed under the MIT license. See LICENSE file.
// https://github.com/ashima/webgl-noise
//

#ifndef BASIC_NOISE_INCLUDED
#define BASIC_NOISE_INCLUDED

float noise(float3 p, int noiseType);
float ridge2D(float3 p);
float noise2D(float3 p);
float noise2D(float2 P);
float4 permuteBN(float4 x);
float2 fadeBN(float2 t);
float4 taylorInvSqrtBN(float4 r);
float mod289BN(float x);

float noise(float3 p, int noiseType)
{
    switch (noiseType)
    {
    case 0:
        return noise2D(p);
    case 1:
        return ridge2D(p);
    default:
        return 0;
    }
}

float ridge2D(float3 p)
{
    return 2 * (0.5 - abs(0.5 - noise2D(p.xy)));
}

float noise2D(float3 p)
{
	return noise2D(p.xy);
}

float noise2D(float2 P)
{
    float4 Pi = floor(P.xyxy) + float4(0.0, 0.0, 1.0, 1.0);
    float4 Pf = frac(P.xyxy) - float4(0.0, 0.0, 1.0, 1.0);

    float4 ix = Pi.xzxz;
    float4 iy = Pi.yyww;
    float4 fx = Pf.xzxz;
    float4 fy = Pf.yyww;

    float4 i = permuteBN(permuteBN(ix) + iy);

    float4 gx = frac(i / 41.0) * 2.0 - 1.0;
    float4 gy = abs(gx) - 0.5;
    float4 tx = floor(gx + 0.5);
    gx = gx - tx;

    float2 g00 = float2(gx.x, gy.x);
    float2 g10 = float2(gx.y, gy.y);
    float2 g01 = float2(gx.z, gy.z);
    float2 g11 = float2(gx.w, gy.w);

    float4 norm = taylorInvSqrtBN(float4(dot(g00, g00), dot(g01, g01), dot(g10, g10), dot(g11, g11)));
    g00 *= norm.x;
    g01 *= norm.y;
    g10 *= norm.z;
    g11 *= norm.w;

    float n00 = dot(g00, float2(fx.x, fy.x));
    float n10 = dot(g10, float2(fx.y, fy.y));
    float n01 = dot(g01, float2(fx.z, fy.z));
    float n11 = dot(g11, float2(fx.w, fy.w));

    float2 fade_xy = fadeBN(Pf.xy);
    float2 n_x = lerp(float2(n00, n01), float2(n10, n11), fade_xy.x);
    float n_xy = lerp(n_x.x, n_x.y, fade_xy.y);
    return n_xy;
}

float4 permuteBN(float4 x)
{
    return fmod(34.0 * pow(x, 2) + x, 289.0);
}

float2 fadeBN(float2 t) {
    return 6.0 * pow(t, 5.0) - 15.0 * pow(t, 4.0) + 10.0 * pow(t, 3.0);
}

float4 taylorInvSqrtBN(float4 r) {
    return 1.79284291400159 - 0.85373472095314 * r;
}

#define DIV_289 0.00346020761245674740484429065744

float mod289BN(float x) {
    return x - floor(x * DIV_289) * 289.0;
}
#endif