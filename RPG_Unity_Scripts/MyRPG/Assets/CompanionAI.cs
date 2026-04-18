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
    private float regenTimer; // YC: Hồi máu tự động
    private Vector3 followOffset; // Khoảng cách lệch để không đứng đè lên chủ

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        
        // Tìm chủ nhân
        PlayerStats[] allStats = FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
        foreach (var s in allStats) if (s.isPlayer) { master = s; break; }

        // Tạo một độ lệch ngẫu nhiên để khi 4 đệ tử đi theo sẽ không bị trùng khít
        followOffset = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);

        if (stats != null) stats.isPlayer = false;
    }

    void Update()
    {
        if (stats == null || master == null || stats.currentHealth <= 0) return;

        // --- TÌM MỤC TIÊU (Tầm quét rộng hơn: 12f) ---
        Monster qTarget = FindNearestMonster(12f);

        // --- DI CHUYỂN & CHIẾN ĐẤU ---
        float range = (type == CompanionType.Archer) ? 6f : 1.5f;
        
        if (qTarget != null)
        {
            float minDis = Vector2.Distance(transform.position, qTarget.transform.position);
            if (minDis > range) {
                float speed = (4.5f + stats.AGI * 0.12f) * Time.deltaTime; // Tăng nhẹ tốc độ rượt đuổi
                transform.position = Vector3.MoveTowards(transform.position, qTarget.transform.position, speed);
                FlipSprite(qTarget.transform.position.x);
                SetAnimBool("IsWalking", true);
            } else {
                SetAnimBool("IsWalking", false);
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
            if (distToMaster > 0.8f) {
                float speed = (4f + stats.AGI * 0.1f) * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, master.transform.position + followOffset, speed);
                FlipSprite(master.transform.position.x);
                SetAnimBool("IsWalking", true);
            } else {
                SetAnimBool("IsWalking", false);
            }

            // --- HỒI MÁU TỰ ĐỘNG (Khi không có quái) ---
            regenTimer += Time.deltaTime;
            if (regenTimer >= 3f) {
                int regenAmt = Mathf.Max(1, stats.maxHealth / 50); // Hồi 2% HP
                if (stats.currentHealth < stats.maxHealth) {
                    stats.currentHealth = Mathf.Min(stats.currentHealth + regenAmt, stats.maxHealth);
                    // if (GameUI.instance != null) GameUI.instance.ShowDamage(transform.position, "✚", Color.green);
                }
                regenTimer = 0;
            }
        }

        // --- LOGIC KỸ NĂNG HỖ TRỢ ---
        skillTimer += Time.deltaTime;
        if (skillTimer >= 5f) { CastSupportSkills(); skillTimer = 0; }
    }

    void SetAnimBool(string paramName, bool val)
    {
        if (anim == null) return;
        foreach (var p in anim.parameters) if (p.name == paramName) { anim.SetBool(paramName, val); break; }
    }

    void CastSupportSkills()
    {
        if (stats == null || master == null) return;
        if (stats.unlockedSkills.Contains("❤ Trị Thương (Lv6)"))
        {
            int healAmt = 15 + (stats.VIT / 2);
            master.currentHealth = Mathf.Min(master.currentHealth + healAmt, master.maxHealth);
            stats.currentHealth = Mathf.Min(stats.currentHealth + healAmt, stats.maxHealth);
            if (GameUI.instance != null) GameUI.instance.ShowDamage(master.transform.position, "❤ +" + healAmt, Color.green);
        }

        if (stats.unlockedSkills.Contains("🛡 Hộ Vệ (Lv3)"))
        {
            if (Vector2.Distance(transform.position, master.transform.position) < 4f)
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
        target.TakeDamage(dmg);
        if (type == CompanionType.Archer && GameUI.instance != null) 
            GameUI.instance.ShowDamage(target.transform.position, "🏹 -" + dmg, Color.green);
    }

    void FlipSprite(float tx) { if (sr != null) sr.flipX = tx < transform.position.x; }

    // --- VẼ HÀO QUANG (AURA) ---
    void OnGUI()
    {
        if (stats == null || stats.currentHealth <= 0) return;
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 screenPos = cam.WorldToScreenPoint(transform.position);
        if (screenPos.z > 0)
        {
            // Vẽ một vòng tròn nhỏ dưới chân
            float size = 40f;
            GUI.color = new Color(0f, 1f, 1f, 0.4f); // Màu xanh Cyan nhạt
            GUI.DrawTexture(new Rect(screenPos.x - size / 2, Screen.height - screenPos.y - 10, size, 15), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }
    }
}
