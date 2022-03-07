using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using Klak.Chromatics;

namespace Metatex {

[ScriptedImporter(1, "metatex")]
public sealed class MetatexImporter : ScriptedImporter
{
    #region Editable attributes

    [SerializeField] Vector2Int _dimensions = new Vector2Int(512, 512);
    [SerializeField] Generator _generator = Generator.Shader;
    [SerializeField] Colormap _colormap = Colormap.Hsv;
    [SerializeField] Color _color = Color.gray;
    [SerializeField] Color _color2 = Color.white;
    [SerializeField] Gradient _gradient = null;
    [SerializeField] Vector2 _scale = new Vector2(1, 1);
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

    static readonly string BuiltinShaderPath =
        "Packages/jp.keijiro.metatex/Editor/MetatexBuiltin.shader";

    Material BuiltinMaterial => GetBuiltinMaterialSafe();

    Material _builtinMaterial;

    Material GetBuiltinMaterialSafe()
    {
        if (_builtinShader == null)
            _builtinShader = (Shader)EditorGUIUtility.Load(BuiltinShaderPath);

        if (_builtinMaterial == null)
            _builtinMaterial = new Material(_builtinShader);

        return _builtinMaterial;
    }

    void UpdateBuiltinMaterial()
    {
        BuiltinMaterial.color = _color;
        BuiltinMaterial.SetColor("_Color2", _color2);
        BuiltinMaterial.SetLinearGradient("_Gradient", _gradient);
        BuiltinMaterial.SetVector("_Scale", _scale);
        BuiltinMaterial.SetVector("_Dimensions", (Vector2)_dimensions);
    }

    int GetGeneratorPass()
    {
        if (_generator < Generator.Colormap)
            return (int)_generator - (int)Generator.SolidColor;

        if (_generator == Generator.Colormap)
            return (int)_colormap + 3; // 3: Colormap (HSV)

        return (int)_generator - (int)Generator.Checkerboard + 10;
    }

    #endregion

    #region Texture generator implementation

    Texture GenerateTexture()
    {
        UpdateBuiltinMaterial();

        var texture = new Texture2D(_dimensions.x, _dimensions.y);

        switch (_generator)
        {
            case Generator.Shader:
                BakeTexture(_shader, texture);
                break;

            case Generator.Material:
                BakeTexture(_material, texture);
                break;

            default:
                BakeTexture(BuiltinMaterial, texture, GetGeneratorPass());
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

    void BakeTexture(Material material, Texture2D texture, int pass = 0)
    {
        if (material == null) return;

        var rt = new RenderTexture(texture.width, texture.height, 0);

        var prevRT = RenderTexture.active;
        Graphics.Blit(null, rt, material, pass);

        texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        texture.Apply();

        RenderTexture.active = prevRT;

        DestroyImmediate(rt);
    }

    #endregion
}

} // namespace Metatex
