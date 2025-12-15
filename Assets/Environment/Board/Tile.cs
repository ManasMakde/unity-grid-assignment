using UnityEngine;

public interface ITileStats
{
    Vector2Int GetTilePoint();
    Vector3 GetPlatformPosition();
}

public class Tile : MonoBehaviour, ITileStats  // Main tile script responsible for holding tile data
{
    // Properties
    [SerializeField] public int row = 0;
    [SerializeField] public int column = 0;
    [SerializeField] public Vector3 platformOffset = new Vector3(0.0f, 0.5f, 0.0f);


    // Interface Methods
    public Vector2Int GetTilePoint()
    {
        return new Vector2Int(row, column);
    }
    public Vector3 GetPlatformPosition()  // This returns the world position of platform i.e. where objects can be placed on top of
    {
        return gameObject.transform.position + platformOffset;
    }
}
