using UnityEngine;
using UnityEditor;
#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif
using Klak.Chromatics;

namespace Metatex {

[ScriptedImporter(1, "metatex")]
public sealed class MetatexImporter : ScriptedImporter
{
    #region Editable attributes

    [SerializeField] Vector2Int _dimensions = new Vector2Int(512, 512);
    [SerializeField] Generator _generator = Generator.Checkerboard;

    [SerializeField] Colormap _colormap = Colormap.Hsv;
    [SerializeField] Color _color = Color.gray;
    [SerializeField] Color _color2 = Color.white;
    [SerializeField] Gradient _gradient = GradientUtil.Default();
    [SerializeField] Vector2 _scale = new Vector2(1, 1);
    [SerializeField] string _codepoint = "1f600";

    [SerializeField] Shader _shader = null;
    [SerializeField] Material _material = null;

    [SerializeField] TextureWrapMode _wrapMode = TextureWrapMode.Repeat;
    [SerializeField] FilterMode _filterMode = FilterMode.Trilinear;
    [SerializeField, Range(0, 16)] int _anisoLevel = 4;

    [SerializeField] bool _compression = true;
    [SerializeField] bool _linear = false;

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

        var texture = Factory.NewTexture(_dimensions, _linear);
        texture.filterMode = _filterMode;
        texture.wrapMode = _wrapMode;
        texture.anisoLevel = _anisoLevel;

        switch (_generator)
        {
            case Generator.Shader:
                BakeTexture(_shader, texture);
                break;

            case Generator.Material:
                BakeTexture(_material, texture);
                break;

            case Generator.Emoji:
                EmojiDownloader.Get(_codepoint, texture, _compression);
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

        var rt = Factory.NewRenderTexture(_dimensions, _linear);
        var prevRT = RenderTexture.active;
        Graphics.Blit(null, rt, material, pass);

        texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        if (_compression) texture.Compress(true);
        texture.Apply(true, true);

        RenderTexture.active = prevRT;

        DestroyImmediate(rt);
    }

    #endregion
}

} // namespace Metatex
