using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int width = 3;
    public int height = 4;
    private Vector2Int start;
    private Vector2Int end;
    private List<Vector2Int> path = new List<Vector2Int>();
    
    public GameObject[] gridMarkers=new GameObject[12]; // 预先在Unity编辑器中赋值
    public GameObject roadStraightPrefab; // 直道模型的预制体
    public GameObject roadTurnPrefab; // 转弯道模型的预制体
    public GameObject roadTJunctionPrefab;

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        // 随机起点和终点
        start = new Vector2Int(0, Random.Range(0, width));
        end = new Vector2Int(height - 1, Random.Range(0, width));

        // 使用深度优先搜索(DFS)生成路径
        DFS(start, new HashSet<Vector2Int>());

        // 为每个单元格分配道路类型并放置模型
        foreach (var cell in path)
        {
            PlaceModel(cell, DetermineRoadType(cell));
        }

        // 在这里可以添加额外的逻辑来创建死胡同或分支
    }

    bool DFS(Vector2Int current, HashSet<Vector2Int> visited)
    {
        if (current == end)
        {
            path.Add(current);
            return true;
        }

        var directions = new List<Vector2Int>
        {
            new Vector2Int(0, 1), // 向右
            new Vector2Int(1, 0), // 向下
            new Vector2Int(0, -1), // 向左
            new Vector2Int(-1, 0) // 向上
        };
        Shuffle(directions); // 随机化方向

        foreach (var direction in directions)
        {
            Vector2Int nextCell = current + direction;
            if (IsInBounds(nextCell) && !visited.Contains(nextCell))
            {
                visited.Add(nextCell);
                if (DFS(nextCell, visited))
                {
                    path.Add(current);
                    return true;
                }
                visited.Remove(nextCell); // 回溯
            }
        }

        return false;
    }

    bool IsInBounds(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < height && cell.y >= 0 && cell.y < width;
    }

    void Shuffle(List<Vector2Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Vector2Int temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    
    // 此方法检查指定的cell是否在路径中
    bool IsCellInPath(Vector2Int cell)
    {
        return path.Contains(cell);
    }
    
    // 这个方法确定并返回当前cell的道路类型
    string DetermineRoadType(Vector2Int cell)
    {
        // 用来表示是否有相邻的道路
        bool up = IsCellInPath(cell + Vector2Int.up);
        bool down = IsCellInPath(cell + Vector2Int.down);
        bool left = IsCellInPath(cell + Vector2Int.left);
        bool right = IsCellInPath(cell + Vector2Int.right);

        // 根据相邻单元格判断道路类型
        int count = (up ? 1 : 0) + (down ? 1 : 0) + (left ? 1 : 0) + (right ? 1 : 0);
        if (count == 1)
        {
            // 如果只有一个相邻单元格在路径中，那么这是一个死胡同
            return "DeadEnd";
        }
        else if (count == 2)
        {
            // 如果有两个相邻单元格在路径中，它可能是直道或转弯
            if ((up && down) || (left && right))
            {
                return "Straight";
            }
            else
            {
                return "Turn";
            }
        }
        else if (count == 3)
        {
            // 三个相邻单元格在路径中表示这是一个三叉路口
            return "T-Junction";
        }
        else if (count == 4)
        {
            // 四个相邻单元格在路径中表示这是一个四叉路口
            return "Crossroads";
        }

        // 如果没有相邻单元格在路径中，那么这个单元格不是道路的一部分
        return "None";
    }

    void PlaceModel(Vector2Int gridPos, string roadType)
    {
        // 找到对应的标记点
        GameObject marker = gridMarkers[gridPos.x * width + gridPos.y];
        
        // 根据道路类型实例化模型
        GameObject prefabToPlace = null;
        switch(roadType)
        {
            case "Straight":
                prefabToPlace = roadStraightPrefab;
                break;
            case "Turn":
                prefabToPlace = roadTurnPrefab;
                break;
            case "T-Junction":
                prefabToPlace = roadTJunctionPrefab;
                break;
            default:
                prefabToPlace = roadStraightPrefab;
                break;
            // ...处理其他道路类型
        }

        if(prefabToPlace != null)
        {
            // 实例化模型并放置到标记点的位置
            Instantiate(prefabToPlace, marker.transform.position, Quaternion.identity, marker.transform);
        }
    }
    
    //标记点验证自检
    void ValidateGridMarkers()
    {
        // 确保网格标记点的数量是正确的
        if(gridMarkers.Length != width * height)
        {
            Debug.LogError("标记点数量不符合预期的网格大小。预期: " + (width * height) + " 实际: " + gridMarkers.Length);
            return;
        }

        for (int i = 0; i < gridMarkers.Length; i++)
        {
            // 计算理论上的行列位置
            int row = i / width;
            int col = i % width;

            // 获取标记点的GameObject
            GameObject marker = gridMarkers[i];
            if (marker == null)
            {
                Debug.LogError("网格位置 (" + row + ", " + col + ") 上没有标记点。");
                continue;
            }

            // 输出标记点的名称和理论上的网格位置
            Debug.Log(marker.name + " 应该在网格位置 (" + row + ", " + col + ")");
        }
    }

    public void Reload()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("RandomScene");
    }
}
