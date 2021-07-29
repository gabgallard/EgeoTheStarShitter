using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Editor.Graph_Variable_Editors;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using UnityEditor;
using static ABXY.Layers.Runtime.ThirdParty.XNode.Scripts.NodePort;
using System;

namespace ABXY.Layers.Editor.Node_Editors.Variables.Combine_and_Split
{
    [NodeEditor.CustomNodeEditorAttribute(typeof(SplitNode))]
    public class SplitNodeEditor : CombineSplitEditorBase {

        SerializedProperty typeNameProp;

        GraphVariableEditor editor;

        public override void OnCreate()
        {
            base.OnCreate();
            typeNameProp = serializedObject.FindProperty ("typeName"); 


            editor = LoadEditor(typeNameProp.stringValue);
            if (editor != null/* && target.DynamicPorts.Count() == 0*/) 
                ReloadPorts();


        }


        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();

            LayersGUIUtilities.DrawExpandableProperty(serializedObject, () => {
                LayersGUIUtilities.DrawTypeSelector(typeNameProp, "Type", VariableInspectorUtility.EditorFilter.Splittable, () => {
                    editor = LoadEditor(typeNameProp.stringValue);
                    if (editor == null)
                        return;

                    ReloadPorts();
                });
            });

            
        

            editor = LoadEditor(typeNameProp.stringValue);

            VariableInspectorDrawFunctions.SplittableFNs.DrawSplitGUI(target as SplitNode, editor, this);
            serializedObject.ApplyModifiedProperties();

        }

        protected override bool CanExpand()
        {
            return true;
        }

        public override void ReloadPorts()
        {
            /*
            target.ClearDynamicPorts();

            foreach (NodePort inPort in editor.GetSplitInputPorts(target as SplitNode, this))
                target.AddDynamicInput(inPort.ValueType, inPort.connectionType, inPort.typeConstraint, inPort.fieldName);

            foreach (NodePort outPort in editor.GetSplitOutputPorts(target as SplitNode, this))
                target.AddDynamicOutput(outPort.ValueType, outPort.connectionType, outPort.typeConstraint, outPort.fieldName);
            */

            NodeEditorWindow currentEditorWindow = NodeEditorWindow.current;
            if (currentEditorWindow != null)
            {;
                //currentEditorWindow.onLateGUI += () =>
                //{
                    List<PortDefinition> portsReportedByEditor = VariableInspectorDrawFunctions.SplittableFNs.GetSplitPorts(target as SplitNode, editor, this);

                    //portsReportedByEditor.AddRange((editor as SplittableInspector).GetSplitInputPorts(target as SplitNode, this));
                    //portsReportedByEditor.AddRange((editor as SplittableInspector).GetSplitOutputPorts(target as SplitNode, this));

                    List<PortDefinition> portsToAdd = new List<PortDefinition>();
                    List<string> portsToRemove = new List<string>();

                    foreach (PortDefinition editorPort in portsReportedByEditor)
                    {
                        NodePort currentPort = target.GetPort(editorPort.fieldName);
                        if (currentPort == null ||
                            currentPort.direction != editorPort.direction ||
                            currentPort.connectionType != editorPort.connectionType ||
                            currentPort.typeConstraint != editorPort.typeConstraint ||
                            currentPort.ValueType != editorPort.valueType)
                            portsToAdd.Add(editorPort);
                    }

                    foreach (NodePort currentPort in target.DynamicPorts)
                    {
                        PortDefinition editorPort = portsReportedByEditor.Find(x => x.fieldName == currentPort.fieldName);
                        if (editorPort == null ||
                            currentPort.direction != editorPort.direction ||
                            currentPort.connectionType != editorPort.connectionType ||
                            currentPort.typeConstraint != editorPort.typeConstraint ||
                            currentPort.ValueType != editorPort.valueType)
                            portsToRemove.Add(currentPort.fieldName);
                    }

                    foreach (string removedPort in portsToRemove)
                        target.RemoveDynamicPort(removedPort);

                    foreach (PortDefinition portToAdd in portsToAdd)
                    {
                        if (portToAdd.direction == NodePort.IO.Input)
                            target.AddDynamicInput(portToAdd.valueType, portToAdd.connectionType, portToAdd.typeConstraint, portToAdd.fieldName);
                        else
                            target.AddDynamicOutput(portToAdd.valueType, portToAdd.connectionType, portToAdd.typeConstraint, portToAdd.fieldName);
                    }
                //};
            }

            

        }

        private GraphVariableEditor LoadEditor(string targetTypeName)
        {
            if (editor == null || editor.handlesType.FullName != targetTypeName)
            {
                System.Type editorType = VariableInspectorUtility.GetEditorType(targetTypeName, VariableInspectorUtility.EditorFilter.Splittable);
                if (editorType != null)
                {
                    editor = (GraphVariableEditor)System.Activator.CreateInstance(editorType);

                }
            }
            return editor;
        }

        public override int GetWidth()
        {
            if (editor != null)
                return (editor as SplittableInspector).GetSplitNodeWidth();
            else
                return base.GetWidth();
        }

    }

    
}