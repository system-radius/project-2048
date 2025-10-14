using UnityEditor;

[CustomEditor(typeof(TutorialStepController))]
public class TutorialStepEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //DrawPropertiesExcluding(serializedObject, "direction", "currentPlayer", "nextPlayer", "spawnTile", "location", "tileValue", "playerOwner");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("panel"));

        SerializedProperty reqProp = serializedObject.FindProperty("requirement");
        EditorGUILayout.PropertyField(reqProp);
        bool enable = (StepRequirement)reqProp.enumValueIndex == StepRequirement.Swipe || (StepRequirement)reqProp.enumValueIndex == StepRequirement.AI;
        EditorGUI.BeginDisabledGroup(!enable);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("direction"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("currentPlayer"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("nextPlayer"));
        EditorGUI.EndDisabledGroup();

        SerializedProperty spawnTileProp = serializedObject.FindProperty("spawnTile");
        EditorGUILayout.PropertyField(spawnTileProp);
        EditorGUI.BeginDisabledGroup(!spawnTileProp.boolValue);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("location"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tileValue"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("playerOwner"));
        EditorGUI.EndDisabledGroup();

        SerializedProperty highlightProp = serializedObject.FindProperty("highlightRegion");
        EditorGUILayout.PropertyField(highlightProp);
        EditorGUI.BeginDisabledGroup(!highlightProp.boolValue);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maskPanel"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("highlightStart"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("highlightEnd"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("size"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("targetObject"));
        EditorGUI.EndDisabledGroup();

        serializedObject.ApplyModifiedProperties();
    }
}