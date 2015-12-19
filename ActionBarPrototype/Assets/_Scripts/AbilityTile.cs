using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Author: Matt Gipson
/// Contact: Deadwynn@gmail.com
/// Domain: www.livingvalkyrie.com
/// 
/// Description: AbilityTile is a draggable tile that holds a set ability for actionbars
/// </summary>
[RequireComponent(typeof(Image))]
public class AbilityTile : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
    #region Fields

    public Ability ability;
    public Sprite icon;

    Vector3 startingPos;

    #endregion

    void Start() {
        icon = ability.icon;
        GetComponent<Image>().sprite = icon;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        //escape if right click drag
        if (eventData.button == PointerEventData.InputButton.Right) {
            return;
        }

        startingPos = transform.position;
    }

    public void OnDrag(PointerEventData eventData) {
        //escape if right click drag
        if ( eventData.button == PointerEventData.InputButton.Right) {
            return;
        }

        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData) {
        //escape if right click drag
        if (eventData.button == PointerEventData.InputButton.Right) {
            return;
        }

        //graphics raycaster
        GraphicRaycaster gRaycaster = FindObjectOfType<GraphicRaycaster>();
        List<RaycastResult> results = new List<RaycastResult>();

        gRaycaster.Raycast(eventData, results);
        foreach (RaycastResult raycastResult in results) {
            //print(raycastResult.gameObject.name);
            //find first actionbar tile in results
            if (raycastResult.gameObject.GetComponent<ActionBarTile>() != null) {
                //create variable for tile
                ActionBarTile tile = raycastResult.gameObject.GetComponent<ActionBarTile>();

                //set tile variables
                tile.Ability = ability;

                break;

            }
            transform.position = startingPos;
        }
    }
}

#region Failed attempts

//raycasting doesnt work
//Ray ray = Camera.main.ScreenPointToRay(eventData.position);
////Used to get information back from a raycast
//RaycastHit hit;
////Determine if our raycast hits anything
//if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
//    print(hit.transform.name);
//}

//hover stack doesnt work
//foreach (GameObject o in eventData.hovered) {
//        print(o.name);
//    if (o.GetComponent<ActionBarTile>() != null) {}
//}

#endregion