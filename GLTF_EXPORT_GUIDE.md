# GLTF/GLB Animation Export

## Overview

The GLTF/GLB export feature allows you to export Uma Musume character models with their current animation state to the industry-standard GLTF or GLB format. This provides a more convenient alternative to recording animations as VDB files.

## Features

- Export character models with animations in GLTF (.gltf) or GLB (.glb) format
- Includes skinned meshes, bones, and animation data
- Compatible with most 3D software and game engines (Blender, Unity, Unreal Engine, etc.)
- Simple one-click export process

## How to Use

### Quick Export

1. Load a character in the viewer
2. Select the animation you want to export
3. Click the "Export GLTF" button in the Animation Settings panel
4. The file will be automatically exported to `UmaViewer_Data/GLTFExports/`

### Export Location

By default, GLTF/GLB files are saved to:
```
<UmaViewer Installation>/UmaViewer_Data/GLTFExports/
```

Files are named with the character name and timestamp:
```
<CharacterName>_YYYYMMDD_HHMMSS.glb
```

## Format Information

### GLB (Binary GLTF)
- Default export format
- Single binary file containing all assets
- Recommended for most use cases
- Smaller file size and easier to share

### GLTF (Text GLTF)
- Available as an alternative format
- JSON-based format with separate asset files
- Easier to inspect and modify manually

## Compatibility

The exported GLTF/GLB files should work in:
- Blender 2.8+
- Unity 2020.1+ (with glTFast package)
- Unreal Engine 4.27+
- Three.js and other web 3D frameworks
- Most 3D modeling and animation software

## Technical Details

The exporter uses Unity's glTFast package (com.unity.cloud.gltfast v6.1.0) to ensure compatibility with the GLTF 2.0 specification.

### What's Exported

- Skinned meshes
- Bone hierarchy
- Materials and textures
- Current animation state
- Morph targets (blend shapes)

### Limitations

- Only exports the current animation clip loaded in the viewer
- Does not export multiple animations in a single file
- Camera and lighting are not included

## Troubleshooting

### Export Failed
- Make sure you have a normal (non-mini) character loaded
- Ensure the animation is loaded and playing
- Check that you have write permissions to the export directory

### Missing Animations
- The exporter captures the current state of the model
- Make sure the animation is loaded before exporting

### File Not Found
- Check the UmaViewer_Data/GLTFExports/ folder in your installation directory
- Look for the success message after export completes

## Comparison with Other Export Formats

| Feature | GLTF/GLB | VMD | PMX |
|---------|----------|-----|-----|
| Format Type | Industry Standard | MMD Specific | MMD Specific |
| Animation Export | ✓ | ✓ | x |
| Model Export | ✓ | x | ✓ |
| Software Support | Universal | MMD/MMM | MMD/MMM |
| File Size | Medium | Small | Small |
| Ease of Use | Very Easy | Medium | Medium |

## Credits

This feature uses the Unity glTFast package by Unity Technologies.
