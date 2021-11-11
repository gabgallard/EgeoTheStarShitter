using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Sound_graph_players;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor
{
    public static class LayersGUIUtilities
    {

        #region styles
        private static GUIStyle _dropDownStyle;
        public static GUIStyle dropDownStyle
        {
            get
            {
                if (_dropDownStyle == null)
                {
                    _dropDownStyle = new GUIStyle(EditorStyles.miniButton);
                    _dropDownStyle.fontSize = EditorStyles.label.fontSize - 2;
                    _dropDownStyle.alignment = TextAnchor.MiddleLeft;

                    //_dropDownStyle.normal.background = EditorStyles.miniButton.normal.background;
                    //_dropDownStyle.normal.background = EditorStyles.miniButton.focused.background;
                    _dropDownStyle.padding = new RectOffset(5, 1, 2, 1);
                    _dropDownStyle.border = EditorStyles.miniButton.border;
                }
                return _dropDownStyle;
            }
        }

        private static GUIStyle _rightAlignDropDownStyle;
        public static GUIStyle rightAlignDropDownStyle
        {
            get
            {
                if (_rightAlignDropDownStyle == null)
                {
                    _rightAlignDropDownStyle = new GUIStyle(EditorStyles.label);
                    _rightAlignDropDownStyle.alignment = TextAnchor.MiddleRight;
                    _rightAlignDropDownStyle.padding = new RectOffset(1, 15, 1, 1);
                }
                return _rightAlignDropDownStyle;
            }
        }

        private static Texture2D _dropdownArrow;
        private static Texture2D dropdownArrow
        {
            get
            {
                if (_dropdownArrow == null)
                    _dropdownArrow = Resources.Load<Texture2D>("Symphony/Dropdown arrow");
                return _dropdownArrow;
            }
        }

        private static Texture2D _threeDotMenu;
        private static Texture2D threeDotMenu
        {
            get
            {
                if (_threeDotMenu == null)
                    _threeDotMenu = Resources.Load<Texture2D>("Symphony/baseline_more_vert_white_18dp");
                return _threeDotMenu;
            }
        }

        private static GUIStyle _rightAlignedCenteredText;
        public static GUIStyle rightAlignedCenteredText
        {
            get
            {
                if (_rightAlignedCenteredText == null)
                {
                    _rightAlignedCenteredText = new GUIStyle(EditorStyles.label);
                    _rightAlignedCenteredText.padding = new RectOffset(0, 5, 0, 0);
                    _rightAlignedCenteredText.alignment = TextAnchor.MiddleRight;
                }
                return _rightAlignedCenteredText;
            }
        }
        #endregion

        #region tabs
        public static void DrawTabs(Rect tabArea, int currentSelection, params string[] labels)
        {
            for (int index = 0; index < labels.Length; index++)
            {
                float buttonWidth = tabArea.width / labels.Length;
                Rect buttonRect = new Rect(tabArea.x + (buttonWidth*index), tabArea.y, buttonWidth, tabArea.height);

            }
        }
        #endregion

        #region Draw Dropdown

        public static void DrawDropdown(GUIContent title, System.Enum selection, System.Action<System.Enum> onSelection, bool searcheable = false)
        {
            Rect controlRect = EditorGUILayout.GetControlRect(false);
            DrawDropdown(controlRect, title, selection, onSelection, searcheable);
        }

        public static void DrawDropdown( System.Enum selection, System.Action<System.Enum> onSelection, bool searcheable = false)
        {
            Rect controlRect = EditorGUILayout.GetControlRect(false);
            string[] labels = System.Enum.GetNames(selection.GetType()).Select(x => ObjectNames.NicifyVariableName(x)).ToArray();
            DrawDropdown(controlRect, Mathf.Clamp(System.Convert.ToInt32(selection), 0, labels.Length - 1), labels, false, (newSelection) => {
                onSelection?.Invoke((System.Enum)System.Enum.ToObject(selection.GetType(), newSelection));
            }, searcheable);
        }

        public static void DrawDropdown(Rect position, GUIContent title, System.Enum selection, System.Action<System.Enum> onSelection, bool searcheable = false)
        {
            string[] labels = System.Enum.GetNames(selection.GetType()).Select(x => ObjectNames.NicifyVariableName(x)).ToArray();
            DrawDropdown(title, position,  Mathf.Clamp(System.Convert.ToInt32(selection), 0, labels.Length - 1), labels, false, (newSelection) => {
                onSelection?.Invoke((System.Enum)System.Enum.ToObject(selection.GetType(), newSelection));
            }, searcheable);
        }

        public static void DrawDropdown(int selection, string[] options, bool rightAlign, System.Action<int> onSelection, bool searcheable = false)
        {
            Rect controlRect = EditorGUILayout.GetControlRect();
            DrawDropdown(controlRect, selection, options, rightAlign, onSelection,searcheable);
        }

        public static void DrawDropdown(Rect dropdownRect, int selection, string[] options, bool rightAlign, System.Action<int> onSelection, bool searcheable = false, string noOptionsMessage = "No options available")
        {
            if (selection < 0)
            {
                onSelection?.Invoke(0);
                EditorGUI.LabelField(dropdownRect, noOptionsMessage);
                return;
            }
            dropdownRect = new Rect(dropdownRect.x + EditorGUIUtility.standardVerticalSpacing, dropdownRect.y, dropdownRect.width - 2f * EditorGUIUtility.standardVerticalSpacing, dropdownRect.height);
            DrawDropdown(dropdownRect, options[selection], options, rightAlign, onSelection,searcheable);
        }

        public static void DrawDropdown(Rect dropdownRect, string selection, string[] options, System.Action<string> onSelection, string noOptionsMessage = "No options available")
        {
            int selectionIndex = new List<string>(options).IndexOf(selection);
            DrawDropdown(dropdownRect, selectionIndex, options, false, (newSelectionIndex) => {
                if (newSelectionIndex < options.Length)
                    onSelection(options[newSelectionIndex]);
                else
                    onSelection("");
            },false, noOptionsMessage);
        }

        public static void DrawDropdown(GUIContent title, Rect dropdownRect, int selection, string[] options, bool rightAlign, System.Action<int> onSelection, bool searcheable = false)
        {
            Rect controlRect = EditorGUI.PrefixLabel(dropdownRect, title);
            DrawDropdown(controlRect, selection, options, rightAlign, onSelection, searcheable);
        }

        public static void DrawDropdown(GUIContent title, SerializedProperty property, bool rightAlign, bool searcheable = false)
        {
            Rect labelRect = EditorGUILayout.GetControlRect(false);
            DrawDropdown(labelRect, title, property, rightAlign, searcheable);

        }

        public static void DrawDropdown(Rect controlRect, SerializedProperty property, bool searcheable = false)
        {
            DrawDropdown(controlRect, property, false, searcheable);
        }

        public static void DrawDropdown(SerializedProperty property, bool searcheable = false)
        {
            DrawDropdown(property, false, searcheable);
        }

        public static void DrawDropdown(Rect controlRect, GUIContent label, SerializedProperty property, bool searcheable = false)
        {
            DrawDropdown(controlRect, label, property, false, searcheable);
        }

        public static void DrawDropdown(GUIContent label, SerializedProperty property, bool searcheable = false)
        {
            DrawDropdown(label, property, false, searcheable);
        }

        public static void DrawDropdown(SerializedProperty property, bool rightAlign, bool searcheable = false)
        {
            Rect controlRect = EditorGUILayout.GetControlRect();
            DrawDropdown(controlRect, property, rightAlign, searcheable);
        }

        public static void DrawDropdown(Rect drawRect, GUIContent label, SerializedProperty property, bool rightAlign, bool searcheable = false)
        {
            Rect controlRect = EditorGUI.PrefixLabel(drawRect, label);
            DrawDropdown(controlRect, property, rightAlign, searcheable);
        }

        public static void DrawDropdown(Rect drawRect, SerializedProperty property, bool rightAlign, bool searcheable = false)
        {
            property.enumValueIndex = Mathf.Clamp(property.enumValueIndex, 0, property.enumDisplayNames.Length);
            DrawDropdown(drawRect, property.enumDisplayNames[property.enumValueIndex], property.enumDisplayNames, rightAlign, (value) => {
                property.enumValueIndex = value;
                property.serializedObject.ApplyModifiedProperties();
            }, searcheable);
        }

        public static void DrawDropdown(Rect drawRect, string displayedLabel, string[] options, bool rightAlign, System.Action<int> onSelection, bool searcheable = false)
        {
            DrawDropdownCustomIcon(drawRect, displayedLabel, options, rightAlign, onSelection, dropdownArrow,10f,0f, searcheable);
        }
        #endregion


        #region 3 dot dropdown
        public static void DrawThreeDotDropdown(Rect drawRect, string displayedLabel, string[] options, bool rightAlign, System.Action<int> onSelection, bool searcheable = false)
        {
            DrawDropdownCustomIcon(drawRect, displayedLabel, options, rightAlign, onSelection, threeDotMenu,16f,1f, searcheable);
        }

        #endregion

        #region Dropdown common
        public static void DrawDropdownCustomIcon(Rect drawRect, string displayedLabel, string[] options, bool rightAlign, System.Action<int> onSelection, Texture icon, float iconSize, float verticalOffset, bool searcheable = false)
        {


            GUIStyle styleToUse = rightAlign ? rightAlignDropDownStyle : dropDownStyle;

            //EditorGUI.LabelField(drawRect, displayedLabel);
            if (GUI.Button(drawRect, displayedLabel, styleToUse))
            {
                if (NodeEditorWindow.current != null)
                {
                    NodeEditorWindow.current.onLateGUI += () =>
                    {
                        Rect screenRectPosition = new Rect(Event.current.mousePosition, new Vector2(0, 0));
                        NonBlockingMenu menu = new NonBlockingMenu();
                        for (int index = 0; index < options.Length; index++)
                            menu.AddItem(new GUIContent(options[index]), false, (object data) => { onSelection?.Invoke((int)data); }, index, searcheable);
                        menu.DropDown(screenRectPosition);
                    };
                }
                else
                {
                    Rect screenRectPosition = new Rect(Event.current.mousePosition, new Vector2(0, 0));
                    NonBlockingMenu menu = new NonBlockingMenu();
                    for (int index = 0; index < options.Length; index++)
                        menu.AddItem(new GUIContent(options[index]), false, (object data) => { onSelection?.Invoke((int)data); }, index, searcheable);
                    menu.DropDown(screenRectPosition);
                }


            }

            Color guiColor = GUI.color;
            Color newGUIColor = dropDownStyle.normal.textColor;
            newGUIColor.a = 0.5f;
            GUI.color = newGUIColor;

            float margin = (drawRect.height - iconSize) / 2f;
            Rect dropdownArrowRect = new Rect(drawRect.x + drawRect.width - iconSize - margin, drawRect.y + margin + verticalOffset, iconSize, iconSize);
            GUI.DrawTexture(dropdownArrowRect, icon);
            GUI.color = guiColor;
        }

        #endregion


        #region Right aligned checkbox
        public static void DrawRightAlignedCheckbox(SerializedProperty property)
        {
            DrawRightAlignedCheckbox(property, property.displayName);
        }
        public static void DrawRightAlignedCheckbox(Rect controlRect, SerializedProperty property)
        {
            DrawRightAlignedCheckbox(controlRect, property, new GUIContent( property.displayName));
        }

        public static void DrawRightAlignedCheckbox(SerializedProperty property, string label)
        {
            DrawRightAlignedCheckbox(property, new GUIContent(label));
        }
        public static void DrawRightAlignedCheckbox(Rect controlRect, SerializedProperty property, GUIContent label)
        {
            property.boolValue = DrawRightAlignedCheckbox(controlRect, label, property.boolValue);
        }

        public static void DrawRightAlignedCheckbox(SerializedProperty property, GUIContent label)
        {
            Rect position = EditorGUILayout.GetControlRect();
            property.boolValue = DrawRightAlignedCheckbox(position, label, property.boolValue);
        }
        public static bool DrawRightAlignedCheckbox(Rect position, string label, bool value)
        {
            return DrawRightAlignedCheckbox(position, new GUIContent(label), value);
        }


        public static bool DrawRightAlignedCheckbox(Rect position, GUIContent label, bool value)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = position.width - EditorGUIUtility.singleLineHeight;
            bool returnValue = EditorGUI.Toggle(position, label, value);
            EditorGUIUtility.labelWidth = labelWidth;

            return returnValue;
        }
        #endregion

        #region Parameter Selector

        public static void DrawParameterSelector(SerializedProperty eventStringProperty, SerializedProperty parameterStringProperty, PlayerBase playerbase, bool allowNone = false, System.Type filterType = null)
        {
            Rect controlRect = EditorGUILayout.GetControlRect();
            DrawParameterSelector(controlRect, eventStringProperty, parameterStringProperty, playerbase, allowNone, filterType);
        }

        public static void DrawParameterSelector(GUIContent label, SerializedProperty eventStringProperty, SerializedProperty parameterStringProperty, PlayerBase playerbase, bool allowNone = false, System.Type filterType = null)
        {
            Rect controlRect = EditorGUILayout.GetControlRect();
            DrawParameterSelector(controlRect, label, eventStringProperty, parameterStringProperty, playerbase, allowNone, filterType);
        }

        public static void DrawParameterSelector(Rect position, GUIContent label, SerializedProperty eventStringProperty, SerializedProperty parameterStringProperty, PlayerBase playerbase, bool allowNone = false, System.Type filterType = null)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
            EditorGUI.LabelField(labelRect, label);

            Rect dropdownRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, position.height);
            DrawParameterSelector(dropdownRect, eventStringProperty, parameterStringProperty, playerbase, allowNone, filterType);
        }

        private static void DrawParameterSelector(Rect position, SerializedProperty eventStringProperty, SerializedProperty parameterStringProperty, PlayerBase playerbase, bool allowNone = false, System.Type filterType = null)
        {
            if (playerbase == null)
            {
                EditorGUI.HelpBox(position, "No Sound Graph player on Game Object", MessageType.Error);
                return;
            }

            SoundGraph soundgraph = playerbase.soundGraph;
            if (soundgraph == null)
            {
                EditorGUI.HelpBox(position, "No sound graph in player", MessageType.Error);
                return;
            }

            GraphEvent selectedEvent = soundgraph.GetAllEvents().Where(x => x.expose && x.eventID == eventStringProperty.stringValue).FirstOrDefault();
            if (selectedEvent == null)
            {
                EditorGUI.HelpBox(position, "No event selected", MessageType.Info);
                eventStringProperty.stringValue = "";
                eventStringProperty.serializedObject.ApplyModifiedProperties();
                return;
            }


            List<GraphEvent.EventParameterDef> parameters = filterType == null ? selectedEvent.parameters : selectedEvent.parameters.Where(x => x.parameterTypeName == filterType.FullName).ToList();

            int currentSelectionIndex = parameters.FindIndex(x => x.parameterName == parameterStringProperty.stringValue);

            if (allowNone)
            {
                currentSelectionIndex++;
                if (currentSelectionIndex < 0)
                {
                    currentSelectionIndex = 0;
                    eventStringProperty.stringValue = "";
                    eventStringProperty.serializedObject.ApplyModifiedProperties();
                }

                List<string> parameterNames = parameters.Select(x => x.parameterName).ToList();
                parameterNames.Insert(0, "None");

                DrawDropdown(position, currentSelectionIndex, parameterNames.ToArray(), false, (newSelection) => {

                    if (newSelection - 1 >= 0)
                    {
                        newSelection = newSelection - 1;
                        parameterStringProperty.stringValue = parameters[newSelection].parameterName;
                    }
                    else
                    {
                        parameterStringProperty.stringValue = "";
                    }
                    parameterStringProperty.serializedObject.ApplyModifiedProperties();

                });
            }
            else
            {
                if (currentSelectionIndex < 0)
                {
                    currentSelectionIndex = 0;
                    parameterStringProperty.stringValue = parameters[0].parameterName;
                    parameterStringProperty.serializedObject.ApplyModifiedProperties();
                }

                DrawDropdown(position, currentSelectionIndex, parameters.Select(x => x.parameterName).ToArray(), false, (newSelection) => {
                    parameterStringProperty.stringValue = parameters[newSelection].parameterName;
                    parameterStringProperty.serializedObject.ApplyModifiedProperties();
                });
            }
        }
        #endregion

        #region Event Selector
        public static void DrawEventSelector(SerializedProperty eventStringProperty, PlayerBase playerbase, bool allowNone = false)
        {
            Rect controlRect = EditorGUILayout.GetControlRect();
            DrawEventSelector(controlRect, eventStringProperty, playerbase, allowNone);
        }

        public static void DrawEventSelector(GUIContent label, SerializedProperty eventStringProperty, PlayerBase playerbase, bool allowNone = false)
        {
            Rect controlRect = EditorGUILayout.GetControlRect();
            DrawEventSelector(controlRect, label, eventStringProperty, playerbase, allowNone);
        }

        public static void DrawEventSelector(Rect position, GUIContent label, SerializedProperty eventStringProperty, PlayerBase playerbase, bool allowNone = false)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
            EditorGUI.LabelField(labelRect, label);

            Rect dropdownRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, position.height);
            DrawEventSelector(dropdownRect, eventStringProperty, playerbase, allowNone);
        }

        public static void DrawEventSelector(Rect position, SerializedProperty eventStringProperty, PlayerBase playerbase, bool allowNone = false)
        {
            if (playerbase == null)
            {
                EditorGUI.HelpBox(position, "No Sound Graph player on Game Object", MessageType.Error);
                return;
            }

            SoundGraph soundgraph = playerbase.soundGraph;
            DrawEventSelector(position, eventStringProperty, soundgraph, allowNone);
        }


        public static void DrawEventSelector(Rect position, GUIContent label, SerializedProperty eventStringProperty, SoundGraph soundgraph, bool allowNone = false)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
            EditorGUI.LabelField(labelRect, label);

            Rect dropdownRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, position.height);
            DrawEventSelector(dropdownRect, eventStringProperty, soundgraph, allowNone);
        }


        public static void DrawEventSelector(Rect position, SerializedProperty eventStringProperty, SoundGraph soundgraph, bool allowNone = false)
        {
            
            if (soundgraph == null)
            {
                EditorGUI.HelpBox(position, "No sound graph in target", MessageType.Error);
                return;
            }

            List<GraphEvent> events = soundgraph.GetAllEvents().Where(x => x.expose).ToList();
            if (events.Count == 0)
            {
                EditorGUI.HelpBox(position, "No events in Sound Graph", MessageType.Warning);
                return;
            }


            int currentSelectionIndex = events.FindIndex(x => x.eventID == eventStringProperty.stringValue);

            if (allowNone)
            {
                currentSelectionIndex++;
                if (currentSelectionIndex < 0)
                {
                    currentSelectionIndex = 0;
                    eventStringProperty.stringValue = "";
                    eventStringProperty.serializedObject.ApplyModifiedProperties();
                }

                List<string> eventNames = events.Select(x => x.eventName).ToList();
                eventNames.Insert(0, "None");

                DrawDropdown(position, currentSelectionIndex, eventNames.ToArray(), false, (newSelection) => {

                    if (newSelection - 1 >= 0)
                    {
                        newSelection = newSelection - 1;
                        eventStringProperty.stringValue = events[newSelection].eventID;
                    }
                    else
                    {
                        eventStringProperty.stringValue = "";
                    }
                    eventStringProperty.serializedObject.ApplyModifiedProperties();

                });
            }
            else
            {
                if (currentSelectionIndex < 0)
                {
                    currentSelectionIndex = 0;
                    eventStringProperty.stringValue = events[0].eventID;
                    eventStringProperty.serializedObject.ApplyModifiedProperties();
                }

                DrawDropdown(position, currentSelectionIndex, events.Select(x => x.eventName).ToArray(), false, (newSelection) => {
                    eventStringProperty.stringValue = events[newSelection].eventID;
                    eventStringProperty.serializedObject.ApplyModifiedProperties();
                });
            }
        }

        #endregion

        #region Type Selector





        public static void DrawTypeSelector(SerializedProperty typeProperty, VariableInspectorUtility.EditorFilter filter, System.Action onChange = null)
        {
            Rect controlRect = EditorGUILayout.GetControlRect();
            DrawTypeSelector(controlRect, typeProperty, filter, onChange);
        }

        public static void DrawTypeSelector(SerializedProperty typeProperty, System.Type[] allowedTypes, System.Action onChange = null)
        {
            DrawTypeSelector(typeProperty, new List<System.Type>(allowedTypes), onChange);
        }



        public static void DrawTypeSelector(SerializedProperty typeProperty, List<System.Type> allowedTypes, System.Action onChange = null)
        {
            // Doing Variable Type Selector
            Rect controlRect = EditorGUILayout.GetControlRect();
            DrawTypeSelector(controlRect, typeProperty, allowedTypes, onChange);
        }



        public static void DrawTypeSelector(SerializedProperty typeProperty, string label, VariableInspectorUtility.EditorFilter filter, System.Action onChange = null)
        {
            Rect controlRect = EditorGUILayout.GetControlRect();
            DrawTypeSelector(controlRect, typeProperty, label, filter, onChange);
        }

        public static void DrawTypeSelector(SerializedProperty typeProperty, string label, System.Type[] allowedTypes, System.Action onChange = null)
        {

            Rect controlRect = EditorGUILayout.GetControlRect();
            DrawTypeSelector(controlRect, typeProperty, label, allowedTypes, onChange);
        }

        public static void DrawTypeSelector(SerializedProperty typeProperty, string label, List<System.Type> allowedTypes, System.Action onChange = null)
        {

            Rect controlRect = EditorGUILayout.GetControlRect();
            DrawTypeSelector(controlRect, typeProperty, label, allowedTypes, onChange);
        }

        public static void DrawTypeSelector(SerializedProperty typeProperty, string label, List<string> allowedTypes, List<string> prettyNames, System.Action onChange = null)
        {
            Rect controlRect = EditorGUILayout.GetControlRect();
            DrawTypeSelector(controlRect,typeProperty, label, allowedTypes, prettyNames, onChange);
        }




        public static void DrawTypeSelector(Rect position, SerializedProperty typeProperty, string label, VariableInspectorUtility.EditorFilter filter, System.Action onChange = null)
        {

            List<string> variableTypes = VariableInspectorUtility.GetManagedTypes(filter);
            List<string> prettyNames = VariableInspectorUtility.GetPrettyNames(filter);
            DrawTypeSelector(position, typeProperty, label, variableTypes, prettyNames, onChange);
        }

        public static void DrawTypeSelector(Rect position, SerializedProperty typeProperty, string label, System.Type[] allowedTypes, System.Action onChange = null)
        {
            DrawTypeSelector(position, typeProperty, label, new List<System.Type>(allowedTypes), onChange);
        }

        public static void DrawTypeSelector(Rect position, SerializedProperty typeProperty, string label, List<System.Type> allowedTypes, System.Action onChange = null)
        {
            List<string> fullNames = allowedTypes.Select(x => x.FullName).ToList();
            List<string> prettyNames = fullNames.Select(x => VariableInspectorUtility.GetPrettyName(x)).ToList();
            DrawTypeSelector(position, typeProperty, label, fullNames, prettyNames , onChange);
        }

        public static void DrawTypeSelector(Rect position, SerializedProperty typeProperty, string label, List<string> allowedTypes, List<string> prettyNames, System.Action onChange = null)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
            EditorGUI.LabelField(labelRect, label);

            Rect controlRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, position.height);
            DrawTypeSelector(controlRect, typeProperty, allowedTypes, prettyNames, onChange);
        }


        public static void DrawTypeSelector(Rect position, SerializedProperty typeProperty, VariableInspectorUtility.EditorFilter filter, System.Action onChange = null)
        {
            // Doing Variable Type Selector
            List<string> variableTypes = VariableInspectorUtility.GetManagedTypes(filter);
            List<string> prettyNames = VariableInspectorUtility.GetPrettyNames(filter);
            DrawTypeSelector(position, typeProperty, variableTypes, prettyNames, onChange);
        }

        

        public static void DrawTypeSelector(Rect position, SerializedProperty typeProperty, List<System.Type> allowedTypes, System.Action onChange = null)
        {
            // Doing Variable Type Selector
            List<string> fullNames = allowedTypes.Select(x => x.FullName).ToList();
            List<string> prettyNames = fullNames.Select(x => VariableInspectorUtility.GetPrettyName(x)).ToList();
            DrawTypeSelector(position, typeProperty, fullNames, prettyNames, onChange);
        }

        public static void DrawTypeSelector(Rect position, SerializedProperty typeProperty, List<string> allowedTypeFullNames, List<string> prettyNames, System.Action onChange = null)
        {
            if (allowedTypeFullNames.Count == 0)
            {
                EditorGUI.LabelField(position, "No possible options");
                return;
            }


            int currentSelection = allowedTypeFullNames.IndexOf(typeProperty.stringValue);
            if (currentSelection < 0)
            {
                currentSelection = 0;
                typeProperty.stringValue = allowedTypeFullNames[0];
                typeProperty.serializedObject.ApplyModifiedProperties();
                onChange?.Invoke();
            }

            LayersGUIUtilities.DrawDropdown(position, currentSelection, prettyNames.ToArray(), false, (newSelection) => {
                typeProperty.stringValue = allowedTypeFullNames[newSelection];
                typeProperty.serializedObject.ApplyModifiedProperties();
                onChange?.Invoke();
            }, true);
        }


        public static void DrawTypeSelector(string type, List<string> allowedTypeFullNames, List<string> prettyNames, System.Action<string> onChange)
        {
            Rect controlRect = EditorGUILayout.GetControlRect();
            DrawTypeSelector(controlRect, type, allowedTypeFullNames, prettyNames, onChange);
        }

        public static void DrawTypeSelector(string type, VariableInspectorUtility.EditorFilter filter, System.Action<string> onChange)
        {
            Rect controlRect = EditorGUILayout.GetControlRect();
            DrawTypeSelector(controlRect, type, filter, onChange);
        }

        public static void DrawTypeSelector(Rect position, string type, VariableInspectorUtility.EditorFilter filter, System.Action<string> onChange)
        {
            List<string> variableTypes = VariableInspectorUtility.GetManagedTypes(filter);
            List<string> prettyNames = VariableInspectorUtility.GetPrettyNames(filter);
            DrawTypeSelector(position, type, variableTypes, prettyNames, onChange);
        }

        public static void DrawTypeSelector(Rect position, string type, List<string> allowedTypeFullNames, List<string> allowedTypePrettyNames, System.Action<string> onChange)
        {
            // Doing Variable Type Selector

            int currentSelection = allowedTypeFullNames.IndexOf(type);
            if (currentSelection < 0)
            {
                currentSelection = 0;
                type = allowedTypeFullNames[0];
                onChange?.Invoke(type);
            }

            LayersGUIUtilities.DrawDropdown(position, currentSelection, allowedTypePrettyNames.ToArray(), false, (newSelection) => {
                type = allowedTypeFullNames[newSelection];
                onChange?.Invoke(type);
            });
        }


        public static void DrawTypeSelector(string type, string label, List<string> allowedTypeFullNames, List<string> prettyNames, System.Action<string> onChange)
        {
            Rect controlRect = EditorGUILayout.GetControlRect();
            DrawTypeSelector(controlRect, label, type, allowedTypeFullNames, prettyNames, onChange);

        }

        public static void DrawTypeSelector(string type, string label, VariableInspectorUtility.EditorFilter filter, System.Action<string> onChange)
        {
            Rect controlRect = EditorGUILayout.GetControlRect();
            DrawTypeSelector(controlRect, label, type, filter, onChange);
        }

        public static void DrawTypeSelector(Rect position, string label, string type, VariableInspectorUtility.EditorFilter filter, System.Action<string> onChange)
        {
            List<string> variableTypes = VariableInspectorUtility.GetManagedTypes(filter);
            List<string> prettyNames = VariableInspectorUtility.GetPrettyNames(filter);
            DrawTypeSelector(position, label, type, variableTypes, prettyNames, onChange);
        }

        public static void DrawTypeSelector(Rect position, string label, string type, List<string> allowedTypeFullNames, List<string> prettyNames, System.Action<string> onChange)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
            EditorGUI.LabelField(labelRect, label);

            Rect controlRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, position.height);
            DrawTypeSelector(controlRect, type, allowedTypeFullNames, prettyNames, onChange);
        }



        #endregion

        #region label size

        private static float lastLabelLevel = 0f;
        public static void BeginNewLabelWidth(float labelWidth)
        {
            lastLabelLevel = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = labelWidth;
        }

        public static void EndNewLabelWidth()
        {
            EditorGUIUtility.labelWidth = lastLabelLevel;
        }
        #endregion


        #region Variable Selector

        /// <summary>
        /// Draws a selector for variables in the given soundgraph
        /// </summary>
        /// <param name="variableIDProp"></param>
        /// <param name="soundGraph"></param>
        /// <param name="filterFunction"></param>
        public static void DrawVariableSelector( SerializedProperty variableIDProp, SoundGraph soundGraph, System.Func<GraphVariable, bool> filterFunction = null)
        {
            Rect controlRect = EditorGUILayout.GetControlRect();
            DrawVariableSelector(controlRect, variableIDProp, soundGraph, filterFunction);
        }

        /// <summary>
        /// Draws a selector for variables in the given soundgraph
        /// </summary>
        /// <param name="variableIDProp"></param>
        /// <param name="label"></param>
        /// <param name="soundGraph"></param>
        /// <param name="filterFunction"></param>
        public static void DrawVariableSelector(SerializedProperty variableIDProp, string label, SoundGraph soundGraph, System.Func<GraphVariable, bool> filterFunction = null)
        {
            Rect controlRect = EditorGUILayout.GetControlRect();
            DrawVariableSelector(controlRect, label, variableIDProp, soundGraph, filterFunction);
        }

        /// <summary>
        /// Draws a selector for variables in the given soundgraph
        /// </summary>
        /// <param name="position"></param>
        /// <param name="label"></param>
        /// <param name="variableIDProp"></param>
        /// <param name="soundGraph"></param>
        /// <param name="filterFunction"></param>
        public static void DrawVariableSelector(Rect position, string label, SerializedProperty variableIDProp, SoundGraph soundGraph, System.Func<GraphVariable, bool> filterFunction = null)
        {
            DrawVariableSelector(position, new GUIContent(label), variableIDProp, soundGraph, filterFunction);
        }

        /// <summary>
        /// Draws a selector for variables in the given soundgraph
        /// </summary>
        /// <param name="position"></param>
        /// <param name="label"></param>
        /// <param name="variableIDProp"></param>
        /// <param name="soundGraph"></param>
        /// <param name="filterFunction"></param>
        public static void DrawVariableSelector(Rect position, GUIContent label, SerializedProperty variableIDProp, SoundGraph soundGraph, System.Func<GraphVariable, bool> filterFunction = null)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
            EditorGUI.LabelField(labelRect, label);

            Rect variableSelectorRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, position.height);
            DrawVariableSelector(variableSelectorRect, variableIDProp, soundGraph, filterFunction);
        }

        /// <summary>
        /// Draws a selector for variables in the given soundgraph
        /// </summary>
        /// <param name="position"></param>
        /// <param name="variableIDProp"></param>
        /// <param name="soundGraph"></param>
        /// <param name="filterFunction"></param>
        public static void DrawVariableSelector(Rect position, SerializedProperty variableIDProp, SoundGraph soundGraph, System.Func<GraphVariable, bool> filterFunction = null)
        {
            if (soundGraph == null)
            {
                EditorGUI.HelpBox(position, "No sound graph in target", MessageType.Error);
                return;
            }

            List<GraphVariable> variables = soundGraph.GetAllVariables();

            if (filterFunction != null)
                variables = variables.Where(filterFunction).ToList();


            if (variables.Count == 0) {
                EditorGUI.HelpBox(position, "No valid variables available", MessageType.Info);
                return;
            }

            List<string> variableNames = variables.Select(x => x.name).ToList();

            int selection = variables.FindIndex(x => x.variableID == variableIDProp.stringValue);
            if (selection < 0)
            {
                variableIDProp.stringValue = variables[0].variableID;
                selection = 0;
                variableIDProp.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }


            DrawDropdown(position, selection, variableNames.ToArray(), false, (newSelection) => {
                variableIDProp.stringValue = variables[newSelection].variableID;
                variableIDProp.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            });
        }


        #endregion

        #region expandable header

        private static GUIStyle _expandHeaderStyle;
        private static GUIStyle expandHeaderStyle
        {
            get
            {
                if (_expandHeaderStyle == null)
                {
                    _expandHeaderStyle = new GUIStyle(EditorStyles.label);
                    _expandHeaderStyle.padding = new RectOffset(5, 0, 0, 0);
                    _expandHeaderStyle.alignment = TextAnchor.MiddleLeft;
                }
                return _expandHeaderStyle;
            }
        }

        public static bool ExpandHeader(Rect position, string label, bool expanded, Color headerColor)
        {
            EditorGUI.DrawRect(position, headerColor);
            EditorGUI.LabelField(position, label, expandHeaderStyle);
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && position.Contains(Event.current.mousePosition))
                return !expanded;
            return expanded;
        }
        #endregion

        #region expandableParameter
        public static void DrawExpandableProperty(string portName, SerializedObjectTree so, System.Action drawFunction)
        {
            Node target = so.targetObject as Node;
            DrawExpandableProperty(target.GetPort(portName),so, drawFunction);
        }

        public static void DrawExpandableProperty(string portName, SerializedObjectTree so)
        {
            Node target = so.targetObject as Node;
            DrawExpandableProperty(target.GetPort(portName), so);
        }

        public static void DrawExpandableProperty(NodePort port, SerializedObjectTree serializedObject, System.Action drawFunction)
        {
            if (port == null)
                return;

            DrawExpandableProperty(port.IsConnected, serializedObject, drawFunction);
        }

        public static void DrawExpandableProperty(SerializedObjectTree serializedObject, System.Action drawFunction)
        {
            DrawExpandableProperty(false, serializedObject, drawFunction);
        }

        public static void DrawExpandableProperty(bool alwaysExpanded, SerializedObjectTree serializedObject, System.Action drawFunction)
        {
            if (alwaysExpanded || serializedObject.FindProperty("expanded").boolValue)
                drawFunction?.Invoke();
        }

        public static void DrawExpandableProperty(SerializedPropertyTree property)
        {
            Node target = property.serializedObject.targetObject as Node;
            NodePort port = target.GetInputPort(property.name);
            if (port == null)
                DrawExpandableProperty(false, property.serializedObject,() =>
                {
                    EditorGUILayout.PropertyField(property);
                });
            else
                DrawExpandableProperty(port, property.serializedObject, () => {
                    NodeEditorGUILayout.PropertyField(property);
                });
        }
        /*
        public static void DrawExpandableProperty(NodeLayoutUtility layout, NodePort port,SerializedObjectTree serializedObjectTree)
        {
            if (port == null)
                return;

            DrawExpandableProperty(port.)
            NodeEditorGUIDraw.PortField(layout.DrawLine(), port, serializedObjectTree);
        }*/

        public static void DrawExpandableProperty(NodeLayoutUtility layout, SerializedPropertyTree property)
        {
            Node target = property.serializedObject.targetObject as Node;
            NodePort port = target.GetInputPort(property.name);
            if (port == null)
                DrawExpandableProperty(false, property.serializedObject, () =>
                {
                    LayersGUIUtilities.FastPropertyField(layout.DrawLine(), property);
                });
            else
                DrawExpandableProperty(port, property.serializedObject, () => {
                    LayersGUIUtilities.FastPropertyField(layout.DrawLine(), property);
                });
        }

        public static void DrawExpandableProperty(NodePort nodePort, SerializedObjectTree target)
        {
            DrawExpandableProperty(nodePort.IsConnected, target, () => {
                NodeEditorGUILayout.PortField(nodePort);
            });
        }

        public static void DrawExpandableProperty(NodeLayoutUtility layout, NodePort nodePort, SerializedObjectTree target)
        {
            DrawExpandableProperty(nodePort.IsConnected, target, () => {
                NodeEditorGUIDraw.PortField(layout.DrawLine(),nodePort, target);
            });
        }

        public static void DrawExpandableProperty(NodePort input, NodePort output, SerializedObjectTree target)
        {
            GUILayout.BeginHorizontal();

            DrawExpandableProperty(input,target, () => {
                NodeEditorGUILayout.PortField(input, GUILayout.MinWidth(0));
            });

            DrawExpandableProperty(output, target, () => {
                NodeEditorGUILayout.PortField(output, GUILayout.MinWidth(0));
            });
            GUILayout.EndHorizontal();
        }

        public static void DrawExpandableProperty(NodeLayoutUtility layout, NodePort input, NodePort output, SerializedObjectTree target)
        {
            bool firstElementDrawn = false;
            Rect firstElementRect = new Rect();
            DrawExpandableProperty(input, target, () => {
                firstElementRect = layout.DrawLine();
                firstElementDrawn = true;
                NodeEditorGUIDraw.PortField(firstElementRect, input, target);
            });

            DrawExpandableProperty(output, target, () => {
                if (!firstElementDrawn)
                    firstElementRect = layout.DrawLine();

                NodeEditorGUIDraw.PortField(firstElementRect, output, target);
            });
        }

        #endregion


        #region Draw Or Create Port
        public static void DrawOrCreatePort(Rect position, Node target, NodePort.IO direction, System.Type type, Node.ConnectionType connectionType, Node.TypeConstraint typeConstraint, string portname, string displayName)
        {

            DrawOrCreatePort(position, 0, target, direction, type, connectionType, typeConstraint, portname, displayName);
        }

        public static void DrawOrCreatePort(float nodeOffset, Node target, NodePort.IO direction, System.Type type, Node.ConnectionType connectionType, Node.TypeConstraint typeConstraint, string portname, string displayName)
        {
            Rect controlRect = EditorGUILayout.GetControlRect();
            DrawOrCreatePort(controlRect, nodeOffset, target, direction, type, connectionType, typeConstraint, portname, displayName);
        }

        public static void DrawOrCreatePort(Rect position, float nodeOffset, Node target, NodePort.IO direction, System.Type type, Node.ConnectionType connectionType, Node.TypeConstraint typeConstraint, string portname, string displayName)
        {
            if (type == null)
            {
                if (GUI.Button(position, "Regen Scripts"))
                {
                    Code_generation.RegenList.GenerateAll();
                }
            }
            else
            {

                if (direction == NodePort.IO.Input)
                    EditorGUI.LabelField(position, displayName);
                else
                    EditorGUI.LabelField(position, displayName, rightAlignedCenteredText);

                Vector2 vectorPos = new Vector2(position.x - EditorGUIUtility.singleLineHeight + nodeOffset, position.y);
                if (direction == NodePort.IO.Output)
                    vectorPos = new Vector2(position.x + position.width + nodeOffset, position.y);
                DrawOrCreatePort(vectorPos, target, direction, type, connectionType, typeConstraint, portname);
            }
        }

        public static void DrawOrCreatePort(Rect position, Node target, string displayName,
            float leftPortOffset,  System.Type leftType, Node.ConnectionType leftConnectionType, Node.TypeConstraint leftTypeConstraint, string leftPortname,
            float rightPortOffset, System.Type rightType, Node.ConnectionType rightConnectionType, Node.TypeConstraint rightTypeConstraint, string rightPortname)
        {

            if(leftType == null && rightType == null)
            {
                if (GUI.Button(position, "Regen Scripts"))
                {
                    Code_generation.RegenList.GenerateAll();
                }
            }
            else if (leftType == null && rightType != null)
            {
                Rect buttonRect = new Rect(position.x, position.y, position.width / 2f, position.height);
                if (GUI.Button(buttonRect, "Regen Scripts"))
                {
                    Code_generation.RegenList.GenerateAll();
                }
                Rect rightPortRect = new Rect(position.x + (position.width / 2f), position.y, position.width / 2f, position.height);
                DrawOrCreatePort(rightPortRect, rightPortOffset, target, NodePort.IO.Output, rightType, rightConnectionType, rightTypeConstraint, rightPortname, displayName);
            }
            else if (leftType != null && rightType == null)
            {
                Rect leftRect = new Rect(position.x, position.y, position.width / 2f, position.height);
                DrawOrCreatePort(leftRect, leftPortOffset, target, NodePort.IO.Input, leftType, leftConnectionType, leftTypeConstraint, leftPortname, displayName);
                
                Rect rightRect = new Rect(position.x + (position.width / 2f), position.y, position.width / 2f, position.height);
                if (GUI.Button(rightRect, "Regen Scripts"))
                {
                    Code_generation.RegenList.GenerateAll();
                }
            }
            else
            {

                EditorGUI.LabelField(position, displayName);

                Vector2 leftPortPosition = new Vector2(position.x - EditorGUIUtility.singleLineHeight + leftPortOffset, position.y);
                Vector2 rightPortPosition = new Vector2(position.x + position.width + rightPortOffset, position.y);
                DrawOrCreatePort(leftPortPosition, target, NodePort.IO.Input, leftType, leftConnectionType, leftTypeConstraint, leftPortname);
                DrawOrCreatePort(rightPortPosition, target, NodePort.IO.Output, rightType, rightConnectionType, rightTypeConstraint, rightPortname);
            }
        }

        public static void DrawOrCreatePort(Vector2 position, Node target, NodePort.IO direction, System.Type type, Node.ConnectionType connectionType, Node.TypeConstraint typeConstraint, string portname)
        {
            NodePort port = null;
            port = target.GetPort(portname);

            if (port != null && (port.direction != direction || port.ValueType != type || port.connectionType != connectionType || port.typeConstraint != typeConstraint))
            {
                target.RemoveDynamicPort(port);
                port = null;
            }

            if (port == null)
            {
                if (direction == NodePort.IO.Input)
                    port = target.AddDynamicInput(type, connectionType, typeConstraint, portname);
                else
                    port = target.AddDynamicOutput(type, connectionType, typeConstraint, portname);
            }

            if (port != null)
            {
                
                NodeEditorGUILayout.PortField(position, port);
            }
        }

        #endregion

        public static void FastPropertyField(Rect position, SerializedProperty property)
        {
            FastPropertyField(position, null, property);
        }
        public static void FastPropertyField(Rect position, GUIContent label, SerializedProperty property)
        {
            if (label == null)
                label = new GUIContent(property.displayName);

            switch (property.propertyType)
            {
                case SerializedPropertyType.Generic:
                    break;
                case SerializedPropertyType.Integer:
                    property.intValue = EditorGUI.IntField(position, label, property.intValue);
                    break;
                case SerializedPropertyType.Boolean:
                    property.boolValue = EditorGUI.Toggle(position, label, property.boolValue);
                    break;
                case SerializedPropertyType.Float:
                    property.floatValue = EditorGUI.FloatField(position, label, property.floatValue);
                    break;
                case SerializedPropertyType.String:
                    property.stringValue = EditorGUI.TextField(position, label, property.stringValue);
                    break;
                case SerializedPropertyType.Color:
                    property.colorValue = EditorGUI.ColorField(position, label, property.colorValue);
                    break;
                case SerializedPropertyType.ObjectReference:
                    EditorGUI.ObjectField(position,  property, label);
                    break;
                case SerializedPropertyType.LayerMask:
                    property.intValue = EditorGUI.LayerField(position, label, property.intValue);
                    break;
                case SerializedPropertyType.Enum:
                    EditorGUI.PropertyField(position,property, label);//Using this as a fallback. Enums shouldn't be used this way anyways
                    break;
                case SerializedPropertyType.Vector2:
                    property.vector2Value = EditorGUI.Vector2Field(position, label, property.vector2Value);
                    break;
                case SerializedPropertyType.Vector3:
                    property.vector3Value = EditorGUI.Vector3Field(position, label, property.vector3Value);
                    break;
                case SerializedPropertyType.Vector4:
                    property.vector4Value = EditorGUI.Vector4Field(position, label, property.vector4Value);
                    break;
                case SerializedPropertyType.Rect:
                    property.rectValue = EditorGUI.RectField(position, label, property.rectValue);
                    break;
                case SerializedPropertyType.ArraySize:
                    property.arraySize = EditorGUI.IntField(position, label, property.arraySize);
                    break;
                case SerializedPropertyType.Character:
                    break;
                case SerializedPropertyType.AnimationCurve:
                    property.animationCurveValue = EditorGUI.CurveField(position, label, property.animationCurveValue);
                    break;
                case SerializedPropertyType.Bounds:
                    property.boundsValue = EditorGUI.BoundsField(position, label, property.boundsValue);
                    break;
                case SerializedPropertyType.Gradient:
                    
                    break;
                case SerializedPropertyType.Quaternion:
                    property.quaternionValue =Quaternion.Euler( EditorGUI.Vector3Field(position, label, property.quaternionValue.eulerAngles));
                    break;
                case SerializedPropertyType.ExposedReference:
                    break;
                case SerializedPropertyType.FixedBufferSize:
                    break;
                case SerializedPropertyType.Vector2Int:
                    property.vector2IntValue = EditorGUI.Vector2IntField(position, label, property.vector2IntValue);
                    break;
                case SerializedPropertyType.Vector3Int:
                    property.vector3IntValue = EditorGUI.Vector3IntField(position, label, property.vector3IntValue);
                    break;
                case SerializedPropertyType.RectInt:
                    property.rectIntValue = EditorGUI.RectIntField(position, label, property.rectIntValue);
                    break;
                case SerializedPropertyType.BoundsInt:
                    property.boundsIntValue = EditorGUI.BoundsIntField(position, label, property.boundsIntValue);
                    break;
            }
        }

        public static void DrawBox(int lines, string title, System.Action drawFunction)
        {
            Rect boxRect = EditorGUILayout.GetControlRect();
            boxRect.height = (lines + 1) * EditorGUIUtility.singleLineHeight + (lines + 2) * EditorGUIUtility.standardVerticalSpacing;
            GUI.Box(boxRect, title, GUI.skin.window);

            EditorGUI.indentLevel++;
            drawFunction?.Invoke();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }


        #region Draw Note
        public static void DrawNote(SerializedProperty noteNumberProp)
        {
            Rect controlRect = EditorGUILayout.GetControlRect();
            DrawNote(controlRect, noteNumberProp);
        }

        public static void DrawNote(int noteNumber, System.Action<int> onChange)
        {
            Rect controlRect = EditorGUILayout.GetControlRect();
            DrawNote(controlRect, noteNumber,onChange);
        }

        public static void DrawNote(Rect controlRect, SerializedProperty noteNumberProp)
        {
            DrawNote(controlRect, noteNumberProp.intValue, (newValue)=> {
                noteNumberProp.intValue = newValue;
                noteNumberProp.serializedObject.ApplyModifiedProperties();
            }, noteNumberProp.displayName);
        }

        private static string[] noteNames = new string[0];

        public static void DrawNote(Rect controlRect, int noteNumber, System.Action<int> onChange, string label = "Note")
        {
            if (noteNames.Length == 0)
            {
                noteNames = new string[128];
                for (int index = 0; index < noteNames.Length; index++)
                    noteNames[index] = MidiUtils.NoteNumberToMenuFormattedName(index);
            }

            Rect numberRect = new Rect(controlRect.x, controlRect.y, controlRect.width - 35, controlRect.height);
            noteNumber = EditorGUI.IntField(numberRect, new GUIContent(label), noteNumber);


            Rect noteRect = new Rect(controlRect.x + controlRect.width - 35, controlRect.y, 35, controlRect.height);
            DrawDropdown(noteRect,MidiUtils.NoteNumberToName(noteNumber),  noteNames, false, onChange, true);

        }
        #endregion

        #region Incoming Parameter Selector
        public static void IncomingParameterSelector(FlowNode thisNode, SerializedProperty parameterNameProp, string incomingPort, System.Type type)
        {
            Rect layoutPosition = EditorGUILayout.GetControlRect();

            IncomingParameterSelector(layoutPosition, thisNode, parameterNameProp, incomingPort, type);
        }
        public static void IncomingParameterSelector(Rect postition, FlowNode thisNode, SerializedProperty parameterNameProp, string incomingPort, System.Type type)
        {
            List<string> defs = thisNode.GetIncomingEventParameterDefsOnPort(incomingPort, new List<Node>()).Where(x=>x.parameterTypeName == type.FullName).Select(x=>x.parameterName).ToList();

            string noParameterMessage = "No parameters of type " + type.Name;
            NodePort port = thisNode.GetPort(incomingPort);
            if (!port.IsConnected)
                noParameterMessage = "No incoming events";

            DrawDropdown(postition, parameterNameProp.stringValue, defs.ToArray(), (newSelection)=>{
                parameterNameProp.stringValue = newSelection;
            }, noParameterMessage);
        }
        #endregion
    }
}
