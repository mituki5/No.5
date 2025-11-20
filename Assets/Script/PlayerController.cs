using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("�ړ��ݒ�")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float doubleTapTime = 0.3f;

    [Header("�W�����v�ݒ�")]
    public float minJumpPower = 2f;
    public float maxJumpPower = 7f;
    public float chargeSpeed = 8f;   // �������ŗ͂����܂鑬�x

    [Header("�A�j���[�V����")]
    public Animator anim;

    private Rigidbody2D rb;
    private float lastTapTime = 0f;
    private float currentJumpPower = 0f;
    private bool isCharging = false;

    private float idleTimer = 0f;
    private float faceFrontTime = 5f;

    private int moveDirection = 0;
    private bool running = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleIdleFace();
    }

    void HandleMovement()
    {
        float horizontal = 0f;
        moveDirection = 0;

        if (Input.GetKey(KeyCode.A)) { horizontal = -1; moveDirection = -1; }
        if (Input.GetKey(KeyCode.D)) { horizontal = 1; moveDirection = 1; }

        // ��񉟂�����
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            if (Time.time - lastTapTime < doubleTapTime)
            {
                running = true;
            }
            lastTapTime = Time.time;
        }

        // ���������ĂȂ���Α������
        if (horizontal == 0)
        {
            running = false;
        }

        // ���ۂ̑��x
        float speed = running ? runSpeed : walkSpeed;
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);

        // �A�j���[�V����
        if (horizontal == 0)
        {
            anim.Play("Idle");
        }
        else
        {
            idleTimer = 0; // �����Ă�̂Ń��Z�b�g
            if (running) anim.Play("Run");
            else anim.Play("Walk");

            // �����ɂ���Ĕ��]
            Vector3 scale = transform.localScale;
            scale.x = (moveDirection == -1) ? -1 : 1;
            transform.localScale = scale;
        }
    }

    void HandleJump()
    {
        // �����n��
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isCharging = true;
            currentJumpPower = minJumpPower;
            anim.Play("Jump_Charge");
        }

        // ��������
        if (Input.GetKey(KeyCode.Space) && isCharging)
        {
            currentJumpPower += chargeSpeed * Time.deltaTime;
            if (currentJumpPower > maxJumpPower)
                currentJumpPower = maxJumpPower;
        }

        // �������u�ԂɃW�����v
        if (Input.GetKeyUp(KeyCode.Space) && isCharging)
        {
            isCharging = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentJumpPower);
            anim.Play("Jump_Up");
        }
    }

    void HandleIdleFace()
    {
        // �����Ă���Ȃ�J�E���g���Ȃ�
        if (moveDirection != 0)
        {
            idleTimer = 0f;
            return;
        }

        // �W�����v���̓J�E���g���Ȃ�
        if (!IsGrounded()) return;

        idleTimer += Time.deltaTime;

        if (idleTimer >= faceFrontTime)
        {
            anim.Play("Idle_Front");
        }
    }

    bool IsGrounded()
    {
        return rb.linearVelocity.y == 0;
    }
}
