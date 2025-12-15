using System;
using System.Collections.Generic;
using UnityEngine;


public static class AStarPath
{
    // Structs
    public struct Cell
    {
        public Vector2Int parent;
        public double f, g, h;  // g = movement cost; h = heuristic; f = g + h
    }


    // Properties
    public static Vector2Int[] neighbourOffsets = new Vector2Int[]{
        new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1),
        new Vector2Int(-1, 0), new Vector2Int(1, 0),
        new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1)
    };


    // Utility Methods
    public static bool IsValid(Vector2Int loc, int TOTAL_ROWs, int TOTAL_COLS)
    {
        return (loc.x >= 0) && (loc.x < TOTAL_ROWs) && (loc.y >= 0) && (loc.y < TOTAL_COLS);
    }
    public static bool IsBlocked(int[,] grid, Vector2Int loc)
    {
        return grid[loc.x, loc.y] == 0;
    }
    public static double CalculateHValue(Vector2Int loc, Vector2Int dest)
    {
        return Math.Sqrt(Math.Pow(loc.x - dest.x, 2) + Math.Pow(loc.y - dest.y, 2));
    }
    public static Vector2Int[] TraceBackPath(Cell[,] cellDetails, Vector2Int dest)
    {
        // Create an array with possible path length (for worst case which could be the entire grid)
        int maxPathLength = cellDetails.GetLength(0) * cellDetails.GetLength(1);
        Vector2Int[] pathArray = new Vector2Int[maxPathLength];
        int pathIndex = 0;


        // Start tracing from the destination and follow the parent pointers to the source
        int row = dest.x;
        int col = dest.y;
        while (!(cellDetails[row, col].parent.x == row && cellDetails[row, col].parent.y == col))
        {
            pathArray[pathIndex++] = new Vector2Int(row, col);  // Add the current cell to the path
            int tempRow = cellDetails[row, col].parent.x;
            int tempCol = cellDetails[row, col].parent.y;
            row = tempRow;
            col = tempCol;
        }


        // Add the source location to path as well
        pathArray[pathIndex++] = new Vector2Int(row, col);


        // Shrink to only required path length & invert
        Vector2Int[] validPath = new Vector2Int[pathIndex];
        Array.Copy(pathArray, validPath, pathIndex);
        Array.Reverse(validPath);


        return validPath;
    }


    // Primary Method
    public static Vector2Int[] FindPath(int[,] grid, Vector2Int src, Vector2Int dest)  // Returns points in order from src to dest; grid value should be 0 for blocked and 1 for unblocked
    {
        int TOTAL_ROWs = grid.GetLength(0);
        int TOTAL_COLS = grid.GetLength(1);


        // If the source or destination is out of grid
        if (!IsValid(src, TOTAL_ROWs, TOTAL_COLS) || !IsValid(dest, TOTAL_ROWs, TOTAL_COLS))
        {
            return Array.Empty<Vector2Int>();
        }

        // If the source or destination is blocked
        if (IsBlocked(grid, src) || IsBlocked(grid, dest))
        {
            return Array.Empty<Vector2Int>();
        }

        // If already at destination
        if (src == dest)
        {
            return Array.Empty<Vector2Int>();
        }


        // Create a closed list and initialise all to false
        bool[,] closedCells = new bool[TOTAL_ROWs, TOTAL_COLS];
        Cell[,] cellDetails = new Cell[TOTAL_ROWs, TOTAL_COLS];
        for (int i = 0; i < TOTAL_ROWs; i++)
        {
            for (int j = 0; j < TOTAL_COLS; j++)
            {
                cellDetails[i, j].f = double.MaxValue;
                cellDetails[i, j].g = double.MaxValue;
                cellDetails[i, j].h = double.MaxValue;
                cellDetails[i, j].parent.x = -1;
                cellDetails[i, j].parent.y = -1;
            }
        }


        // Initialising starting cell
        cellDetails[src.x, src.y].f = 0.0;
        cellDetails[src.x, src.y].g = 0.0;
        cellDetails[src.x, src.y].h = 0.0;
        cellDetails[src.x, src.y].parent = src;


        // Create an open list in format (f, Vector2Int)
        SortedSet<(double, Vector2Int)> openList = new SortedSet<(double, Vector2Int)>(
            Comparer<(double, Vector2Int)>.Create((a, b) => a.Item1.CompareTo(b.Item1))
        );


        // Put the starting cell in the open list and set its f = 0
        openList.Add((0.0, src));  


        // Iterate until all cells are no longer open
        while (openList.Count > 0)
        {
            (double f, Vector2Int loc) p = openList.Min;
            openList.Remove(p);
            src = p.loc;
            closedCells[src.x, src.y] = true;


            // Generating all the 8 neighbours of this cell
            Vector2Int[] neighbours = new Vector2Int[8];
            for (int i = 0; i < 8; i++)
            {
                neighbours[i] = src + neighbourOffsets[i];
            }


            // Iterate through all neighbours            
            foreach (Vector2Int neighbour in neighbours){

                // Skip if neighbour is invalid
                if (!IsValid(neighbour, TOTAL_ROWs, TOTAL_COLS))
                {
                    continue;
                }


                // If the destination cell is the same as neighbour
                if (neighbour == dest)
                {
                    cellDetails[neighbour.x, neighbour.y].parent = src;
                    return TraceBackPath(cellDetails, dest);
                }


                // Skip if the neighbour is already on the closed list or blocked
                if (closedCells[neighbour.x, neighbour.y] || IsBlocked(grid, neighbour))
                {
                    continue;
                }


                // If it isn't on the open list then add it
                double gNew = cellDetails[src.x, src.y].g + 1.0;
                double hNew = CalculateHValue(neighbour, dest);
                double fNew = gNew + hNew;
                if (cellDetails[neighbour.x, neighbour.y].f == double.MaxValue || cellDetails[neighbour.x, neighbour.y].f > fNew)
                {
                    // Add to open list
                    openList.Add((fNew, new Vector2Int(neighbour.x, neighbour.y)));


                    // Update the details of this cell
                    cellDetails[neighbour.x, neighbour.y].f = fNew;
                    cellDetails[neighbour.x, neighbour.y].g = gNew;
                    cellDetails[neighbour.x, neighbour.y].h = hNew;
                    cellDetails[neighbour.x, neighbour.y].parent = src;
                }
            }
        }

 
        // Failed to find path
        return Array.Empty<Vector2Int>();
    }
}
