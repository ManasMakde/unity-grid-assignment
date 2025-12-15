using UnityEngine;


public class Board : MonoBehaviour
{
    // Properties
    public int[,] cacheObstacleGrid { private set; get;} = new int[10, 10]; // Caching the grid but also possible to alter if required
    

    // Components
    ObstacleManager obstacleManager;


    // Exposed Methods
    public Vector2 GetTilePosition(Vector2Int Point){

        // Return origin if tile not found
        var tile = obstacleManager.Tiles[Point.x][Point.y];
        if(tile == null){
            return Vector2.zero;
        }


        return tile.transform.position;
    }
    public Vector2Int[] GetPathToPoint(Vector2Int fromPoint, Vector2Int toPoint){
        return AStarPath.FindPath(cacheObstacleGrid, fromPoint, toPoint);
    }


    // Override Methods
    void Start()
    {
        // Get Components
        obstacleManager = GetComponent<ObstacleManager>();

        // Calculate cache
        cacheObstacleGrid = obstacleManager.obstacleData.CalcGrid();
    }
}
