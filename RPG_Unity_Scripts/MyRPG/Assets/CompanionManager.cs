using UnityEngine;
using System.Collections.Generic;

// ===========================
// QUẢN LÝ ĐỆ TỬ (Companion Manager)
// Chịu trách nhiệm Lưu/Tải và duy trì đội hình đệ tử xuyên suốt các màn chơi
// ===========================
public class CompanionManager : MonoBehaviour
{
    public static CompanionManager instance;

    // Prefabs tự động nạp từ Resources/Companions
    private GameObject warriorPrefab;
    private GameObject archerPrefab;
    private GameObject slimePrefab;

    private List<CompanionAI> activeCompanions = new List<CompanionAI>();

    void Awake()
    {
        if (instance == null) { 
            instance = this; 
            DontDestroyOnLoad(gameObject); 
        }
        else { 
            Destroy(gameObject); 
        }
    }

    public void RegisterCompanion(CompanionAI comp)
    {
        if (!activeCompanions.Contains(comp)) activeCompanions.Add(comp);
    }

    public void UnregisterCompanion(CompanionAI comp)
    {
        if (activeCompanions.Contains(comp)) activeCompanions.Remove(comp);
    }

    // Lấy danh sách đệ tử hiện tại (để PlayerStats gọi khi lưu game)
    public void SaveCompanions()
    {
        List<string> dataList = new List<string>();
        foreach (var comp in activeCompanions)
        {
            if (comp == null) continue;
            PlayerStats s = comp.GetComponent<PlayerStats>();
            if (s == null) continue;

            // Định dạng: TypeIndex, Level, Exp, CurrentHP, STR, VIT, AGI, StatPts, SkillPts, Skills(Name1:Lv1;Name2:Lv2), Buffs(B1,B2)
            List<string> skillDat = new List<string>();
            foreach(var sk in s.unlockedSkills) if(sk.data != null) skillDat.Add(sk.data.name + ":" + sk.level);
            string skillsStr = skillDat.Count > 0 ? string.Join(";", skillDat) : "None";
            string buffStr = s.activeBuffs.Count > 0 ? string.Join(",", s.activeBuffs) : "None";

            string data = $"{(int)comp.type}${s.level}${s.currentExp}${s.currentHealth}${s.STR}${s.VIT}${s.AGI}${s.statPoints}${s.skillPoints}${skillsStr}${buffStr}";
            
            // 3. Lưu trang bị của đệ tử (Thành phần thứ 11 trong chuỗi con)
            string eqHead = s.SerializeInstance(s.eqHead);
            string eqBody = s.SerializeInstance(s.eqBody);
            string eqLegs = s.SerializeInstance(s.eqLegs);
            string eqWMain = s.SerializeInstance(s.eqWeaponMain);
            string eqWOff = s.SerializeInstance(s.eqWeaponOff);
            string eqR1 = s.SerializeInstance(s.eqRing1);
            string eqR2 = s.SerializeInstance(s.eqRing2);
            string eqNeck = s.SerializeInstance(s.eqNecklace);
            string eqAnc = s.SerializeInstance(s.eqAncientGold);
            
            string eqStr = $"{eqHead}#{eqBody}#{eqLegs}#{eqWMain}#{eqWOff}#{eqR1}#{eqR2}#{eqNeck}#{eqAnc}";
            data += "$" + eqStr;

            dataList.Add(data);
        }
        PlayerPrefs.SetString("SavedComps", string.Join("|", dataList));
        Debug.Log("💾 [CompanionManager] Đã lưu " + dataList.Count + " đệ tử kèm trang bị đầy đủ.");
    }

    // Triệu hồi lại đội hình dựa trên dữ liệu đã lưu
    public void LoadAndSpawnCompanions(Transform playerTransform)
    {
        string raw = PlayerPrefs.GetString("SavedComps", "");
        if (string.IsNullOrEmpty(raw)) {
            Debug.Log("ℹ️ [CompanionManager] Không có đệ tử nào để triệu hồi.");
            return;
        }

        Debug.Log("📂 [CompanionManager] Bắt đầu triệu hồi đệ tử từ dữ liệu: " + raw);

        // Xóa sạch danh sách cũ và HỦY các đối tượng cũ để tránh duplicate khi load thủ công hoặc chuyển scene
        foreach(var c in activeCompanions) if(c != null) Destroy(c.gameObject);
        activeCompanions.Clear();

        string[] compDatArr = raw.Split('|');
        foreach (string dat in compDatArr)
        {
            if (string.IsNullOrEmpty(dat)) continue;
            
            // TỰ ĐỘNG NHẬN DIỆN ĐỊNH DẠNG (Dấu $ hoặc dấu ,)
            char separator = dat.Contains("$") ? '$' : ',';
            string[] parts = dat.Split(separator);
            
            if (parts.Length < 4) continue;

            try {
                int typeIdx = int.Parse(parts[0]);
                int level = int.Parse(parts[1]);
                int exp = int.Parse(parts[2]);
                int hp = int.Parse(parts[3]);
                
                // Dữ liệu mở rộng (nếu có)
                int str = (parts.Length > 4) ? int.Parse(parts[4]) : 5;
                int vit = (parts.Length > 5) ? int.Parse(parts[5]) : 5;
                int agi = (parts.Length > 6) ? int.Parse(parts[6]) : 5;
                int sPts = (parts.Length > 7) ? int.Parse(parts[7]) : 0;
                int skPts = (parts.Length > 8) ? int.Parse(parts[8]) : 0;
                string skStr = (parts.Length > 9) ? parts[9] : "None";
                string bfStr = (parts.Length > 10) ? parts[10] : "None";
                string eqStr = (parts.Length > 11) ? parts[11] : "None";

                SpawnSingleCompanion((CompanionAI.CompanionType)typeIdx, level, exp, hp, str, vit, agi, sPts, skPts, skStr, bfStr, eqStr, playerTransform);
            } catch (System.Exception e) {
                Debug.LogError("⚠️ [CompanionManager] Lỗi phân tích dữ liệu đệ tử: " + e.Message);
            }
        }
    }

    void SpawnSingleCompanion(CompanionAI.CompanionType type, int level, int exp, int hp, int str, int vit, int agi, int sPts, int skPts, string skStr, string bfStr, string eqStr, Transform player)
    {
        GameObject prefab = null;
        string typeName = type.ToString();

        // TỰ ĐỘNG NẠP TỪ RESOURCES (Bất tử xuyên map)
        if (type == CompanionAI.CompanionType.Warrior) prefab = Resources.Load<GameObject>("Companions/HiepSi");
        else if (type == CompanionAI.CompanionType.Archer) prefab = Resources.Load<GameObject>("Companions/Archer");
        else if (type == CompanionAI.CompanionType.Slime) prefab = Resources.Load<GameObject>("Companions/Smile_detu");

        if (prefab == null) {
            Debug.LogError($"❌ [CompanionManager] KHÔNG TÌM THẤY file trong Resources/Companions cho loại: {typeName}");
            return;
        }

        // Sinh ra xung quanh player
        Vector3 spawnPos = player.position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
        GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity);
        go.name = typeName + "_Companion"; // Đặt tên để dễ theo dõi trong Hierarchy
        
        CompanionAI ai = go.GetComponent<CompanionAI>();
        PlayerStats stats = go.GetComponent<PlayerStats>();

        if (stats != null)
        {
            stats.level = level;
            stats.currentExp = exp;
            stats.STR = str;
            stats.VIT = vit;
            stats.AGI = agi;
            stats.statPoints = sPts;
            stats.skillPoints = skPts;
            
            // Load lại kỹ năng đã học
            stats.unlockedSkills.Clear();
            if (skStr != "None") {
                string[] sArray = skStr.Split(';');
                foreach(var sPart in sArray) {
                    var sSub = sPart.Split(':');
                    if (sSub.Length == 2) {
                        SkillData sData = Resources.Load<SkillData>("Skills/" + sSub[0]);
                        if (sData != null) stats.unlockedSkills.Add(new PlayerStats.SkillProgress(sData, int.Parse(sSub[1])));
                    }
                }
            }
            
            // Load lại Buffs
            stats.activeBuffs.Clear();
            if (bfStr != "None") {
                stats.activeBuffs = new List<string>(bfStr.Split(new char[]{','}, System.StringSplitOptions.RemoveEmptyEntries));
            }

            // Load lại Trang bị (Equips) - Giải mã chuỗi #
            if (eqStr != "None") {
                string[] eqArray = eqStr.Split('#');
                if (eqArray.Length >= 9) {
                    stats.eqHead = stats.DeserializeInstance(eqArray[0]);
                    stats.eqBody = stats.DeserializeInstance(eqArray[1]);
                    stats.eqLegs = stats.DeserializeInstance(eqArray[2]);
                    stats.eqWeaponMain = stats.DeserializeInstance(eqArray[3]);
                    stats.eqWeaponOff = stats.DeserializeInstance(eqArray[4]);
                    stats.eqRing1 = stats.DeserializeInstance(eqArray[5]);
                    stats.eqRing2 = stats.DeserializeInstance(eqArray[6]);
                    stats.eqNecklace = stats.DeserializeInstance(eqArray[7]);
                    stats.eqAncientGold = stats.DeserializeInstance(eqArray[8]);
                }
            }

            stats.CalculateBonus();
            stats.currentHealth = hp;
            stats.isPlayer = false; 
        }

        if (ai != null) {
            ai.type = type; // Ép kiểu để chắc chắn AI chạy đúng loại
            RegisterCompanion(ai);
            
            // ĐẶC BIỆT: Nếu đệ tử có script Monster, phải đánh dấu là Đồng Minh
            Monster m = go.GetComponent<Monster>();
            if (m != null) m.isAlly = true;
        }

        Debug.Log($"✅ [CompanionManager] Đã triệu hồi thành công {typeName} (Level {level})");
    }
}
