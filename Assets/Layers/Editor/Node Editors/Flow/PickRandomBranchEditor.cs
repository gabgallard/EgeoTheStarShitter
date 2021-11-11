using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using ABXY.Layers.Runtime.Nodes.Flow;
using UnityEditor;
using UnityEngine;
using ABXY.Layers.ThirdParty.Malee.List;
using System.Collections.Generic;
using System.Linq;

namespace ABXY.Layers.Editor.Node_Editors.Flow
{
    [NodeEditor.CustomNodeEditor(typeof(PickRandomBranch))]
    public class PickRandomBranchEditor : FlowNodeEditor
    {
        NodePort playPort;
        ReorderableList ports;

        public override void OnCreate()
        {
            base.OnCreate();
            playPort = target.GetInputPort("play");
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
            List<NodePort> branches = (target as PickRandomBranch).branches;
            branches.Clear();
            foreach(NodePort port in target.DynamicOutputs)
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
            EditorGUI.LabelField(rect, "Branch");
            NodeEditorGUILayout.PortField(new Vector2(rect.x + rect.width + 7, rect.y), target.GetOutputPort(fieldName));
        }

        private void OnRemovePort(ReorderableList list)
        {
            List<NodePort> branches = (target as PickRandomBranch).branches;
            List<string> fieldNamesToRemove = list.Selected.Select(x => branches[x].fieldName).ToList();

            foreach(string fieldName in fieldNamesToRemove)
            {
                branches.RemoveAll(x => x.fieldName == fieldName);
                target.RemoveDynamicPort(fieldName);
            }
            serializedObject.UpdateIfRequiredOrScript();
        }

        private void OnAddPort(ReorderableList list)
        {
            NodePort port = target.AddDynamicOutput(typeof(LayersEvent), Node.ConnectionType.Multiple, Node.TypeConstraint.Strict, System.Guid.NewGuid().ToString());
            (target as PickRandomBranch).branches.Add(port);
            serializedObject.UpdateIfRequiredOrScript();
        }

        public override void OnBodyGUI()
        {

            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();

            float labelWidth = EditorGUIUtility.labelWidth;
            NodeEditorGUIDraw.PortField(layout.DrawLine(), new GUIContent("In"), playPort, serializedObjectTree);
            EditorGUIUtility.labelWidth = 104f;
            SerializedPropertyTree dontRepeatProp = serializedObject.FindProperty("dontRepeat");
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), dontRepeatProp, new GUIContent("Don't Repeat"));

            ports.DoList(layout.Draw(ports.GetHeight()), new GUIContent("Branches"));
            //NodeEditorGUILayout.DynamicPortList("Out", typeof(LayersEvent), serializedObject, NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict);
            EditorGUIUtility.labelWidth = labelWidth;
            serializedObject.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 160;
        }
    }
}
