#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

/// <summary>
/// Editor utility to help set up the GLTF export button in the UI
/// </summary>
public class GLTFExportSetup : EditorWindow
{
    [MenuItem("Tools/UmaViewer/Setup GLTF Export Button")]
    static void ShowWindow()
    {
        GetWindow<GLTFExportSetup>("GLTF Export Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("GLTF Export Button Setup", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "This tool will help you set up the GLTF export button in the Animation Settings UI.\n\n" +
            "Make sure you have the Version2 scene open before proceeding.",
            MessageType.Info
        );
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Auto-Setup GLTF Button", GUILayout.Height(40)))
        {
            SetupGLTFButton();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Manual Setup:\n" +
            "1. Duplicate the VMD button in the Animation Settings panel\n" +
            "2. Rename it to 'GLTFButton'\n" +
            "3. Configure its OnClick() to call UmaViewerUI.ExportGLTF()\n" +
            "4. Link it to UISettingsAnimation.GLTFButton field",
            MessageType.None
        );
    }

    static void SetupGLTFButton()
    {
        // Find the UISettingsAnimation component
        var uiSettings = FindObjectOfType<UISettingsAnimation>();
        if (uiSettings == null)
        {
            EditorUtility.DisplayDialog("Error", "UISettingsAnimation component not found!\n\nMake sure the Version2 scene is open.", "OK");
            return;
        }

        // Check if VMD button exists
        if (uiSettings.VMDButton == null)
        {
            EditorUtility.DisplayDialog("Error", "VMD Button not found in UISettingsAnimation!\n\nPlease check the scene setup.", "OK");
            return;
        }

        // Check if GLTF button already exists
        if (uiSettings.GLTFButton != null)
        {
            bool overwrite = EditorUtility.DisplayDialog(
                "GLTF Button Exists", 
                "A GLTF button is already configured. Do you want to recreate it?", 
                "Yes", "No"
            );
            
            if (!overwrite)
            {
                return;
            }
            
            // Destroy existing button
            if (uiSettings.GLTFButton.gameObject != null)
            {
                DestroyImmediate(uiSettings.GLTFButton.gameObject);
            }
        }

        // Clone the VMD button
        var vmdButton = uiSettings.VMDButton;
        var gltfButtonObj = Instantiate(vmdButton.gameObject, vmdButton.transform.parent);
        gltfButtonObj.name = "GLTFButton";
        
        // Register undo
        Undo.RegisterCreatedObjectUndo(gltfButtonObj, "Create GLTF Button");

        // Update position (place it below VMD button)
        var rectTransform = gltfButtonObj.GetComponent<RectTransform>();
        var vmdRect = vmdButton.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(
            vmdRect.anchoredPosition.x,
            vmdRect.anchoredPosition.y - 45  // Offset below VMD button
        );

        // Update text
        var text = gltfButtonObj.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = "Export GLTF";
        }

        // Setup button click event
        var button = gltfButtonObj.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {
            if (UmaViewerUI.Instance != null)
            {
                UmaViewerUI.Instance.ExportGLTF();
            }
        });

        // Link to UISettingsAnimation
        SerializedObject serializedSettings = new SerializedObject(uiSettings);
        SerializedProperty gltfButtonProp = serializedSettings.FindProperty("GLTFButton");
        gltfButtonProp.objectReferenceValue = button;
        serializedSettings.ApplyModifiedProperties();

        // Mark scene as dirty
        EditorUtility.SetDirty(uiSettings);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene()
        );

        EditorUtility.DisplayDialog(
            "Success", 
            "GLTF Export button has been created and configured!\n\n" +
            "Location: Below the VMD button in Animation Settings\n" +
            "Function: Calls UmaViewerUI.ExportGLTF()\n\n" +
            "Don't forget to save the scene!",
            "OK"
        );
        
        Debug.Log("GLTF Export button created successfully!");
        
        // Select the new button in hierarchy
        Selection.activeGameObject = gltfButtonObj;
    }
}
#endif
