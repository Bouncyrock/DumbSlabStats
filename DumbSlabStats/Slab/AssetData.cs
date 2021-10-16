using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public readonly struct AssetData
{
    const int BITS_PER_COMPONENT = 18;
    const int BITS_FOR_ROTATION = 5;
    const int ROT_MASK = (1 << BITS_FOR_ROTATION) - 1;
    const int ENCODED_POSITION_MAX_VALUE = (1 << BITS_PER_COMPONENT) - 1;
    const int Y_BITS_OFFSET = BITS_PER_COMPONENT;
    const int Z_BITS_OFFSET = Y_BITS_OFFSET + BITS_PER_COMPONENT;
    const int ROTATION_BITS_OFFSET = Z_BITS_OFFSET + BITS_PER_COMPONENT;
    const int EXTRA_BITS_OFFSET = ROTATION_BITS_OFFSET + BITS_FOR_ROTATION;
    const float DEGREES_PER_ROT_STEP = 360f / 24;

    readonly ulong _encoded;

    // accessors
    public float3 Origin => UnpackPos();
    public byte Rotation => UnpackRot();
    public float RotationDegrees => UnpackRot() * DEGREES_PER_ROT_STEP;
    public byte AdditionalFiveBits => (byte)(_encoded >> EXTRA_BITS_OFFSET);

    AssetData(ulong encoded) => _encoded = encoded;

    public static AssetData Create(float3 pos, byte rotation, byte fiveBits)
    {
        var scaledPos = (int3)math.round(pos * 100);

        #if DEBUG
        if (math.any(scaledPos < 0))
        {
            throw new Exception("scaled origin was negative");
        }
        if (math.any(scaledPos > ENCODED_POSITION_MAX_VALUE))
        {
            throw new Exception("scaled origin out of bounds");
        }
        #endif

        var masked = scaledPos & ENCODED_POSITION_MAX_VALUE;

        var rotMasked = rotation & ROT_MASK;
        
        return new AssetData(
            ((ulong) masked.x)
            | ((ulong) masked.y << Y_BITS_OFFSET)
            | ((ulong) masked.z << Z_BITS_OFFSET)
            | ((ulong) rotMasked << ROTATION_BITS_OFFSET)
            | ((ulong) fiveBits << EXTRA_BITS_OFFSET));
    }

    public float3 UnpackPos()
    {
        var scaledOrigin = new int3(
            (int)(_encoded & ENCODED_POSITION_MAX_VALUE),
            (int)((_encoded >> Y_BITS_OFFSET) & ENCODED_POSITION_MAX_VALUE),
            (int)((_encoded >> Z_BITS_OFFSET) & ENCODED_POSITION_MAX_VALUE));
        return (float3)scaledOrigin / 100f;
    }
    
    public byte UnpackRot() => (byte) ((_encoded >> ROTATION_BITS_OFFSET) & 0b11111);
}