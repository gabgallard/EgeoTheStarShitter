using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Variables;
using UnityEditor;
using ABXY.Layers.Runtime.Nodes;

namespace ABXY.Layers.Editor.Node_Editors.Variables
{
    [CustomNodeEditor(typeof(WriteNode))]
    public class WriteNodeEditor : FlowNodeEditor
    {

        SerializedProperty variableName;

        public override void OnCreate()
        {
            base.OnCreate();
            variableName = serializedObject.FindProperty("graphVariableID");
        }

        //TODO: REwrite this node
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();


            NodeEditorGUIDraw.PortPair(layout.DrawLine(), target.GetInputPort("startWrite"), target.GetOutputPort("writeEnded"));

            LayersGUIUtilities.DrawVariableSelector(layout.DrawLine(), variableName, (target as FlowNode).soundGraph);
            


            serializedObject.ApplyModifiedProperties();
            DoPort();

        }

        private void DoPort()
        {
            GraphVariable currentVariable = ((target as WriteNode).graph as SoundGraph).GetGraphVariableByID(variableName.stringValue);
            if (currentVariable == null)
            {
                if (target.Inputs.Where(x => x.fieldName == "Input").Count() != 0)
                    target.RemoveDynamicPort("Input");
            }
            else
            {
                System.Type variableType = currentVariable.GetVariableType();
                NodePort inputPort = target.GetInputPort("Input");
                if (inputPort == null)
                    inputPort = target.AddDynamicInput(variableType, Node.ConnectionType.Override, Node.TypeConstraint.Inherited, "Input");
                if (inputPort.ValueType != variableType)
                {
                    target.RemoveDynamicPort(inputPort);
                    inputPort = target.AddDynamicInput(variableType, Node.ConnectionType.Override, Node.TypeConstraint.Inherited, "Input");
                }

                NodeEditorGUIDraw.AddPortToRect(layout.LastRect(), inputPort);
            }

        }

        public override int GetWidth()
        {
            return 170;
        }
    }
}
