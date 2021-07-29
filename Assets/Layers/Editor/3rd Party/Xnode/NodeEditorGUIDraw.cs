using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace ABXY.Layers.Editor.ThirdParty.Xnode
{
    public static class NodeEditorGUIDraw
    {
        public static void PropertyField(Rect controlRect, SerializedProperty property, bool includeChildren = true)
        {
            PropertyField(controlRect,property, (GUIContent)null, includeChildren);
        }

        /// <summary> Make a field for a serialized property. Automatically displays relevant node port. </summary>
        public static void PropertyField(Rect controlRect, SerializedProperty property, GUIContent label, bool includeChildren = true)
        {
            if (property == null) throw new System.NullReferenceException();
            Node node = property.serializedObject.targetObject as Node;
            NodePort port = node.GetPort(property.name);
            PropertyField(controlRect, property, label, port, includeChildren);
        }

        /// <summary> Make a field for a serialized property. Manual node port override. </summary>
        public static void PropertyField(Rect controlRect, SerializedProperty property, NodePort port, bool includeChildren = true)
        {
            PropertyField(controlRect, property, null, port, includeChildren);
        }

        /// <summary> Make a field for a serialized property. Manual node port override. </summary>
        public static void PropertyField(Rect controlRect, SerializedProperty property, GUIContent label, NodePort port, bool includeChildren = true)
        {
            if (property == null) throw new System.NullReferenceException();

            // If property is not a port, display a regular property field
            if (port == null) LayersGUIUtilities.FastPropertyField(controlRect, label,property);
            else
            {
                Rect rect = new Rect();

                List<PropertyAttribute> propertyAttributes = NodeEditorUtilities.GetCachedPropertyAttribs(port.node.GetType(), property.name);

                // If property is an input, display a regular property field and put a port handle on the left side
                if (port.direction == NodePort.IO.Input)
                {
                    // Get data from [Input] attribute
                    Node.ShowBackingValue showBacking = Node.ShowBackingValue.Unconnected;
                    Node.InputAttribute inputAttribute;
                    bool dynamicPortList = false;
                    if (NodeEditorUtilities.GetCachedAttrib(port.node.GetType(), property.name, out inputAttribute))
                    {
                        dynamicPortList = inputAttribute.dynamicPortList;
                        showBacking = inputAttribute.backingValue;
                    }

                    bool usePropertyAttributes = dynamicPortList ||
                        showBacking == Node.ShowBackingValue.Never ||
                        (showBacking == Node.ShowBackingValue.Unconnected && port.IsConnected);

                    float spacePadding = 0;
                    foreach (var attr in propertyAttributes)
                    {
                        if (attr is SpaceAttribute)
                        {
                            if (usePropertyAttributes) GUILayout.Space((attr as SpaceAttribute).height);
                            else spacePadding += (attr as SpaceAttribute).height;
                        }
                        else if (attr is HeaderAttribute)
                        {
                            if (usePropertyAttributes)
                            {
                                //GUI Values are from https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/ScriptAttributeGUI/Implementations/DecoratorDrawers.cs
                                Rect position = controlRect;
                                position.yMin += EditorGUIUtility.singleLineHeight * 0.5f;
                                position = EditorGUI.IndentedRect(position);
                                GUI.Label(position, (attr as HeaderAttribute).header, EditorStyles.boldLabel);
                            }
                            else spacePadding += EditorGUIUtility.singleLineHeight * 1.5f;
                        }
                    }

                    /*if (dynamicPortList)
                    {
                        System.Type type = GetType(property);
                        Node.ConnectionType connectionType = inputAttribute != null ? inputAttribute.connectionType : Node.ConnectionType.Multiple;
                        DynamicPortList(property.name, type, property.serializedObject, port.direction, connectionType);
                        return;
                    }*/
                    switch (showBacking)
                    {
                        case Node.ShowBackingValue.Unconnected:
                            // Display a label if port is connected
                            if (port.IsConnected) EditorGUI.LabelField(controlRect, label != null ? label : new GUIContent(property.displayName));
                            // Display an editable property field if port is not connected
                            else LayersGUIUtilities.FastPropertyField(controlRect,label, property);
                            break;
                        case Node.ShowBackingValue.Never:
                            // Display a label
                            EditorGUI.LabelField(controlRect,label != null ? label : new GUIContent(property.displayName));
                            break;
                        case Node.ShowBackingValue.Always:
                            // Display an editable property field
                            LayersGUIUtilities.FastPropertyField(controlRect,label, property);
                            break;
                    }

                    rect = controlRect;
                    rect.position = rect.position - new Vector2(16, -spacePadding);
                    // If property is an output, display a text label and put a port handle on the right side
                }
                else if (port.direction == NodePort.IO.Output)
                {
                    // Get data from [Output] attribute
                    Node.ShowBackingValue showBacking = Node.ShowBackingValue.Unconnected;
                    Node.OutputAttribute outputAttribute;
                    bool dynamicPortList = false;
                    if (NodeEditorUtilities.GetCachedAttrib(port.node.GetType(), property.name, out outputAttribute))
                    {
                        dynamicPortList = outputAttribute.dynamicPortList;
                        showBacking = outputAttribute.backingValue;
                    }

                    bool usePropertyAttributes = dynamicPortList ||
                        showBacking == Node.ShowBackingValue.Never ||
                        (showBacking == Node.ShowBackingValue.Unconnected && port.IsConnected);

                    float spacePadding = 0;
                    foreach (var attr in propertyAttributes)
                    {
                        if (attr is SpaceAttribute)
                        {
                            if (usePropertyAttributes) GUILayout.Space((attr as SpaceAttribute).height);
                            else spacePadding += (attr as SpaceAttribute).height;
                        }
                        else if (attr is HeaderAttribute)
                        {
                            if (usePropertyAttributes)
                            {
                                //GUI Values are from https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/ScriptAttributeGUI/Implementations/DecoratorDrawers.cs
                                Rect position = controlRect;
                                position.yMin += EditorGUIUtility.singleLineHeight * 0.5f;
                                position = EditorGUI.IndentedRect(position);
                                GUI.Label(position, (attr as HeaderAttribute).header, EditorStyles.boldLabel);
                            }
                            else spacePadding += EditorGUIUtility.singleLineHeight * 1.5f;
                        }
                    }

                    /*if (dynamicPortList)
                    {
                        Type type = GetType(property);
                        Node.ConnectionType connectionType = outputAttribute != null ? outputAttribute.connectionType : Node.ConnectionType.Multiple;
                        DynamicPortList(property.name, type, property.serializedObject, port.direction, connectionType);
                        return;
                    }*/
                    switch (showBacking)
                    {
                        case Node.ShowBackingValue.Unconnected:
                            // Display a label if port is connected
                            if (port.IsConnected) EditorGUI.LabelField(controlRect, label != null ? label : new GUIContent(property.displayName), NodeEditorResources.OutputPort);
                            // Display an editable property field if port is not connected
                            else LayersGUIUtilities.FastPropertyField(controlRect,label, property);
                            break;
                        case Node.ShowBackingValue.Never:
                            // Display a label
                            EditorGUI.LabelField(controlRect,label != null ? label : new GUIContent(property.displayName), NodeEditorResources.OutputPort);
                            break;
                        case Node.ShowBackingValue.Always:
                            // Display an editable property field
                            LayersGUIUtilities.FastPropertyField(controlRect, label, property);
                            break;
                    }

                    rect = GUILayoutUtility.GetLastRect();
                    rect.position = rect.position + new Vector2(rect.width, spacePadding);
                }

                rect.size = new Vector2(16, 16);

                /*NodeEditor editor = NodeEditor.GetEditor(port.node, NodeEditorWindow.current);
                Color backgroundColor = editor.GetTint();
                Color col = NodeEditorWindow.current.graphEditor.GetPortColor(port);
                DrawPortHandle(rect, backgroundColor, col);

                // Register the handle position
                Vector2 portPos = rect.center;
                NodeEditor.portPositions[port] = portPos;*/
                PortRenderer.current.DrawPort(new Vector2(rect.x, rect.y), port);
            }
        }



        /// <summary> Make a simple port field. </summary>
        public static void PortField(Rect controlRect, NodePort port)
        {
            PortField(controlRect,null, port);
        }


        /// <summary> Make a simple port field. </summary>
        public static void PortField(Rect controlRect, GUIContent label, NodePort port)
        {
            if (port == null) return;

            Vector2 position = Vector3.zero;
            GUIContent content = label != null ? label : new GUIContent(ObjectNames.NicifyVariableName(port.fieldName));

            // If property is an input, display a regular property field and put a port handle on the left side
            if (port.direction == NodePort.IO.Input)
            {
                // Display a label
                EditorGUI.LabelField(controlRect, content);

                //Rect rect = GUILayoutUtility.GetLastRect();
                position = controlRect.position - new Vector2(16, 0);
            }
            // If property is an output, display a text label and put a port handle on the right side
            else if (port.direction == NodePort.IO.Output)
            {
                // Display a label
                EditorGUI.LabelField(controlRect, content, NodeEditorResources.OutputPort);

                position = controlRect.position + new Vector2(controlRect.width, 0);
            }
            PortRenderer.current.DrawPort(position, port);
        }

        public static void AddPortToRect(Rect controlRect, NodePort port)
        {
            if (port == null) return;

            Vector2 position = Vector3.zero;

            if (port.direction == NodePort.IO.Input)
            {
                //Rect rect = GUILayoutUtility.GetLastRect();
                position = controlRect.position - new Vector2(16, 0);
            }
            // If property is an output, display a text label and put a port handle on the right side
            else if (port.direction == NodePort.IO.Output)
            {
                position = controlRect.position + new Vector2(controlRect.width, 0);
            }
            PortRenderer.current.DrawPort(position, port);
        }


        /// <summary> Draws an input and an output port on the same line </summary>
        public static void PortPair(Rect controlRect, GUIContent inputLabel, NodePort input, GUIContent outputLabel, NodePort output)
        {
            Rect leftRect = new Rect(controlRect.x, controlRect.y, controlRect.width / 2f, controlRect.height);
            Rect rightRect = new Rect(controlRect.x + (controlRect.width / 2f), controlRect.y, controlRect.width / 2f, controlRect.height);
            
            PortField(leftRect,inputLabel, input);
            PortField(rightRect, outputLabel, output);
        }

        /// <summary> Draws an input and an output port on the same line </summary>
        public static void PortPair(Rect controlRect,  NodePort input,  NodePort output)
        {
            Rect leftRect = new Rect(controlRect.x, controlRect.y, controlRect.width / 2f, controlRect.height);
            Rect rightRect = new Rect(controlRect.x + (controlRect.width / 2f), controlRect.y, controlRect.width / 2f, controlRect.height);

            PortField(leftRect, null, input);
            PortField(rightRect, null, output);
        }

        


        static Material _nodePortMaterial;
        static Material nodePortMaterial
        {
            get
            {
                if (_nodePortMaterial == null) {
                    Shader shader = Shader.Find("Hidden/NodePort");
                    _nodePortMaterial = new Material(shader);
                    _nodePortMaterial.hideFlags = HideFlags.HideAndDontSave;
                    _nodePortMaterial.SetTexture("_MainTexture", NodeEditorResources.dotOuter);
                    //_nodePortMaterial.SetColor("_Color", Color.green);
                }
                return _nodePortMaterial;
            }
        }


        static Material _lineMaterial;
        static Material lineMaterial
        {
            get
            {
                if (_lineMaterial == null)
                {
                    Shader shader = Shader.Find("Hidden/Internal-Colored");
                    _lineMaterial = new Material(shader);
                    _lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                    _lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    _lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    _lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                    _lineMaterial.SetInt("_ZWrite", 0);
                }
                return _lineMaterial;
            }
        }

        public static void GLTexture(Rect rect, Color color)
        {
            if (Event.current.type == EventType.Repaint)
            {
                
                GL.PushMatrix();
                nodePortMaterial.SetPass(0);
                GL.LoadPixelMatrix();
                GL.Begin(GL.QUADS);
                GL.Color(color);

                GL.TexCoord2(0, 0);
                GL.Vertex3(rect.x, rect.y, 0);

                GL.TexCoord2(0, 0);
                GL.Vertex3(rect.x, rect.y + rect.height, 0);

                GL.TexCoord2(0, 1);
                GL.Vertex3(rect.x + rect.width, rect.y + rect.height, 0);

                GL.TexCoord2(1, 1);
                GL.Vertex3(rect.x + rect.width, rect.y, 0);
                GL.End();
                GL.PopMatrix();
            }
        }

        public static bool ImageButton(Rect position, Texture2D texture)
        {
            if (Event.current.type == EventType.Repaint)
                GUI.DrawTexture(position, texture);

            if (Event.current.type == EventType.MouseDown && position.Contains(Event.current.mousePosition))
            {
                Event.current.Use();
                return true;
            }
            return false;
        }

        public static bool ImageToggle(Rect position, bool state, Texture2D offTexture, Texture2D onTexture)
        {
            if (Event.current.type == EventType.Repaint)
                GUI.DrawTexture(position, state? onTexture:offTexture);

            if (Event.current.type == EventType.MouseDown && position.Contains(Event.current.mousePosition))
            {
                Event.current.Use();
                return !state;
            }
            return state;
        }

    }

    public class NodeLayoutUtility
    {
        Rect startingRect;
        public NodeLayoutUtility(Rect drawArea)
        {
            startingRect = drawArea;
            startingRect.y += EditorGUIUtility.standardVerticalSpacing;
            startingRect.height = EditorGUIUtility.singleLineHeight;
        }

        public NodeLayoutUtility(float heightInPixels)
        {
            startingRect = EditorGUILayout.GetControlRect(false, heightInPixels);
            startingRect.y += EditorGUIUtility.standardVerticalSpacing;
            startingRect.height = EditorGUIUtility.singleLineHeight;
        }

        public NodeLayoutUtility(int heightInLines)
        {
            startingRect = EditorGUILayout.GetControlRect(false, heightInLines * EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * (heightInLines + 1));
            startingRect.y += EditorGUIUtility.standardVerticalSpacing;
            startingRect.height = EditorGUIUtility.singleLineHeight;
        }

        private float lastCalculatedHeight = 0;
        private float startHeight = 0f;
        public NodeLayoutUtility()
        {

        }


        public void StartDraw()
        {
            startingRect = EditorGUILayout.GetControlRect(false, lastCalculatedHeight);
            startHeight = startingRect.y;
            //Event current = Event.current;
            //if (current.type == EventType.Repaint && lastCalculatedHeight == 0)
                //Debug.Log("Test");

        }


        public void EndDraw()
        {
            float newCalculateHeight = startingRect.y - startHeight;


            // On first draw, lastCalculatedHeight 
            if (newCalculateHeight > lastCalculatedHeight)
            {
                EditorGUILayout.GetControlRect(false, newCalculateHeight - lastCalculatedHeight);
            }

            lastCalculatedHeight = newCalculateHeight;
        }

        public Rect DrawLine()
        {
            return DrawLines(1);
        }

        public Rect DrawLines(int numLines)
        {
            float height = numLines * EditorGUIUtility.singleLineHeight + Mathf.Clamp((numLines - 1), 0, int.MaxValue) * EditorGUIUtility.standardVerticalSpacing;
            return Draw(height);
        }

        private Rect lastDrawnRect = new Rect();

        public Rect Draw(float height)
        {
            startingRect.height = height;
            Rect returnRect = startingRect;
            lastDrawnRect = returnRect;
            startingRect.y += startingRect.height + EditorGUIUtility.standardVerticalSpacing;
            return returnRect;
        }

        public Rect LastRect()
        {
            return lastDrawnRect;
        }
    }
}