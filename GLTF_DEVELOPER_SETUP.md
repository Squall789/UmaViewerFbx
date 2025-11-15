# GLTF Export - Developer Setup Instructions

## Overview

This document provides instructions for developers on how to wire up the GLTF export button in the Unity Editor.

## Prerequisites

- Unity 2022.3.62f1 (or compatible version)
- The project should be opened and all packages imported
- The glTFast package should be automatically installed from Packages/manifest.json

## UI Setup Instructions

### 1. Locate the Animation Settings Panel

1. Open the `Assets/Scenes/Version2.unity` scene in Unity
2. In the Hierarchy, find the UI object that contains the Animation Settings panel
3. Look for the object that has the `UISettingsAnimation` component attached

### 2. Add GLTF Export Button

1. In the Animation Settings panel, locate the existing VMD Button
2. Duplicate the VMD Button (right-click → Duplicate or Ctrl+D)
3. Rename the duplicated button to "GLTFButton" or "Export GLTF"
4. Position it next to or below the VMD button

### 3. Configure the Button

1. Select the new GLTF button in the Hierarchy
2. In the Inspector, find the `Button` component
3. Under `On Click ()` events:
   - Click the `+` button to add a new event
   - Drag the `UmaViewerUI` GameObject to the object field
   - In the function dropdown, select `UmaViewerUI → ExportGLTF()`

### 4. Link the Button to UISettingsAnimation

1. Select the GameObject with the `UISettingsAnimation` component
2. In the Inspector, find the `UISettingsAnimation` component
3. Locate the `GLTF Button` field (it should be visible after recompiling)
4. Drag the GLTF button GameObject into this field

### 5. Update Button Text

1. Select the GLTF button
2. Find the child TextMeshProUGUI component (usually named "Text")
3. Set the text to "Export GLTF" or "Export GLB"

## Alternative: Programmatic Setup (Advanced)

If you prefer to set up the button programmatically, you can add code similar to this in a Unity Editor script:

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class GLTFButtonSetup : MonoBehaviour
{
    [MenuItem("Tools/Setup GLTF Export Button")]
    static void SetupGLTFButton()
    {
        // Find the UISettingsAnimation component
        var uiSettings = FindObjectOfType<UISettingsAnimation>();
        if (uiSettings == null)
        {
            Debug.LogError("UISettingsAnimation not found!");
            return;
        }

        // Find or create the button
        var vmdButton = uiSettings.VMDButton;
        if (vmdButton == null)
        {
            Debug.LogError("VMD Button not found!");
            return;
        }

        // Clone the VMD button
        var gltfButton = Instantiate(vmdButton.gameObject, vmdButton.transform.parent);
        gltfButton.name = "GLTFButton";
        
        // Update position
        var rectTransform = gltfButton.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(
            vmdButton.GetComponent<RectTransform>().anchoredPosition.x,
            vmdButton.GetComponent<RectTransform>().anchoredPosition.y - 40
        );

        // Update text
        var text = gltfButton.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = "Export GLTF";
        }

        // Setup button click event
        var button = gltfButton.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {
            UmaViewerUI.Instance.ExportGLTF();
        });

        // Link to UISettingsAnimation
        uiSettings.GLTFButton = button;

        Debug.Log("GLTF Export button created and configured!");
    }
}
#endif
```

Save this script in `Assets/Editor/GLTFButtonSetup.cs`, then use `Tools → Setup GLTF Export Button` from the Unity menu.

## Testing

1. Play the scene in Unity
2. Load a character and animation
3. Click the "Export GLTF" button
4. Check the console for success message
5. Verify the exported file in `UmaViewer_Data/GLTFExports/`

## Troubleshooting

### Button doesn't appear
- Make sure you've recompiled the scripts (Unity should do this automatically)
- Check that the `UISettingsAnimation.cs` file has the `public Button GLTFButton;` field

### Button click does nothing
- Verify the button's `On Click ()` event is properly configured
- Check the Console for any error messages
- Ensure `UmaViewerUI.Instance` is not null

### Export fails
- Make sure glTFast package is installed (check Package Manager)
- Verify the character is loaded (not null)
- Check write permissions for the export directory

## Code References

- `Assets/Scripts/Exporters/GLTFAnimationExporter.cs` - The main exporter class
- `Assets/Scripts/UmaViewerUI.cs` - Contains the `ExportGLTF()` method
- `Assets/Scripts/Settings/UISettingsAnimation.cs` - UI settings class with button reference

## Next Steps

After setup, you can:
1. Test the export with different characters and animations
2. Customize the export settings in `GLTFAnimationExporter.cs`
3. Add additional UI options (format selection, export path, etc.)
4. Extend the exporter to support batch exports
