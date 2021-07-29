using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace ABXY.Layers.Editor.Node_Editors
{
    [CustomNodeEditor(typeof(MoveTowardsNode))]
    public class MoveTowardsNodeEditor : FlowNodeEditor
    {


        ///For flow mode
        TypedPortGUI targetvalueIn;
        TypedPortGUI targetvalueOut;

        // For variable mode
        TypedPortGUI targetVariablePort;



        private List<string> allowedTypes = new List<string>(new string[] { typeof(double).FullName, typeof(float).FullName, typeof(Vector3).FullName, typeof(Quaternion).FullName });
        private List<string> allowedPrettyNames = new List<string>();

        public override void OnCreate()
        {
            base.OnCreate();

            foreach (string allowedType in allowedTypes)
                allowedPrettyNames.Add(VariableInspectorUtility.GetPrettyName(allowedType));


            SerializedPropertyTree style = serializedObjectTree.FindProperty("style");

            // Setting intital port types
            string typeName = typeof(float).FullName;
            if (style.enumValueIndex == (int)MoveTowardsNode.Styles.Variable)
            {
                GraphVariable graphVariable = (target as FlowNode).soundGraph.GetGraphVariableByID(serializedObjectTree.FindProperty("variableID").stringValue);
                typeName = graphVariable == null ? typeName : graphVariable.typeName;
            }
            else
            {
                GraphVariable targetVariable = (target as MoveTowardsNode).targetValue;
                typeName = targetVariable.typeName;
            }

            targetVariablePort = new TypedPortGUI(serializedObjectTree.FindProperty("targetValue"), NodePort.IO.Input, typeName);
            targetvalueIn = new TypedPortGUI(serializedObjectTree.FindProperty("targetValue"), NodePort.IO.Input, typeName, showBackingValue: Node.ShowBackingValue.Never);
            targetvalueOut = new TypedPortGUI(serializedObjectTree.FindProperty("targetOutput"), NodePort.IO.Output, typeName, showBackingValue: Node.ShowBackingValue.Never);
        }


        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();

            SerializedPropertyTree maxDelta = serializedObjectTree.FindProperty("maxDelta");
            SerializedPropertyTree variableID = serializedObjectTree.FindProperty("variableID");
            SerializedPropertyTree style = serializedObjectTree.FindProperty("style");

            LayersGUIUtilities.DrawDropdown(layout.DrawLine(), new GUIContent("Output Mode"), style);

            if (style.enumValueIndex == (int)MoveTowardsNode.Styles.Variable)
            {


                GraphVariable graphVariable = (target as FlowNode).soundGraph.GetGraphVariableByID(variableID.stringValue);
                string typeName = graphVariable == null ? "" : graphVariable.typeName;

                LayersGUIUtilities.DrawVariableSelector(layout.DrawLine(),
                    variableID, (target as FlowNode).soundGraph, (variable) => { return allowedTypes.Contains(variable.typeName); });

                targetVariablePort.expectedType = typeName;
                targetVariablePort.Draw(layout.Draw(targetVariablePort.CalculateHeight("Target Value")), "Target Value", (target.graph as SoundGraph).isRunningSoundGraph);
            }
            else
            {
                GraphVariable targetVariable = (target as MoveTowardsNode).targetValue;
                GraphVariable targetOutput = (target as MoveTowardsNode).targetOutput;
                LayersGUIUtilities.DrawTypeSelector(layout.DrawLine(), "Type", targetVariable.typeName, allowedTypes, allowedPrettyNames, (newValue) => {
                    //targetVariable.typeName = newValue;
                    //targetOutput.typeName = newValue;
                    serializedObject.UpdateIfRequiredOrScript();
                    targetvalueIn.expectedType = newValue;
                    targetvalueOut.expectedType = newValue;
                    serializedObject.ApplyModifiedProperties();
                });  
                targetOutput.typeName = targetVariable.typeName;

                Rect portDrawRect = layout.DrawLine();
                targetvalueIn.Draw(portDrawRect, "Target Value", (target.graph as SoundGraph).isRunningSoundGraph);
                targetvalueOut.Draw(portDrawRect, "Output", (target.graph as SoundGraph).isRunningSoundGraph);
            }

            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), maxDelta);
                       
            serializedObject.ApplyModifiedProperties();
        }
    }
}