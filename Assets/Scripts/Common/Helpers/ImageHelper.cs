using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common.Helpers
{
    public static class ImageHelper
    {
        public static void SetActiveIcon(Image image)
        {
            Color color = image.color;
            color.a = 1.0f;
            image.color = color;
        }

        public static void SetInactiveIcon(Image image)
        {
            int alpha255 = 100;
            float alphaNormalized = alpha255 / 255f;

            Color color = image.color;
            color.a = alphaNormalized;
            image.color = color;
        }
    }
}
