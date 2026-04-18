using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ===========================
// Hệ thống Chỉ số & RPG Cơ bản
// Gán vào Người chơi (Player) và Đệ Tử (Companion)
// ===========================
public class PlayerStats : MonoBehaviour
{
    // Chỉ có Người chơi thật mới giữ Singleton này
    public static PlayerStats instance;

    [Header("Loại Nhân Vật")]
    public bool isPlayer = true; // Tích vào nếu là người chơi chính
    public string characterName = "Hiệp Sĩ";

    void Awake()
    {
        if (isPlayer)
        {
            if (instance == null) 
            { 
                instance = this; 
                DontDestroyOnLoad(gameObject);
                // Đăng ký tự động lưu khi chuyển map
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else { Destroy(gameObject); return; }
        }
        
        // KHỞI TẠO MÁU SỚM ĐỂ TRÁNH LỖI UI 0/HP
        CalculateBonus();
        currentHealth = maxHealth;
    }

    void OnDestroy()
    {
        if (isPlayer) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Tự động load data khi vào Scene mới
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isPlayer && PlayerPrefs.HasKey("Level"))
        {
            LoadGame();
            
            // TRIỆU HỒI ĐỆ TỬ KHI CHUYỂN MAP
            if (CompanionManager.instance != null)
            {
                CompanionManager.instance.LoadAndSpawnCompanions(transform);
            }
            
            Debug.Log($"[Save] ✅ Đã tải dữ liệu & Đội ngũ vào Scene: {scene.name}");
        }
    }

    [Header("Chỉ số cơ bản")]
    public int level = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Tiềm năng & Vàng")]
    public int statPoints = 0;
    public int skillPoints = 0;
    public int gold = 0; // Đệ tử có thể dùng chung gold với Player
    public int STR = 5; public int VIT = 5; public int AGI = 5;

    [Header("Kho đồ & Kỹ năng")]
    public List<string> inventory = new List<string>();
    public List<string> unlockedSkills = new List<string>();

    [Header("Trang bị Đang Mặc")]
    public string eqHead = "Trống";
    public string eqBody = "Trống";
    public string eqLegs = "Trống";
    public string eqWeaponMain = "Tay Không";
    public string eqWeaponOff = "Trống";
    public string eqRing1 = "Trống";
    public string eqRing2 = "Trống";
    public string eqNecklace = "Trống";
    public string eqAncientGold = "Trống";

    [Header("Sức mạnh Tăng Cường")]
    public int bonusDamage = 0;
    public int bonusDefense = 0;

    void Start()
    {
        if (isPlayer)
        {
            // Nếu đã có save thì tải, không thì giữ nguyên giá trị mặc định
            if (PlayerPrefs.HasKey("Level"))
            {
                LoadGame();
                Debug.Log("[Save] 📂 Đã tải dữ liệu game.");
            }
            else
            {
                CalculateBonus();
                currentHealth = maxHealth;
            }
        }
        else
        {
            // Đệ tử
            CalculateBonus();
            currentHealth = maxHealth;
            if (characterName == "Hiệp Sĩ" || characterName == "") characterName = "Đệ Tử";
        }
    }

    public void TakeDamage(int damage)
    {
        // VIT tăng phòng thủ tự nhiên
        int giapTuNhien = VIT / 2;
        int thucTe = damage - bonusDefense - giapTuNhien;
        if (thucTe < 1) thucTe = 1;
        currentHealth -= thucTe;
        
        if (GameUI.instance != null) 
            GameUI.instance.ShowDamage(transform.position, "-" + thucTe, isPlayer ? Color.red : new Color(1f, 0.5f, 0.5f));
            
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            if (isPlayer) Debug.Log("💀 GAME OVER!");
            else Debug.Log("🛡 Đệ tử đã gục ngã!");
        }
    }

    public void AddGold(int amount) 
    { 
        if (!isPlayer && instance != null) instance.gold += amount;
        else gold += amount; 
    }

    public void AddExp(int amount) 
    { 
        currentExp += amount; 
        if (currentExp >= expToNextLevel) LevelUp(); 
    }

    // Hàm tĩnh để chia sẻ EXP cho cả Player và Companion
    public static void ShareExp(int amount)
    {
        PlayerStats p = instance;
        PlayerStats c = null;
        PlayerStats[] all = Object.FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
        foreach(var s in all) if(!s.isPlayer) { c = s; break; }

        if (p != null && c != null)
        {
            p.AddExp(amount / 2);
            c.AddExp(amount / 2);
        }
        else if (p != null)
        {
            p.AddExp(amount);
        }
        else if (c != null)
        {
            c.AddExp(amount);
        }
    }

    void LevelUp()
    {
        currentExp -= expToNextLevel; level++;
        expToNextLevel = (int)(expToNextLevel * 1.2f) + 50;
        statPoints += 5; // Tặng 5 điểm tiềm năng mỗi cấp
        if (level % 3 == 0) skillPoints++; // Tặng điểm kỹ năng mỗi 3 cấp
        
        CalculateBonus();
        currentHealth = maxHealth;
        
        if (GameUI.instance != null)
            GameUI.instance.ShowDamage(transform.position, (isPlayer ? "✨ " : "🐕 ") + "LÊN CẤP " + level + "!", Color.yellow);
    }

    public void AddStat(string statName)
    {
        if (statPoints <= 0) return;
        if (statName == "STR") STR++;
        else if (statName == "VIT") VIT++;
        else if (statName == "AGI") AGI++;
        statPoints--;
        CalculateBonus();
    }

    public void PickUpItem(string itemName) { inventory.Add(itemName); }

    public void EquipItem(int index, string subSlot = "")
    {
        if (index < 0 || index >= inventory.Count) return;
        string newItem = inventory[index];
        string oldItem = "Trống";

        if (newItem.Contains("Mũ")) { oldItem = eqHead; eqHead = newItem; }
        else if (newItem.Contains("Áo")) { oldItem = eqBody; eqBody = newItem; }
        else if (newItem.Contains("Giày")) { oldItem = eqLegs; eqLegs = newItem; }
        else if (newItem.Contains("Dây")) { oldItem = eqNecklace; eqNecklace = newItem; }
        else if (newItem.Contains("Vàng Cổ")) { oldItem = eqAncientGold; eqAncientGold = newItem; }
        else if (newItem.Contains("Nhẫn"))
        {
            if (subSlot == "Ring1") { oldItem = eqRing1; eqRing1 = newItem; }
            else { oldItem = eqRing2; eqRing2 = newItem; }
        }
        else if (newItem.Contains("Khiên"))
        {
            if (IsTwoHanded(eqWeaponMain)) { Debug.Log("Vũ khí 2 tay không thể dùng Khiên!"); return; }
            oldItem = eqWeaponOff; eqWeaponOff = newItem;
        }
        else // Vũ khí
        {
            if (IsTwoHanded(newItem))
            {
                if (eqWeaponOff != "Trống") { inventory.Add(eqWeaponOff); eqWeaponOff = "Trống"; }
            }
            oldItem = eqWeaponMain; eqWeaponMain = newItem;
        }

        inventory.RemoveAt(index);
        if (oldItem != "Trống" && oldItem != "Tay Không") inventory.Add(oldItem);
        CalculateBonus();
    }

    public void Unequip(string slot)
    {
        string item = "Trống";
        if (slot == "Head") { item = eqHead; eqHead = "Trống"; }
        else if (slot == "Body") { item = eqBody; eqBody = "Trống"; }
        else if (slot == "Legs") { item = eqLegs; eqLegs = "Trống"; }
        else if (slot == "WepMain") { item = eqWeaponMain; eqWeaponMain = "Tay Không"; }
        else if (slot == "WepOff") { item = eqWeaponOff; eqWeaponOff = "Trống"; }
        else if (slot == "Ring1") { item = eqRing1; eqRing1 = "Trống"; }
        else if (slot == "Ring2") { item = eqRing2; eqRing2 = "Trống"; }
        else if (slot == "Neck") { item = eqNecklace; eqNecklace = "Trống"; }
        else if (slot == "Ancient") { item = eqAncientGold; eqAncientGold = "Trống"; }

        if (item != "Trống" && item != "Tay Không") { inventory.Add(item); CalculateBonus(); }
    }

    bool IsTwoHanded(string name) { return name.Contains("Đại") || name.Contains("2 Tay") || name.Contains("Huyết Kiếm"); }

    public void SellItem(int index)
    {
        if (index < 0 || index >= inventory.Count) return;
        int p = GetItemPrice(inventory[index]);
        if (isPlayer) gold += p;
        else if (instance != null) instance.gold += p;
        inventory.RemoveAt(index);
    }

    public int GetItemPrice(string n)
    {
        if (n.Contains("Huyết Kiếm")) return 200;
        if (n.Contains("Khiên")) return 50;
        if (n.Contains("Nhẫn")) return 100;
        if (n.Contains("Áo")) return 60;
        if (n.Contains("Mũ")) return 40;
        if (n.Contains("Giày")) return 35;
        if (n.Contains("Dây")) return 80;
        if (n.Contains("Vàng Cổ")) return 300;
        if (n.Contains("Ngọc Đỏ")) return 150;
        if (n.Contains("Ngọc Xanh")) return 150;
        if (n.Contains("Ngọc Tím")) return 150;
        return 15;
    }

    public void CalculateBonus()
    {
        // STR tăng sát thương: 1 STR = 2 Dame
        bonusDamage = STR * 2;
        // VIT tăng máu: 1 VIT = 15 HP
        maxHealth = 100 + (VIT * 15);
        bonusDefense = 0;

        ApplyStats(eqHead); ApplyStats(eqBody); ApplyStats(eqLegs);
        ApplyStats(eqWeaponMain); ApplyStats(eqWeaponOff);
        ApplyStats(eqRing1); ApplyStats(eqRing2);
        ApplyStats(eqNecklace); ApplyStats(eqAncientGold);

        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    void ApplyStats(string n)
    {
        if (n == "Trống" || n == "Tay Không") return;
        if (n.Contains("Kiếm Gỗ")) bonusDamage += 8;
        if (n.Contains("Kiếm Sắt")) bonusDamage += 15;
        if (n.Contains("Huyết Kiếm")) bonusDamage += 40;
        
        if (n.Contains("Áo Da")) bonusDefense += 12;
        if (n.Contains("Áo Giáp")) bonusDefense += 18;
        
        if (n.Contains("Mũ Sắt")) bonusDefense += 8;
        if (n.Contains("Giày")) bonusDefense += 5;
        
        if (n.Contains("Khiên Gỗ")) bonusDefense += 10;
        if (n.Contains("Khiên Sắt") || n.Contains("Khiên")) bonusDefense += 20;
        
        if (n.Contains("Nhẫn Kim Cương")) { bonusDamage += 5; bonusDefense += 5; }
        if (n.Contains("Dây Chuyền Bạc")) maxHealth += 30;
        if (n.Contains("Vàng Cổ")) { bonusDamage += 10; bonusDefense += 10; maxHealth += 50; }
        
        if (n.Contains("Ngọc Đỏ")) bonusDamage += 20;
        if (n.Contains("Ngọc Xanh")) bonusDefense += 15;
        if (n.Contains("Ngọc Tím")) maxHealth += 80;
    }

    public void UseConsumable(int index)
    {
        if (index < 0 || index >= inventory.Count) return;
        string item = inventory[index];
        if (item.Contains("Bình Máu Nhỏ")) { currentHealth = Mathf.Min(currentHealth + 30, maxHealth); inventory.RemoveAt(index); }
        else if (item.Contains("Bình Máu Lớn")) { currentHealth = Mathf.Min(currentHealth + 100, maxHealth); inventory.RemoveAt(index); }
        if (GameUI.instance != null) GameUI.instance.ShowDamage(transform.position, "💊 HỒI MÁU", Color.green);
    }

    public void LearnSkill(string skillName)
    {
        if (skillPoints > 0 && !unlockedSkills.Contains(skillName))
        {
            unlockedSkills.Add(skillName);
            skillPoints--;
        }
    }

    public void SaveGame()
    {
        if (!isPlayer) return;
        PlayerPrefs.SetInt("Level", level); 
        PlayerPrefs.SetInt("Gold", gold);
        PlayerPrefs.SetInt("HP", currentHealth);
        PlayerPrefs.SetInt("MaxHP", maxHealth);
        PlayerPrefs.SetInt("CurExp", currentExp);
        PlayerPrefs.SetInt("ExpNext", expToNextLevel);
        PlayerPrefs.SetInt("StatPts", statPoints);
        PlayerPrefs.SetInt("SkillPts", skillPoints);
        PlayerPrefs.SetInt("STR", STR); PlayerPrefs.SetInt("VIT", VIT); PlayerPrefs.SetInt("AGI", AGI);
        PlayerPrefs.SetString("Skills", string.Join(",", unlockedSkills));
        PlayerPrefs.SetString("Inv", string.Join(",", inventory));
        PlayerPrefs.SetString("WepM", eqWeaponMain); PlayerPrefs.SetString("WepO", eqWeaponOff);
        PlayerPrefs.SetString("R1", eqRing1); PlayerPrefs.SetString("R2", eqRing2);
        PlayerPrefs.SetString("Head", eqHead); PlayerPrefs.SetString("Body", eqBody);
        PlayerPrefs.SetString("Legs", eqLegs); PlayerPrefs.SetString("Neck", eqNecklace);
        PlayerPrefs.SetString("Anc", eqAncientGold);
        PlayerPrefs.SetString("CharName", characterName);
        
        // LƯU LUÔN ĐỘI NGŨ ĐỆ TỬ
        if (CompanionManager.instance != null)
        {
            CompanionManager.instance.SaveCompanions();
        }

        PlayerPrefs.Save();
        Debug.Log("[Save] 💾 Đã lưu game!");
    }

    public void LoadGame()
    {
        if (!isPlayer || !PlayerPrefs.HasKey("Level")) return;
        level       = PlayerPrefs.GetInt("Level");
        gold        = PlayerPrefs.GetInt("Gold");
        currentExp  = PlayerPrefs.GetInt("CurExp", 0);
        expToNextLevel = PlayerPrefs.GetInt("ExpNext", 100);
        statPoints  = PlayerPrefs.GetInt("StatPts", 0);
        skillPoints = PlayerPrefs.GetInt("SkillPts", 0);
        STR = PlayerPrefs.GetInt("STR", 5);
        VIT = PlayerPrefs.GetInt("VIT", 5);
        AGI = PlayerPrefs.GetInt("AGI", 5);
        eqWeaponMain = PlayerPrefs.GetString("WepM", "Tay Không");
        eqWeaponOff  = PlayerPrefs.GetString("WepO", "Trống");
        eqRing1 = PlayerPrefs.GetString("R1", "Trống");
        eqRing2 = PlayerPrefs.GetString("R2", "Trống");
        eqHead  = PlayerPrefs.GetString("Head", "Trống");
        eqBody  = PlayerPrefs.GetString("Body", "Trống");
        eqLegs  = PlayerPrefs.GetString("Legs", "Trống");
        eqNecklace   = PlayerPrefs.GetString("Neck", "Trống");
        eqAncientGold= PlayerPrefs.GetString("Anc", "Trống");
        if (PlayerPrefs.HasKey("CharName")) characterName = PlayerPrefs.GetString("CharName");
        
        string skillsRaw = PlayerPrefs.GetString("Skills", "");
        unlockedSkills = skillsRaw == "" ? new List<string>() : new List<string>(skillsRaw.Split(','));
        
        string invRaw = PlayerPrefs.GetString("Inv", "");
        inventory = invRaw == "" ? new List<string>() : new List<string>(invRaw.Split(','));
        
        CalculateBonus();
        currentHealth = PlayerPrefs.GetInt("HP", maxHealth);
    }

    // Đặt lại toàn bộ dữ liệu về mặc định (Dành cho nút New Game)
    public void ResetGame()
    {
        if (!isPlayer) return;
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetString("SavedComps", ""); // Xóa danh sách đệ tử
        level = 1; gold = 0; currentExp = 0; expToNextLevel = 100;
        statPoints = 0; skillPoints = 0; STR = 5; VIT = 5; AGI = 5;
        inventory.Clear(); unlockedSkills.Clear();
        eqHead = "Trống"; eqBody = "Trống"; eqLegs = "Trống";
        eqWeaponMain = "Tay Không"; eqWeaponOff = "Trống";
        eqRing1 = "Trống"; eqRing2 = "Trống";
        eqNecklace = "Trống"; eqAncientGold = "Trống";
        CalculateBonus();
        currentHealth = maxHealth;
        Debug.Log("[Save] 🗑️ Đã xóa dữ liệu - Bắt đầu game mới!");
    }
}

