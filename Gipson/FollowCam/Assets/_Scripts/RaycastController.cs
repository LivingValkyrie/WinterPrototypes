using UnityEngine;

/// <summary>
/// Author: Matt Gipson
/// Contact: deadwynn@gmail.com
/// RaycastController is a base class for using raycasting as a collision detection system. The rays
/// are inset by SKIN_WIDTH, so that rays can still be cast after collisions are detected. RaycastController doesn't 
/// have to be used for collision detection however, as it only gives the child the information needed to cast multiple rays
/// from its X and Y axis.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour {
    #region Fields

    [Tooltip("layers that this object will collide with. DO NOT SELECT THIS OBJECT'S LAYER.")]
    public LayerMask collisionMask;

    //how thick the skin around the bounds is. this allows for rays to be cast even when touching another surface.
    public const float SKIN_WIDTH = 0.015f;

    [Tooltip("The amount of horizontal rays cast, from top to bottom")]
    public int horizontalRayCount = 4;

    [Tooltip("The amount of horizontal rays cast, from left to right")]
    public int verticalRayCount = 4;

    [HideInInspector]
    public float horizontalRaySpacing;

    [HideInInspector]
    public float verticalRaySpacing;

    [HideInInspector]
    public BoxCollider2D collider;

    public RaycastOrigins raycastOrigins;

    #endregion

    /// <summary>
    /// Initializes the rayCastController. must use an override method that calls the base start to avoid errors on children.
    /// </summary>
    public virtual void Start() {
        collider = GetComponent<BoxCollider2D>();

        CalculateRaySpacing();
    }

    /// <summary>
    /// Updates the raycast origins. Call this whenenver you move the bounds.
    /// </summary>
    public void UpdateRaycastOrigins() {
        //get bounds of collider
        Bounds bounds = collider.bounds;

        //shrink bounds by skinWidth
        bounds.Expand( SKIN_WIDTH * -2 );

        //assign raycast origins to inner bounds
        raycastOrigins.bottomLeft = new Vector2( bounds.min.x, bounds.min.y );
        raycastOrigins.bottomRight = new Vector2( bounds.max.x, bounds.min.y );
        raycastOrigins.topLeft = new Vector2( bounds.min.x, bounds.max.y );
        raycastOrigins.topRight = new Vector2( bounds.max.x, bounds.max.y );
    }

    /// <summary>
    /// Calculates the ray spacing based on the size of the bouunds and how many rays are being cast. 
    /// call this whenever you change the amount of rays being cast by the controller.
    /// </summary>
    public void CalculateRaySpacing() {
        //get bounds of collider
        Bounds bounds = collider.bounds;

        //shrink bounds by skinWidth
        bounds.Expand( SKIN_WIDTH * -2 );

        //set ray counts, clamped to a minimum of 2, one for each corner
        horizontalRayCount = Mathf.Clamp( horizontalRayCount, 2, int.MaxValue );
        verticalRayCount = Mathf.Clamp( verticalRayCount, 2, int.MaxValue );

        //calculate spacing
        horizontalRaySpacing = bounds.size.y / ( horizontalRayCount - 1 );
        verticalRaySpacing = bounds.size.x / ( verticalRayCount - 1 );
    }

    /// <summary>
    /// holds origin points for the 4 corners of a bounds to be used in raycast projection
    /// </summary>
    public struct RaycastOrigins {

        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;

    }

}