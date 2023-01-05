#ifndef LIB_UTILS_INCLUDED
#define LIB_UTILS_INCLUDED

float Remap(float In, float2 InMinMax, float2 OutMinMax)
{
	return OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

float Ridge(float value)
{
	return -2.0*abs(value)+0.9;
}

#endif