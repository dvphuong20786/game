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

    [Header("Phần thưởng")]
    public int expReward = 35; // Điểm kinh nghiệm cho người chơi khi chết
    public GameObject itemDropPrefab; // Bỏ vật phẩm (vd: Bình máu) vào ô này để nó rớt ra

    void Start()
    {
        currentHealth = maxHealth;
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

        // 1. Tìm người chơi để cống nạp EXP
        PlayerStats player = FindObjectOfType<PlayerStats>();
        if (player != null)
        {
            player.AddExp(expReward);
        }

        // 2. Rớt đồ ra đất ngay tại vị trí của con quái vừa chết
        if (itemDropPrefab != null)
        {
            Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
            Debug.Log("Quái vật đã rớt ra một món đồ!");
        }

        // 3. Tiêu hủy chính vật thể này khởi màn hình đồ họa
        Destroy(gameObject);
    }
}
