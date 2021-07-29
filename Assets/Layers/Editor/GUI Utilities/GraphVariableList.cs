using ABXY.Layers.Editor.Graph_Variable_Editors;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.ThirdParty.Malee.List;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor
{
    public class GraphVariableList
    {
        private ReorderableList varList;

        private bool _allTypesShouldBeSame = false;
        public bool allTypesShouldBeSame
        {
            get { return _allTypesShouldBeSame; }
            set
            {
                if (_allTypesShouldBeSame != value)
                {
                    _allTypesShouldBeSame = value;
                    SetupPortsAndTypes();
                }
            }
        }

        public bool arrayTypesHideTypeSelector = true;

        public VariableInspectorUtility.EditorFilter typeFilter = VariableInspectorUtility.EditorFilter.All;

        public NodePort.IO direction = NodePort.IO.Input;

        public System.Func<SerializedProperty, int, string> getElementName;

        private string _typeConstraint;

        public string typeConstraint
        {
            get
            {
                return _typeConstraint;
            }
            set
            {
                if (_typeConstraint != value)
                {
                    _typeConstraint = value;
                    SetupPortsAndTypes();
                }
            }
        }

        private string _arrayTypeConstraint;

        public string arrayTypeConstraint
        {
            get
            {
                return _arrayTypeConstraint;
            }
            set
            {
                if (_arrayTypeConstraint != value)
                {
                    _arrayTypeConstraint = value;
                    SetupPortsAndTypes();
                }
            }
        }


        public Node.ShowBackingValue showBackingValue = Node.ShowBackingValue.Always;


        private Dictionary<string, GraphVariableEditor> path2VariableEditor = new Dictionary<string, GraphVariableEditor>();

        private Node flownode;

        private System.Func<Object, SerializedPropertyTree, string, GraphVariableEditor, GraphVariable.RetrievalTypes, bool, float> getHeightFunction;

        private System.Action<Rect, Object, string, SerializedPropertyTree, GraphVariableEditor, GraphVariable.RetrievalTypes, bool> drawFunction;

        public bool expanded
        {
            get { return varList.List.isExpanded; }
            set { varList.List.isExpanded = value; }
        }

        public GraphVariableList(SerializedPropertyTree property,
            System.Func<Object, SerializedPropertyTree, string, GraphVariableEditor, GraphVariable.RetrievalTypes, bool, float> getHeightFunction,
            System.Action<Rect, Object, string, SerializedPropertyTree, GraphVariableEditor, GraphVariable.RetrievalTypes, bool> drawFunction,
        NodePort.IO direction = NodePort.IO.Input,
            bool allTypesShouldBeTheSame = false,
            string typeConstraint = "",
            Node.ShowBackingValue showBackingValue = Node.ShowBackingValue.Always)
        {
            this.direction = direction;
            this._allTypesShouldBeSame = allTypesShouldBeTheSame;
            this._typeConstraint = typeConstraint;
            this._arrayTypeConstraint = arrayTypeConstraint;
            this.showBackingValue = showBackingValue;
            this.drawFunction = drawFunction;
            this.getHeightFunction = getHeightFunction;
            varList = new ReorderableList(property);
            varList.drawElementCallback += OnDrawInput;
            varList.getElementHeightCallback += CalculateInputHeight;
            varList.onAddCallback += OnAdd;
            varList.onRemoveCallback += OnRemove;
            varList.onAppendDragDropCallback += OnDragDrop;
            varList.onValidateDragAndDropCallback += ValidateDragDrop;

            if (property.serializedObject.targetObject is Node)
                flownode = (Node)property.serializedObject.targetObject;


            SetupPortsAndTypes();
        }

        private Object ValidateDragDrop(Object[] references, ReorderableList list)
        {
            foreach (Object obj in references)
            {
                if (obj != null && obj.GetType().FullName == typeConstraint)
                    return obj;
            }
            return null;
        }

        private void OnDragDrop(Object reference, ReorderableList list)
        {
            if (reference == null)
                return;

            OnAdd(list);
            SerializedPropertyTree property = list.List.GetArrayElementAtIndex(list.List.arraySize - 1);

            property.FindPropertyRelative("unityObjectValue").objectReferenceValue = reference;


        }

        private void OnRemove(ReorderableList list)
        {
            list.List.serializedObject.ApplyModifiedProperties();
            foreach (int selection in list.Selected)
            {
                SerializedProperty graphVar = list.List.GetArrayElementAtIndex(selection);
                if (flownode != null)
                {
                    NodePort port = flownode.GetPort(graphVar.FindPropertyRelative("variableID").stringValue);
                    if (port != null)
                        flownode.RemoveDynamicPort(port);
                }
            }
            list.List.serializedObject.UpdateIfRequiredOrScript();
            list.Remove(list.Selected);
        }

        private void OnAdd(ReorderableList list)
        {
            list.AddItem();
            SerializedProperty newItem = list.List.GetArrayElementAtIndex(list.List.arraySize - 1);
            newItem.FindPropertyRelative("variableID").stringValue = System.Guid.NewGuid().ToString();
            list.List.serializedObject.ApplyModifiedProperties();
            SetupPortsAndTypes();
        }


        public void DoLayoutList()
        {
            varList.DoLayoutList();
        }

        public void DoList(Rect position, GUIContent label)
        {
            varList.DoList(position, label);
        }

        public float GetHeight()
        {
            return varList.GetHeight();
        }

        private float CalculateInputHeight(SerializedPropertyTree element)
        {
            float height = allTypesShouldBeSame ? 0f : EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;


            NodePort port = flownode != null ? flownode.GetPort(element.FindPropertyRelative("variableID").stringValue) : null;
            bool show = (showBackingValue == Node.ShowBackingValue.Always ||
            (showBackingValue == Node.ShowBackingValue.Unconnected && (port == null || !port.IsConnected)));

            GraphVariableEditor editor = LoadEditor(element);
            if (editor == null || !show)
                return EditorGUIUtility.singleLineHeight + height;

            string elementLabel = GetElementName(element);


            // if flownode is null, we're probably running in the player
            bool canAccessSceneObjects = flownode == null || (flownode.graph as SoundGraph).isRunningSoundGraph;

            return Mathf.Max(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                getHeightFunction(varList.List.serializedObject.targetObject, element, elementLabel, editor, GraphVariable.RetrievalTypes.ActualValue, canAccessSceneObjects)) + height;
        }

        private void OnDrawInput(Rect rect, SerializedPropertyTree element, GUIContent label, bool selected, bool focused)
        {




            float typeSelectorHeight = allTypesShouldBeSame ? 0f : EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            if (!allTypesShouldBeSame)
            {
                Rect typeSelector = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                LayersGUIUtilities.DrawTypeSelector(typeSelector, element.FindPropertyRelative("typeName"), typeFilter, SetupPortsAndTypes);
            }


            NodePort port = flownode != null ? flownode.GetPort(element.FindPropertyRelative("variableID").stringValue) : null;
            bool show = (showBackingValue == Node.ShowBackingValue.Always ||
            (showBackingValue == Node.ShowBackingValue.Unconnected && (port == null || !port.IsConnected)));

            if (show)
            {
                GraphVariableEditor editor = LoadEditor(element);
                if (editor != null)
                {

                    string elementLabel = GetElementName(element);

                    // if flownode is null, we're probably running in the player
                    bool canAccessSceneObjects = flownode == null || (flownode.graph as SoundGraph).isRunningSoundGraph;

                    Rect editorRect = new Rect(rect.x, rect.y + typeSelectorHeight, rect.width,
                        getHeightFunction(element.serializedObject.targetObject, element, elementLabel, editor, GraphVariable.RetrievalTypes.ActualValue, canAccessSceneObjects));

                    drawFunction(editorRect, varList.List.serializedObject.targetObject, elementLabel, element, editor, GraphVariable.RetrievalTypes.ActualValue, canAccessSceneObjects);

                    // hacky way for displaying the element number for elements with no gui
                    if (editorRect.height == 0)
                    {
                        editorRect.height = EditorGUIUtility.singleLineHeight;
                        EditorGUI.LabelField(editorRect, GetElementName(element));
                    }
                }
                else
                {
                    Rect labelRect = new Rect(rect.x, rect.y + typeSelectorHeight, rect.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(labelRect, GetElementName(element));
                }
            }
            else
            {
                Rect labelRect = new Rect(rect.x, rect.y + typeSelectorHeight, rect.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(labelRect, GetElementName(element));
            }


            if (flownode != null)
            {

                if (port != null)
                {
                    if (direction == NodePort.IO.Input)
                        NodeEditorGUILayout.PortField(new Vector2(rect.x - 47, rect.y), port);
                    else
                        NodeEditorGUILayout.PortField(new Vector2(rect.x + rect.width, rect.y), port);
                }
            }
        }

        private string GetElementName(SerializedProperty property)
        {
            int lastOpenBracket = property.propertyPath.LastIndexOf('[');
            string indexString = property.propertyPath.Substring(lastOpenBracket + 1, property.propertyPath.Length - lastOpenBracket - 2);
            int index = 0;
            int.TryParse(indexString, out index);

            if (getElementName == null)
                return string.Format("Element {0}", index + 1);

            return getElementName.Invoke(property, index + 1);
        }

        private void SetupPortsAndTypes()
        {
            varList.List.serializedObject.ApplyModifiedProperties();


            for (int index = 0; index < varList.Length; index++)
            {
                SerializedProperty property = varList.List.GetArrayElementAtIndex(index);

                if (flownode != null)
                {
                    //deleting port if direction is wrong
                    NodePort wrongDirectionPort = direction == NodePort.IO.Output ? flownode.GetInputPort(property.FindPropertyRelative("variableID").stringValue) : flownode.GetOutputPort(property.FindPropertyRelative("variableID").stringValue);
                    if (wrongDirectionPort != null)
                        flownode.RemoveDynamicPort(wrongDirectionPort);

                }

                System.Type expectedType = ReflectionUtils.FindType(allTypesShouldBeSame ? typeConstraint : property.FindPropertyRelative("typeName").stringValue);
                string arrayType = allTypesShouldBeSame ? arrayTypeConstraint : "";


                //setting type
                if (allTypesShouldBeSame)
                {
                    property.FindPropertyRelative("typeName").stringValue = expectedType != null ? expectedType.FullName : "";
                    SerializedProperty arrayTypeProp = property.FindPropertyRelative("arrayType");
                    if (arrayTypeProp != null)
                        arrayTypeProp.stringValue = arrayType;
                    property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                }

                if (expectedType == typeof(List<GraphVariable>))
                {
                    System.Type arrayElementType = ReflectionUtils.FindType(arrayType);
                    if (arrayElementType != null)
                        expectedType = arrayElementType.MakeArrayType();
                }

                if (flownode != null)
                {
                    NodePort port = direction == NodePort.IO.Input ? flownode.GetInputPort(property.FindPropertyRelative("variableID").stringValue) : flownode.GetOutputPort(property.FindPropertyRelative("variableID").stringValue);
                    if (port != null && port.ValueType != expectedType)
                    {
                        flownode.RemoveDynamicPort(port);
                        varList.List.serializedObject.UpdateIfRequiredOrScript();
                        port = null;
                    }

                    if (port == null && expectedType != null)
                    {
                        if (direction == NodePort.IO.Input)
                        {
                            flownode.AddDynamicInput(expectedType, Node.ConnectionType.Override, Node.TypeConstraint.Inherited, property.FindPropertyRelative("variableID").stringValue);
                        }
                        else
                        {
                            flownode.AddDynamicOutput(expectedType, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited, property.FindPropertyRelative("variableID").stringValue);
                        }
                        varList.List.serializedObject.UpdateIfRequiredOrScript();
                    }
                }

            }
            varList.List.serializedObject.UpdateIfRequiredOrScript();
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

            if (editor is ArrayVariableEditor)
                ((ArrayVariableEditor)editor).showTypeSelector = !arrayTypesHideTypeSelector;

            return editor;
        }
    }
}