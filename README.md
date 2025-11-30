# Metatex

![photo](https://user-images.githubusercontent.com/343936/157153911-9c396e39-7243-40bc-b8f4-1bebecd2a543.jpg)

**Metatex** is a metadata-only texture asset importer for Unity. It
procedurally generates a texture from metadata at import time.

It currently provides the following texture generators:

- Shader/Material baker
- Solid color
- Linear/radial gradient
- [Colormaps] (Matplotlib colormaps, Turbo, HSLuv, and HSV)
- Checkerboard
- UV checker
- TV test card
- Emoji

[Colormaps]: https://github.com/keijiro/Pugrad

## About Emoji Textures

The Emoji texture generator downloads color emoji assets (image files) from the
[noto-emoji] repository on demand during import, so an internet connection is
required.

You must follow the noto-emoji license terms when using imported assets in your
project. See the repository documentation for details.

[noto-emoji]: https://github.com/googlefonts/noto-emoji

## Related Projects

- [Metamesh] - Surface mesh asset generator
- [Metawire] - Wireframe mesh asset generator

[Metamesh]: https://github.com/keijiro/Metamesh
[Metawire]: https://github.com/keijiro/Metawire

## Installation

You can install the Metatex package (`jp.keijiro.metatex`) via the "Keijiro"
scoped registry using the Unity Package Manager. To add the registry to your
project, follow [these instructions].

[these instructions]:
  https://gist.github.com/keijiro/f8c7e8ff29bfe63d86b888901b82644c
