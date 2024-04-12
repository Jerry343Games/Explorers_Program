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

        // 更新行列数以匹配新的地图尺寸
        int rows = 4;      // 行数保持为4
        int columns = 5;   // 列数更新为5

        // 检查网格标记数组的大小是否正确
        if (gridMarkersProp.arraySize != rows * columns)
        {
            EditorGUILayout.HelpBox("网格标记数量不正确，预期为: " + (rows * columns) + " 实际为: " + gridMarkersProp.arraySize, MessageType.Error);
        }
        else
        {
            EditorGUI.BeginChangeCheck();

            // 循环显示所有行和列的网格标记编辑器
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

        // 绘制默认检查器其余部分
        DrawDefaultInspector();
    }
    
}

