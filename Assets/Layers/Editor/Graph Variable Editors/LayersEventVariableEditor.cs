using System;
using System.Collections.Generic;
using ABXY.Layers.Editor.Node_Editors.Variables.Combine_and_Split;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEditor;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using System.Text;
using ABXY.Layers.ThirdParty.Malee.List;
using UnityEngine;
using System.Linq;

namespace ABXY.Layers.Editor.Graph_Variable_Editors
{
    public class LayersEventVariableEditor : GraphVariableEditor, SplittableInspector, CombinableInspector
    {
        public override Type handlesType => typeof(LayersEvent);

        #region Combine node stuff

        ReorderableList parameterList;

        CombineSplitData combineNodeData;
        public void DrawCombineGUI(CombineSplitData data)
        {
            data.nodeSerializedObject.UpdateIfRequiredOrScript();
            combineNodeData = data;
            InitializeList(data);
            NodeEditorGUILayout.PortPair(data.GetInputPort("Input"), data.GetOutputPort("Output"));

            LayersGUIUtilities.DrawExpandableProperty(parameterList.List.arraySize != 0, data.nodeSerializedObject, () => {
                parameterList.DoLayoutList();
            });
            data.nodeSerializedObject.ApplyModifiedProperties();
        }

        private void InitializeList(CombineSplitData data)
        {
            if (parameterList == null)
            {
                parameterList = new ReorderableList(data.nodeSerializedObject.FindProperty("parameters"));
                parameterList.drawElementCallback += DrawParameter;
                parameterList.getElementHeightCallback += CalculateParameterHeight;
                parameterList.onAddCallback += OnAddParameterElement;
                parameterList.onRemoveCallback += OnRemoveParameterElement;
            }
        }

        private void OnRemoveParameterElement(ReorderableList list)
        {
            list.Remove(list.Selected);
            parameterList.List.serializedObject.ApplyModifiedProperties();
            combineNodeData.ReloadPorts();
        }

        private void DrawParameter(Rect position, SerializedPropertyTree property, GUIContent label, bool selected, bool focused)
        {
            Rect nameRect = new Rect(position.x, position.y, (position.width / 2f) - EditorGUIUtility.standardVerticalSpacing, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), new GUIContent());

            SerializedProperty variableID = property.FindPropertyRelative("variableID");
            NodeEditorGUILayout.PortField(new Vector2(nameRect.x - 45, nameRect.y + EditorGUIUtility.standardVerticalSpacing), combineNodeData.GetInputPort(variableID.stringValue));


            Rect dropdownRect = new Rect(position.x + (position.width / 2f), position.y, position.width / 2f, EditorGUIUtility.singleLineHeight);

            List<string> typeNames = VariableInspectorUtility.GetManagedTypes(VariableInspectorUtility.EditorFilter.All);
            List<string> prettyNames = typeNames.Select(x => VariableInspectorUtility.GetPrettyName(x)).ToList();

            string currentTypeName = property.FindPropertyRelative("typeName").stringValue;
            int currentSelectionIndex = typeNames.IndexOf(currentTypeName);
            if (currentSelectionIndex < 0)
            {
                currentSelectionIndex = 0;
                property.FindPropertyRelative("typeName").stringValue = typeNames[currentSelectionIndex];
                property.FindPropertyRelative("typeName").serializedObject.ApplyModifiedProperties();
            }

            LayersGUIUtilities.DrawDropdown(dropdownRect, currentSelectionIndex, prettyNames.ToArray(), false, (newSelection) => {
                property.FindPropertyRelative("typeName").stringValue = typeNames[newSelection];
                property.FindPropertyRelative("typeName").serializedObject.ApplyModifiedProperties();
                combineNodeData.ReloadPorts();
            });

            GraphVariableEditor editor = LoadEditor(property);

            NodePort inputPort = combineNodeData.GetInputPort(variableID.stringValue);

            if (editor == null || (inputPort != null && combineNodeData.GetInputPort(variableID.stringValue).IsConnected))
                return;
            
            float editorHeight = VariableInspectorDrawFunctions.InputNodeFNs.GetValueHeight(combineNodeData.nodeSerializedObject.targetObject, property, "Value",editor, GraphVariable.RetrievalTypes.ActualValue, combineNodeData.isRunningSoundgraph );

            //checking for null Values

            Rect propertyRect = new Rect(position.x, position.y + dropdownRect.height + EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.standardVerticalSpacing, position.width, editorHeight);
            VariableInspectorDrawFunctions.InputNodeFNs.DrawValue(propertyRect, combineNodeData.nodeSerializedObject.targetObject, "Value", property, editor, GraphVariable.RetrievalTypes.ActualValue, combineNodeData.isRunningSoundgraph);


        }

        private float CalculateParameterHeight(SerializedPropertyTree property)
        {

            float emptyHeight = (EditorGUIUtility.singleLineHeight * 1) + (EditorGUIUtility.standardVerticalSpacing * 3f);

            GraphVariableEditor editor = LoadEditor(property);

            NodePort parameterPort = combineNodeData.GetInputPort(property.FindPropertyRelative("variableID").stringValue);

            if (editor == null || property == null || (parameterPort != null && parameterPort.IsConnected))
                return emptyHeight;

            return VariableInspectorDrawFunctions.InputNodeFNs.GetValueHeight(combineNodeData.nodeSerializedObject.targetObject, property, "Value", editor, GraphVariable.RetrievalTypes.ActualValue, combineNodeData.isRunningSoundgraph) + emptyHeight;
        }

        private void OnAddParameterElement(ReorderableList list)
        {

            SerializedProperty prop = list.AddItem();
            SerializedProperty eventName = prop.FindPropertyRelative("name");
            eventName.stringValue = "Parameter-" + UnityEngine.Random.Range(1, 1000);

            SerializedProperty variableID = prop.FindPropertyRelative("variableID");
            variableID.stringValue = System.Guid.NewGuid().ToString();


            SerializedProperty variableType = prop.FindPropertyRelative("typeName");
            variableType.stringValue = typeof(bool).FullName;

            combineNodeData.nodeSerializedObject.ApplyModifiedProperties();

            combineNodeData.ReloadPorts();
        }

        private Dictionary<string, GraphVariableEditor> path2VariableEditor = new Dictionary<string, GraphVariableEditor>();
        private GraphVariableEditor LoadEditor(SerializedProperty property)
        {
            GraphVariableEditor editor = null;
            string targetTypeName = property.FindPropertyRelative("typeName").stringValue;
            if (!path2VariableEditor.TryGetValue(property.propertyPath, out editor) || editor.handlesType.FullName != targetTypeName)
            {
                System.Type editorType = VariableInspectorUtility.GetEditorType(targetTypeName, VariableInspectorUtility.EditorFilter.All);
                if (editorType != null)
                {
                    editor = (GraphVariableEditor)System.Activator.CreateInstance(editorType);


                    if (path2VariableEditor.ContainsKey(property.propertyPath))
                        path2VariableEditor[property.propertyPath] = editor;
                    else
                        path2VariableEditor.Add(property.propertyPath, editor);
                }
            }
            return editor;
        }

        
        public int GetCombineNodeWidth()
        {
            return 260;
        }

        public List<PortDefinition> GetCombinePorts(CombineSplitData data)
        {
            combineNodeData = data;
            List<PortDefinition> ports = new List<PortDefinition>();
            ports.Add(new PortDefinition("Input", typeof(LayersEvent), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("Output", typeof(LayersEvent), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));

            SerializedProperty paramsProp = combineNodeData.nodeSerializedObject.FindProperty("parameters");
            for (int index = 0; index < paramsProp.arraySize; index++)
            {
                SerializedProperty paramProp = paramsProp.GetArrayElementAtIndex(index);
                string variableID = paramProp.FindPropertyRelative("variableID").stringValue;
                string typeName = paramProp.FindPropertyRelative("typeName").stringValue;
                ports.Add(new PortDefinition(variableID, ReflectionUtils.FindType(typeName), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            }

            return ports;
        }

        #endregion

        #region split node stuff
        public List<PortDefinition> GetSplitPorts(CombineSplitData data)
        {
            List<PortDefinition> ports = new List<PortDefinition>();
            ports.Add(new PortDefinition("Event", typeof(LayersEvent), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("EventOut", typeof(LayersEvent), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));

            List<GraphEvent.EventParameterDef> parameterDefs = data.GetIncomingParameterDefsOnPort("Event");
            foreach(GraphEvent.EventParameterDef parameterDef in parameterDefs)
            {
                ports.Add(new PortDefinition(parameterDef.parameterName, ReflectionUtils.FindType( parameterDef.parameterTypeName), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            }

            return ports;
        }

        int lastParamHashCode = -1;
        public void DrawSplitGUI(CombineSplitData data)
        {
            List<GraphEvent.EventParameterDef> parameterDefs = data.GetIncomingParameterDefsOnPort("Event");
            int currentParamHashcode = GetParameterDefHashcode(parameterDefs);
            if (currentParamHashcode != lastParamHashCode)
            {
                data.ReloadPorts();
                lastParamHashCode = currentParamHashcode;
            }

            
            NodeEditorGUILayout.PortPair(data.GetInputPort("Event"), data.GetOutputPort("EventOut"));

            foreach (GraphEvent.EventParameterDef parameterdef in parameterDefs)
            {
                LayersGUIUtilities.DrawExpandableProperty(parameterdef.parameterName, data.nodeSerializedObject, () => {
                    NodeEditorGUILayout.PortField(data.GetOutputPort(parameterdef.parameterName));
                });
            }
        }
        private int GetParameterDefHashcode(List<GraphEvent.EventParameterDef> parameters)
        {
            StringBuilder builder = new StringBuilder();
            foreach (GraphEvent.EventParameterDef param in parameters)
                builder.Append(param.parameterName).Append(param.parameterTypeName);
            return builder.ToString().GetHashCode();
        }

        public int GetSplitNodeWidth()
        {
            return 208;
        }
        #endregion


        public object GetDefaultValue()
        {
            return null;
        }

        public override string GetPrettyTypeName()
        {
            return "Event";
        }


    }
}
