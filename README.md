Metatex
=======

![photo](https://user-images.githubusercontent.com/343936/157153911-9c396e39-7243-40bc-b8f4-1bebecd2a543.jpg)

**Metatex** is a metadata-only texture asset importer for Unity. It
procedurally generates a texture from metadata at import-time.

It provides the following texture generators at the moment:

- Shader/Material baker
- Solid color
- Linear/radial gradient
- [Colormaps] (Matplotlib colormaps, Turbo, HSLuv and HSV)
- Checkerboard
- UV checker
- TV test card
- Emoji

About Emoji Textures
--------------------

The Emoji texture generator downloads color emoji assets (image files) from
the [noto-emoji] repository in an on-demand fashion. It requires an internet
connection on importing.

Note that you should follow the noto-emoji license terms when using imported
assets in your project. See the repository documentation for details.

[noto-emoji]: https://github.com/googlefonts/noto-emoji

[Colormaps]: https://github.com/keijiro/Pugrad

Related Project
---------------

- [Metamesh] - Metadata-only mesh asset importer

[Metamesh]: https://github.com/keijiro/Metamesh

How To Install
--------------

This package uses the [scoped registry] feature to resolve package
dependencies. Add the following lines to the manifest file
(`Packages/manifest.json`).

[scoped registry]: https://docs.unity3d.com/Manual/upm-scoped.html

To the `scopedRegistries` section:

```
{
  "name": "Keijiro",
  "url": "https://registry.npmjs.com",
  "scopes": [ "jp.keijiro" ]
}
```

To the `dependencies` section:

```
"jp.keijiro.metatex": "1.0.2"
```

After the changes, the manifest file should look like:

```
{
  "scopedRegistries": [
    {
      "name": "Keijiro",
      "url": "https://registry.npmjs.com",
      "scopes": [ "jp.keijiro" ]
    }
  ],
  "dependencies": {
    "jp.keijiro.metatex": "1.0.2",
    ...
```
