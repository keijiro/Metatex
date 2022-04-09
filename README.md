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
dependencies. Open the Package Manager page in the Project Settings window and
add the following entry to the Scoped Registries list:

- Name: `Keijiro`
- URL: `https://registry.npmjs.com`
- Scope: `jp.keijiro`

![Scoped Registry](https://user-images.githubusercontent.com/343936/162576797-ae39ee00-cb40-4312-aacd-3247077e7fa1.png)

Now you can install the package from My Registries page in the Package Manager
window.

![My Registries](https://user-images.githubusercontent.com/343936/162576825-4a9a443d-62f9-48d3-8a82-a3e80b486f04.png)

[scoped registry]: https://docs.unity3d.com/Manual/upm-scoped.html
