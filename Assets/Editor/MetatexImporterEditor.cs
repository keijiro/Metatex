using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

namespace Metatex {

[CustomEditor(typeof(MetatexImporter))]
sealed class MetatexImporterEditor : ScriptedImporterEditor
{
    SerializedProperty _dimensions;
    SerializedProperty _generator;
    SerializedProperty _color;
    SerializedProperty _gradient;
    SerializedProperty _shader;
    SerializedProperty _material;

    public override void OnEnable()
    {
        base.OnEnable();
        _dimensions = serializedObject.FindProperty("_dimensions");
        _generator = serializedObject.FindProperty("_generator");
        _color = serializedObject.FindProperty("_color");
        _gradient = serializedObject.FindProperty("_gradient");
        _shader = serializedObject.FindProperty("_shader");
        _material = serializedObject.FindProperty("_material");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_dimensions);
        EditorGUILayout.PropertyField(_generator);

        switch ((Generator)_generator.enumValueIndex)
        {
            case Generator.SolidColor:
                EditorGUILayout.PropertyField(_color);
                break;

            case Generator.LinearGradient:
            case Generator.RadialGradient:
                EditorGUILayout.PropertyField(_gradient);
                break;

            case Generator.Shader:
                EditorGUILayout.PropertyField(_shader);
                break;

            case Generator.Material:
                EditorGUILayout.PropertyField(_material);
                break;
        }

        serializedObject.ApplyModifiedProperties();
        ApplyRevertGUI();
    }

    [MenuItem("Assets/Create/Metatex")]
    public static void CreateNewAsset()
      => ProjectWindowUtil.CreateAssetWithContent("New Metatex.metatex", "");
}

} // namespace Metatex
