using System.Collections.Generic;
using System.IO;
using System.Linq;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Editor.Node_Editor_Window;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Playback;
using ABXY.Layers.Runtime.Nodes.Signal_Sources;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editors.Playback
{
    [NodeEditor.CustomNodeEditorAttribute(typeof(SubGraph))]
    public class SubgraphNodeEditor : FlowNodeEditor
    {

        //SerializedProperty subGraphProp;
        NodePort endAllPort;
        public override void OnCreate()
        {
            base.OnCreate();
            serializedObject.UpdateIfRequiredOrScript();
            endAllPort = target.GetInputPort("EndAll");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();

            SerializedPropertyTree subGraph = serializedObject.FindProperty("subGraph");
            LayersGUIUtilities.FastPropertyField(layout.DrawLine(), subGraph);

            SoundGraph targetSoundgraph = (target as SubGraph).soundGraph.isRunningSoundGraph ? (target as SubGraph).runtimeSoundGraph : (target as SubGraph).subGraph;

            if (targetSoundgraph != null && GUI.Button(layout.DrawLine(), "Open"))
            {
                Selection.activeObject = targetSoundgraph;


                SoundGraphEditorWindow.Open(targetSoundgraph);

            }

            if (subGraph.objectReferenceValue == null && GUI.Button(layout.DrawLine(), "New SubGraph"))
            {

                string path = EditorUtility.SaveFilePanelInProject("Create new sound graph", "SubGraph", "asset", "Create new sound graph");
                if (!string.IsNullOrEmpty(path))
                {
                    SoundGraph newSoundGraph = ScriptableObject.CreateInstance<SoundGraph>();
                    newSoundGraph.name = Path.GetFileNameWithoutExtension(path);


                    AssetDatabase.CreateAsset(newSoundGraph, Path.Combine(path));

                    //newSoundGraph.SetupAsSubgraph(target as SubGraph, null);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    subGraph.objectReferenceValue = newSoundGraph;
                }
            }


            if (subGraph.objectReferenceValue == null)
            {
                for (int index = 0; index < target.DynamicPorts.Count(); index++)
                    target.RemoveDynamicPort(target.DynamicPorts.ElementAt(index));
            }

            if (subGraph.objectReferenceValue == null)
                return;


            //(subGraph.objectReferenceValue as SoundGraph).SetupAsSubgraph(target as SubGraph, null);

            DoPorts();

            serializedObject.ApplyModifiedProperties();



            /*serializedObject.Update();
        if (GUILayout.Button("Open"))
        {
            Selection.activeObject = serializedObject.FindProperty("subPlayer").objectReferenceValue;
        }

        EditorGUILayout.LabelField(target.DynamicInputs.Count() + "" + target.DynamicOutputs.Count());
        serializedObject.ApplyModifiedProperties();*/
        }

        private void DoPorts()
        {
            SerializedPropertyTree subGraphProp = serializedObject.FindProperty("subGraph");
            SoundGraph subGraph = (SoundGraph)subGraphProp.objectReferenceValue;


            GraphInputs[] inputs = subGraph.GetNodesOfType<GraphInputs>();
            GraphVariable[] variables = inputs.SelectMany(x => x.variables).Where(x=>x.expose != GraphVariableBase.ExposureTypes.DoNotExpose) .ToArray();
            GraphEvent[] events = inputs.SelectMany(x => x.events).Where(x=>x.expose).ToArray();

            EditorGUI.LabelField(layout.DrawLine(), "Subgraph events", EditorStyles.boldLabel);


            foreach (GraphEvent graphEvent in events)
            {
                List<GraphEvent.EventParameterDef> incomingParameters = (target as FlowNode).GetIncomingEventParameterDefsOnPort(graphEvent.eventID + "In", new List<Node>());
                MakeInputEventPort(graphEvent);
                MakeOutputEventPort(graphEvent);

                NodeEditorGUIDraw.PortPair(layout.DrawLine(), new GUIContent(graphEvent.eventName + " in"), 
                    target.GetInputPort(graphEvent.eventID + "In"), new GUIContent("Out"), 
                    target.GetOutputPort(graphEvent.eventID + "Out"), serializedObjectTree);

                foreach (GraphEvent.EventParameterDef parameter in graphEvent.parameters)
                {
                    if (incomingParameters.FindAll(x => x.parameterName == parameter.parameterName && x.parameterTypeName == parameter.parameterTypeName).Count == 0)
                        EditorGUI.HelpBox(layout.DrawLine(), string.Format("Parameter {0} of type {1} does not exist in incoming flow", parameter.parameterName, parameter.parameterTypeName), MessageType.Error);
                }
            }

            NodeEditorGUIDraw.PortField(layout.DrawLine(), endAllPort, serializedObjectTree);


            if (variables.Length != 0)
            {
                EditorGUI.LabelField(layout.DrawLine(), "Subgraph variables", EditorStyles.boldLabel);
                foreach (GraphVariable variable in variables)
                {

                    MakeInputVariablePort(variable);
                    MakeOutputVariablePort(variable);
                    NodeEditorGUIDraw.PortPair(layout.DrawLine(), 
                        new GUIContent(variable.name + " in"), target.GetInputPort(variable.variableID + "In"),
                        new GUIContent(variable.name + " Out"), target.GetOutputPort(variable.variableID + "Out"), 
                        serializedObjectTree);
                }
            }
        }

        private void MakeInputVariablePort(GraphVariable variable)
        {
            string variableInId = variable.variableID + "In";
            System.Type expectedType = ReflectionUtils.FindType(variable.typeName);

            if (typeof(System.Collections.IList).IsAssignableFrom(expectedType))
                expectedType = ReflectionUtils.FindType(variable.arrayType).MakeArrayType();

            NodePort port = target.GetInputPort(variableInId);

            if (port != null && (port.ValueType != expectedType || variable.expose != GraphVariable.ExposureTypes.AsInput))
            {
                target.RemoveDynamicPort(variableInId);
                port = null;
            }

            if (port == null && variable.expose == GraphVariable.ExposureTypes.AsInput)
            {
                target.AddDynamicInput(expectedType, Node.ConnectionType.Override, Node.TypeConstraint.Inherited, variableInId);
            }
        }

        private void MakeOutputVariablePort(GraphVariable variable)
        {
            string variableOutId = variable.variableID + "Out";
            System.Type expectedType = ReflectionUtils.FindType(variable.typeName);

            if (typeof(System.Collections.IList).IsAssignableFrom(expectedType))
                expectedType = ReflectionUtils.FindType(variable.arrayType).MakeArrayType();

            NodePort port = target.GetOutputPort(variableOutId);

            if (port != null && (port.ValueType != expectedType || variable.expose != GraphVariable.ExposureTypes.AsOutput))
            {
                target.RemoveDynamicPort(variableOutId);
                port = null;
            }

            if (port == null && variable.expose == GraphVariable.ExposureTypes.AsOutput)
            {
                target.AddDynamicOutput(expectedType, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited, variableOutId);
            }
        }

        private void MakeInputEventPort(GraphEvent graphEvent)
        {
            string eventID = graphEvent.eventID + "In";

            NodePort port = target.GetInputPort(eventID);

            if (port != null && !graphEvent.expose)
                target.RemoveDynamicPort(port);

            if (port == null && graphEvent.expose)
            {
                target.AddDynamicInput(typeof(LayersEvent), Node.ConnectionType.Multiple, Node.TypeConstraint.Strict, eventID);
            }
        }

        private void MakeOutputEventPort(GraphEvent graphEvent)
        {
            string eventID = graphEvent.eventID + "Out";

            NodePort port = target.GetOutputPort(eventID);

            if (port != null && !graphEvent.expose)
                target.RemoveDynamicPort(port);

            if (port == null && graphEvent.expose)
            {
                target.AddDynamicOutput(typeof(LayersEvent), Node.ConnectionType.Multiple, Node.TypeConstraint.Strict, eventID);
            }
        }



        public override int GetWidth()
        {
            return 200;
        }
    }
}
