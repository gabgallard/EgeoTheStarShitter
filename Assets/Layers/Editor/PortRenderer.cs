using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortRenderer
{
    private List<PortInstance> portCache = new List<PortInstance>();

    private Dictionary<Color, List<Vector2>> drawCache = new Dictionary<Color, List<Vector2>>();

    private bool needsRefresh = false;
    private int currentPort = 0;

    public static PortRenderer current;

    static Material _nodePortMaterialOuter;
    static Material nodePortMaterialOuter
    {
        get
        {
            if (_nodePortMaterialOuter == null)
            {
                Shader shader = Shader.Find("Hidden/NodePort");
                _nodePortMaterialOuter = new Material(shader);
                _nodePortMaterialOuter.hideFlags = HideFlags.HideAndDontSave;
                _nodePortMaterialOuter.SetTexture("_MainTexture", NodeEditorResources.dotOuter);
                //_nodePortMaterial.SetColor("_Color", Color.green);
            }
            return _nodePortMaterialOuter;
        }
    }

    static Material _nodePortMaterialInner;
    static Material nodePortMaterialInner
    {
        get
        {
            if (_nodePortMaterialInner == null)
            {
                Shader shader = Shader.Find("Hidden/NodePort");
                _nodePortMaterialInner = new Material(shader);
                _nodePortMaterialInner.hideFlags = HideFlags.HideAndDontSave;
                _nodePortMaterialInner.SetTexture("_MainTexture", NodeEditorResources.dot);
                //_nodePortMaterial.SetColor("_Color", Color.green);
            }
            return _nodePortMaterialInner;
        }
    }


    public void BeginDraw()
    {
        current = this;
        if (Event.current.type == EventType.Layout)
        {
            currentPort = 0;
            needsRefresh = false;
        }
    }

    public void EndDraw(Node node)
    {
        if (Event.current.type == EventType.Repaint)
        {
            if (portCache.Count != currentPort) {
                needsRefresh = true;

                if (currentPort < portCache.Count)
                    portCache = portCache.GetRange(0, currentPort);
            }



            if (needsRefresh)
            {
                drawCache.Clear();
                foreach(PortInstance port in portCache)
                {
                    Color color = NodeEditorWindow.current.graphEditor.GetTypeColor(port.type);
                    if (!drawCache.ContainsKey(color))
                    {
                        drawCache.Add(color, new List<Vector2>());
                    }
                    drawCache[color].Add(port.position);
                }
            }


        

            GL.PushMatrix();
            nodePortMaterialOuter.SetPass(0);
            GL.LoadPixelMatrix();
            GL.Begin(GL.QUADS);

            NodeEditor editor = NodeEditor.GetEditor(node, NodeEditorWindow.current);
            Color backgroundColor = editor.GetTint();

            GL.Color(backgroundColor);
            foreach (KeyValuePair<Color, List<Vector2>> ports in drawCache)
            {
                foreach (Vector2 position in ports.Value)
                {
                    Rect rect = new Rect(position, new Vector2(16, 16));
                    

                    GL.TexCoord2(0, 0);
                    GL.Vertex3(rect.x, rect.y, 0);

                    GL.TexCoord2(0, 1);
                    GL.Vertex3(rect.x, rect.y + rect.height, 0);

                    GL.TexCoord2(1, 1);
                    GL.Vertex3(rect.x + rect.width, rect.y + rect.height, 0);

                    GL.TexCoord2(1, 0);
                    GL.Vertex3(rect.x + rect.width, rect.y, 0);
                }
            }

            nodePortMaterialInner.SetPass(0);

            foreach (KeyValuePair<Color, List<Vector2>> ports in drawCache)
            {
                GL.Color(ports.Key);
                foreach (Vector2 position in ports.Value)
                {
                    Rect rect = new Rect(position, new Vector2(16, 16));


                    GL.TexCoord2(0, 0);
                    GL.Vertex3(rect.x, rect.y, 0);

                    GL.TexCoord2(0, 1);
                    GL.Vertex3(rect.x, rect.y + rect.height, 0);

                    GL.TexCoord2(1, 1);
                    GL.Vertex3(rect.x + rect.width, rect.y + rect.height, 0);

                    GL.TexCoord2(1, 0);
                    GL.Vertex3(rect.x + rect.width, rect.y, 0);
                }
            }



            GL.End();
            GL.PopMatrix();
        }
    }


    public void DrawPort(Vector2 position, NodePort port)
    {
        if (Event.current.type == EventType.Repaint)
        {
            if (portCache.Count > currentPort)// then we don't need to add
            {
                if (portCache[currentPort].hashCode != CalculateHash(position, port))
                {
                    portCache[currentPort] = new PortInstance(position, port);
                    needsRefresh = true;
                }
            }
            else
            {
                portCache.Add(new PortInstance(position, port));
                needsRefresh = true;
            }
            currentPort++;
        }

        NodeEditor.portPositions[port] = position + new Vector2(8, 7);
    }

    private struct PortInstance
    {
        public Vector2 position { get; private set; }
        public System.Type type { get; private set; }

        public int hashCode { get; private set; }

        public PortInstance(Vector2 position, NodePort port) : this()
        {
            this.position = position;
            type = port.ValueType;
            hashCode = CalculateHash(position, port);
        }


    }

    private static int CalculateHash(Vector2 position, NodePort port)
    {
        int positionHash = position.GetHashCode();
        int portHash = port.GetHashCode();
        return positionHash + portHash;
    }
}
