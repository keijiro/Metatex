using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Metatex {

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
