using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    [SerializeField] public TileSet tileSet;
    [HideInInspector]
    public List<LevelRow> column = new List<LevelRow>();
    public bool fillEdges = true;
    public bool invertY = true;
    public bool spawnFloor = true;
    private List<ObjectRow> objectColumn = new List<ObjectRow>();
    private GameObject currentGameObect;

    public void AddColumn()
    {
        column.Add(new LevelRow());
        objectColumn.Add(new ObjectRow());
        if (column.Count > 1)
        {
            for(int i = 0; i < column[column.Count - 2].row.Count; i++)
            {
                if (spawnFloor)
                {
                    column[column.Count - 1].row.Add(i==((invertY) ? column[0].row.Count-2:1));
                }
                else
                {
                    column[column.Count - 1].row.Add(false);
                }
                objectColumn[objectColumn.Count - 1].objectRow.Add(null);
            }
        }
    }
    public void RemoveColumn()
    {
        if(column.Count > 0)
        {
            for (int i = 0; i < objectColumn[objectColumn.Count - 1].objectRow.Count; i++)
            {
                if(objectColumn[objectColumn.Count - 1].objectRow[i] != null)
                {
                    DestroyImmediate(objectColumn[objectColumn.Count - 1].objectRow[i]);
                }
            }
            objectColumn.RemoveAt(objectColumn.Count - 1);
            column.RemoveAt(column.Count - 1);
        }
    }
    public void AddRow()
    {
        for(int i = 0; i < column.Count; i++)
        {
            if (invertY)
            {
                column[i].row.Insert(0, false);
                objectColumn[i].objectRow.Insert(0, null);
            }
            else
            {
                column[i].row.Add(false);
                objectColumn[i].objectRow.Add(null);
            }
        }
    }
    public void RemoveRow()
    {
        for (int i = 0; i < column.Count; i++)
        {
            if (invertY && column[i].row.Count > 0)
            {
                if (objectColumn[i].objectRow[0] != null)
                {
                    DestroyImmediate(objectColumn[i].objectRow[0]);
                }
                column[i].row.RemoveAt(0);
                objectColumn[i].objectRow.RemoveAt(0);
            }
            else if (column[i].row.Count > 0)
            {
                if (objectColumn[i].objectRow[objectColumn[i].objectRow.Count - 1] != null)
                {
                    DestroyImmediate(objectColumn[i].objectRow[objectColumn[i].objectRow.Count - 1]);
                    
                }
                column[i].row.RemoveAt(column[i].row.Count - 1);
                objectColumn[i].objectRow.RemoveAt(objectColumn[i].objectRow.Count - 1);
            }
        }
    }

    public void GenerateLevel()
    {
        print(column);
        if(tileSet == null)
        {
            print("Please load a Tile Set! Level generation aborted");
            return;
        }
        DeleteLevel();
        for (int columnNumber = 1; columnNumber < column.Count - 1; columnNumber++)
        {
            for (int rowNumber = 1; rowNumber < column[0].row.Count - 1; rowNumber++)
            {
                if (column[columnNumber].row[(invertY)? column[0].row.Count - 1 - rowNumber: rowNumber])
                {
                    Tile tile = IndexTile(columnNumber, (invertY) ? column[0].row.Count - 1 - rowNumber : rowNumber);
                    currentGameObect = new GameObject("Tile [" + columnNumber + ", " + rowNumber + "]");
                    currentGameObect.transform.position = new Vector3(transform.position.x + columnNumber, transform.position.y + rowNumber, transform.position.z);
                    currentGameObect.transform.parent = transform;
                    currentGameObect.layer = 7;
                    currentGameObect.AddComponent<SpriteRenderer>();
                    SpriteRenderer spriteRenderer = currentGameObect.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = tile.sprite;

                    currentGameObect.AddComponent<BoxCollider2D>();

                    objectColumn[columnNumber].objectRow[rowNumber] = currentGameObect;
                }
            }
        }
    }
    public void DeleteLevel()
    {
        for (int columnNumber = 0; columnNumber < objectColumn.Count; columnNumber++)
        {
            for (int rowNumber = 0; rowNumber < objectColumn[0].objectRow.Count; rowNumber++)
            {
                if (objectColumn[columnNumber].objectRow[rowNumber] != null)
                {
                    DestroyImmediate(objectColumn[columnNumber].objectRow[rowNumber]);
                }
            }
        }
    }
    public void SaveLevel(string path)
    {
        SerializedLevelData serializedLevelData = new SerializedLevelData();
        serializedLevelData.column = column;
        string json = JsonUtility.ToJson(serializedLevelData);
        System.IO.File.WriteAllText(path, json);
    }

    public void LoadLevel(string path)
    {
        DeleteLevel();
        string json = System.IO.File.ReadAllText(path);
        column = JsonUtility.FromJson<SerializedLevelData>(json).column;
        objectColumn = new List<ObjectRow>();
        while(objectColumn.Count < column.Count)
        {
            objectColumn.Add(new ObjectRow());
        }
        for(int i = 0; i < column.Count; i++)
        {
            while (objectColumn[i].objectRow.Count < column[i].row.Count)
            {
                if (invertY)
                {
                    objectColumn[i].objectRow.Insert(0, null);
                }
                else
                {
                    objectColumn[i].objectRow.Add(null);
                }
            }
        }
    }

    public Tile IndexTile(int x, int y)
    {
        TileBorders tileBorder;
        int yCheck = (invertY) ? -1 : 1;
        tileBorder.top = column[x].row[y + yCheck];
        tileBorder.bottom = column[x].row[y - yCheck];
        tileBorder.left = column[x - 1].row[y];
        tileBorder.right = column[x + 1].row[y];
        tileBorder.topLeft = column[x - 1].row[y + yCheck];
        tileBorder.topRight = column[x + 1].row[y + yCheck];
        tileBorder.bottomLeft = column[x - 1].row[y - yCheck];
        tileBorder.bottomRight = column[x + 1].row[y - yCheck];     
        for(int i = 0; i < tileSet.tiles.Count; i++)
        {
            if(tileSet.tiles[i].tileBorder.BordersMatch(tileBorder))
            {
                return tileSet.tiles[i];
            }
        }
        return tileSet.defaultTile;
    }
    [Serializable]
    public class LevelRow
    {
        #if UNITY_EDITOR
        [HideInInspector] public bool showBoard;
        #endif
        public List<bool> row = new List<bool>();

        public void Empty()
        {
            for (int i = 0; i < row.Count; i++)
            {
                row[i] = false;
            }
        }
    }
    [Serializable]
    public class ObjectRow
    {
        public List<GameObject> objectRow = new List<GameObject>();

        public void Empty()
        {
            for (int i = 0; i < objectRow.Count; i++)
            {
                DestroyImmediate(objectRow[i]);
            }
        }
    }
    [Serializable]
    public class SerializedLevelData
    {
        public List<LevelRow> column = new List<LevelRow>();
    }
}
