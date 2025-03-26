using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorMap // store the colors of the title cosmetics
{
        public static readonly Dictionary<string, Color> colormap = new Dictionary<string, Color>
        {
            { "white", Color.white }, // Cardsmith and rank titles
            { "darkyellow", new Color(0.729411765f,0.549019608f,0.121568627f,1f) }, // season 1
            { "emerald", new Color(0.431372549f,0.57254902f,0.298039216f,1f) }, // season 2
            { "orange", new Color(0.764705882f,0.490196078f,0.22745098f,1f) }, // season 3
            { "lightblue", new Color(0.243137255f,0.725490196f,0.82745098f,1f) }, // season 4
            { "blue", new Color(0.490196078f,0.611764706f,0.823529412f,1f) }, // season 5
            { "yellow", new Color(0.776470588f,0.752941176f,0.321568627f,1f) }, // season 6
            { "lightgreen", new Color(0.403921569f,0.537254902f,0.152941176f,1f) }, // scoiatael title
            { "purple", new Color(0.349019608f,0.164705882f,0.68627451f,1f) }, // skellige title
            { "red", new Color(0.8f,0.125490196f,0.125490196f,1f) }, // monster and midwinter title
            { "nrblue", new Color(0.37254902f,0.62745098f,0.91372549f,1f) }, // nr title
            { "darkgreen", new Color(0.074509804f,0.545098039f,0.439215686f,1f) }, // pioneer
        };
}
