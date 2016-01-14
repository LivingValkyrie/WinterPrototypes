using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Author: Matt Gipson
/// Contact: deadwynn@gmail.com
/// very basic player script for a platformer-style game. 
/// </summary>
[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {
    #region Fields

    [Tooltip("How high you can jump")][SerializeField][HideInInspector]
    float maxJumpHeight = 4;
    [Tooltip( "The lowest you can jump" )]
    public float minJumpHeight = 1;
    [Tooltip("How long it takes you to reach the top of a jump")]
    public float timeToJumpApex = 0.4f;

    [Tooltip("The speed of the player")]
    public float moveSpeed = 6;


    [ExposeProperty]
    public float MaxJumpHeight {
        get { return maxJumpHeight; }
        set {
            if(value > 0){maxJumpHeight = value;}
            CalculateGravityAndJumpVelocity();
        }
    }

    //horizontal smoothing
    float accelerationTimeAirborn = .2f;
    float accelerationTimeGrounded = .1f;

    //wall jumping
    public float wallSlideSpeedMax = 3;
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    public float wallStickTime = .25f;
    float timeToWallRelease;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    Controller2D controller;

    #endregion

    void Start() {
        controller = GetComponent<Controller2D>();

        CalculateGravityAndJumpVelocity();
    }

    /// <summary>
    /// Calculates the gravity and jump velocity.
    /// </summary>
    void CalculateGravityAndJumpVelocity() {
        //calculate gravity
        gravity = -( 2 * maxJumpHeight ) / Mathf.Pow( timeToJumpApex, 2 );

        //calculate jump velocity
        maxJumpVelocity = Mathf.Abs( gravity ) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt( 2 * Mathf.Abs( gravity ) * minJumpHeight );

        //debug to see calulation
        print( "Gravity: " + gravity + " jump velocity: " + maxJumpVelocity );
    }

    void Update() {
        //take input
        Vector2 input = new Vector2( Input.GetAxisRaw( "Horizontal" ), Input.GetAxisRaw( "Vertical" ) );
        int wallDirX = ( controller.collisions.left ) ? -1 : 1;

        //smooth x movement based on ground or in the air
        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp( velocity.x,
                                       targetVelocityX,
                                       ref velocityXSmoothing,
                                       ( controller.collisions.below ) ? accelerationTimeGrounded : accelerationTimeAirborn );

        //initialize wallSliding to false each frame
        bool wallSliding = false;

        //if you meet the requirements of a wallslide
        if(( controller.collisions.left || controller.collisions.right ) && !controller.collisions.below && velocity.y < 0) {
            wallSliding = true;

            //if falling faster than slide speed cap at slidespeed
            if(velocity.y < -wallSlideSpeedMax) {
                velocity.y = -wallSlideSpeedMax;
            }

            //stick to wall at the start to assist in jumping away from the wall
            if(timeToWallRelease > 0) {
                //housekeeping
                velocityXSmoothing = 0;
                velocity.x = 0;

                //reset wall sticking aswell as do countdown
                if(input.x != wallDirX && input.x != 0) {
                    timeToWallRelease -= Time.deltaTime;
                } else {
                    timeToWallRelease = wallStickTime;
                }
            } else {
                timeToWallRelease = wallStickTime;
            }
        }

        //jump button
        if(Input.GetKeyDown( KeyCode.Space )) {
            if(wallSliding) {
                //jumping toward wall
                if(wallDirX == input.x) {
                    velocity.x = -wallDirX * wallJumpClimb.x;
                    velocity.y = wallJumpClimb.y;
                } else if(input.x == 0) { //jumping off wall
                    velocity.x = -wallDirX * wallJumpOff.x;
                    velocity.y = wallJumpOff.y;
                } else { //jumping away from wall
                    velocity.x = -wallDirX * wallLeap.x;
                    velocity.y = wallLeap.y;
                }
            }
            if(controller.collisions.below) { velocity.y = maxJumpVelocity; }
        }
        if(Input.GetKeyUp( KeyCode.Space )) {
            if(velocity.y > minJumpVelocity) {
                velocity.y = minJumpVelocity;
            }
        }

        //apply gravity
        velocity.y += gravity * Time.deltaTime;

        //move the player
        controller.Move( velocity * Time.deltaTime, input );

        //stop vertical movement if collision
        if ( controller.collisions.above || controller.collisions.below ) {
            velocity.y = 0;
        }

    }

}