using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
[CustomEditor(typeof(LevelCreator))]
[CanEditMultipleObjects]
public class LevelEditor : Editor
{
    LevelCreator levelCreator;
    public bool showGrid = true;
    private Vector2 scrollPosition = Vector2.zero;

    public override void OnInspectorGUI()
    {
        levelCreator = (LevelCreator)target;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Column"))
        {
            levelCreator.AddColumn();
        }
        if (GUILayout.Button("Remove Column"))
        {
            levelCreator.RemoveColumn();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Row"))
        {
            levelCreator.AddRow();
        }
        if (GUILayout.Button("Remove Row"))
        {
            levelCreator.RemoveRow();
        }
        EditorGUILayout.EndHorizontal();
        showGrid = EditorGUILayout.Foldout(showGrid, "Design Grid");
        if (showGrid)
        {
            if (levelCreator.column.Count < 3)
            {
                for(int i = levelCreator.column.Count; i < 3; i++)
                {
                    levelCreator.AddColumn();
                }
            }
            if (levelCreator.column[0].row.Count < 3)
            {
                for (int i = levelCreator.column[0].row.Count; i < 3; i++)
                {
                    levelCreator.AddRow();
                }
            }
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUIStyle tableStyle = new GUIStyle("box");
            tableStyle.padding = new RectOffset(10, 10, 10, 10);
            tableStyle.margin.left = 32;

            GUIStyle headerColumnStyle = new GUIStyle();
            headerColumnStyle.fixedWidth = 35;

            GUIStyle columnStyle = new GUIStyle();
            columnStyle.fixedWidth = 20;

            GUIStyle rowStyle = new GUIStyle();
            rowStyle.fixedHeight = 20;

            GUIStyle rowHeaderStyle = new GUIStyle();
            rowHeaderStyle.fixedWidth = columnStyle.fixedWidth - 1;

            GUIStyle columnHeaderStyle = new GUIStyle();
            columnHeaderStyle.fixedWidth = 20;
            columnHeaderStyle.fixedHeight = 20;

            GUIStyle columnLabelStyle = new GUIStyle();
            columnLabelStyle.fixedWidth = rowHeaderStyle.fixedWidth - 6;
            columnLabelStyle.alignment = TextAnchor.MiddleCenter;
            columnLabelStyle.fontStyle = FontStyle.Bold;

            GUIStyle cornerLabelStyle = new GUIStyle();
            cornerLabelStyle.fixedWidth = 30;
            cornerLabelStyle.alignment = TextAnchor.MiddleRight;
            cornerLabelStyle.fontStyle = FontStyle.Bold;

            GUIStyle rowLabelStyle = new GUIStyle();
            rowLabelStyle.fixedWidth = 20;
            rowLabelStyle.alignment = TextAnchor.MiddleRight;
            rowLabelStyle.fontStyle = FontStyle.Bold;

            EditorGUILayout.BeginHorizontal(tableStyle);
            for (int x = 0; x < levelCreator.column.Count - 1; x++)
            {
                EditorGUILayout.BeginVertical((x == 0) ? headerColumnStyle : columnStyle);
                for (int y = 0; y < levelCreator.column[0].row.Count - 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        EditorGUILayout.BeginVertical(rowHeaderStyle);
                        EditorGUILayout.LabelField("[X,Y]", cornerLabelStyle);
                        EditorGUILayout.EndHorizontal();
                    }
                    else if (x == 0)
                    {
                        EditorGUILayout.BeginVertical(columnHeaderStyle);
                        EditorGUILayout.LabelField((levelCreator.column[0].row.Count-2-(y)).ToString(), rowLabelStyle);
                        EditorGUILayout.EndHorizontal();
                    }
                    else if (y == 0)
                    {
                        EditorGUILayout.BeginVertical(rowHeaderStyle);
                        EditorGUILayout.LabelField((x - 1).ToString(), columnLabelStyle);
                        EditorGUILayout.EndHorizontal();
                    }

                    if (x > 0 && y > 0)
                    {
                        EditorGUILayout.BeginHorizontal(rowStyle);
                        levelCreator.column[x].row[y] = EditorGUILayout.Toggle(levelCreator.column[x].row[y]);
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Level Data"))
        {
            string path = EditorUtility.SaveFilePanel("Save new", Application.dataPath, "LevelData", "json");
            levelCreator.SaveLevel(path);
        }
        if (GUILayout.Button("Load Level Data"))
        {
            string path = EditorUtility.OpenFilePanel("Load new ", Application.dataPath, "json");
            if (string.IsNullOrEmpty(path)) return;
            levelCreator.LoadLevel(path);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Level"))
        {
            levelCreator.GenerateLevel();
        }
        if (GUILayout.Button("Delete Level"))
        {
            levelCreator.DeleteLevel();
        }
        EditorGUILayout.EndHorizontal();
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            DrawDefaultInspector();
        }
    }
}
