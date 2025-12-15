using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class Player : BaseCharacter
{
    // Delegates
    public event Action<Vector2Int /* OldPoint */, Vector2Int /* NewPoint */> OnHoverPointUpdated;


    // Properties
    public Vector2Int hoverPoint { private set; get; } = INVALID_POINT;


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


        // Store old point & set new point
        var oldHoverPoint = hoverPoint;
        hoverPoint = newHoverPoint;


        // Broadcast
        OnHoverPointUpdated?.Invoke(oldHoverPoint, newHoverPoint);
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
        // Listen for inputs if no travel points
        if(travelPoints.Count==0){
            MouseHoverInputUpdate();
            MouseClickInputUpdate();
        }

        base.Update();
    }
}
