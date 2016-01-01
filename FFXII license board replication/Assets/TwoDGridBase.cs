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

    public int[,] array;
    public int width;
    public int height;

    #endregion

    public void Start() {
        array = GridHelper.GenerateGrid<int>(width, height);
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                //print(array[x, y]);
            }
        }

        List<int> neighbors = GridHelper.FindNeighbors(array, 5, 5, NeighborType.Diagonal, 2);
        foreach (int i in neighbors) {
            //print("neighbor " + i);
        }
    }

}