public static class NativeHelpers
{
    public static unsafe void DeserializeUnmanagedAndAdvance<T>(ref byte* data, out T val) where T : unmanaged
    {
        val = *((T*) data);
        data += sizeof(T);
    }
}