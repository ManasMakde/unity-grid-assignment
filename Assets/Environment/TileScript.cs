using UnityEngine;

public interface ITileStats
{
    Vector2Int TileLocation();
}

public class TileScript : MonoBehaviour, ITileStats  // Main tile script responsible for holding tile data
{
    // Properties
    [SerializeField] public int row = 0;
    [SerializeField] public int column = 0;


    // Interface Methods
    public Vector2Int TileLocation()
    {
        return new Vector2Int(row, column);
    }

}
