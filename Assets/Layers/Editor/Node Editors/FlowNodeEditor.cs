using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Editor.Node_Editor_Window;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Playback;
using UnityEditor;
using UnityEngine;
using ABXY.Layers.Runtime.Settings;

namespace ABXY.Layers.Editor.Node_Editors
{
    public class FlowNodeEditor : NodeEditor
    {
        private Color currentColor = Color.white;

        public static SoundgraphCombinedStyle style = new SoundgraphCombinedStyle();

        private bool _targetIsAsset = false;
        private bool hasCheckedIfAsset = false;
        protected bool targetIsAsset
        {
            get
            {
                if (!hasCheckedIfAsset)
                    _targetIsAsset = AssetDatabase.GetAssetPath(target) != "";
                hasCheckedIfAsset = true;
                return _targetIsAsset;
            }
        }

        protected bool targetIsRuntimeGraph
        {
            get { return (target.graph as SoundGraph).isRunningSoundGraph; }
        }

        private static Texture2D _maximizeTexture;
        private static Texture2D maximizeTexture
        {
            get
            {
                if (_maximizeTexture == null)
                    _maximizeTexture = Resources.Load<Texture2D>("Symphony/Maximize");
                return _maximizeTexture;
            }
        }


        private static Texture2D _minimizeTexture;
        private static Texture2D minimizeTexture
        {
            get
            {
                if (_minimizeTexture == null)
                    _minimizeTexture = Resources.Load<Texture2D>("Symphony/Minimize");
                return _minimizeTexture;
            }
        }



        private NodePort playFinishedPort;

        public SerializedObjectTree serializedObjectTree;

        private DeprecatedNode deprecationNotice;

        public override void OnCreate()
        {
            var attributes = target.GetType().GetCustomAttributes(typeof(DeprecatedNode), false);
            if (attributes.Length != 0)
                deprecationNotice = attributes[0] as DeprecatedNode;

            playFinishedPort = target.GetOutputPort("playFinished");
            serializedObjectTree = new SerializedObjectTree(serializedObject);
        }

        public override void OnBodyGUI()
        {
            if (playFinishedPort == null || !playFinishedPort.node) OnCreate();
            if (deprecationNotice != null)
                EditorGUI.HelpBox(layout.DrawLines(4), deprecationNotice.DeprecationNotice, MessageType.Warning);
        }

        protected virtual bool CanExpand()
        {
            return false;
        }

        public override Color GetTint()
        {
            if (currentColor == Color.white)
                currentColor = style.nodeBackgroundColor;

            FlowNode flowNode = (serializedObject.targetObject as FlowNode);

#if SYMPHONY_DEV
            if (flowNode == null)
                Debug.Log(string.Format("Hey dummy, you forgot to make {0} a FlowNode", serializedObject.targetObject.GetType()));
#endif

            Color32 targetBackgroundColor = (target as FlowNode). isActive ? style.nodeMidLightBackground : style.nodeBackgroundColor;

            Color targetColor = AudioSettings.dspTime - flowNode.lastAccessDSPTime > 0.25? targetBackgroundColor : style.nodeHighlightBackground;

            if (AudioSettings.dspTime - flowNode.lastAccessDSPTime > 0.05)
            {
                currentColor = new Color(
                    Mathf.MoveTowards(currentColor.r, targetColor.r, 0.1f * Time.deltaTime),
                    Mathf.MoveTowards(currentColor.g, targetColor.g, 0.1f * Time.deltaTime),
                    Mathf.MoveTowards(currentColor.b, targetColor.b, 0.1f * Time.deltaTime));
            }
            else
            {
                currentColor = targetColor;
                
            }



            return currentColor;


        }

        protected void DrawEventDerivableProperty(string label, SerializedProperty property, List<GraphEvent.EventParameterDef> parameters, System.Action drawFunction)
        {
            Rect drawRect = EditorGUILayout.GetControlRect(false, 0);
            Rect buttonRect = new Rect(drawRect.x + drawRect.width - EditorGUIUtility.singleLineHeight, drawRect.y + EditorGUIUtility.standardVerticalSpacing, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);

            NodePort targetNode = target.GetInputPort(property.name);

            FlowNode.VariableDrivenByParameter parameter = (target as FlowNode).variablesDrivenByParameters.Find(x => x.serializedPropertyPath == property.propertyPath);

            //clearing parameter if node is connected
            if (targetNode != null && targetNode.IsConnected)
                (target as FlowNode).variablesDrivenByParameters.RemoveAll(x => x.serializedPropertyPath == property.propertyPath);

            if (parameter != null)
            {
                //if (GUI.Button(buttonRect, ""))
                //(target as FlowNode).variablesDrivenByParameters.RemoveAll(x => x.serializedPropertyPath == property.propertyPath);


                GUILayout.BeginHorizontal();

                List<string> potentialParameters = parameters.Where(x => ReflectionUtils.FindType(x.parameterTypeName).Equals(GetTypeByPath(target.GetType(), property.propertyPath))).Select(x => x.parameterName).ToList();

                if (potentialParameters.Count == 0)
                    EditorGUILayout.LabelField(label + ": No available event parameters for type: " + property.type);
                else
                {
                    int currentIndex = potentialParameters.IndexOf(parameter.parameterName);
                    currentIndex = currentIndex < 0 ? 0 : currentIndex;
                    int newIndex = EditorGUILayout.Popup(label, currentIndex, potentialParameters.ToArray());
                    parameter.parameterName = potentialParameters[newIndex];
                }
                GUILayout.Space(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                GUILayout.EndHorizontal();
            }
            else
            {
                if (targetNode == null || !targetNode.IsConnected)
                {
                    //if (GUI.Button(buttonRect, ""))
                    //(target as FlowNode).variablesDrivenByParameters.Add(new FlowNode.VariableDrivenByParameter(property.propertyPath, ""));
                }

                GUILayout.BeginHorizontal();
                drawFunction?.Invoke();
                GUILayout.Space(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                GUILayout.EndHorizontal();
            }
            LayersGUIUtilities.DrawDropdown(buttonRect, "", new string[] { "Get value from port", "Get value from event parameter" }, true, (value) => { 
                if (value == 0)
                    (target as FlowNode).variablesDrivenByParameters.RemoveAll(x => x.serializedPropertyPath == property.propertyPath);
                else
                    (target as FlowNode).variablesDrivenByParameters.Add(new FlowNode.VariableDrivenByParameter(property.propertyPath, ""));
            });
        }

        protected void DrawEventDerivableProperty(Rect drawRect,SerializedProperty property, List<GraphEvent.EventParameterDef> parameters, System.Action<Rect> drawFunction)
        {
            DrawEventDerivableProperty(drawRect, property.displayName, property, parameters, drawFunction);
        }

        protected void DrawEventDerivableProperty(Rect drawRect, string label, 
            SerializedProperty property, List<GraphEvent.EventParameterDef> parameters, System.Action<Rect> drawFunction)
        {
            Rect buttonRect = new Rect(drawRect.x + drawRect.width - EditorGUIUtility.singleLineHeight, drawRect.y + EditorGUIUtility.standardVerticalSpacing, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);

            NodePort targetNode = target.GetInputPort(property.name);

            FlowNode.VariableDrivenByParameter parameter = (target as FlowNode).variablesDrivenByParameters.Find(x => x.serializedPropertyPath == property.propertyPath);

            //clearing parameter if node is connected
            if (targetNode != null && targetNode.IsConnected)
                (target as FlowNode).variablesDrivenByParameters.RemoveAll(x => x.serializedPropertyPath == property.propertyPath);

            if (parameter != null)
            {
                //if (GUI.Button(buttonRect, ""))
                //(target as FlowNode).variablesDrivenByParameters.RemoveAll(x => x.serializedPropertyPath == property.propertyPath);



                List<string> potentialParameters = parameters.Where(x => ReflectionUtils.FindType(x.parameterTypeName).Equals(GetTypeByPath(target.GetType(), property.propertyPath))).Select(x => x.parameterName).ToList();

                if (potentialParameters.Count == 0)
                    EditorGUI.LabelField(drawRect, label + ": No available event parameters for type: " + property.type);
                else
                {
                    int currentIndex = potentialParameters.IndexOf(parameter.parameterName);
                    currentIndex = currentIndex < 0 ? 0 : currentIndex;
                    int newIndex = EditorGUI.Popup(drawRect, label, currentIndex, potentialParameters.ToArray());
                    parameter.parameterName = potentialParameters[newIndex];
                }
                
            }
            else
            {
                if (targetNode == null || !targetNode.IsConnected)
                {
                    //if (GUI.Button(buttonRect, ""))
                    //(target as FlowNode).variablesDrivenByParameters.Add(new FlowNode.VariableDrivenByParameter(property.propertyPath, ""));
                }

                drawFunction?.Invoke(new Rect(drawRect.x, drawRect.y, drawRect.width -16, drawRect.height));
            }
            LayersGUIUtilities.DrawDropdown( buttonRect, "", new string[] { "Get value from port", "Get value from event parameter" }, true, (value) => {
                if (value == 0)
                    (target as FlowNode).variablesDrivenByParameters.RemoveAll(x => x.serializedPropertyPath == property.propertyPath);
                else
                    (target as FlowNode).variablesDrivenByParameters.Add(new FlowNode.VariableDrivenByParameter(property.propertyPath, ""));
            });
        }

        public override void OnHeaderGUI()
        {
        

            Rect headerRect = EditorGUILayout.GetControlRect(false, 0f);
            Rect buttonRect = new Rect(headerRect.x + headerRect.width - EditorGUIUtility.singleLineHeight, headerRect.y + 5, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
            GUILayout.Label(target.name, style.headerLabelStyle, GUILayout.Height(30));
            if (!CanExpand())
                return;
            SerializedProperty expandedProp = serializedObject.FindProperty("expanded");
            if (NodeEditorGUIDraw.ImageButton(buttonRect, expandedProp.boolValue ? minimizeTexture : maximizeTexture))
            {
                serializedObject.UpdateIfRequiredOrScript();
                expandedProp.boolValue = !expandedProp.boolValue;
                serializedObject.ApplyModifiedProperties();
            }
        }


        protected void DrawEventDerivableProperty(SerializedProperty property, List<GraphEvent.EventParameterDef> parameters, System.Action drawFunction)
        {

            DrawEventDerivableProperty(property.displayName, property, parameters, drawFunction);

        
        }

        

        protected void FastPortPair(GUIContent inputLabel, NodePort input, GUIContent outputLabel, NodePort output)
        {
            Rect controlRect = EditorGUILayout.GetControlRect();
            Rect leftRect = new Rect(controlRect.x, controlRect.y, controlRect.width / 2f, controlRect.height);
            Rect rightRect = new Rect(controlRect.x + controlRect.width / 2f, controlRect.y, controlRect.width / 2f, controlRect.height);
            EditorGUI.LabelField(leftRect, inputLabel);
            EditorGUI.LabelField(rightRect, outputLabel);
            NodeEditorGUILayout.PortField(new Vector2(leftRect.x, leftRect.y), input);
            NodeEditorGUILayout.PortField(new Vector2(rightRect.x + rightRect.width, rightRect.y), output);
        }

        public static System.Type GetTypeByPath(System.Type parentType, string path)
        {
            System.Reflection.FieldInfo currentFieldInfo = parentType.GetField(path, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
            return currentFieldInfo.FieldType;
        }

        public void DrawAudioOutSelector(Rect area, GUIContent title, NodePort audioPort, SerializedProperty selectedAudioSettingsID, float rightMargin)
        {
            Rect buttonRect = new Rect(area.x, area.y, area.width - rightMargin, area.height);
            if (audioPort.IsConnected)
            {
                Rect labelRect = new Rect(area.x + 13, area.y, area.width - rightMargin, area.height);
                EditorGUI.LabelField(labelRect, title, LayersGUIUtilities.rightAlignDropDownStyle);
            }
            else
            {
                AudioOut[] audioOuts = (target as FlowNode).soundGraph.GetNodesOfType<AudioOut>();
                int oldSelectionIndex = audioOuts.ToList().FindIndex(x => x.nodeID.ToString() == selectedAudioSettingsID.stringValue);
                string[] options = audioOuts.Select(x => "Send to: " + x.name).Prepend("Don't Send").ToArray();
                string displayName = oldSelectionIndex == -1? title.text: "Send->" + audioOuts[oldSelectionIndex].name;

                if (oldSelectionIndex != -1 && !audioOuts[oldSelectionIndex].sendList.Contains(target))
                    SetUpAudioOuts(oldSelectionIndex, oldSelectionIndex, audioOuts, selectedAudioSettingsID);//This happens on copy and paste of nodes

                LayersGUIUtilities.DrawDropdown(buttonRect, displayName, options, true, (value) => {
                    SetUpAudioOuts(oldSelectionIndex, value, audioOuts, selectedAudioSettingsID);
                });
            }

            if (string.IsNullOrEmpty(selectedAudioSettingsID.stringValue))
            {
                Vector2 portPosition = new Vector2(area.x + area.width, area.y);
                NodeEditorGUILayout.PortField(portPosition, audioPort);
            }


        }

        private void SetUpAudioOuts(int oldSelectionIndex, int newSelectionIndex, AudioOut[] audioOuts, SerializedProperty selectedAudioSettingsID)
        {
            List<Object> undoObjects = new List<Object>();

            AudioOut previousAudioOut = oldSelectionIndex >= 0 ? audioOuts[oldSelectionIndex] : null;
            if (previousAudioOut != null) undoObjects.Add(previousAudioOut);

            AudioOut nextAudioOut = newSelectionIndex != 0 ? audioOuts[newSelectionIndex - 1] : null;
            if (nextAudioOut != null) undoObjects.Add(nextAudioOut);

            undoObjects.Add(serializedObject.targetObject);

            Undo.RecordObjects(undoObjects.ToArray(), "Changed Audio Send");

            previousAudioOut?.sendList.Remove(target as FlowNode);
            nextAudioOut?.sendList.Add(target as FlowNode);

            if (newSelectionIndex != 0)
            {
                selectedAudioSettingsID.stringValue = audioOuts[newSelectionIndex - 1].nodeID.ToString();
            }
            else
            {
                selectedAudioSettingsID.stringValue = "";
            }
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    
        public void MarkNeedsCodeRegen()
        {
            LayersSettings.GetOrCreateSettings().MarkSoundGraphAsChanged((target as FlowNode).soundGraph);
        }
    }
}
