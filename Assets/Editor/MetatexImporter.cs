using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

namespace Metatex {

[ScriptedImporter(1, "metatex")]
public sealed class MetatexImporter : ScriptedImporter
{
    #region Editable attributes

    [SerializeField] Vector2Int _dimensions = new Vector2Int(512, 512);
    [SerializeField] Shader _shader = null;

    #endregion

    #region ScriptedImporter implementation

    public override void OnImportAsset(AssetImportContext context)
    {
        var tex = GenerateTexture();
        if (tex != null) context.AddObjectToAsset("texture", tex);
        context.SetMainObject(tex);
    }

    #endregion

    #region Texture generator implementation

    Texture GenerateTexture()
    {
        if (_shader == null) return null;

        var rt = new RenderTexture(_dimensions.x, _dimensions.y, 0);
        var tex = new Texture2D(_dimensions.x, _dimensions.y);
        var mat = new Material(_shader);

        var prevRT = RenderTexture.active;
        Graphics.Blit(null, rt, mat);

        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        RenderTexture.active = prevRT;

        DestroyImmediate(rt);
        DestroyImmediate(mat);

        return tex;
    }

    #endregion
}

} // namespace Metatex
