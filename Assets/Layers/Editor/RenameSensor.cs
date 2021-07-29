using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Utilities
{
    class MyAllPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {


            for (int i = 0; i < movedAssets.Length; i++)
            {
                string assetpath = movedAssets[i];
                SoundGraph graph = AssetDatabase.LoadAssetAtPath<SoundGraph>(assetpath);
                if (graph != null)
                    LayersSettings.GetOrCreateSettings().MarkSoundGraphAsChanged(graph);

                GlobalsAsset globals = AssetDatabase.LoadAssetAtPath<GlobalsAsset>(assetpath);
                if (globals != null)
                    LayersSettings.GetOrCreateSettings().MarkGlobalsAssetAsChanged(globals);
            }

            for(int i = 0; i<deletedAssets.Length; i++)
            {
                string assetpath = deletedAssets[i];
                string guid = AssetDatabase.AssetPathToGUID(assetpath);
                if (!string.IsNullOrEmpty(guid))
                    LayersSettings.GetOrCreateSettings().RemoveDBItem(guid);
            }
        }
    }
}