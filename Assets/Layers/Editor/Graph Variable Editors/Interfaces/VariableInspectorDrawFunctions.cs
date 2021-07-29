using ABXY.Layers.Editor;
using ABXY.Layers.Editor.Graph_Variable_Editors;
using ABXY.Layers.Editor.Node_Editors.Variables.Combine_and_Split;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VariableInspectorDrawFunctions
{
    public static class InputNodeFNs
    {

        public static void DrawValue(Rect position, Object target, string label, SerializedPropertyTree property, GraphVariableEditor editor, GraphVariable.RetrievalTypes retrieval, bool canAccessSceneAssets)
        {
            if (editor is InputNodeInspector)
            {
                InputNodeInspector inspector = editor as InputNodeInspector;

                GraphVariableBase variable = SerializedPropertyUtils.GetPropertyObject(property) as GraphVariableBase;


                CommonFunctions.InitializeVariableIfNeeded(variable, property, inspector.descendsFromUnityObject, inspector.GetDefaultValue);



                VariableEdit edit = new VariableEdit(target, variable, property, retrieval == GraphVariable.RetrievalTypes.DefaultValue, canAccessSceneAssets);
                inspector.DrawInputNodeValue(position, label, edit);
            }
        }


        public static float GetValueHeight(Object target, SerializedPropertyTree property, string label, GraphVariableEditor editor, GraphVariable.RetrievalTypes retrieval, bool canAccessSceneAssets)
        {
            if (editor is InputNodeInspector)
            {
                InputNodeInspector inspector = editor as InputNodeInspector;
                GraphVariableBase variable = SerializedPropertyUtils.GetPropertyObject(property) as GraphVariableBase;
                VariableEdit edit = new VariableEdit(target, variable, property, retrieval == GraphVariable.RetrievalTypes.DefaultValue, canAccessSceneAssets);
                return inspector.CalculateInputNodeValueHeight(edit, label);
            }
            else
                return 0;
        }


        public static void DrawActualValueReadonly(Rect position, string label, Object target, SerializedPropertyTree property, GraphVariableEditor editor)
        {
            if (editor is InputNodeInspector)
            {
                InputNodeInspector inspector = editor as InputNodeInspector;

                GraphVariableBase graphVariable = SerializedPropertyUtils.GetPropertyObject(property) as GraphVariableBase;
                System.Type variableType = ReflectionUtils.FindType(property.FindPropertyRelative("typeName").stringValue);

                CommonFunctions.InitializeVariableIfNeeded(graphVariable, property, inspector.descendsFromUnityObject, inspector.GetDefaultValue);


                float defaultValueHeight = EditorGUIUtility.singleLineHeight;
                Rect labelRet = new Rect(position.x, position.y, position.width - EditorGUIUtility.singleLineHeight, defaultValueHeight);
                //OnDrawActualValue(contentRect, "Current Value", target, property, graphVariable);

                LayersGUIUtilities.BeginNewLabelWidth(100);
                Rect contentRect = EditorGUI.PrefixLabel(labelRet, new GUIContent(label), graphVariable.synchronizeWithGraphVariable ? EditorStyles.label : EditorStyles.boldLabel);
                LayersGUIUtilities.EndNewLabelWidth();

                object currentActualValue = graphVariable.Value();
                EditorGUI.SelectableLabel(contentRect, currentActualValue != null ? currentActualValue.ToString() : "null", LayersGUIUtilities.rightAlignedCenteredText);

                if (graphVariable.expose != GraphVariableBase.ExposureTypes.AsInput)
                {
                    Rect dropdownRect = new Rect(position.x + position.width - EditorGUIUtility.singleLineHeight, position.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
                    LayersGUIUtilities.DrawThreeDotDropdown(dropdownRect, "", new string[] { "Reset" }, true, (selection) =>
                    {
                        graphVariable.ResetToDefaultValue();
                    });
                }

            }
        }

        public static void DrawReadonly(Rect position, string message = "Only writeable through API")
        {
            EditorGUI.LabelField(position, message);
        }

        public static float ReadOnlyHeight()
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }

    public static class PlayerInspectorFNs
    {

        public static void DrawValue(Rect position, Object target, string label, SerializedPropertyTree property, GraphVariableEditor editor, GraphVariable.RetrievalTypes retrieval, bool canAccessSceneAssets)
        {
            if (editor is PlayerInspector)
            {
                PlayerInspector inspector = editor as PlayerInspector;
                GraphVariable variable = SerializedPropertyUtils.GetPropertyObject(property) as GraphVariable;
                System.Type variableType = ReflectionUtils.FindType(property.FindPropertyRelative("typeName").stringValue);


                CommonFunctions.InitializeVariableIfNeeded(variable, property, inspector.descendsFromUnityObject, inspector.GetDefaultValue);

                VariableEdit edit = new VariableEdit(target, variable, property, retrieval == GraphVariable.RetrievalTypes.DefaultValue, canAccessSceneAssets);
                inspector.DrawInPlayerInspector(position, label, edit);
            }
        }


        public static float GetValueHeight(Object target, SerializedPropertyTree property, string label, GraphVariableEditor editor, GraphVariable.RetrievalTypes retrieval, bool canAccessSceneAssets)
        {
            if (editor is PlayerInspector)
            {
                PlayerInspector inspector = editor as PlayerInspector;
                GraphVariableBase variable = SerializedPropertyUtils.GetPropertyObject(property) as GraphVariableBase;
                VariableEdit edit = new VariableEdit(target, variable, property, retrieval == GraphVariable.RetrievalTypes.DefaultValue, canAccessSceneAssets);
                return inspector.CalculateHeightInPlayerInspector(edit, label);
            }
            else
                return 0;
        }

        public static void DrawReadonly(Rect position, string message = "Only writeable through API")
        {
            EditorGUI.LabelField(position, message);
        }

        public static float ReadOnlyHeight()
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }

    public static class CombinableFNs
    {
        public static List<PortDefinition> GetCombinePorts(CombineNode target, GraphVariableEditor editor, CombineNodeEditor nodeEditor)
        {
            if (editor is CombinableInspector)
            {
                CombinableInspector inspector = editor as CombinableInspector;
                return inspector.GetCombinePorts(new CombineSplitData(target, nodeEditor));
            }
            return new List<PortDefinition>();
        }

        public static void DrawCombineGUI(CombineNode target, GraphVariableEditor editor, CombineNodeEditor nodeEditor)
        {
            if (editor is CombinableInspector)
            {
                CombinableInspector inspector = editor as CombinableInspector;
                inspector.DrawCombineGUI(new CombineSplitData(target, nodeEditor));
            }
        }

        public static int GetCombineNodeWidth(GraphVariableEditor editor)
        {
            if (editor is CombinableInspector)
            {
                CombinableInspector inspector = editor as CombinableInspector;
                return inspector.GetCombineNodeWidth();
            }
            return 208;
        }
    }

    public static class SplittableFNs
    {
        public static List<PortDefinition> GetSplitPorts(SplitNode target, GraphVariableEditor variableEditor, SplitNodeEditor nodeEditor)
        {
            if (variableEditor is SplittableInspector)
            {
                SplittableInspector inspector = variableEditor as SplittableInspector;

                return inspector.GetSplitPorts(new CombineSplitData(target, nodeEditor));
            }
            return new List<PortDefinition>();
        }

        public static void DrawSplitGUI(SplitNode target, GraphVariableEditor editor, SplitNodeEditor nodeEditor)
        {
            if (editor is SplittableInspector)
            {
                SplittableInspector inspector = editor as SplittableInspector;
                inspector.DrawSplitGUI(new CombineSplitData(target, nodeEditor));
            }
        }

        public static int GetSplitNodeWidth(GraphVariableEditor editor)
        {
            if (editor is SplittableInspector)
            {
                SplittableInspector inspector = editor as SplittableInspector;
                return inspector.GetSplitNodeWidth();
            }
            return 208;
        }
    }


    public static class CommonFunctions
    {
        public static void InitializeVariableIfNeeded(GraphVariableBase variable, SerializedPropertyTree property, bool descendsFromUnityObject, System.Func<object> getDefaultValue)
        {
            System.Type variableType = ReflectionUtils.FindType(property.FindPropertyRelative("typeName").stringValue);

            // Initializing default value
            if (descendsFromUnityObject)
            {
                if (variable.defaultUnityObjectValue == null || variableType != variable.defaultUnityObjectValue.GetType())
                {
                    variable.defaultUnityObjectValue = (Object)getDefaultValue();
                    property.serializedObject.UpdateIfRequiredOrScript();
                }
            }
            else
            {
                if (variable.defaultObjectValue == null || variableType != variable.defaultObjectValue.GetType())
                {
                    variable.defaultObjectValue = getDefaultValue();
                    property.FindPropertyRelative("defaultSerializedObjectValue").stringValue = "";//hackety hack
                }
            }


            // Initializing actual value
            if (descendsFromUnityObject)
            {
                if (variable.unityObjectValue == null || !variableType.IsAssignableFrom(variable.unityObjectValue.GetType()))
                {
                    variable.unityObjectValue = (Object)getDefaultValue();
                    property.serializedObject.UpdateIfRequiredOrScript();
                }
            }
            else
            {
                if (variable.objectValue == null || !variableType.IsAssignableFrom(variable.objectValue.GetType()))
                {
                    variable.objectValue = getDefaultValue();
                    property.FindPropertyRelative("defaultSerializedObjectValue").stringValue = "";//hackety hack
                }
            }

        }
    }
}

public struct VariableEdit
{
    private Object undoTarget;
    private GraphVariableBase variable;
    public bool settingDefaultValue { get; private set; }
    public bool canAccessSceneAssets { get; private set; }


    internal SerializedPropertyTree graphVariableProperty { get; private set; }

    public VariableEdit(Object undoTarget, GraphVariableBase variable, SerializedPropertyTree graphVariableProperty, bool settingDefaultValue, bool canAccessSceneAssets)
    {
        this.undoTarget = undoTarget ?? throw new System.ArgumentNullException(nameof(undoTarget));
        this.variable = variable ?? throw new System.ArgumentNullException(nameof(variable));
        this.settingDefaultValue = settingDefaultValue;
        this.graphVariableProperty = graphVariableProperty;
        this.canAccessSceneAssets = canAccessSceneAssets;
    }

    public object objectValue
    {
        get
        {
            if (settingDefaultValue)
                return variable.defaultObjectValue;
            else
                return variable.objectValue;
        }
        set
        {
            object lastValue = settingDefaultValue ? variable.defaultObjectValue : objectValue;

            bool changed = false;
            
            if (value == null && value != lastValue)
            {
                changed = true;
            }
            else if (value != null && lastValue == null)
            {
                changed = true;
            }
            else if (value != null && lastValue != null)
            {
                if (value.GetType().IsByRef && value != lastValue)
                    changed = true;
                else if(!lastValue.Equals(value))
                    changed = true;
            }

            if (changed)
                Undo.RecordObject(undoTarget, variable.name + " changed");

            if (settingDefaultValue)
                variable.defaultObjectValue = value;
            else
                variable.objectValue = value;

        }
    }

    public Object unityObjectValue
    {
        get
        {
            if (settingDefaultValue)
                return variable.defaultUnityObjectValue;
            else
                return variable.unityObjectValue;
        }
        set
        {
            object lastValue = settingDefaultValue ? variable.defaultUnityObjectValue : variable.unityObjectValue;

            bool changed = false;
#pragma warning disable CS0253
            if (value == null && value != lastValue)
            {
                changed = true;
            }
            else if (value != null && lastValue == null)
            {
                changed = true;
            }
            else if (value != null && lastValue != null)
            {
                if (value.GetType().IsByRef && value != lastValue)
                    changed = true;
                else if (!lastValue.Equals(value))
                    changed = true;
            }
#pragma warning restore CS0253
            if (changed)
                Undo.RecordObject(undoTarget, variable.name + " changed");

            if (settingDefaultValue)
                variable.defaultUnityObjectValue = value;
            else
                variable.unityObjectValue = value;
        }
    }


}