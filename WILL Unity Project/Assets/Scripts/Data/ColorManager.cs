using UnityEngine;

public static class ColorManager
{
    public enum HighlightColor
    {
        Blue = 0x78bfe3,
        Green = 0x00a6a6,
        Yellow = 0xefca08,
        Orange = 0xf49f0a,
        Red = 0xd63230
    }

    public enum CharacterColor
    {
        ChinaPink = 0xdb6b98,
        LavenderFlorel = 0xa77acd,
        VioletBlueCrayola = 0x6a70c8,
        FieryRose = 0xeb5c6c,
    }

    public enum OutcomeColor
    {
        SalmonPink = 0xF3919B,
        Jasmine = 0xFCE388,
        Black = 0x000000
    }

    public enum EdgeColor
    {
        MintGreen = 0xAAF683,
        OrangeYellowCrayola = 0xFFD97D,
        VividTangerine = 0xFF9B85,
        Black = 0x000000
    }

    public static Color GetColor(System.Enum value)
    {
        int color = System.Convert.ToInt32(value);
        return new Color32((byte)((color >> 16) & 255), (byte)((color >> 8) & 255), (byte)(color & 255), 255);
    }
}