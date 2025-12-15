using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Board : MonoBehaviour
{
    // Properties
    public int[,] cacheObstacleGrid { private set; get;} = new int[10, 10];  // Caching the grid but also possible to alter if required
    

    // Components
    ObstacleManager obstacleManager;


    // Exposed Methods
    public Vector3 GetPlatformPosition(Vector2Int Point){

        // Return origin if tile not found
        var tile = obstacleManager.Tiles[Point.x][Point.y];
        if(tile == null){
            return Vector3.zero;
        }


        // Return platform component if implements tile stats interface
        if(tile.TryGetComponent<ITileStats>(out var tileStats)){
            return tileStats.GetPlatformPosition();
        }

        return Vector3.zero;
    }
    public List<Vector2Int> GetPathToPoint(Vector2Int fromPoint, Vector2Int toPoint, bool toIncludeFromPoint = false){

        var points = AStarPath.FindPath(cacheObstacleGrid, fromPoint, toPoint).ToList();


        // If to remove the starting `fromPoint` from list of points
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

        // Calculate cache
        cacheObstacleGrid = obstacleManager.obstacleData.CalcGrid();

        for (int i = 0; i < cacheObstacleGrid.GetLength(0); i++)
        {
            string row = "";
            for (int j = 0; j < cacheObstacleGrid.GetLength(1); j++)
            {
                row += cacheObstacleGrid[i, j] + " ";
            }
        }
    }
}
