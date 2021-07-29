using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Editor.Graph_Variable_Editors;
using ABXY.Layers.ThirdParty.Malee.List;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Graph_Variable_Values;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Math_Operations;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editors.Math_Operations
{
    [NodeEditor.CustomNodeEditor(typeof(MathOperationNode))]
    public abstract class MathOperationNodeEditor : FlowNodeEditor
    {
        ReorderableList inputList;

        //SerializedProperty typeNameProp;


        //SerializedProperty primaryVariableProp;

        //SerializedProperty secondaryVariableProp;

        private Dictionary<string, GraphVariableEditor> path2VariableEditor = new Dictionary<string, GraphVariableEditor>();

        public override void OnCreate()
        {

            base.OnCreate();
            inputList = new ReorderableList(serializedObjectTree.FindProperty("variables"));
            inputList.onAddCallback += AddNode;
            inputList.drawElementCallback += DrawNode;
            inputList.getElementHeightCallback += CalculateVariableHeight;
            inputList.onRemoveCallback += OnRemoveNode;
        }

        private void OnRemoveNode(ReorderableList list)
        {
            base.OnBodyGUI();
            serializedObjectTree.ApplyModifiedProperties();
            serializedObjectTree.UpdateIfRequiredOrScript();
            foreach (int index in list.Selected)
            {
                SerializedProperty nodeProp = list.List.GetArrayElementAtIndex(index);
                target.RemoveDynamicPort(nodeProp.FindPropertyRelative("variableID").stringValue);
            }
            serializedObjectTree.ApplyModifiedProperties();
            serializedObjectTree.UpdateIfRequiredOrScript();
            list.Remove(list.Selected);
        }

        private void DrawNode(Rect position, SerializedPropertyTree property, GUIContent label, bool selected, bool focused)
        {

            SerializedProperty variableID = property.FindPropertyRelative("variableID");
            NodePort portIn = target.GetInputPort(variableID.stringValue);

            if (!portIn.IsConnected)
            {
                GraphVariableEditor editor = LoadEditor(property);

                if (editor == null)
                    return;

                //float editorHeight = VariableInspectorDrawFunctions.InputNodeFNs.GetActualValueHeight(target as FlowNode, property, editor);

                //checking for null Values

                VariableInspectorDrawFunctions.InputNodeFNs.DrawValue(position, target as FlowNode, "", property, editor, GraphVariable.RetrievalTypes.ActualValue, targetIsRuntimeGraph);
            }


            //EditorGUI.PropertyField(position, property);
        
            NodeEditorGUILayout.PortField(new Vector2(position.x - 47, position.y + EditorGUIUtility.standardVerticalSpacing), portIn) ;
        }

        private void DrawNodeSecondaryMode(Rect position, SerializedPropertyTree property)
        {

            SerializedProperty variableID = property.FindPropertyRelative("variableID");
            NodePort portIn = target.GetInputPort(variableID.stringValue);

            if (!portIn.IsConnected)
            {
                GraphVariableEditor editor = LoadEditor(property);

                if (editor == null)
                    return;

                //checking for null Values

                VariableInspectorDrawFunctions.InputNodeFNs.DrawValue(position, target as FlowNode, "", property, editor, GraphVariable.RetrievalTypes.ActualValue, targetIsRuntimeGraph);
            }


            //EditorGUI.PropertyField(position, property);

            NodeEditorGUILayout.PortField(new Vector2(position.x - 17, position.y + EditorGUIUtility.standardVerticalSpacing), portIn);
        }

        private void AddNode(ReorderableList list)
        {
            SerializedPropertyTree primaryVariableProp = serializedObjectTree.FindProperty("primaryVariable");
            SerializedPropertyTree typeNameProp = primaryVariableProp.FindPropertyRelative("typeName");

            SerializedPropertyTree prop = list.AddItem();
            SerializedPropertyTree eventName = prop.FindPropertyRelative("name");
            eventName.stringValue = "Variable-" + Random.Range(1, 1000);

            SerializedPropertyTree variableID = prop.FindPropertyRelative("variableID");
            variableID.stringValue = System.Guid.NewGuid().ToString();

            prop.FindPropertyRelative("typeName").stringValue = typeNameProp.stringValue;
            serializedObjectTree.ApplyModifiedProperties();
            serializedObjectTree.UpdateIfRequiredOrScript();
            target.AddDynamicInput(ReflectionUtils.FindType(typeNameProp.stringValue), Node.ConnectionType.Override, Node.TypeConstraint.Inherited, variableID.stringValue);
        }

    

        private float CalculateVariableHeight(SerializedPropertyTree property)
        {


            GraphVariableEditor editor = LoadEditor(property);

            if (editor == null)
                return EditorGUIUtility.singleLineHeight;

            return VariableInspectorDrawFunctions.InputNodeFNs.GetValueHeight(target as FlowNode, property,"",editor, GraphVariable.RetrievalTypes.ActualValue, targetIsRuntimeGraph);
        }

        public override void OnBodyGUI()
        {
            serializedObjectTree.UpdateIfRequiredOrScript();

            SerializedPropertyTree primaryVariableProp = serializedObjectTree.FindProperty("primaryVariable");
            SerializedPropertyTree typeNameProp = primaryVariableProp.FindPropertyRelative("typeName"); 
            SerializedPropertyTree secondaryVariableProp = serializedObjectTree.FindProperty("secondVariable");

            SetupSecondaryModePorts();

            GraphVariableValue value = ValueUtility.GetVariableValue(typeNameProp.stringValue, GetFilter());

            bool valueHasSecondaryTypes = value is SecondaryMultipliableValue || value is SecondaryDividableValue;

            if (value != null && AllowSecondaryTypes() && valueHasSecondaryTypes)
            {

                List<string> primaryTypeNames = ValueUtility.GetManagedTypes(GetFilter());
                List<string> prettyPrimaryTypeNames = primaryTypeNames.Select(x => VariableInspectorUtility.GetPrettyName(x)).ToList();

                string currentPrimaryTypeName = primaryVariableProp.FindPropertyRelative("typeName").stringValue;
                int currentPrimarySelectionIndex = primaryTypeNames.IndexOf(currentPrimaryTypeName);

                if (currentPrimarySelectionIndex < 0)
                {
                    primaryVariableProp.FindPropertyRelative("typeName").stringValue = primaryTypeNames[0];
                    primaryVariableProp.serializedObject.ApplyModifiedProperties();
                    currentPrimarySelectionIndex = 0;
                    UpdateTypes();
                }

                LayersGUIUtilities.DrawDropdown(layout.DrawLine(), currentPrimarySelectionIndex, prettyPrimaryTypeNames.ToArray(), false, (selection) => {

                    primaryVariableProp.FindPropertyRelative("typeName").stringValue = primaryTypeNames[selection];
                    primaryVariableProp.serializedObject.ApplyModifiedProperties();
                    UpdateTypes();
                });


                //currentPrimarySelectionIndex = EditorGUILayout.Popup(currentPrimarySelectionIndex, primaryTypeNames.ToArray());
                //primaryVariableProp.FindPropertyRelative("typeName").stringValue = primaryTypeNames[currentPrimarySelectionIndex];


                //drawing primary Input
                float primaryVariableHeight = CalculateVariableHeight(primaryVariableProp);
                //Rect primaryVarRect = EditorGUILayout.GetControlRect(false, primaryVariableHeight);
                DrawNodeSecondaryMode(layout.Draw(primaryVariableHeight), primaryVariableProp);

                List<string> secondaryTypeNames = new List<string>( GetSecondaryTypes(value));
                List<string> prettySecondaryTypeNames = secondaryTypeNames.Select(x => VariableInspectorUtility.GetPrettyName(x)).ToList();


                string currentSecondaryTypeName = secondaryVariableProp.FindPropertyRelative("typeName").stringValue;
                int currentSecondarySelectionIndex = secondaryTypeNames.IndexOf(currentSecondaryTypeName);

                if (currentSecondarySelectionIndex < 0)
                {
                    secondaryVariableProp.FindPropertyRelative("typeName").stringValue = secondaryTypeNames[0];
                    secondaryVariableProp.serializedObject.ApplyModifiedProperties();
                    currentSecondarySelectionIndex = 0;
                }

                LayersGUIUtilities.DrawDropdown(layout.DrawLine(), currentSecondarySelectionIndex, prettySecondaryTypeNames.ToArray(), false, (selection) => {

                    secondaryVariableProp.FindPropertyRelative("typeName").stringValue = secondaryTypeNames[selection];
                    secondaryVariableProp.serializedObject.ApplyModifiedProperties();
                    UpdateTypes();
                });


                //currentSecondarySelectionIndex = EditorGUILayout.Popup(currentSecondarySelectionIndex, secondaryTypeNames.ToArray());
                //secondaryVariableProp.FindPropertyRelative("typeName").stringValue = secondaryTypeNames[currentSecondarySelectionIndex];


                float secondaryVariableHeight = CalculateVariableHeight(secondaryVariableProp);
                DrawNodeSecondaryMode(layout.Draw(secondaryVariableHeight), secondaryVariableProp);
                ClearListNodeports();
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                List<string> typeNames = ValueUtility.GetManagedTypes(GetFilter());
                List<string> prettyTypeNames = typeNames.Select(x => VariableInspectorUtility.GetPrettyName(x)).ToList();


                string currentTypeName = typeNameProp.stringValue;
                int currentSelectionIndex = typeNames.IndexOf(currentTypeName);


                if (currentSelectionIndex < 0)
                {
                    typeNameProp.stringValue = typeNames[0];
                    currentSelectionIndex = 0;
                    typeNameProp.serializedObject.ApplyModifiedProperties();
                    UpdateTypes();
                }

                LayersGUIUtilities.DrawDropdown(layout.DrawLine(), currentSelectionIndex, prettyTypeNames.ToArray(), false, (selection) => {

                    typeNameProp.stringValue = typeNames[selection];
                    typeNameProp.serializedObject.ApplyModifiedProperties();
                    UpdateTypes();
                });



                //currentSelectionIndex = EditorGUILayout.Popup(currentSelectionIndex, typeNames.ToArray());
                //typeNameProp.stringValue = typeNames[currentSelectionIndex];
                if (EditorGUI.EndChangeCheck())
                    UpdateTypes();

                inputList.DoList(layout.Draw(inputList.GetHeight()), new GUIContent(""));

                serializedObjectTree.ApplyModifiedProperties();
                ClearSecondaryModePorts();
            }

            DoOutput();
        }

        private void SetupSecondaryModePorts()
        {
            SerializedPropertyTree primaryVariableProp = serializedObjectTree.FindProperty("primaryVariable");
            SerializedPropertyTree typeNameProp = primaryVariableProp.FindPropertyRelative("typeName");
            SerializedPropertyTree secondaryVariableProp = serializedObjectTree.FindProperty("secondVariable");

            SerializedProperty primaryVarId = primaryVariableProp.FindPropertyRelative("variableID");
            if (primaryVarId.stringValue == System.Guid.Empty.ToString())
            {
                primaryVarId.stringValue = System.Guid.NewGuid().ToString();
                serializedObjectTree.ApplyModifiedProperties();
            }

            System.Type expectedType = ReflectionUtils.FindType(primaryVariableProp.FindPropertyRelative("typeName").stringValue);
            NodePort primaryNode = target.GetInputPort(primaryVarId.stringValue);
            if (primaryNode == null)
                primaryNode = target.AddDynamicInput(expectedType, Node.ConnectionType.Override, Node.TypeConstraint.Inherited, primaryVarId.stringValue);
            if (primaryNode.ValueType != expectedType)
            {
                target.RemoveDynamicPort(primaryNode);
                primaryNode = target.AddDynamicInput(expectedType, Node.ConnectionType.Override, Node.TypeConstraint.Inherited, primaryVarId.stringValue);
            }

            SerializedProperty secondaryVarId = secondaryVariableProp.FindPropertyRelative("variableID");
            if (secondaryVarId.stringValue == System.Guid.Empty.ToString())
            {
                secondaryVarId.stringValue = System.Guid.NewGuid().ToString();
                serializedObjectTree.ApplyModifiedProperties();
            }

            System.Type secondaryExpectedType = ReflectionUtils.FindType(secondaryVariableProp.FindPropertyRelative("typeName").stringValue);
            NodePort secondaryNode = target.GetInputPort(secondaryVarId.stringValue);
            if (secondaryNode == null)
                secondaryNode = target.AddDynamicInput(secondaryExpectedType, Node.ConnectionType.Override, Node.TypeConstraint.Inherited, secondaryVarId.stringValue);
            if (secondaryNode.ValueType != secondaryExpectedType)
            {
                target.RemoveDynamicPort(secondaryNode);
                secondaryNode = target.AddDynamicInput(secondaryExpectedType, Node.ConnectionType.Override, Node.TypeConstraint.Inherited, secondaryVarId.stringValue);
            }
        }

        public override int GetWidth()
        {
            SerializedPropertyTree primaryVariableProp = serializedObjectTree.FindProperty("primaryVariable");
            SerializedPropertyTree typeNameProp = primaryVariableProp.FindPropertyRelative("typeName");
            GraphVariableValue value = ValueUtility.GetVariableValue(typeNameProp.stringValue, GetFilter());

            bool valueHasSecondaryTypes = value is SecondaryMultipliableValue || value is SecondaryDividableValue;


            if (typeNameProp.stringValue == typeof(Vector3).FullName)
                return 160;

            return 140;
        }


        private void UpdateTypes()
        {
            SerializedPropertyTree primaryVariableProp = serializedObjectTree.FindProperty("primaryVariable");
            SerializedPropertyTree typeNameProp = primaryVariableProp.FindPropertyRelative("typeName");

            serializedObjectTree.ApplyModifiedProperties();
            for (int index = 0; index < inputList.List.arraySize; index++)
            {
                SerializedProperty input = inputList.List.GetArrayElementAtIndex(index);
                input.FindPropertyRelative("typeName").stringValue = typeNameProp.stringValue;
            }
            serializedObjectTree.ApplyModifiedProperties();

            for (int index = 0; index < inputList.List.arraySize; index++)
            {
                SerializedProperty input = inputList.List.GetArrayElementAtIndex(index);
                target.RemoveDynamicPort(input.FindPropertyRelative("variableID").stringValue);
                target.AddDynamicInput(ReflectionUtils.FindType(typeNameProp.stringValue), Node.ConnectionType.Override, Node.TypeConstraint.Inherited, input.FindPropertyRelative("variableID").stringValue);
            }
            serializedObjectTree.UpdateIfRequiredOrScript();
        }

        private void DoOutput()
        {
            SerializedPropertyTree primaryVariableProp = serializedObjectTree.FindProperty("primaryVariable");
            SerializedPropertyTree typeNameProp = primaryVariableProp.FindPropertyRelative("typeName");

            System.Type targetType = ReflectionUtils.FindType(typeNameProp.stringValue);
            NodePort outputPort = target.GetOutputPort("Result");
            if (outputPort == null)
                outputPort = target.AddDynamicOutput(targetType, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited, "Result");
            if (outputPort.ValueType != targetType)
            {
                target.RemoveDynamicPort(outputPort);

                outputPort = target.AddDynamicOutput(targetType, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited, "Result");
            }

            NodeEditorGUIDraw.PortField(layout.DrawLine(), outputPort);
        }

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

        protected virtual string[] GetSecondaryTypes(GraphVariableValue value)
        {
            return new string[0];
        }

        protected virtual bool AllowSecondaryTypes()
        {
            return false;
        }

        private void ClearListNodeports()
        {
            for (int index = 0; index < inputList.List.arraySize; index++)
                target.GetInputPort(inputList.List.GetArrayElementAtIndex(index).FindPropertyRelative("variableID").stringValue).ClearConnections();
        }

        private void ClearSecondaryModePorts()
        {
            SerializedPropertyTree primaryVariableProp = serializedObjectTree.FindProperty("primaryVariable");
            SerializedPropertyTree secondaryVariableProp = serializedObjectTree.FindProperty("secondVariable");

            target.GetInputPort(primaryVariableProp.FindPropertyRelative("variableID").stringValue)?.ClearConnections();
            target.GetInputPort(secondaryVariableProp.FindPropertyRelative("variableID").stringValue)?.ClearConnections();
        }

        protected abstract ValueUtility.ValueFilter GetFilter();
    }
}
