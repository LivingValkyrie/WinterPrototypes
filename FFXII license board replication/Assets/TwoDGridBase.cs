using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Author: Matt Gipson
/// Contact: Deadwynn@gmail.com
/// Domain: www.livingvalkyrie.com
/// 
/// Description: TwoDGridBase 
/// </summary>
public class TwoDGridBase : MonoBehaviour {
    #region Fields

    public Grid grid;
    public int[,] array;

    #endregion

    public void Start() {
        array = grid.GenerateGrid<int>();
        for ( int x = 0; x < grid.width; x++ ) {
            for ( int y = 0; y < grid.height; y++ ) {
                print(array[x, y]);
            }
        }
    }

}

[System.Serializable]
public struct Grid {
    public int width;
    public int height;

    public t[,] GenerateGrid<t>() where t : new() {
        t[,] arrayToReturn = new t[width,height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                arrayToReturn[x,y] = new t();
            }
        }

        return arrayToReturn;
    }
}