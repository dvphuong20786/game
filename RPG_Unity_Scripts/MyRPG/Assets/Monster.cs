using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Gắn script này vào các cục hình học đại diện cho Quái vật
public class Monster : MonoBehaviour
{
    [Header("Chỉ số Quái vật")]
    public string monsterName = "Slime Nhỏ";
    public int maxHealth = 50;
    int currentHealth;

    [Header("Chỉ số Tấn Công (Mới Thêm)")]
    public int attackDamage = 15; // Sức mạnh cắn
    public float attackRange = 1.5f; // Bán kính vòng tròn nó có thể với tới
    public float attackSpeed = 1.2f; // Giãn cách giữa 2 cú cắn (giây)
    private float attackTimer = 0f;

    [Header("Phần thưởng")]
    public int expReward = 35; // Điểm kinh nghiệm cho người chơi khi chết
    public GameObject itemDropPrefab; // Bỏ vật phẩm (vd: Bình máu) vào ô này để nó rớt ra

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        // Đồng hồ bấm giờ trong game (Thời gian trôi qua)
        attackTimer += Time.deltaTime;

        // 1. Dò la tìm tung tích Người chơi
        PlayerStats player = FindObjectOfType<PlayerStats>();
        if (player != null)
        {
            // Đo khoảng cách giữa mình (Quái) và chữ Player
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            // 2. Nếu con mồi lọt vào tầm đánh & Đã chờ đủ thời gian thì CẮN
            if (distanceToPlayer <= attackRange && attackTimer >= attackSpeed)
            {
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
}
