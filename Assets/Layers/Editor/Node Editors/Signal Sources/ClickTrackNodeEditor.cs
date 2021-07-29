using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Signal_Sources;
using UnityEditor;

namespace ABXY.Layers.Editor.Node_Editors.Signal_Sources
{
    [CustomNodeEditor(typeof(ClickTrackNode))]
    public class ClickTrackNodeEditor : FlowNodeEditor
    {
        
        NodePort enterPort;
        NodePort clickTrackFinishedPort;
        NodePort onBarPort;
        NodePort onBeatPort;
        NodePort onClickPort;
        public override void OnCreate()
        {
            base.OnCreate();
            enterPort = target.GetInputPort("enter");
            clickTrackFinishedPort = target.GetOutputPort("ClickTrackFinished");
            onBarPort = target.GetOutputPort("onBar");
            onBeatPort = target.GetOutputPort("onBeat");
            onClickPort = target.GetOutputPort("onClick");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();

            SerializedPropertyTree bpmProp = serializedObject.FindProperty("BPM");
            SerializedPropertyTree numBarsProp = serializedObject.FindProperty("numberOfBars");
            SerializedPropertyTree beatsPerBarProp = serializedObject.FindProperty("beatsPerBar");
            SerializedPropertyTree clicksPerBeatProp = serializedObject.FindProperty("clicksPerBeat");
            SerializedPropertyTree playClick = serializedObject.FindProperty("playClick");

            LayersGUIUtilities.DrawExpandableProperty(layout, enterPort, clickTrackFinishedPort, serializedObject);

            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100;
            LayersGUIUtilities.DrawExpandableProperty(layout, bpmProp);
            LayersGUIUtilities.DrawExpandableProperty(layout, numBarsProp);
            LayersGUIUtilities.DrawExpandableProperty(layout, beatsPerBarProp);
            LayersGUIUtilities.DrawExpandableProperty(layout, clicksPerBeatProp);

            LayersGUIUtilities.DrawExpandableProperty(serializedObject, () => {
                LayersGUIUtilities.DrawRightAlignedCheckbox(layout.DrawLine(), playClick);
            });

            EditorGUIUtility.labelWidth = labelWidth;

            LayersGUIUtilities.DrawExpandableProperty(layout, onBarPort, serializedObject);
            LayersGUIUtilities.DrawExpandableProperty(layout, onBeatPort, serializedObject);
            LayersGUIUtilities.DrawExpandableProperty(layout, onClickPort, serializedObject);
            serializedObject.ApplyModifiedProperties();
        }

        protected override bool CanExpand()
        {
            return true;
        }

    
    }
}
