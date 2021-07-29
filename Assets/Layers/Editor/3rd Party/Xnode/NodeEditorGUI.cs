using System;
using System.Collections.Generic;
using System.Reflection;
using ABXY.Layers.Editor.ThirdParty.Xnode.Internal;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.ThirdParty.Xnode
{
    /// <summary> Contains GUI methods </summary>
    public partial class NodeEditorWindow
    {
        public NodeGraphEditor graphEditor;
        private List<UnityEngine.Object> selectionCache;
        private List<Node> culledNodes;
        /// <summary> 19 if docked, 22 if not </summary>
        private int topPadding { get { return isDocked() ? 19 : 22; } }
        /// <summary> Executed after all other window GUI. Useful if Zoom is ruining your day. Automatically resets after being run.</summary>
        public event Action onLateGUI;
        private static readonly Vector3[] polyLineTempArray = new Vector3[2];


        protected virtual void OnGUI()
        {
            Event e = Event.current;
            Matrix4x4 m = GUI.matrix;



            if (graph == null) return;
            ValidateGraphEditor();

            if (!CheckControlBlock())
                Controls();

            DrawGrid(position, zoom, panOffset);
            DrawConnections();
            DrawDraggedConnection();

            DrawNodes();
            DrawSelectionBox();
            DrawTooltip();
            graphEditor.OnGUI();

            // Run and reset onLateGUI
            if (onLateGUI != null)
            {
                onLateGUI();
                onLateGUI = null;
            }

            GUI.matrix = m;
        }

        private bool CheckControlBlock()
        {
            foreach (Rect blockingRect in GetControlBlocks())
                if (blockingRect.Contains(Event.current.mousePosition))
                    return true;
            return false;
        }

        protected virtual List<Rect> GetControlBlocks()
        {
            return new List<Rect>();
        }

        public static void BeginZoomed(Rect rect, float zoom, float topPadding)
        {
            GUI.EndClip();

            GUIUtility.ScaleAroundPivot(Vector2.one / zoom, rect.size * 0.5f);
            Vector4 padding = new Vector4(0, topPadding, 0, 0);
            padding *= zoom;
            GUI.BeginClip(new Rect(-((rect.width * zoom) - rect.width) * 0.5f, -(((rect.height * zoom) - rect.height) * 0.5f) + (topPadding * zoom),
                rect.width * zoom,
                rect.height * zoom));
        }

        public static void EndZoomed(Rect rect, float zoom, float topPadding)
        {
            GUIUtility.ScaleAroundPivot(Vector2.one * zoom, rect.size * 0.5f);
            Vector3 offset = new Vector3(
                (((rect.width * zoom) - rect.width) * 0.5f),
                (((rect.height * zoom) - rect.height) * 0.5f) + (-topPadding * zoom) + topPadding,
                0);
            GUI.matrix = Matrix4x4.TRS(offset, Quaternion.identity, Vector3.one);
        }

        public void DrawGrid(Rect rect, float zoom, Vector2 panOffset)
        {

            rect.position = Vector2.zero;

            Vector2 center = rect.size / 2f;
            Texture2D gridTex = graphEditor.GetGridTexture();
            Texture2D crossTex = graphEditor.GetSecondaryGridTexture();

            // Offset from origin in tile units
            float xOffset = -(center.x * zoom + panOffset.x) / gridTex.width;
            float yOffset = ((center.y - rect.size.y) * zoom + panOffset.y) / gridTex.height;

            Vector2 tileOffset = new Vector2(xOffset, yOffset);

            // Amount of tiles
            float tileAmountX = Mathf.Round(rect.size.x * zoom) / gridTex.width;
            float tileAmountY = Mathf.Round(rect.size.y * zoom) / gridTex.height;

            Vector2 tileAmount = new Vector2(tileAmountX, tileAmountY);

            // Draw tiled background
            GUI.DrawTextureWithTexCoords(rect, gridTex, new Rect(tileOffset, tileAmount));
            GUI.DrawTextureWithTexCoords(rect, crossTex, new Rect(tileOffset + new Vector2(0.5f, 0.5f), tileAmount));
        }

        public void DrawSelectionBox()
        {
            if (currentActivity == NodeActivity.DragGrid)
            {
                Vector2 curPos = WindowToGridPosition(Event.current.mousePosition);
                Vector2 size = curPos - dragBoxStart;
                Rect r = new Rect(dragBoxStart, size);
                r.position = GridToWindowPosition(r.position);
                r.size /= zoom;
                Handles.DrawSolidRectangleWithOutline(r, new Color(0, 0, 0, 0.1f), new Color(1, 1, 1, 0.6f));
            }
        }

        public static bool DropdownButton(string name, float width)
        {
            return GUILayout.Button(name, EditorStyles.toolbarDropDown, GUILayout.Width(width));
        }

        /// <summary> Show right-click context menu for hovered reroute </summary>
        void ShowRerouteContextMenu(RerouteReference reroute)
        {
            GenericMenu contextMenu = new GenericMenu();
            contextMenu.AddItem(new GUIContent("Remove"), false, () => reroute.RemovePoint());
            contextMenu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
            if (NodeEditorPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
        }

        /// <summary> Show right-click context menu for hovered port </summary>
        void ShowPortContextMenu(NodePort hoveredPort)
        {
            GenericMenu contextMenu = new GenericMenu();
            contextMenu.AddItem(new GUIContent("Clear Connections"), false, () => hoveredPort.ClearConnections());
            contextMenu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
            if (NodeEditorPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
        }

        static Vector2 CalculateBezierPoint(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            float u = 1 - t;
            float tt = t * t, uu = u * u;
            float uuu = uu * u, ttt = tt * t;
            return new Vector2(
                (uuu * p0.x) + (3 * uu * t * p1.x) + (3 * u * tt * p2.x) + (ttt * p3.x),
                (uuu * p0.y) + (3 * uu * t * p1.y) + (3 * u * tt * p2.y) + (ttt * p3.y)
            );
        }

        /// <summary> Draws a line segment without allocating temporary arrays </summary>
        static void DrawAAPolyLineNonAlloc(float thickness, Vector2 p0, Vector2 p1)
        {
            polyLineTempArray[0].x = p0.x;
            polyLineTempArray[0].y = p0.y;
            polyLineTempArray[1].x = p1.x;
            polyLineTempArray[1].y = p1.y;
            //Handles.DrawAAPolyLine(thickness, polyLineTempArray);
            DrawAAPolyLine(thickness, polyLineTempArray);
        }

        private static System.Action<Color[], Vector3[], Color, int, Texture2D, float, Matrix4x4> _drawPolyLine;

        private static System.Action<Color[], Vector3[], Color, int, Texture2D, float, Matrix4x4> drawPolyLine
        {
            get
            {
                if (_drawPolyLine == null)
                    _drawPolyLine = (System.Action<Color[], Vector3[], Color, int, Texture2D, float, Matrix4x4>)
                        Delegate.CreateDelegate(typeof(System.Action<Color[], Vector3[], Color, int, Texture2D, float, Matrix4x4>), typeof(Handles).GetMethod("Internal_DrawAAPolyLine", BindingFlags.NonPublic | BindingFlags.Static));
                return _drawPolyLine;
            }
        }

        private static System.Action<UnityEngine.Rendering.CompareFunction> _ApplyWireMaterial;

        private static System.Action<UnityEngine.Rendering.CompareFunction> ApplyWireMaterial
        {
            get
            {
                if (_ApplyWireMaterial == null)
                {
                    MethodInfo method = typeof(HandleUtility).GetMethod("ApplyWireMaterial", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(UnityEngine.Rendering.CompareFunction) }, null);
                    _ApplyWireMaterial = (System.Action<UnityEngine.Rendering.CompareFunction>)Delegate.CreateDelegate(typeof(System.Action<UnityEngine.Rendering.CompareFunction>), method);
                }
                return _ApplyWireMaterial;
            }
        }

        private void StartDrawingPolylines()
        {
            ApplyWireMaterial?.Invoke(Handles.zTest);
        }

        private static void DrawAAPolyLine(float width, params Vector3[] points)
        {
            DrawAAPolyLine(null, points, -1, null, width, 0.75f);
        }

        private static void DrawAAPolyLine(Color[] colors, Vector3[] points, int actualNumberOfPoints, Texture2D lineTex, float width, float alpha)
        {
            if (Event.current.type != EventType.Repaint)
                return;
            //HandleUtility.ApplyWireMaterial(Handles.zTest);

            Color defaultColor = new Color(1, 1, 1, alpha);

            if (colors != null)
            {
                for (int i = 0; i < colors.Length; i++)
                    colors[i] *= defaultColor;
            }
            else
                defaultColor *= Handles.color;

            drawPolyLine.Invoke(colors, points, defaultColor, actualNumberOfPoints, lineTex, width, Handles.matrix);
        }

        /// <summary> Draw a bezier from output to input in grid coordinates </summary>
        public void DrawNoodle(Gradient gradient, NoodlePath path, NoodleStroke stroke, float thickness, List<Vector2> gridPoints)
        {
            if (Event.current.type != EventType.Repaint)
                return;
            // convert grid points to window points
            for (int i = 0; i < gridPoints.Count; ++i)
                gridPoints[i] = GridToWindowPosition(gridPoints[i]);

            StartDrawingLines();
            SetLineColor( gradient.Evaluate(0f));
            int length = gridPoints.Count;
            //StartDrawingPolylines();
            switch (path)
            {
                case NoodlePath.Curvy:
                    Vector2 outputTangent = Vector2.right;
                    for (int i = 0; i < length - 1; i++)
                    {
                        Vector2 inputTangent;
                        // Cached most variables that repeat themselves here to avoid so many indexer calls :p
                        Vector2 point_a = gridPoints[i];
                        Vector2 point_b = gridPoints[i + 1];
                        float dist_ab = Vector2.Distance(point_a, point_b);
                        if (i == 0) outputTangent = zoom * dist_ab * 0.01f * Vector2.right;
                        if (i < length - 2)
                        {
                            Vector2 point_c = gridPoints[i + 2];
                            Vector2 ab = (point_b - point_a).normalized;
                            Vector2 cb = (point_b - point_c).normalized;
                            Vector2 ac = (point_c - point_a).normalized;
                            Vector2 p = (ab + cb) * 0.5f;
                            float tangentLength = (dist_ab + Vector2.Distance(point_b, point_c)) * 0.005f * zoom;
                            float side = ((ac.x * (point_b.y - point_a.y)) - (ac.y * (point_b.x - point_a.x)));

                            p = tangentLength * Mathf.Sign(side) * new Vector2(-p.y, p.x);
                            inputTangent = p;
                        }
                        else
                        {
                            inputTangent = zoom * dist_ab * 0.01f * Vector2.left;
                        }

                        // Calculates the tangents for the bezier's curves.
                        float zoomCoef = 50 / zoom;
                        Vector2 tangent_a = point_a + outputTangent * zoomCoef;
                        Vector2 tangent_b = point_b + inputTangent * zoomCoef;
                        // Hover effect.
                        int division = Mathf.RoundToInt(.2f * dist_ab) + 3;
                        // Coloring and bezier drawing.
                        int draw = 0;
                        Vector2 bezierPrevious = point_a;
                        for (int j = 1; j <= division; ++j)
                        {
                            if (stroke == NoodleStroke.Dashed)
                            {
                                draw++;
                                if (draw >= 2) 
                                    draw = -2;
                                if (draw < 0) 
                                    continue;
                                if (draw == 0) 
                                    bezierPrevious = CalculateBezierPoint(point_a, tangent_a, tangent_b, point_b, (j - 1f) / (float)division);
                            }
                            if (i == length - 2)
                                SetLineColor( gradient.Evaluate((j + 1f) / division));
                            Vector2 bezierNext = CalculateBezierPoint(point_a, tangent_a, tangent_b, point_b, j / (float)division);
                            //DrawAAPolyLineNonAlloc(thickness, bezierPrevious, bezierNext);
                            DrawLine(thickness, bezierPrevious, bezierNext);
                            bezierPrevious = bezierNext;
                        }
                        outputTangent = -inputTangent;
                    }
                    break;
                case NoodlePath.Straight:
                    for (int i = 0; i < length - 1; i++)
                    {
                        Vector2 point_a = gridPoints[i];
                        Vector2 point_b = gridPoints[i + 1];
                        // Draws the line with the coloring.
                        Vector2 prev_point = point_a;
                        // Approximately one segment per 5 pixels
                        int segments = (int)Vector2.Distance(point_a, point_b) / 5;
                        segments = Math.Max(segments, 1);

                        int draw = 0;
                        for (int j = 0; j <= segments; j++)
                        {
                            draw++;
                            float t = j / (float)segments;
                            Vector2 lerp = Vector2.Lerp(point_a, point_b, t);
                            if (draw > 0)
                            {
                                if (i == length - 2)
                                    SetLineColor( gradient.Evaluate(t));
                                DrawLine(thickness, prev_point, lerp);
                            }
                            prev_point = lerp;
                            if (stroke == NoodleStroke.Dashed && draw >= 2) draw = -2;
                        }
                    }
                    break;
                case NoodlePath.Angled:
                    for (int i = 0; i < length - 1; i++)
                    {
                        if (i == length - 1) continue; // Skip last index
                        if (gridPoints[i].x <= gridPoints[i + 1].x - (50 / zoom))
                        {
                            float midpoint = (gridPoints[i].x + gridPoints[i + 1].x) * 0.5f;
                            Vector2 start_1 = gridPoints[i];
                            Vector2 end_1 = gridPoints[i + 1];
                            start_1.x = midpoint;
                            end_1.x = midpoint;
                            if (i == length - 2)
                            {
                            DrawLine(thickness, gridPoints[i], start_1);
                                SetLineColor( gradient.Evaluate(0.5f));
                                DrawLine(thickness, start_1, end_1);
                                SetLineColor(gradient.Evaluate(1f));
                                DrawLine(thickness, end_1, gridPoints[i + 1]);
                            }
                            else
                            {
                                DrawLine(thickness, gridPoints[i], start_1);
                                DrawLine(thickness, start_1, end_1);
                                DrawLine(thickness, end_1, gridPoints[i + 1]);
                            }
                        }
                        else
                        {
                            float midpoint = (gridPoints[i].y + gridPoints[i + 1].y) * 0.5f;
                            Vector2 start_1 = gridPoints[i];
                            Vector2 end_1 = gridPoints[i + 1];
                            start_1.x += 25 / zoom;
                            end_1.x -= 25 / zoom;
                            Vector2 start_2 = start_1;
                            Vector2 end_2 = end_1;
                            start_2.y = midpoint;
                            end_2.y = midpoint;
                            if (i == length - 2)
                            {
                            DrawLine(thickness, gridPoints[i], start_1);
                                SetLineColor(gradient.Evaluate(0.25f));
                                DrawLine(thickness, start_1, start_2);
                                SetLineColor(gradient.Evaluate(0.5f));
                                DrawLine(thickness, start_2, end_2);
                                SetLineColor(gradient.Evaluate(0.75f));
                                DrawLine(thickness, end_2, end_1);
                                SetLineColor(gradient.Evaluate(1f));
                                DrawLine(thickness, end_1, gridPoints[i + 1]);
                            }
                            else
                            {
                                DrawLine(thickness, gridPoints[i], start_1);
                                DrawLine(thickness, start_1, start_2);
                                DrawLine(thickness, start_2, end_2);
                                DrawLine(thickness, end_2, end_1);
                                DrawLine(thickness, end_1, gridPoints[i + 1]);
                            }
                        }
                    }

                    break;
                case NoodlePath.ShaderLab:
                    Vector2 start = gridPoints[0];
                    Vector2 end = gridPoints[length - 1];
                    //Modify first and last point in array so we can loop trough them nicely.
                    gridPoints[0] = gridPoints[0] + Vector2.right * (20 / zoom);
                    gridPoints[length - 1] = gridPoints[length - 1] + Vector2.left * (20 / zoom);
                    
                    //Draw first vertical lines going out from nodes
                    DrawLine(thickness, start, gridPoints[0]);
                    SetLineColor(gradient.Evaluate(1f));
                    DrawLine(thickness, end, gridPoints[length - 1]);
                    for (int i = 0; i < length - 1; i++)
                    {
                        Vector2 point_a = gridPoints[i];
                        Vector2 point_b = gridPoints[i + 1];
                        // Draws the line with the coloring.
                        Vector2 prev_point = point_a;
                        // Approximately one segment per 5 pixels
                        int segments = (int)Vector2.Distance(point_a, point_b) / 5;
                        segments = Math.Max(segments, 1);

                        int draw = 0;
                        for (int j = 0; j <= segments; j++)
                        {
                            draw++;
                            float t = j / (float)segments;
                            Vector2 lerp = Vector2.Lerp(point_a, point_b, t);
                            if (draw > 0)
                            {
                                if (i == length - 2) 
                                    SetLineColor( gradient.Evaluate(t));
                                DrawLine(thickness, prev_point, lerp);
                            }
                            prev_point = lerp;
                            if (stroke == NoodleStroke.Dashed && draw >= 2) draw = -2;
                        }
                    }
                    gridPoints[0] = start;
                    gridPoints[length - 1] = end;
                    break;
            }

            FinishDrawingLines();
        }

        /*static Material _lineMaterial;
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
        }*/

        static Material _lineMaterial;
        static Material lineMaterial
        {
            get
            {
                if (_lineMaterial == null)
                {
                    Shader shader = Shader.Find("Hidden/NodePort");
                    _lineMaterial = new Material(shader);
                    _lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                    _lineMaterial.SetTexture("_MainTexture", Resources.Load<Texture2D>("Symphony/LineAA"));
                    //_nodePortMaterial.SetColor("_Color", Color.green);
                }
                return _lineMaterial;
            }
        }

        private void StartDrawingLines()
        {
            firstDraw = true;
            GL.PushMatrix();
            lineMaterial.SetPass(0);
            GL.LoadPixelMatrix();
            GL.Begin(GL.QUADS);
        }

        private void FinishDrawingLines()
        {
            GL.End();
            GL.PopMatrix();
        }

        private void SetLineColor(Color color)
        {
            color.a = .75f;
            GL.Color(color);
        }
        bool firstDraw = false;
        Vector2 lastLine90Angle;
        private void DrawLine(float thickness, Vector2 p1, Vector2 p2)
        {
            //GL.Vertex(p1);
            //GL.Vertex(p2);
            //return;

            Vector2 lineDirection = (p2 - p1).normalized;
            //Vector2 offset = lineDirection * 2.5f;
            
            Vector2 vector90 = Rotate90(lineDirection);

            if (firstDraw)
            {
                firstDraw = false;
                lastLine90Angle = vector90;
            }

            Vector2 a = p1 -(lastLine90Angle * thickness / 2f) ; //p1 * vector90 * thickness / 2f;
            Vector2 b = p2 - (vector90 * thickness / 2f); //p2 * vector90 * thickness / 2f;
            Vector2 c = p1 + (lastLine90Angle * thickness / 2f); //p2 * -vector90 * thickness / 2f;
            Vector2 d = p2 + (vector90 * thickness/2f);//p1 * -vector90 * thickness / 2f;
            lastLine90Angle = vector90;
            GL.TexCoord(new Vector2(0,0));
            GL.Vertex(a);
            GL.TexCoord(new Vector2(0, 0));
            GL.Vertex(b);
            GL.TexCoord(new Vector2(0, 1));
            GL.Vertex(d);
            GL.TexCoord(new Vector2(0, 1));
            GL.Vertex(c);
        }

        private Vector2 Rotate90(Vector2 direction)
        {
            float sin = Mathf.Sin(90 * Mathf.Deg2Rad);
            float cos = Mathf.Cos(90 * Mathf.Deg2Rad);

            float tx = direction.x;
            float ty = direction.y;
            direction.x = (cos * tx) - (sin * ty);
            direction.y = (sin * tx) + (cos * ty);
            return direction;
        }

        /// <summary> Draws all connections </summary>
        public void DrawConnections()
        {
            Event e = Event.current;
            if (e.button == 2 || e.isScrollWheel)
            {
                //return;
            }

            if (e.type != EventType.Repaint)
                return;

            Vector2 mousePos = Event.current.mousePosition;
            List<RerouteReference> selection = preBoxSelectionReroute != null ? new List<RerouteReference>(preBoxSelectionReroute) : new List<RerouteReference>();
            hoveredReroute = new RerouteReference();

            List<Vector2> gridPoints = new List<Vector2>(2);

            Color col = GUI.color;
            foreach (Node node in graph.nodes)
            {
                //If a null node is found, return. This can happen if the nodes associated script is deleted. It is currently not possible in Unity to delete a null asset.
                if (node == null) continue;

                // Draw full connections and output > reroute
                foreach (NodePort output in node.Outputs)
                {
                    //Needs cleanup. Null checks are ugly
                    Rect fromRect;
                    if (!_portConnectionPoints.TryGetValue(output, out fromRect)) continue;

                    Color portColor = graphEditor.GetPortColor(output);
                    for (int k = 0; k < output.ConnectionCount; k++)
                    {
                        NodePort input = output.GetConnection(k);

                        Gradient noodleGradient = graphEditor.GetNoodleGradient(output, input);
                        float noodleThickness = graphEditor.GetNoodleThickness(output, input);
                        NoodlePath noodlePath = graphEditor.GetNoodlePath(output, input);
                        NoodleStroke noodleStroke = graphEditor.GetNoodleStroke(output, input);

                        // Error handling
                        if (input == null) continue; //If a script has been updated and the port doesn't exist, it is removed and null is returned. If this happens, return.
                        if (!input.IsConnectedTo(output)) input.Connect(output);
                        Rect toRect;
                        if (!_portConnectionPoints.TryGetValue(input, out toRect)) continue;

                        List<Vector2> reroutePoints = output.GetReroutePoints(k);

                        gridPoints.Clear();
                        gridPoints.Add(fromRect.center);
                        gridPoints.AddRange(reroutePoints);
                        gridPoints.Add(toRect.center);

                        bool culled = true;
                        foreach (Vector2 point in gridPoints)
                        {
                            if (!ShouldBeCulled(point))
                            {
                                culled = false;
                                break;
                            }
                        }

                        if (!culled)
                        {
                            DrawNoodle(noodleGradient, noodlePath, noodleStroke, noodleThickness, gridPoints);
                        }
                        // Loop through reroute points again and draw the points
                        for (int i = 0; i < reroutePoints.Count; i++)
                        {
                            RerouteReference rerouteRef = new RerouteReference(output, k, i);
                            // Draw reroute point at position
                            Rect rect = new Rect(reroutePoints[i], new Vector2(12, 12));
                            rect.position = new Vector2(rect.position.x - 6, rect.position.y - 6);
                            rect = GridToWindowRect(rect);

                            // Draw selected reroute points with an outline
                            if (selectedReroutes.Contains(rerouteRef))
                            {
                                GUI.color = NodeEditorPreferences.GetSettings().highlightColor;
                                GUI.DrawTexture(rect, NodeEditorResources.dotOuter);
                            }

                            GUI.color = portColor;
                            GUI.DrawTexture(rect, NodeEditorResources.dot);
                            if (rect.Overlaps(selectionBox)) selection.Add(rerouteRef);
                            if (rect.Contains(mousePos)) hoveredReroute = rerouteRef;

                        }
                    }
                }
            }


            GUI.color = col;
            if (Event.current.type != EventType.Layout && currentActivity == NodeActivity.DragGrid) selectedReroutes = selection;
        }

        private long lastDragFrame;

        private long lastMouseMoveFrame;
        /*
        private long lastLayoutFrame;
        private int numLayouts = 0;

        private long lastRepaintFrame;*/


        //private bool blockDraw = false;
        private void DrawNodes()
        {
            Event e = Event.current;
            if (e.button == 2 || e.isScrollWheel)
            {
                //return;
            }


            /*
            if (e.type == EventType.MouseDrag && lastDragFrame == Time.frameCount)
                return;
            else if (e.type == EventType.MouseDrag)
                lastDragFrame = Time.frameCount;

            if (e.type == EventType.MouseMove && lastMouseMoveFrame == Time.frameCount)
                return;
            else if (e.type == EventType.MouseMove)
                lastMouseMoveFrame = Time.frameCount;*/
            /*
            if (e.type == EventType.Layout && lastLayoutFrame == Time.frameCount)
            {
                numLayouts++;
                if (numLayouts == 2)
                    return;
            }
            else if (e.type == EventType.Layout)
            {
                numLayouts = 0;
                lastLayoutFrame = Time.frameCount;
            }*/

            /*if (e.type == EventType.Repaint && lastRepaintFrame == Time.frameCount)
                return;
            else if (e.type == EventType.Repaint)
                lastRepaintFrame = Time.frameCount;*/

            //Debug.Log($"{Time.frameCount} {e.type}");

            if (e.type == EventType.Layout)
            {
                selectionCache = new List<UnityEngine.Object>(Selection.objects);
            }

            System.Reflection.MethodInfo onValidate = null;
            if (Selection.activeObject != null && Selection.activeObject is Node)
            {
                onValidate = Selection.activeObject.GetType().GetMethod("OnValidate");
                if (onValidate != null) EditorGUI.BeginChangeCheck();
            }

            BeginZoomed(position, zoom, topPadding);

            Vector2 mousePos = Event.current.mousePosition;

            if (e.type != EventType.Layout)
            {
                hoveredNode = null;
                hoveredPort = null;
            }

            List<UnityEngine.Object> preSelection = preBoxSelection != null ? new List<UnityEngine.Object>(preBoxSelection) : new List<UnityEngine.Object>();

            // Selection box stuff
            Vector2 boxStartPos = GridToWindowPositionNoClipped(dragBoxStart);
            Vector2 boxSize = mousePos - boxStartPos;
            if (boxSize.x < 0) { boxStartPos.x += boxSize.x; boxSize.x = Mathf.Abs(boxSize.x); }
            if (boxSize.y < 0) { boxStartPos.y += boxSize.y; boxSize.y = Mathf.Abs(boxSize.y); }
            Rect selectionBox = new Rect(boxStartPos, boxSize);

            //Save guiColor so we can revert it
            Color guiColor = GUI.color;

            List<NodePort> removeEntries = new List<NodePort>();

            if (e.type == EventType.Layout) culledNodes = new List<Node>();

            for (int n = 0; n < graph.nodes.Count; n++)
            {
                if (graph.nodes[n] == null) continue;
                if (n >= graph.nodes.Count) return;
                Node node = graph.nodes[n];
                NodeEditor nodeEditor = NodeEditor.GetEditor(node, this);

                Vector2 nodePos = GridToWindowPositionNoClipped(node.position);

                GUILayout.BeginArea(new Rect(nodePos, new Vector2(nodeEditor.GetWidth(), 4000)));

                nodeEditor.OnPreDrawBody();
                GUILayout.EndArea();
            }

            //if((Event.current.type == EventType.MouseDrag && Event.current.button != 0) || Event.current.isScrollWheel)
            //blockDraw = true;

            for (int n = 0; n < graph.nodes.Count; n++)
            {
                // Skip null nodes. The user could be in the process of renaming scripts, so removing them at this point is not advisable.
                if (graph.nodes[n] == null) continue;
                if (n >= graph.nodes.Count) return;
                Node node = graph.nodes[n];

                // Culling
                if (e.type == EventType.Layout)
                {
                    // Cull unselected nodes outside view
                    if (!Selection.Contains(node) && ShouldBeCulled(node))
                    {
                        culledNodes.Add(node);
                        continue;
                    }
                }
                else if (culledNodes.Contains(node)) continue;



                if (e.type == EventType.Repaint)
                {
                    removeEntries.Clear();
                    foreach (var kvp in _portConnectionPoints)
                        if (kvp.Key.node == node) removeEntries.Add(kvp.Key);
                    foreach (var k in removeEntries) _portConnectionPoints.Remove(k);
                }

                NodeEditor nodeEditor = NodeEditor.GetEditor(node, this);

                NodeEditor.portPositions.Clear();

                // Set default label width. This is potentially overridden in OnBodyGUI
                EditorGUIUtility.labelWidth = 84;

                //Get node position
                Vector2 nodePos = GridToWindowPositionNoClipped(node.position);

                GUILayout.BeginArea(new Rect(nodePos, new Vector2(nodeEditor.GetWidth(), 4000)));

                bool selected = selectionCache.Contains(graph.nodes[n]);

                if (selected)
                {
                    GUIStyle style = new GUIStyle(nodeEditor.GetBodyStyle());
                    GUIStyle highlightStyle = new GUIStyle(nodeEditor.GetBodyHighlightStyle());
                    highlightStyle.padding = style.padding;
                    style.padding = new RectOffset();
                    GUI.color = nodeEditor.GetTint();
                    GUILayout.BeginVertical(style);
                    GUI.color = NodeEditorPreferences.GetSettings().highlightColor;
                    GUILayout.BeginVertical(new GUIStyle(highlightStyle));
                }
                else
                {
                    GUIStyle style = new GUIStyle(nodeEditor.GetBodyStyle());
                    GUI.color = nodeEditor.GetTint();
                    GUILayout.BeginVertical(style);
                }

                GUI.color = guiColor;
                EditorGUI.BeginChangeCheck();

#if !SYMPHONY_DEV
                try
                {
#endif
                //Draw node contents
                nodeEditor.OnHeaderGUI();

#if !SYMPHONY_DEV
                    try
                    {
#endif
                //if (!(blockDraw && Event.current.type != EventType.Layout))
                nodeEditor.portRenderer.BeginDraw();
                nodeEditor.layout.StartDraw();
                nodeEditor.OnBodyGUI();
                nodeEditor.layout.EndDraw();
                nodeEditor.portRenderer.EndDraw(node);
#if !SYMPHONY_DEV
                    }
                    catch (Exception)
                    {


                    }
#endif
#if !SYMPHONY_DEV
                }
                catch (Exception exception) when (exception.GetType() != typeof(UnityEngine.ExitGUIException))
                {
                    Debug.LogError(string.Format("Error drawing node, exception details: {0}", exception));
                }
#endif
                //If user changed a value, notify other scripts through onUpdateNode
                if (EditorGUI.EndChangeCheck())
                {
                    if (NodeEditor.onUpdateNode != null) NodeEditor.onUpdateNode(node);
                    EditorUtility.SetDirty(node);
                    nodeEditor.serializedObject.ApplyModifiedProperties();
                }

                GUILayout.EndVertical();

                //Cache data about the node for next frame
                if (e.type == EventType.Repaint)
                {
                    Vector2 size = GUILayoutUtility.GetLastRect().size;
                    if (nodeSizes.ContainsKey(node)) nodeSizes[node] = size;
                    else nodeSizes.Add(node, size);

                    foreach (var kvp in NodeEditor.portPositions)
                    {
                        Vector2 portHandlePos = kvp.Value;
                        portHandlePos += node.position;
                        Rect rect = new Rect(portHandlePos.x - 8, portHandlePos.y - 8, 16, 16);
                        portConnectionPoints[kvp.Key] = rect;
                    }
                }

                if (selected) GUILayout.EndVertical();

                if (e.type != EventType.Layout)
                {
                    //Check if we are hovering this node
                    Vector2 nodeSize = GUILayoutUtility.GetLastRect().size;
                    node.lastDrawSize = nodeSize;

                    Rect windowRect = new Rect(nodePos, nodeSize);
                    if (windowRect.Contains(mousePos)) hoveredNode = node;

                    //If dragging a selection box, add nodes inside to selection
                    if (currentActivity == NodeActivity.DragGrid)
                    {
                        if (windowRect.Overlaps(selectionBox)) preSelection.Add(node);
                    }

                    //Check if we are hovering any of this nodes ports
                    //Check input ports
                    foreach (NodePort input in node.Inputs)
                    {
                        //Check if port rect is available
                        if (!portConnectionPoints.ContainsKey(input)) continue;
                        Rect r = GridToWindowRectNoClipped(portConnectionPoints[input]);
                        if (r.Contains(mousePos)) hoveredPort = input;
                    }
                    //Check all output ports
                    foreach (NodePort output in node.Outputs)
                    {
                        //Check if port rect is available
                        if (!portConnectionPoints.ContainsKey(output)) continue;
                        Rect r = GridToWindowRectNoClipped(portConnectionPoints[output]);
                        if (r.Contains(mousePos)) hoveredPort = output;
                    }
                }

                GUILayout.EndArea();
            }

            //if (Event.current.type == EventType.Repaint)
            //blockDraw = false;

            if (e.type != EventType.Layout && currentActivity == NodeActivity.DragGrid) Selection.objects = preSelection.ToArray();
            EndZoomed(position, zoom, topPadding);

            //If a change in is detected in the selected node, call OnValidate method. 
            //This is done through reflection because OnValidate is only relevant in editor, 
            //and thus, the code should not be included in build.
            if (onValidate != null && EditorGUI.EndChangeCheck()) onValidate.Invoke(Selection.activeObject, null);
        }

        private bool ShouldBeCulled(Node node)
        {
            if (node.DoNotCull())
                return false;

            Vector2 nodePos = GridToWindowPositionNoClipped(node.position);
            if (nodePos.x / _zoom > position.width) return true; // Right
            else if (nodePos.y / _zoom > position.height) return true; // Bottom
            else if (nodeSizes.ContainsKey(node))
            {
                Vector2 size = nodeSizes[node];
                if (nodePos.x + size.x < 0) return true; // Left
                else if (nodePos.y + size.y < 0) return true; // Top
            }
            return false;
        }

        private bool ShouldBeCulled(Vector3 positionIn)
        {


            Vector2 nodePos = GridToWindowPositionNoClipped(positionIn);
            if (nodePos.x / _zoom > position.width) return true; // Right
            else if (nodePos.y / _zoom > position.height) return true; // Bottom
            else
            {
                if (nodePos.x < 0) return true; // Left
                else if (nodePos.y < 0) return true; // Top
            }
            return false;
        }

        private void DrawTooltip()
        {
            if (hoveredPort != null && NodeEditorPreferences.GetSettings().portTooltips && graphEditor != null)
            {
                string tooltip = graphEditor.GetPortTooltip(hoveredPort);
                if (string.IsNullOrEmpty(tooltip)) return;
                GUIContent content = new GUIContent(tooltip);
                Vector2 size = NodeEditorResources.styles.tooltip.CalcSize(content);
                size.x += 8;
                Rect rect = new Rect(Event.current.mousePosition - (size), size);
                EditorGUI.LabelField(rect, content, NodeEditorResources.styles.tooltip);
                Repaint();
            }
        }
    }
}