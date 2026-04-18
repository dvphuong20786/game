using UnityEngine;
using System.Collections.Generic;

// ===========================
// QUẢN LÝ ĐỆ TỬ (Companion Manager)
// Chịu trách nhiệm Lưu/Tải và duy trì đội hình đệ tử xuyên suốt các màn chơi
// ===========================
public class CompanionManager : MonoBehaviour
{
    public static CompanionManager instance;

    [Header("Companion Prefabs (Kéo thảo vào đây)")]
    public GameObject warriorPrefab;
    public GameObject archerPrefab;
    public GameObject slimePrefab;

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

            // Định dạng: TypeIndex, Level, Exp, CurrentHP
            string data = $"{(int)comp.type},{s.level},{s.currentExp},{s.currentHealth}";
            dataList.Add(data);
        }
        PlayerPrefs.SetString("SavedComps", string.Join("|", dataList));
        Debug.Log("💾 [CompanionManager] Đã lưu " + dataList.Count + " đệ tử.");
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

        // Xóa sạch danh sách cũ để tránh duplicate khi chuyển scene
        activeCompanions.Clear();

        string[] compDatArr = raw.Split('|');
        foreach (string dat in compDatArr)
        {
            if (string.IsNullOrEmpty(dat)) continue;
            string[] parts = dat.Split(',');
            if (parts.Length < 4) continue;

            try {
                int typeIdx = int.Parse(parts[0]);
                int level = int.Parse(parts[1]);
                int exp = int.Parse(parts[2]);
                int hp = int.Parse(parts[3]);

                SpawnSingleCompanion((CompanionAI.CompanionType)typeIdx, level, exp, hp, playerTransform);
            } catch (System.Exception e) {
                Debug.LogError("⚠️ [CompanionManager] Lỗi phân tích dữ liệu đệ tử: " + e.Message);
            }
        }
    }

    void SpawnSingleCompanion(CompanionAI.CompanionType type, int level, int exp, int hp, Transform player)
    {
        GameObject prefab = null;
        string typeName = type.ToString();

        if (type == CompanionAI.CompanionType.Warrior) prefab = warriorPrefab;
        else if (type == CompanionAI.CompanionType.Archer) prefab = archerPrefab;
        else if (type == CompanionAI.CompanionType.Slime) prefab = slimePrefab;

        if (prefab == null) {
            Debug.LogError($"❌ [CompanionManager] KHÔN_TÌM_THẤY_PREFAB cho loại: {typeName}. Hãy kiểm tra lại ô kéo thả trong CompanionManager!");
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
            stats.CalculateBonus();
            stats.currentHealth = hp;
            stats.isPlayer = false; // Đảm bảo không bị nhầm là người chơi chính
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
