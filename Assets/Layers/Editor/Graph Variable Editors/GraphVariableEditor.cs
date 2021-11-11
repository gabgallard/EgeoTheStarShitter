using System.Collections.Generic;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using UnityEditor;
using UnityEngine;
using ABXY.Layers.Editor.Node_Editors.Variables.Combine_and_Split;

namespace ABXY.Layers.Editor.Graph_Variable_Editors
{
    public abstract class GraphVariableEditor
    {

        public abstract System.Type handlesType {get;}

        public bool descendsFromUnityObject { 
            get
            {
                return typeof(Object).IsAssignableFrom(handlesType);
            } 
        }


        public abstract string GetPrettyTypeName();


        

    



        



        protected delegate object PortDefaultDrawFunction(object defaultInput);

        /// <summary>
        /// Used to draw ports with in-node values in combine/split nodes
        /// </summary>
        /// <param name="port"></param>
        /// <param name="drawDefaults"></param>
        protected void DrawPortWithDefaults(NodePort port, PortDefaultDrawFunction drawDefaults)
        {
            CombineNode node = port.node as CombineNode;
            if (node == null)
            {
                Debug.LogError("Cannot use DrawPortWithDefaults on non-combine node GUIs");
                return;
            }


            if (!port.IsConnected && drawDefaults != null)
            {
                object defaultValue = node.GetDefaultValue(port.fieldName, null);

                EditorGUI.BeginChangeCheck();

                defaultValue = drawDefaults.Invoke(defaultValue);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(port.node, port.fieldName + " changed");
                    node.SetDefaultValue(port.fieldName, defaultValue);
                }

            

                NodeEditorGUILayout.AddPortField(port);
            }
            else
                NodeEditorGUILayout.PortField(port);
        }


        protected delegate void DelayedDefaultDrawFunction(object defaultInput, System.Action<object> OnValueChange);
        protected void DrawPortWithDefaults(NodePort port, DelayedDefaultDrawFunction drawDefaults)
        {
            CombineNode node = port.node as CombineNode;
            if (node == null)
            {
                Debug.LogError("Cannot use DrawPortWithDefaults on non-combine node GUIs");
                return;
            }


            if (!port.IsConnected && drawDefaults != null)
            {
                object defaultValue = node.GetDefaultValue(port.fieldName, null);


                drawDefaults?.Invoke(defaultValue, (newValue) => {
                    Undo.RecordObject(port.node, port.fieldName + " changed");
                    node.SetDefaultValue(port.fieldName, newValue);
                });



                NodeEditorGUILayout.AddPortField(port);
            }
            else
                NodeEditorGUILayout.PortField(port);
        }


        protected void RecordUndo(Object owningObject, GraphVariableBase variable)
        {
            Undo.RecordObject(owningObject, variable.name + " changed");
        }
    }
}
