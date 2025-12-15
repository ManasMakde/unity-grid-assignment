using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Board : MonoBehaviour
{
    // Properties
    public int[,] cacheObstacleGrid { private set; get; } = new int[10, 10];  // Caching the grid but also possible to alter if required


    // Components
    ObstacleManager obstacleManager;


    // Exposed Methods
    public Vector3 GetStandPosition(Vector2Int Point)
    {
        // Return origin if tile not found
        var tile = obstacleManager.Grid[Point.x][Point.y];
        if (tile == null)
        {
            return Vector3.zero;
        }


        // Return tile position if implements the tile interface
        if (tile.TryGetComponent<ITileStats>(out var tileStats))
        {
            return tileStats.GetStandPosition();
        }


        return Vector3.zero;
    }
    public List<Vector2Int> GetPathToPoint(Vector2Int fromPoint, Vector2Int toPoint, bool toIncludeFromPoint = false)
    {

        // Get a list of all points to get from A to B
        List<Vector2Int> points = AStarPath.FindPath(cacheObstacleGrid, fromPoint, toPoint).ToList();


        // Remove first point in the list which is the same as `fromPoint`
        if (!toIncludeFromPoint && 0 < points.Count)
        {
            points.RemoveAt(0);
        }


        return points;
    }


    // Override Methods
    void Awake()
    {
        // Get Components
        obstacleManager = GetComponent<ObstacleManager>();


        // Calculate cache grid
        cacheObstacleGrid = obstacleManager.obstacleData.CalcGrid();
    }
}
