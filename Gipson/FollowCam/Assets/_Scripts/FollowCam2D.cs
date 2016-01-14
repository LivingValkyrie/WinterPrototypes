using UnityEngine;
using System.Collections;

/// <summary>
/// Author: Matt Gipson
/// Contact: deadwynn@gmail.com
/// </summary>
[RequireComponent(typeof(Camera))]
public class FollowCam2D : MonoBehaviour {
    #region Fields

    //
    public GameObject followTarget;

    //following flags
    public bool followHorizontal, followVertical;
    public bool useBordersHorizontal, useBordersVertical;

    //border size
    public float borderLeft, borderRight;
    public float borderTop, borderBottom;

    //movement
    public float timeToCatchUp;
    Vector2 velocity;

    //hidden
    Camera cam;
    float xTarget, yTarget;
    bool catchingUpLeft, catchingUpRight;
    bool catchingUpTop, catchingUpBottom;

    //forgot my screen recording was running...

    #endregion

    #region	Build in Methods

    void Start() {
        cam = GetComponent<Camera>();
        xTarget = transform.position.x;
        yTarget = transform.position.y;
    }

    void Update() {
        Vector3 targetVPPos = cam.WorldToViewportPoint( followTarget.transform.position );
        Vector3 newPos = transform.position;
        if(followHorizontal) { FollowOnX( ref newPos, targetVPPos.x ); }
        if(followVertical) { FollowOnY( ref newPos, targetVPPos.y ); }
        transform.position = newPos;
    }

    #endregion

    #region	Custom Methods

    void FollowOnX(ref Vector3 newPos, float targetViewPortPos) {

        if(useBordersHorizontal) {

            if(targetViewPortPos > borderRight) {
                catchingUpRight = true;
                catchingUpLeft = false;
            } else if(targetViewPortPos < borderLeft) {
                catchingUpLeft = true;
                catchingUpRight = false;
            }

            if(catchingUpRight || catchingUpLeft) {
                xTarget = followTarget.transform.position.x;
            }

            if(xTarget >= transform.position.x) {
                print( "caught up on the left" );
                catchingUpLeft = false;
            } else if(xTarget <= transform.position.x) {
                print( "caught up on the right" );
                catchingUpRight = false;
            }
        } else {
            xTarget = followTarget.transform.position.x;
        }

        newPos.x = Mathf.SmoothDamp( newPos.x, xTarget, ref velocity.x, timeToCatchUp );

    }

    void FollowOnY( ref Vector3 newPos, float targetViewPortPos ) {
        if ( useBordersVertical ) {

            if ( targetViewPortPos > borderTop ) {
                catchingUpTop = true;
                catchingUpBottom = false;
            } else if ( targetViewPortPos < borderBottom ) {
                catchingUpBottom = true;
                catchingUpTop = false;
            }

            if ( catchingUpTop || catchingUpBottom ) {
                yTarget = followTarget.transform.position.y;
            }

            if ( yTarget >= transform.position.y ) {
                print( "caught up on the bottom" );
                catchingUpBottom = false;
            } else if ( yTarget <= transform.position.y ) {
                print( "caught up on the top" );
                catchingUpTop = false;
            }
        } else {
            yTarget = followTarget.transform.position.y;
        }

        newPos.y = Mathf.SmoothDamp( newPos.y, yTarget, ref velocity.y, timeToCatchUp );
    }

    #endregion
}

/**
 *  one variable for each border size: right, left, top, bottom (properties for failsafe values)
 *  gameObject for the followTarget
 *  booleans for each runtime method
 *  float speed
 *  
 *  maximum and minimum zoom. 
 *  
 *  update calls methods based on flags
 * 
 *  methods for:
 *      following in x
 *      following in y
 *      zoom, which checks camera type and processes accordingly
 *          if ortho, change size, if not, use z pos. 
 *      delayed swipe (float delay)(float delay, float speed)
 *      panAtSpeed(float speed)
 *          moves to the target at speed. example use: slow pans to the player when starting levels
 *      
 * 
 * 
 */