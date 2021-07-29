using UnityEditor;

//[CustomPropertyDrawer(typeof(GraphVariable))]
namespace ABXY.Layers.Editor
{
    public class GraphVariableProperty : PropertyDrawer
    {
    
        //TODO: Remove this node
        /*
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        EditorGUIUtility.labelWidth = 0f;
        Rect nameRect = new Rect(position.x, position.y + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"),new GUIContent(""));

        Rect typeRect = new Rect(position.x, position.y + 2 * EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight,
            position.width / 2f, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("varType"), new GUIContent(""));

        Rect propRect = new Rect(position.x + (position.width / 2f) , position.y + 2*EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight, 
            position.width / 2f, EditorGUIUtility.singleLineHeight);
        DrawProp(propRect, property, (GraphVariable.varTypes)property.FindPropertyRelative("varType").enumValueIndex);

        Rect exposeRect = new Rect(position.x, position.y + 3 * EditorGUIUtility.standardVerticalSpacing + 2f* EditorGUIUtility.singleLineHeight,
            position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(exposeRect, property.FindPropertyRelative("expose"));

        Rect complexRect = new Rect(position.x, position.y + 4 * EditorGUIUtility.standardVerticalSpacing + 3f * EditorGUIUtility.singleLineHeight, position.width, 0f);
    }

    private void DrawProp(Rect position, SerializedProperty sp, GraphVariable.varTypes type)
    {
        switch (type)
        {
            case GraphVariable.varTypes.Int:
                EditorGUI.PropertyField(position, sp.FindPropertyRelative("intValue"),new GUIContent());
                break;
            case GraphVariable.varTypes.Float:
                EditorGUI.PropertyField(position, sp.FindPropertyRelative("floatValue"), new GUIContent());
                break;
            case GraphVariable.varTypes.Boolean:
                EditorGUI.PropertyField(position, sp.FindPropertyRelative("boolValue"), new GUIContent());
                break;
            case GraphVariable.varTypes.Vector3:
                EditorGUI.PropertyField(position, sp.FindPropertyRelative("vector3Value"), new GUIContent());
                break;
            case GraphVariable.varTypes.String:
                EditorGUI.PropertyField(position, sp.FindPropertyRelative("stringValue"), new GUIContent());
                break;
            case GraphVariable.varTypes.AudioMixer:
                EditorGUI.PropertyField(position, sp.FindPropertyRelative("audioMixerValue"), new GUIContent());
                break;
            case GraphVariable.varTypes.AudioMixerSnapshot:
                EditorGUI.PropertyField(position, sp.FindPropertyRelative("snapshotValue"), new GUIContent());
                break;

        }
    }

    

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        GraphVariable.varTypes varTypeProp = (GraphVariable.varTypes)property.FindPropertyRelative("varType").enumValueIndex;
        return 4 * EditorGUIUtility.standardVerticalSpacing + 3 * EditorGUIUtility.singleLineHeight;
    }*/
    }
}
