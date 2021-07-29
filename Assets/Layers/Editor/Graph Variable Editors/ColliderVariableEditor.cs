using ABXY.Layers.Editor.Node_Editors.Variables.Combine_and_Split;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace ABXY.Layers.Editor.Graph_Variable_Editors
{
    public class ColliderVariableEditor : GraphVariableEditor, InputNodeInspector, PlayerInspector, SplittableInspector
    {
        public override Type handlesType => typeof(Collider);

        public object GetDefaultValue()
        {
            return new Collider(); ;
        }

        public override string GetPrettyTypeName()
        {
            return "Collider";
        }

        /*protected override void DoDrawDefaultValueInPlayerInspector(UnityEngine.Object target, string label, SerializedProperty property, GraphVariableBase graphVariable)
        {
            property.FindPropertyRelative("defaultUnityObjectValue").objectReferenceValue = graphVariable.defaultUnityObjectValue = EditorGUILayout.ObjectField(
                new GUIContent(graphVariable.name), property.FindPropertyRelative("defaultUnityObjectValue").objectReferenceValue, typeof(Collider), true);
        }*/

        //Input 

        public void DrawInputNodeValue(Rect position, string label, VariableEdit variable)
        {
            VariableInspectorDrawFunctions.InputNodeFNs.DrawReadonly(position, "Only assignable in Player");
        }

        public float CalculateInputNodeValueHeight(VariableEdit variable, string label)
        {
            return VariableInspectorDrawFunctions.InputNodeFNs.ReadOnlyHeight();
        }


        // Player
        public void DrawInPlayerInspector(Rect position, string label, VariableEdit variable)
        {
            variable.unityObjectValue = EditorGUI.ObjectField(position, label, variable.unityObjectValue, typeof(Collider), true);
        }

        public float CalculateHeightInPlayerInspector(VariableEdit variable, string label)
        {
            return EditorGUIUtility.singleLineHeight;
        }



        public List<PortDefinition> GetSplitPorts(CombineSplitData data)
        {
            List<PortDefinition> ports = new List<PortDefinition>();
            ports.Add(new PortDefinition("collider", typeof(Collider), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("attachedRigidbody", typeof(Rigidbody), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("bounds", typeof(Bounds), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("contactOffset", typeof(float), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("enabled", typeof(bool), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("isTrigger", typeof(bool), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("material", typeof(PhysicMaterial), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("sharedMaterial", typeof(PhysicMaterial), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            return ports;
        }

        public void DrawSplitGUI(CombineSplitData data)
        {
            LayersGUIUtilities.DrawExpandableProperty(data.GetInputPort("collider"), data.GetOutputPort("attachedRigidbody"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("bounds"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("contactOffset"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("enabled"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("isTrigger"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("material"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("sharedMaterial"), data.nodeSerializedObject);
        }

        public int GetSplitNodeWidth()
        {
            return 260;
        }

        
    }
}