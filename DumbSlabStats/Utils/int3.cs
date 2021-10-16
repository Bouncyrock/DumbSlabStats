public struct int3
{
    public int x;
    public int y;
    public int z;

    public int3(int xv, int yv, int zv)
    {
        x = xv;
        y = yv;
        z = zv;
    }
    
    public static bool3 operator <(int3 a, int3 b) => new bool3(a.x < b.x, a.y < b.y, a.z < b.z);
    public static bool3 operator >(int3 a, int3 b) => new bool3(a.x > b.x, a.y > b.y, a.z > b.z);
    
    public static bool3 operator <(int3 a, int b) => new bool3(a.x < b, a.y < b, a.z < b);
    public static bool3 operator >(int3 a, int b) => new bool3(a.x > b, a.y > b, a.z > b);
    
    public static int3 operator &(int3 a, int b) => new int3(a.x & b, a.y & b, a.z & b);
    
    public static explicit operator float3(int3 v) => new float3(v.x, v.y, v.z);
}