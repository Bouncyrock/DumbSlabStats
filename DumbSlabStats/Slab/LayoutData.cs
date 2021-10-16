public readonly struct LayoutData
{
    public readonly NGuid AssetKindId;
    public readonly ushort AssetCount;

    public LayoutData(NGuid assetKindId, ushort assetCount)
    {
        AssetKindId = assetKindId;
        AssetCount = assetCount;
    }
}