using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Author: Matt Gipson
/// Contact: Deadwynn@gmail.com
/// Domain: www.livingvalkyrie.com
/// 
/// Description: GameManager 
/// </summary>
[RequireComponent(typeof (BoardManager))]
public class GameManager : MonoBehaviour {
    #region Fields

    public static GameManager instance = null;
    public BoardManager boardScript;

    int level = 3;

    #endregion

    void Awake() {
        if (instance == null) {
            instance = this;
        }else if (instance != null) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    void InitGame() {
        boardScript.SetupScene(level);
    }

    void Update() {}
}