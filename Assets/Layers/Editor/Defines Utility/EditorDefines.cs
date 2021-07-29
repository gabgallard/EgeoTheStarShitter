using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Layers_Settings_Inspector
{
    public class EditorDefines
    {
        List<string> allDefines = new List<string>();
        public EditorDefines()
        {
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            allDefines = definesString.Split(';').ToList();
        }

        public bool ContainsDefine(string define)
        {
            return allDefines.Contains(define);
        }

        public void AddDefine(string define)
        {
            if (!allDefines.Contains(define))
                allDefines.Add(define);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", allDefines.ToArray()));
        }

        public void RemoveDefine(string define)
        {
            allDefines.Remove(define);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", allDefines.ToArray()));
        }

    }
}
