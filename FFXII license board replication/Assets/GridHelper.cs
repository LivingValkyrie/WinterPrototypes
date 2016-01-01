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