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
    private float regenTimer; 
    private Vector3 followOffset; 
    
    [Header("Kỹ năng Assets")]
    public SkillData skillHoVe;
    public SkillData skillTriThuong;

    private Dictionary<string, float> cdTimers = new Dictionary<string, float>();

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        
        // --- ÉP BUỘC ĐỆ TỬ KHÔNG PHẢI LÀ PLAYER CHÍNH ---
        if (stats != null) stats.isPlayer = false;

        // Tìm chủ nhân (chỉ tìm người có isPlayer = true thực sự)
        PlayerStats[] allStats = FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
        foreach (var s in allStats) {
            if (s.isPlayer) { master = s; break; }
        }

        // Khởi tạo Cooldown
        if (!cdTimers.ContainsKey("HoVe")) cdTimers["HoVe"] = 0f;
        if (!cdTimers.ContainsKey("TriThuong")) cdTimers["TriThuong"] = 0f;

        // Tự động nạp kỹ năng nếu Inspector bị trống
        if (skillHoVe == null) skillHoVe = Resources.Load<SkillData>("Skills/HoVe");
        if (skillTriThuong == null) skillTriThuong = Resources.Load<SkillData>("Skills/TriThuong");

        // Tạo một độ lệch ngẫu nhiên
        followOffset = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);

        // TỰ ĐĂNG KÝ VỚI MANAGER
        if (CompanionManager.instance != null) CompanionManager.instance.RegisterCompanion(this);
    }

    // --- CÀI ĐẶT KHOẢNG CÁCH THÔNG MINH ---
    public float leashDistance = 8f;   // Nếu xa chủ quá 8m sẽ tự chạy về
    public float protectionRange = 10f; // Chỉ đánh quái nếu quái ở gần chủ trong tầm 10m

    void Update()
    {
        // --- TỰ ĐỘNG TÌM LẠI CHỦ NHÂN NẾU BỊ LẠC ---
        if (master == null) {
            PlayerStats[] all = Object.FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
            foreach (var s in all) if (s.isPlayer) { master = s; break; }
        }

        if (stats == null || master == null || stats.currentHealth <= 0) return;

        // Cập nhật bộ đếm hồi chiêu (Đảm bảo an toàn Key)
        if (!cdTimers.ContainsKey("HoVe")) cdTimers["HoVe"] = 0;
        if (!cdTimers.ContainsKey("TriThuong")) cdTimers["TriThuong"] = 0;

        if (cdTimers["HoVe"] > 0) cdTimers["HoVe"] -= Time.deltaTime;
        if (cdTimers["TriThuong"] > 0) cdTimers["TriThuong"] -= Time.deltaTime;

        // --- TỰ ĐỘNG TUNG CHIÊU KHI HẾT HỒI (Ưu tiên kiểm tra liên tục) ---
        if (skillHoVe != null && cdTimers["HoVe"] <= 0) { 
            if (TryCastHoVe()) cdTimers["HoVe"] = skillHoVe.baseCooldown; 
        }
        if (skillTriThuong != null && cdTimers["TriThuong"] <= 0) { 
            if (TryCastTriThuong()) cdTimers["TriThuong"] = skillTriThuong.baseCooldown; 
        }

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
    }

    public float GetCooldown(string skillKey)
    {
        if (cdTimers.ContainsKey(skillKey)) return cdTimers[skillKey];
        return 0;
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

    bool TryCastTriThuong()
    {
        if (stats == null || master == null || skillTriThuong == null) return false;
        int sLv = stats.GetSkillLevel(skillTriThuong.skillName);
        if (sLv <= 0) return false;

        bool masterHurt = master.currentHealth < (master.maxHealth * 0.8f);
        bool selfHurt = stats.currentHealth < (stats.maxHealth * 0.8f);

        if (masterHurt || selfHurt)
        {
            // Hồi máu: baseHealOrDef + (VIT/2) + Level*valueIncrease
            int healAmt = skillTriThuong.baseHealOrDef + (stats.VIT / 2) + (sLv * skillTriThuong.valueIncreasePerLevel);
            master.currentHealth = Mathf.Min(master.currentHealth + healAmt, master.maxHealth);
            stats.currentHealth = Mathf.Min(stats.currentHealth + healAmt, stats.maxHealth);
            if (GameUI.instance != null) GameUI.instance.ShowDamage(master.transform.position, "❤ REC +" + healAmt, Color.green);
            return true;
        }
        return false;
    }

    bool TryCastHoVe()
    {
        if (stats == null || master == null || skillHoVe == null) return false;
        int sLv = stats.GetSkillLevel(skillHoVe.skillName);
        if (sLv <= 0) return false;

        if (Vector2.Distance(transform.position, master.transform.position) < 4f)
        {
            // Thủ tăng: baseHealOrDef + Level*valueIncrease
            int bonusDef = skillHoVe.baseHealOrDef + (sLv * skillHoVe.valueIncreasePerLevel);
            master.bonusDefense += bonusDef;
            if (!master.activeBuffs.Contains("🛡 Hộ Vệ")) master.activeBuffs.Add("🛡 Hộ Vệ");
            
            StartCoroutine(RemoveBuff(master, bonusDef, 4.5f, "🛡 Hộ Vệ"));
            if (GameUI.instance != null) GameUI.instance.ShowDamage(master.transform.position, "🛡 HỘ VỆ +" + bonusDef, Color.cyan);
            return true;
        }
        return false;
    }

    IEnumerator RemoveBuff(PlayerStats targetStats, int amt, float delay, string buffName)
    {
        yield return new WaitForSeconds(delay);
        if (targetStats != null) 
        {
            targetStats.bonusDefense -= amt;
            if (targetStats.activeBuffs.Contains(buffName)) targetStats.activeBuffs.Remove(buffName);
        }
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

        // --- XỬ LÝ THEO LOẠI ĐỆ TỬ (Archer: Bắn cung, Slime: Phun Độc) ---
        if (type == CompanionType.Archer)
        {
            // TẠO MŨI TÊN (Projectile)
            GameObject arrowObj = new GameObject("Arrow_Projectile");
            arrowObj.transform.position = transform.position;
            Projectile proj = arrowObj.AddComponent<Projectile>();
            
            // Vẽ mũi tên đơn giản
            SpriteRenderer arrowSR = arrowObj.AddComponent<SpriteRenderer>();
            arrowSR.sprite = Resources.Load<Sprite>("Icons/Cung_va_Ten");
            arrowSR.sortingOrder = 10;
            arrowObj.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

            proj.Launch(target.transform, dmg, Color.white);
        }
        else if (type == CompanionType.Slime)
        {
            // Sát thương trực tiếp nhẹ và GÂY ĐỘC (YÊU CẦU MỤC 11)
            target.TakeDamage(dmg / 2);
            target.ApplyPoison(5 + (stats.level / 2), 5.0f); 

            if (GameUI.instance != null) 
                GameUI.instance.ShowDamage(target.transform.position, "🧪 TRÚNG ĐỘC!", Color.green);
        }
        else // Warrior (Hiệp sĩ) - Cận chiến
        {
            target.TakeDamage(dmg);
            if (GameUI.instance != null) 
                GameUI.instance.ShowDamage(target.transform.position, "⚔️ -" + dmg, Color.yellow);
        }
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
