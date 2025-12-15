using System.Collections;
using UnityEngine;


[System.Serializable]
public class RowBool
{
    public bool[] columns;
}


[CreateAssetMenu(fileName = "ObstacleData", menuName = "Custom/Obstacle Data")]
public class ObstacleData : ScriptableObject
{
    // Properties
    public RowBool[] rows;


    // Methods
    public bool IsBlocked(int rowIndex, int columnIndex)
    {
        // If not within row range
        if (rowIndex < 0  || rows.Length <= rowIndex)
        {
            return false;
        }


        // If not within column range
        var checkingRow = rows[rowIndex];
        if (columnIndex < 0 || checkingRow.columns.Length <= columnIndex)
        {
            return false;
        }


        return checkingRow.columns[columnIndex];
    }
    public int[,] CalcGrid()
    {
        // Check if rows have been assigned
        if (rows == null || rows.Length == 0)
        {
            return new int[0, 0]; // Return an empty grid
        }


        // Calculate dimensions of grid
        int rowCount = rows.Length;
        int ColCount = rows[0].columns.Length;


        // Create grid with same dimensions
        int[,] grid = new int[rowCount, ColCount];


        // Iterate through each row and column to populate the grid
        for (int row = 0; row < rowCount; row++)
        {
            // Skip if invalid row
            RowBool rowBool = rows[row];
            if (rowBool == null || rowBool.columns.Length != ColCount)
            {
                continue;
            }


            // Assign values in grid
            for (int col = 0; col < ColCount; col++)
            {
                grid[row, col] = rowBool.columns[col] ? 0 : 1; // Blocked = 1, Unblocked = 0
            }
        }


        return grid;
    }
}
