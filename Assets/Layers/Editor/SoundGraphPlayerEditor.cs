using System.Collections.Generic;
using ABXY.Layers.Editor.Code_generation;
using ABXY.Layers.Editor.Graph_Variable_Editors;
using ABXY.Layers.Editor.Node_Editor_Window;
using ABXY.Layers.ThirdParty.Malee.List;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Sound_graph_players;
using UnityEditor;
using UnityEngine;
using ABXY.Layers.Runtime.Settings;

namespace ABXY.Layers.Editor
{
    [CustomEditor(typeof(PlayerBase),true)]
    public class SoundGraphPlayerEditor : UnityEditor.Editor
    {
        //SerializedProperty soundGraphProp = null;
        //SerializedProperty playOnAwakeProp = null;
        //SerializedProperty dontDestroyOnLoadProp = null;

        //private SerializedProperty varList;
        //private SerializedProperty eventList;

        private ReorderableList startingEventsList;

        private Dictionary<string, GraphVariableEditor> path2VariableEditors = new Dictionary<string, GraphVariableEditor>();

        private SerializedObjectTree serializedObjectTree;

        private bool connectedToRuntimeGraph { get { return (target as SoundGraphPlayer).runtimeGraphCopy != null; } }

        private void OnEnable()
        {
            serializedObjectTree = new SerializedObjectTree(serializedObject);

            startingEventsList = new ReorderableList(serializedObjectTree.FindProperty("startingEvents"));
            startingEventsList.drawElementCallback += DrawStartingEvent;
            startingEventsList.getElementHeightCallback += CalculateStartingEventHeight;
        }

        

        private float CalculateStartingEventHeight(SerializedPropertyTree element)
        {
            float startingHeight = EditorGUIUtility.singleLineHeight + 2f*EditorGUIUtility.standardVerticalSpacing;
            SerializedPropertyTree parameterList = element.FindPropertyRelative("parameters");

            for (int index = 0; index < parameterList.arraySize; index++)
            {
                SerializedPropertyTree parameterProperty = parameterList.GetArrayElementAtIndex(index);
                GraphVariableEditor editor = LoadEditor(parameterProperty);
                float parameterHeight = VariableInspectorDrawFunctions.InputNodeFNs.GetValueHeight(target, parameterProperty,"",editor, GraphVariable.RetrievalTypes.DefaultValue,true);

                startingHeight += parameterHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            return startingHeight;
        }

        private void DrawStartingEvent(Rect rect, SerializedPropertyTree element, GUIContent label, bool selected, bool focused)
        {

            SerializedPropertyTree eventList = serializedObjectTree.FindProperty("localEventsList");
            if (eventList.arraySize == 0)
            {
                EditorGUI.HelpBox(rect, "No events are currently exposed in this graph!", MessageType.Info);
                return;
            }
            List<string> graphEventNames = new List<string>();
            List<SerializedProperty> graphEventParameters = new List<SerializedProperty>();
            for (int index = 0; index < eventList.arraySize; index++)
            {
                SerializedProperty eventProp = eventList.GetArrayElementAtIndex(index);
                graphEventNames.Add(eventProp.FindPropertyRelative("eventName").stringValue);
                graphEventParameters.Add(eventProp.FindPropertyRelative("parameters"));
            }

            // Event selector
            SerializedProperty eventNameProp = element.FindPropertyRelative("eventName");
            int selectionIndex = Mathf.Clamp(graphEventNames.IndexOf(eventNameProp.stringValue), 0, int.MaxValue);
            Rect eventSelectorRect = new Rect(rect.x, rect.y + EditorGUIUtility.standardVerticalSpacing, rect.width, EditorGUIUtility.singleLineHeight);
            selectionIndex = EditorGUI.Popup(eventSelectorRect, selectionIndex, graphEventNames.ToArray());
            eventNameProp.stringValue = graphEventNames[selectionIndex];

            // drawing parameters
            SerializedPropertyTree parameterList = element.FindPropertyRelative("parameters");
            float yPosition = eventSelectorRect.y + eventSelectorRect.height + EditorGUIUtility.standardVerticalSpacing;
            for (int index = 0; index < parameterList.arraySize; index++)
            {
                SerializedPropertyTree parameterProperty = parameterList.GetArrayElementAtIndex(index);
                GraphVariableEditor editor = LoadEditor(parameterProperty);
                float parameterHeight = VariableInspectorDrawFunctions.PlayerInspectorFNs.GetValueHeight(target, parameterProperty,"",editor, GraphVariable.RetrievalTypes.DefaultValue, true);
                //GUILayout.BeginArea();
                
                Rect drawArea = new Rect(rect.x, yPosition, rect.width, parameterHeight);
                VariableInspectorDrawFunctions.PlayerInspectorFNs.DrawValue(drawArea, target, "", parameterProperty, editor, GraphVariable.RetrievalTypes.DefaultValue, true);
                //GUILayout.EndArea();
                yPosition += parameterHeight + EditorGUIUtility.standardVerticalSpacing;
            }

        }

    

        public override void OnInspectorGUI()
        {
            Sync();
            //SyncVariables();
            SyncEvents();
            serializedObject.UpdateIfRequiredOrScript();

           
            SerializedPropertyTree soundGraphProp = serializedObjectTree.FindProperty("_soundGraph");
            SerializedPropertyTree dontDestroyOnLoadProp = serializedObjectTree.FindProperty("dontDestroyOnLoad");
            SerializedPropertyTree playOnAwakeProp = serializedObjectTree.FindProperty("playOnAwake");
            
            if (soundGraphProp.objectReferenceValue != null && GUILayout.Button("Edit"))
            {
                if ((target as SoundGraphPlayer) == null || (target as SoundGraphPlayer).runtimeGraphCopy == null)
                    SoundGraphEditorWindow.Open(soundGraphProp.objectReferenceValue as SoundGraph);
                else
                    SoundGraphEditorWindow.Open((target as SoundGraphPlayer).runtimeGraphCopy);

            }

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            LayersGUIUtilities.DrawRightAlignedCheckbox(dontDestroyOnLoadProp, new GUIContent("Don't Destroy on Load"));
            EditorGUI.EndDisabledGroup();

            if (target is SoundGraphPlayer)
            {
                LayersGUIUtilities.DrawRightAlignedCheckbox(playOnAwakeProp);

                serializedObject.ApplyModifiedProperties();
                SyncStartingEventParameters();
                serializedObject.UpdateIfRequiredOrScript();

                if (playOnAwakeProp.boolValue)
                    startingEventsList.DoLayoutList();
            }
        
            DrawVariables();
            DrawEvents();

        

            serializedObject.ApplyModifiedProperties();

            

            if (LayersSettings.GetOrCreateSettings().CheckIfSoundGraphNeedsRegen((target as PlayerBase).soundGraph)){
                EditorGUILayout.HelpBox("This Sound Graph needs to regenerate code", MessageType.Warning);
                if (GUILayout.Button("Regenerate code"))
                    RegenList.Generate();
            }
        
        }

        /// <summary>
        /// Run all synchronization tasks between the player and the graph
        /// </summary>
        private void Sync()
        {
            if (!Application.isPlaying)
            {
                CopyVariableValuesFromAssetGraph();
            }
        }


        private void DrawVariables()
        {
            SerializedPropertyTree varList = serializedObjectTree.FindProperty("localVariablesList");

            if (varList.arraySize == 0)
                return;

            SerializedProperty variablesExpanded = serializedObject.FindProperty("variablesExpanded");
            variablesExpanded.boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(variablesExpanded.boolValue, new GUIContent("Variables"));

            serializedObjectTree.ApplyModifiedProperties();


            if (variablesExpanded.boolValue)
            {

                for (int index = 0; index < varList.arraySize; index++)
                {
                    SerializedPropertyTree variableProp = varList.GetArrayElementAtIndex(index);
                    GraphVariableEditor editor = LoadEditor(variableProp);
                    if (editor == null)
                        continue;

                    EditorGUILayout.BeginHorizontal();

                    //turning off array type picker
                    if (editor is ArrayVariableEditor)
                        (editor as ArrayVariableEditor).showTypeSelector = false;

                    EditorGUI.BeginChangeCheck();


                    GraphVariable.RetrievalTypes drawStyle = connectedToRuntimeGraph ? GraphVariable.RetrievalTypes.ActualValue : GraphVariable.RetrievalTypes.DefaultValue;
                    Rect valueRect = EditorGUILayout.GetControlRect(false,
                        VariableInspectorDrawFunctions.PlayerInspectorFNs.GetValueHeight(target,variableProp, variableProp.FindPropertyRelative("name").stringValue,editor, drawStyle, true));
                    VariableInspectorDrawFunctions.PlayerInspectorFNs.DrawValue(valueRect, target, variableProp.FindPropertyRelative("name").stringValue, variableProp, editor, drawStyle, true);

                    bool needToIgnoreEdit = false;
                    if (editor is ArrayVariableEditor)
                        needToIgnoreEdit = (editor as ArrayVariableEditor).expansionChangedThisFrame;

                    //turning on array type picker (Just to clean up)
                    if (editor is ArrayVariableEditor)
                        (editor as ArrayVariableEditor).showTypeSelector = true;

                    variableProp.serializedObject.UpdateIfRequiredOrScript();
                    if (EditorGUI.EndChangeCheck() && !needToIgnoreEdit)
                    {
                        variableProp.FindPropertyRelative("synchronizeWithGraphVariable").boolValue = false;
                        variableProp.serializedObject.ApplyModifiedProperties();
                        //if (Application.isPlaying) //then need to write directly to graph
                            //WriteLocalVariableToUnderlyingGraph(variableProp.FindPropertyRelative("name").stringValue);
                    }
                    
                    // dropdown for modified properties. Only need to do this in edit mode
                    if (!Application.isPlaying && !variableProp.FindPropertyRelative("synchronizeWithGraphVariable").boolValue)
                    {
                        Rect dropdownRect = EditorGUILayout.GetControlRect(false, GUILayout.Width(15));
                        LayersGUIUtilities.DrawDropdown(dropdownRect, "", new string[] { "Reset to Default" }, true, (newValue) => {
                            variableProp.FindPropertyRelative("synchronizeWithGraphVariable").boolValue = true;
                            variableProp.FindPropertyRelative("synchronizeWithGraphVariable").serializedObject.ApplyModifiedProperties();
                        });
                    }

                    variableProp.serializedObject.ApplyModifiedProperties();
                    EditorGUILayout.EndHorizontal();
                }
            }
            
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawEvents()
        {
            SerializedPropertyTree eventList = serializedObjectTree.FindProperty("localEventsList");
            if (eventList.arraySize == 0)
                return;

            SerializedProperty eventsExpanded = serializedObject.FindProperty("eventsExpanded");
            eventsExpanded.boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(eventsExpanded.boolValue, new GUIContent("Events"));

            if (eventsExpanded.boolValue)
            {
                for (int index = 0; index < eventList.arraySize; index++)
                {
                    SerializedProperty eventProp = eventList.GetArrayElementAtIndex(index);
                    EditorGUILayout.PropertyField(eventProp.FindPropertyRelative("onGraphEventCalled"), new GUIContent(eventProp.FindPropertyRelative("eventName").stringValue));
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private GraphVariableEditor LoadEditor(SerializedProperty property)
        {
            GraphVariableEditor editor = null;
            string targetTypeName = property.FindPropertyRelative("typeName").stringValue;
            if (!path2VariableEditors.TryGetValue(property.propertyPath, out editor) || editor.handlesType.FullName != targetTypeName)
            {
                System.Type editorType = VariableInspectorUtility.GetEditorType(targetTypeName, VariableInspectorUtility.EditorFilter.Player);
                if (editorType != null)
                {
                    editor = (GraphVariableEditor)System.Activator.CreateInstance(editorType);


                    if (path2VariableEditors.ContainsKey(property.propertyPath))
                        path2VariableEditors[property.propertyPath] = editor;
                    else
                        path2VariableEditors.Add(property.propertyPath, editor);
                }
            }
            return editor;
        }


        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        /// <summary>
        /// Loads values from the asset graph, not overwriting modified values
        /// </summary>
        private void CopyVariableValuesFromAssetGraph()
        {
            typeof(SoundGraphPlayer).GetMethod("CopyVariableValuesFromAssetGraph", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(target, null);

        }

        private void SyncEvents()
        {
            typeof(PlayerBase).GetMethod("SyncEvents", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(target, null);
        }

        private void SyncStartingEventParameters()
        {
            if (target as SoundGraphPlayer != null)
                typeof(SoundGraphPlayer).GetMethod("SyncStartingEventParameters", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(target, null);
        }

        /*
        private void SyncVariables()
        {
            typeof(PlayerBase).GetMethod("SyncVariables", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(target, null);
        }

        private void SyncEvents()
        {
            typeof(PlayerBase).GetMethod("SyncEvents", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(target, null);
        }

        private void SyncStartingEventParameters()
        {
            if (target as SoundGraphPlayer != null)
            typeof(SoundGraphPlayer).GetMethod("SyncStartingEventParameters", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(target, null);
        }

        private void WriteLocalVariableToUnderlyingGraph(string varName)
        {
            if (target as SoundGraphPlayer != null)
            {
                System.Reflection.MethodInfo method = typeof(SoundGraphPlayer).GetMethod("WriteLocalVariableToUnderlyingGraph", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                method.Invoke(target, new object[] { varName });
            }
        }*/
    }
}
