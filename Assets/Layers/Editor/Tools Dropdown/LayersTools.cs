using ABXY.Layers.Editor.Code_generation;
using UnityEditor;

namespace ABXY.Layers.Editor.Tools_Dropdown
{
    public class LayersTools
    {
        [MenuItem("Tools/Layers/Regenerate Player Scripts", priority =-20)]
        public static void RegenerateAllScripts()
        {
            if (EditorUtility.DisplayDialog("Confirm Script Regeneration?", "This will regenerate all Sound Graph Players in your project. Depending on the number of players, this may take some time!", "Continue", "Cancel"))
            {
                RegenList.GenerateAll();
            }
        }
    }
}
