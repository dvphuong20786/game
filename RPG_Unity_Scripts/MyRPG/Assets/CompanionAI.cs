using UnityEngine;

// ===========================
// Gán script này vào Đệ Tử
// Đệ tử sẽ lấy chỉ số từ PlayerStats gắn trên mình
// ===========================
public class CompanionAI : MonoBehaviour
{
    private PlayerStats stats;
    private PlayerStats master;
    private Animator anim;
    private SpriteRenderer sr;

    private float attackTimer;
    private float skillTimer;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        
        // Tìm chủ nhân (là người chơi có isPlayer = true)
        PlayerStats[] allStats = FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
        foreach (var s in allStats) if (s.isPlayer) { master = s; break; }

        if (stats != null)
        {
            stats.isPlayer = false; // Đảm bảo đệ tử không bị nhầm là player chính
            if (sr != null) sr.color = Color.cyan;
        }
    }

    void Update()
    {
        if (stats == null || master == null || stats.currentHealth <= 0) return;

        // Tự tìm Quái gần nhất
        Monster[] allMonsters = FindObjectsByType<Monster>(FindObjectsSortMode.None);
        Monster qTarget = null;
        float minDis = 999f;

        foreach (Monster mm in allMonsters)
        {
            if (mm.currentHealth <= 0) continue;
            float d = Vector2.Distance(transform.position, mm.transform.position);
            if (d < minDis) { minDis = d; qTarget = mm; }
        }

        // --- TRÍ TUỆ CHIẾN ĐẤU ---
        if (qTarget != null && minDis < 7f)
        {
            float range = 1.5f; // Tầm đánh cơ bản
            // Nếu có kỹ năng AOE (Chém Gió) và quái ở gần
            if (stats.unlockedSkills.Contains("Chém Gió (Lv3)") && minDis < 3f && skillTimer >= 5f)
            {
                CastSkill_ChemGio();
                skillTimer = 0f;
            }
            else if (minDis > range)
            {
                // Tiếp cận quái
                float speed = 3.5f + (stats.AGI * 0.1f);
                transform.position = Vector3.MoveTowards(transform.position, qTarget.transform.position, speed * Time.deltaTime);
                FlipSprite(qTarget.transform.position.x);
            }
            else
            {
                // Tấn công thường
                attackTimer += Time.deltaTime;
                if (attackTimer >= 1.2f - (stats.AGI * 0.02f))
                {
                    if (anim != null) anim.SetTrigger("Attack");
                    int totalDmg = 15 + stats.bonusDamage;
                    qTarget.TakeDamage(totalDmg);
                    attackTimer = 0f;
                    Debug.Log($"🐕 Đệ tử vung kiếm: {totalDmg} dame!");
                }
            }
        }
        else
        {
            // --- ĐI THEO CHỦ ---
            float distToMaster = Vector2.Distance(transform.position, master.transform.position);
            if (distToMaster > 2.5f)
            {
                float speed = 4f + (stats.AGI * 0.1f);
                transform.position = Vector3.MoveTowards(transform.position, master.transform.position, speed * Time.deltaTime);
                FlipSprite(master.transform.position.x);
            }
        }

        skillTimer += Time.deltaTime;
    }

    void CastSkill_ChemGio()
    {
        if (anim != null) anim.SetTrigger("Attack");
        int skillDmg = (15 + stats.bonusDamage) * 2;
        
        Monster[] targets = FindObjectsByType<Monster>(FindObjectsSortMode.None);
        foreach (var m in targets)
        {
            if (Vector2.Distance(transform.position, m.transform.position) < 3.5f)
            {
                m.TakeDamage(skillDmg);
            }
        }
        if (GameUI.instance != null)
            GameUI.instance.ShowDamage(transform.position, "💨 CHÉM GIÓ!", Color.cyan);
    }

    void FlipSprite(float targetX)
    {
        if (sr == null) return;
        if (targetX > transform.position.x) sr.flipX = false;
        else sr.flipX = true;
    }
}
