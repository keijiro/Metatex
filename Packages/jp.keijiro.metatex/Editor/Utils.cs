using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Metatex {

// Texture factory with local default settings
static class Factory
{
    public static Texture2D NewTexture
      (Vector2Int dimensions, bool linear)
      => new Texture2D(dimensions.x, dimensions.y,
                       TextureFormat.RGBA32, true, linear)
         { alphaIsTransparency = true };

    public static RenderTexture NewRenderTexture
      (Vector2Int dimensions, bool linear)
      => new RenderTexture(dimensions.x, dimensions.y, 0,
                           RenderTextureFormat.Default,
                           linear ? RenderTextureReadWrite.Linear :
                                    RenderTextureReadWrite.Default);
}

// Default gradient
static class GradientUtil
{
    public static Gradient Default()
    {
        var colorKeys = new GradientColorKey[]
          { new GradientColorKey(Color.white, 0),
            new GradientColorKey(Color.gray, 1) };

        var alphaKeys = new GradientAlphaKey[]
          { new GradientAlphaKey(1, 0),
            new GradientAlphaKey(0, 1) };

        var grad = new Gradient();
        grad.SetKeys(colorKeys, alphaKeys);

        return grad;
    }
}

// Emoji downloader
static class EmojiDownloader
{
    static readonly string BaseURL
      = "https://github.com/googlefonts/noto-emoji/raw/main/png/512";

    static string NormalizeCode(string code)
    {
        var temp = code.ToLower();
        return temp.StartsWith("u+") ? temp.Substring(2) : temp;
    }

    public static void Get(string code, Texture2D texture, bool compression)
    {
        var filename = $"emoji_u{NormalizeCode(code)}.png";
        var tempFilePath = "Temp/" + filename;

        using var client = new System.Net.WebClient();
        client.DownloadFile($"{BaseURL}/{filename}", tempFilePath);
        var bytes = System.IO.File.ReadAllBytes(tempFilePath);

        var temp = new Texture2D(1, 1);
        ImageConversion.LoadImage(temp, bytes);

        var rt = new RenderTexture(texture.width, texture.height, 0);

        var prevRT = RenderTexture.active;
        Graphics.Blit(temp, rt);

        texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        if (compression) texture.Compress(true);
        texture.Apply(true, true);

        RenderTexture.active = prevRT;

        Object.DestroyImmediate(rt);
        Object.DestroyImmediate(temp);
    }
}

// Simple string label with GUIContent
struct Label
{
    GUIContent _guiContent;

    public static implicit operator GUIContent(Label label)
      => label._guiContent;

    public static implicit operator Label(string text)
      => new Label { _guiContent = new GUIContent(text) };
}

// Auto-scanning serialized property wrapper
struct AutoProperty
{
    SerializedProperty _prop;

    public SerializedProperty Target => _prop;

    public AutoProperty(SerializedProperty prop)
      => _prop = prop;

    public static implicit operator
      SerializedProperty(AutoProperty prop) => prop._prop;

    public static void Scan<T>(T target) where T : UnityEditor.Editor
    {
        var so = target.serializedObject;

        var flags = BindingFlags.Public | BindingFlags.NonPublic;
        flags |= BindingFlags.Instance;

        foreach (var f in typeof(T).GetFields(flags))
            if (f.FieldType == typeof(AutoProperty))
                f.SetValue(target, new AutoProperty(so.FindProperty(f.Name)));
    }
}

} // namespace Metatex
