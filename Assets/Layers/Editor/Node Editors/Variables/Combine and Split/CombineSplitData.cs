using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editors.Variables.Combine_and_Split
{
    public struct CombineSplitData
    {
        private CombineSplitBase node;
        private CombineSplitEditorBase editor;
        public Dictionary<string, string> arbitraryStrings { get { return node.arbitraryStrings; } }

        public SerializedObjectTree nodeSerializedObject { get { return editor.serializedObjectTree; } }

        public bool isRunningSoundgraph { get; private set; }

        public CombineSplitData(CombineSplitBase node, CombineSplitEditorBase editor)
        {
            this.node = node ?? throw new ArgumentNullException(nameof(node));
            this.editor = editor ?? throw new ArgumentNullException(nameof(editor));
            isRunningSoundgraph = node.soundGraph.isRunningSoundGraph;
        }

        public List<GraphEvent.EventParameterDef> GetIncomingParameterDefsOnPort(string portName)
        {
            return node.GetIncomingEventParameterDefsOnPort(portName, new List<Node>());
        }

        public NodePort GetOutputPort(string fieldName)
        {
            return node.GetOutputPort(fieldName);
        }

        public NodePort GetInputPort(string fieldName)
        {
            return node.GetInputPort(fieldName);
        }

        public NodePort GetPort(string fieldName)
        {
            return node.GetPort(fieldName);
        }

        public bool HasPort(string fieldName)
        {
            return node.HasPort(fieldName);
        }

        public T GetInputValue<T>(string fieldName, T fallback = default(T))
        {
            return node.GetInputValue<T>(fieldName, fallback);
        }

        public object GetInputValue(string fieldName, object fallback = null)
        {
            return node.GetInputValue(fieldName, fallback);
        }

        public T[] GetInputValues<T>(string fieldName, params T[] fallback)
        {
            return node.GetInputValues<T>(fieldName, fallback);
        }


        public void ReloadPortsImmediately()
        {
            try
            {
                editor.ReloadPorts();
            }
#if SYMPHONY_DEV
            catch (Exception e)
            {

                Debug.Log(e);

            }
#else
            catch (Exception)
            {

            }
#endif
        }
        public void ReloadPorts()
        {
            try
            {
                NodeEditorWindow currentEditorWindow = NodeEditorWindow.current;
                if (currentEditorWindow != null)
                {
                    CombineSplitEditorBase currentEditor = editor;
                    currentEditorWindow.onLateGUI += () =>
                    {
                        currentEditor.ReloadPorts();
                    };
                }
            }
#if SYMPHONY_DEV
            catch (Exception e)
            {

                Debug.Log(e);

            }
#else
            catch (Exception)
            {

            }
#endif
        }
    }

    public class PortDefinition
    {
        public string fieldName { get; private set; }
        public System.Type valueType { get; private set; }
        public NodePort.IO direction { get; private set; }
        public Node.ConnectionType connectionType { get; private set; }
        public Node.TypeConstraint typeConstraint { get; private set; }


        public PortDefinition(string fieldName, System.Type type, NodePort.IO direction, Node.ConnectionType connectionType, Node.TypeConstraint typeConstraint)
        {
            this.fieldName = fieldName;
            this.valueType = type;
            this.direction = direction;
            this.connectionType = connectionType;
            this.typeConstraint = typeConstraint;
        }
    }
}