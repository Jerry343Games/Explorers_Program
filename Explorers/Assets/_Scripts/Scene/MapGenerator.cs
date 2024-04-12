using System.Collections;
using System.Collections.Generic;
using DG.DemiLib;
using UnityEngine;
using UnityEngine.Serialization;

public class MapGenerator : MonoBehaviour
{
    public int width = 4;
    public int height = 5;
    private Vector2Int start;
    private Vector2Int end;
    [Header("路径长度范围")]
    public Vector2Int pathRange;
    private List<Vector2Int> path = new List<Vector2Int>();
    
    public GameObject[] gridMarkers; // 预先在Unity编辑器中赋值
    public GameObject roadStraightHorizontalPrefab; //水平道模型的预制体
    public GameObject roadStraightVerticalPrefab;
    public GameObject roadCrossPrefab; // T形交叉道模型的预制体
    public GameObject startPrefab; // 起点模型的预制体
    public GameObject endPrefab; // 终点模型的预制体

    public GameObject defaultPrefab;

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        bool isValidPath = false;
        while (!isValidPath)
        {
            path.Clear(); // 清除之前的路径
            // 随机起点和终点
            start = new Vector2Int(0, Random.Range(0, width));
            end = new Vector2Int(height - 1, Random.Range(0, width));

            // 使用深度优先搜索(DFS)生成路径
            DFS(start, new HashSet<Vector2Int>());

            if (path.Count >= pathRange.x && path.Count <= pathRange.y)
            {
                isValidPath = true; // 如果路径长度在min和max之间，则结束循环
            }
        }
        // 创建一个HashSet包含所有路径单元格
        HashSet<Vector2Int> pathCells = new HashSet<Vector2Int>(path);

        // 遍历整个地图
        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                Vector2Int cell = new Vector2Int(x, y);
                if (pathCells.Contains(cell))
                {
                    // 这是路径的一部分，放置相应的道路模型
                    PlaceModel(cell, DetermineRoadType(cell));
                }
                else
                {
                    // 这不是路径的一部分，放置默认模型
                    PlaceModel(cell, "None"); // 修改这里以发送"None"作为类型
                }
            }
        }
        
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
        bool up = IsCellInPath(cell + Vector2Int.up);
        bool down = IsCellInPath(cell + Vector2Int.down);
        bool left = IsCellInPath(cell + Vector2Int.left);
        bool right = IsCellInPath(cell + Vector2Int.right);

        int count = (up ? 1 : 0) + (down ? 1 : 0) + (left ? 1 : 0) + (right ? 1 : 0);
        
        // 修改部分开始
        if (count == 2)
        {
            // 如果有两个相邻单元格在路径中，区分直线是竖直还是水平
            if (up && down)
            {
                return "Horizontal"; // 水平
            }
            else if (left && right)
            {
                return "Vertical"; // 竖直
            }
            else
            {
                //两个开口以上
                return "Crossroads";
            }
        }
        else if (count > 2)
        {
            // 如果开口数量大于2，统一标记为十字路口
            return "Crossroads";
        }
        else
        {
            // 其他情况（如死胡同和单独的格子）不再单独分类
            return "Crossroads";
        }
    }

    void PlaceModel(Vector2Int gridPos, string roadType)
    {
        GameObject marker = gridMarkers[gridPos.x * width + gridPos.y];
        GameObject prefabToPlace = null;

        // 检查是否为起点或终点，并选择适当的预制体
        if (gridPos == start)
        {
            prefabToPlace = startPrefab;
        }
        else if (gridPos == end)
        {
            prefabToPlace = endPrefab;
        }
        else
        {
            // 如果不是起点或终点，根据道路类型放置道路模型
            switch(roadType)
            {
                case "Vertical":
                    prefabToPlace = roadStraightVerticalPrefab;
                    break;
                case "Horizontal":
                    prefabToPlace = roadStraightHorizontalPrefab;
                    break;
                case "Crossroads":
                    prefabToPlace = roadCrossPrefab;
                    break;
                case "None":
                    prefabToPlace = defaultPrefab;
                    break;
                // 必要时处理其他道路类型...
            }
        }

        if(prefabToPlace != null)
        {
            Instantiate(prefabToPlace, marker.transform.position, Quaternion.identity, marker.transform);
        }
    }
    
    /// <summary>
    /// 标记点自检
    /// </summary>
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
