using UnityEditor;
using UnityEngine;
using System.IO;

public class FolderGenerator : EditorWindow
{
    private bool createView = true;
    private bool createViewModel = true;
    private bool createModel = true;
    private bool createStyles = true;

    [MenuItem("Tools/Generate MVC Folders and Files")]
    public static void ShowWindow()
    {
        GetWindow<FolderGenerator>("Generate MVC Folders and Files");
    }

    private void OnGUI()
    {
        GUILayout.Label("Generate MVC Folders and Files", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate Folders Views, ViewModel, Model, Styles"))
        {
            GenerateFoldersAndFilesInSelectedFolder(FolderCreateType.Views);
            GenerateFoldersAndFilesInSelectedFolder(FolderCreateType.ViewModel);
            GenerateFoldersAndFilesInSelectedFolder(FolderCreateType.Model);
            GenerateFoldersAndFilesInSelectedFolder(FolderCreateType.Styles);
        }
        GUILayout.Label("Select folder to create", EditorStyles.boldLabel);
        FolderCreateType folderType = FolderCreateType.Views;
        folderType = (FolderCreateType)EditorGUILayout.EnumPopup("Folder Type", folderType);

    }

    void GenerateFoldersAndFilesInSelectedFolder(FolderCreateType folderType)
    {
        string selectedFolderPath = GetSelectedFolderPath();

        if (string.IsNullOrEmpty(selectedFolderPath))
        {
            Debug.LogError("Please select a folder in the Project view.");
            return;
        }

        folderType.CreateFileWithName(selectedFolderPath);

        string folderName = Path.GetFileName(selectedFolderPath);
        AssetDatabase.Refresh();
    }

    private string GetSelectedFolderPath()
    {
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(Selection.activeObject));

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (AssetDatabase.IsValidFolder(path))
        {
            return path;
        }

        return null;
    }
}

public enum FolderCreateType
{
    Views,
    ViewModel,
    Model,
    Styles
}

public static class FolderTypeExtension
{
    public static string GetFolderName(this FolderCreateType folderType)
    {
        switch (folderType)
        {
            case FolderCreateType.Views:
                return "Views";
            case FolderCreateType.ViewModel:
                return "ViewModel";
            case FolderCreateType.Model:
                return "Model";
            case FolderCreateType.Styles:
                return "Styles";
            default:
                return "";
        }
    }

    public static string GetFileContent(this FolderCreateType folderType)
    {
        switch (folderType)
        {
            case FolderCreateType.Views:
                return "<UXML>\n</UXML>";
            case FolderCreateType.ViewModel:
                return $"public class {folderType.GetFolderName()} {{ }}";
            case FolderCreateType.Model:
                return "";
            case FolderCreateType.Styles:
                return "";
            default:
                return "";
        }
    }

    public static string GetFilePrefix(this FolderCreateType folderType)
    {
        switch (folderType)
        {
            case FolderCreateType.Views:
                return ".uxml";
            case FolderCreateType.ViewModel:
                return "ViewModel.cs";
            case FolderCreateType.Model:
                return "Model.cs";
            case FolderCreateType.Styles:
                return ".uss";
            default:
                return "";
        }
    }


    public static void CreateFileWithName(this FolderCreateType folderType, string parentFolderPath)
    {
        string folderPath = $"{parentFolderPath}/{folderType.GetFolderName()}";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            string filePath = Path.Combine(folderPath, Selection.activeObject.name + folderType.GetFilePrefix());
            if (!File.Exists(filePath))
            {
                AssetDatabase.CreateFolder(parentFolderPath, folderType.GetFolderName());
                File.WriteAllText(filePath, folderType.GetFileContent());
                Debug.Log($"Created file: {filePath}");
            }
            else
            {
                Debug.LogWarning($"File already exists: {filePath}");
            }
        }
    }
}
