using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Gắn script này vào nhân vật Player cùng với PlayerStats
public class PlayerCombat : MonoBehaviour
{
    [Header("Thông số sức mạnh")]
    public int attackDamage = 10;   // YC#10: Giảm từ 25 → 10 cho cân bằng
    public float attackRange = 1.8f;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame) Attack();
            if (Keyboard.current.digit1Key.wasPressedThisFrame) CastSkill_ChemGio();
            if (Keyboard.current.digit2Key.wasPressedThisFrame) CastSkill_LoiDinh();
        }
#else
        if (Input.GetKeyDown(KeyCode.Space)) Attack();
        if (Input.GetKeyDown(KeyCode.Alpha1)) CastSkill_ChemGio();
        if (Input.GetKeyDown(KeyCode.Alpha2)) CastSkill_LoiDinh();
#endif
    }

    // ===== ĐÁNH THƯỜNG: Chỉ trúng 1 quái GẦN NHẤT =====
    // YC#9: Đánh thường KHÔNG đánh AOE, chỉ hit 1 mục tiêu gần nhất
    void Attack()
    {
        if (anim != null) anim.SetTrigger("Attack");

        int tongDame = attackDamage;
        PlayerStats stats = GetComponent<PlayerStats>();
        if (stats != null) tongDame += stats.bonusDamage;

        // Tìm 1 quái GẦN NHẤT trong tầm
        Monster target = FindNearestMonster(attackRange);
        if (target != null)
        {
            target.TakeDamage(tongDame);
            Debug.Log($"⚔️ Chém thường → {target.monsterName}: -{tongDame} dame");
        }
        else
        {
            Debug.Log("⚔️ Vung kiếm nhưng không trúng ai.");
        }
    }

    // ===== KỸ NĂNG 1: Chém Gió (Lv3) — ĐÁNH TẤT CẢ quái xung quanh =====
    // Phím số 1
    void CastSkill_ChemGio()
    {
        PlayerStats stats = GetComponent<PlayerStats>();
        if (stats == null || !stats.unlockedSkills.Contains("Chém Gió (Lv3)"))
        {
            Debug.Log("❌ Bạn chưa học Chém Gió! (Cần Lv3)");
            // Hiện thông báo trên màn hình
            if (GameUI.instance != null)
                GameUI.instance.ShowDamage(transform.position, "Chưa học kỹ năng!", Color.gray);
            return;
        }

        if (anim != null) anim.SetTrigger("Attack");

        // Chém Gió: AOE x2 dame, tầm xa hơn
        int skillDame = (attackDamage + stats.bonusDamage) * 2;
        HitAllMonstersInRange(skillDame, attackRange + 1.5f);
        if (GameUI.instance != null)
            GameUI.instance.ShowDamage(transform.position, "💨 CHÉM GIÓ!", Color.yellow);
        Debug.Log($"💨 CHÉM GIÓ! Đánh AOE {skillDame} dame trong tầm {attackRange + 1.5f}");
    }

    // ===== KỸ NĂNG 2: Lôi Đình (Lv6) — ĐÁNH TẤT CẢ, tầm rất rộng =====
    // Phím số 2
    void CastSkill_LoiDinh()
    {
        PlayerStats stats = GetComponent<PlayerStats>();
        if (stats == null || !stats.unlockedSkills.Contains("Lôi Đình (Lv6)"))
        {
            Debug.Log("❌ Bạn chưa học Lôi Đình! (Cần Lv6)");
            if (GameUI.instance != null)
                GameUI.instance.ShowDamage(transform.position, "Chưa học kỹ năng!", Color.gray);
            return;
        }

        if (anim != null) anim.SetTrigger("Attack");

        // Lôi Đình: AOE x3 dame, tầm cực rộng
        int skillDame = (attackDamage + stats.bonusDamage) * 3;
        HitAllMonstersInRange(skillDame, attackRange + 3f);
        if (GameUI.instance != null)
            GameUI.instance.ShowDamage(transform.position, "⚡ LÔI ĐÌNH!", new Color(0.5f, 0.5f, 1f));
        Debug.Log($"⚡ LÔI ĐÌNH! AOE {skillDame} dame trong tầm {attackRange + 3f}");
    }

    // ===== HÀM TÌM QUÁI GẦN NHẤT =====
    Monster FindNearestMonster(float range)
    {
        Monster[] allMonsters = FindObjectsByType<Monster>(FindObjectsSortMode.None);
        Monster nearest = null;
        float minDist = float.MaxValue;

        foreach (Monster m in allMonsters)
        {
            float dist = Vector2.Distance(transform.position, m.transform.position);
            if (dist <= range && dist < minDist)
            {
                minDist = dist;
                nearest = m;
            }
        }
        return nearest;
    }

    // ===== HÀM ĐÁNH TẤT CẢ TRONG TẦM (AOE — Dùng cho Kỹ năng) =====
    void HitAllMonstersInRange(int damage, float range)
    {
        Monster[] allMonsters = FindObjectsByType<Monster>(FindObjectsSortMode.None);
        int hitCount = 0;
        foreach (Monster m in allMonsters)
        {
            if (Vector2.Distance(transform.position, m.transform.position) <= range)
            {
                m.TakeDamage(damage);
                hitCount++;
            }
        }
        Debug.Log($"[AOE] Trúng {hitCount} quái.");
    }
}
