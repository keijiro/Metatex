using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

namespace Metatex {

[CustomEditor(typeof(MetatexImporter))]
sealed class MetatexImporterEditor : ScriptedImporterEditor
{
    SerializedProperty _dimensions;
    SerializedProperty _shader;

    public override void OnEnable()
    {
        base.OnEnable();
        _dimensions = serializedObject.FindProperty("_dimensions");
        _shader = serializedObject.FindProperty("_shader");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_dimensions);
        EditorGUILayout.PropertyField(_shader);

        serializedObject.ApplyModifiedProperties();
        ApplyRevertGUI();
    }

    [MenuItem("Assets/Create/Metatex")]
    public static void CreateNewAsset()
      => ProjectWindowUtil.CreateAssetWithContent("New Metatex.metatex", "");
}

} // namespace Metatex
