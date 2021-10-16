public struct float3
{
    public float x;
    public float y;
    public float z;

    public float3(float xv, float yv, float zv)
    {
        x = xv;
        y = yv;
        z = zv;
    }
    
    public static float3 operator *(float3 a, float b) => new float3(a.x * b, a.y * b, a.z * b);
    public static float3 operator /(float3 a, float b) => new float3(a.x / b, a.y / b, a.z / b);
    
    public static explicit operator int3(float3 v) => new int3((int) v.x, (int) v.y, (int) v.z);
}