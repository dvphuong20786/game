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

    // --- CÀI ĐẶT KHOẢNG CÁCH THÔNG MINH ---
    public float leashDistance = 8f;   // Nếu xa chủ quá 8m sẽ tự chạy về
    public float protectionRange = 10f; // Chỉ đánh quái nếu quái ở gần chủ trong tầm 10m

    void Update()
    {
        if (stats == null || master == null || stats.currentHealth <= 0) return;

        float distToMaster = Vector2.Distance(transform.position, master.transform.position);

        // --- 1. KIỂM TRA "DÂY XÍCH" (ƯU TIÊN HÀNG ĐẦU) ---
        // Nếu quá xa chủ, bỏ qua mọi thứ để chạy về
        if (distToMaster > leashDistance)
        {
            MoveToMaster();
            return;
        }

        // --- 2. TÌM MỤC TIÊU TRONG VÙNG BẢO VỆ ---
        Monster qTarget = FindNearestMonster(12f);
        
        // Kiểm tra xem quái có ở quá xa chủ nhân không
        if (qTarget != null)
        {
            float monsterDistToMaster = Vector2.Distance(qTarget.transform.position, master.transform.position);
            if (monsterDistToMaster > protectionRange) qTarget = null; // Quái ở xa chủ quá, mặc kệ nó
        }

        // --- 3. DI CHUYỂN & CHIẾN ĐẤU ---
        float range = (type == CompanionType.Archer) ? 6f : 1.5f;
        
        if (qTarget != null)
        {
            float distToMonster = Vector2.Distance(transform.position, qTarget.transform.position);
            if (distToMonster > range) {
                float speed = (4.5f + stats.AGI * 0.12f) * Time.deltaTime;
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
        else // Không có quái hoặc quái ở xa vùng bảo vệ -> Quay về đi theo chủ
        {
            MoveToMaster();

            // --- HỒI MÁU TỰ ĐỘNG (Khi không có quái) ---
            regenTimer += Time.deltaTime;
            if (regenTimer >= 3f) {
                int regenAmt = Mathf.Max(1, stats.maxHealth / 50); // Hồi 2% HP
                if (stats.currentHealth < stats.maxHealth) {
                    stats.currentHealth = Mathf.Min(stats.currentHealth + regenAmt, stats.maxHealth);
                }
                regenTimer = 0;
            }
        }

        // --- LOGIC KỸ NĂNG HỖ TRỢ ---
        skillTimer += Time.deltaTime;
        if (skillTimer >= 5f) { CastSupportSkills(); skillTimer = 0; }
    }

    void MoveToMaster()
    {
        float distToFollowPos = Vector2.Distance(transform.position, master.transform.position + followOffset);
        if (distToFollowPos > 0.8f) {
            float speed = (4f + stats.AGI * 0.1f) * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, master.transform.position + followOffset, speed);
            FlipSprite(master.transform.position.x);
            SetAnimBool("IsWalking", true);
        } else {
            SetAnimBool("IsWalking", false);
        }
    }

    void SetAnimBool(string paramName, bool val)
    {
        if (anim == null) return;
        // Kiểm tra an toàn trước khi set parameter
        foreach (var p in anim.parameters) {
            if (p.name == paramName) {
                anim.SetBool(paramName, val);
                break;
            }
        }
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
        foreach (Monster m in all) {
            if (m.isAlly) continue; // Không đánh đồng đội
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
        
        // Hiện hiệu ứng chữ bay cho đồng nhất
        Color c = (type == CompanionType.Archer) ? Color.green : Color.yellow;
        string icon = (type == CompanionType.Archer) ? "🏹" : (type == CompanionType.Slime ? "🟢" : "⚔️");
        
        if (GameUI.instance != null) 
            GameUI.instance.ShowDamage(target.transform.position, icon + " -" + dmg, c);
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
            float barW = 50f;
            float barH = 5f;
            float yOffset = 50f; // Độ cao trên đầu

            // 1. Vẽ Hào quang dưới chân
            GUI.color = new Color(0f, 1f, 1f, 0.3f);
            GUI.DrawTexture(new Rect(screenPos.x - 20, Screen.height - screenPos.y - 5, 40, 10), Texture2D.whiteTexture);

            // 2. Vẽ Thanh máu trên đầu
            float hpRatio = (float)stats.currentHealth / stats.maxHealth;
            Rect bgR = new Rect(screenPos.x - barW/2, Screen.height - screenPos.y - yOffset, barW, barH);
            
            GUI.color = Color.black;
            GUI.DrawTexture(bgR, Texture2D.whiteTexture);
            
            GUI.color = Color.green;
            GUI.DrawTexture(new Rect(bgR.x, bgR.y, barW * hpRatio, barH), Texture2D.whiteTexture);
            
            GUI.color = Color.white;
        }
    }
}
