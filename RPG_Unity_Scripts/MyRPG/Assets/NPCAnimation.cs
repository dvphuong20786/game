using UnityEngine;

// ===========================
// Điều khiển hoạt ảnh NPC: Đứng im, Đi bộ, Chào hỏi
// ===========================
public class NPCAnimation : MonoBehaviour
{
    private Animator anim;
    private PlayerStats player;
    private SpriteRenderer sr;

    [Header("Cài đặt di chuyển")]
    public bool canPatrol = true; // Tích vào nếu muốn NPC đi qua đi lại
    public float moveSpeed = 1.2f;
    public float patrolDistance = 3f;
    
    private Vector3 startPos;
    private bool walkingRight = true;
    private float greetTimer = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        startPos = transform.position;
        player = FindAnyObjectByType<PlayerStats>();
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.transform.position);

        // --- 1. LOGIC CHÀO HỎI (GREET) ---
        if (dist < 3f && greetTimer <= 0f)
        {
            if (anim != null) anim.SetTrigger("Greet");
            Debug.Log("👋 NPC: Xin chào hiệp sĩ!");
            greetTimer = 8f; // 8 giây sau mới chào lại lần nữa
        }
        if (greetTimer > 0) greetTimer -= Time.deltaTime;

        // --- 2. LOGIC DI CHUYỂN (WALK/IDLE) ---
        if (canPatrol && dist > 3.5f) // Chỉ đi lại khi người chơi đứng xa
        {
            float currentDist = transform.position.x - startPos.x;

            if (walkingRight && currentDist >= patrolDistance) walkingRight = false;
            else if (!walkingRight && currentDist <= -patrolDistance) walkingRight = true;

            float dir = walkingRight ? 1 : -1;
            transform.Translate(Vector3.right * dir * moveSpeed * Time.deltaTime);

            if (sr != null) sr.flipX = !walkingRight;
            if (anim != null) anim.SetBool("IsWalking", true);
        }
        else
        {
            if (anim != null) anim.SetBool("IsWalking", false);
        }
    }
}
