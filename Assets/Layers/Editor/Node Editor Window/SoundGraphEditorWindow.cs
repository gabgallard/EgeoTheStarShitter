using System.Collections.Generic;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Editor.Code_generation;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using SoundGraph = ABXY.Layers.Runtime.SoundGraph;
using ABXY.Layers.Editor.Node_Editors;
using ABXY.Layers.Editor.Timeline_Editor.Variants.PlayNode;
using ABXY.Layers.Runtime.Nodes;
using System.Linq;
using ABXY.Layers.Runtime.Settings;
using static ABXY.Layers.Runtime.Settings.LayersSettings;

namespace ABXY.Layers.Editor.Node_Editor_Window
{
    public class SoundGraphEditorWindow : NodeEditorWindow
    {

        //float topBarHeight = 30;

        private bool wasPlaying = false;

        //private bool stopButtonClicked = false;

        internal SoundgraphCombinedStyle style = new SoundgraphCombinedStyle();

        private static GUIStyle _runtimeCopyStyle;

        private static GUIStyle runtimeCopyStyle
        {
            get
            {
                if (_runtimeCopyStyle == null)
                {
                    _runtimeCopyStyle = new GUIStyle(EditorStyles.boldLabel);
                    _runtimeCopyStyle.fontSize += 25;
                    _runtimeCopyStyle.normal.textColor = new Color32(57,237,150,255);
                }
                return _runtimeCopyStyle;
            }
        }

        private static GUIStyle _assetCopyStyle;
        private static GUIStyle assetCopyStyle
        {
            get
            {
                if (_assetCopyStyle == null)
                {
                    _assetCopyStyle = new GUIStyle(EditorStyles.boldLabel);
                    _assetCopyStyle.fontSize += 25;
                    _assetCopyStyle.normal.textColor = new Color32(90, 97, 105, 255);
                }
                return _assetCopyStyle;
            }
        }

        private Texture2D _windowIcon = null;
        private Texture2D windowIcon { 
            get {
                if (_windowIcon == null)
                    _windowIcon = Resources.Load<Texture2D>("Symphony/SoundWindowIcon");
                return _windowIcon;
            } 
        }

        public static SoundGraphEditorWindow instance { get; private set; }

        private TriggerPopup triggerPopup;


        protected override void OnEnable()
        {

            if (instance == null)
                instance = this;

            base.OnEnable();

            triggerPopup = new TriggerPopup(this);

            SoundGraph castGraph = graph as SoundGraph;
            if (castGraph != null)
            {
                castGraph.GraphAwake();
                castGraph.GraphStart();
            }
        }

        protected override void OnGUI()
        {
            if (instance == null)
                instance = this;
            
            Event e = Event.current;
            if (e.type != EventType.Layout && e.type != EventType.Repaint && !UnityEditorInternal.InternalEditorUtility.isApplicationActive)
                return;

            //EventInterceptor.Begin(true);

            //drawing this once to intercept events. This is an awful hack of Unity's event system
            triggerPopup.Draw();

            // closing sound graphs that have been returned to the pool
            if (graph != null && (graph as SoundGraph).isCurrentlyInPool)
                graph = null;

            // If the currently displayed sound graph is an asset, and the selected player is a runtime copy of that graph, switch to it
            if (Application.isPlaying && graph != null && !(graph as SoundGraph).isRunningSoundGraph && Selection.activeGameObject != null)
            {
                SoundGraphPlayer player = Selection.activeGameObject.GetComponent<SoundGraphPlayer>();
                if (player != null && player.runtimeGraphCopy != null && 
                    player.runtimeGraphCopy.graphID == (graph as SoundGraph).graphID)
                    SoundGraphEditorWindow.Open(player.runtimeGraphCopy);
                //if no graph is displayed but we're selecting a player, switch to that player's graph
            }else if (graph==null && Selection.activeGameObject != null)
            {
                SoundGraphPlayer player = Selection.activeGameObject.GetComponent<SoundGraphPlayer>();
                if (player != null && player.runtimeGraphCopy != null)
                    SoundGraphEditorWindow.Open(player.runtimeGraphCopy);
                else if (player != null && player.soundGraph != null)
                    SoundGraphEditorWindow.Open(player.soundGraph);
            }

            wasPlaying = Application.isPlaying;


            if (graph != null)
                this.titleContent = new GUIContent(graph.name, windowIcon);
            else
                this.titleContent = new GUIContent("Sound graph editor", windowIcon);


            base.OnGUI();
            GUI.EndClip();

            GUI.BeginClip(new Rect(0, 20, position.width, position.height));
        
            DrawTopBar();
            DrawRuntimeCopyText();

            DrawRegenWindow();

            //EventInterceptor.End();

            triggerPopup.CheckForSpacebarEvent();
            triggerPopup.Draw();

            // escape key to kill playback
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                Event.current.Use();
                EndAllPlayback();
            }

            if (graph as SoundGraph != null)
                (graph as SoundGraph).GraphUpdate();

            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
        }

        

        private void DrawRegenWindow()
        {
            SoundGraphDBEntry[] graphs = LayersSettings.GetOrCreateSettings().GetGraphsNeedingCodeGen();

            GUILayout.BeginArea(new Rect(EditorGUIUtility.standardVerticalSpacing * 2f, 20, 170, position.height- 20 - (EditorGUIUtility.standardVerticalSpacing * 2f)));
            GUILayout.BeginVertical();

            GUILayout.FlexibleSpace();

            foreach (SoundGraphDBEntry graph in graphs)
            {
                if (graph == null)
                    break;

                string content = graph.name + " requires code regeneration";
                float height = style.messageStyle.CalcHeight(new GUIContent(content),170);

                Rect controlRect = EditorGUILayout.GetControlRect(false, height);

                EditorGUI.DrawRect(controlRect, ((Color)style.nodeBackgroundColor) * 0.8f);

                Rect boxRect = new Rect(controlRect.x + 1, controlRect.y + 1, controlRect.width - 2, controlRect.height - 2);
                EditorGUI.DrawRect(boxRect, style.nodeBackgroundColor);

                GUI.Label(controlRect, content, style.messageStyle);


            }

            if (graphs.Length != 0 && GUILayout.Button("Regenerate Code"))
                RegenList.Generate();

            GUILayout.EndVertical();
            GUILayout.EndArea();
        
        }

        private void DrawRuntimeCopyText()
        {
            if (Application.isPlaying && graph != null)
            {
                if ((graph as SoundGraph).isRunningSoundGraph)
                {
                    float width = 290;
                    Rect labelRect = new Rect(position.width - width, position.height - 50, width, 50);
                    EditorGUI.LabelField(labelRect, new GUIContent("Runtime Copy"), runtimeCopyStyle);
                }
                else
                {
                    float width = 260;
                    Rect labelRect = new Rect(position.width - width, position.height - 50, width, 50);
                    EditorGUI.LabelField(labelRect, new GUIContent("Graph Asset"), assetCopyStyle);
                }
            }
        }

        private void DrawTopBar()
        {
            Rect toolbarRect = new Rect(0, 0, position.width, 30);
            EditorGUI.DrawRect(toolbarRect, style.headerColor);

            if (graph == null)
                return;

            GUILayout.Space(7);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField( graph.name, style.soundGraphNameLabelStyle);

            if (LayersSettings.GetOrCreateSettings().CheckIfSoundGraphNeedsRegen(graph as SoundGraph))
            {
                Rect regenLabelPosition = toolbarRect;
                EditorGUI.LabelField(regenLabelPosition, "(Needs code regeneration)", style.codeRegenLabelStyle);
            }

        

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Stop Playback"))
            {
                EndAllPlayback();
            }

            EditorGUILayout.EndHorizontal();


        }

        //Stops all playback in the sound graph
        private void EndAllPlayback()
        {
            if (graph == null)
                return;

            (graph as SoundGraph).CallEvent("EndAll", AudioSettings.dspTime,0);
            if (!Application.isPlaying)
                (graph as SoundGraph).ResetVariablesToDefaults();
        }

        protected override List<Rect> GetControlBlocks()
        {
            List<Rect> rectList = new List<Rect>();
            rectList.Add(new Rect(0, 0, position.width, 30));
            return rectList;
        }

        [OnOpenAsset(0)]
        public static bool OnDoubleClick(int instanceID, int line)
        {
            SoundGraph nodeGraph = EditorUtility.InstanceIDToObject(instanceID) as SoundGraph;
            if (nodeGraph != null)
            {
                Open(nodeGraph);
                return true;
            }
            return false;
        }

        public new static SoundGraphEditorWindow Open(NodeGraph graph)
        {
            if (!graph) return null;

            LayersSettings.GetOrCreateSettings().RegisterSoundGraph((SoundGraph)graph);

            PlayNodeTimelineWindow.CloseLastWindowIfOpen();

            SoundGraphEditorWindow w = GetWindow(typeof(SoundGraphEditorWindow), false, "xNode", true) as SoundGraphEditorWindow;
            w.wantsMouseMove = true;
            /*
            if (w.graph != null && !Application.isPlaying)
            {
                (w.graph as SoundGraph).CallEvent("EndAll", AudioSettings.dspTime,0);

                if (!Application.isPlaying)
                    (graph as SoundGraph)?.ResetVariablesToDefaults();
            }

            if (w.graph != null && w.graph is SoundGraph && !(w.graph as SoundGraph).isRunningSoundGraph)
                (w.graph as SoundGraph).subgraphNode = null;
            */
            w.graph = graph;

            w.titleContent = new GUIContent(w.titleContent.text,Resources.Load<Texture2D>("Symphony/SoundWindowIcon"));

            List<Node> nodes = graph.nodes;
            foreach (FlowNode node in nodes)
                node.OnNodeOpenedInGraphEditor();


            return w;
        }
        
        
        public void ShowTriggerDialog()
        {
            triggerPopup.Show();
        }

        public void ShowTriggerDialog(GraphEvent gevent)
        {
            triggerPopup.Show(gevent);
        }


        private void OnDestroy()
        {
            if (graph != null && !Application.isPlaying)
                (graph as SoundGraph).CallEvent("EndAll", AudioSettings.dspTime, 0);

            if (!Application.isPlaying)
                (graph as SoundGraph)?.ResetVariablesToDefaults();

            PlayNodeTimelineWindow.CloseLastWindowIfOpen();
        }
    }
}
