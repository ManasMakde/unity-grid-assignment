using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class Player : MonoBehaviour
{
    // Delegates
    public event Action<Vector2Int /* OldPoint */, Vector2Int /* NewPoint */> OnHoverPointUpdated;
    public event Action<Vector2Int /* OldPoint */, Vector2Int /* NewPoint */> OnCurrentPointUpdated;


    // Constants
    public static readonly Vector2Int INVALID_POINT = new Vector2Int(-1, -1);


    // To-Set Properties
    [SerializeField] public float movementSpeed = 10.0f; 
    [SerializeField] public LayerMask tileMask;
    [SerializeField] public GameObject boardObj;


    // Properties
    public Vector2Int currentPoint { private set; get; } = new Vector2Int(0, 0);
    public Vector2Int hoverPoint { private set; get; } = INVALID_POINT;
    private List<Vector2Int> TravelPoints = new List<Vector2Int>();


    // Components
    private Camera cam;
    private Board board;


    // Set-Get Methods
    private void SetCurrentPoint(Vector2Int newCurrentPoint)
    {
        // Return if trying to assign the same value
        if(newCurrentPoint == currentPoint)
        {
            return;
        }


        // Store old point & set new point
        var oldCurrentPoint = currentPoint;
        currentPoint = newCurrentPoint;


        // Broadcast
        OnCurrentPointUpdated?.Invoke(oldCurrentPoint, newCurrentPoint);
    }
    private void SetHoverPoint(Vector2Int newHoverPoint)
    {
        // Return if trying to assign the same value
        if(newHoverPoint == hoverPoint)
        {
            return;
        }


        // Store old point & set new point
        var oldHoverPoint = hoverPoint;
        hoverPoint = newHoverPoint;


        // Broadcast
        OnHoverPointUpdated?.Invoke(oldHoverPoint, newHoverPoint);
    }


    // Utility Methods
    public static bool IsOnLineSegment(Vector3 P, Vector3 A, Vector3 B, float tolerance = 0.0001f)
    {

        // Check if collinear
        Vector3 AP = P - A;
        Vector3 AB = B - A;
        if (Vector3.Cross(AP, AB).magnitude > tolerance)
        {
            return false;
        }


        // Check if within range
        float sqrMagnitudeAB = AB.sqrMagnitude; 
        float dotProduct = Vector3.Dot(AP, AB);
        bool isWihinRange = dotProduct >= -tolerance && dotProduct <= sqrMagnitudeAB + tolerance;


        return isWihinRange;
    }


    // Update Methods
    void MouseHoverInputUpdate()
    {
        // Raycast in the direction of camera location to mouse location
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        // Return if raycast didn't hit anything
        if (!Physics.Raycast(ray, out hit, 100, tileMask))
        {
            SetHoverPoint(INVALID_POINT);
            return;
        }


        // Return if hit object does not have Interface
        if (!hit.collider.TryGetComponent<ITileStats>(out var tileStats))
        {
            SetHoverPoint(INVALID_POINT);
            return;
        }


        // Set the new hover point
        Vector2Int newHoverPoint = tileStats.GetTilePoint();
        SetHoverPoint(newHoverPoint);
    }
    void MouseClickInputUpdate()
    {
        // Return if mouse was not clicked
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        
        // Return if hover point is invalid
        if(hoverPoint == INVALID_POINT)
        {
            return;
        }


        // Get Target points to travel along
        TravelPoints.Clear();
        TravelPoints = board.GetPathToPoint(currentPoint, hoverPoint);


        // To remove first self point
        if(0 < TravelPoints.Count){
            TravelPoints.RemoveAt(0); 
        }
    }
    void MovementUpdate(){

        // Get self position
        Vector3 SelfPosition = transform.position;


        // Get the position to move towards
        Vector3 TilePlatformPos = board.GetPlatformPosition(TravelPoints[0]);
        TilePlatformPos.y = SelfPosition.y; // To avoid moving along y axis


        // Store old position
        Vector3 oldPosition = transform.position;


        // Move towards
        Vector3 direction = (TilePlatformPos - transform.position).normalized;
        transform.Translate(direction * movementSpeed * Time.deltaTime);


        // Check if we overshot target position
        Vector3 newPosition = transform.position;
        if(IsOnLineSegment(oldPosition, newPosition, TilePlatformPos)){
            transform.position = TilePlatformPos;
        }


        // Remove travel point if reached point
        if(Vector3.Distance(transform.position, TilePlatformPos) < 0.1f){
            SetCurrentPoint(TravelPoints[0]);
            TravelPoints.RemoveAt(0);
        }
    }


    // Override Methods
    void Start()
    {
        // Get Components
        cam = Camera.main;
        board = boardObj.GetComponent<Board>();
    }
    void Update()
    {
        // Listen for inputs if no travel points
        if(TravelPoints.Count==0){
            MouseHoverInputUpdate();
            MouseClickInputUpdate();
        }
        else{
            MovementUpdate();
        }
    }
}
