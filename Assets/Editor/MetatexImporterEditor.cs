using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

namespace Metatex {

[CustomEditor(typeof(MetatexImporter))]
sealed class MetatexImporterEditor : ScriptedImporterEditor
{
    SerializedProperty _dimensions;
    SerializedProperty _generator;
    SerializedProperty _shader;
    SerializedProperty _material;

    public override void OnEnable()
    {
        base.OnEnable();
        _dimensions = serializedObject.FindProperty("_dimensions");
        _generator = serializedObject.FindProperty("_generator");
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
