using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    SerializedProperty gridMarkersProp;

    void OnEnable()
    {
        // Find the property, which is an array in this case
        gridMarkersProp = serializedObject.FindProperty("gridMarkers");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Assuming the map is always a 3x4 grid as per the original requirement
        int rows = 4;
        int columns = 3;

        // Check if the array size is as expected
        if(gridMarkersProp.arraySize != rows * columns)
        {
            EditorGUILayout.HelpBox("不是3*4网格", MessageType.Error);
        }
        else
        {
            EditorGUI.BeginChangeCheck();

            for (int i = 0; i < rows; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < columns; j++)
                {
                    int index = i * columns + j;
                    SerializedProperty prop = gridMarkersProp.GetArrayElementAtIndex(index);
                    EditorGUILayout.PropertyField(prop, GUIContent.none);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        // Draw the rest of the default inspector
        DrawDefaultInspector();
    }
    
    

}

