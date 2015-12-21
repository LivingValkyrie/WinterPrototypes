using UnityEngine;
using System.Collections;
using LivingValkyrie.ActionBar;

/// <summary>
/// Author: Matt Gipson
/// Contact: Deadwynn@gmail.com
/// Domain: www.livingvalkyrie.com
/// 
/// Description: Ability 
/// </summary>
[System.Serializable]
public class Ability :  ICastable {
    #region Fields

    public string name;
    public string description;
    public Sprite icon;

    #endregion

    public void OnCast() {
        
    }

    public void Print(string message) {
        Debug.Log(message);
    }

    public override string ToString() {
        return name + " icon: " + icon;
    }
}

/*
enum of abilities
array of all ability objects in a class to be referenced. 
use enum as index for what ability is being held
*/