using UnityEngine;
using UnityEditor;
#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif

namespace Metatex {

[CustomEditor(typeof(MetatexImporter)), CanEditMultipleObjects]
sealed class MetatexImporterEditor : ScriptedImporterEditor
{
    AutoProperty _dimensions;
    AutoProperty _generator;

    AutoProperty _colormap;
    AutoProperty _color;
    AutoProperty _color2;
    AutoProperty _gradient;
    AutoProperty _scale;
    AutoProperty _codepoint;

    AutoProperty _shader;
    AutoProperty _material;

    AutoProperty _filterMode;
    AutoProperty _wrapMode;
    AutoProperty _anisoLevel;

    AutoProperty _compression;
    AutoProperty _linear;

    static class Labels
    {
        public static Label FirstColor = "First Color";
        public static Label SecondColor = "Second Color";
        public static Label LineColor = "Line Color";
    }

    public override void OnEnable()
    {
        base.OnEnable();
        AutoProperty.Scan(this);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_dimensions.Target);
        EditorGUILayout.PropertyField(_generator.Target);

        switch ((Generator)_generator.Target.enumValueIndex)
        {
            case Generator.Shader:
                EditorGUILayout.PropertyField(_shader.Target);
                break;

            case Generator.Material:
                EditorGUILayout.PropertyField(_material.Target);
                break;

            case Generator.SolidColor:
                EditorGUILayout.PropertyField(_color.Target);
                break;

            case Generator.LinearGradient:
            case Generator.RadialGradient:
                EditorGUILayout.PropertyField(_gradient.Target);
                break;

            case Generator.Colormap:
                EditorGUILayout.PropertyField(_colormap.Target);
                break;

            case Generator.Checkerboard:
                EditorGUILayout.PropertyField(_color.Target, Labels.FirstColor);
                EditorGUILayout.PropertyField(_color2.Target, Labels.SecondColor);
                EditorGUILayout.PropertyField(_scale.Target);
                break;

            case Generator.UVChecker:
                EditorGUILayout.PropertyField(_color.Target, Labels.LineColor);
                EditorGUILayout.PropertyField(_scale.Target);
                break;

            case Generator.Emoji:
                EditorGUILayout.PropertyField(_codepoint.Target);
                break;
        }

        EditorGUILayout.PropertyField(_filterMode.Target);
        EditorGUILayout.PropertyField(_wrapMode.Target);
        EditorGUILayout.PropertyField(_anisoLevel.Target);

        EditorGUILayout.PropertyField(_compression.Target);

        if (QualitySettings.activeColorSpace == ColorSpace.Linear)
            EditorGUILayout.PropertyField(_linear.Target);

        serializedObject.ApplyModifiedProperties();
        ApplyRevertGUI();
    }

    [MenuItem("Assets/Create/Metatex")]
    public static void CreateNewAsset()
      => ProjectWindowUtil.CreateAssetWithContent("New Metatex.metatex", "");
}

} // namespace Metatex
