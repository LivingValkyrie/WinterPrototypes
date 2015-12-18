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

    GameObject carrySprite;

    #endregion

    void Start() {
        if (Icon == null) {
            Icon = defaultSprite;
        }
    }

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

    public void OnBeginDrag(PointerEventData eventData) {
        if (Ability != null) {
            //create icon for cursor to drag
            carrySprite = new GameObject();
            carrySprite.transform.SetParent(transform);

            //set rect transform data to that of this action bar tile
            RectTransform carryTransform = carrySprite.AddComponent<RectTransform>();
            carryTransform.anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            carryTransform.sizeDelta = GetComponent<RectTransform>().sizeDelta;
            carryTransform.eulerAngles = GetComponent<RectTransform>().eulerAngles;

            //set parent higher for proper drawing
            carryTransform.SetParent(transform.parent);

            //set carry sprites image
            carrySprite.AddComponent<Image>().sprite = Icon;

            //set name and position
            carrySprite.transform.position = eventData.position;
            carrySprite.name = "carrySprite";

            //default tile sprite
            Icon = defaultSprite;
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if (carrySprite != null) {
            carrySprite.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (carrySprite != null) {
            //graphics raycaster
            GraphicRaycaster gRaycaster = FindObjectOfType<GraphicRaycaster>();
            List<RaycastResult> results = new List<RaycastResult>();

            gRaycaster.Raycast(eventData, results);
            foreach (RaycastResult raycastResult in results) {
                print(raycastResult.gameObject.name);
                //find first actionbar tile in results
                if (raycastResult.gameObject.GetComponent<ActionBarTile>() != null) {
                    //create variable for tile
                    ActionBarTile tile = raycastResult.gameObject.GetComponent<ActionBarTile>();

                    //set tile variables
                    tile.Ability = Ability;

                    //reset this tile if target is a different tile
                    if (tile.transform != this.transform) {
                        ability = null;
                    }

                    break;
                }
            }

            //need a reset for when the result list does not find one
            //change carrySprite to an ability tile instead

            //clean up
            Destroy(carrySprite);
        }
    }
}