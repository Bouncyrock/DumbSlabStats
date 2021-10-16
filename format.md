# Slab Format V2

We will use the following types:

- u8: 8bit unsigned integer (byte in c#)
- u16: 16bit unsigned integer (ushort in c#)
- u32: 32bit unsigned integer (uint in c#)
- u64: 64bit unsigned integer (ulong in c#)
- uuid: 128bit id

## String Processing

Given a string which potentially contains a slab:

- Trim whitespace from both ends
- The string may have tripple backquotes (```) at the beginning and end, remove those if present
- Base64 decode the string

You should now have a binary array no more than 30720 byte in length

## Decompress

The data is gzip compressed. Decompress the array to get the slab data.

For C# `new GZipStream(new MemoryStream(arr), CompressionMode.Decompress)` gives you a stream you can get the decompressed data from.

## Read

### Header

> The first u32 will always be 0xD1CEFACE
>
> the next u16 is the version of slab format. Will be 2 for this version

### Counts

> the next u16 is the number of `Layout`s. We will refer to this as `layoutCount`
>
> the next u16 is the number of creatures and will always be zero in a v2 slab

### Layouts

Each Layout is 20 bytes in size and has the following form:

```
Layout
{
    uuid AssetKindId;
    u16 AssetCount;
    u16 _RESERVED_;
}
```

> The next (`layoutCount` * 20) bytes is packed `Layout`s data

### Assets

The data of assets of the same kind are packed contiguously. The layouts describe the order of the asset kinds and the number of assets of each kind.

Each Asset is 8 bytes in size (`u64`) and the data is packed within it as follows:

```
 most significant bits -- least significant bits
| 5 bits | 5 bits | 18 bits | 18 bits | 18 bits |
| unused |   rot  | scaledZ | scaledY | scaledX |
```

The position of the asset is always >= 0 and is obtained as follows:

```
x = scaledX / 100.0;
y = scaledY / 100.0;
z = scaledZ / 100.0;
```

The rot value is a unsigned integer greater than or equal to zero and less than 24.

To convert from rotation steps to degrees you do so as follows:

```
degrees = rot * 15.0;
```

The remaining data after layouts is as follows:

> (`layout[0].AssetCount` * 8) bytes of data for assets of kind `layout[0].AssetKindId`
>
> (`layout[1].AssetCount` * 8) bytes of data for assets of kind `layout[1].AssetKindId`
>
> ..
>
> (`layout[layoutCount-1].AssetCount` * 8) bytes of data for assets of kind `layout[layoutCount-1].AssetKindId`
