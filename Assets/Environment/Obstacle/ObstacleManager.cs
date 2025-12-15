using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    // Properties
    [SerializeField] public ObstacleData obstacleData;
    [SerializeField] public GameObject obstaclePrefab;
    [SerializeField] public Vector3 obstacleOffset;
    private List<List<GameObject>> TileGrid;


    // Helper Methods
    List<List<GameObject>> CalcTileGrid()
    {
        // Get all child tiles
        GameObject board = this.gameObject;
        var grid = new List<List<GameObject>>();
        for (int rowIndex = 0; rowIndex < board.transform.childCount; rowIndex++)
        {
            // Iterate through children of row
            Transform rowTransform = board.transform.GetChild(rowIndex);
            List<GameObject> rowTiles = new List<GameObject>();
            for (int colIndex = 0; colIndex < rowTransform.childCount; colIndex++)
            {
                // Skip if not tile i.e. doesn't implement tile interface
                GameObject tile = rowTransform.GetChild(colIndex).gameObject;
                if (!tile.TryGetComponent<TileScript>(out var script))
                {
                    continue;
                }

                // Add to list if valid tile
                rowTiles.Add(tile);
            }

            grid.Add(rowTiles);
        }

        return grid;
    }
    void CreateObstacles()
    {
        // Return if invalid properties
        if (TileGrid == null || obstaclePrefab == null || obstacleData == null)
        {
            return;
        }


        // Iterate through obstacle data
        for (int rowIndex = 0; rowIndex < TileGrid.Count; rowIndex++)
        {
            var row = TileGrid[rowIndex];
            for (int colIndex = 0; colIndex < row.Count; colIndex++)
            {
                // Skip if tile is not blocked
                if (!obstacleData.IsBlocked(rowIndex, colIndex))
                {
                    continue;
                }

                // Instantiate obstacle & place it above tile
                var tile = row[colIndex];
                Vector3 obstaclePosition = tile.transform.position + obstacleOffset;
                GameObject obstacle = Instantiate(obstaclePrefab, obstaclePosition, Quaternion.identity);
            }
        }
    }


    // Override Methods
    void Start()
    {
        // Assign fresh tile grid
        TileGrid = CalcTileGrid();


        // Create obstacles on grid
        CreateObstacles();
    }
}
