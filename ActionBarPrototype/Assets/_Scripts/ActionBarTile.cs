using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Author: Matt Gipson
/// Contact: Deadwynn@gmail.com
/// Domain: www.livingvalkyrie.com
/// 
/// Description: ActionBarTile 
/// </summary>
public class ActionBarTile : MonoBehaviour {
    #region Fields

    Ability ability;
    public string activationKey;
    Sprite icon;

    public Sprite Icon {
        get { return icon; }
        set {
            icon = value;
            OnIconChange();
        }
    }

    public Ability Ability {
        get { return ability; }
        set {
            ability = value;
            Icon = ability.icon;
        }
    }

    #endregion

    void Start() {}

    void Update() {
        if (!string.IsNullOrEmpty(activationKey)) {
            if (Input.GetKey(activationKey)) {
                print(ability.name + " has been cast");

                //actually cast the ability
                //ability.ability.OnCast();
            }
        }
    }

    void OnIconChange() {
        GetComponent<Image>().sprite = icon;
    }

}