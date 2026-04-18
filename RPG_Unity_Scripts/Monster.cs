using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Gắn script này vào các cục hình học đại diện cho Quái vật
public class Monster : MonoBehaviour
{
    Animator anim;
    private SpriteRenderer sr; // Cache SpriteRenderer

    [Header("Chỉ số Quái vật")]
    public string monsterName = "Slime Nhỏ";
    public int maxHealth = 50;
    public int currentHealth;

    [Header("Chỉ số Tấn Công")]
    public int attackDamage = 15; // Sức mạnh cắn
    public float attackRange = 1.0f; // Tầm cắn (Đã giảm theo YC)
    public float attackSpeed = 1.2f; // Giãn cách giữa 2 cú cắn (giây)
    public float moveSpeed = 1.5f; // Tốc độ chạy rượt theo bạn
    private float attackTimer = 10f; // Cho phép đánh ngay khi vừa gặp lần đầu

    [Header("Cơ chế Lỗ (Pit Logic)")]
    private Vector3 pitHome;
    private float pitAggro;
    private float pitReset;
    private PlayerStats pitTarget;
    private bool isReturning = false;
    private bool isPatrolling = false;
    private Vector3 patrolTarget;
    private float patrolTimer = 0f;

    [Header("Phần thưởng")]
    public int expReward = 35; // Điểm kinh nghiệm cho người chơi khi chết
    public GameObject itemDropPrefab; // Bỏ vật phẩm (vd: Bình máu) vào ô này để nó rớt ra

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>(); // Cache tại đây để tối ưu hiệu năng
        pitHome = transform.position; // Mặc định nhà là nơi sinh ra
    }

    public void SetPitLogic(Vector3 home, float aggro, float reset, PlayerStats target)
    {
        pitHome = home;
        pitAggro = aggro;
        pitReset = reset;
        pitTarget = target;
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        // --- 1. XÁC ĐỊNH MỤC TIÊU ---
        PlayerStats target = pitTarget;
        if (target == null) target = FindNearestTarget(pitAggro > 0 ? pitAggro : 10f);

        // --- 2. KIỂM TRA PHẠM VI QUAY VỀ (Pit Logic) ---
        if (pitAggro > 0) 
        {
            float distToHome = Vector2.Distance(transform.position, pitHome);
            if (target != null)
            {
                float distToTargetFromHome = Vector2.Distance(target.transform.position, pitHome);
                if (distToTargetFromHome > pitReset) isReturning = true;
                else if (distToTargetFromHome < pitAggro) isReturning = false;
            }
            else
            {
                if (distToHome > 5f) isReturning = true;
            }
        }

        // --- 3. THỰC HIỆN HÀNH ĐỘNG ---
        if (isReturning)
        {
            Vector3 huongVe = (pitHome - transform.position).normalized;
            transform.Translate(huongVe * moveSpeed * Time.deltaTime);
            FlipSprite(huongVe.x);
            if (Vector2.Distance(transform.position, pitHome) < 0.5f) { isReturning = false; isPatrolling = true; }
        }
        else if (target != null && target.currentHealth > 0)
        {
            isPatrolling = false;
            float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);

            if (distanceToTarget > attackRange)
            {
                Vector3 huongDi = (target.transform.position - transform.position).normalized;
                transform.Translate(huongDi * moveSpeed * Time.deltaTime);
                FlipSprite(huongDi.x);
            }
            
            if (distanceToTarget <= attackRange + 0.2f && attackTimer >= attackSpeed)
            {
                if (anim != null) anim.SetTrigger("Attack");
                target.TakeDamage(attackDamage);
                attackTimer = 0f; 
            }
        }
        else if (pitAggro > 0) // --- LOGIC ĐI TUẦN (PATROL) ---
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= 3f || Vector2.Distance(transform.position, patrolTarget) < 0.2f)
            {
                patrolTarget = pitHome + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
                patrolTimer = 0;
                isPatrolling = true;
            }

            if (isPatrolling)
            {
                Vector3 huongDi = (patrolTarget - transform.position).normalized;
                transform.Translate(huongDi * (moveSpeed * 0.5f) * Time.deltaTime); 
                FlipSprite(huongDi.x);
            }
        }
    }

    PlayerStats FindNearestTarget(float range)
    {
        PlayerStats[] all = Object.FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
        PlayerStats nearest = null;
        float minD = range;

        foreach (var p in all)
        {
            if (p.currentHealth <= 0) continue;
            float d = Vector2.Distance(transform.position, p.transform.position);
            if (d < minD) { minD = d; nearest = p; }
        }
        return nearest;
    }

    void FlipSprite(float xDir)
    {
        if (sr != null)
        {
            if (xDir > 0) sr.flipX = true;
            else if (xDir < 0) sr.flipX = false;
        }
    }

    // Hàm nhận sát thương (Người chơi chém)
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        // Hiện số máu bị trừ bay lên trên đầu Quái
        if (GameUI.instance != null) GameUI.instance.ShowDamage(transform.position, "-" + damage, Color.white);

        Debug.Log(monsterName + " bị chém " + damage + " máu! Còn lại: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Quái vật " + monsterName + " đã CHẾT!");

        // 1. Chia sẻ EXP cho đội ngũ
        PlayerStats.ShareExp(expReward);

        // 2. Rớt đồ ra đất (Prefab)
        if (itemDropPrefab != null)
        {
            Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
            Debug.Log("Quái vật đã rớt ra chiến lợi phẩm!");
        }

        // 3. Tiêu hủy cái xác
        Destroy(gameObject);
    }


    // Vẽ thanh máu nổi bồng bềnh trên đỉnh đầu con quái vật
    void OnGUI()
    {
        // Nhờ Camera dịch tọa độ 2D của quái vật thành tọa độ thực trên màn hình máy tính
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(transform.position);

            // Phải đảm bảo quái vật đang nằm trong khung hình thì mới vẽ
            if (screenPos.z > 0 && currentHealth > 0)
            {
                // Màn hình giao diện GUI lộn ngược trục Y so với game, nên phải trừ đi
                float screenY = Screen.height - screenPos.y;

                // Kích thước thanh máu: ngang 60, dọc 8 (Nhỏ ríu). Cách đỉnh đầu 50 đơn vị.
                float barWidth = 60f;
                float barHeight = 8f;
                float yOffset = 50f; 
                float startX = screenPos.x - (barWidth / 2);
                float startY = screenY - yOffset;

                // 1. Vẽ Viền/Đáy đen
                GUI.color = Color.black;
                GUI.DrawTexture(new Rect(startX, startY, barWidth, barHeight), Texture2D.whiteTexture);

                // 2. Vẽ Lõi đỏ
                GUI.color = Color.red;
                float heathRatio = (float)currentHealth / maxHealth;
                GUI.DrawTexture(new Rect(startX, startY, barWidth * heathRatio, barHeight), Texture2D.whiteTexture);
                
                // Khôi phục màu gốc để không dính sang UI khác
                GUI.color = Color.white;
            }
        }
    }
}
