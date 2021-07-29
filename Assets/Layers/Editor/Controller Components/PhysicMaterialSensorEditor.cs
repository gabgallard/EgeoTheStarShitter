using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Sound_graph_players;
using ABXY.Layers.ThirdParty.Malee.List;

namespace ABXY.Layers.Editor
{
    [CustomEditor(typeof(PhysicMaterialSensor))]
    public class PhysicMaterialSensorEditor : UnityEditor.Editor
    {
        ReorderableList reorderableList;
        SerializedObjectTree serializedObjectTree;
        private void OnEnable()
        {
            serializedObjectTree = new SerializedObjectTree(serializedObject);
            reorderableList = new ReorderableList(serializedObjectTree.FindProperty("tagList"));
            reorderableList.drawElementCallback += OnDrawTag;
        }

        private void OnDrawTag(Rect rect, SerializedPropertyTree element, GUIContent label, bool selected, bool focused)
        {
            element.stringValue = EditorGUI.TagField(rect, element.stringValue);
        }

        public override void OnInspectorGUI()
        {
            serializedObjectTree.UpdateIfRequiredOrScript();
            PlayerBase playerbase = (target as PhysicMaterialSensor).player;

            if (playerbase == null)
                return;

            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            LayersGUIUtilities.DrawVariableSelector(serializedObjectTree.FindProperty("physicMaterialPropertyID"), "Target Variable", playerbase.soundGraph,
                (variable) => {
                    return variable.typeName == typeof(PhysicMaterial).FullName; 
                });

            EditorGUILayout.PropertyField(serializedObjectTree.FindProperty("raycastStartPosition"));
            EditorGUILayout.PropertyField(serializedObjectTree.FindProperty("raycastDirection"));
            EditorGUILayout.PropertyField(serializedObjectTree.FindProperty("raycastDistance"));
            EditorGUILayout.PropertyField(serializedObjectTree.FindProperty("raycastSpace"));

            SerializedProperty tagStyle = serializedObjectTree.FindProperty("tagStyle");
            EditorGUILayout.PropertyField(tagStyle, new GUIContent("Raycast Tags"));

            if (tagStyle.enumValueIndex != ((int)PhysicMaterialSensor.TagStyles.AllTags))
            {
                reorderableList.DoLayoutList();
            }


            EditorGUI.EndDisabledGroup();

            serializedObjectTree.ApplyModifiedProperties();

            if (Application.isPlaying)
                EditorGUILayout.LabelField($"Current Physic Material: {(target as PhysicMaterialSensor).targetPhysicMaterial}");
        }
    }
}