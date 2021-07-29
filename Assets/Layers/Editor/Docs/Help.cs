using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace ABXY.Layers.Editor.Docs
{
    public static class Help
    {
        private static Dictionary<string, string> _portDescriptions = null;
        public static Dictionary<string, string> portDescriptions
        {
            get
            {
                if (_portDescriptions == null)
                {
                    _portDescriptions = new Dictionary<string, string>();
                    string descriptionsString = Resources.Load<TextAsset>("Symphony/Help Files/Nodes/Descriptions").text;
                    string[] descriptionsArray = descriptionsString.Split('|');
                    for (int index = 0; index + 1 < descriptionsArray.Length; index += 2)
                    {
                        if (!_portDescriptions.ContainsKey(descriptionsArray[index]))
                            _portDescriptions.Add(descriptionsArray[index], descriptionsArray[index + 1]);
                    }
                }
                return _portDescriptions;
            }
        }

        [MenuItem ("Tools/Layers/Help",priority = 0)]
        public static void LoadWiki()
        {
            Application.OpenURL("https://github.com/mwahnish/Layers-Adaptive-Audio/wiki");
        }

        [MenuItem("Tools/Layers/Request a feature", priority = 1)]
        public static void LoadIssues()
        {
            Application.OpenURL("https://github.com/mwahnish/Layers-Adaptive-Audio/issues");
        }
    }
}
