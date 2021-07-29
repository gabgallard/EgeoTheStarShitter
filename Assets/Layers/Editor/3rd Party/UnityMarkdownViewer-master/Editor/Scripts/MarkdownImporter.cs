#if UNITY_2018_1_OR_NEWER

using System.IO;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace ABXY.Layers.Editor.ThirdParty.Editor.Scripts
{
    #if SYMPHONY_DEV
    [ScriptedImporter( 1, "markdown" )]
    public class MarkdownAssetImporter : ScriptedImporter
    {
        public override void OnImportAsset( AssetImportContext ctx )
        {
            var md = new TextAsset( File.ReadAllText( ctx.assetPath ) );
            ctx.AddObjectToAsset( "main", md );
            ctx.SetMainObject( md );
        }
    }
#endif
}

#endif
