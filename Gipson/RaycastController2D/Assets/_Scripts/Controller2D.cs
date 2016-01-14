using UnityEngine;

/// <summary>
/// Author: Matt Gipson
/// Contact: deadwynn@gmail.com 
/// Controller2D is a raycastController build for the purposes of a platformer-style game. It handles collision detection,
/// including upward and downward slopes and whether to allow movement on them or to treat them as a vertical surface.
/// </summary>
public class Controller2D : RaycastController {
    #region Fields

    [Range(1, 90)]
    [Tooltip("The maximum angle of slope the controller can climb")]
    public float maxClimbAngle = 80;

    [Range(1, 90)]
    [Tooltip("The maximum angle of slope the controller can climb down smoothly")]
    public float maxDescendAgnle = 75;

    //all of the info of the controllers current collisions are stored here
    public CollisionInfo collisions;

    #endregion

    /// <summary>
    /// Initializes the rayCastController. must use an override method that calls the base start to avoid errors on children.
    /// </summary>
    public override void Start() {
        base.Start();
        collisions.faceDir = 1;
    }

    /// <summary>
    /// Moves based on the specified velocity after checking collisions.
    /// </summary>
    /// <param name="velocity">The velocity of the controller.</param>
    /// <param name="standingOnPlatform">if set to <c>true</c> [standing on platform]. used to force collisions.below in cases 
    /// where a collision below the controller wouldn't be read.</param>
    public void Move(Vector3 velocity, bool standingOnPlatform = false) {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.velocityOld = velocity;

        //set face dir
        if(velocity.x != 0) {
            collisions.faceDir = (int)Mathf.Sign(velocity.x);
        }

        //if falling check if you are descending a slope
        if(velocity.y < 0) { DescendSlope( ref velocity ); }

        //if moving in the X axis check collisions
        HorizontalCollisions( ref velocity ); 

        //if moving in the Y axis check collisions
        if(velocity.y != 0) { VerticalCollisions( ref velocity ); }

        //actually move the controller
        transform.Translate( velocity );

        //force collisions.below
        if(standingOnPlatform) {
            collisions.below = true;
        }
    }

    /// <summary>
    /// Checks Horizontal collisions and modifies the velocity if needed.
    /// </summary>
    /// <param name="velocity">The velocity of the controller.</param>
    void HorizontalCollisions(ref Vector3 velocity) {

        //get direction of movement
        float directionX = collisions.faceDir;

        //get the raylength, based on how fast you are moving + skinwidth
        float rayLength = Mathf.Abs( velocity.x ) + SKIN_WIDTH;

        //set ray incase of wall jumping
        if(Mathf.Abs( velocity.x ) < SKIN_WIDTH) {
            rayLength = 2 * SKIN_WIDTH;
        }

        //cast rays
        for(int i = 0; i < horizontalRayCount; i++) {
            //decide raycast origin
            Vector2 rayOrigin = ( directionX == -1 ) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * ( horizontalRaySpacing * i );

            //cast the ray
            RaycastHit2D hit = Physics2D.Raycast( rayOrigin, Vector2.right * directionX, rayLength, collisionMask );

            //draw raycast gizmo for testing
            Debug.DrawRay( rayOrigin, Vector2.right * directionX * rayLength, Color.blue );

            if(hit) {
                //if inside of the collision move to next ray
                if(hit.distance == 0) {
                    continue;
                }

                //get angle of surface
                float slopeAngle = Vector2.Angle( hit.normal, Vector2.up );

                //check if you can climb the slope
                if(i == 0 && slopeAngle <= maxClimbAngle) {
                    //if you were descending
                    if(collisions.descendingSlope) {
                        collisions.descendingSlope = false;
                        velocity = collisions.velocityOld;
                    }

                    //change slope angle smoothly
                    float distanceToSlopeStart = 0;
                    if(slopeAngle != collisions.slopeAngleOld) {
                        distanceToSlopeStart = hit.distance - SKIN_WIDTH;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }

                    //climb the slope
                    ClimbSlope( ref velocity, slopeAngle );
                    velocity.x += distanceToSlopeStart * directionX;
                }

                //side collisions that are not climbable
                if(!collisions.climbingSlope || slopeAngle > maxClimbAngle) {

                    //assign new velocity
                    velocity.x = ( hit.distance - SKIN_WIDTH ) * directionX;

                    //change rayLength to prevent missed collisions
                    rayLength = hit.distance;

                    //if already on a slope
                    if(collisions.climbingSlope) {
                        velocity.y = Mathf.Tan( collisions.slopeAngle * Mathf.Deg2Rad ) * Mathf.Abs( velocity.x );
                    }

                    //set collisionInfo variables
                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }

    /// <summary>
    /// Checks Vertical collisions and modifies the velocity if needed.
    /// </summary>
    /// <param name="velocity">The velocity of the controller.</param>
    void VerticalCollisions(ref Vector3 velocity) {

        //get direction of movement
        float directionY = Mathf.Sign( velocity.y );

        //get the raylength, based on how fast you are moving + skinwidth
        float rayLength = Mathf.Abs( velocity.y ) + SKIN_WIDTH;

        //cast rays
        for(int i = 0; i < verticalRayCount; i++) {
            //decide raycast origin
            Vector2 rayOrigin = ( directionY == -1 ) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * ( verticalRaySpacing * i + velocity.x );

            //cast the ray
            RaycastHit2D hit = Physics2D.Raycast( rayOrigin, Vector2.up * directionY, rayLength, collisionMask );

            //draw raycast gizmo for testing
            Debug.DrawRay( rayOrigin, Vector2.up * directionY * rayLength, Color.red );

            if(hit) {
                //assign new velocity
                velocity.y = ( hit.distance - SKIN_WIDTH ) * directionY;

                //change rayLength to prevent missed collisions
                rayLength = hit.distance;

                //if climbing a slope
                if(collisions.climbingSlope) {
                    velocity.x = velocity.y / Mathf.Tan( collisions.slopeAngle * Mathf.Deg2Rad ) * Mathf.Sign( velocity.x );
                }

                //set collisionInfo variables
                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }

        }

        //check for new slopes and adjust if found
        if(collisions.climbingSlope) {
            float directionX = Mathf.Sign( velocity.x );
            rayLength = Mathf.Abs( velocity.x ) + SKIN_WIDTH;
            Vector2 rayOrigin = ( ( directionX == -1 ) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight ) +
                                Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast( rayOrigin, Vector2.right * directionX, rayLength, collisionMask );

            if(hit) {
                float slopeAngle = Vector2.Angle( hit.normal, Vector2.up );
                if(slopeAngle != collisions.slopeAngle) {
                    velocity.x = ( hit.distance - SKIN_WIDTH ) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }

    /// <summary>
    /// Calculates the velocity of ascending a slope.
    /// </summary>
    /// <param name="velocity">The velocity of the controller.</param>
    /// <param name="slopeAngle">The angle of the slope.</param>
    void ClimbSlope(ref Vector3 velocity, float slopeAngle) {

        float moveDistance = Mathf.Abs( velocity.x );
        float climbVelocityY = Mathf.Sin( slopeAngle * Mathf.Deg2Rad ) * moveDistance;

        if(velocity.y <= climbVelocityY) {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos( slopeAngle * Mathf.Deg2Rad ) * moveDistance * Mathf.Sign( velocity.x );
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }

    /// <summary>
    /// Calculates the velocity of descending on a slope.
    /// </summary>
    /// <param name="velocity">The velocity.</param>
    void DescendSlope(ref Vector3 velocity) {
        float directionX = Mathf.Sign( velocity.x );
        Vector2 rayOrigin = ( directionX == -1 ) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast( rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask );

        if(hit) {
            float slopeAngle = Vector2.Angle( hit.normal, Vector2.up );

            //if there a slope below you?
            if(slopeAngle != 0 && slopeAngle <= maxDescendAgnle) {
                //are you facing the same direction as the slope?
                if(Mathf.Sign( hit.normal.x ) == directionX) {
                    //are you close enough for the slope to matter?
                    if(hit.distance - SKIN_WIDTH <= Mathf.Tan( slopeAngle * Mathf.Deg2Rad ) * Mathf.Abs( velocity.x )) {
                        float moveDistance = Mathf.Abs( velocity.x );
                        float descendVelocityY = Mathf.Sin( slopeAngle * Mathf.Deg2Rad ) * moveDistance;
                        velocity.x = Mathf.Cos( slopeAngle * Mathf.Deg2Rad ) * moveDistance * Mathf.Sign( velocity.x );
                        velocity.y -= descendVelocityY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Holds all of the controllers collision info.
    /// </summary>
    public struct CollisionInfo {

        //directional collision
        public bool above, below;
        public bool left, right;

        //slope collisions
        public bool climbingSlope, descendingSlope;

        //slope angle and the angle of the previous frame, used for changing slopes
        public float slopeAngle, slopeAngleOld;

        //previous velocity, used for special cases
        public Vector3 velocityOld;
        public int faceDir;

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset() {
            above = below = false;
            left = right = false;
            climbingSlope = descendingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }

    }

}