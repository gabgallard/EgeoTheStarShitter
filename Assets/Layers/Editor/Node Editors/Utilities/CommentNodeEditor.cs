using ABXY.Layers.Editor.Node_Editor_Window;
using ABXY.Layers.Runtime.Nodes.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Editor.ThirdParty.Xnode.Internal;

namespace ABXY.Layers.Editor.Node_Editors.Utilities
{
    [CustomNodeEditor(typeof(CommentNode))]
    public class CommentNodeEditor : FlowNodeEditor
    {


        Vector2 lastNodePosition = Vector2.zero;

        private bool dragging = false;

        List<Node> nodesInRect = new List<Node>();
        List<RerouteReference> reroutesInRect = new List<RerouteReference>();

        private float timeOfLastMove;

        public override void OnCreate()
        {
            base.OnCreate();
            lastNodePosition = target.position;
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            serializedObject.UpdateIfRequiredOrScript();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("comment"));
            serializedObject.ApplyModifiedProperties();


            

        }

        public override void OnPreDrawBody()
        {
            GUILayout.EndArea();

            Vector2 nodePosition = SoundGraphEditorWindow.instance.zoom * SoundGraphEditorWindow.instance.GridToWindowPosition(target.position);
            GUILayout.BeginArea(new Rect(nodePosition, new Vector2(4000, 4000)));


            SerializedProperty rectProp = serializedObject.FindProperty("commentDimensions");
            DrawCommentRect(rectProp);


            //EditorGUI.DrawRect(new Rect(0, 0, 100, 100), Color.red);
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(target.position, new Vector2(GetWidth(), 4000)));


            if (lastNodePosition != target.position && Event.current.modifiers == EventModifiers.Shift)
            {
                timeOfLastMove = Time.time;
                if (!dragging)
                {
                    nodesInRect.Clear();
                    reroutesInRect.Clear();
                    Rect searchPosition = new Rect(target.position.x, target.position.y, target.position.x + rectProp.vector2Value.x + 5, target.position.y + rectProp.vector2Value.y + 30);

                    foreach (Node node in target.graph.nodes)
                    {
                        //finding nodes to move
                        if (searchPosition.Contains(node.position) && node != target)
                            nodesInRect.Add(node);

                        //finding ports to move
                        for (int portIndex = 0; portIndex < node.Ports.Count(); portIndex++)
                        {
                            NodePort port = node.Ports.ElementAt(portIndex);
                            for (int connectionIndex = 0; connectionIndex < port.ConnectionCount; connectionIndex++)
                            {
                                List<Vector2> reroutePoints = port.GetReroutePoints(connectionIndex);
                                for (int reroutePointIndex = 0; reroutePointIndex < reroutePoints.Count; reroutePointIndex++)
                                {
                                    if (searchPosition.Contains(reroutePoints[reroutePointIndex]))
                                    {
                                        reroutesInRect.Add(new RerouteReference(port, connectionIndex, reroutePointIndex));
                                    }
                                }
                            }
                        }
                    }

                }
                dragging = true;

                foreach (Node node in nodesInRect)
                {
                    node.position += (target.position - lastNodePosition);
                }

                foreach (RerouteReference reroute in reroutesInRect)
                {
                    Vector2 point = reroute.GetPoint();
                    point = point + (target.position - lastNodePosition);
                    reroute.SetPoint(point);
                }

            }
            lastNodePosition = target.position;

            if (Time.time > timeOfLastMove + 0.5f)
                dragging = false;
        }



        private bool rightDrag = false;
        private bool bottomDrag = false;
        private void DrawCommentRect(SerializedProperty rectProp)
        {
            Vector2 dimensions = rectProp.vector2Value;
            Color frameColor = style.nodeBackgroundColor;

            float nodeWidth = GetWidth();
            Vector2 root = new Vector2(5,30);

            Rect top = new Rect(nodeWidth + root.x-10, root.y, dimensions.x - nodeWidth + 10, 2f);
            EditorGUI.DrawRect(top, frameColor);

            Rect bottom = new Rect(root.x, root.y + dimensions.y-2, dimensions.x, 2f);
            EditorGUI.DrawRect(bottom, frameColor);

            Rect right = new Rect(dimensions.x + root.x, root.y, 2f, dimensions.y);
            EditorGUI.DrawRect(right, frameColor);

            Rect left = new Rect(root.x, root.y, 2f, dimensions.y);
            EditorGUI.DrawRect(left, frameColor);

            Rect dragTab = new Rect(root.x + dimensions.x - 20, root.y - dimensions.y, 20, 20);
            EditorGUI.DrawRect(dragTab, frameColor);

            Color backgroundColor = new Color(frameColor.r, frameColor.g, frameColor.b, 0.5f);
            EditorGUI.DrawRect(new Rect(root, dimensions), backgroundColor);


            float dragWidth = 8 * SoundGraphEditorWindow.instance.zoom;

            Rect rightClickTarget = new Rect(dimensions.x + root.x- dragWidth/2f, root.y, dragWidth, dimensions.y);

            // Bottom Right Corner Drag
            Rect bottomRightCornerDrag = new Rect(root.x + dimensions.x - dragWidth, root.y + dimensions.y - dragWidth-2, dragWidth,  dragWidth);
            EditorGUI.DrawRect(bottomRightCornerDrag, frameColor);

            if (Event.current.type == EventType.MouseDown && bottomRightCornerDrag.Contains(Event.current.mousePosition))
            {
                rightDrag = true;
                bottomDrag = true;
                Event.current.Use();
            }

            //right drag
            if (Event.current.type == EventType.MouseDown && rightClickTarget.Contains(Event.current.mousePosition))
            {
                rightDrag = true;
                Event.current.Use();
            }

            if (rightDrag && Event.current.type == EventType.MouseDrag)
            {
                SoundGraphEditorWindow.currentActivity = ThirdParty.Xnode.NodeEditorWindow.NodeActivity.Idle;
                float newWidth = Mathf.Clamp( rectProp.vector2Value.x + Event.current.delta.x, nodeWidth+20f, float.MaxValue);
                rectProp.vector2Value = new Vector2(newWidth, rectProp.vector2Value.y);
                rectProp.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                //Event.current.Use();
            }


            //bottom Drag
            Rect bottomClickTarget = new Rect(root.x, root.y + dimensions.y - dragWidth/2f, dimensions.x, dragWidth);

            if (Event.current.type == EventType.MouseDown && bottomClickTarget.Contains(Event.current.mousePosition))
            {
                bottomDrag = true;
                Event.current.Use();
            }

            if (Event.current.type == EventType.MouseUp)
            {
                bottomDrag = false;
                rightDrag = false;
                rectProp.serializedObject.ApplyModifiedProperties();
            }

            if (bottomDrag && Event.current.type == EventType.MouseDrag)
            {
                SoundGraphEditorWindow.currentActivity = ThirdParty.Xnode.NodeEditorWindow.NodeActivity.Idle;
                float newHeight = Mathf.Clamp(rectProp.vector2Value.y + Event.current.delta.y, 200f, float.MaxValue);
                rectProp.vector2Value = new Vector2(rectProp.vector2Value.x, newHeight);
                rectProp.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                //Event.current.Use();
            }

        }


    }
}