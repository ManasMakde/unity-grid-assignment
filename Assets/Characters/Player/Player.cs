using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    // Delegates
    public event Action<Vector2Int /* OldLocation */, Vector2Int /* NewLocation */> OnHoverLocationUpdated;
    public event Action<Vector2Int /* OldLocation */, Vector2Int /* NewLocation */> OnLocationUpdated;


    // Properties
    [SerializeField] public LayerMask tileMask; 
    public Vector2Int currentLocation { private set; get; } = new Vector2Int(1, 1);
    private Vector2Int hoverLocation = new Vector2Int(-1, -1);


    // Components
    private Camera cam;


    // Set-Get Methods
    private void SetHoverLocation(Vector2Int newHoverLocation)
    {
        // Return if trying to assign the same value
        if(newHoverLocation == hoverLocation)
        {
            return;
        }


        // Store old location & set new location
        var oldHoverLocation = hoverLocation;
        hoverLocation = newHoverLocation;


        // Broadcast
        OnHoverLocationUpdated?.Invoke(oldHoverLocation, newHoverLocation);
    }


    // Helper Methods
    void MouseHoverInputUpdate()
    {
        // Raycast in the direction of camera location to mouse location
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        // Return if raycast didn't hit anything
        if (!Physics.Raycast(ray, out hit, 100, tileMask))
        {
            SetHoverLocation(new Vector2Int(-1, -1));
            return;
        }


        // Return if hit object does not have Interface
        if (!hit.collider.TryGetComponent<ITileStats>(out var tileStats))
        {
            SetHoverLocation(new Vector2Int(-1, -1));
            return;
        }


        // Set the new hover location
        Vector2Int newHoverLocation = tileStats.TileLocation();
        SetHoverLocation(newHoverLocation);
    }
    void MouseClickInputUpdate()
    {
        // Return if no mouse was not clicked
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

    }


    // Override Methods
    void Awake()
    {
    }
    void Start()
    {
        cam = Camera.main;
    }
    void Update()
    {
        MouseHoverInputUpdate();
        MouseClickInputUpdate();
    }
}
