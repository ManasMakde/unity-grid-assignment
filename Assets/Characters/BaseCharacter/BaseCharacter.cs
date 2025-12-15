using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class BaseCharacter : MonoBehaviour
{
    // Delegates
    public event Action<Vector2Int /* OldPoint */, Vector2Int /* NewPoint */> OnCurrentPointUpdated;  // Broadcasted when the character's point on the grid has changed
    public event Action OnTravelComplete;  // Broadcasted when the characer has completed travelling and has just stopped


    // Constants
    public static readonly Vector2Int INVALID_POINT = new Vector2Int(-1, -1);


    // To-Set Properties
    [SerializeField] public Vector2Int initialPoint = new Vector2Int(1, 1);  // The point on the grid where the character initially starts from 
    [SerializeField] public float movementSpeed = 10.0f;  // The speed at which the character moves across the grid
    [SerializeField] public LayerMask tileMask;  // Layer mask for detecting tiles with raycast 
    [SerializeField] public GameObject boardObj;  // The board object which contains the grid of tiles


    // Properties
    public Vector2Int currentPoint { private set; get; } = new Vector2Int(0, 0);  // Which point on the board the character currently resides on 
    protected List<Vector2Int> travelPoints = new List<Vector2Int>();  // Grid points (in order) to travel along, As the points are reached they are removed from the list


    // Components
    protected Board board;


    // Set-Get Methods
    protected void SetCurrentPoint(Vector2Int newCurrentPoint)
    {
        // Return if trying to assign the same value
        if (newCurrentPoint == currentPoint)
        {
            return;
        }


        // Store old point for broadcasting
        var oldCurrentPoint = currentPoint;


        // Assign new point
        currentPoint = newCurrentPoint;


        // Broadcasting
        OnCurrentPointUpdated?.Invoke(oldCurrentPoint, newCurrentPoint);
    }


    // Update Methods
    void MovementUpdate()  // Responsible for moving character along travel points
    {
        // Get self and tile position to move towards
        Vector3 SelfPosition = transform.position;
        Vector3 TilePlatformPos = board.GetStandPosition(travelPoints[0]);
        TilePlatformPos.y = SelfPosition.y;  // To avoid moving along y axis


        // Move towards tile position
        transform.position = Vector3.MoveTowards(SelfPosition, TilePlatformPos, movementSpeed * Time.deltaTime);
        

        // When reached destination point
        if (Vector3.Distance(transform.position, TilePlatformPos) < 0.01f)
        {
            // Set the newly reached point as current
            SetCurrentPoint(travelPoints[0]);


            // Remove the newly reached point from travel list to avoid retravelling
            travelPoints.RemoveAt(0);


            // Broadcast travel complete when exhausted travel points
            if (travelPoints.Count == 0)
            {
                OnTravelComplete?.Invoke();
            }
        }
    }


    // Override Methods
    protected virtual void Start()
    {
        // Get Components
        board = boardObj.GetComponent<Board>();


        // Set initial point & it's position
        SetCurrentPoint(initialPoint);


        // Set initial position from point
        Vector3 IntialPosition = board.GetStandPosition(initialPoint);
        IntialPosition.y = transform.position.y;  // To avoid moving along y axis
        transform.position = IntialPosition;
    }
    protected virtual void Update()
    {
        // Move if given travel points
        if (travelPoints.Count != 0)
        {
            MovementUpdate();
        }
    }
}
