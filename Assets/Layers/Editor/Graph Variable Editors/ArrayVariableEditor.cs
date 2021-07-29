using System;
using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.ThirdParty.Malee.List;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes;
using UnityEditor;
using UnityEngine;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using ABXY.Layers.Editor.Node_Editors.Variables.Combine_and_Split;

namespace ABXY.Layers.Editor.Graph_Variable_Editors
{
    public class ArrayVariableEditor : GraphVariableEditor, PlayerInspector, InputNodeInspector, SplittableInspector
    {
        public override Type handlesType => typeof(List<GraphVariable>);

        //private ReorderableList arrayElementList;
        private GraphVariableList arrayVariableList;

       // private SerializedProperty arrayTypeProp;
        private Dictionary<string, GraphVariableEditor> path2VariableEditor = new Dictionary<string, GraphVariableEditor>();

      //  private SerializedProperty arrayGraphVarProp;

        public bool showTypeSelector = true;

        private static List<string> _allowedTypes = new List<string>();
        private static List<string> allowedTypes
        {
            get
            {
                if (_allowedTypes.Count == 0)
                {
                    _allowedTypes = VariableInspectorUtility.GetManagedTypes(VariableInspectorUtility.EditorFilter.All).Where(x => x != typeof(List<GraphVariable>).FullName && x != typeof(Transform).FullName).ToList();
                }
                return _allowedTypes;
            }
        }

        private static List<string> _prettyNames = new List<string>();
        private static List<string> prettyNames
        {
            get
            {
                if (_prettyNames.Count == 0)
                {
                    foreach (string allowedType in allowedTypes)
                    {
                        _prettyNames.Add(VariableInspectorUtility.GetPrettyName(allowedType));
                    }
                }
                return _prettyNames;
            }
        }

        /// <summary>
        /// If the rray has been expanded or contracted this frame, this will be true. Use this to ignore change detections triggered
        /// by expansion, if necessary.
        /// </summary>
        public bool expansionChangedThisFrame { get; private set; }

        public object GetDefaultValue()
        {
            return new List<GraphVariable>();
        }

        // Default value in input
        public void DrawInputNodeValue(Rect position, string label, VariableEdit edit)
        {
            edit.graphVariableProperty.serializedObject.ApplyModifiedProperties();
            //arrayGraphVarProp = edit.graphVariableProperty;
            edit.graphVariableProperty.serializedObject.UpdateIfRequiredOrScript();
            SetupReorderableList(edit, !edit.settingDefaultValue);

            BeginExpansionCheck();

            Rect selectorRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            if (showTypeSelector)
                LayersGUIUtilities.DrawTypeSelector(selectorRect, edit.graphVariableProperty.FindPropertyRelative("arrayType"), "Array Type", allowedTypes, prettyNames);

            //DrawTypeSelector(selectorRect, property);

            float listHeight = arrayVariableList.GetHeight();

            float typeSelectoryHeight = showTypeSelector ? EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing : 0f;

            Rect listRect = new Rect(position.x, position.y + typeSelectoryHeight, position.width, listHeight);

            arrayVariableList.typeConstraint = edit.graphVariableProperty.FindPropertyRelative("arrayType").stringValue;
            arrayVariableList.DoList(listRect, new GUIContent("Array " + label));

            edit.graphVariableProperty.serializedObject.ApplyModifiedProperties();

            EndExpansionCheck();
        }

        public float CalculateInputNodeValueHeight(VariableEdit edit, string label)
        {

            edit.graphVariableProperty.serializedObject.UpdateIfRequiredOrScript();
            SetupReorderableList(edit, !edit.settingDefaultValue);
            float height = showTypeSelector ? 2f * EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight : EditorGUIUtility.standardVerticalSpacing;

            height += arrayVariableList.GetHeight();


            edit.graphVariableProperty.serializedObject.ApplyModifiedProperties();
            return height;
        }



        // Draw in player

        public void DrawInPlayerInspector(Rect position, string label, VariableEdit edit)
        {
            edit.graphVariableProperty.serializedObject.ApplyModifiedProperties();
            // = edit.graphVariableProperty;
            edit.graphVariableProperty.serializedObject.UpdateIfRequiredOrScript();
            SetupReorderableList(edit);

            BeginExpansionCheck();

            Rect selectorRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            if (showTypeSelector)
                LayersGUIUtilities.DrawTypeSelector(selectorRect, edit.graphVariableProperty.FindPropertyRelative("arrayType"), "Array Type", allowedTypes, prettyNames);

            //DrawTypeSelector(selectorRect, property);

            float listHeight = arrayVariableList.GetHeight();

            float typeSelectoryHeight = showTypeSelector ? EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing : 0f;

            Rect listRect = new Rect(position.x, position.y + typeSelectoryHeight, position.width, listHeight);

            arrayVariableList.typeConstraint = edit.graphVariableProperty.FindPropertyRelative("arrayType").stringValue;
            arrayVariableList.DoList(listRect, new GUIContent("Array " + label));

            edit.graphVariableProperty.serializedObject.ApplyModifiedProperties();

            expansionChangedThisFrame = wasExpanded != arrayVariableList.expanded;

            EndExpansionCheck();
        }

        public float CalculateHeightInPlayerInspector(VariableEdit edit, string label)
        {
            edit.graphVariableProperty.serializedObject.UpdateIfRequiredOrScript();
            SetupReorderableList(edit);
            float height = showTypeSelector ? 2f * EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight : EditorGUIUtility.standardVerticalSpacing;

            height += arrayVariableList.GetHeight();


            edit.graphVariableProperty.serializedObject.ApplyModifiedProperties();
            return height;
        }


        /// <summary>
        /// Used to check if the variable was set to display the actual value, but has changed to the default, or vice versa. This allows us to regenerate the array var list;
        /// </summary>
        private bool lastActualValue = false;
        private void SetupReorderableList(VariableEdit edit, bool actualValue = false)
        {
            if (actualValue != lastActualValue)
                arrayVariableList = null;

            string arrayPropName = actualValue ? "arrayElements" : "defaultArrayElements";
            if (arrayVariableList == null)
            {
                //arrayTypeProp = property.FindPropertyRelative("arrayType");
                arrayVariableList = new GraphVariableList(edit.graphVariableProperty.FindPropertyRelative(arrayPropName), 
                    VariableInspectorDrawFunctions.InputNodeFNs.GetValueHeight,
                    VariableInspectorDrawFunctions.InputNodeFNs.DrawValue,
                    NodePort.IO.Input, true, 
                    edit.graphVariableProperty.FindPropertyRelative("arrayType").stringValue, Node.ShowBackingValue.Unconnected);

                arrayVariableList.expanded = false;
            }

            lastActualValue = actualValue;
        }

        



        private string GetArrayType(CombineSplitData data)
        {
            string typeValue = "";
            data.arbitraryStrings.TryGetValue("arrayType", out typeValue);


            if (string.IsNullOrEmpty(typeValue) || ReflectionUtils.FindType(typeValue) == null)
            {
                typeValue = typeof(bool).FullName;
                if (data.arbitraryStrings.ContainsKey("arrayType"))
                    data.arbitraryStrings["arrayType"] = typeValue;
                else
                    data.arbitraryStrings.Add("arrayType", typeValue);
                data.ReloadPorts();
            }
            return typeValue;
        }


        //TODO: organizing the inputs to this function
        public void DrawSplitGUI(CombineSplitData data)
        {
            string typeValue = GetArrayType(data);

            LayersGUIUtilities.DrawExpandableProperty(data.nodeSerializedObject, () => {
                LayersGUIUtilities.DrawTypeSelector(typeValue, "Array Type", allowedTypes, prettyNames, (newValue) => {
                    if (data.arbitraryStrings.ContainsKey("arrayType"))
                        data.arbitraryStrings["arrayType"] = newValue;
                    else
                        data.arbitraryStrings.Add("arrayType", newValue);
                    data.ReloadPorts();
                });
            });

            LayersGUIUtilities.DrawExpandableProperty(data.GetInputPort("arrayIn"), data.GetOutputPort("value"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetInputPort("index"), data.GetOutputPort("length"), data.nodeSerializedObject);



        }

        public List<PortDefinition> GetSplitPorts(CombineSplitData data)
        {
            string typeValue = GetArrayType(data);

            List<PortDefinition> ports = new List<PortDefinition>();
            ports.Add(new PortDefinition("arrayIn", ReflectionUtils.FindType(typeValue).MakeArrayType(), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("index", typeof(int), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("value", ReflectionUtils.FindType(typeValue), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("length", typeof(int), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            return ports;
        }


        public override string GetPrettyTypeName()
        {
            return "Array";
        }

        public int GetSplitNodeWidth()
        {
            return 208;
        }

        bool wasExpanded;
        /// <summary>
        /// Used to set the expansionChangedThisFrame variable
        /// </summary>
        private void BeginExpansionCheck()
        {
            expansionChangedThisFrame = false;
            wasExpanded = arrayVariableList.expanded;
        }
        /// <summary>
        /// Used to set the expansionChangedThisFrame variable
        /// </summary
        private void EndExpansionCheck()
        {
            expansionChangedThisFrame = wasExpanded != arrayVariableList.expanded;
        }


    }
}
