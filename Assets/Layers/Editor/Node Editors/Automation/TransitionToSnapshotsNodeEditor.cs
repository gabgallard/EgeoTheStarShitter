using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Automation;
using UnityEditor;
using UnityEngine.Audio;

namespace ABXY.Layers.Editor.Node_Editors.Automation
{
    [NodeEditor.CustomNodeEditor(typeof(TransitionToSnapshotsNode))]
    public class TransitionToSnapshotsNodeEditor : FlowNodeEditor
    {

        NodePort startTransitionNode;
        NodePort endTransitionNode;
        /*SerializedProperty snapshotsProp;
        SerializedProperty weightsProp;
        SerializedProperty audioMixerProp;
        SerializedProperty timeToReachProp;*/

        public override void OnCreate()
        {
            base.OnCreate();
            startTransitionNode = target.GetInputPort("startTransition");
            endTransitionNode = target.GetOutputPort("endTransition");
        }


        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();
            NodeEditorGUIDraw.PortPair(layout.DrawLine(), startTransitionNode, endTransitionNode);
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), serializedObject.FindProperty("snapshots"));
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), serializedObject.FindProperty("weights"));

            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), serializedObject.FindProperty("audioMixer"));
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), serializedObject.FindProperty("timeToReach"));

            AudioMixerSnapshot[] snapshots = target.GetInputValue<AudioMixerSnapshot[]>("snapshots");
            float numberSnapshots = snapshots != null ? snapshots.Length : 0;
            float[] weights = target.GetInputValue<float[]>("weights");
            float numberWeights = weights != null ? weights.Length : 0;
            if (numberWeights != numberSnapshots)
                EditorGUI.HelpBox(layout.DrawLine(), "The number of snapshots and the number of weights must be equal!", MessageType.Error);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
