using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size=16)]
public readonly struct NGuid : IComparable, IComparable<NGuid>, IEquatable<NGuid>
{
    //------------------------------------------------------------

    public static readonly NGuid Empty = new NGuid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0); // could just use 'default' here

    //------------------------------------------------------------

    public readonly uint4 Data;

    //------------------------------------------------------------

    public unsafe NGuid(int a, short b, short c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k)
    {
        uint4 tmp;
        var ptr = &tmp;

        var bPtr = (byte*)ptr;
        var iPtr = (int*)ptr;
        var sPtr = (short*)ptr;
        iPtr[0] = a;

        sPtr[2] = b;
        sPtr[3] = c;

        bPtr[8] = d;
        bPtr[9] = e;

        bPtr[10] = f;
        bPtr[11] = g;
        bPtr[12] = h;
        bPtr[13] = i;
        bPtr[14] = j;
        bPtr[15] = k;

        Data = tmp;
    }

    public NGuid(uint4 data) => Data = data;

    #if !NET_DOTS
    public NGuid(Guid guid) : this(guid.ToByteArray()) {}

    public unsafe NGuid(byte[] bytes)
    {
        uint4 tmp;
        var dstPtr = &tmp;
        fixed (byte* srcPtr = bytes)
        {
            Buffer.MemoryCopy(srcPtr, dstPtr, 16, 16);
        }
        Data = tmp;
    }

    public unsafe NGuid(string hex)
    {
        var a = Convert.ToInt32(hex.Substring(0, 8), 16);

        var b = Convert.ToInt16(hex.Substring(9, 4), 16);
        var c = Convert.ToInt16(hex.Substring(14, 4), 16);

        var d = Convert.ToByte(hex.Substring(19, 2), 16);
        var e = Convert.ToByte(hex.Substring(21, 2), 16);

        var f = Convert.ToByte(hex.Substring(24, 2), 16);
        var g = Convert.ToByte(hex.Substring(26, 2), 16);
        var h = Convert.ToByte(hex.Substring(28, 2), 16);
        var i = Convert.ToByte(hex.Substring(30, 2), 16);
        var j = Convert.ToByte(hex.Substring(32, 2), 16);
        var k = Convert.ToByte(hex.Substring(34, 2), 16);

        uint4 tmp;
        var ptr = &tmp;
        var bPtr = ((byte*)ptr);
        var iPtr = ((int*)ptr);
        var sPtr = ((short*)ptr);
        iPtr[0] = a;

        sPtr[2] = b;
        sPtr[3] = c;

        bPtr[8] = d;
        bPtr[9] = e;

        bPtr[10] = f;
        bPtr[11] = g;
        bPtr[12] = h;
        bPtr[13] = i;
        bPtr[14] = j;
        bPtr[15] = k;

        Data = tmp;
    }

    public static bool TryParseAcceptingNull(string str, out NGuid guid)
    {
        if (str == null)
        {
            guid = default;
            return true;
        }
        else
        {
            return TryParse(str, out guid);
        }
    }

    public static bool TryParse(string str, out NGuid guid)
    {
        bool IsHexChar(char c)
        {
            return ((c >= '0' && c <= '9') ||
                    (c >= 'a' && c <= 'f') ||
                    (c >= 'A' && c <= 'F'));
        }
        guid = default;
        if (str != null
            && str.Length == 36
            && str[8] == '-'
            && str[13] == '-'
            && str[18] == '-'
            && str[23] == '-')
        {
            for (var i = 0; i < 8; i++)
            {
                if (!IsHexChar(str[i])) return false;
            }
            for (var i = 9; i < 13; i++)
            {
                if (!IsHexChar(str[i])) return false;
            }
            for (var i = 14; i < 18; i++)
            {
                if (!IsHexChar(str[i])) return false;
            }
            for (var i = 19; i < 23; i++)
            {
                if (!IsHexChar(str[i])) return false;
            }
            for (var i = 24; i < 36; i++)
            {
                if (!IsHexChar(str[i])) return false;
            }
            guid = new NGuid(str);
            return true;
        }
        return false;
    }

    public static bool TryParseHexString(string str, out NGuid guid)
    {
        if (str.Length != 32)
        {
            guid = default;
            return false;
        }
        else
        {
            try
            {
                var x = Convert.ToUInt32(str.Substring(0, 8), 16);
                var y = Convert.ToUInt32(str.Substring(8, 8), 16);
                var z = Convert.ToUInt32(str.Substring(16, 8), 16);
                var w = Convert.ToUInt32(str.Substring(24, 8), 16);
                guid = new NGuid(new uint4(x, y, z, w));
                return true;
            }
            catch
            {
                guid = default;
                return false;
            }
        }
    }

    #endif

    //------------------------------------------------------------

    #if !NET_DOTS
    public override bool Equals(object obj) => obj is NGuid guid && Equals(guid);
    #endif

    public bool Equals(NGuid other)
    {
        return Data.Equals(other.Data);
    }

    public override int GetHashCode()
    {
        return Data.GetHashCode();
    }

    public static bool operator ==(NGuid left, NGuid right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(NGuid left, NGuid right)
    {
        return !left.Equals(right);
    }

    public int CompareTo(NGuid ng)
    {
        var a = Data;
        var b = ng.Data;
        if (a.x != b.x) return a.x < b.x ? -1 : 1;
        if (a.y != b.y) return a.y < b.y ? -1 : 1;
        if (a.z != b.z) return a.z < b.z ? -1 : 1;
        if (a.w != b.w) return a.w < b.w ? -1 : 1;
        return 0;
    }

    public static unsafe NGuid Merge(NGuid a, NGuid b)
    {
        var data = new uint4(
            a.Data.x ^ b.Data.x,
            a.Data.y ^ b.Data.y,
            a.Data.z ^ b.Data.z,
            a.Data.w ^ b.Data.w);

        // Set up v4 guid required bits
        var ptr = (byte*)&data;
        ptr[7] = (byte)((ptr[7] & 0x0f) | 0x40);
		ptr[8] = (byte)((ptr[8] & 0x3f) | 0x80);

        return new NGuid(data);
    }

    #if !NET_DOTS
    public int CompareTo(Object obj)
    {
        if (obj == null)
        {
            return 1;
        }
        if (!(obj is NGuid))
        {
            throw new ArgumentException("object has to be a NGuid");
        }
        return CompareTo((NGuid)obj);
    }

    public static unsafe explicit operator Guid(NGuid v)
    {
        var tmp = &v.Data;
        var bytes = (byte*)tmp;
        var a = (bytes[3] << 24) | (bytes[2] << 16) | (bytes[1] << 8) | bytes[0];
        var b = (short)((bytes[5] << 8) | bytes[4]);
        var c = (short)((bytes[7] << 8) | bytes[6]);
        var d = bytes[8];
        var e = bytes[9];
        var f = bytes[10];
        var g = bytes[11];
        var h = bytes[12];
        var i = bytes[13];
        var j = bytes[14];
        var k = bytes[15];
        return new Guid(a, b, c, d, e, f, g, h, i, j, k);
    }

    public override unsafe string ToString()
    {
        var tmp = Data;
        var bytes = (byte*)&tmp;
        var iPtr = ((int*)bytes);
        var sPtr = ((short*)bytes);
        return
            iPtr[0].ToString("x8") + "-" +
            sPtr[2].ToString("x4") + "-" +
            sPtr[3].ToString("x4") + "-" +
            bytes[8].ToString("x2") + bytes[9].ToString("x2") + "-" +
            bytes[10].ToString("x2") +
            bytes[11].ToString("x2") +
            bytes[12].ToString("x2") +
            bytes[13].ToString("x2") +
            bytes[14].ToString("x2") +
            bytes[15].ToString("x2");
    }

    public string ToHexString() =>
        Data.x.ToString("x8") +
        Data.y.ToString("x8") +
        Data.z.ToString("x8") +
        Data.w.ToString("x8");

    public unsafe byte[] ToArray()
    {
        var res = new byte[16];
        fixed (byte* ptr = res)
        {
            *((uint4*) ptr) = Data;
        }
        return res;
    }
    #endif
}

