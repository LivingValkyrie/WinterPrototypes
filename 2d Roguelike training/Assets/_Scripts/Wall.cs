using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Author: Matt Gipson
/// Contact: Deadwynn@gmail.com
/// Domain: www.livingvalkyrie.com
/// 
/// Description: Wall 
/// </summary>
public class Wall : MonoBehaviour {
    #region Fields

    public Sprite dmgSprite;
    public int hp = 4;

    SpriteRenderer spriteRenderer;

    #endregion

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void DamageWall(int loss) {
        spriteRenderer.sprite = dmgSprite;
        hp -= loss;
        if (hp <= 0) {
            gameObject.SetActive(false);
        }
    }

}