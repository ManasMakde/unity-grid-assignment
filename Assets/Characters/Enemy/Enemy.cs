using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public interface IEnemyAI
{
    bool HasOccupiedPoint(Vector2Int point);
}

public class Enemy : BaseCharacter, IEnemyAI
{
    // To-Set Properties
    [SerializeField] public string playerTag = "Player";


    // Properties
    private Player player; // Player to chase
    private List<Vector2Int> playerSurroundOffsets = new List<Vector2Int> { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };


    // Utility Methods
    void CalcPointsToPlayer(Vector2Int OldPoint, Vector2Int NewPoint)
    {
        // Return if already travelling
        if (travelPoints.Count != 0)
        {
            return;
        }


        // Return if already surrounding player
        foreach (var surroundOffset in playerSurroundOffsets)
        {
            // Get new travel points
            Vector2Int surroundPoint = player.currentPoint + surroundOffset;
            if (surroundPoint == currentPoint)
            {
                return;
            }
        }


        // Get Travel Points
        foreach (var surroundOffset in playerSurroundOffsets)
        {
            // Get new travel points
            Vector2Int surroundPoint = player.currentPoint + surroundOffset;
            var newTravelPoints = board.GetPathToPoint(currentPoint, surroundPoint);


            // Skip if no travel points found
            if (newTravelPoints.Count <= 0 || currentPoint == surroundPoint)
            {
                continue;
            }


            // Assign travel points
            travelPoints = newTravelPoints;
            break;
        }
    }
    void Catchup()
    {
        CalcPointsToPlayer(Vector2Int.zero, player.currentPoint);
    }


    // Interface Methods
    public bool HasOccupiedPoint(Vector2Int point)
    {
        return point == currentPoint;
    }


    // Override Methods
    protected override void Start()
    {
        base.Start();


        // Get player & bind to it's relevant delegates
        var playerObject = GameObject.FindWithTag(playerTag);
        if (playerObject.TryGetComponent<Player>(out Player outPlayer))
        {
            player = outPlayer;
            player.OnCurrentPointUpdated += CalcPointsToPlayer;
            CalcPointsToPlayer(Vector2Int.zero, player.currentPoint);
        }


        // Bind to self
        OnTravelComplete += Catchup;
    }
}