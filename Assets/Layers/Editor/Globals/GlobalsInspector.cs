﻿using ABXY.Layers.Editor;
using ABXY.Layers.Editor.Code_generation;
using ABXY.Layers.Editor.Graph_Variable_Editors;
using ABXY.Layers.Editor.Node_Editor_Window;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.FlowTypes;
using ABXY.Layers.Runtime.Graph_Variable_Values;
using ABXY.Layers.Runtime.Settings;
using ABXY.Layers.ThirdParty.Malee.List;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GlobalsAsset))]
public class GlobalsInspector : Editor
{
    ReorderableList variablesList;
    ReorderableList eventsList;

    private Dictionary<string, GraphVariableEditor> path2VariableEditor = new Dictionary<string, GraphVariableEditor>();

    SerializedObjectTree serializedObjectTree;


    public static SoundgraphCombinedStyle style = new SoundgraphCombinedStyle();

    public NodeLayoutUtility layout = new NodeLayoutUtility();
    private void OnEnable()
    {
        LayersSettings.GetOrCreateSettings().RegisterGlobalsAsset(target as GlobalsAsset);

        serializedObjectTree = new SerializedObjectTree(serializedObject);
        variablesList = new ReorderableList(serializedObjectTree.FindProperty("variables"));
        variablesList.onAddCallback += OnAddVariableElement;
        variablesList.onRemoveCallback += OnRemoveVariableElement;
        variablesList.drawElementCallback += DrawVariableElement;
        variablesList.getElementHeightCallback += CalculateVariableHeight;
        variablesList.expandable = false;


        eventsList = new ReorderableList(serializedObjectTree.FindProperty("events"));
        eventsList.drawElementCallback += DrawEventElement;
        eventsList.getElementHeightCallback += GetEventElementHeightDelegate;
        eventsList.onAddCallback += OnAddEventElement;
        eventsList.onRemoveCallback += OnRemoveEventElement;
        eventsList.expandable = false;
    }

    public override void OnInspectorGUI()
    {

        layout.StartDraw();

        if (!CheckAssetPath())
        {
            EditorGUI.HelpBox(layout.DrawLines(2), "This asset needs to be located inside a Resources folder to work correctly", MessageType.Error);
        }

        serializedObject.UpdateIfRequiredOrScript();

        SynchronizeDefaultValues();

        //EditorGUI.BeginChangeCheck();

        variablesList.DoList(layout.Draw(variablesList.GetHeight()), new GUIContent("Variables"));

        //if (EditorGUI.EndChangeCheck())
            //LayersSettings.GetOrCreateSettings().MarkGlobalsAssetAsChanged((target as GlobalsAsset));


        // Checking variable errors
        List<string> variableNames = new List<string>();
        for (int index = 0; index < variablesList.List.arraySize; index++)
        {
            string varName = variablesList.List.GetArrayElementAtIndex(index).FindPropertyRelative("name").stringValue;
            if (variableNames.Contains(varName))
                EditorGUI.HelpBox(layout.DrawLine(), string.Format("Variable name {0} must not be duplicated", varName), MessageType.Error);
            else if (GraphVariableValue.bannedNames.Contains(varName))
                EditorGUI.HelpBox(layout.DrawLine(), string.Format("Variable name {0} is not allowed", varName), MessageType.Error);
            else
                variableNames.Add(varName);
        }


        eventsList.DoList(layout.Draw(eventsList.GetHeight()), new GUIContent("Event"));
        //NodeEditorGUILayout.PortField(target.GetOutputPort("start"));


        // Checking Event errors
        List<string> eventNames = new List<string>();
        for (int index = 0; index < eventsList.List.arraySize; index++)
        {
            string eventName = eventsList.List.GetArrayElementAtIndex(index).FindPropertyRelative("eventName").stringValue;
            if (eventNames.Contains(eventName))
                EditorGUI.HelpBox(layout.DrawLine(), string.Format("Event name {0} must not be duplicated", eventName), MessageType.Error);
            else if (GraphVariableValue.bannedNames.Contains(eventName))
                EditorGUI.HelpBox(layout.DrawLine(), string.Format("Event name {0} is not allowed", eventName), MessageType.Error);
            else
                eventNames.Add(eventName);

            if (variableNames.Contains(eventName))
                EditorGUI.HelpBox(layout.DrawLine(), string.Format("Event name {0} is named the same as a variable. This is not allowed", eventName),
                    MessageType.Error);
        }


        serializedObject.ApplyModifiedProperties();

        if (LayersSettings.GetOrCreateSettings().CheckIfGlobalsAssetNeedsRegen((target as GlobalsAsset)))
        {
            EditorGUI.HelpBox(layout.DrawLines(2), "This Globals Asset needs to regenerate code", MessageType.Warning);
            if (GUILayout.Button("Regenerate code"))
                RegenList.Generate();
        }

        layout.EndDraw();
    }

    private Dictionary<string, EventPropertyGroup> eventPropertyGroups = new Dictionary<string, EventPropertyGroup>();
    private EventPropertyGroup GetEventPropertyGroup(SerializedPropertyTree property)
    {
        if (!eventPropertyGroups.ContainsKey(property.propertyPath))
            eventPropertyGroups.Add(property.propertyPath, new EventPropertyGroup(property, target as GlobalsAsset));
        return eventPropertyGroups[property.propertyPath];
    }

    private void DrawEventElement(Rect position, SerializedPropertyTree property, GUIContent label, bool selected, bool focused)
    {

        EventPropertyGroup eventProperty = GetEventPropertyGroup(property);

        Color headerColor = 0.8f * (Color)(selected ? new Color32(89, 137, 207, 255) : style.nodeBackgroundColor);
        // header
        Rect headerBGRect = new Rect(position.x, position.y, position.width + 2f * EditorGUIUtility.standardVerticalSpacing, EditorGUIUtility.singleLineHeight + 2f * EditorGUIUtility.standardVerticalSpacing);
        EditorGUI.DrawRect(headerBGRect, headerColor);
        Rect headerRect = new Rect(headerBGRect.x, headerBGRect.y, headerBGRect.width - 70f, headerBGRect.height);
        eventProperty.expanded.boolValue = LayersGUIUtilities.ExpandHeader(headerRect, eventProperty.eventName.stringValue, eventProperty.expanded.boolValue, headerColor);


        SerializedPropertyTree nameProperty = eventProperty.eventName;

        Rect buttonRect = new Rect(position.x + position.width - 70f, position.y + 1f * EditorGUIUtility.standardVerticalSpacing, 70f, EditorGUIUtility.singleLineHeight);
        if (GUI.Button(buttonRect, "Trigger"))
        {
            /*GraphEvent gevent = (target as GraphInputs).soundGraph.GetEvent(nameProperty.stringValue);

            NodeEditorWindow.current.onLateGUI += () =>
            {

                Rect screenRectPosition = new Rect(GUIUtility.GUIToScreenPoint(Event.current.mousePosition), new Vector2(0, 0));
                CallEventWithParametersWindow.Display(screenRectPosition, (target as FlowNode).soundGraph, gevent);
            };*/

        }


        float errorListPosition = headerRect.y + headerRect.height;

        if (eventProperty.expanded.boolValue)
        {

            EditorGUI.BeginChangeCheck();

            EditorGUIUtility.labelWidth = 0f;
            Rect nameRect = new Rect(position.x, position.y + 2f * EditorGUIUtility.standardVerticalSpacing + headerRect.height, position.width , EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(nameRect, nameProperty, new GUIContent(""));


            //EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            if (EditorGUI.EndChangeCheck())
                LayersSettings.GetOrCreateSettings().MarkGlobalsAssetAsChanged((target as GlobalsAsset));



            //EditorGUI.EndDisabledGroup();

            // EditorGUI.BeginChangeCheck();

            LayersGUIUtilities.BeginNewLabelWidth(78);
            //Rect exposeRect = new Rect(position.x + (position.width * 2f / 3f) + EditorGUIUtility.standardVerticalSpacing
                //, position.y + headerRect.height + 2f * EditorGUIUtility.standardVerticalSpacing, position.width * 2f / 3f, EditorGUIUtility.singleLineHeight);
            //EditorGUI.PropertyField(exposeRect, eventProperty.expose);
            LayersGUIUtilities.EndNewLabelWidth();

            //if (EditorGUI.EndChangeCheck())
                //LayersSettings.GetOrCreateSettings().MarkSoundGraphAsChanged((target).soundGraph);

            float parametersListHeight = eventProperty.parametersReorderableList.GetHeight();
            Rect parameterListRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight * 1f) + 3f * EditorGUIUtility.standardVerticalSpacing + headerRect.height, position.width, parametersListHeight);
            eventProperty.parametersReorderableList.DoList(parameterListRect, new GUIContent("Parameters"));

            errorListPosition = parameterListRect.y + parameterListRect.height;
        }


        SerializedPropertyTree eventID = eventProperty.eventID;
        





        //checking for errors
        float indexPosition = errorListPosition + EditorGUIUtility.standardVerticalSpacing;
        List<string> eventNames = new List<string>();
        for (int index = 0; index < eventProperty.parametersReorderableList.List.arraySize; index++)
        {
            string varName = eventProperty.parametersReorderableList.List.GetArrayElementAtIndex(index).FindPropertyRelative("parameterName").stringValue;
            if (eventNames.Contains(varName))
            {
                Rect helpBoxPosition = new Rect(position.x, indexPosition, position.width, 2f * EditorGUIUtility.singleLineHeight);
                EditorGUI.HelpBox(helpBoxPosition, string.Format("Parameter name {0} must not be duplicated", varName), MessageType.Error);
                indexPosition += helpBoxPosition.height;
            }
            else
                eventNames.Add(varName);
        }


        Rect highlightRect = new Rect(position.x - 10, position.y + 2f, 2f, position.height - 4f);
        EditorGUI.DrawRect(highlightRect, LayersSettings.GetOrCreateSettings().GetColor(typeof(LayersEvent).FullName, EditorGUIUtility.isProSkin));

    }



    private void OnAddEventElement(ReorderableList list)
    {

        SerializedPropertyTree prop = list.AddItem();
        SerializedPropertyTree eventName = prop.FindPropertyRelative("eventName");
        eventName.stringValue = "Event-" + Random.Range(1, 1000);

        SerializedPropertyTree eventID = prop.FindPropertyRelative("eventID");
        eventID.stringValue = System.Guid.NewGuid().ToString();

        //LayersSettings.GetOrCreateSettings().MarkSoundGraphAsChanged((target).soundGraph);
    }

    private void OnRemoveEventElement(ReorderableList list)
    {
        serializedObject.UpdateIfRequiredOrScript();
        int[] selections = list.Selected;
        foreach (int selection in selections)
        {
            string eventID = list.List.GetArrayElementAtIndex(selection).FindPropertyRelative("eventID").stringValue;
        }
        list.Remove(selections);
        serializedObject.ApplyModifiedProperties();


        //LayersSettings.GetOrCreateSettings().MarkSoundGraphAsChanged((target).soundGraph);
    }

    private float GetEventElementHeightDelegate(SerializedPropertyTree property)
    {
        EventPropertyGroup eventProperty = GetEventPropertyGroup(property);

        SerializedPropertyTree eventName = eventProperty.eventName;
        SerializedPropertyTree expose = eventProperty.expose;

        float titleHeight = EditorGUIUtility.singleLineHeight + 2f * EditorGUIUtility.standardVerticalSpacing;


        float parameterListHeight = 0;
        float otherProperties = 0;
        if (eventProperty.expanded.boolValue)
        {
            parameterListHeight = eventProperty.parametersReorderableList.GetHeight();
            otherProperties = EditorGUI.GetPropertyHeight(eventName) + 3f * EditorGUIUtility.standardVerticalSpacing;
        }


        float errorHeight = EditorGUIUtility.standardVerticalSpacing;
        List<string> eventNames = new List<string>();
        for (int index = 0; index < eventProperty.parametersReorderableList.List.arraySize; index++)
        {
            string varName = eventProperty.parametersReorderableList.List.GetArrayElementAtIndex(index).FindPropertyRelative("parameterName").stringValue;
            if (eventNames.Contains(varName))
            {
                errorHeight += 2f * EditorGUIUtility.singleLineHeight;
            }
            else
                eventNames.Add(varName);
        }


        return titleHeight + otherProperties + parameterListHeight + errorHeight;
    }


    private void OnAddVariableElement(ReorderableList list)
    {

        SerializedPropertyTree prop = list.AddItem();
        SerializedPropertyTree eventName = prop.FindPropertyRelative("name");
        eventName.stringValue = "Variable-" + Random.Range(1, 1000);

        SerializedPropertyTree variableID = prop.FindPropertyRelative("variableID");
        variableID.stringValue = System.Guid.NewGuid().ToString();

        prop.FindPropertyRelative("typeName").stringValue = typeof(bool).FullName;

        prop.FindPropertyRelative("defaultSerializedObjectValue").stringValue = false.ToString();

        prop.FindPropertyRelative("synchronizeWithGraphVariable").boolValue = true;
    }

    private void OnRemoveVariableElement(ReorderableList list)
    {
        serializedObject.UpdateIfRequiredOrScript();
        int[] selections = list.Selected;
        foreach (int selection in selections)
        {
            string variableID = list.List.GetArrayElementAtIndex(selection).FindPropertyRelative("variableID").stringValue;
        }
        list.Remove(selections);
        serializedObject.ApplyModifiedProperties();
    }



    private void DrawVariableElement(Rect position, SerializedPropertyTree property, GUIContent label, bool selected, bool focused)
    {
        VariablePropertyGroup variablePropertyGroup = GetVariablePropertyGroup(property);
        Color typeColor = LayersSettings.GetOrCreateSettings().GetColor(variablePropertyGroup.typeName.stringValue, EditorGUIUtility.isProSkin);

        GraphVariableEditor editor = LoadEditor(property);

        float yPosition = position.y;

        SerializedPropertyTree expanded = property.FindPropertyRelative("expanded");

        Color headerColor = 0.8f * (Color)(selected ? new Color32(89, 137, 207, 255) : style.nodeBackgroundColor);
        // header
        Rect headerRect = new Rect(position.x, yPosition, position.width + 2f * EditorGUIUtility.standardVerticalSpacing, EditorGUIUtility.singleLineHeight + 2f * EditorGUIUtility.standardVerticalSpacing);
        expanded.boolValue = LayersGUIUtilities.ExpandHeader(headerRect, variablePropertyGroup.name.stringValue, expanded.boolValue, headerColor);
        EditorGUI.LabelField(headerRect, editor != null ? editor.GetPrettyTypeName() : "", LayersGUIUtilities.rightAlignedCenteredText);

        yPosition += headerRect.height + 2f * EditorGUIUtility.standardVerticalSpacing;



        if (expanded.boolValue)
        {
           EditorGUI.BeginChangeCheck();

            Rect nameRect = new Rect(position.x, yPosition, (position.width / 2f) - EditorGUIUtility.standardVerticalSpacing, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(nameRect, variablePropertyGroup.name, new GUIContent());

            Rect dropdownRect = new Rect(position.x + (position.width / 2f), yPosition, position.width / 2f, EditorGUIUtility.singleLineHeight);

            LayersGUIUtilities.DrawTypeSelector(dropdownRect, variablePropertyGroup.typeName, VariableInspectorUtility.EditorFilter.InputNode);


            property.serializedObject.ApplyModifiedProperties();

            yPosition += EditorGUIUtility.singleLineHeight + 2f * EditorGUIUtility.standardVerticalSpacing;

            if (editor != null)
            {
                //Drawing input/output selector
                Rect exposedRect = new Rect(position.x, yPosition, position.width, EditorGUIUtility.singleLineHeight);
                if (variablePropertyGroup.expose.enumValueIndex == -1)
                    variablePropertyGroup.expose.enumValueIndex = 2;
                //LayersGUIUtilities.DrawDropdown(exposedRect, new GUIContent("Expose"), variablePropertyGroup.expose, false);
                //yPosition += EditorGUIUtility.singleLineHeight + 2f * EditorGUIUtility.standardVerticalSpacing;

                //Drawing editor
                float editorHeight = VariableInspectorDrawFunctions.InputNodeFNs.GetValueHeight(target, property, "Default Value", editor, GraphVariable.RetrievalTypes.DefaultValue, false);
                Rect propertyRect = new Rect(position.x, yPosition, position.width, editorHeight);
                VariableInspectorDrawFunctions.InputNodeFNs.DrawValue(propertyRect, target, "Default Value", property, editor, GraphVariable.RetrievalTypes.DefaultValue, false);



            }

            if (EditorGUI.EndChangeCheck())
                LayersSettings.GetOrCreateSettings().MarkGlobalsAssetAsChanged((target as GlobalsAsset));

            if (editor != null)
            {
                float editorHeight = VariableInspectorDrawFunctions.InputNodeFNs.GetValueHeight(target, property, "Default Value", editor, GraphVariable.RetrievalTypes.DefaultValue, false);
                yPosition += editorHeight + EditorGUIUtility.standardVerticalSpacing;

                float defaultValueHeight = EditorGUIUtility.singleLineHeight;
                Rect defaultValueRect = new Rect(position.x, yPosition, position.width, defaultValueHeight);
                VariableInspectorDrawFunctions.InputNodeFNs.DrawActualValueReadonly(defaultValueRect, "Current Value:", target, property, editor);
            }

            //EditorGUI.PropertyField(exposedRect, variablePropertyGroup.expose);

        }

        //EditorGUI.PropertyField(position, property);
        SerializedPropertyTree variableID = variablePropertyGroup.variableID;

        Rect highlightRect = new Rect(position.x - 10, position.y + 2f, 2f, position.height - 4f);
        EditorGUI.DrawRect(highlightRect, typeColor);

    }

    Dictionary<string, VariablePropertyGroup> variablePropertyGroups = new Dictionary<string, VariablePropertyGroup>();
    private VariablePropertyGroup GetVariablePropertyGroup(SerializedPropertyTree property)
    {
        if (!variablePropertyGroups.ContainsKey(property.propertyPath))
            variablePropertyGroups.Add(property.propertyPath, new VariablePropertyGroup(property));
        return variablePropertyGroups[property.propertyPath];
    }

    private float CalculateVariableHeight(SerializedPropertyTree property)
    {

        float emptyHeight = (EditorGUIUtility.singleLineHeight * 1) + (EditorGUIUtility.standardVerticalSpacing * 2f);

        GraphVariableEditor editor = LoadEditor(property);

        if (editor == null || !property.FindPropertyRelative("expanded").boolValue)
            return emptyHeight;

        emptyHeight += (EditorGUIUtility.singleLineHeight * 1) + (EditorGUIUtility.standardVerticalSpacing * 6f);

        emptyHeight += EditorGUIUtility.singleLineHeight;

        return VariableInspectorDrawFunctions.InputNodeFNs.GetValueHeight(target, property, "Default Value", editor, GraphVariable.RetrievalTypes.DefaultValue, false) + emptyHeight;
    }

    private GraphVariableEditor LoadEditor(SerializedPropertyTree property)
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
        return editor;
    }

    private void SynchronizeDefaultValues()
    {
        for (int index = 0; index < variablesList.List.arraySize; index++)
        {
            SerializedPropertyTree variableProperty = variablesList.List.GetArrayElementAtIndex(index);
            if (variableProperty.FindPropertyRelative("synchronizeWithGraphVariable").boolValue)
            {
                GraphVariable variable = SerializedPropertyUtils.GetPropertyObject(variableProperty) as GraphVariable;
                variable.ResetToDefaultValue();
            }
        }
        serializedObject.UpdateIfRequiredOrScript();
    }

    private bool CheckAssetPath()
    {
        string assetPath = AssetDatabase.GetAssetPath(target);
        Regex regex = new Regex("(?<=/)Resources(?=/)|^Resources(?=/)");
        return regex.IsMatch(assetPath);
    }

    public struct VariablePropertyGroup
    {
        public SerializedPropertyTree name { get { return graphVariableProp.FindPropertyRelative("name"); } }
        public SerializedPropertyTree variableID { get { return graphVariableProp.FindPropertyRelative("variableID"); } }
        public SerializedPropertyTree expose { get { return graphVariableProp.FindPropertyRelative("expose"); } }
        public SerializedPropertyTree typeName { get { return graphVariableProp.FindPropertyRelative("typeName"); } }
        public SerializedPropertyTree synchronizeWithGraphVariable { get { return graphVariableProp.FindPropertyRelative("synchronizeWithGraphVariable"); } }
        public SerializedPropertyTree unityObjectValue { get { return graphVariableProp.FindPropertyRelative("unityObjectValue"); } }

        private SerializedPropertyTree graphVariableProp;
        public VariablePropertyGroup(SerializedPropertyTree graphVariableProp)
        {
            this.graphVariableProp = graphVariableProp;
        }
    }

    public struct EventPropertyGroup
    {
        public SerializedPropertyTree eventName { get { return variablePropertyTree.FindPropertyRelative("eventName"); } }
        public SerializedPropertyTree expose { get { return variablePropertyTree.FindPropertyRelative("expose"); } }
        public SerializedPropertyTree eventID { get { return variablePropertyTree.FindPropertyRelative("eventID"); } }
        public SerializedPropertyTree parameters { get { return variablePropertyTree.FindPropertyRelative("parameters"); } }
        public SerializedPropertyTree expanded { get { return variablePropertyTree.FindPropertyRelative("expanded"); } }

        public ReorderableList parametersReorderableList;

        private Dictionary<string, ParameterPropertyGroup> parameterProperties;

        private GlobalsAsset targetAsset;
        SerializedPropertyTree variablePropertyTree;

        public EventPropertyGroup(SerializedPropertyTree variablePropertyTree, GlobalsAsset target)
        {
            this.variablePropertyTree = variablePropertyTree;

            parametersReorderableList = new ReorderableList(variablePropertyTree.FindPropertyRelative("parameters"));

            parameterProperties = new Dictionary<string, ParameterPropertyGroup>();

            targetAsset = target;
            parametersReorderableList.drawElementCallback += DrawParameter;
            parametersReorderableList.getElementHeightCallback += GetParameterHeight;
            parametersReorderableList.onAddCallback += OnAddParameter;
            parametersReorderableList.onRemoveCallback += OnRemoveParameter;
            parametersReorderableList.List.isExpanded = false;
            parametersReorderableList.List.serializedObject.ApplyModifiedProperties();
        }

        private void OnAddParameter(ReorderableList list)
        {
            SerializedPropertyTree newItem = list.AddItem();
            newItem.FindPropertyRelative("parameterName").stringValue = "Parameter-" + Random.Range(1, 1000);
            LayersSettings.GetOrCreateSettings().MarkGlobalsAssetAsChanged(targetAsset);
        }

        private void OnRemoveParameter(ReorderableList list)
        {
            list.Remove(list.Selected); 
            LayersSettings.GetOrCreateSettings().MarkGlobalsAssetAsChanged(targetAsset);
        }

        private float GetParameterHeight(SerializedPropertyTree prop)
        {
            return (EditorGUIUtility.singleLineHeight * 2f) + (EditorGUIUtility.standardVerticalSpacing * 3f);
        }

        private void DrawParameter(Rect position, SerializedPropertyTree property, GUIContent label, bool selected, bool focused)
        {
            EditorGUI.BeginChangeCheck();
            ParameterPropertyGroup parameterProperty = GetEventPropertyGroup(property);
            Rect parameterNameRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(parameterNameRect, parameterProperty.parameterName, new GUIContent("Name"));

            Rect typeDropdown = new Rect(parameterNameRect.x, parameterNameRect.y + EditorGUIUtility.standardVerticalSpacing + parameterNameRect.height, parameterNameRect.width, EditorGUIUtility.singleLineHeight);

            LayersGUIUtilities.DrawTypeSelector(typeDropdown, parameterProperty.parameterTypeName, "Parameter Type", VariableInspectorUtility.EditorFilter.All);

            if (EditorGUI.EndChangeCheck())
                LayersSettings.GetOrCreateSettings().MarkGlobalsAssetAsChanged(targetAsset);

            Rect highlightRect = new Rect(position.x - 10, position.y + 2f, 2f, position.height - 4f);
            EditorGUI.DrawRect(highlightRect, LayersSettings.GetOrCreateSettings().GetColor(parameterProperty.parameterTypeName.stringValue, EditorGUIUtility.isProSkin));
        }

        private ParameterPropertyGroup GetEventPropertyGroup(SerializedPropertyTree property)
        {
            if (!parameterProperties.ContainsKey(property.propertyPath))
                parameterProperties.Add(property.propertyPath, new ParameterPropertyGroup(property));
            return parameterProperties[property.propertyPath];
        }
    }

    private struct ParameterPropertyGroup
    {
        public SerializedPropertyTree parameterName;
        public SerializedPropertyTree parameterTypeName;

        public ParameterPropertyGroup(SerializedPropertyTree parameterProp)
        {
            parameterName = parameterProp.FindPropertyRelative("parameterName");
            parameterTypeName = parameterProp.FindPropertyRelative("parameterTypeName");
        }
    }

}
