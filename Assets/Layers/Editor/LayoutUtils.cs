using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor
{
    public static class LayoutUtils
    {
        public static void ReversedPortField(GUIContent label, NodePort port, params GUILayoutOption[] options)
        {
            if (port == null) return;
            if (options == null) options = new GUILayoutOption[] { GUILayout.MinWidth(30) };
            Vector2 position = Vector3.zero;
            GUIContent content = label != null ? label : new GUIContent(ObjectNames.NicifyVariableName(port.fieldName));
            
            // If property is an input, display a regular property field and put a port handle on the left side
            if (port.direction == NodePort.IO.Output)
            {
                // Display a label
                EditorGUILayout.LabelField(content, options);

                Rect rect = GUILayoutUtility.GetLastRect();
                position = rect.position - new Vector2(16, 0);
            }
            // If property is an output, display a text label and put a port handle on the right side
            else if (port.direction == NodePort.IO.Input)
            {
                // Display a label
                EditorGUILayout.LabelField(content, NodeEditorResources.OutputPort, options);

                Rect rect = GUILayoutUtility.GetLastRect();
                position = rect.position + new Vector2(rect.width, 0);
            }
            NodeEditorGUILayout.PortField(position, port);
        }
    }
}