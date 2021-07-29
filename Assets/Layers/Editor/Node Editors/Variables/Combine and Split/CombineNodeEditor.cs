using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Editor.Graph_Variable_Editors;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editors.Variables.Combine_and_Split
{
    [NodeEditor.CustomNodeEditorAttribute(typeof(CombineNode))]
    public class CombineNodeEditor : CombineSplitEditorBase
    {

        SerializedProperty typeNameProp;

        GraphVariableEditor editor;

        public override void OnCreate()
        {
            base.OnCreate();
            typeNameProp = serializedObject.FindProperty("typeName");


            editor = LoadEditor(typeNameProp.stringValue);
            if (editor != null && target.DynamicPorts.Count() == 0)
                ReloadPorts();


        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();
          
            editor = LoadEditor(typeNameProp.stringValue);


            LayersGUIUtilities.DrawExpandableProperty(serializedObject, () => {
                LayersGUIUtilities.DrawTypeSelector(typeNameProp, "Type", VariableInspectorUtility.EditorFilter.Combineable, () => {
                    editor = LoadEditor(typeNameProp.stringValue);
                    if (editor == null)
                        return;

                    (target as CombineNode).ClearDefaults();
                    ReloadPorts();
                });
            });

            VariableInspectorDrawFunctions.CombinableFNs.DrawCombineGUI(target as CombineNode, editor, this);
            serializedObject.ApplyModifiedProperties();

        }

        public override void ReloadPorts()
        {
            ReconnectionUtility reconnector = new ReconnectionUtility(target);
            target.ClearDynamicPorts();
            
            List<PortDefinition> portDefs = VariableInspectorDrawFunctions.CombinableFNs.GetCombinePorts(target as CombineNode, editor, this);

            foreach(PortDefinition port in portDefs)
            {
                if (port.direction == NodePort.IO.Input)
                {
                    target.AddDynamicInput(port.valueType, port.connectionType, port.typeConstraint, port.fieldName);
                }
                else
                {
                    target.AddDynamicOutput(port.valueType, port.connectionType, port.typeConstraint, port.fieldName);
                }
            }



            (target as CombineNode).BuildDefaults();

            reconnector.Reload();

        }

        private GraphVariableEditor LoadEditor(string targetTypeName)
        {
            if (editor == null || editor.handlesType.FullName != targetTypeName)
            {
                System.Type editorType = VariableInspectorUtility.GetEditorType(targetTypeName, VariableInspectorUtility.EditorFilter.Combineable);
                if (editorType != null)
                {
                    editor = (GraphVariableEditor)System.Activator.CreateInstance(editorType);
                }
            }
            return editor;
        }

    }
    public struct CombineNodeData
    {
        private CombineNode node;
        private CombineNodeEditor editor;
        public Dictionary<string, string> arbitraryStrings { get { return node.arbitraryStrings; } }

        public SerializedObject nodeSerializedObject { get { return editor.serializedObject; } }

        public CombineNodeData(CombineNode node, CombineNodeEditor editor)
        {
            this.node = node ?? throw new System.ArgumentNullException(nameof(node));
            this.editor = editor ?? throw new System.ArgumentNullException(nameof(editor));
        }

        public NodePort GetOutputPort(string fieldName)
        {
            return node.GetOutputPort(fieldName);
        }

        public NodePort GetInputPort(string fieldName)
        {
            return node.GetInputPort(fieldName);
        }

        public NodePort GetPort(string fieldName)
        {
            return node.GetPort(fieldName);
        }

        public bool HasPort(string fieldName)
        {
            return node.HasPort(fieldName);
        }

        public T GetInputValue<T>(string fieldName, T fallback = default(T))
        {
            return node.GetInputValue<T>(fieldName, fallback);
        }

        public object GetInputValue(string fieldName, object fallback = null)
        {
            return node.GetInputValue(fieldName, fallback);
        }

        public T[] GetInputValues<T>(string fieldName, params T[] fallback)
        {
            return node.GetInputValues<T>(fieldName, fallback);
        }



        public void ReloadPorts()
        {
            editor.ReloadPorts();
        }
    }
}