using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Author: Matt Gipson
/// Contact: Deadwynn@gmail.com
/// Domain: www.livingvalkyrie.com
/// 
/// Description: Player 
/// </summary>
public class Player : MovingObject {
    #region Fields

    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;

    Animator animator;
    int food;

    #endregion

    protected override void Start() {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints;

        base.Start();
    }

    void OnDisable() {
        GameManager.instance.playerFoodPoints = food;
    }

    protected override void AttemptMove<TComponent>(int xDir, int yDir) {
        food--;
        base.AttemptMove<TComponent>(xDir, yDir);

        RaycastHit2D hit;
        CheckIfGameOver();

        GameManager.instance.playersTurn = false;
    }

    void CheckIfGameOver() {
        if (food <= 0) {
            GameManager.instance.GameOver();
        }
    }

    protected override void OnCantMove<TComponent>(TComponent component) {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }

    void Restart() {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void LoseFood(int loss) {
        animator.SetTrigger("playerHit");
        food -= loss;
        CheckIfGameOver();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Exit") {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }else if (other.tag == "Food") {
            food += pointsPerFood;
            other.gameObject.SetActive(false);
        } else if ( other.tag == "Soda" ) {
            food += pointsPerSoda;
            other.gameObject.SetActive( false );
        }
    }

    void Update() {
        if (!GameManager.instance.playersTurn) {
            return;
        }

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int) Input.GetAxisRaw("Horizontal");
        vertical = (int) Input.GetAxisRaw("Vertical");

        if (horizontal != 0) {
            vertical = 0;
        }

        if (horizontal != 0 || vertical != 0) {
            AttemptMove<Wall>(horizontal, vertical);
        }
    }

}