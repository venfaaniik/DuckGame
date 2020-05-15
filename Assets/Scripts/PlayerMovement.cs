using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float speed = 1f;
    public float jumpSpeed = 3f;
    public bool groundCheck;
    public bool isSwinging;
    public Vector2 ropeHook;
    public float swingForce = 4f;

    private SpriteRenderer playerSprite;
    private Rigidbody2D rBody;
    private bool isJumping;
    private Animator animator;
    private float jumpInput;
    private float horizontalInput;

    public Sprite spriteWalking;
    public Sprite spriteJumping;

    void Awake() {
        playerSprite = GetComponent<SpriteRenderer>();
        rBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        this.GetComponent<SpriteRenderer>().sprite = spriteWalking;
    }

    void Update() {
        jumpInput = Input.GetAxis("Jump");
        horizontalInput = Input.GetAxis("Horizontal");
        var halfHeight = transform.GetComponent<SpriteRenderer>().bounds.extents.y;
        groundCheck = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - halfHeight - 0.04f), Vector2.down, 0.025f);
    }

    void FixedUpdate() {
        if (horizontalInput < 0f || horizontalInput > 0f) {
            animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
            playerSprite.flipX = horizontalInput < 0f;
            if (isSwinging) {
                this.GetComponent<SpriteRenderer>().sprite = spriteJumping;
                animator.SetBool("IsSwinging", true);

                // 1 - Get a normalized direction vector from the player to the hook point
                var playerToHookDirection = (ropeHook - (Vector2)transform.position).normalized;

                // 2 - Inverse the direction to get a perpendicular direction
                Vector2 perpendicularDirection;
                if (horizontalInput < 0) {
                    perpendicularDirection = new Vector2(-playerToHookDirection.y, playerToHookDirection.x);
                    var leftPerpPos = (Vector2)transform.position - perpendicularDirection * -2f;
                    Debug.DrawLine(transform.position, leftPerpPos, Color.green, 0f);
                }
                else {
                    perpendicularDirection = new Vector2(playerToHookDirection.y, -playerToHookDirection.x);
                    var rightPerpPos = (Vector2)transform.position + perpendicularDirection * 2f;
                    Debug.DrawLine(transform.position, rightPerpPos, Color.green, 0f);
                }

                var force = perpendicularDirection * swingForce;
                rBody.AddForce(force, ForceMode2D.Force);
            }
            else {
                animator.SetBool("IsSwinging", false);
                if (groundCheck) {
                    var groundForce = speed * 2f;
                    rBody.AddForce(new Vector2((horizontalInput * groundForce - rBody.velocity.x) * groundForce, 0));
                    rBody.velocity = new Vector2(rBody.velocity.x, rBody.velocity.y);
                    this.GetComponent<SpriteRenderer>().sprite = spriteWalking;
                }
            }
        }
        else {
            animator.SetBool("IsSwinging", false);
            animator.SetFloat("Speed", 0f);
            this.GetComponent<SpriteRenderer>().sprite = spriteWalking;


        }

        if (!isSwinging) {
            if (!groundCheck) {
                this.GetComponent<SpriteRenderer>().sprite = spriteJumping;
                return;
            }

            isJumping = jumpInput > 0f;
            if (isJumping) {
                this.GetComponent<SpriteRenderer>().sprite = spriteJumping;
                rBody.velocity = new Vector2(rBody.velocity.x, jumpSpeed);
            }
        }
    }
}