using System.Collections.Generic;
using System.IO;
using ABXY.Layers.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Layers_Settings_Inspector
{
    public static class LayersSettingsInspector
    {
        private static bool regColorsExpanded = false;
        private static bool darkColorsExpanded = false;

        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider("Project/Layers", SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = "Layers Settings",
                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                    var settings = LayersSettings.GetOrCreateSettings() ;

                    string folderID = settings.codeGenFolderID;

                    string relPath = AssetDatabase.GUIDToAssetPath(folderID);

                    Rect fullRect = EditorGUILayout.GetControlRect();
                    Rect controlRect = EditorGUI.PrefixLabel(fullRect,new GUIContent("Generated code directory"));

                    float buttonWidth = 70;

                    EditorGUI.TextField(new Rect(controlRect.x, controlRect.y, controlRect.width - buttonWidth, controlRect.height), Directory.Exists(relPath) ? relPath : "No directory selected");

                    if (GUI.Button(new Rect(controlRect.x + controlRect.width - buttonWidth, controlRect.y, buttonWidth, controlRect.height), "Choose"))
                    {
                        string newFilePath = EditorUtility.OpenFolderPanel("Choose Code Generation Location", Application.dataPath, "");
                        string relativeFilePath = MakeRelativePath(Application.dataPath.Substring(0, Application.dataPath.Length -7), newFilePath);
                        string newFileID = AssetDatabase.AssetPathToGUID(relativeFilePath);
                        settings.codeGenFolderID = newFileID;
                    }

                    settings.enableMIDIInBuilds = EditorGUILayout.Toggle("Enable MIDI in builds", settings.enableMIDIInBuilds);

                    // Colors!
                    regColorsExpanded = EditorGUILayout.Foldout(regColorsExpanded, "Light mode colors");

                    EditorGUI.indentLevel++;
                    if (regColorsExpanded)
                    {
                        foreach (string colorName in settings.GetColorNames())
                        {
                            Color color = settings.GetColor(colorName, false);
                            Color newColor = EditorGUILayout.ColorField(VariableInspectorUtility.GetPrettyName(colorName), color);
                            if (color != newColor)
                                settings.SetColor(colorName, newColor,false);
                        }
                    }

                    EditorGUI.indentLevel--;

                    darkColorsExpanded = EditorGUILayout.Foldout(darkColorsExpanded, "Dark mode colors");

                    EditorGUI.indentLevel++;
                    if (darkColorsExpanded)
                    {
                        foreach (string colorName in settings.GetColorNames())
                        {
                            Color color = settings.GetColor(colorName, true);
                            Color newColor = EditorGUILayout.ColorField(VariableInspectorUtility.GetPrettyName(colorName), color);
                            if (color != newColor)
                                settings.SetColor(colorName, newColor, true);
                        }
                    }

                    EditorGUI.indentLevel--;


                    if (GUILayout.Button("Reset colors to defaults"))
                        settings.ResetColors();


                    settings.indexingStyle = (LayersSettings.IndexingStyles)EditorGUILayout.EnumPopup("Indexing Style",
                        settings.indexingStyle);

                    EditorDefines defines = new EditorDefines();
                    bool hasDefine = defines.ContainsDefine("SYMPHONY_DEV");

                    if (hasDefine)
                        settings.enableGreenScreen = EditorGUILayout.Toggle("Greenscreen mode", settings.enableGreenScreen);
                    else
                        settings.enableGreenScreen = false;


                    bool newHasDefine = EditorGUILayout.Toggle("Developer mode", hasDefine);
                    if (newHasDefine != hasDefine)
                    {

                        if (newHasDefine) 
                            defines.AddDefine("SYMPHONY_DEV");
                        else
                            defines.RemoveDefine("SYMPHONY_DEV");
                    }



                    bool hasWhiteScreen = defines.ContainsDefine("LAYERS_SCREENSHOT");



                    settings.enableScreenshot = hasWhiteScreen;

                    bool newHasWhiteScreen = hasDefine?EditorGUILayout.Toggle("Screenshot mode", hasWhiteScreen):false;
                    if (hasWhiteScreen != newHasWhiteScreen)
                    {
                        settings.enableScreenshot = newHasDefine;
                        if (newHasWhiteScreen)
                        {
                            defines.AddDefine("LAYERS_SCREENSHOT");
                            settings.enableGreenScreen = false;
                        }
                        else
                            defines.RemoveDefine("LAYERS_SCREENSHOT");
                    }



                    //EditorGUILayout.TextField(settings.codeGenerationPath);
                    //EditorGUILayout.PropertyField(settings.FindProperty("m_Number"), new GUIContent("My Number"));
                    //EditorGUILayout.PropertyField(settings.FindProperty("m_SomeString"), new GUIContent("My String"));
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "Number", "Some String" })
            };



            return provider;
        }




        private static string MakeRelativePath(string baseDirectory, string targetDirectory)
        {
            DirectoryInfo directory = new DirectoryInfo(baseDirectory);
            DirectoryInfo fileInfo = new DirectoryInfo(targetDirectory);
            if (baseDirectory.Length < targetDirectory.Length && fileInfo.FullName.StartsWith(directory.FullName))
                return fileInfo.FullName.Substring(directory.FullName.Length + 1);
            return "";
        }
    }
}
