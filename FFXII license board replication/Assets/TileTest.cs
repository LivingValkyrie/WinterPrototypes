using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Author: Matt Gipson
/// Contact: Deadwynn@gmail.com
/// Domain: www.livingvalkyrie.com
/// 
/// Description: TileTest 
/// </summary>
public class TileTest : MonoBehaviour {
    #region Fields

    GridNode<TileTest> node;

    #endregion

    void Awake() {
        node = new GridNode<TileTest>(this);
    }

    void Start() {}

    void Update() {}

}