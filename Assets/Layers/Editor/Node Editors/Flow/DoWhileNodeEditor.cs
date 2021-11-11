using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Flow;
using UnityEditor;

namespace ABXY.Layers.Editor.Node_Editors.Flow
{
    [NodeEditor.CustomNodeEditor(typeof(DoWhile))]
    public class DoWhileNodeEditor : FlowNodeEditor
    {
        //SerializedProperty useCustomlogicProp;
        NodePort continueLoopPort;
        NodePort enterPort;
        NodePort conditionReachedPort;
        NodePort conditionPort;
        //SerializedProperty maxIterationCountProperty;
        NodePort resetIterationsPort;
        NodePort indexPort;

        public override void OnCreate()
        {
            base.OnCreate();
            continueLoopPort = target.GetOutputPort("continueLoop");
            enterPort = target.GetInputPort("enter");
            conditionReachedPort = target.GetOutputPort("conditionReached");
            conditionPort = target.GetInputPort("condition");
            resetIterationsPort = target.GetInputPort("resetIterations");
            indexPort = target.GetOutputPort("index");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();

            SerializedPropertyTree useCustomLogic = serializedObject.FindProperty("useCustomLogic");

            NodeEditorGUIDraw.PortPair(layout.DrawLine(), enterPort, continueLoopPort, serializedObjectTree);

            LayersGUIUtilities.DrawExpandableProperty(layout, conditionReachedPort,serializedObject);

            float labelWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = 206;
            LayersGUIUtilities.DrawExpandableProperty(layout,useCustomLogic);

            if (useCustomLogic.boolValue)
            {
                SetupCustomLogicPorts();
                LayersGUIUtilities.DrawExpandableProperty(layout, conditionPort,serializedObject);
            }
            else
            {
                EditorGUIUtility.labelWidth = 130;
                SetupIteratorLogicPorts();
                LayersGUIUtilities.DrawExpandableProperty(layout, serializedObject.FindProperty("maxIterationCount"));

            }

            LayersGUIUtilities.DrawExpandableProperty(layout,resetIterationsPort, indexPort,serializedObject);
            serializedObject.ApplyModifiedProperties();



            EditorGUIUtility.labelWidth = labelWidth;
        }

        protected override bool CanExpand()
        {
            return true;
        }

        private void SetupCustomLogicPorts()
        {
            serializedObject.ApplyModifiedProperties();
            if (conditionPort == null)
                conditionPort = target.AddDynamicInput(typeof(bool), Node.ConnectionType.Override, Node.TypeConstraint.Strict, "condition");
            serializedObject.UpdateIfRequiredOrScript();
        }

        private void SetupIteratorLogicPorts()
        {
            serializedObject.ApplyModifiedProperties();
            if (conditionPort != null)
                target.RemoveDynamicPort(conditionPort);
            serializedObject.UpdateIfRequiredOrScript();
        }

        public override int GetWidth()
        {
            return 250;
        }
    }
}
