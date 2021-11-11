using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Math_Operations;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editors.Math_Operations
{
    [NodeEditor.CustomNodeEditor(typeof(RandomNumberNode))]
    public class RandomNumberNodeEditor : FlowNodeEditor
    {

        //SerializedProperty randomNumberTypeProp = null;
        NodePort getNewNumberPort = null;
        NodePort changedPort = null;
        NodePort valuePort = null;
        //SerializedProperty intFromProp = null;
        //SerializedProperty intToProp = null;
        NodePort floatFromPort = null;
        NodePort floatToPort = null;
        //SerializedProperty floatFromProp = null;
        //SerializedProperty floatToProp = null;
        NodePort intFromPort = null;
        NodePort intToPort = null;

        public override void OnCreate()
        {
            base.OnCreate();
            getNewNumberPort = target.GetInputPort("getNewNumber");
            changedPort = target.GetOutputPort("changed");
            valuePort = target.GetOutputPort("value");


            floatFromPort = target.GetInputPort("floatFrom");
            floatToPort = target.GetInputPort("floatTo");
            intFromPort = target.GetInputPort("intFrom");
            intToPort = target.GetInputPort("intTo");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();


            SerializedPropertyTree randomNumberTypeProp = serializedObject.FindProperty("randomNumberType");
            SerializedPropertyTree intFromProp = serializedObject.FindProperty("intFrom");
            SerializedPropertyTree intToProp = serializedObject.FindProperty("intTo");
            SerializedPropertyTree floatFromProp = serializedObject.FindProperty("floatFrom");
            SerializedPropertyTree floatToProp = serializedObject.FindProperty("floatTo");


            System.Type expectedValueType = GetRNGType((RandomNumberNode.RandomNumberTypes)randomNumberTypeProp.enumValueIndex);


            NodeEditorGUIDraw.PortPair(layout.DrawLine(), new GUIContent("Get Number"), 
                getNewNumberPort, new GUIContent("On Changed"), changedPort, serializedObjectTree);

            serializedObject.ApplyModifiedProperties();
            if (valuePort != null && valuePort.ValueType != expectedValueType)
            {
                target.RemoveDynamicPort(valuePort);
                valuePort = null;
            }

            if (valuePort == null)
                valuePort = target.AddDynamicOutput(expectedValueType, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited, "value");

            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();

            LayersGUIUtilities.FastPropertyField(layout.DrawLine(), new GUIContent("Type"), randomNumberTypeProp);

            switch ((RandomNumberNode.RandomNumberTypes)randomNumberTypeProp.enumValueIndex)
            {
                case RandomNumberNode.RandomNumberTypes.Integer:
                    NodeEditorGUIDraw.PropertyField(layout.DrawLine(),intFromProp,new GUIContent("From"));
                    NodeEditorGUIDraw.PropertyField(layout.DrawLine(), intToProp, new GUIContent("To"));
                    if (floatFromPort.ConnectionCount != 0)
                        floatFromPort.ClearConnections();
                    if (floatToPort.ConnectionCount != 0)
                        floatToPort.ClearConnections();
                    break;
                case RandomNumberNode.RandomNumberTypes.Float:
                    NodeEditorGUIDraw.PropertyField(layout.DrawLine(), floatFromProp, new GUIContent("From"));
                    NodeEditorGUIDraw.PropertyField(layout.DrawLine(), floatToProp, new GUIContent("To"));
                    if (intFromPort.ConnectionCount != 0)
                        intFromPort.ClearConnections();
                    if (intToPort.ConnectionCount != 0)
                        intToPort.ClearConnections();
                    break;
            }

            NodeEditorGUIDraw.PortField(layout.DrawLine(), valuePort, serializedObjectTree);


            serializedObject.ApplyModifiedProperties();
        }

        private System.Type GetRNGType(RandomNumberNode.RandomNumberTypes rngType)
        {
            switch (rngType)
            {
                case RandomNumberNode.RandomNumberTypes.Integer:
                    return typeof(int);
                case RandomNumberNode.RandomNumberTypes.Float:
                    return typeof(float);
            }
            return null;
        }
    }
}
