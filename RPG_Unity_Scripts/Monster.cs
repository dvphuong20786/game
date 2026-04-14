using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Gắn script này vào các cục hình học đại diện cho Quái vật
public class Monster : MonoBehaviour
{
    Animator anim;
    [Header("Chỉ số Quái vật")]
    public string monsterName = "Slime Nhỏ";
    public int maxHealth = 50;
    public int currentHealth;

    [Header("Chỉ số Tấn Công (Mới Thêm)")]
    public int attackDamage = 15; // Sức mạnh cắn
    public float attackRange = 1.5f; // Bán kính vòng tròn nó có thể với tới
    public float attackSpeed = 1.2f; // Giãn cách giữa 2 cú cắn (giây)
    public float moveSpeed = 1.5f; // Tốc độ chạy xé gió rượt theo bạn
    private float attackTimer = 0f;

    [Header("Phần thưởng")]
    public int expReward = 35; // Điểm kinh nghiệm cho người chơi khi chết
    public GameObject itemDropPrefab; // Bỏ vật phẩm (vd: Bình máu) vào ô này để nó rớt ra

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Đồng hồ bấm giờ trong game (Thời gian trôi qua)
        attackTimer += Time.deltaTime;

        // 1. Dò la tìm tung tích Người chơi
        PlayerStats player = FindAnyObjectByType<PlayerStats>();
        if (player != null)
        {
            // Đo khoảng cách giữa mình (Quái) và chữ Player
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            // TÍNH NĂNG MỚI: NẾU CHƯA CẮN ĐƯỢC THÌ RƯỢT THEO!
            if (distanceToPlayer > attackRange)
            {
                // Vẽ đường thẳng hướng thẳng tới vị trí người chơi và chạy tới
                Vector3 huongDi = (player.transform.position - transform.position).normalized;
                transform.Translate(huongDi * moveSpeed * Time.deltaTime);

                // Lật mặt quái vật hướng về phía người chơi
                if (huongDi.x > 0)
                    GetComponent<SpriteRenderer>().flipX = true; // Quay phải
                else if (huongDi.x < 0)
                    GetComponent<SpriteRenderer>().flipX = false; // Quay trái
            }
            // 2. Nếu con mồi lọt vào tầm đánh & Đã chờ đủ thời gian thì CẮN
            else if (attackTimer >= attackSpeed)
            {
                if (anim != null) anim.SetTrigger("Attack");
                player.TakeDamage(attackDamage);
                
                // Mở lại bảng Console ở dưới để xem chữ này nhé
                Debug.Log($"⚠️ OÁY! {monsterName} vừa ngoạm bạn mất {attackDamage} Máu!");

                // Reset đồng hồ về 0 để chờ 1.2 giây sau cắn hiệp tiếp theo
                attackTimer = 0f; 
            }
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

        // 1. Cống nạp EXP
        PlayerStats player = FindObjectOfType<PlayerStats>();
        if (player != null)
        {
            player.AddExp(expReward);
        }

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
