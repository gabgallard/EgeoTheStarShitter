using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using ABXY.Layers.Runtime.Nodes.Flow;
using UnityEngine;
using ABXY.Layers.ThirdParty.Malee.List;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace ABXY.Layers.Editor.Node_Editors.Flow
{
    [NodeEditor.CustomNodeEditor(typeof(WaitForAll))]
    public class WaitForAllEditor : FlowNodeEditor
    {
        NodePort playFinishedPort;
        NodePort reset;
        ReorderableList ports;

        public override void OnCreate()
        {
            base.OnCreate();
            playFinishedPort = target.GetOutputPort("playFinished");
            reset = target.GetInputPort("reset");

            ports = new ReorderableList(serializedObjectTree.FindProperty("branches"));
            ports.onAddCallback += OnAddPort;
            ports.onRemoveCallback += OnRemovePort;
            ports.drawElementCallback += OnDrawElement;
            ports.getElementHeightCallback += GetPortHeight;
            ports.draggable = false;
            LoadList();
        }

        private void LoadList()
        {
            List<NodePort> branches = (target as WaitForAll).branches;
            branches.Clear();
            foreach (NodePort port in target.DynamicInputs)
            {
                branches.Add(port);
            }
            serializedObjectTree.UpdateIfRequiredOrScript();
        }

        private float GetPortHeight(SerializedPropertyTree element)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        private void OnDrawElement(Rect rect, SerializedPropertyTree element, GUIContent label, bool selected, bool focused)
        {

            string fieldName = element.FindPropertyRelative("_fieldName").stringValue;
            EditorGUI.LabelField(rect, "Branch");
            NodeEditorGUILayout.PortField(new Vector2(rect.x -31 , rect.y), target.GetInputPort(fieldName));
        }

        private void OnRemovePort(ReorderableList list)
        {
            List<NodePort> branches = (target as WaitForAll).branches;
            List<string> fieldNamesToRemove = list.Selected.Select(x => branches[x].fieldName).ToList();

            foreach (string fieldName in fieldNamesToRemove)
            {
                branches.RemoveAll(x => x.fieldName == fieldName);
                target.RemoveDynamicPort(fieldName);
            }
            serializedObjectTree.UpdateIfRequiredOrScript();
        }

        private void OnAddPort(ReorderableList list)
        {
            NodePort port = target.AddDynamicInput(typeof(LayersEvent), Node.ConnectionType.Multiple, Node.TypeConstraint.Strict, System.Guid.NewGuid().ToString());
            (target as WaitForAll).branches.Add(port);
            serializedObjectTree.UpdateIfRequiredOrScript();
        }


        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObjectTree.UpdateIfRequiredOrScript();
            //NodeEditorGUILayout.PortField(new GUIContent("Out"), playFinishedPort);
            //NodeEditorGUILayout.PortField(new GUIContent("Flow"), target.GetInputPort("input"));
            //NodeEditorGUILayout.PortField(new GUIContent("Flow"), target.GetOutputPort("output"));

            ports.DoList(layout.Draw(ports.GetHeight()), new GUIContent("Branches"));

            NodeEditorGUIDraw.PortPair(layout.DrawLine(), new GUIContent("Reset"), reset, new GUIContent("Out"), playFinishedPort, serializedObjectTree);
            serializedObjectTree.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 160;
        }
    }
}
