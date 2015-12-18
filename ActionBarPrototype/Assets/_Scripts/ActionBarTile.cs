using UnityEngine;
using System.Collections;

/// <summary>
/// Author: Matt Gipson
/// Contact: Deadwynn@gmail.com
/// Domain: www.livingvalkyrie.com
/// 
/// Description: ActionBarTile 
/// </summary>
public class ActionBarTile : MonoBehaviour {
    #region Fields

    public AbilityTile ability;
    public string activationKey;

    #endregion

    void Start() { }

    void Update() {
        if (!string.IsNullOrEmpty(activationKey)) {
            if (Input.GetKey(activationKey)) {
                print(ability.ability.name + " has been cast");

                //actually cast the ability
                //ability.ability.OnCast();
            }
        }
    }

}