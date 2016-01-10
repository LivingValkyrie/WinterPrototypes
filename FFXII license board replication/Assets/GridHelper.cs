using System;
using System.Collections.Generic;

/// <summary>
/// Author: Matt Gipson
/// Contact: Deadwynn@gmail.com
/// Domain: www.livingvalkyrie.com
/// 
/// Description: GridHelper is a class with grid based functions made to work with generics. 
/// </summary>
public static class GridHelper {

    public static TGridType[,] GenerateGrid<TGridType>(int width, int height) where TGridType : new() {
        TGridType[,] arrayToReturn = new TGridType[width, height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                arrayToReturn[x, y] = new TGridType();
            }
        }

        return arrayToReturn;
    }

    public static List<TGridType> FindNeighbors<TGridType>(TGridType[,] grid, int gridX, int gridY, NeighborType type,
                                                           int radius = 1) {
        List<TGridType> neighborsToReturn = new List<TGridType>();

        for (int x = -radius; x <= radius; x++) {
            for (int y = -radius; y <= radius; y++) {
                if (x == 0 && y == 0) {
                    continue; //inside current node
                }

                int checkX = gridX + x;
                int checkY = gridY + y;

                //check if in bounds
                if (checkX >= 0 && checkX < grid.GetLength(0) && checkY >= 0 && checkY < grid.GetLength(1)) {
                    switch (type) {
                        case NeighborType.Vertical:

                            //same vertical plane
                            if (x == 0) {
                                neighborsToReturn.Add(grid[checkX, checkY]);
                            }
                            break;
                        case NeighborType.Horizontal:

                            //same horizontal plane
                            if (y == 0) {
                                neighborsToReturn.Add(grid[checkX, checkY]);
                            }
                            break;
                        case NeighborType.Cross:

                            //vertical and horizontal planes are both the same
                            if (x == 0 || y == 0) {
                                neighborsToReturn.Add(grid[checkX, checkY]);
                            }
                            break;
                        case NeighborType.Diagonal:

                            //test if location has the same x and y compared to the base node
                            if (Math.Abs(checkX - gridX) == Math.Abs(checkY - gridY)) {
                                neighborsToReturn.Add(grid[checkX, checkY]);
                            }
                            break;
                        case NeighborType.All:

                            //add all results
                            neighborsToReturn.Add(grid[checkX, checkY]);
                            break;
                    }
                }
            }
        }

        //return
        return neighborsToReturn;
    }
}

public enum NeighborType {
    Vertical, //top and bottom neighbors
    Horizontal, //left and right neighbors
    Cross, //horizontal and vertical neighbors
    Diagonal, //diagonal neighbors
    All //diagonal and cross
}

public class GridNode<TGridType> {
    public GridNode<TGridType> leftNeighbor;
    public GridNode<TGridType> rightNeighbor;
    public GridNode<TGridType> topNeighbor;
    public GridNode<TGridType> bottomNeighbor;
    public GridNode<TGridType> topLeftNeighbor;
    public GridNode<TGridType> topRightNeighbor;
    public GridNode<TGridType> bottomLeftNeighbor;
    public GridNode<TGridType> bottomRightNeighbor;

    /// <summary>
    /// The tile that holds this grid node instance, should be set in start or awake for unity or in the constructor in standard c#.
    /// </summary>
    public TGridType tile;

    public GridNode() {}

    /// <summary>
    /// Initializes a new instance of the <see cref="GridNode{TGridType}"/> class.
    /// </summary>
    /// <param name="tile">The tile that holds this instance.</param>
    public GridNode(TGridType tile) {
        this.tile = tile;
    }

}