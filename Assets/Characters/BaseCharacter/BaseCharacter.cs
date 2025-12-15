using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class BaseCharacter : MonoBehaviour
{
    // Delegates
    public event Action<Vector2Int /* OldPoint */, Vector2Int /* NewPoint */> OnCurrentPointUpdated;
    public event Action OnTravelComplete;


    // Constants
    public static readonly Vector2Int INVALID_POINT = new Vector2Int(-1, -1);


    // To-Set Properties
    [SerializeField] public Vector2Int initialPoint = new Vector2Int(1, 1);
    [SerializeField] public float movementSpeed = 10.0f; 
    [SerializeField] public LayerMask tileMask;
    [SerializeField] public GameObject boardObj;


    // Properties
    public Vector2Int currentPoint { private set; get; } = new Vector2Int(0, 0);
    protected List<Vector2Int> travelPoints = new List<Vector2Int>();


    // Components
    protected Board board;


    // Set-Get Methods
    protected void SetCurrentPoint(Vector2Int newCurrentPoint)
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
    void MovementUpdate()
    {

        // Get self position
        Vector3 SelfPosition = transform.position;


        // Get the position to move towards
        Vector3 TilePlatformPos = board.GetPlatformPosition(travelPoints[0]);
        TilePlatformPos.y = SelfPosition.y; // To avoid moving along y axis


        // Store old position
        Vector3 oldPosition = transform.position;


        // Move towards
        Vector3 direction = (TilePlatformPos - transform.position).normalized;
        transform.Translate(direction * movementSpeed * Time.deltaTime, Space.World);


        // Check if we overshot target position
        Vector3 newPosition = transform.position;
        if(IsOnLineSegment(oldPosition, newPosition, TilePlatformPos)){
            transform.position = TilePlatformPos;
        }


        // Remove travel point if reached point
        if(Vector3.Distance(transform.position, TilePlatformPos) < 0.001f){
            SetCurrentPoint(travelPoints[0]);
            travelPoints.RemoveAt(0);


            // Broadcast travel complete
            if (travelPoints.Count == 0)
            {
                OnTravelComplete?.Invoke();
            }
        }
    }


    // Override Methods
    protected virtual void Start()
    {
        // Set initial point
        SetCurrentPoint(initialPoint);


        // Get Components
        board = boardObj.GetComponent<Board>();
    }
    protected virtual void Update()
    {
        // Listen for inputs if no travel points
        if(travelPoints.Count != 0){
            MovementUpdate();
        }
    }
}
