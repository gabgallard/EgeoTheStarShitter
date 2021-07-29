using ABXY.Layers.Editor;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Sound_graph_players;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor
{
    //[CustomEditor(typeof(CollisionAudioController))]
    /**
    public class CollisionAudioControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            PlayerBase playerbase = (target as CollisionAudioController).player;

            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            LayersGUIUtilities.DrawBox(2, "On Collision Enter", () => {
                LayersGUIUtilities.DrawEventSelector(new GUIContent("Event"), serializedObject.FindProperty("onCollisionStartEventID"), playerbase, true);
                LayersGUIUtilities.DrawParameterSelector(new GUIContent("Parameter"), serializedObject.FindProperty("onCollisionStartEventID"),
                    serializedObject.FindProperty("onCollisionStartParameterID"), playerbase, true, typeof(Collision));
            });

            LayersGUIUtilities.DrawBox(2, "While Colliding", () => {
                LayersGUIUtilities.DrawEventSelector(new GUIContent("Event"), serializedObject.FindProperty("whileCollidingEventID"), playerbase, true);
                LayersGUIUtilities.DrawParameterSelector(new GUIContent("Parameter"), serializedObject.FindProperty("whileCollidingEventID"),
                    serializedObject.FindProperty("whileCollidingParameterID"), playerbase, true, typeof(Collision));
            });


            LayersGUIUtilities.DrawBox(2, "On Collision End", () => {
                LayersGUIUtilities.DrawEventSelector(new GUIContent("Event"), serializedObject.FindProperty("onCollisionEndEventID"), playerbase, true);
                LayersGUIUtilities.DrawParameterSelector(new GUIContent("Parameter"), serializedObject.FindProperty("onCollisionEndEventID"),
                    serializedObject.FindProperty("onCollisionEndParameterID"), playerbase, true, typeof(Collision));
            });

            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }

        
    }*/
}