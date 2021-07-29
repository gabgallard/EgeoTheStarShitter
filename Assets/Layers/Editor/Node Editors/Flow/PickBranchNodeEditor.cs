using System.Collections.Generic;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.ThirdParty.Malee.List;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Flow;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editors.Flow
{
    [NodeEditor.CustomNodeEditor(typeof(PickBranch))]
    public class PickBranchNodeEditor : FlowNodeEditor
    {
        NodePort inputPort;
        ReorderableList nodeList;

        public override void OnCreate()
        {
            base.OnCreate();
            inputPort = target.GetInputPort("input");
            nodeList = new ReorderableList(serializedObjectTree.FindProperty("outputs"));
            nodeList.onAddCallback += OnAddOutput;
            nodeList.onRemoveCallback += OnRemovedOutput;
            nodeList.drawElementCallback += NodeList_drawElementCallback;
        }

        private void NodeList_drawElementCallback(Rect rect, SerializedPropertyTree element, GUIContent label, bool selected, bool focused)
        {
            int firstBracketLocation = element.propertyPath.IndexOf("[");
            string index = element.propertyPath.Substring(firstBracketLocation+1, element.propertyPath.Length - firstBracketLocation-2);
            EditorGUI.LabelField(rect, "Output " + (int.Parse( index) + 1));
            NodeEditorGUILayout.PortField(new Vector2(rect.x + rect.width + 5, rect.y), target.GetOutputPort(element.stringValue));
            //NodeEditorGUILayout.AddPortField(target.GetOutputPort(element.stringValue));
        }

        private void OnRemovedOutput(ReorderableList list)
        {
            serializedObjectTree.UpdateIfRequiredOrScript();
            for (int index = 0; index<list.Selected.Length; index++)
            {
                target.RemoveDynamicPort(list.List.GetArrayElementAtIndex(list.Selected[index]).stringValue);
            }
            serializedObjectTree.ApplyModifiedProperties();
            serializedObjectTree.UpdateIfRequiredOrScript();
            list.Remove(list.Selected);
            serializedObjectTree.ApplyModifiedProperties();
        }

        private void OnAddOutput(ReorderableList list)
        {
            list.List.InsertArrayElementAtIndex(list.List.arraySize);
            SerializedProperty outputProp = list.List.GetArrayElementAtIndex(list.List.arraySize - 1);
            string nodeName = System.Guid.NewGuid().ToString();
            outputProp.stringValue = nodeName;
            serializedObjectTree.ApplyModifiedProperties();
            serializedObjectTree.UpdateIfRequiredOrScript();
            target.AddDynamicOutput(typeof(LayersEvent), Node.ConnectionType.Multiple, Node.TypeConstraint.Strict, nodeName);
            serializedObjectTree.ApplyModifiedProperties();

        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObjectTree.UpdateIfRequiredOrScript();
            SerializedPropertyTree selectedBranch = serializedObjectTree.FindProperty("selectedBranch");
            NodeEditorGUIDraw.PortField(layout.DrawLine(), inputPort);

            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100;

            Rect selectedBranchRect = layout.DrawLine();
            this.DrawEventDerivableProperty(selectedBranchRect, selectedBranch, (target as FlowNode).GetIncomingEventParameterDefsOnPort("input", new List<Node>()), (drawRect) => {
                NodeEditorGUIDraw.PropertyField(drawRect, selectedBranch);
            });
            EditorGUIUtility.labelWidth = labelWidth;

            nodeList.DoList(layout.Draw(nodeList.GetHeight()), new GUIContent("Outputs"));
            serializedObjectTree.ApplyModifiedProperties();
        }


    }
}
