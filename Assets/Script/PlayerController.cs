using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float maxJumpHoldTime = 0.5f;

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private float jumpHoldTimer = 0f;

    private StageManager stageManager;
    private int myStageIndex = 0;

    public bool hasItem = false;

    private Vector3 respawnPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(StageManager manager, int stageIndex)
    {
        stageManager = manager;
        myStageIndex = stageIndex;
    }

    void Update()
    {
        HandleMovementInput();
    }

    private void HandleMovementInput()
    {
        float h = 0f;
        if (Input.GetKey(KeyCode.A)) h = -1f;
        else if (Input.GetKey(KeyCode.D)) h = 1f;
        rb.linearVelocity = new Vector2(h * moveSpeed, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpHoldTimer = 0f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }
        if (Input.GetKey(KeyCode.Space) && !isGrounded)
        {
            jumpHoldTimer += Time.deltaTime;
            if (jumpHoldTimer < maxJumpHoldTime)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }
    }

    public void SetRespawnToHouse()
    {
        if (stageManager != null)
            respawnPosition = stageManager.GetRespawnPositionForStage(myStageIndex);
    }

    public void DieAndRespawn()
    {
        Vector3 newPos = stageManager != null ? stageManager.GetRespawnPositionForStage(myStageIndex) : respawnPosition;
        transform.position = newPos;
        rb.linearVelocity = Vector2.zero;
        hasItem = false;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Ground") || col.collider.CompareTag("Platform"))
        {
            isGrounded = true;
        }

        if (col.collider.CompareTag("Enemy"))
        {
            DieAndRespawn();
        }

    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.collider.CompareTag("Ground") || col.collider.CompareTag("Platform"))
        {
            isGrounded = false;
        }
    }

    public void OnPickedItem()
    {
        hasItem = true;
    }
}
