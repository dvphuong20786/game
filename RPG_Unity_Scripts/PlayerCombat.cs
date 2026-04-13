using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Gắn script này vào nhân vật của bạn (Player) cùng với PlayerStats
public class PlayerCombat : MonoBehaviour
{
    [Header("Thông số sức mạnh")]
    public int attackDamage = 25;
    public float attackRange = 2f; // Tầm đánh xa hay gần

    // Update được gọi mỗi khung hình
    void Update()
    {
        // Nhận diện phím theo hệ thống Input mới của Unity 6
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Attack();
        }
#else
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
#endif
    }

    void Attack()
    {
        Debug.Log("Người chơi vung kiếm!");

        // Lấy thông tin Trang bị đang mặc để cộng dồn TỔNG LƯC CHIẾN
        int tongSatThuongThucTe = attackDamage;
        PlayerStats stats = GetComponent<PlayerStats>();
        if (stats != null) 
        {
            tongSatThuongThucTe += stats.bonusDamage;
        }

        // Tìm MỌI quái vật đang có trên màn hình (để đơn giản)
        Monster[] allMonsters = FindObjectsOfType<Monster>();

        foreach (Monster monster in allMonsters)
        {
            // Tính toán khoảng cách
            float distanceToMonster = Vector2.Distance(transform.position, monster.transform.position);

            // Nếu quái vật nằm TRONG tầm đánh
            if (distanceToMonster <= attackRange)
            {
                monster.TakeDamage(tongSatThuongThucTe); // Phóng dame sấm sét ra nè!
            }
        }
    }
}
