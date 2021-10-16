using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;

public class SlabV2
{
    const uint MAGIC_HEX = 0xD1CEFACE;
    const ushort CURRENT_VERSION = 2;
    const string MARKDOWN_CODE_BRACE = "```";
    const int MARKDOWN_CODE_BRACE_LEN = 3;
    const int COMPRESSED_SIZE_LIMIT = 1024 * 30;
    const int HEADER_BYTE_SIZE =
        sizeof(uint)      // magic hex
        + sizeof(ushort)  // version
        + sizeof(ushort); // number of keys
    
    public readonly List<Layout> Layouts;

    public SlabV2(List<Layout> layouts) => Layouts = layouts;
    
    public static unsafe SlabV2 DecodeString(string maybeSlabString)
    {
        if (maybeSlabString.Substring(0, MARKDOWN_CODE_BRACE_LEN) == MARKDOWN_CODE_BRACE)
        {
            maybeSlabString = maybeSlabString.Substring(
                MARKDOWN_CODE_BRACE_LEN, maybeSlabString.Length - (MARKDOWN_CODE_BRACE_LEN * 2));
        }

        var decoded = Convert.FromBase64String(maybeSlabString);

        if (decoded.Length > COMPRESSED_SIZE_LIMIT)
        {
            throw new Exception("Compressed data is too large");
        }

        // Decompress
        byte[] decompressedBytes;
        using (var memStream = new MemoryStream(decoded))
        {
            using (var gzipStream = new GZipStream(memStream, CompressionMode.Decompress))
            {
                using (var decompressed = new MemoryStream())
                {
                    gzipStream.CopyTo(decompressed);
                    decompressedBytes = decompressed.ToArray();
                }
            }
        }

        if (decompressedBytes.Length < HEADER_BYTE_SIZE)
        {
            throw new Exception("Header too short");
        }
        
        fixed (byte* dataPtr = decompressedBytes)
        {
            var data = dataPtr;
            
            NativeHelpers.DeserializeUnmanagedAndAdvance(ref data, out uint magicHex);
            if (magicHex != MAGIC_HEX)
            {
                throw new Exception("Invalid hex");
            }
            
            NativeHelpers.DeserializeUnmanagedAndAdvance(ref data, out ushort version);
            if (version != CURRENT_VERSION)
            {
                throw new Exception("Incorrect version");
            }

            NativeHelpers.DeserializeUnmanagedAndAdvance(ref data, out ushort layoutsCount);
            NativeHelpers.DeserializeUnmanagedAndAdvance(ref data, out ushort creatureCount);
            if (creatureCount != 0)
            {
                throw new Exception("creature count in v2 is always zero");
            }

            var ah0 = sizeof(NGuid);
            var bop = sizeof(LayoutData);
            var bip = sizeof(AssetData);

            var layouts = new List<Layout>(layoutsCount);
            for (var i = 0; i < layoutsCount; i++)
            {
                NativeHelpers.DeserializeUnmanagedAndAdvance(ref data, out LayoutData layoutData);
                layouts.Add(new Layout(layoutData));
            }
            
            for (var i = 0; i < layoutsCount; i++)
            {
                var layout = layouts[i];
                var assets = layout.Assets;
                var assetLen = assets.Length;
                for (var j = 0; j < assetLen; j++)
                {
                    NativeHelpers.DeserializeUnmanagedAndAdvance(ref data, out AssetData assetData);
                    layout.Assets[j] = assetData;
                }
            }

            return new SlabV2(layouts);
        }
    }
}