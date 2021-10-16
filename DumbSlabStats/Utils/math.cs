using System;

static class math
{
    public static float3 round(float3 vec) => new float3(MathF.Floor(vec.x), MathF.Floor(vec.y), MathF.Floor(vec.z));
    public static bool any(bool3 vec) => vec.x || vec.y || vec.z;
}