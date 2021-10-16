public class Layout
{
    public LayoutData LayoutData;
    public readonly AssetData[] Assets;

    public Layout(LayoutData data)
    {
        LayoutData = data;
        Assets = new AssetData[data.AssetCount];
    }
}