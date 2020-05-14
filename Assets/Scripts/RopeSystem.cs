using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using System.Linq;

public class RopeSystem : MonoBehaviour {
    public GameObject ropeHingeAnchor;
    public DistanceJoint2D ropeJoint;
    public Transform crosshair;
    public SpriteRenderer crosshairSprite;
    public PlayerMovement playerMovement;
    public LineRenderer ropeRenderer;
    public LayerMask ropeLayerMask;
    public float ropeMaxCastDistance = 30f;

    private bool ropeAttached;
    private Vector2 playerPosition;
    private Rigidbody2D ropeHingeAnchorRb;
    private SpriteRenderer ropeHingeAnchorSprite;
    private List<Vector2> ropePositions = new List<Vector2>();
    private bool distanceSet;
    //private float mouseDstFromPlayer;
    private Dictionary<Vector2, int> wrapPointsLookup = new Dictionary<Vector2, int>();

    void Awake() {
        ropeJoint.enabled = false;
        playerPosition = transform.position;
        ropeHingeAnchorRb = ropeHingeAnchor.GetComponent<Rigidbody2D>();
        ropeHingeAnchorSprite = ropeHingeAnchor.GetComponent<SpriteRenderer>();
    }

    void Update() {
        var worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        //mouseDstFromPlayer = Vector2.Distance(worldMousePosition, transform.position);
        var facingDirection = worldMousePosition - transform.position;
        var aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
        if (aimAngle < 0f) {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }

        var aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;
        playerPosition = transform.position;

        if (!ropeAttached) {
            SetCrosshairPosition(aimAngle);
        }
        else {
            crosshairSprite.enabled = false;
            // 1
            if (ropePositions.Count > 0) {
                // 2
                var lastRopePoint = ropePositions.Last();
                var playerToCurrentNextHit = Physics2D.Raycast(playerPosition, (lastRopePoint - playerPosition).normalized, Vector2.Distance(playerPosition, lastRopePoint) - 0.1f, ropeLayerMask);

                // 3
                if (playerToCurrentNextHit) {
                    var colliderWithVertices = playerToCurrentNextHit.collider as PolygonCollider2D;
                    if (colliderWithVertices != null) {
                        var closestPointToHit = GetClosestColliderPointFromRaycastHit(playerToCurrentNextHit, colliderWithVertices);

                        // 4
                        if (wrapPointsLookup.ContainsKey(closestPointToHit)) {
                            ResetRope();
                            return;
                        }

                        // 5
                        ropePositions.Add(closestPointToHit);
                        wrapPointsLookup.Add(closestPointToHit, 0);
                        distanceSet = false;
                    }
                }
            }
        }
        HandleInput(aimDirection);
        UpdateRopePositions();
    }

    private void SetCrosshairPosition(float aimAngle) {
        if (!crosshairSprite.enabled) {
            crosshairSprite.enabled = true;
        }
        //var x = transform.position.x + 10f * Mathf.Cos(aimAngle);
        //var y = transform.position.y + 10f * Mathf.Sin(aimAngle);

        //var crossHairPosition = new Vector3(x, y, 0);
        var crossHairPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        crosshair.transform.position = crossHairPosition;
    }

    private void HandleInput(Vector2 aimDirection) {

        var shit = Physics2D.Raycast(playerPosition, aimDirection, ropeMaxCastDistance, ropeLayerMask);
        if (shit.collider != null) {
            crosshairSprite.color = Color.green;
        }
        else {
            crosshairSprite.color = Color.red;
        }

        if (Input.GetMouseButtonDown(0)) {
            if (ropeAttached) return;
            ropeRenderer.enabled = true;

            var hit = Physics2D.Raycast(playerPosition, aimDirection, ropeMaxCastDistance, ropeLayerMask);

            if (hit.collider != null) {
                ropeAttached = true;
                if (!ropePositions.Contains(hit.point)) {
                    // Jump slightly to distance the player a little from the ground after grappling to something.
                    transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 2f), ForceMode2D.Impulse);
                    ropePositions.Add(hit.point);
                    ropeJoint.distance = Vector2.Distance(playerPosition, hit.point);
                    ropeJoint.enabled = true;
                    ropeHingeAnchorSprite.enabled = true;
                }
            }
            else {
                ropeRenderer.enabled = false;
                ropeAttached = false;
                ropeJoint.enabled = false;
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            ResetRope();
        }
    }

    private void ResetRope() {
        ropeJoint.enabled = false;
        ropeAttached = false;
        playerMovement.isSwinging = false;
        ropeRenderer.positionCount = 2;
        ropeRenderer.SetPosition(0, transform.position);
        ropeRenderer.SetPosition(1, transform.position);
        ropePositions.Clear();
        ropeHingeAnchorSprite.enabled = false;
        wrapPointsLookup.Clear();
    }

    private void UpdateRopePositions() {
        if (!ropeAttached) {
            return;
        }
        ropeRenderer.positionCount = ropePositions.Count + 1;

        for (var i = ropeRenderer.positionCount - 1; i >= 0; i--) {
            if (i != ropeRenderer.positionCount - 1) // if not the Last point of line renderer
            {
                ropeRenderer.SetPosition(i, ropePositions[i]);
                if (i == ropePositions.Count - 1 || ropePositions.Count == 1) {
                    var ropePosition = ropePositions[ropePositions.Count - 1];
                    if (ropePositions.Count == 1) {
                        ropeHingeAnchorRb.transform.position = ropePosition;
                        if (!distanceSet) {
                            ropeJoint.distance = Vector2.Distance(transform.position, ropePosition);
                            distanceSet = true;
                        }
                    }
                    else {
                        ropeHingeAnchorRb.transform.position = ropePosition;
                        if (!distanceSet) {
                            ropeJoint.distance = Vector2.Distance(transform.position, ropePosition);
                            distanceSet = true;
                        }
                    }
                }
                else if (i - 1 == ropePositions.IndexOf(ropePositions.Last())) {
                    var ropePosition = ropePositions.Last();
                    ropeHingeAnchorRb.transform.position = ropePosition;
                    if (!distanceSet) {
                        ropeJoint.distance = Vector2.Distance(transform.position, ropePosition);
                        distanceSet = true;
                    }
                }
            }
            else {
                ropeRenderer.SetPosition(i, transform.position);
            }
        }
    }

    //this weird ass shit
    private Vector2 GetClosestColliderPointFromRaycastHit(RaycastHit2D hit, PolygonCollider2D polyCollider) {
        var distanceDictionary = polyCollider.points.ToDictionary<Vector2, float, Vector2>(
            position => Vector2.Distance(hit.point, polyCollider.transform.TransformPoint(position)),
            position => polyCollider.transform.TransformPoint(position));

        var orderedDictionary = distanceDictionary.OrderBy(e => e.Key);
        return orderedDictionary.Any() ? orderedDictionary.First().Value : Vector2.zero;
    }
}
