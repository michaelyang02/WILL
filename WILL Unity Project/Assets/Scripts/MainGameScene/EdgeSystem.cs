using System.Collections.Generic;
using UnityEngine;

public class EdgeSystem
{
    public enum EdgeColor
    {
        MintGreen = 0xAAF683,
        OrangeYellowCrayola = 0xFFD97D,
        VividTangerine = 0xFF9B85,
        White = 0xFFFFFF,
        Black = 0x000000
    }

    public enum EdgeType
    {
        Children,
        Companion = EdgeColor.VividTangerine,
        Enable = EdgeColor.White,
        Disable = EdgeColor.Black
    }

    public struct EdgeData
    {
        public int startSquareIndex { get; set; }
        public int endSquareIndex { get; set; }
        public EdgeType edgeType { get; set; }

        public Color GetColor()
        {
            int color = (int)edgeType;
            return new Color32((byte)((color >> 16) & 255), (byte)((color >> 8) & 255), (byte)(color & 255), 255);
        }
    }

    
}