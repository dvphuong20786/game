using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// ===========================
// HỆ THỐNG CHIẾN ĐẤU NGƯỜI CHƠI (NÂNG CẤP 4 SLOTS)
// Cho phép sử dụng mọi loại kỹ năng (Tấn công, Buff, Heal) bằng phím 1, 2, 3, 4
// ===========================
public class PlayerCombat : MonoBehaviour
{
    [Header("Thông số tấn công cơ bản")]
    public int attackDamage = 10;
    public float attackRange = 1.2f;

    private Animator anim;
    private PlayerStats stats;

    [Header("4 Ô Kỹ Năng (Kéo Asset vào đây)")]
    public SkillData[] equippedSkills = new SkillData[4];

    // Quản lý hồi chiêu theo từng Slot
    private float[] skillCooldowns = new float[4];

    // Biến cho hiệu ứng Buff tạm thời
    private float buffTimer = 0f;
    private int buffDefAmount = 0;

    void Start()
    {
        anim = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        // 1. Cập nhật đếm ngược hồi chiêu
        for (int i = 0; i < 4; i++)
        {
            if (skillCooldowns[i] > 0) skillCooldowns[i] -= Time.deltaTime;
        }

        // 2. Cập nhật thời gian hiệu lực của Buff
        if (buffTimer > 0)
        {
            buffTimer -= Time.deltaTime;
            if (buffTimer <= 0) {
                stats.bonusDefense -= buffDefAmount;
                buffDefAmount = 0;
                if (GameUI.instance != null) GameUI.instance.ShowDamage(transform.position, " hết Buff!", Color.gray);
            }
        }

        // 3. Phân tích phím bấm
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame) Attack();
            if (Keyboard.current.digit1Key.wasPressedThisFrame) TryCastSkill(0);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) TryCastSkill(1);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) TryCastSkill(2);
            if (Keyboard.current.digit4Key.wasPressedThisFrame) TryCastSkill(3);
        }
#else
        if (Input.GetKeyDown(KeyCode.Space)) Attack();
        if (Input.GetKeyDown(KeyCode.Alpha1)) TryCastSkill(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TryCastSkill(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TryCastSkill(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) TryCastSkill(3);
#endif
    }

    // Đánh thường (Gần nhất)
    void Attack()
    {
        if (anim != null) anim.SetTrigger("Attack");
        int finalDmg = attackDamage + (stats != null ? stats.bonusDamage : 0);

        Monster target = FindNearestMonster(attackRange);
        if (target != null) target.TakeDamage(finalDmg);
    }

    // Kiểm tra và tung kỹ năng
    void TryCastSkill(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= equippedSkills.Length) return;
        SkillData skill = equippedSkills[slotIndex];

        if (skill == null) return;

        // Kiểm tra Level
        int sLv = (stats != null) ? stats.GetSkillLevel(skill.skillName) : 0;
        if (sLv <= 0)
        {
            if (GameUI.instance != null) GameUI.instance.ShowDamage(transform.position, "Chưa học kỹ năng!", Color.gray);
            return;
        }

        // Kiểm tra Hồi chiêu
        if (skillCooldowns[slotIndex] > 0)
        {
            if (GameUI.instance != null) GameUI.instance.ShowDamage(transform.position, $"⌛ {skillCooldowns[slotIndex]:F1}s", Color.white);
            return;
        }

        // TUNG CHIÊU
        ExecuteSkill(skill, sLv);
        skillCooldowns[slotIndex] = skill.baseCooldown;
    }

    void ExecuteSkill(SkillData skill, int level)
    {
        if (anim != null) anim.SetTrigger("Attack");

        string sName = skill.skillName;

        // A. KỸ NĂNG TẤN CÔNG (Gây AOE)
        if (sName.Contains("Chém Gió") || sName.Contains("Lôi Đình")) {
            int dameGoc = attackDamage + (stats != null ? stats.bonusDamage : 0);
            float mult = skill.baseDamageMultiplier + (level * skill.damageIncreasePerLevel);
            int skillDame = (int)(dameGoc * mult);
            float rangeAdd = sName.Contains("Lôi Đình") ? 3f : 1.5f;
            float range = attackRange + rangeAdd + (level * skill.rangeIncreasePerLevel);
            
            HitAllMonstersInRange(skillDame, range);
            ShowSkillFX(sName, Color.yellow);
        }
        // B. KỸ NĂNG HỒI MÁU (Tri Thương)
        else if (sName.Contains("Trị Thương")) {
            int heal = skill.baseHealOrDef + (level * skill.valueIncreasePerLevel);
            stats.currentHealth = Mathf.Min(stats.currentHealth + heal, stats.maxHealth);
            ShowSkillFX("HỒI MÁU!", Color.green);
        }
        // C. KỸ NĂNG TĂNG THỦ (Hộ Vệ)
        else if (sName.Contains("Hộ Vệ")) {
            int def = skill.baseHealOrDef + (level * skill.valueIncreasePerLevel);
            // Cộng giáp tạm thời
            if (buffTimer > 0) stats.bonusDefense -= buffDefAmount; // Reset buff cũ nếu đang có
            buffDefAmount = def;
            stats.bonusDefense += buffDefAmount;
            buffTimer = 10f; // Buff trong 10 giây
            ShowSkillFX("GIÁP HỘ VỆ!", Color.cyan);
        }
    }

    void ShowSkillFX(string text, Color col)
    {
        if (GameUI.instance != null) GameUI.instance.ShowDamage(transform.position, text, col);
    }

    Monster FindNearestMonster(float range)
    {
        Monster[] all = FindObjectsByType<Monster>(FindObjectsSortMode.None);
        Monster nearest = null; float minDist = float.MaxValue;
        foreach (Monster m in all) {
            if (m.isAlly) continue;
            float d = Vector2.Distance(transform.position, m.transform.position);
            if (d <= range && d < minDist) { minDist = d; nearest = m; }
        }
        return nearest;
    }

    void HitAllMonstersInRange(int damage, float range)
    {
        Monster[] all = FindObjectsByType<Monster>(FindObjectsSortMode.None);
        foreach (Monster m in all) {
            if (!m.isAlly && Vector2.Distance(transform.position, m.transform.position) <= range)
                m.TakeDamage(damage);
        }
    }

    public float GetSkillCooldown(int slotIndex) {
        if (slotIndex >= 0 && slotIndex < skillCooldowns.Length) return skillCooldowns[slotIndex];
        return 0;
    }
}
