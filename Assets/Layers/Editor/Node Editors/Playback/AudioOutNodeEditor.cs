using System.Collections.Generic;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Editor.Curve_Editor;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Curves;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Playback;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace ABXY.Layers.Editor.Node_Editors.Playback
{
    [NodeEditor.CustomNodeEditor(typeof(AudioOut))]
    public class AudioOutNodeEditor: FlowNodeEditor
    {
        private NodePort volumeNode;

        private NodePort mixerGroupNode;

        private NodePort bypassEffectsNode;

        private NodePort bypassListenerNode;

        private NodePort bypassReverbNode;

        private NodePort priorityPropNode;

        private NodePort pitchPropNode;

        private NodePort stereoPanNode;

        private NodePort spatialBlendPropNode;
        private NodePort spatialBlendCurveNode;

        private NodePort reverbZoneMixNode;
        private NodePort reverbZoneMixCurveNode;

        private NodePort spreadNode;
        private NodePort spreadCurveNode;


        private NodePort worldPositionNode;

        private NodePort transformNode;

        private NodePort audioSettingsPort;

        private NodePort dopplerLevelPort;

        // curve editor stuff
        private CurveEditor audioCurveEditor;
        SerializedProperty minDistance;
        SerializedProperty maxDistance;

        private bool curvesExpanded = false;


        public override void OnCreate()
        {
            base.OnCreate();

            (target as AudioOut).Migrate();
            serializedObject.Update();

            SerializedPropertyTree volumeFalloffCurve = serializedObject.FindProperty("volumeFalloffCurve");
            SerializedPropertyTree rolloffModeProp = serializedObject.FindProperty("rolloffMode");


            volumeNode = target.GetInputPort("volume");

            mixerGroupNode = target.GetInputPort("audioMixerGroup");

            bypassEffectsNode = target.GetInputPort("bypassEffects");

            bypassListenerNode = target.GetInputPort("bypassListener");

            bypassReverbNode = target.GetInputPort("bypassReverb");

            priorityPropNode = target.GetInputPort("priority");

            pitchPropNode = target.GetInputPort("pitch");

            stereoPanNode = target.GetInputPort("stereoPan");

            spatialBlendPropNode = target.GetInputPort("spatialBlend");
            spatialBlendCurveNode = target.GetInputPort("spatialCurve");

            reverbZoneMixNode = target.GetInputPort("reverbZoneMix");
            reverbZoneMixCurveNode = target.GetInputPort("reverbZoneMixCurve");

            worldPositionNode = target.GetInputPort("worldPosition");

            transformNode = target.GetInputPort("playAtTransform");

            spreadNode = target.GetInputPort("spread");
            spreadCurveNode = target.GetInputPort("spreadCurve");


            audioSettingsPort = target.GetInputPort("audioSettings");

            dopplerLevelPort = target.GetInputPort("dopplerLevel");

            // Setting up curve editor
            minDistance = serializedObject.FindProperty("minDistance");
            maxDistance = serializedObject.FindProperty("maxDistance");
            audioCurveEditor = new CurveEditor();
            audioCurveEditor.SetupHorizontalAxis(new Color(0.0f, 0.0f, 0.0f, 0.15f), new Color(0.0f, 0.0f, 0.0f, 0.15f), distLabel: 30);
            audioCurveEditor.SetupVerticalAxis(new Color(0.0f, 0.0f, 0.0f, 0.15f), new Color(0.0f, 0.0f, 0.0f, 0.15f), distLabel: 20);
            audioCurveEditor.SetMargins(25);
            audioCurveEditor.showHorizontalSlider = false;
            audioCurveEditor.showVerticalSlider = false;
            audioCurveEditor.SetHorizontalRange(0f, 1f);
            audioCurveEditor.SetVerticalRange(0f, 1.1f);
            audioCurveEditor.ignoreScrollWheelUntilClicked = true;
            audioCurveEditor.AddCurve(serializedObject.FindProperty("reverbZoneMixCurve"), Color.yellow, "Reverb");
            audioCurveEditor.AddCurve(serializedObject.FindProperty("spreadCurve"), Color.blue, "Spread");
            audioCurveEditor.AddCurve(serializedObject.FindProperty("spatialCurve"), Color.green, "Spatial");
            audioCurveEditor.AddCurve(volumeFalloffCurve, Color.red, "Volume");
            audioCurveEditor.onCurveChanged += (SerializedProperty curveProp) =>
            {
                if (curveProp.propertyPath == volumeFalloffCurve.propertyPath)
                    rolloffModeProp.enumValueIndex = (int)AudioRolloffMode.Custom;
            };
        }


        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();


            SerializedPropertyTree volumeProp = serializedObject.FindProperty("volume");
            SerializedPropertyTree mixerGroupProp = serializedObject.FindProperty("audioMixerGroup");
            SerializedPropertyTree bypassEffectsProp = serializedObject.FindProperty("bypassEffects");
            SerializedPropertyTree bypassListenerProp = serializedObject.FindProperty("bypassListener");
            SerializedPropertyTree bypassReverbProp = serializedObject.FindProperty("bypassReverb");
            SerializedPropertyTree priorityProp = serializedObject.FindProperty("priority");
            SerializedPropertyTree pitchProp = serializedObject.FindProperty("pitch");
            SerializedPropertyTree stereoPanProp = serializedObject.FindProperty("stereoPan");
            SerializedPropertyTree spatialBlendProp = serializedObject.FindProperty("spatialBlend");
            SerializedPropertyTree spatialBlendCurveProp = serializedObject.FindProperty("spatialCurve");
            SerializedPropertyTree reverbZoneMixProp = serializedObject.FindProperty("reverbZoneMix");
            SerializedPropertyTree reverbZoneMixCurve = serializedObject.FindProperty("reverbZoneMixCurve");
            SerializedPropertyTree worldPositionProp = serializedObject.FindProperty("worldPosition");
            SerializedPropertyTree transformProp = serializedObject.FindProperty("playAtTransform");
            SerializedPropertyTree spreadProperty = serializedObject.FindProperty("spread");
            SerializedPropertyTree spreadCurveProperty = serializedObject.FindProperty("spreadCurve");
            SerializedPropertyTree volumeFalloffCurve = serializedObject.FindProperty("volumeFalloffCurve");
            SerializedPropertyTree settingsSourceProp = serializedObject.FindProperty("settingsSource");
            SerializedPropertyTree shareSettingsProp = serializedObject.FindProperty("shareSettings");
            SerializedPropertyTree rolloffModeProp = serializedObject.FindProperty("rolloffMode");
            SerializedPropertyTree dopplerLevelProp = serializedObject.FindProperty("dopplerLevel");


            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 160;

            int sendCount = (target as AudioOut).sendList.Count;
            NodeEditorGUIDraw.PortField(
                layout.DrawLine(), 
                new GUIContent(string.Format("Audio In  ({0} Send{1})", sendCount, sendCount == 1?"":"s")),
                target.GetInputPort("audioIn"), serializedObjectTree);

            LayersGUIUtilities.DrawExpandableProperty(serializedObject,() =>
            {
                LayersGUIUtilities.DrawDropdown(layout.DrawLine(), new GUIContent("Settings Source"),settingsSourceProp, false);
                //EditorGUILayout.PropertyField(settingsSourceProp);
            });

            /*LayersGUIUtilities.DrawExpandableProperty(serializedObject,() =>
            {
                LayersGUIUtilities.DrawDropdown(layout.DrawLine(), new GUIContent("Share settings"), shareSettingsProp, false);
                //EditorGUILayout.PropertyField(shareSettingsProp);
            });*/

            if (settingsSourceProp.enumValueIndex == (int)AudioOut.SettingsSources.Input)
                NodeEditorGUIDraw.PortField(layout.DrawLine(), audioSettingsPort, serializedObjectTree);
            else
                audioSettingsPort.ClearConnections();

            List<GraphEvent.EventParameterDef> parameters = (target as FlowNode).GetIncomingEventParameterDefsOnPort("audioIn", new List<Node>(new Node[] { target}));

            if (settingsSourceProp.enumValueIndex == (int)AudioOut.SettingsSources.Node)
            {

                LayersGUIUtilities.DrawExpandableProperty(volumeNode, serializedObject, () => {
                    DrawEventDerivablePropertyWithInstanceOption(layout.DrawLine(), volumeProp, parameters, (controlRect) => {
                        NodeEditorGUIDraw.PropertyField(controlRect, volumeProp, GetInstanceDerivedLabel(volumeProp));
                    });
                });

                LayersGUIUtilities.DrawExpandableProperty( pitchPropNode, serializedObject, () =>
                {
                    DrawEventDerivablePropertyWithInstanceOption(layout.DrawLine(), pitchProp, parameters, (controlRect) =>
                    {
                        NodeEditorGUIDraw.PropertyField(controlRect,pitchProp, GetInstanceDerivedLabel(pitchProp));
                    });
                });

                LayersGUIUtilities.DrawExpandableProperty(stereoPanNode, serializedObject, () =>
                {
                    DrawEventDerivablePropertyWithInstanceOption(layout.DrawLine(), stereoPanProp, parameters, (controlRect) =>
                    {
                        NodeEditorGUIDraw.PropertyField(controlRect,stereoPanProp, GetInstanceDerivedLabel(stereoPanProp));
                    });
                });

                LayersGUIUtilities.DrawExpandableProperty(mixerGroupNode, serializedObject, () =>
                {
                    DrawEventDerivablePropertyWithInstanceOption(layout.DrawLine(), mixerGroupProp, parameters, (controlRect) =>
                    {
                        NodeEditorGUIDraw.PropertyField(controlRect, mixerGroupProp, GetInstanceDerivedLabel(mixerGroupProp));
                    });
                });

                float currentLabelWidth = 0;

                LayersGUIUtilities.DrawExpandableProperty(bypassEffectsNode, serializedObject, () =>
                {
                    DrawEventDerivablePropertyWithInstanceOption(layout.DrawLine(), bypassEffectsProp, parameters, (controlRect) =>
                    {
                        currentLabelWidth = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = 237;
                        NodeEditorGUIDraw.PropertyField(controlRect,bypassEffectsProp, GetInstanceDerivedLabel(bypassEffectsProp));
                        EditorGUIUtility.labelWidth = currentLabelWidth;
                    });
                });

                LayersGUIUtilities.DrawExpandableProperty(bypassListenerNode, serializedObject, () =>
                {
                    DrawEventDerivablePropertyWithInstanceOption(layout.DrawLine(), bypassListenerProp, parameters, (controlRect) =>
                    {
                        currentLabelWidth = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = 237;
                        NodeEditorGUIDraw.PropertyField(controlRect,bypassListenerProp, GetInstanceDerivedLabel(bypassListenerProp));
                        EditorGUIUtility.labelWidth = currentLabelWidth;
                    });
                });

                LayersGUIUtilities.DrawExpandableProperty(bypassReverbNode, serializedObject, () =>
                {
                    DrawEventDerivablePropertyWithInstanceOption(layout.DrawLine(), bypassReverbProp, parameters, (controlRect) =>
                    {
                        currentLabelWidth = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = 237;
                        NodeEditorGUIDraw.PropertyField(controlRect,bypassReverbProp, GetInstanceDerivedLabel(bypassReverbProp));
                        EditorGUIUtility.labelWidth = currentLabelWidth;
                    });
                });

                LayersGUIUtilities.DrawExpandableProperty(priorityPropNode, serializedObject, () =>
                {
                    DrawEventDerivablePropertyWithInstanceOption(layout.DrawLine(), priorityProp, parameters, (controlRect) =>
                    {
                        NodeEditorGUIDraw.PropertyField(controlRect,priorityProp, GetInstanceDerivedLabel(priorityProp));
                    });
                });

            
                bool worldPositionAssigned = worldPositionNode.IsConnected || (target as FlowNode).variablesDrivenByParameters.Find(x => x.serializedPropertyPath == worldPositionProp.propertyPath) != null;
                bool transformAssigned = transformNode.IsConnected || (target as FlowNode).variablesDrivenByParameters.Find(x => x.serializedPropertyPath == transformProp.propertyPath) != null;

                if (!transformAssigned)
                {
                    LayersGUIUtilities.DrawExpandableProperty(worldPositionNode, serializedObject, () =>
                    {
                        DrawEventDerivablePropertyWithInstanceOption(layout.DrawLines(2), worldPositionProp, parameters, (controlRect) =>
                        {
                            NodeEditorGUIDraw.PropertyField(controlRect, worldPositionProp, GetInstanceDerivedLabel(worldPositionProp));
                        });
                    });
                }
                else
                    worldPositionNode.ClearConnections();

                if (!worldPositionAssigned)
                {
                    LayersGUIUtilities.DrawExpandableProperty(transformNode, serializedObject, () =>
                    {
                        DrawEventDerivablePropertyWithInstanceOption(layout.DrawLine(), transformProp, parameters, (controlRect) =>
                        {
                            NodeEditorGUIDraw.PropertyField(controlRect, transformProp, GetInstanceDerivedLabel(transformProp));
                        });
                    });
                }
                else
                    transformNode.ClearConnections();


                LayersGUIUtilities.DrawExpandableProperty(dopplerLevelPort, serializedObject, () =>
                {
                    DrawEventDerivablePropertyWithInstanceOption(layout.DrawLine(), dopplerLevelProp, parameters, (controlRect) =>
                    {
                        NodeEditorGUIDraw.PropertyField(controlRect, dopplerLevelProp, GetInstanceDerivedLabel(dopplerLevelProp));
                    });
                });


                layout.DrawLine();

                LayersGUIUtilities.DrawExpandableProperty(reverbZoneMixNode.IsConnected || reverbZoneMixCurveNode.IsConnected, serializedObject, () =>
                {
                    DrawLinearAudioProperty("Reverb Zone Mix", reverbZoneMixCurve, reverbZoneMixProp, 0f, 1.1f, false, parameters);
                });

                LayersGUIUtilities.DrawExpandableProperty(spatialBlendPropNode.IsConnected || spatialBlendCurveNode.IsConnected, serializedObject, () =>
                {
                    DrawLinearAudioProperty("Spatial Blend", spatialBlendCurveProp, spatialBlendProp, 0f, 1f, false, parameters);
                });


                LayersGUIUtilities.DrawExpandableProperty(spreadCurveNode.IsConnected || spreadNode.IsConnected, serializedObject, () =>
                {
                    DrawLinearAudioProperty("Spread", spreadCurveProperty, spreadProperty, 0f, 360f, true, parameters);
                });

                DrawRollOff(parameters);

                // doing rolloff curve setup
                if (((AudioRolloffMode)rolloffModeProp.enumValueIndex) == AudioRolloffMode.Logarithmic)
                {
                    AnimationCurve logCurve = AnimationCurveUtils.LogarithmicCurve(minDistance.floatValue / maxDistance.floatValue, 1f, 1f);
                    AnimationCurve curve = volumeFalloffCurve.animationCurveValue;
                    curve.keys = logCurve.keys;
                    volumeFalloffCurve.animationCurveValue = curve;
                }
                else if (((AudioRolloffMode)rolloffModeProp.enumValueIndex) == AudioRolloffMode.Linear)
                {
                    AnimationCurve linearCurve = AnimationCurveUtils.Line(Vector2.up, Vector2.right);
                    AnimationCurve curve = volumeFalloffCurve.animationCurveValue;
                    curve.keys = linearCurve.keys;
                    volumeFalloffCurve.animationCurveValue = curve;
                }


                LayersGUIUtilities.DrawExpandableProperty(serializedObject, () =>
                {
                    curvesExpanded = EditorGUI.Foldout(layout.DrawLine(), curvesExpanded, "3D Audio", true);
                    if (curvesExpanded)
                    {
                        audioCurveEditor.Draw(layout.Draw(300f / 1.5f));
                    }
                });

            }
            else
            {
                volumeNode.ClearConnections();
                bypassEffectsNode.ClearConnections();
                bypassListenerNode.ClearConnections();
                bypassReverbNode.ClearConnections();
                pitchPropNode.ClearConnections();
                stereoPanNode.ClearConnections();
                spatialBlendPropNode.ClearConnections();
                reverbZoneMixNode.ClearConnections();
                mixerGroupNode.ClearConnections();
                priorityPropNode.ClearConnections();
                worldPositionNode.ClearConnections();
            }

            EditorGUIUtility.labelWidth = labelWidth;
            serializedObject.ApplyModifiedProperties();
        }


        protected GUIContent GetInstanceDerivedLabel( SerializedProperty property)
        {
            bool instanced = serializedObject.FindProperty(property.name + "Instance").boolValue;
            string addendum = instanced ? " (Event Start)" : " (Continuous)";
            GUIContent label = new GUIContent(property.displayName + addendum);
            return label;

        }

        protected void DrawEventDerivablePropertyWithInstanceOption(Rect drawRect, SerializedProperty property, List<GraphEvent.EventParameterDef> parameters, System.Action<Rect> drawFunction)
        {
            DrawEventDerivablePropertyWithInstanceOption(drawRect, property.displayName, property, parameters, drawFunction);
        }


        protected void DrawEventDerivablePropertyWithInstanceOption(Rect drawRect, string label,
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

                drawFunction?.Invoke(new Rect(drawRect.x, drawRect.y, drawRect.width - 16, drawRect.height));
            }
            LayersGUIUtilities.DrawDropdown(buttonRect, "", new string[] { "Value input/Get value from port", "Value input/Get value from event parameter", "Read settings/Read continuously", "Read settings/Read at event start" }, true, (value) => {
                if (value == 0)
                    (target as FlowNode).variablesDrivenByParameters.RemoveAll(x => x.serializedPropertyPath == property.propertyPath);
                else if (value == 1)
                    (target as FlowNode).variablesDrivenByParameters.Add(new FlowNode.VariableDrivenByParameter(property.propertyPath, ""));
                else if (value == 2)
                {
                    serializedObject.FindProperty(property.name + "Instance").boolValue = false;
                    serializedObject.ApplyModifiedProperties();
                }
                else if (value == 3)
                {
                    serializedObject.FindProperty(property.name + "Instance").boolValue = true;
                    serializedObject.ApplyModifiedProperties();
                }
            });
        }



        private void DrawLinearAudioProperty(string label, SerializedProperty curveProperty, SerializedProperty floatProperty, float min, float max, bool useLerp, List<GraphEvent.EventParameterDef> parameters)
        {
            AudioOut castTarget = target as AudioOut;

            //EditorGUILayout.Space();

            //getting info
            NodePort curvePort = target.GetInputPort(curveProperty.name);
            NodePort floatPort = target.GetInputPort(floatProperty.name);

            bool curvePortParameterExists = castTarget.variablesDrivenByParameters.Find(x => x.serializedPropertyPath == curveProperty.propertyPath) != null;
            bool floatPortParameterExists = castTarget.variablesDrivenByParameters.Find(x => x.serializedPropertyPath == floatProperty.propertyPath) != null;


            AnimationCurve curve = curveProperty.animationCurveValue;
            bool isLinear = curve.keys.Length == 1 && curve.keys[0].inTangent == 0 && curve.keys[0].outTangent == 0;


            int height = ((curvePort.IsConnected || curvePortParameterExists ? 0:1) + (floatPort.IsConnected || floatPortParameterExists ? 0 : 1) ) * (isLinear?1:0);
            Rect propertyEditorRect = layout.DrawLine();
        
            Rect propertyBgRect = new Rect(propertyEditorRect.x, propertyEditorRect.y, propertyEditorRect.width, EditorGUIUtility.singleLineHeight * (height + 1) + EditorGUIUtility.standardVerticalSpacing *2f* (height + 2));
            GUI.Box(propertyBgRect, "", GUI.skin.window);

      

            if (isLinear && !curvePort.IsConnected && !floatPort.IsConnected && !floatPortParameterExists && !curvePortParameterExists)
            {
                EditorGUI.BeginChangeCheck();
                Keyframe keyFrame = curve.keys[0];

                float originalValue = useLerp ? Mathf.Lerp(min, max, keyFrame.value) : keyFrame.value;
                float newValue = EditorGUI.Slider(propertyEditorRect, "  "+label, originalValue, min, max);
                newValue = useLerp ? Mathf.InverseLerp(min, max, newValue) : newValue;

                //clamping
                newValue = useLerp ? Mathf.Clamp(newValue, min, max) : Mathf.Clamp01(newValue);

                if (EditorGUI.EndChangeCheck())
                {
                    keyFrame.value = newValue;
                    keyFrame.time = 0f;
                    curve.MoveKey(0, keyFrame);
                    curveProperty.animationCurveValue = curve;
                }
            }
            else if (!isLinear && !curvePort.IsConnected && !floatPort.IsConnected)
            {
                EditorGUI.LabelField(propertyEditorRect, "  "+label + ": Controlled by curve");
            }
            else
            {
                EditorGUI.LabelField(propertyEditorRect, "  " + label);
            }




            // Showing curve port
            if (!floatPort.IsConnected && isLinear && !floatPortParameterExists)
            {
                // drawing curve port property
                DrawEventDerivableProperty(layout.DrawLine(), "  As curve", curveProperty, parameters, (controlRect) => {
                    NodeEditorGUIDraw.PortField(controlRect, new GUIContent("  As curve"), curvePort, serializedObjectTree);
                });

                // setting graph vis
                audioCurveEditor.SetCurveReadOnly(curveProperty, curvePortParameterExists);
                audioCurveEditor.SetCurveVisibility(curveProperty, !curvePortParameterExists);

                //writing new data
                if (!curvePortParameterExists && curvePort.IsConnected)
                {


                    AnimationCurve newCurve = target.GetInputValue<AnimationCurve>(curveProperty.name, null);

                    if (newCurve != null)
                    {
                        curve.keys = newCurve.keys;
                        curveProperty.animationCurveValue = curve;
                    }
                }

            }

            // Showing float port
            if (!curvePort.IsConnected && isLinear && !curvePortParameterExists)
            {
                // drawing float port property
                DrawEventDerivableProperty(layout.DrawLine(), "  As Float",floatProperty, parameters, (controlRect) => {
                    NodeEditorGUIDraw.PortField(controlRect, new GUIContent("  As Float"), floatPort, serializedObjectTree);
                });

                audioCurveEditor.SetCurveVisibility(curveProperty, !floatPortParameterExists);

                //writing new data
                if (!floatPortParameterExists && floatPort.IsConnected)
                {
                    Keyframe keyFrame = curve.keys[0];
                    float valueFromCurve = Mathf.Lerp(min, max, keyFrame.value);
                    float newFloat = target.GetInputValue<float>(floatProperty.name, valueFromCurve);
                    newFloat = Mathf.Clamp(newFloat, min, max);
                    keyFrame.value = newFloat;
                    keyFrame.time = 0f;
                    curve.MoveKey(0, keyFrame);
                    curveProperty.animationCurveValue = curve;
                
                }
            }
            layout.DrawLine();


        }

        private void DrawRollOff(List<GraphEvent.EventParameterDef> parameters)
        {
            SerializedPropertyTree rolloffModeProp = serializedObject.FindProperty("rolloffMode");
            LayersGUIUtilities.DrawExpandableProperty(target.GetInputPort("minDistance").IsConnected && target.GetInputPort("maxDistance").IsConnected, serializedObject, () => {
                int height = 3;
                Rect propertyEditorRect = layout.DrawLine();

                Rect propertyBgRect = new Rect(propertyEditorRect.x, propertyEditorRect.y, propertyEditorRect.width, EditorGUIUtility.singleLineHeight * (height + 1) + EditorGUIUtility.standardVerticalSpacing * 2f * (height + 2));
                GUI.Box(propertyBgRect, "Volume Rolloff Settings", GUI.skin.window);


                DrawEventDerivableProperty(layout.DrawLine(), minDistance, parameters, (controlRect) =>
                {
                    NodeEditorGUIDraw.PropertyField(controlRect, minDistance, new GUIContent("  Min Distance"));
                });


                DrawEventDerivableProperty(layout.DrawLine(),maxDistance, parameters, (controlRect) => {
                    NodeEditorGUIDraw.PropertyField(controlRect, maxDistance, new GUIContent("  Max Distance"));
                });

                LayersGUIUtilities. DrawExpandableProperty(serializedObject, () => {
                    LayersGUIUtilities.DrawDropdown(layout.DrawLine(), new GUIContent("  Rolloff Mode"), rolloffModeProp, false);
                    //EditorGUILayout.PropertyField(rolloffModeProp, new GUIContent("  Rolloff Mode"));
                });
                layout.DrawLine();
            });

        
        }
        public override int GetWidth()
        {
            return 300;
        }

        protected override bool CanExpand()
        {
            return true;
        }
    }
}
