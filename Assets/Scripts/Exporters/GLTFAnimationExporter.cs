using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using GLTFast.Export;
using GLTFast;

/// <summary>
/// Exports Unity animations to GLTF/GLB format
/// </summary>
public class GLTFAnimationExporter : MonoBehaviour
{
    public const string FileSavePath = "/../GLTFExports";
    
    public bool IsRecording { get; private set; } = false;
    public bool IsExporting { get; private set; } = false;
    
    private UmaContainerCharacter container;
    private Animator animator;
    private AnimationClip currentClip;
    
    /// <summary>
    /// Initialize the exporter with the character container
    /// </summary>
    public void Initialize()
    {
        container = GetComponentInParent<UmaContainerCharacter>();
        if (container != null)
        {
            animator = container.UmaAnimator;
            if (animator != null && container.OverrideController != null)
            {
                currentClip = container.OverrideController["clip_2"];
            }
        }
    }
    
    /// <summary>
    /// Start recording animation
    /// </summary>
    public void StartRecording()
    {
        if (container == null || animator == null)
        {
            Debug.LogError("GLTFAnimationExporter: Container or Animator not initialized");
            return;
        }
        
        IsRecording = true;
        Debug.Log("GLTFAnimationExporter: Recording started");
    }
    
    /// <summary>
    /// Stop recording animation
    /// </summary>
    public void StopRecording()
    {
        IsRecording = false;
        Debug.Log("GLTFAnimationExporter: Recording stopped");
    }
    
    /// <summary>
    /// Export the current animation to GLTF/GLB format
    /// </summary>
    /// <param name="modelName">Name of the model for the filename</param>
    /// <param name="format">Export format (GLTF or GLB)</param>
    public async void ExportAnimation(string modelName, GltfFormat format = GltfFormat.Binary)
    {
        if (IsExporting)
        {
            Debug.LogWarning("GLTFAnimationExporter: Export already in progress");
            return;
        }
        
        if (container == null)
        {
            Debug.LogError("GLTFAnimationExporter: Container not initialized");
            return;
        }
        
        IsExporting = true;
        
        try
        {
            // Create export directory if it doesn't exist
            string exportPath = Application.dataPath + FileSavePath;
            if (!Directory.Exists(exportPath))
            {
                Directory.CreateDirectory(exportPath);
            }
            
            // Generate filename with timestamp
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string extension = format == GltfFormat.Binary ? ".glb" : ".gltf";
            string fileName = $"{modelName}_{timestamp}{extension}";
            string fullPath = Path.Combine(exportPath, fileName);
            
            // Configure export settings
            var exportSettings = new ExportSettings
            {
                Format = format,
                FileConflictResolution = FileConflictResolution.Overwrite,
                // Include animations and skinned meshes
                ComponentMask = ~0u // Export all components
            };
            
            // Create exporter
            var export = new GameObjectExport(exportSettings);
            
            // Add the character's position root to export
            var rootTransform = container.transform.Find("Position");
            if (rootTransform == null)
            {
                rootTransform = container.transform;
            }
            
            // Add scene with the character
            export.AddScene(new[] { rootTransform.gameObject }, rootTransform.worldToLocalMatrix);
            
            // Export to file
            bool success = await export.SaveToFileAndDispose(fullPath);
            
            if (success)
            {
                Debug.Log($"GLTFAnimationExporter: Successfully exported to {fullPath}");
                UmaViewerUI.Instance.ShowMessage($"GLTF/GLB exported to {Path.GetFullPath(fullPath)}", UIMessageType.Success);
            }
            else
            {
                Debug.LogError("GLTFAnimationExporter: Export failed");
                UmaViewerUI.Instance.ShowMessage("GLTF/GLB export failed", UIMessageType.Error);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"GLTFAnimationExporter: Exception during export: {e.Message}\n{e.StackTrace}");
            UmaViewerUI.Instance.ShowMessage($"GLTF/GLB export error: {e.Message}", UIMessageType.Error);
        }
        finally
        {
            IsExporting = false;
        }
    }
    
    /// <summary>
    /// Quick export with default settings
    /// </summary>
    /// <param name="modelName">Name of the model for the filename</param>
    public void QuickExport(string modelName)
    {
        ExportAnimation(modelName, GltfFormat.Binary);
    }
}
