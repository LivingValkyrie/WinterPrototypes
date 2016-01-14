using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// Author: Matt Gipson
/// Contact: deadwynn@gmail.com
/// Moving platform for use with a platformer-style game. Uses a waypoint system to determine movement. 
/// </summary>
public class PlatformController : RaycastController {
    #region Fields

    [Tooltip("Layers that contain objects that can be moved by the platform")]
    public LayerMask passengerMask;

    [Tooltip("The speed at which the platform moves between waypoints")]
    public float speed;

    [Tooltip("Does the platform cycle through waypoints? If not the platform will reverse at the end")]
    public bool cyclic;

    [Tooltip("Ease movement between waypoints?")]
    public bool easedMovement;

    [Range(0, 2)]
    [Tooltip("The amount by which to ease.")]
    public float easeAmount;

    [Tooltip("How long the platform will wait at each waypoint before continuing to the next")]
    public float waitTime;

    float nextMoveTime;

    [Tooltip("The waypoints that the platofrm will travel between")]
    public Vector3[] localWaypoints;

    [HideInInspector]
    public Vector3[] globalWaypoints;

    //current waypoint
    int fromWaypointIndex;

    //percentage from 0-1 of the current position between current and next waypoint
    float percentBetweenWaypoints;

    //passenger info
    List<PassengerMovement> passengerMovement;

    //this will be used to make it so that the getComponent call will only be made once per passenger, improving performance
    Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();

    #endregion

    public override void Start() {
        base.Start();

        //set global waypoints based on local waypoints
        globalWaypoints = new Vector3[localWaypoints.Length];
        for(int i = 0; i < globalWaypoints.Length; i++) {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }

    void Update() {
        //update raycast origins
        UpdateRaycastOrigins();

        //calculates platform velocity then calculates how to affecct passengers
        Vector3 velocity = CalculatePlatformMovement();
        CalculatePassengerMovement( velocity );

        //moves passengers and itself
        MovePassengers( true );
        transform.Translate( velocity );
        MovePassengers( false );

    }

    /// <summary>
    /// Creates an ease value based on x^a / (x^a + 1-x^a) where a = x+1
    /// </summary>
    /// <param name="x">the value to be eased</param>
    /// <returns></returns>
    float Ease(float x) {
        float a = easeAmount + 1;
        return Mathf.Pow( x, a ) / ( Mathf.Pow( x, a ) + Mathf.Pow( 1 - x, a ) );
    }

    /// <summary>
    /// calculates how the platform should move based on waypoints, cyclical movement, waitTime and easing.
    /// </summary>
    /// <returns></returns>
    Vector3 CalculatePlatformMovement() {

        //if waitTime hasnt passed dont move
        if(Time.time < nextMoveTime) {
            return Vector3.zero;
        }

        //if starting waypoint is equal to the length of the array set it to 0
        fromWaypointIndex %= globalWaypoints.Length;
        //if next waypoint is equal to the length of the array + 1 set it to 0
        int toWaypointIndex = ( fromWaypointIndex + 1 ) % globalWaypoints.Length;
        //calculate the distance between waypoints
        float distanceBetweenWaypoints = Vector3.Distance( globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex] );
        //calculate the percentage of how far the platform is between waypoints
        percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;

        //calculate the new position on the platform based on whether easing is enabled or not
        Vector3 newPos;
        if(easedMovement) {
            //clamp precentBetweenWaypoints to allow proper easing
            percentBetweenWaypoints = Mathf.Clamp01( percentBetweenWaypoints );
            float easedPercentBetweenWaypoints = Ease( percentBetweenWaypoints );

            newPos = Vector3.Lerp( globalWaypoints[fromWaypointIndex],
                                   globalWaypoints[toWaypointIndex],
                                   easedPercentBetweenWaypoints );
        } else {
            newPos = Vector3.Lerp( globalWaypoints[fromWaypointIndex],
                                   globalWaypoints[toWaypointIndex],
                                   percentBetweenWaypoints );
        }

        //if waypoint has been reached
        if(percentBetweenWaypoints >= 1) {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;

            //if not cyclical reverse waypoint array
            if(!cyclic) {
                if(fromWaypointIndex >= globalWaypoints.Length - 1) {
                    fromWaypointIndex = 0;
                    System.Array.Reverse( globalWaypoints );
                }
            }

            //set the time at which the platform can be moved again
            nextMoveTime = Time.time + waitTime;
        }

        return newPos - transform.position;
    }

    /// <summary>
    /// Moves each passenger with its own move method.
    /// </summary>
    /// <param name="beforeMovePlatform">Whether this is being called before the platform is moved or not</param>
    void MovePassengers(bool beforeMovePlatform) {
        foreach(PassengerMovement passenger in passengerMovement) {
            //if the passenger has yet to be added to the dictionary do so
            if(!passengerDictionary.ContainsKey( passenger.transform )) {
                passengerDictionary.Add( passenger.transform, passenger.transform.GetComponent<Controller2D>() );
            }

            //call the move method for the passenger if it matches the method call (before or after platform movement)
            if(passenger.moveBeforePlatform == beforeMovePlatform) {
                passengerDictionary[passenger.transform].Move( passenger.velocity, passenger.standingOnPlatform );
            }
        }
    }

    /// <summary>
    /// calculates how the platform should affect the passengers velocity based on how the platform itself is moving.
    /// </summary>
    /// <param name="velocity">the platforms velocity</param>
    void CalculatePassengerMovement(Vector3 velocity) {
        //hashset of passengers, hashsets are best due to speed
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovement = new List<PassengerMovement>();

        float directionX = Mathf.Sign( velocity.x );
        float directionY = Mathf.Sign( velocity.y );

        //vertical platform
        if(velocity.y != 0) {
            //get the raylength, based on how fast you are moving + skinwidth
            float rayLength = Mathf.Abs( velocity.y ) + SKIN_WIDTH;

            //cast rays
            for(int i = 0; i < verticalRayCount; i++) {
                //decide raycast origin and cast ray
                Vector2 rayOrigin = ( directionY == -1 ) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * ( verticalRaySpacing * i );
                RaycastHit2D hit = Physics2D.Raycast( rayOrigin, Vector2.up * directionY, rayLength, passengerMask );

                //draw raycast gizmo for testing
                Debug.DrawRay( rayOrigin, Vector2.right * directionX * rayLength, Color.red );

                if(hit) {
                    //if passenger has not been moved yet
                    if(!movedPassengers.Contains( hit.transform )) {
                        //add to hashset
                        movedPassengers.Add( hit.transform );
                        Debug.Log( hit.transform.name );

                        //only add X velocity if passenger is ABOVE platform
                        float pushX = ( directionY == 1 ) ? velocity.x : 0;

                        //add vertical velocity
                        float pushY = velocity.y - ( hit.distance - SKIN_WIDTH ) * directionY;

                        //move passenger
                        passengerMovement.Add( new PassengerMovement( hit.transform,
                                                                      new Vector3( pushX, pushY ),
                                                                      directionY == 1,
                                                                      true ) );
                    }
                }
            }
        }

        //horizontal platform

        if(velocity.x != 0) {
            float rayLength = Mathf.Abs( velocity.x ) + SKIN_WIDTH;

            //cast rays
            for(int i = 0; i < horizontalRayCount; i++) {
                //decide raycast origin and cast ray
                Vector2 rayOrigin = ( directionX == -1 ) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * ( horizontalRaySpacing * i );
                RaycastHit2D hit = Physics2D.Raycast( rayOrigin, Vector2.right * directionX, rayLength, passengerMask );

                //draw raycast gizmo for testing
                Debug.DrawRay( rayOrigin, Vector2.right * directionX * rayLength, Color.blue );
                if(hit) {
                    //if passenger has not been moved yet
                    if(!movedPassengers.Contains( hit.transform )) {
                        //add to hashset
                        movedPassengers.Add( hit.transform );

                        //only add X velocity if passenger is ABOVE platform
                        float pushX = velocity.x - ( hit.distance - SKIN_WIDTH ) * directionX;

                        //add vertical velocity, -skinwidth allows the passenger to movedown and kno when it is grounded
                        float pushY = -SKIN_WIDTH;

                        //move passenger
                        passengerMovement.Add( new PassengerMovement( hit.transform, new Vector3( pushX, pushY ), false, true ) );
                    }
                }

            }

        }

        //if passenger is on top of horizontally or downward platform
        if(directionY == -1 || velocity.y == 0 && velocity.x != 0) {

            float rayLength = SKIN_WIDTH * 2;

            //cast rays
            for(int i = 0; i < verticalRayCount; i++) {
                //decide raycast origin and cast ray
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * ( verticalRaySpacing * i );
                RaycastHit2D hit = Physics2D.Raycast( rayOrigin, Vector2.up, rayLength, passengerMask );
                if(hit) {
                    //if passenger has not been moved yet
                    if(!movedPassengers.Contains( hit.transform )) {
                        //add to hashset
                        movedPassengers.Add( hit.transform );

                        //only add X velocity if passenger is ABOVE platform
                        float pushX = velocity.x;

                        //add vertical velocity
                        float pushY = velocity.y;

                        //move passenger
                        passengerMovement.Add( new PassengerMovement( hit.transform, new Vector3( pushX, pushY ), true, false ) );
                    }
                }
            }

        }

    }

    /// <summary>
    /// holds information about the passengers
    /// </summary>
    struct PassengerMovement {

        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;

        /// <summary>
        /// stores the information from the passenger
        /// </summary>
        /// <param name="_transform">the passengers transform</param>
        /// <param name="_velocity">the passengers velocity</param>
        /// <param name="_standingOnPlatform">whether the passenger is standing on the platform</param>
        /// <param name="_moveBeforePlatform"> whether the passenger should move before or after the platform</param>
        public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform) {
            transform = _transform;
            velocity = _velocity;
            standingOnPlatform = _standingOnPlatform;
            moveBeforePlatform = _moveBeforePlatform;
        }

    }

    /// <summary>
    /// draws gizmos of the local/global waypoint positions, aswell as numbering them. only works in the editor.
    /// </summary>
    void OnDrawGizmos() {
        if(localWaypoints != null) {
            Gizmos.color = Color.red;
            float size = 0.3f;

            for(int i = 0; i < localWaypoints.Length; i++) {
                Vector3 globalWaypointPos = ( Application.isPlaying )
                                                ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawLine( globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size );
                Gizmos.DrawLine( globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size );
                Handles.Label( globalWaypointPos, "waypoint " + ( i + 1 ) );

            }

        }
    }

}