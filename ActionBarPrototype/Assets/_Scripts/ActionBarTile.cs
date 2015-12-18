using UnityEngine;
using System.Collections;
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

    public void OnBeginDrag( PointerEventData eventData ) {
        //create icon for cursor to drag
        carrySprite = new GameObject();
        carrySprite.transform.SetParent(transform);

        //set rect transform data to that of this action bar tile
        RectTransform carryTransform = carrySprite.AddComponent<RectTransform>();
        carryTransform.anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        carryTransform.sizeDelta = GetComponent<RectTransform>().sizeDelta;
        carryTransform.eulerAngles = GetComponent<RectTransform>().eulerAngles;

        //set carry sprites image
        carrySprite.AddComponent<Image>().sprite = Icon;

        carrySprite.transform.position = eventData.position;
        carrySprite.name = "carrySprite";

        Icon = defaultSprite;
    }

    public void OnDrag(PointerEventData eventData) {
        carrySprite.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData) {
        ability = null;
        Icon = defaultSprite;
        Destroy(carrySprite);

        //modify to support placing tile in slots
    }
}