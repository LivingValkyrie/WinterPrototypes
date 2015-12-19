using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Author: Matt Gipson
/// Contact: Deadwynn@gmail.com
/// Domain: www.livingvalkyrie.com
/// 
/// Description: ActionBarTile 
/// </summary>
public class ActionBarTile : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler {
    #region Fields

    Ability ability;
    public string activationKey;
    public Sprite defaultSprite;
    Sprite icon;

    GameObject carryAbility;

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
            if (ability != null) {
                Icon = ability.icon;
            } else {
                Icon = defaultSprite;
            }
        }
    }

    #endregion

    void Start() {
        if (Icon == null) {
            Icon = defaultSprite;
        }
    }

    void Update() {
        if (!string.IsNullOrEmpty(activationKey)) {
            if (Input.GetButtonDown(activationKey) && Ability != null) {
                print(ability.name + " has been cast");

                //actually cast the ability
                //Ability.OnCast();
            }
        }
    }

    void OnIconChange() {
        GetComponent<Image>().sprite = icon;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        //return if right click drag
        if (eventData.button == PointerEventData.InputButton.Right) {
            return;
        }

        //move object out of tile for dragging
        if (Ability != null) {
            //create icon for cursor to drag
            carryAbility = new GameObject();
            carryAbility.transform.SetParent(transform);

            //set rect transform data to that of this action bar tile
            RectTransform carryTransform = carryAbility.AddComponent<RectTransform>();
            carryTransform.anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            carryTransform.sizeDelta = GetComponent<RectTransform>().sizeDelta;
            carryTransform.eulerAngles = GetComponent<RectTransform>().eulerAngles;

            //set parent higher for proper drawing
            carryTransform.SetParent(transform.parent);

            //set carry sprites image
            carryAbility.AddComponent<AbilityTile>().ability = Ability;

            //set name and position
            carryAbility.transform.position = eventData.position;
            carryAbility.name = "carryAbility";

            //default tile sprite
            Icon = defaultSprite;
            Ability = null;
        }
    }

    public void OnDrag(PointerEventData eventData) {
        //escape if right click drag
        if (eventData.button == PointerEventData.InputButton.Right) {
            return;
        }

        //move carry along with cursor
        if (carryAbility != null) {
            carryAbility.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        //escape if right click drag
        if (eventData.button == PointerEventData.InputButton.Right) {
            return;
        }

        if (carryAbility != null) {
            //graphics raycaster
            GraphicRaycaster gRaycaster = FindObjectOfType<GraphicRaycaster>();
            List<RaycastResult> results = new List<RaycastResult>();

            gRaycaster.Raycast(eventData, results);
            foreach (RaycastResult raycastResult in results) {
                //find first actionbar tile in results
                if (raycastResult.gameObject.GetComponent<ActionBarTile>() != null) {
                    //create variable for tile
                    ActionBarTile tile = raycastResult.gameObject.GetComponent<ActionBarTile>();

                    //set tile variables
                    tile.Ability = carryAbility.GetComponent<AbilityTile>().ability;

                    break;
                }
            }

            //need a reset for when the result list does not find one
            //change carryAbility to an ability tile instead

            //clean up
            Destroy(carryAbility);
        }
    }
}