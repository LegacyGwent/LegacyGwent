using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorMap // store the colors of the title cosmetics
{
        public static readonly Dictionary<string, Color> colormap = new Dictionary<string, Color>
        {
            { "white", Color.blue },
            { "lightblue", new Color(0f,0f,0f,0f) }
        };
}
