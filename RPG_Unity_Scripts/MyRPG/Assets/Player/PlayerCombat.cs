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
    public float attackRange = 2f; 

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }


    // Update được gọi mỗi khung hình
    void Update()
    {
        // Nhận diện phím theo hệ thống Input mới của Unity 6
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame) Attack();
            if (Keyboard.current.digit1Key.wasPressedThisFrame) CastSkill();
        }
#else
        if (Input.GetKeyDown(KeyCode.Space)) Attack();
        if (Input.GetKeyDown(KeyCode.Alpha1)) CastSkill();
#endif
    }

    void CastSkill()
    {
        PlayerStats stats = GetComponent<PlayerStats>();
        if (stats == null || !stats.unlockedSkills.Contains("Chém Gió (Lv3)")) 
        {
            Debug.Log("Bạn chưa học kỹ năng này!");
            return;
        }

        Debug.Log("🔥 TUYỆT CHIÊU: CHÉM GIÓ!");
        if (anim != null) anim.SetTrigger("Attack");

        // Gây sát thương X2
        int skillDamage = (attackDamage + stats.bonusDamage) * 2;
        HitMonsters(skillDamage, attackRange + 1f); // Tầm đánh xa hơn chiêu thường
    }

    void Attack()
    {
        Debug.Log("Người chơi vung kiếm!");
        if (anim != null) anim.SetTrigger("Attack");

        int tongSatThuongThucTe = attackDamage;
        PlayerStats stats = GetComponent<PlayerStats>();
        if (stats != null) tongSatThuongThucTe += stats.bonusDamage;

        HitMonsters(tongSatThuongThucTe, attackRange);
    }

    void HitMonsters(int damage, float range)
    {
        Monster[] allMonsters = FindObjectsOfType<Monster>();
        foreach (Monster monster in allMonsters)
        {
            if (Vector2.Distance(transform.position, monster.transform.position) <= range)
            {
                monster.TakeDamage(damage);
            }
        }
    }

}
