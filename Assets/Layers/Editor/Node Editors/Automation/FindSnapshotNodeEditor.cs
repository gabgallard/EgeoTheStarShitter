using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Automation;
using UnityEditor;

namespace ABXY.Layers.Editor.Node_Editors.Automation
{
    [NodeEditor.CustomNodeEditor(typeof(FindSnapshotNode))]
    public class FindSnapshotNodeEditor : FlowNodeEditor
    {
        //private SerializedProperty snapshotNameProperty;
        //private SerializedProperty audioMixerProperty;
        private NodePort snapshotPort;

        public override void OnCreate()
        {
            base.OnCreate();
            //snapshotNameProperty = serializedObject.FindProperty("snapshotName");
            //audioMixerProperty = serializedObject.FindProperty("audioMixer");
            snapshotPort = target.GetOutputPort("snapshot");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), serializedObject.FindProperty("snapshotName"));
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), serializedObject.FindProperty("audioMixer"));
            NodeEditorGUIDraw.PortField(layout.DrawLine(),snapshotPort, serializedObjectTree);
            serializedObject.ApplyModifiedProperties();
        }


    }
}
