using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Math_Operations;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editors.Math_Operations
{
    [NodeEditor.CustomNodeEditor(typeof(RepeatNode))]
    public class RepeatNodeEditor : FlowNodeEditor
    {
        NodePort floatValuePort;
        NodePort floatResultPort;
        NodePort floatLength;
        NodePort intValuePort;
        NodePort intResultPort;
        NodePort intLengthPort;

        public override void OnCreate()
        {
            base.OnCreate();
            
            floatValuePort = target.GetInputPort("floatValue");
            floatResultPort = target.GetOutputPort("floatResult");
            floatLength = target.GetInputPort("floatLength");
            intValuePort = target.GetInputPort("intValue");
            intResultPort = target.GetOutputPort("intResult");
            intLengthPort = target.GetInputPort("intLength");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();

            SerializedPropertyTree numericTypeProp = serializedObject.FindProperty("numericType");
            SerializedPropertyTree intValueProp = serializedObject.FindProperty("intValue");
            SerializedPropertyTree intLengthProp = serializedObject.FindProperty("intLength");
            SerializedPropertyTree intResultProp = serializedObject.FindProperty("intResult");
            SerializedPropertyTree floatValueProp = serializedObject.FindProperty("floatValue");
            SerializedPropertyTree floatLengthProp = serializedObject.FindProperty("floatLength");



            if ((RepeatNode.numericTypes)numericTypeProp.enumValueIndex  == RepeatNode.numericTypes.Integer)
            {
                LayersGUIUtilities.FastPropertyField(layout.DrawLine(), new GUIContent("Type"),numericTypeProp);

                NodeEditorGUIDraw.PropertyField(layout.DrawLine(), intValueProp, new GUIContent("Value"));
                NodeEditorGUIDraw.PropertyField(layout.DrawLine(), intLengthProp, new GUIContent("Length"));

                if (floatValuePort.IsConnected)
                    floatValuePort.ClearConnections();

                if (floatResultPort.IsConnected)
                    floatResultPort.ClearConnections();

                if (floatLength.IsConnected)
                    floatLength.ClearConnections();

                NodeEditorGUIDraw.PropertyField(layout.DrawLine(), intResultProp);
            }
            else
            {
                LayersGUIUtilities.FastPropertyField(layout.DrawLine(), numericTypeProp);
                NodeEditorGUIDraw.PropertyField(layout.DrawLine(), floatValueProp, new GUIContent("Value"));
                NodeEditorGUIDraw.PropertyField(layout.DrawLine(), floatLengthProp, new GUIContent("Length"));

                if (intValuePort.IsConnected)
                    intValuePort.ClearConnections();

                if (intResultPort.IsConnected)
                    intResultPort.ClearConnections();

                if (intLengthPort.IsConnected)
                    intLengthPort.ClearConnections();

                NodeEditorGUIDraw.PortField(layout.DrawLine(), floatResultPort);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
