using ABXY.Layers.Editor.Node_Editor_Window;
using ABXY.Layers.Editor.Node_Editors;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.FlowTypes;
using ABXY.Layers.Runtime.Nodes.Playback;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.ThirdParty.Malee.List;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Editor.ThirdParty.Xnode;

namespace ABXY.Layers.Editor.Node_Editors.Playback
{
    [CustomNodeEditor(typeof(StateMachineNode))]
    public class StateMachineNodeEditor : FlowNodeEditor
    {
        ReorderableList states;

        //SerializedProperty transitionStyle;

        //SerializedProperty stateMachineName;

        //SerializedProperty stateEnumTypeName;


        public override void OnCreate()
        {
            base.OnCreate();
            if (!target.name.EndsWith(" (Beta)"))
                target.name += " (Beta)";
            states = new ReorderableList(serializedObjectTree.FindProperty("states"));
            states.drawElementCallback += DrawStateElement;
            states.getElementHeightCallback += GetStateElementHeight;
            states.onAddCallback += OnAddState;
            states.onRemoveCallback += OnRemoveState;
            states.onReorderCallback += States_onReorderCallback;
            states.expandable = false;
            SerializedPropertyTree transitionStyle = serializedObjectTree.FindProperty("stateMachineStyle");

            SerializedPropertyTree stateMachineName = serializedObjectTree.FindProperty("_stateMachineName");
            serializedObjectTree.UpdateIfRequiredOrScript();
            EditorGUI.BeginChangeCheck();
            if (string.IsNullOrEmpty(stateMachineName.stringValue))
                stateMachineName.stringValue = "State Machine-" + Random.Range(1, 1000);
            if (EditorGUI.EndChangeCheck())
                MarkNeedsCodeRegen();

            SerializedPropertyTree stateEnumTypeName = serializedObjectTree.FindProperty("statesEnumTypeName");

            SerializedProperty created = serializedObjectTree.FindProperty("created");
            if (!created.boolValue)
                MarkNeedsCodeRegen();
            created.boolValue = true;

            serializedObjectTree.ApplyModifiedProperties();
        }

        private void OnRemoveState(ReorderableList list)
        {
            foreach(int selection in list.Selected)
            {
                SerializedPropertyTree stateProp = list.List.GetArrayElementAtIndex(selection);
                stateContainers.Remove(stateProp.FindPropertyRelative("guid").stringValue);
            }
            list.Remove(list.Selected);
            MarkNeedsCodeRegen();
        }

        private void States_onReorderCallback(ReorderableList list)
        {
            stateContainers.Clear();
            MarkNeedsCodeRegen();
        }

        private void OnAddState(ReorderableList list)
        {
            SerializedPropertyTree newItem = list.AddItem();
            string newGUID = System.Guid.NewGuid().ToString();
            newItem.FindPropertyRelative("guid").stringValue = newGUID;
            newItem.serializedObject.ApplyModifiedPropertiesWithoutUndo();

            StateGUIContainer stateContainer = GetStateContainer(newItem);
            stateContainer.ResetValues();
            newItem.serializedObject.ApplyModifiedProperties();
            MarkNeedsCodeRegen();


        }

        private float GetStateElementHeight(SerializedPropertyTree element)
        {
            StateGUIContainer guiContainer = GetStateContainer(element);
            return guiContainer.GetStateHeight();
        }

        private void DrawStateElement(Rect rect, SerializedPropertyTree element, GUIContent label, bool selected, bool focused)
        {
            StateGUIContainer guiContainer = GetStateContainer(element);
            guiContainer.DrawStateElement(rect, selected,target);
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObjectTree.UpdateIfRequiredOrScript();
            
            SerializedPropertyTree transitionStyle = serializedObjectTree.FindProperty("stateMachineStyle");
            SerializedPropertyTree stateMachineName = serializedObjectTree.FindProperty("_stateMachineName");
            SerializedPropertyTree stateEnumTypeName = serializedObjectTree.FindProperty("statesEnumTypeName");

            stateEnumTypeName.stringValue = GetCurrentStatePortTypename();

            LayersGUIUtilities.BeginNewLabelWidth(150f);

            EditorGUI.BeginChangeCheck();
            LayersGUIUtilities.FastPropertyField(layout.DrawLine(), stateMachineName);
            if (EditorGUI.EndChangeCheck())
                MarkNeedsCodeRegen();

            LayersGUIUtilities.DrawDropdown(layout.DrawLine(),new GUIContent("State Machine Style"), transitionStyle);
            LayersGUIUtilities.EndNewLabelWidth();

            states.DoList(layout.Draw(states.GetHeight()), new GUIContent("States"));

            NodeEditorGUIDraw.PortField(layout.DrawLine(), target.GetOutputPort("onStateChange"));

            LayersGUIUtilities.DrawOrCreatePort(layout.DrawLine(), 0f, target, NodePort.IO.Output, ReflectionUtils.FindType(stateEnumTypeName.stringValue),
                Node.ConnectionType.Multiple, Node.TypeConstraint.Strict, "CurrentState", "Current State");


            serializedObjectTree.ApplyModifiedProperties();

            if (Event.current.type == EventType.Repaint)
                PrunePortList();
        }

        private void PrunePortList()
        {
            List<ExpectedPortInfo> portIDSInUse = new List<ExpectedPortInfo>();

            portIDSInUse.Add(new ExpectedPortInfo(NodePort.IO.Output, "CurrentState", GetCurrentStatePortTypename()));

            foreach (StateGUIContainer state in stateContainers.Values)
                portIDSInUse.AddRange(state.GetPortIDSInUse());

            List<string> portIDSToRemove = new List<string>();

            foreach(NodePort port in target.DynamicPorts)
            {
                ExpectedPortInfo targetExpectedPort = portIDSInUse.Find(x => x.fieldName == port.fieldName);
                if (!targetExpectedPort.isValid)
                    portIDSToRemove.Add(port.fieldName);
                else if (targetExpectedPort.direction  != port.direction)
                    portIDSToRemove.Add(port.fieldName);
                else if (targetExpectedPort.typeName != port.ValueType.FullName)
                    portIDSToRemove.Add(port.fieldName);

            }

            foreach (string portID in portIDSToRemove)
                target.RemoveDynamicPort(portID);
        }

        private string GetCurrentStatePortTypename()
        {
            SerializedPropertyTree stateMachineName = serializedObjectTree.FindProperty("_stateMachineName");
            return ReflectionUtils.RemoveSpecialCharacters((target as FlowNode).soundGraph.name) + "+"
                + ReflectionUtils.RemoveSpecialCharacters(stateMachineName.stringValue + "States");
        }

        public struct ExpectedPortInfo
        {
            public NodePort.IO direction;
            public string fieldName;
            public string typeName;
            public bool isValid;
            public ExpectedPortInfo(NodePort.IO direction, string fieldName, string typeName)
            {
                this.direction = direction;
                this.fieldName = fieldName;
                this.typeName = typeName;
                this.isValid = true;
            }
        }

        public override int GetWidth()
        {
            return 350;
        }


        private Dictionary<string, StateGUIContainer> stateContainers = new Dictionary<string, StateGUIContainer>();
        private StateGUIContainer GetStateContainer(SerializedPropertyTree state)
        {
            SerializedProperty guid = state.FindPropertyRelative("guid");
            if (!stateContainers.ContainsKey(guid.stringValue))
                stateContainers.Add(guid.stringValue, new StateGUIContainer(state, this));
            return stateContainers[guid.stringValue];
        }

        
        
    }
}