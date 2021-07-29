using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using UnityEditor;
/*
namespace ABXY.Layers.Editor.Node_Editors.Variables.Combine_and_Split
{
    [NodeEditor.CustomNodeEditor(typeof(CombineMidiData))]
    public class CombineMidiDataEditor : FlowNodeEditor
    {

        NodePort output;

        SerializedProperty noteNumber;

        SerializedProperty channelNumber;

        SerializedProperty velocity;
        NodePort velocityNodePort;

        public override void OnCreate()
        {
            base.OnCreate();
            output = target.GetOutputPort("output");
            noteNumber = serializedObject.FindProperty("noteNumber");
            channelNumber = serializedObject.FindProperty("channelNumber");
            velocity = serializedObject.FindProperty("velocity");
            velocityNodePort = target.GetInputPort("velocity");

        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.Update();

            float labelWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = 120;
            LayersGUIUtilities.DrawExpandableProperty(noteNumber);
            //NodeEditorGUILayout.PropertyAndPortPair(serializedObject, noteNumber, output);
            LayersGUIUtilities.DrawExpandableProperty(channelNumber);

            EditorGUIUtility.labelWidth = 60;
            LayersGUIUtilities.DrawExpandableProperty(velocity);
            EditorGUIUtility.labelWidth = labelWidth;
            LayersGUIUtilities.DrawExpandableProperty(output,serializedObject);

            serializedObject.ApplyModifiedProperties();
        }

        protected override bool CanExpand()
        {
            return true;
        }

        public override int GetWidth()
        {
            return 250;
        }
    }
}*/
