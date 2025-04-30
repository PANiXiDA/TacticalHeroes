using UnityEngine;

namespace Assets.Scripts.Common.Helpers
{
    public static class ColorHelper
    {
        public static Color ToColor(this uint rgb24)
        {
            byte r = (byte)((rgb24 >> 16) & 0xFF);
            byte g = (byte)((rgb24 >> 8) & 0xFF);
            byte b = (byte)(rgb24 & 0xFF);
            return new Color32(r, g, b, 255);
        }
    }
}
