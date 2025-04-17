using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    private PhotonView photonView;
    private Rigidbody2D rb;
    private Animator animator;
    private PlayerCombat combat;
    public float moveSpeed = 5f;
    public float jumpForce = 6f;
    public float jumpCooldown = 0.5f;
    private float lastJumpTime;
    private bool isGrounded = true;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        combat = GetComponent<PlayerCombat>();
        if (photonView.IsMine)
        {
            CameraFollow camFollow = Camera.main.GetComponent<CameraFollow>();
            if (camFollow != null)
            {
                camFollow.target = this.transform;
            }
        }
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (!photonView.IsMine) return;
        if (combat != null && combat.isStunned) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);
        animator.SetBool("isMoving", Mathf.Abs(moveX) > 0.1f);
        Vector3 scale = transform.localScale;
        if (moveX > 0)
            scale.x = Mathf.Abs(scale.x);
        else if (moveX < 0)
            scale.x = -Mathf.Abs(scale.x);

        transform.localScale = scale;
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && Time.time > lastJumpTime + jumpCooldown)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            lastJumpTime = Time.time;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform"))
        {
            isGrounded = true;
            lastJumpTime = Time.time - jumpCooldown;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform"))
        {
            isGrounded = false;
        }
    }


}
