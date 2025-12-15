using UnityEngine;

public interface ITileStats
{
    Vector2Int GetTilePoint();
    Vector3 GetStandPosition();
}

public class Tile : MonoBehaviour, ITileStats  // Main tile script responsible for holding tile data
{
    // Properties
    [SerializeField] public int row = 0;  // which row in the grid this tile belongs to
    [SerializeField] public int column = 0;  // which column in the grid this tile belongs to
    [SerializeField] public Vector3 standOffset = new Vector3(0.0f, 0.5f, 0.0f);  // Offset from position of tile such that any object could appear "standing on top" the tile


    // Interface Methods
    public Vector2Int GetTilePoint() // Get the tile "point" i.e. row & column on the grid
    {
        return new Vector2Int(row, column);
    }
    public Vector3 GetStandPosition()  // Gives the position at which an object would "stand on top" of the tile  
    {
        return gameObject.transform.position + standOffset;
    }
}
