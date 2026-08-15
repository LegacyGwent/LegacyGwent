using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Builds a small mipmapped copy for cards rendered around 100x125 pixels.
/// Full card inspection keeps the original Addressable sprite. This avoids
/// high-frequency shimmer without increasing every packaged card texture.
/// </summary>
public static class StableMiniCardArt
{
    // High-resolution desktop canvases display the nominal 200px compact card
    // above its authored size. Keep a little sampling headroom there while
    // retaining the smaller cache on memory-constrained mobile players.
    private static int TextureSize => Application.isMobilePlatform ? 256 : 320;
    private static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();

    public static Sprite Get(string artId, Sprite source)
    {
        if (source == null || string.IsNullOrWhiteSpace(artId)) return source;
        if (Cache.TryGetValue(artId, out var cached) && cached != null) return cached;

        var sourceTexture = source.texture;
        sourceTexture.filterMode = FilterMode.Bilinear;
        sourceTexture.wrapMode = TextureWrapMode.Clamp;

        var textureSize = TextureSize;
        var temporary = RenderTexture.GetTemporary(
            textureSize, textureSize, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        temporary.filterMode = FilterMode.Bilinear;
        temporary.wrapMode = TextureWrapMode.Clamp;

        var rect = source.rect;
        var scale = new Vector2(rect.width / sourceTexture.width, rect.height / sourceTexture.height);
        var offset = new Vector2(rect.x / sourceTexture.width, rect.y / sourceTexture.height);
        Graphics.Blit(sourceTexture, temporary, scale, offset);

        var previous = RenderTexture.active;
        RenderTexture.active = temporary;
        var texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, true, false)
        {
            name = $"MiniCard_{artId}",
            filterMode = FilterMode.Trilinear,
            wrapMode = TextureWrapMode.Clamp,
            anisoLevel = 1
        };
        texture.ReadPixels(new Rect(0, 0, textureSize, textureSize), 0, 0, false);
        texture.Apply(true, true);
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(temporary);

        var pivot = new Vector2(source.pivot.x / rect.width, source.pivot.y / rect.height);
        var pixelsPerUnit = source.pixelsPerUnit * textureSize / rect.width;
        var sprite = Sprite.Create(texture, new Rect(0, 0, textureSize, textureSize), pivot,
            pixelsPerUnit, 0, SpriteMeshType.FullRect);
        sprite.name = $"MiniCard_{artId}";
        Cache[artId] = sprite;
        return sprite;
    }
}
