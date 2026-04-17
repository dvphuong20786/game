using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// ===========================
// Gán script này vào Đệ Tử
// Hỗ trợ cả Cận chiến và Tầm xa, tối ưu di chuyển cho đội hình 4 người
// ===========================
public class CompanionAI : MonoBehaviour
{
    public enum CompanionType { Warrior, Archer, Slime }
    public CompanionType type = CompanionType.Warrior;

    private PlayerStats stats;
    private PlayerStats master;
    private Animator anim;
    private SpriteRenderer sr;

    private float attackTimer;
    private float skillTimer;
    private Vector3 followOffset; // Khoảng cách lệch để không đứng đè lên chủ

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        
        // Tìm chủ nhân
        PlayerStats[] allStats = FindObjectsOfType<PlayerStats>();
        foreach (var s in allStats) if (s.isPlayer) { master = s; break; }

        // Tạo một độ lệch ngẫu nhiên để khi 4 đệ tử đi theo sẽ không bị trùng khít
        followOffset = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);

        if (stats != null) stats.isPlayer = false;
    }

    void Update()
    {
        if (stats == null || master == null || stats.currentHealth <= 0) return;

        // --- TÌM MỤC TIÊU ---
        Monster qTarget = FindNearestMonster(8f);

        // --- DI CHUYỂN & CHIẾN ĐẤU ---
        float range = (type == CompanionType.Archer) ? 6f : 1.5f;
        
        if (qTarget != null)
        {
            float minDis = Vector2.Distance(transform.position, qTarget.transform.position);
            if (minDis > range) {
                float speed = (3.5f + stats.AGI * 0.1f) * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, qTarget.transform.position, speed);
                FlipSprite(qTarget.transform.position.x);
            } else {
                attackTimer += Time.deltaTime;
                float cd = (type == CompanionType.Archer) ? 1.5f : 1.2f;
                if (attackTimer >= cd - (stats.AGI * 0.02f)) {
                    Attack(qTarget);
                    attackTimer = 0;
                }
            }
        }
        else // Không có quái -> Quay về đi theo chủ
        {
            float distToMaster = Vector2.Distance(transform.position, master.transform.position + followOffset);
            if (distToMaster > 0.5f) {
                float speed = (4f + stats.AGI * 0.1f) * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, master.transform.position + followOffset, speed);
                FlipSprite(master.transform.position.x);
            }
        }

        // --- LOGIC KỸ NĂNG HỖ TRỢ (MỚI) ---
        skillTimer += Time.deltaTime;
        if (skillTimer >= 5f) // Thử dùng kỹ năng mỗi 5 giây
        {
            CastSupportSkills();
            skillTimer = 0;
        }
    }

    void CastSupportSkills()
    {
        if (stats == null || master == null) return;

        // 1. ❤ Trị Thương (Lv6) - Hồi máu cho chủ và bản thân
        if (stats.unlockedSkills.Contains("❤ Trị Thương (Lv6)"))
        {
            int healAmt = 15 + (stats.VIT / 2);
            master.currentHealth = Mathf.Min(master.currentHealth + healAmt, master.maxHealth);
            stats.currentHealth = Mathf.Min(stats.currentHealth + healAmt, stats.maxHealth);
            if (GameUI.instance != null) GameUI.instance.ShowDamage(master.transform.position, "❤ +" + healAmt, Color.green);
            Debug.Log("🐕 Đệ tử dùng Trị Thương!");
        }

        // 2. 🛡 Hộ Vệ (Lv3) - Tăng thủ tạm thời (Logic tạm thời: cộng thẳng bonusDefense trong 5s)
        if (stats.unlockedSkills.Contains("🛡 Hộ Vệ (Lv3)"))
        {
            float distToMaster = Vector2.Distance(transform.position, master.transform.position);
            if (distToMaster < 4f)
            {
                master.bonusDefense += 10;
                StartCoroutine(RemoveBuff(master, 10, 4.5f));
                if (GameUI.instance != null) GameUI.instance.ShowDamage(master.transform.position, "🛡 HỘ VỆ", Color.cyan);
            }
        }
    }

    IEnumerator RemoveBuff(PlayerStats targetStats, int amt, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (targetStats != null) targetStats.bonusDefense -= amt;
    }

    private Monster FindNearestMonster(float r)
    {
        Monster[] all = Object.FindObjectsByType<Monster>(FindObjectsSortMode.None);
        Monster nearest = null; float minD = r;
        foreach (var m in all) {
            float d = Vector2.Distance(transform.position, m.transform.position);
            if (d < minD) { minD = d; nearest = m; }
        }
        return nearest;
    }


    void Attack(Monster target)
    {
        if (anim != null) anim.SetTrigger("Attack");
        int dmg = 15 + stats.bonusDamage;
        
        if (type == CompanionType.Archer) {
            // Logic bắn tên (ở đây tạm thời gây sát thương thẳng, bạn có thể tạo Arrow Prefab sau)
            target.TakeDamage(dmg);
            if (GameUI.instance != null) GameUI.instance.ShowDamage(target.transform.position, "🏹 -" + dmg, Color.green);
        } else {
            target.TakeDamage(dmg);
        }
    }

    void CastSkill_Warrior() {
        if (anim != null) anim.SetTrigger("Attack");
        int dmg = (15 + stats.bonusDamage) * 2;
        Monster[] targets = FindObjectsOfType<Monster>();
        foreach (var m in targets) {
            if (Vector2.Distance(transform.position, m.transform.position) < 3.5f) m.TakeDamage(dmg);
        }
        if (GameUI.instance != null) GameUI.instance.ShowDamage(transform.position, "💥 CHÉM GIÓ!", Color.cyan);
    }

    void FlipSprite(float tx) {
        if (sr != null) sr.flipX = tx < transform.position.x;
    }
}
