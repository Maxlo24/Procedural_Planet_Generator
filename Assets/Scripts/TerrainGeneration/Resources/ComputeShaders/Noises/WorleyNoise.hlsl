#ifndef _INCLUDE_WORLEY_NOISE_ 
#define _INCLUDE_WORLEY_NOISE_

#include "Assets/Scripts/TerrainGeneration/Resources/ComputeShaders/Noises/NoiseUtils.hlsl"

float3 dist(float3 x, float3 y, bool manhattanDistance) {
	return manhattanDistance ? abs(x) + abs(y) : (x * x + y * y);
}

float2 wnoise(float2 P, float jitter, bool manhattanDistance) {
	float K = 0.142857142857; // 1/7
	float Ko = 0.428571428571;// 3/7
	float2 Pi = mod289(floor(P));
	float2 Pf = fract(P);
	float3 oi = float3(-1.0, 0.0, 1.0);
	float3 of = float3(-0.5, 0.5, 1.5);
	float3 px = permute(Pi.x + oi);
	float3 p = permute(px.x + Pi.y + oi); // p11, p12, p13
	float3 ox = fract(p * K) - Ko;
	float3 oy = mod(floor(p * K), 7.0) * K - Ko;
	float3 dx = Pf.x + 0.5 + jitter * ox;
	float3 dy = Pf.y - of + jitter * oy;
	float3 d1 = dist(dx, dy, manhattanDistance); // d11, d12 and d13, squared
	p = permute(px.y + Pi.y + oi); // p21, p22, p23
	ox = fract(p * K) - Ko;
	oy = mod(floor(p * K), 7.0) * K - Ko;
	dx = Pf.x - 0.5 + jitter * ox;
	dy = Pf.y - of + jitter * oy;
	float3 d2 = dist(dx, dy, manhattanDistance); // d21, d22 and d23, squared
	p = permute(px.z + Pi.y + oi); // p31, p32, p33
	ox = fract(p * K) - Ko;
	oy = mod7(floor(p * K)) * K - Ko;
	dx = Pf.x - 1.5 + jitter * ox;
	dy = Pf.y - of + jitter * oy;
	float3 d3 = dist(dx, dy, manhattanDistance); // d31, d32 and d33, squared
	// Sort out the two smallest distances (F1, F2)
	float3 d1a = min(d1, d2);
	d2 = max(d1, d2); // Swap to keep candidates for F2
	d2 = min(d2, d3); // neither F1 nor F2 are now in d3
	d1 = min(d1a, d2); // F1 is now in d1
	d2 = max(d1a, d2); // Swap to keep candidates for F2
	d1.xy = (d1.x < d1.y) ? d1.xy : d1.yx; // Swap if smaller
	d1.xz = (d1.x < d1.z) ? d1.xz : d1.zx; // F1 is in d1.x
	d1.yz = min(d1.yz, d2.yz); // F2 is now not in d2.yz
	d1.y = min(d1.y, d1.z); // nor in  d1.z
	d1.y = min(d1.y, d2.x); // F2 is in d1.y, we're done.
	return sqrt(d1.xy);
}
#endif 