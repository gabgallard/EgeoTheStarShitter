using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ABXY.Layers.Editor.Code_generation.Core;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Graph_Variable_Values;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Signal_Sources;
using ABXY.Layers.Runtime.Settings;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using UnityEditor;
using UnityEngine;
using static ABXY.Layers.Runtime.Settings.LayersSettings;

namespace ABXY.Layers.Editor.Code_generation
{
    public static class CodeGenerator
    {
        public static void Generate(SoundGraphDBEntry graphEntry)
        {
            string assetGUID = "";
            

            foreach (string assetID in graphEntry.generatedAssets)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetID);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    AssetDatabase.DeleteAsset(assetPath);
                }
            }
            graphEntry.generatedAssets.Clear();

            if (graphEntry.soundGraph != null && graphEntry.soundGraph.graphInput != null)
            {
                long instanceID;
                if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(graphEntry.soundGraph, out assetGUID, out instanceID))
                {
                    Debug.LogError("Generation failed. Source graph isn't an asset");
                    return;
                }
                GenerateSoundGraphRuntimeCode(graphEntry, assetGUID);
                GenerateSoundGraphEditorCode(graphEntry, assetGUID);
            }else if (graphEntry.globalsAsset != null)
            {
                long instanceID;
                if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(graphEntry.globalsAsset, out assetGUID, out instanceID))
                {
                    Debug.LogError("Generation failed. Source graph isn't an asset");
                    return;
                }
                GenerateGlobalsAssetRuntimeCode(graphEntry, assetGUID);
            }

        }

        private static void GenerateGlobalsAssetRuntimeCode(SoundGraphDBEntry graphEntry, string assetGUID)
        {
            string resourcesPath = GetResourcesPath(graphEntry);

            string safeName = ReflectionUtils.RemoveSpecialCharacters(graphEntry.globalsAsset.name);
            FileBuilder file = new FileBuilder();

            //generation
            file.AddPreambleField(new ArbitraryLineBuilder("using UnityEngine;"));
            file.AddPreambleField(new ArbitraryLineBuilder("using System.Collections.Generic;"));
            file.AddPreambleField(new ArbitraryLineBuilder("using ABXY.Layers.Runtime;"));
            file.AddPreambleField(new ArbitraryLineBuilder("using ABXY.Layers.Runtime.Midi;"));

            ClassBuilder assetClass = new ClassBuilder(safeName, false, "", true);

            assetClass.AddField(new FieldBuilder(FieldBuilder.AccessibilityValues.Private, true, true, "string", "resourcesPath", $"\"{resourcesPath}\""));
            assetClass.AddField(new FieldBuilder(FieldBuilder.AccessibilityValues.Private, true, false, "GlobalsAsset", "_instance", "null"));

            PropertyGetBuild instanceGetter = new PropertyGetBuild(false);
            instanceGetter.AddLine("if (_instance == null)");
            instanceGetter.AddLine("    _instance = Resources.Load<GlobalsAsset>(resourcesPath);");
            instanceGetter.AddLine("return _instance;");
            PropertyBuilder instanceBuilder = new PropertyBuilder(FieldBuilder.AccessibilityValues.Private, true, "GlobalsAsset", "instance", instanceGetter, null);
            assetClass.AddField(instanceBuilder);


            // Writing variables
            GraphVariable[] variables = graphEntry.globalsAsset.GetAllVariables().ToArray();
            foreach (GraphVariable variable in variables)
            {
                string typename = variable.typeName;
                if (variable.typeName == typeof(List<GraphVariable>).FullName)
                    typename = string.Format("GraphArray<{0}>", variable.arrayType);

                typename = typename.Replace("+", ".");

                string safePropName = ReflectionUtils.RemoveSpecialCharacters(variable.name);
                
                ArbitraryLineBuilder set = new ArbitraryLineBuilder(string.Format("instance.SetVariable(\"{0}\",{1});", variable.name, "value"));
                ArbitraryLineBuilder get = new ArbitraryLineBuilder(string.Format("return instance.GetVariable<{0}>(\"{1}\");", typename, variable.name));
                PropertyBuilder property = new PropertyBuilder(FieldBuilder.AccessibilityValues.Public, true, typename, safePropName, new PropertyGetBuild(get), new PropertySetBuild(set));
                assetClass.AddField(property);
               
            }


            foreach (GraphEvent gevent in graphEntry.globalsAsset.GetAllEvents())
            {
                if (gevent.parameters.Count != 0)
                {
                    assetClass.AddField(WriteGlobalAssetEventMethod(gevent, true, true));
                    assetClass.AddField(WriteGlobalAssetEventMethod(gevent, false, true));
                }
                else
                {
                    assetClass.AddField(WriteGlobalAssetEventMethod(gevent, false, false));
                    assetClass.AddField(WriteGlobalAssetEventMethod(gevent, true, false));
                }
            }

            //Callbacks
            foreach (GraphEvent gevent in graphEntry.globalsAsset.GetAllEvents())
            {
                string safeEventName = ReflectionUtils.RemoveSpecialCharacters(gevent.eventName);
                DelegateBuilder eventDelegateDef = new DelegateBuilder("void", FieldBuilder.AccessibilityValues.Public, safeEventName + "DelayedDelegate");
                eventDelegateDef.AddParameter(new ParameterBuilder("System.Double", "time"));
                foreach (GraphEvent.EventParameterDef parameter in gevent.parameters)
                {
                    string safeParameterName = ReflectionUtils.RemoveSpecialCharacters(parameter.parameterName);
                    eventDelegateDef.AddParameter(new ParameterBuilder(parameter.parameterTypeName, safeParameterName));
                }
                assetClass.AddField(eventDelegateDef);
                assetClass.AddField(new NewLineBuilder());
                assetClass.AddField(new FieldBuilder(FieldBuilder.AccessibilityValues.Private, true, false, safeEventName + "DelayedDelegate", "_on" + safeEventName+"Delayed", "null"));

                PropertyBuilder delegateProperty = new PropertyBuilder(FieldBuilder.AccessibilityValues.Public, true, safeEventName + "DelayedDelegate", "on" + safeEventName + "Delayed",
                    new PropertyGetBuild(new ArbitraryLineBuilder($"EventSetup(); return _on{safeEventName}Delayed;")),
                    new PropertySetBuild(new ArbitraryLineBuilder($"EventSetup(); _on{safeEventName}Delayed = value;")));

                assetClass.AddField(delegateProperty);

                assetClass.AddField(new NewLineBuilder());
            }


            // Callback Setup Function
            foreach (GraphEvent gevent in graphEntry.globalsAsset.GetAllEvents())
            {

                string safeEventName = ReflectionUtils.RemoveSpecialCharacters(gevent.eventName);
                MethodBuilder creationMethod = new MethodBuilder("void", true, false, FieldBuilder.AccessibilityValues.Private, "On" + safeEventName + "Internal");
                creationMethod.AddParameter(new ParameterBuilder("double", "time"));
                creationMethod.AddParameter(new ParameterBuilder("Dictionary<string, object>", "data"));

                foreach (GraphEvent.EventParameterDef parameter in gevent.parameters)
                {
                    string safeParameterName = ReflectionUtils.RemoveSpecialCharacters(parameter.parameterName);

                    GraphVariableValue value = ValueUtility.GetVariableValue(parameter.parameterTypeName, ValueUtility.ValueFilter.All);
                    string initializer = value != null ? value.GetValueInitializationString() : "null";

                    creationMethod.AddContent(new ArbitraryLineBuilder("object " + safeParameterName + " = " + initializer + ";"));
                    creationMethod.AddContent(new ArbitraryLineBuilder("data.TryGetValue(\"" + safeParameterName + "\", out " + safeParameterName + ");"));

                }

                string callbackParamsString = "time,";

                foreach (GraphEvent.EventParameterDef parameter in gevent.parameters)
                {
                    string safeParameterName = ReflectionUtils.RemoveSpecialCharacters(parameter.parameterName);
                    callbackParamsString += "(" + parameter.parameterTypeName + ")" + safeParameterName + ",";
                }

                callbackParamsString = callbackParamsString.Length != 0 ? callbackParamsString.Substring(0, callbackParamsString.Length - 1) : "";
                string callEventFunction = "on" + safeEventName + "Delayed?.Invoke(" + callbackParamsString + ");";
                creationMethod.AddContent(new ArbitraryBuilder(new List<string>(new string[] { callEventFunction })));

                assetClass.AddField(creationMethod);
            }

            assetClass.AddField(new FieldBuilder(FieldBuilder.AccessibilityValues.Private, true, false, "bool", "hasBeenSetup", "false"));



            MethodBuilder eventSetupMethod = new MethodBuilder("void", true, false, FieldBuilder.AccessibilityValues.Private, "EventSetup");
            eventSetupMethod.AddContent(new ArbitraryLineBuilder("if (hasBeenSetup) return;"));
            foreach (GraphEvent gevent in graphEntry.globalsAsset.GetAllEvents())
            {
                string safeEventName = ReflectionUtils.RemoveSpecialCharacters(gevent.eventName);
                eventSetupMethod.AddContent(new ArbitraryLineBuilder("instance.RegisterEventListener(\"" + gevent.eventName + "\", On" + safeEventName + "Internal);"));

            }
            eventSetupMethod.AddContent(new ArbitraryLineBuilder("hasBeenSetup = true;"));
            assetClass.AddField(eventSetupMethod);



            file.AddField(assetClass);




            //writing out
            string codeGenLocation = AssetDatabase.GUIDToAssetPath(LayersSettings.GetOrCreateSettings().codeGenFolderID);
            codeGenLocation = Directory.Exists(codeGenLocation) ? codeGenLocation : Application.dataPath;

            string newFileLocation = codeGenLocation + "/" + safeName + ".cs";


            CodeBuilder.WriteToFile(newFileLocation, file);
            AssetDatabase.Refresh();
            graphEntry.generatedAssets.Add(AssetDatabase.AssetPathToGUID(newFileLocation));
        }

        private static string GetResourcesPath(SoundGraphDBEntry entry)
        {
            string assetPath = AssetDatabase.GetAssetPath(entry.globalsAsset);
            Regex regex = new Regex("(?<=/)Resources(?=/)|^Resources(?=/)");
            if (regex.IsMatch(assetPath))
            {
                Match match = regex.Match(assetPath);
                int startIndex =  match.Index + match.Length + 1;
                return assetPath.Substring(startIndex, assetPath.Length - startIndex-6);
            }
            else
                return null;
        }

        private static MethodBuilder WriteGlobalAssetEventMethod(GraphEvent gevent, bool includeStartTime, bool includeParameters)
        {
            string safeEventName = ReflectionUtils.RemoveSpecialCharacters(gevent.eventName);
            List<ParameterBuilder> parameters = new List<ParameterBuilder>();

            if (includeStartTime)
                parameters.Add(new ParameterBuilder("System.Double", "startTime"));


            if (includeParameters)
            {
                foreach (GraphEvent.EventParameterDef parameterDefs in gevent.parameters)
                {
                    string safeParameterName = ReflectionUtils.RemoveSpecialCharacters(parameterDefs.parameterName);
                    parameters.Add(new ParameterBuilder(parameterDefs.parameterTypeName, safeParameterName));
                }
            }


            List<FieldBase> content = new List<FieldBase>();

            content.Add(new ArbitraryLineBuilder("Dictionary<string, object> parameters = new Dictionary<string, object>();"));

            if (includeParameters)
            {
                foreach (GraphEvent.EventParameterDef parameterDefs in gevent.parameters)
                {
                    string safeParameterName = ReflectionUtils.RemoveSpecialCharacters(parameterDefs.parameterName);
                    content.Add(new ArbitraryLineBuilder(string.Format("parameters.Add(\"{0}\",{1});", parameterDefs.parameterName, safeParameterName)));
                }
            }


            content.Add(new ArbitraryLineBuilder(string.Format("instance.CallEvent(\"{0}\", {1}, parameters);",
                gevent.eventName,
                includeStartTime ? "startTime" : "AudioSettings.dspTime")));

            return new MethodBuilder("void", true, false, FieldBuilder.AccessibilityValues.Public, safeEventName, content, parameters.ToArray());
        }

        private static void GenerateSoundGraphRuntimeCode(SoundGraphDBEntry graphEntry, string assetGUID)
        {

            string safeName = ReflectionUtils.RemoveSpecialCharacters(graphEntry.soundGraph.name);

            FileBuilder file = new FileBuilder();

            GeneratePlayerCode(file, graphEntry, assetGUID);
            GenerateNonPlayerRuntimeCode(file, graphEntry);

            string codeGenLocation = AssetDatabase.GUIDToAssetPath(LayersSettings.GetOrCreateSettings().codeGenFolderID);
            codeGenLocation = Directory.Exists(codeGenLocation) ? codeGenLocation : Application.dataPath;

            string newFileLocation = codeGenLocation + "/" + safeName + ".cs";


            CodeBuilder.WriteToFile(newFileLocation, file);
            AssetDatabase.Refresh();
            graphEntry.generatedAssets.Add(AssetDatabase.AssetPathToGUID(newFileLocation));
        }

        private static void GenerateSoundGraphEditorCode(SoundGraphDBEntry graphEntry, string assetGUID)
        {
            FileBuilder fileBuilder = new FileBuilder();
            foreach (Node node in graphEntry.soundGraph.nodes)
            {
                if (node == null)
                    continue;

                FlowNodeCodeGenerator generator = FlowNodeCodeGenerator.GetGenerator(node.GetType());
                generator?.InsertEditorCode(fileBuilder, node as FlowNode);
            }


            if (!fileBuilder.isEmpty)
            {
                string codeGenLocation = AssetDatabase.GUIDToAssetPath(LayersSettings.GetOrCreateSettings().codeGenFolderID);
                codeGenLocation = Directory.Exists(codeGenLocation) ? codeGenLocation : Application.dataPath;

                codeGenLocation = Path.Combine(codeGenLocation, "Editor");

                if (!Directory.Exists(codeGenLocation))
                    Directory.CreateDirectory(codeGenLocation);

                string safeName = ReflectionUtils.RemoveSpecialCharacters(graphEntry.soundGraph.name+"Editor");

                string newFileLocation = codeGenLocation + "/" + safeName + ".cs";
                AssetDatabase.Refresh();
                graphEntry.generatedAssets.Add(AssetDatabase.AssetPathToGUID(newFileLocation));

                CodeBuilder.WriteToFile(newFileLocation, fileBuilder);
            }



        }

        private static void GeneratePlayerCode(FileBuilder file, SoundGraphDBEntry graphEntry, string assetGUID)
        {
            file.AddPreambleField(new ArbitraryLineBuilder("using UnityEngine;"));
            file.AddPreambleField(new ArbitraryLineBuilder("using System.Collections.Generic;"));
            file.AddPreambleField(new ArbitraryLineBuilder("using ABXY.Layers.Runtime;"));
            file.AddPreambleField(new ArbitraryLineBuilder("using ABXY.Layers.Runtime.Midi;"));

            file.AddField(new ArbitraryLineBuilder("[ExecuteInEditMode]"));
            file.AddField(new ArbitraryLineBuilder("[AddComponentMenu(\"Layers / Soundgraphs / " + graphEntry.soundGraph.name + "\")]"));
    
            file.AddField(MakePlayer(graphEntry, assetGUID));
        }

        private static void GenerateNonPlayerRuntimeCode(FileBuilder file, SoundGraphDBEntry graphEntry)
        {
            foreach (Node node in graphEntry.soundGraph.nodes)
            {
                if (node == null)
                    continue;

                FlowNodeCodeGenerator generator = FlowNodeCodeGenerator.GetGenerator(node.GetType());
                generator?.InsertNonPlayerRuntimeCode(file, node as FlowNode);
            }
        }


        private static ClassBuilder MakePlayer(SoundGraphDBEntry graphEntry, string assetGUID, bool isSubclass=false)
        {
            string safeName = ReflectionUtils.RemoveSpecialCharacters(graphEntry.soundGraph.name);

        

            ClassBuilder graphClass = new ClassBuilder(safeName + (isSubclass?"Player":""), false, "SoundGraphPlayer");

            if (graphEntry.soundGraph.graphInput.events.Where(x => x.expose).Count() != 0)
            {

               

                //Callbacks
                foreach (GraphEvent gevent in graphEntry.soundGraph.graphInput.events.Where(x => x.expose))
                {
                    string safeEventName = ReflectionUtils.RemoveSpecialCharacters(gevent.eventName);

                    //immediate delegate
                    DelegateBuilder eventDelegateDef = new DelegateBuilder("void", FieldBuilder.AccessibilityValues.Public, safeEventName + "Delegate");
                    foreach(GraphEvent.EventParameterDef parameter in gevent.parameters)
                    {
                        string safeParameterName = ReflectionUtils.RemoveSpecialCharacters(parameter.parameterName);
                        eventDelegateDef.AddParameter(new ParameterBuilder(parameter.parameterTypeName, safeParameterName));
                    }
                    graphClass.AddField(eventDelegateDef);

                    //delayed delegate
                    DelegateBuilder eventDelegateDefDelayed = new DelegateBuilder("void", FieldBuilder.AccessibilityValues.Public, safeEventName + "DelayedDelegate");
                    eventDelegateDefDelayed.AddParameter(new ParameterBuilder("System.Double", "time"));
                    foreach (GraphEvent.EventParameterDef parameter in gevent.parameters)
                    {
                        string safeParameterName = ReflectionUtils.RemoveSpecialCharacters(parameter.parameterName);
                        eventDelegateDefDelayed.AddParameter(new ParameterBuilder(parameter.parameterTypeName, safeParameterName));
                    }
                    graphClass.AddField(eventDelegateDefDelayed);

                    graphClass.AddField(new NewLineBuilder());
                    graphClass.AddField(new FieldBuilder(FieldBuilder.AccessibilityValues.Public, false, false, safeEventName + "Delegate", "on" + safeEventName, "null"));
                    graphClass.AddField(new FieldBuilder(FieldBuilder.AccessibilityValues.Public, false, false, safeEventName + "DelayedDelegate", "on" + safeEventName+"Delayed", "null"));
                    graphClass.AddField(new NewLineBuilder());
                }


                // Callback Setup Function
                foreach (GraphEvent gevent in graphEntry.soundGraph.graphInput.events.Where(x => x.expose))
                {

                    string safeEventName = ReflectionUtils.RemoveSpecialCharacters(gevent.eventName);
                    MethodBuilder creationMethod = new MethodBuilder("void", false, false, FieldBuilder.AccessibilityValues.Private, "On" + safeEventName + "Internal");
                    creationMethod.AddParameter(new ParameterBuilder("double", "time"));
                    creationMethod.AddParameter(new ParameterBuilder("Dictionary<string, object>", "data"));

                    foreach(GraphEvent.EventParameterDef parameter in gevent.parameters)
                    {
                        string safeParameterName = ReflectionUtils.RemoveSpecialCharacters(parameter.parameterName);

                        GraphVariableValue value = ValueUtility.GetVariableValue(parameter.parameterTypeName, ValueUtility.ValueFilter.All);
                        string initializer = value != null ? value.GetValueInitializationString() : "null";

                        creationMethod.AddContent(new ArbitraryLineBuilder("object " + safeParameterName + " = "+ initializer + ";"));
                        creationMethod.AddContent(new ArbitraryLineBuilder("data.TryGetValue(\""+ safeParameterName + "\", out "+ safeParameterName + ");"));

                    }

                    // building delayed and regular function
                    string callbackParamsString = "";

                    foreach (GraphEvent.EventParameterDef parameter in gevent.parameters)
                    {
                        string safeParameterName = ReflectionUtils.RemoveSpecialCharacters(parameter.parameterName);
                        callbackParamsString += "(" + parameter.parameterTypeName + ")" + safeParameterName + ",";
                    }

                    callbackParamsString = callbackParamsString.Length != 0 ? callbackParamsString.Substring(0, callbackParamsString.Length - 1) : "";

                    // immediate function
                    creationMethod.AddContent(new ArbitraryLineBuilder("StartCoroutine(SymphonyUtils.WaitForDSPTime(time, () => {"));
                    string callEventFunction = "    on" + safeEventName + "?.Invoke(" + callbackParamsString + ");";
                    creationMethod.AddContent(new ArbitraryBuilder(new List<string>(new string[] { callEventFunction })));
                    creationMethod.AddContent(new ArbitraryLineBuilder("}));"));

                    // delayed
                    callbackParamsString = string.IsNullOrEmpty(callbackParamsString) ? "time" : "time, " + callbackParamsString;
                    string callEventDelayedFunction = "on" + safeEventName + "Delayed?.Invoke(" + callbackParamsString + ");";
                    creationMethod.AddContent(new ArbitraryBuilder(new List<string>(new string[] { callEventDelayedFunction })));

                    graphClass.AddField(creationMethod);
                }

                //Awake method
                MethodBuilder awakeMethod = new MethodBuilder("void", false, true, FieldBuilder.AccessibilityValues.Protected, "Awake");
                awakeMethod.AddContent(new ArbitraryLineBuilder("base.Awake();"));
                foreach (GraphEvent gevent in graphEntry.soundGraph.graphInput.events.Where(x => x.expose))
                {
                    string safeEventName = ReflectionUtils.RemoveSpecialCharacters(gevent.eventName);
                    awakeMethod.AddContent(new ArbitraryLineBuilder("RegisterEventListener(\""+ gevent.eventName + "\", On"+safeEventName+"Internal);"));
                    
                }

                graphClass.AddField(awakeMethod);

            }

            //Graph asset ID
            graphClass.AddField(new FieldBuilder(FieldBuilder.AccessibilityValues.Private, false, true, "string", "graphAssetID", string.Format("\"{0}\"", assetGUID)));

            //Graph setup method
            List<FieldBase> setupMethodContent = new List<FieldBase>();
            setupMethodContent.Add(new ArbitraryLineBuilder("#if UNITY_EDITOR"));
            setupMethodContent.Add(new ArbitraryLineBuilder("soundGraph = (SoundGraph)UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(graphAssetID), typeof(SoundGraph));"));
            setupMethodContent.Add(new ArbitraryLineBuilder("#endif"));
            MethodBuilder setupMethod = new MethodBuilder("void", false, true, FieldBuilder.AccessibilityValues.Protected, "LoadGraph", setupMethodContent, new ParameterBuilder[] { });
            graphClass.AddField(setupMethod);

            // Writing variables
            GraphVariable[] variables = graphEntry.soundGraph.graphInput.variables.Where(x => x.expose != GraphVariableBase.ExposureTypes.DoNotExpose).ToArray();
            foreach (GraphVariable variable in variables)
            {
                string typename = variable.typeName;
                if (variable.typeName == typeof(List<GraphVariable>).FullName)
                    typename = string.Format("GraphArray<{0}>", variable.arrayType);

                typename = typename.Replace("+", ".");

                string safePropName = ReflectionUtils.RemoveSpecialCharacters(variable.name);
                if (variable.expose == GraphVariableBase.ExposureTypes.AsInput)
                {
                    ArbitraryLineBuilder set = new ArbitraryLineBuilder(string.Format("SetVariable(\"{0}\",{1});", variable.name, "value"));
                    ArbitraryLineBuilder get = new ArbitraryLineBuilder(string.Format("return ({0})GetVariable(\"{1}\");", typename, variable.name));
                    PropertyBuilder property = new PropertyBuilder(FieldBuilder.AccessibilityValues.Public, false, typename, safePropName, new PropertyGetBuild(get), new PropertySetBuild(set));
                    graphClass.AddField(property);
                }
                else
                {
                    ArbitraryLineBuilder get = new ArbitraryLineBuilder(string.Format("return ({0})GetVariable(\"{1}\");", typename, variable.name));
                    PropertyBuilder property = new PropertyBuilder(FieldBuilder.AccessibilityValues.Public, false, typename, safePropName, new PropertyGetBuild(get), null);
                    graphClass.AddField(property);
                }
            }

            // full parameter methods
            foreach (GraphEvent gevent in graphEntry.soundGraph.graphInput.events.Where(x => x.expose))
            {
                if (gevent.parameters.Count != 0)
                {
                    graphClass.AddField(WriteEventMethod(gevent, true, true));
                    graphClass.AddField(WriteEventMethod(gevent, false, true));
                }
                else
                {
                    graphClass.AddField(WriteEventMethod(gevent, false, false));
                    graphClass.AddField(WriteEventMethod(gevent, true, false));
                }
            }

            // Stop all Method
            graphClass.AddField(WriteStopAllMethods(false));
            graphClass.AddField(WriteStopAllMethods(true));

            foreach(Node node in graphEntry.soundGraph.graphInput.soundGraph.nodes)
            {
                if (node == null)
                    continue;

                FlowNodeCodeGenerator generator = FlowNodeCodeGenerator.GetGenerator(node.GetType());
                generator?.InsertPlayerCode(graphClass, node as FlowNode);
            }

            return graphClass;
        }

    
        private static MethodBuilder WriteStopAllMethods(bool includeTime)
        {
            MethodBuilder stopAllMethod = new MethodBuilder("void", false, false, FieldBuilder.AccessibilityValues.Public, "StopAll");
            if (includeTime)
                stopAllMethod.AddParameter(new ParameterBuilder("System.Double", "time"));
            stopAllMethod.AddContent(new ArbitraryLineBuilder(string.Format("TriggerEvent(\"EndAll\", {0}, new Dictionary<string,object>());", includeTime?"time":"AudioSettings.dspTime")));
            return stopAllMethod;
        }

        private static MethodBuilder WriteEventMethod(GraphEvent gevent, bool includeStartTime, bool includeParameters)
        {
            string safeEventName = ReflectionUtils.RemoveSpecialCharacters(gevent.eventName);
            List<ParameterBuilder> parameters = new List<ParameterBuilder>();

            if (includeStartTime)
                parameters.Add(new ParameterBuilder("System.Double", "startTime"));


            if (includeParameters)
            {
                foreach (GraphEvent.EventParameterDef parameterDefs in gevent.parameters)
                {
                    string safeParameterName = ReflectionUtils.RemoveSpecialCharacters(parameterDefs.parameterName);
                    parameters.Add(new ParameterBuilder(parameterDefs.parameterTypeName, safeParameterName));
                }
            }


            List<FieldBase> content = new List<FieldBase>();

            content.Add(new ArbitraryLineBuilder("Dictionary<string, object> parameters = new Dictionary<string, object>();"));

            if (includeParameters)
            {
                foreach (GraphEvent.EventParameterDef parameterDefs in gevent.parameters)
                {
                    string safeParameterName = ReflectionUtils.RemoveSpecialCharacters(parameterDefs.parameterName);
                    content.Add(new ArbitraryLineBuilder(string.Format("parameters.Add(\"{0}\",{1});", parameterDefs.parameterName, safeParameterName)));
                }
            }


            content.Add(new ArbitraryLineBuilder(string.Format("TriggerEvent(\"{0}\", {1}, parameters);", 
                gevent.eventName, 
                includeStartTime? "startTime":"AudioSettings.dspTime")));

            return new MethodBuilder("void", false,false, FieldBuilder.AccessibilityValues.Public, safeEventName, content, parameters.ToArray());
        }



        
    }
}
