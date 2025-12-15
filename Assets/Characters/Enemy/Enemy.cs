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
    private List<Vector2Int> playerSurroundOffsets = new List<Vector2Int> { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };  // 4 adjacent sides enemy can be around player


    // Utility Methods
    void CalcPointsToPlayer(Vector2Int OldPoint, Vector2Int NewPoint)  // Sets the travel points such that they reach the player (Binded to Player.OnCurrentPointUpdated)
    {
        // Return if already travelling
        if (travelPoints.Count != 0)
        {
            return;
        }


        // Return if already surrounding player
        foreach (var surroundOffset in playerSurroundOffsets)
        {
            Vector2Int surroundPoint = player.currentPoint + surroundOffset;
            if (surroundPoint == currentPoint)
            {
                return;
            }
        }


        // Get Travel Points towards whichever of the 4 adjacent side is available
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


            // Assign new travel points
            travelPoints = newTravelPoints;
            break;
        }
    }


    // Interface Methods
    public bool HasOccupiedPoint(Vector2Int point)  // Checks if enemy has occupied given point on the grid
    {
        return point == currentPoint;
    }


    // Override Methods
    protected override void Start()
    {
        // Invoke from parent 
        base.Start();


        // Get player component
        var playerObject = GameObject.FindWithTag(playerTag);
        if (playerObject.TryGetComponent<Player>(out Player outPlayer))
        {
            player = outPlayer;
        }


        // Bind such that enemy starts moving whenever player's current point has been updated
        player.OnCurrentPointUpdated += CalcPointsToPlayer;


        // Start moving towards player the moment the game starts 
        CalcPointsToPlayer(Vector2Int.zero, player.currentPoint);


        // For rechecking if next to player after self travel has completed
        OnTravelComplete += () =>{ CalcPointsToPlayer(Vector2Int.zero, player.currentPoint); };
    }
}