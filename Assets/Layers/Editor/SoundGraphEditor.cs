using System;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Editor.Node_Editor_Window;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Signal_Sources;
using ABXY.Layers.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using Help = ABXY.Layers.Editor.Docs.Help;

namespace ABXY.Layers.Editor
{
    [CustomNodeGraphEditor(typeof(SoundGraph))]
    public class SoundGraphEditor : NodeGraphEditor
    {
        //ReorderableList variablesList = null;

        SoundgraphCombinedStyle style = new SoundgraphCombinedStyle();

        public override void OnCreate()
        {

            base.OnCreate();
        }

        public override void OnGUI()
        {
            SoundGraph soundGraph = target as SoundGraph;
            if (soundGraph != null && soundGraph.graphInput == null)
            {
                GraphInputs gi = soundGraph.AddNode<GraphInputs>();
                gi.name = "Graph Input";
                AssetDatabase.AddObjectToAsset(gi, target);
                AssetDatabase.SaveAssets();
            }

            window.Repaint();
        }

        public override NodeEditorPreferences.Settings GetDefaultPreferences()
        {
            NodeEditorPreferences.Settings settings = new NodeEditorPreferences.Settings();
            settings.gridSnap = false;
            return settings;
        }

        public override Gradient GetNoodleGradient(NodePort output, NodePort input)
        {
            Gradient baseGradient = base.GetNoodleGradient(output, input);

            GradientColorKey outKey = baseGradient.colorKeys[0];
            GradientColorKey inKey = baseGradient.colorKeys[baseGradient.colorKeys.Length - 1];

            //if ((target as SoundGraph).isRunningSoundGraph)
            //{
            if (input != null && input.node != null && (FlowNode)input.node != null)
            {
                float colorMultiplier = Mathf.Lerp(1f, 0.5f, Mathf.Clamp01((float)(AudioSettings.dspTime - (input.node as FlowNode).lastAccessDSPTime)));
                inKey.color = colorMultiplier * inKey.color;
            }

            if (output != null && output.node != null && (FlowNode)output.node != null)
            {
                float colorMultiplier = Mathf.Lerp(1f, 0.5f, Mathf.Clamp01((float)(AudioSettings.dspTime - (output.node as FlowNode).lastAccessDSPTime)));
                outKey.color = colorMultiplier * outKey.color;
            }
            //}

            baseGradient.colorKeys = new GradientColorKey[] { outKey, inKey};

            return baseGradient;
        }

        public override NoodleStroke GetNoodleStroke(NodePort output, NodePort input)
        {
            if (output != null  && output.node != null && input != null && input.node && output.node.position.x > input.node.position.x)
            {
                return NoodleStroke.Dashed;
            }else
                return base.GetNoodleStroke(output, input);
        }

        public override string GetPortTooltip(NodePort port)
        {
            /*string portID = port.node.GetType().Name  + port.fieldName;
            if (Docs.Help.portDescriptions.ContainsKey(portID))
                return Docs.Help.portDescriptions[portID] + "\n"+base.GetPortTooltip(port);
            else*/ return base.GetPortTooltip(port);

        }

        public override Color GetTypeColor(Type type)
        {
            if (type == null)
                return Color.black;
            return LayersSettings.GetOrCreateSettings().GetColor(type.FullName, EditorGUIUtility.isProSkin);
        }


        public override void OnDropObjects(UnityEngine.Object[] objects)
        {

        }

        /*public override void OnDropObjects(Object[] objects)
    {
        foreach(Object droppedObj in objects)
        {
            if (droppedObj.GetType() == typeof(AudioClip))
            {
                Vector2 pos = NodeEditorWindow.current.WindowToGridPosition(Event.current.mousePosition);
                TrackNode newNode = (TrackNode)CreateNode(typeof(TrackNode), pos);
                newNode.sequence.Add(droppedObj as AudioClip);
            }
        }
    }*/
        /*
    public override Color GetPortColor(NodePort port)
    {
        if (port.ValueType != typeof(Flow))
            return base.GetPortColor(port);

        if (port.node == null || (port.node.GetValue(port) as Flow) == null)
            return port.IsInput ? Constants.flowErrorColor : Constants.defaultNodeColor;

        return Color.Lerp(Constants.defaultNodeColor, Constants.playingColor, (port.node.GetValue(port) as Flow).volume);
    }*/


        public override Texture2D GetGridTexture()
        {
            return style.grid;
        }

        public override Texture2D GetSecondaryGridTexture()
        {
            return style.grid;
        }
    }
}
