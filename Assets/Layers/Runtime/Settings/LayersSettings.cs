using System.Collections.Generic;
using System.IO;
using System.Linq;
using ABXY.Layers.Runtime.Graph_Variable_Values;
using UnityEngine;

namespace ABXY.Layers.Runtime.Settings
{
    public class LayersSettings : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        private string _codeGenFolderID;

        public string codeGenFolderID
        {
            get
            {
                return _codeGenFolderID;
            }
            set
            {
#if UNITY_EDITOR
                string oldFolder = UnityEditor.AssetDatabase.GUIDToAssetPath(_codeGenFolderID);
                string newFolder = UnityEditor.AssetDatabase.GUIDToAssetPath(value);
                if (_codeGenFolderID != value && Directory.Exists(oldFolder) && Directory.Exists(newFolder))
                {
                    Copy(oldFolder, newFolder);
                }
#endif
                _codeGenFolderID = value;
                SaveToDisk();
            }
        }

        public bool enableMIDIInBuilds = false;

        public bool enableGreenScreen = false;


        public bool enableScreenshot = false;

        public const string k_settingsPath = "Layers Preferences/Resources/Layers";

        private Dictionary<string, Color> type2Color = new Dictionary<string, Color>();
        [SerializeField]
        private List<string> type2ColorKeys = new List<string>();
        [SerializeField]
        private List<Color> type2ColorValues = new List<Color>();

        private Dictionary<string, Color> type2ColorPro = new Dictionary<string, Color>();
        [SerializeField]
        private List<string> type2ColorKeysPro = new List<string>();
        [SerializeField]
        private List<Color> type2ColorValuesPro = new List<Color>();


        


        private static LayersSettings cachedInstance = null;



        public static LayersSettings GetOrCreateSettings()
        {
            if (cachedInstance == null)
            {
                cachedInstance = Resources.Load<LayersSettings>("Layers/LayersSettings");
                cachedInstance?.ReloadColors();
            }
#if UNITY_EDITOR
            if (cachedInstance == null)
            {
                Debug.Log("Rebuilding layers settings");
                cachedInstance = ScriptableObject.CreateInstance<LayersSettings>();

                cachedInstance?.ReloadColors();

                if (PlayerPrefs.HasKey("LayersInstallDirectory"))
                    cachedInstance.codeGenFolderID = PlayerPrefs.GetString("LayersInstallDirectory");

                Directory.CreateDirectory(Path.Combine("Assets", k_settingsPath));
                UnityEditor.AssetDatabase.CreateAsset(cachedInstance, Path.Combine("Assets", k_settingsPath) + "/LayersSettings.asset");
                UnityEditor.AssetDatabase.SaveAssets();

                string[] soundGraphGUIDS = UnityEditor.AssetDatabase.FindAssets("t:SoundGraph");

                foreach (string soundGraphGUID in soundGraphGUIDS)
                {
                    SoundGraph soundGraph = (SoundGraph)UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(soundGraphGUID)
                        , typeof(SoundGraph));
                    if (soundGraph != null)
                    {
                        cachedInstance.RegisterSoundGraph(soundGraph);
                    }

                }

                string[] globalsObjectGUIDS = UnityEditor.AssetDatabase.FindAssets("t:GlobalsAsset");

                foreach (string globalsAssetGuid in globalsObjectGUIDS)
                {
                    GlobalsAsset globalsAsset = (GlobalsAsset)UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(globalsAssetGuid)
                        , typeof(GlobalsAsset));
                    if (globalsAsset != null)
                    {
                        cachedInstance.RegisterGlobalsAsset(globalsAsset);
                    }

                }
            }
#endif
            return cachedInstance;
        }

#if UNITY_EDITOR
        public static UnityEditor.SerializedObject GetSerializedSettings()
        {
            return new UnityEditor.SerializedObject(GetOrCreateSettings());
        }
#endif

        private void ReloadColors()
        {
            List<string> types = ValueUtility.GetManagedTypes(ValueUtility.ValueFilter.All);
            foreach (string typeName in types)
            {
                if (!type2Color.ContainsKey(typeName))
                {
                    type2Color.Add(typeName, ValueUtility.GetVariableValue(typeName, ValueUtility.ValueFilter.All).GetDefaultColor());

                }

                if (!type2ColorPro.ContainsKey(typeName))
                {
                    type2ColorPro.Add(typeName, ValueUtility.GetVariableValue(typeName, ValueUtility.ValueFilter.All).GetDefaultColorPro());

                }
            }
        }

        public void ResetColors()
        {
            type2Color.Clear();
            type2ColorKeys.Clear();
            type2ColorValues.Clear();
            type2ColorPro.Clear();
            type2ColorKeysPro.Clear();
            type2ColorValuesPro.Clear();
            ReloadColors();
        }

        public Color GetColor(string type, bool proskin)
        {
            Color returnColor = Color.white;
            if (proskin)
            {
                if (type2ColorPro.ContainsKey(type))
                    returnColor = type2ColorPro[type];
                else
                    returnColor = GraphVariableValue.GetTypeColor(type);
            }
            else
            {
                if (type2Color.ContainsKey(type))
                    returnColor = type2Color[type];
                else
                    returnColor = GraphVariableValue.GetTypeColor(type);
            }

            if (enableGreenScreen && GreenDistance(returnColor, Color.green) < 0.1f)
                returnColor = new Color(returnColor.r, Mathf.Repeat( returnColor.g + 0.5f, 1f), returnColor.b);

            return returnColor;
        }

        private float GreenDistance (Color a, Color b)
        {
            return Mathf.Abs(a.g - b.g);
        }

        public void SetColor(string typeName, Color newColor, bool proskin)
        {
            if (proskin)
            {
                if (type2ColorPro.ContainsKey(typeName))
                {
                    bool changed = type2ColorPro[typeName] != newColor;
                    type2ColorPro[typeName] = newColor;
                    if (changed)
                        SaveToDisk();
                }

            }
            else
            {
                if (type2Color.ContainsKey(typeName))
                {
                    bool changed = type2Color[typeName] != newColor;
                    type2Color[typeName] = newColor;
                    if (changed)
                        SaveToDisk();
                }

            }

        }

        public List<string> GetColorNames()
        {
            return type2Color.Keys.Select(x => x.ToString()).ToList();
        }

        /// <summary>
        /// Adapted from https://stackoverflow.com/questions/7146021/copy-all-files-in-directory
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="targetDir"></param>
        void Copy(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir).Where(x => x.EndsWith(".cs")))
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));

            foreach (var directory in Directory.GetDirectories(sourceDir))
                Copy(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
        }

        public void OnBeforeSerialize()
        {
            type2ColorKeys.Clear();
            type2ColorValues.Clear();
            foreach (KeyValuePair<string, Color> color in type2Color)
            {
                type2ColorKeys.Add(color.Key);
                type2ColorValues.Add(color.Value);
            }

            type2ColorKeysPro.Clear();
            type2ColorValuesPro.Clear();
            foreach (KeyValuePair<string, Color> color in type2ColorPro)
            {
                type2ColorKeysPro.Add(color.Key);
                type2ColorValuesPro.Add(color.Value);
            }

            soundGraphDBKeys.Clear();
            soundGraphDBValues.Clear();
            foreach (KeyValuePair<string, SoundGraphDBEntry> entry in soundGraphDB)
            {
                if (!Object.ReferenceEquals(entry.Value.soundGraph, null) || !Object.ReferenceEquals(entry.Value.globalsAsset, null))
                {
                    soundGraphDBKeys.Add(entry.Key);
                    soundGraphDBValues.Add(entry.Value);
                }
            }
        }

        public void OnAfterDeserialize()
        {
            if (type2ColorKeys.Count != type2ColorValues.Count)
                return;
            type2Color.Clear();
            for (int index = 0; index < type2ColorKeys.Count; index++)
            {
                type2Color.Add(type2ColorKeys[index], type2ColorValues[index]);
            }


            if (type2ColorKeysPro.Count != type2ColorValuesPro.Count)
                return;
            type2ColorPro.Clear();
            for (int index = 0; index < type2ColorKeysPro.Count; index++)
            {
                type2ColorPro.Add(type2ColorKeysPro[index], type2ColorValuesPro[index]);
            }

            if (soundGraphDBKeys.Count != soundGraphDBValues.Count)
                return;
            soundGraphDB.Clear();
            for (int index = 0; index < soundGraphDBKeys.Count; index++)
            {
                if (!Object.ReferenceEquals(soundGraphDBValues[index].soundGraph, null) ||  !Object.ReferenceEquals(soundGraphDBValues[index].globalsAsset, null))
                {
                    soundGraphDB.Add(soundGraphDBKeys[index], soundGraphDBValues[index]);
                }
            }
        }

        #region soundGraph
        private Dictionary<string, SoundGraphDBEntry> soundGraphDB = new Dictionary<string, SoundGraphDBEntry>();
        [SerializeField]
        private List<string> soundGraphDBKeys = new List<string>();
        [SerializeField]
        private List<SoundGraphDBEntry> soundGraphDBValues = new List<SoundGraphDBEntry>();

        public void RegisterSoundGraph(SoundGraph soundGraph)
        {
            if (!soundGraphDB.ContainsKey(soundGraph.graphID))
            {
                soundGraphDB.Add(soundGraph.graphID, new SoundGraphDBEntry(soundGraph));
                SaveToDisk();
            }
        }

        private SoundGraphDBEntry GetSoundGraphEntry(SoundGraph soundGraph)
        {
            if (soundGraph == null)
                return null;

            SoundGraphDBEntry targetSoundGraphEntry = null;

            soundGraphDB.TryGetValue(soundGraph.graphID, out targetSoundGraphEntry);

            if (targetSoundGraphEntry == null)
            {
                targetSoundGraphEntry = new SoundGraphDBEntry(soundGraph);
                soundGraphDB.Add(soundGraph.graphID, targetSoundGraphEntry);
            }
            return targetSoundGraphEntry;
        }

        public SoundGraphDBEntry[] GetGraphsNeedingCodeGen()
        {
            return soundGraphDB.Where(x => x.Value.needsRegen).Select(x => x.Value).ToArray();
        }

        public bool CheckIfSoundGraphNeedsRegen(SoundGraph soundGraph)
        {
            if (soundGraph == null)
                return false;

            SoundGraphDBEntry targetSoundGraphEntry = GetSoundGraphEntry(soundGraph);

            if (targetSoundGraphEntry != null)
                return targetSoundGraphEntry.needsRegen;

            return false;
        }

        public void MarkSoundGraphAsChanged(SoundGraph soundGraph)
        {
            if (soundGraph == null)
                return;

            if (soundGraph.isRunningSoundGraph)
                return;

            SoundGraphDBEntry targetSoundGraphEntry = GetSoundGraphEntry(soundGraph);

            if (targetSoundGraphEntry != null)
            {
                if (!targetSoundGraphEntry.needsRegen)
                {
                    targetSoundGraphEntry.needsRegen = true;
                    SaveToDisk();
                }
            }

        }

        public void MarkAsGenerated(SoundGraphDBEntry soundGraph)
        {

            if (soundGraph != null && soundGraph.soundGraph &&  soundGraph.soundGraph.isRunningSoundGraph)
                return;



            soundGraph.needsRegen = false;
            SaveToDisk();


        }

        public SoundGraphDBEntry[] GetAllDBItems()
        {
            return soundGraphDB.Select(x => x.Value).ToArray();
        }

        public void RemoveDBItem(string id)
        {
            if (soundGraphDB.ContainsKey(id))
                soundGraphDB.Remove(id);
        }

        #endregion

        #region globalsAsset

        public void RegisterGlobalsAsset(GlobalsAsset globalsAsset)
        {
            if (!soundGraphDB.ContainsKey(globalsAsset.assetID))
            {
                soundGraphDB.Add(globalsAsset.assetID, new SoundGraphDBEntry(globalsAsset));
                SaveToDisk();
            }
        }

        private SoundGraphDBEntry GetGlobalsAssetEntry(GlobalsAsset globalsAsset)
        {
            if (globalsAsset == null)
                return null;

            SoundGraphDBEntry targetSoundGraphEntry = null;

            soundGraphDB.TryGetValue(globalsAsset.assetID, out targetSoundGraphEntry);

            if (targetSoundGraphEntry == null)
            {
                targetSoundGraphEntry = new SoundGraphDBEntry(globalsAsset);
                soundGraphDB.Add(globalsAsset.assetID, targetSoundGraphEntry);
            }
            return targetSoundGraphEntry;
        }


        public bool CheckIfGlobalsAssetNeedsRegen(GlobalsAsset globalsAsset)
        {
            if (globalsAsset == null)
                return false;

            SoundGraphDBEntry targetSoundGraphEntry = GetGlobalsAssetEntry(globalsAsset);

            if (targetSoundGraphEntry != null)
                return targetSoundGraphEntry.needsRegen;

            return false;
        }

        public void MarkGlobalsAssetAsChanged(GlobalsAsset globalsAsset)
        {
            if (globalsAsset == null)
                return;


            SoundGraphDBEntry targetSoundGraphEntry = GetGlobalsAssetEntry(globalsAsset);

            if (targetSoundGraphEntry != null)
            {
                if (!targetSoundGraphEntry.needsRegen)
                {
                    targetSoundGraphEntry.needsRegen = true;
                    SaveToDisk();
                }
            }

        }

        public void MarkGlobalsAssetAsGenerated(SoundGraphDBEntry soundGraph)
        {
            if (soundGraph == null)
                return;

            soundGraph.needsRegen = false;
            SaveToDisk();


        }

        #endregion

        private void SaveToDisk()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }

        [System.Serializable]
        public class SoundGraphDBEntry
        {
            public SoundGraph soundGraph;
            public GlobalsAsset globalsAsset;
            public List<string> generatedAssets = new List<string>();
            public bool needsRegen;

            public string name { 
                get
                {
                    if (soundGraph != null)
                        return soundGraph.name;
                    else if (globalsAsset != null)
                        return globalsAsset.name;
                    return "";
                } 
            }

            public SoundGraphDBEntry(SoundGraph soundGraph)
            {
                this.soundGraph = soundGraph ?? throw new System.ArgumentNullException(nameof(soundGraph));
                needsRegen = true;
            }

            public SoundGraphDBEntry(GlobalsAsset globalsAsset)
            {
                this.globalsAsset = globalsAsset ?? throw new System.ArgumentNullException(nameof(globalsAsset));
                needsRegen = true;
            }
        }
    }
}
