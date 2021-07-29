using ABXY.Layers.Editor;
using ABXY.Layers.Editor.Graph_Variable_Editors;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TypedPortGUI
{
    
    public string expectedType
    {
        get { return property.FindPropertyRelative("typeName").stringValue; }
        set
        {
            if (property.FindPropertyRelative("typeName").stringValue != value)
            {
                property.FindPropertyRelative("typeName").stringValue = value;
                SetupPort();
            }
        }
    }

    public string arrayType
    {
        get { return property.FindPropertyRelative("arrayType").stringValue; }
        set
        {
            if (property.FindPropertyRelative("arrayType").stringValue != value)
            {
                property.FindPropertyRelative("arrayType").stringValue = value;
                SetupPort();
            }
        }
    }

    private NodePort.IO _direction;
    public NodePort.IO direction
    {
        get { return _direction; }
        set
        {
            if (_direction != value)
            {
                _direction = value;
                SetupPort();
            }
        }
    }

    public bool hideArrayTypeSelector = false;

    public Node.ShowBackingValue showBackingValue = Node.ShowBackingValue.Always;



    private SerializedPropertyTree property;

    private Node flownode;

    public TypedPortGUI(SerializedPropertyTree graphVariableProperty, NodePort.IO direction = NodePort.IO.Input, string expectedType = "", string arrayType = "", Node.ShowBackingValue showBackingValue = Node.ShowBackingValue.Always)
    {
        this.property = graphVariableProperty;
        this._direction = direction;
        this.expectedType = expectedType;
        this.showBackingValue = showBackingValue;
        this.arrayType = arrayType;

        if (property.serializedObject.targetObject is Node)
            flownode = (Node)property.serializedObject.targetObject;


        SetupPort();
    }

    public void Draw(Rect position, string label, bool canAccessSceneObjects)
    {
        NodePort port = flownode != null ? flownode.GetPort(property.propertyPath) : null;
        bool show = showBackingValue == Node.ShowBackingValue.Always ||
            (showBackingValue == Node.ShowBackingValue.Unconnected && port != null && !port.IsConnected);


        if (show)
        {
            GraphVariableEditor editor = LoadEditor(property);
            if (editor != null)
            {
                VariableInspectorDrawFunctions.InputNodeFNs.DrawValue(position, property.serializedObject.targetObject, label, property, editor, GraphVariable.RetrievalTypes.ActualValue, canAccessSceneObjects);
                float heightInEditor = VariableInspectorDrawFunctions.InputNodeFNs.GetValueHeight(property.serializedObject.targetObject, property,label, editor, GraphVariable.RetrievalTypes.ActualValue, canAccessSceneObjects);


                //TODO: Get rid of previous line, it's inefficient
                // hacky way for displaying the element number for elements with no gui
                if (heightInEditor == 0)
                {
                    position.height = EditorGUIUtility.singleLineHeight;
                    if (direction == NodePort.IO.Input)
                        EditorGUI.LabelField(position, label);
                    else
                        EditorGUI.LabelField(position, label, LayersGUIUtilities.rightAlignDropDownStyle);
                }
            }
        }
        else
        {
            if (direction == NodePort.IO.Input)
                EditorGUI.LabelField(position, label);
            else
                EditorGUI.LabelField(position,label, LayersGUIUtilities.rightAlignDropDownStyle);
        }


        if (flownode != null)
        {

            if (port != null)
                NodeEditorGUIDraw.AddPortToRect(position, port);

        }
    }

    public float CalculateHeight(string label)
    {
        GraphVariableEditor editor = LoadEditor(property);
        float heightInEditor = VariableInspectorDrawFunctions.InputNodeFNs.GetValueHeight(property.serializedObject.targetObject, property,label, editor, GraphVariable.RetrievalTypes.ActualValue, true);
        return Mathf.Max(EditorGUIUtility.singleLineHeight, heightInEditor);
    }

    public void DoLayout(string label, bool canAccessSceneObjects)
    {
        float height = CalculateHeight(label);
        Rect controlRect = EditorGUILayout.GetControlRect(false, height);
        Draw(controlRect, label,canAccessSceneObjects);
        /*NodePort port = flownode!= null? flownode.GetPort(property.propertyPath) : null;
        bool show = showBackingValue == Node.ShowBackingValue.Always ||
            (showBackingValue == Node.ShowBackingValue.Unconnected && port != null && !port.IsConnected);


        if (show)
        {
            GraphVariableEditor editor = LoadEditor(property);
            if (editor != null)
            {
                float heightInEditor = VariableInspectorDrawFunctions.InputNodeFNs.GetActualValueHeight(property.serializedObject.targetObject, property, editor);
                Rect editorRect = EditorGUILayout.GetControlRect(false, Mathf.Max(EditorGUIUtility.singleLineHeight, heightInEditor));
                VariableInspectorDrawFunctions.InputNodeFNs.DrawActualValue(editorRect, label, property.serializedObject.targetObject, property, editor);

                // hacky way for displaying the element number for elements with no gui
                if (heightInEditor == 0)
                {
                    editorRect.height = EditorGUIUtility.singleLineHeight;
                    if (direction == NodePort.IO.Input)
                        EditorGUI.LabelField(editorRect, label);
                    else
                        EditorGUI.LabelField(editorRect,label, LayersGUIUtilities.rightAlignDropDownStyle);
                }
            }
        }
        else
        {
            if (direction == NodePort.IO.Input)
                EditorGUILayout.LabelField(label);
            else
                EditorGUILayout.LabelField(label, LayersGUIUtilities.rightAlignDropDownStyle);
        }


        if (flownode != null)
        {

            if (port != null)
                NodeEditorGUILayout.AddPortField(port);

        }*/
    }

    private void SetupPort()
    {
        property.serializedObject.ApplyModifiedProperties();

        if (flownode == null)
            return;

        

        //deleting port if direction is wrong
        NodePort wrongDirectionPort = direction == NodePort.IO.Output ? flownode.GetInputPort(property.propertyPath) : flownode.GetOutputPort(property.propertyPath);
        if (wrongDirectionPort != null)
            flownode.RemoveDynamicPort(wrongDirectionPort);

        NodePort port = direction == NodePort.IO.Input ? flownode.GetInputPort(property.propertyPath) : flownode.GetOutputPort(property.propertyPath);

        System.Type expectedType = ReflectionUtils.FindType(property.FindPropertyRelative("typeName").stringValue);

        if (expectedType == typeof(List<GraphVariable>))
        {
            System.Type arrayType = ReflectionUtils.FindType(property.FindPropertyRelative("arrayType").stringValue);
            if (arrayType != null)
                expectedType = arrayType.MakeArrayType();
        }


        if (port != null && port.ValueType != expectedType)
        {
            flownode.RemoveDynamicPort(port);
            property.serializedObject.UpdateIfRequiredOrScript();
            port = null;
        }

        if (port == null && expectedType != null)
        {
            if (direction == NodePort.IO.Input)
            {
                flownode.AddDynamicInput(expectedType, Node.ConnectionType.Override, Node.TypeConstraint.Inherited, property.propertyPath);
            }
            else
            {
                flownode.AddDynamicOutput(expectedType, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited, property.propertyPath);
            }
            property.serializedObject.UpdateIfRequiredOrScript();
        }

        
        property.serializedObject.UpdateIfRequiredOrScript();
    }


    private Dictionary<string, GraphVariableEditor> path2VariableEditor = new Dictionary<string, GraphVariableEditor>();
    private GraphVariableEditor LoadEditor(SerializedProperty property)
    {
        GraphVariableEditor editor = null;
        string targetTypeName = property.FindPropertyRelative("typeName").stringValue;
        if (!path2VariableEditor.TryGetValue(property.propertyPath, out editor) || editor.handlesType.FullName != targetTypeName)
        {
            System.Type editorType = VariableInspectorUtility.GetEditorType(targetTypeName, VariableInspectorUtility.EditorFilter.InputNode);
            if (editorType != null)
            {
                editor = (GraphVariableEditor)System.Activator.CreateInstance(editorType);


                if (path2VariableEditor.ContainsKey(property.propertyPath))
                    path2VariableEditor[property.propertyPath] = editor;
                else
                    path2VariableEditor.Add(property.propertyPath, editor);
            }
        }
        if (editor is ArrayVariableEditor)
            (editor as ArrayVariableEditor).showTypeSelector = !hideArrayTypeSelector;

        return editor;
    }
}
