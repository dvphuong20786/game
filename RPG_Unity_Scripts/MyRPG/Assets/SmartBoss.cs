using UnityEngine;

// ===========================
// Gắn script này vào Boss cùng với Monster.cs
// Boss có 3 giai đoạn chiến đấu thông minh
// ===========================
public class SmartBoss : MonoBehaviour
{
    private Monster m;
    private PlayerStats player;
    private SpriteRenderer sr;

    private float timer = 0f;
    private int state = 0;
    // state 0: Đi bộ xáp lá cà
    // state 1: Gồng → Lướt tốc độ cao
    // state 2: Triệu hồi quái nhỏ

    private Vector3 dashTarget;
    private bool enraged = false;

    [Header("Cài đặt Boss")]
    public float walkSpeed = 1.8f;
    public float dashSpeed  = 18f;
    public float dashWindup = 3f; 
    public float enrageScale = 1.15f; // Chỉnh trong Unity: 1.15 là to lên một chút, 1.0 là không đổi
    public int   summonCount = 3; 
    public GameObject summonPrefab; 

    void Start()
    {
        m      = GetComponent<Monster>();
        sr     = GetComponent<SpriteRenderer>();
        player = FindAnyObjectByType<PlayerStats>();

        if (m != null)
        {
            m.moveSpeed = 0f; 
            Debug.Log($"💀 BOSS [{m.monsterName}] đã xuất hiện! HP: {m.maxHealth}");
        }
    }

    void Update()
    {
        if (m == null || player == null) return;
        if (m.currentHealth <= 0) return;

        // ===== ENRAGE MODE: Dưới 30% máu =====
        if (!enraged && m.currentHealth < m.maxHealth * 0.3f)
        {
            enraged = true;
            walkSpeed *= 2f;
            dashSpeed *= 1.5f;
            dashWindup = Mathf.Max(1f, dashWindup - 1f);
            if (sr != null) sr.color = Color.magenta;
            
            // Sử dụng biến cài đặt thay vì số cứng 1.3
            transform.localScale *= enrageScale;

            // Hiện thông báo
            if (GameUI.instance != null)
                GameUI.instance.ShowDamage(transform.position, "💢 ENRAGE!", Color.magenta);
            Debug.Log("💢 BOSS HÓA ĐIÊN! Tốc độ x2!");
        }

        timer += Time.deltaTime;

        switch (state)
        {
            case 0: StateWalk();    break;
            case 1: StateDash();    break;
            case 2: StateSummon();  break;
        }
    }

    // ===== STATE 0: Đi bộ + chờ gồng =====
    void StateWalk()
    {
        if (player == null) return;
        transform.position = Vector3.MoveTowards(
            transform.position,
            player.transform.position,
            walkSpeed * Time.deltaTime
        );

        // Lật mặt boss theo hướng đi
        if (sr != null)
        {
            float dx = player.transform.position.x - transform.position.x;
            sr.flipX = dx < 0;
        }

        // Sau dashWindup giây → chuyển sang Lướt
        if (timer >= dashWindup)
        {
            timer = 0f;
            state = 1;
            dashTarget = player.transform.position; // Khóa mục tiêu ngay lúc lao
            if (sr != null) sr.color = enraged ? Color.red : new Color(1f, 0.5f, 0f); // Nháy đỏ/cam
            if (GameUI.instance != null)
                GameUI.instance.ShowDamage(transform.position, "⚡ LẠO!", Color.yellow);
        }

        // Cứ 10 giây triệu hồi quái 1 lần (nếu có prefab)
        if (summonPrefab != null && timer > 8f && !enraged)
        {
            timer = 0f;
            state = 2;
        }
    }

    // ===== STATE 1: Lao lướt siêu tốc =====
    void StateDash()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            dashTarget,
            dashSpeed * Time.deltaTime
        );

        // Hết lao (tới đích hoặc quá 0.8 giây) → quay về đi bộ
        bool reached   = Vector3.Distance(transform.position, dashTarget) < 0.15f;
        bool timedOut  = timer >= 0.8f;

        if (reached || timedOut)
        {
            state = 0;
            timer = 0f;
            if (sr != null) sr.color = enraged ? Color.magenta : Color.white;
        }
    }

    // ===== STATE 2: Triệu hồi quái nhỏ =====
    void StateSummon()
    {
        if (summonPrefab != null)
        {
            for (int i = 0; i < summonCount; i++)
            {
                Vector3 spawnPos = transform.position + new Vector3(
                    Random.Range(-3f, 3f),
                    Random.Range(-3f, 3f),
                    0f
                );
                Instantiate(summonPrefab, spawnPos, Quaternion.identity);
            }
            if (GameUI.instance != null)
                GameUI.instance.ShowDamage(transform.position, "👾 Triệu hồi!", Color.cyan);
            Debug.Log($"👾 Boss triệu hồi {summonCount} quái nhỏ!");
        }

        // Xong → về đi bộ
        state = 0;
        timer = 0f;
    }

    // Vẽ vùng dash trong Scene View
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 5f);
    }
}
