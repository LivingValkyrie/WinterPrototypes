using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Author: Matt Gipson
/// Contact: Deadwynn@gmail.com
/// Domain: www.livingvalkyrie.com
/// 
/// Description: Loader 
/// </summary>
public class Loader : MonoBehaviour {
    #region Fields

    public GameObject gameManager;

    #endregion

    void Awake() {
        if (GameManager.instance == null) {
            Instantiate(gameManager);
        }
    }
}