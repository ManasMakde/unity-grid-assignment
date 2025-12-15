using System.Collections.Generic;
using UnityEngine;


public class ObstacleManager : MonoBehaviour
{
    // Properties
    [SerializeField] public ObstacleData obstacleData;
    [SerializeField] public GameObject obstaclePrefab;  // Prefab of obstacles to spawn on grid
    public List<List<GameObject>> Grid { private set; get;} = null;  // Grid collection of children tile objects


    // Utility Methods
    List<List<GameObject>> CalcGrid()  // Get grid of children tile objects
    {
        // Add all child tiles into the grid
        var newGrid = new List<List<GameObject>>();
        for (int rowIndex = 0; rowIndex < gameObject.transform.childCount; rowIndex++)
        {
            // Iterate through children of row object
            Transform rowTransform = gameObject.transform.GetChild(rowIndex);
            List<GameObject> row = new List<GameObject>();
            for (int colIndex = 0; colIndex < rowTransform.childCount; colIndex++)
            {
                // Add tile to row
                GameObject tile = rowTransform.GetChild(colIndex).gameObject;
                row.Add(tile);
            }


            // Add row of tiles
            newGrid.Add(row);
        }


        return newGrid;
    }
    void CreateObstacles()
    {
        // Return if invalid properties
        if (Grid == null || obstaclePrefab == null || obstacleData == null)
        {
            return;
        }


        // Iterate through obstacle data
        for (int rowIndex = 0; rowIndex < Grid.Count; rowIndex++)
        {
            var row = Grid[rowIndex];
            for (int colIndex = 0; colIndex < row.Count; colIndex++)
            {
                // Skip if tile is not blocked
                if (!obstacleData.IsBlocked(rowIndex, colIndex))
                {
                    continue;
                }


                // Instantiate obstacle & place it above tile
                var tile = row[colIndex];
                if(tile == null){
                    continue;
                }


                // Create an obstacle at tile platform position
                if(tile.TryGetComponent<ITileStats>(out var tileStats)){
                    Vector3 obstaclePosition =  tileStats.GetStandPosition();
                    GameObject obstacle = Instantiate(obstaclePrefab, obstaclePosition, Quaternion.identity);
                }
            }
        }
    }


    // Override Methods
    void Awake()
    {
        // Assign fresh tile newGrid
        Grid = CalcGrid();


        // Create obstacles on newGrid
        CreateObstacles();
    }
}
