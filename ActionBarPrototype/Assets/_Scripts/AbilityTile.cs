using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;

/// <summary>
/// Author: Matt Gipson
/// Contact: Deadwynn@gmail.com
/// Domain: www.livingvalkyrie.com
/// 
/// Description: AbilityTile is a draggable tile that holds a set ability for actionbars
/// </summary>
public class AbilityTile : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
    #region Fields

    public Ability ability;

    Vector3 startingPos;

    #endregion

    public void OnBeginDrag(PointerEventData eventData) {
        print("started drag");
        startingPos = transform.position;
    }

    public void OnDrag(PointerEventData eventData) {
        print("dragging");
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData) {
        print("ended drag");

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

        transform.position = startingPos;
    }
}