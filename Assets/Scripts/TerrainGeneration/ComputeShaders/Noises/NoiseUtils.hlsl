#ifndef _INCLUDE_NOISEUTILS_
#define _INCLUDE_NOISEUTILS_

float2 fract(float2 x)
{
	return x - floor(x);
}

float3 fract(float3 x)
{
	return x - floor(x);
}

float4 mod(float4 x, float4 y)
{
  return x - y * floor(x / y);
}

float3 mod(float3 x, float3 y)
{
  return x - y * floor(x / y);
}

float3 mod(float3 x, float y)
{
	return x - y * floor(x / y);
}

float2 mod289(float2 x)
{
    return x - floor(x / 289.0) * 289.0;
}

float3 mod289(float3 x)
{
  return x - floor(x / 289.0) * 289.0;
}

float4 mod289(float4 x)
{
  return x - floor(x / 289.0) * 289.0;
}

float2 mod7(float2 x)
{
    return x - floor(x / 7.0) * 7.0;
}

float3 mod7(float3 x)
{
	return x - floor(x / 7.0) * 7.0;
}

float4 mod7(float4 x)
{
	return x - floor(x / 7.0) * 7.0;
}

float4 permute(float4 x)
{
  return mod289(((x*34.0)+1.0)*x);
}

float3 permute(float3 x)
{
    return mod289((x * 34.0 + 1.0) * x);
}

float4 taylorInvSqrt(float4 r)
{
  return (float4)1.79284291400159 - r * 0.85373472095314;
}

float3 taylorInvSqrt(float3 r)
{
    return 1.79284291400159 - 0.85373472095314 * r;
}

float3 fade(float3 t) {
  return t*t*t*(t*(t*6.0-15.0)+10.0);
}

float2 fade(float2 t) {
  return t*t*t*(t*(t*6.0-15.0)+10.0);
}


float rand3dTo1d(float3 value, float3 dotDir = float3(12.9898, 78.233, 37.719)){
    //make value smaller to avoid artefacts
    float3 smallValue = sin(value);
    //get scalar value from 3d vector
    float random = dot(smallValue, dotDir);
    //make value more random by making it bigger and then taking the factional part
    random = frac(sin(random) * 143758.5453);
    return random;
}

float rand2dTo1d(float2 value, float2 dotDir = float2(12.9898, 78.233)){
    float2 smallValue = sin(value);
    float random = dot(smallValue, dotDir);
    random = frac(sin(random) * 143758.5453);
    return random;
}

float rand1dTo1d(float3 value, float mutator = 0.546){
	float random = frac(sin(value + mutator) * 143758.5453);
	return random;
}

//to 2d functions

float2 rand3dTo2d(float3 value){
    return float2(
        rand3dTo1d(value, float3(12.989, 78.233, 37.719)),
        rand3dTo1d(value, float3(39.346, 11.135, 83.155))
    );
}

float2 rand2dTo2d(float2 value){
    return float2(
        rand2dTo1d(value, float2(12.989, 78.233)),
        rand2dTo1d(value, float2(39.346, 11.135))
    );
}

float2 rand1dTo2d(float value){
    return float2(
        rand2dTo1d(value, 3.9812),
        rand2dTo1d(value, 7.1536)
    );
}

//to 3d functions

float3 rand3dTo3d(float3 value){
    return float3(
        rand3dTo1d(value, float3(12.989, 78.233, 37.719)),
        rand3dTo1d(value, float3(39.346, 11.135, 83.155)),
        rand3dTo1d(value, float3(73.156, 52.235, 09.151))
    );
}

float3 rand2dTo3d(float2 value){
    return float3(
        rand2dTo1d(value, float2(12.989, 78.233)),
        rand2dTo1d(value, float2(39.346, 11.135)),
        rand2dTo1d(value, float2(73.156, 52.235))
    );
}

float3 rand1dTo3d(float value){
    return float3(
        rand1dTo1d(value, 3.9812),
        rand1dTo1d(value, 7.1536),
        rand1dTo1d(value, 5.7241)
    );
}

float hash(float p)
{
    float x = p * 98102.5453;

    return -1 + 2 * frac(sin(x));
}

float2 hash(float2 p)
{
    float x = dot(p, float2(165.244, 492.128));
    float y = dot(p, float2(382.763, 234.567));

    return -1 + 2 * frac(sin(float2(x, y)) * 98102.5453123);
}

float3 hash(float3 p)
{
    p = float3(dot(p, float3(1234.1, 442.66, 94.2)),    // x
        dot(p, float3(92.12, 639.221, 1.234)),    // y
        dot(p, float3(98.124, 103.83, 55.928)));  // z

    return -1 + 2 * frac(sin(p) * 98102.5453123);
}

float4 hash(float4 p)
{
    p = float4(dot(p, float4(1234.1, 442.66, 94.2, 56.98)),    // x
        dot(p, float4(92.12, 639.221, 1.234, 89.025)),    // y
        dot(p, float4(98.124, 773.83, 55.928, 4.99)),    // z
        dot(p, float4(23.46, 105.1, 200.79, 73.5)));  // w

    return -1 + 2 * frac(sin(p) * 98102.5453123);
}

float2 quintic(float p)
{
    return p * p * p * (p * (6 * p - 15) + 10);
}

float2 quintic(float2 p)
{
    return p * p * p * (p * (6 * p - 15) + 10);
}

float3 quintic(float3 p)
{
    return p * p * p * (p * (6 * p - 15) + 10);
}

float4 quintic(float4 p)
{
    return p * p * p * (p * (6 * p - 15) + 10);
}

inline float4 remap(float4 n, float o, float p, float a, float b)
{
    return a.xxxx + (b.xxxx - a.xxxx) * (n - o.xxxx) / (p.xxxx - o.xxxx);
}

#endif