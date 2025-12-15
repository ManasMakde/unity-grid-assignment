using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class Player : BaseCharacter
{
    // Delegates
    public event Action<Vector2Int /* OldPoint */, Vector2Int /* NewPoint */> OnHoverPointUpdated;  // Whenever the cursor changes which tile point it's hovering over


    // Properties
    public Vector2Int hoverPoint { private set; get; } = INVALID_POINT;  // Point on grid at which the cursor is hovering


    // Components
    private Camera cam;


    // Set-Get Methods
    private void SetHoverPoint(Vector2Int newHoverPoint)
    {
        // Return if trying to assign the same value
        if(newHoverPoint == hoverPoint)
        {
            return;
        }


        // Store old point for broadcasting
        var oldHoverPoint = hoverPoint;


        // Assign new point
        hoverPoint = newHoverPoint;


        // Broadcast
        OnHoverPointUpdated?.Invoke(oldHoverPoint, newHoverPoint);
    }


    // Update Methods
    void MouseHoverInputUpdate()  // Responsible for detecting which tile cursor is hovering over every update
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
    void MouseClickInputUpdate()  // Responsible for detecting clicks on tiles
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


        // Get points to travel along to reach desired tile
        travelPoints.Clear();
        travelPoints = board.GetPathToPoint(currentPoint, hoverPoint);
    }


    // Override Methods
    protected override void Start()
    {
        base.Start();


        // Get Components
        cam = Camera.main;
    }
    protected override void Update()
    {
        // Checking for hovering
        MouseHoverInputUpdate();


        // Listen for inputs if no travel points given
        if(travelPoints.Count==0){
            MouseClickInputUpdate();
        }


        base.Update();
    }
}
