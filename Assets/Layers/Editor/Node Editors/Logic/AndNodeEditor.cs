using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Logic;
using ABXY.Layers.ThirdParty.Malee.List;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace ABXY.Layers.Editor.Node_Editors.Logic
{
    [NodeEditor.CustomNodeEditor(typeof(AndNode))]
    public class AndNodeEditor : FlowNodeEditor
    {
        NodePort valuePort;
        ReorderableList ports;

        public override void OnCreate()
        {
            base.OnCreate();
            valuePort = target.GetOutputPort("value");

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
            List<NodePort> branches = (target as AndNode).branches;
            branches.Clear();
            foreach (NodePort port in target.DynamicInputs)
            {
                branches.Add(port);
            }
            serializedObject.UpdateIfRequiredOrScript();
        }

        private float GetPortHeight(SerializedPropertyTree element)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        private void OnDrawElement(Rect rect, SerializedPropertyTree element, GUIContent label, bool selected, bool focused)
        {

            string fieldName = element.FindPropertyRelative("_fieldName").stringValue;
            EditorGUI.LabelField(rect, "Input");
            NodeEditorGUILayout.PortField(new Vector2(rect.x-32, rect.y), target.GetInputPort(fieldName));
        }

        private void OnRemovePort(ReorderableList list)
        {
            List<NodePort> branches = (target as AndNode).branches;
            List<string> fieldNamesToRemove = list.Selected.Select(x => branches[x].fieldName).ToList();

            foreach (string fieldName in fieldNamesToRemove)
            {
                branches.RemoveAll(x => x.fieldName == fieldName);
                target.RemoveDynamicPort(fieldName);
            }
            serializedObject.UpdateIfRequiredOrScript();
        }

        private void OnAddPort(ReorderableList list)
        {
            NodePort port = target.AddDynamicInput(typeof(bool), Node.ConnectionType.Multiple, Node.TypeConstraint.Strict, System.Guid.NewGuid().ToString());
            (target as AndNode).branches.Add(port);
            serializedObject.UpdateIfRequiredOrScript();
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();
            NodeEditorGUIDraw.PortField(layout.DrawLine(),valuePort);


            ports.DoList(layout.Draw(ports.GetHeight()), new GUIContent("Inputs"));
            //NodeEditorGUILayout.DynamicPortList("Input", typeof(bool), serializedObject, NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Strict);//TODO: Switch to Malee implementation
            serializedObject.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 160;
        }
    }
}
