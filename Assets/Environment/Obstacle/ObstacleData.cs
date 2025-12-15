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
}
