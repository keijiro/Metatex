using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

namespace Metatex {

[ScriptedImporter(1, "metatex")]
public sealed class MetatexImporter : ScriptedImporter
{
    #region Editable attributes

    [SerializeField] Vector2Int _dimensions = new Vector2Int(512, 512);
    [SerializeField] Generator _generator = Generator.Shader;
    [SerializeField] Color _color = Color.gray;
    [SerializeField] Shader _shader = null;
    [SerializeField] Material _material = null;

    #endregion

    #region ScriptedImporter implementation

    public override void OnImportAsset(AssetImportContext context)
    {
        var texture = GenerateTexture();
        context.AddObjectToAsset("texture", texture);
        context.SetMainObject(texture);
    }

    #endregion

    #region Builtin shader

    static Shader _builtinShader;

    Material BuiltinMaterial => GetBuiltinMaterialSafe();

    Material _builtinMaterial;

    Material GetBuiltinMaterialSafe()
    {
        if (_builtinShader == null)
            _builtinShader = (Shader)EditorGUIUtility.Load("MetatexBuiltin.shader");

        if (_builtinMaterial == null)
            _builtinMaterial = new Material(_builtinShader);

        return _builtinMaterial;
    }

    #endregion

    #region Texture generator implementation

    Texture GenerateTexture()
    {
        var texture = new Texture2D(_dimensions.x, _dimensions.y);

        switch (_generator)
        {
            case Generator.SolidColor:
                BuiltinMaterial.color = _color;
                BakeTexture(BuiltinMaterial, texture);
                break;

            case Generator.Shader:
                BakeTexture(_shader, texture);
                break;

            case Generator.Material:
                BakeTexture(_material, texture);
                break;
        }

        return texture;
    }

    void BakeTexture(Shader shader, Texture2D texture)
    {
        if (shader == null) return;
        var material = new Material(shader);
        BakeTexture(material, texture);
        DestroyImmediate(material);
    }

    void BakeTexture(Material material, Texture2D texture)
    {
        if (material == null) return;

        var rt = new RenderTexture(texture.width, texture.height, 0);

        var prevRT = RenderTexture.active;
        Graphics.Blit(null, rt, material);

        texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        texture.Apply();

        RenderTexture.active = prevRT;

        DestroyImmediate(rt);
    }

    #endregion
}

} // namespace Metatex
