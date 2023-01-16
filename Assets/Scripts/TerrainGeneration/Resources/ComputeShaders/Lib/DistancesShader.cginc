
#ifndef DISTANCES_SHADER_INCLUDED
#define DISTANCES_SHADER_INCLUDED

float SquareBump(float x, float y)
{
    return pow(x, 2) + pow(y, 2);
}

float EuclideanSquare(float x, float y)
{
    return min(1, (pow(x, 2) - pow(y, 2)) / sqrt(2));
}

float Euclidean(float x, float y)
{
    return min(1, sqrt(pow(x, 2) + pow(y, 2)));
}

float Manhattan(float x, float y)
{
    return min(1, abs(x) + abs(y));
}

float Distance(float x, float y, int distanceType)
{
    switch (distanceType)
    {
    case 0:
        return SquareBump(x, y);
    case 1:
        return EuclideanSquare(x, y);
    case 2:
        return Euclidean(x, y);
    case 3:
        return Manhattan(x, y);
    default:
        return 0;
    }
}

#endif