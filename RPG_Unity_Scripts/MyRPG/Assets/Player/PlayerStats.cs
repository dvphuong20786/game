using System.Collections.Generic;
using UnityEngine;

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
            if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
            else { Destroy(gameObject); }
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
        currentHealth = maxHealth;
        CalculateBonus();
        
        // Đệ tử tự nhận diện tên nếu chưa đặt
        if (!isPlayer && characterName == "Hiệp Sĩ") characterName = "Đệ Tử Tử Long";
    }

    public void TakeDamage(int damage)
    {
        int thucTe = damage - bonusDefense - (VIT * 1);
        if (thucTe < 1) thucTe = 1;
        currentHealth -= thucTe;
        
        // Hiện damage ngay tại vị trí nhân vật này
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
        if (!isPlayer && instance != null) instance.gold += amount; // Cộng dồn về túi chủ
        else gold += amount; 
    }

    public void AddExp(int amount) 
    { 
        currentExp += amount; 
        if (currentExp >= expToNextLevel) LevelUp(); 
    }

    void LevelUp()
    {
        currentExp -= expToNextLevel; level++;
        expToNextLevel += 50;
        statPoints += 5;
        if (level % 3 == 0) skillPoints++;
        maxHealth += 15;
        currentHealth = maxHealth;
        
        if (GameUI.instance != null)
            GameUI.instance.ShowDamage(transform.position, (isPlayer ? "✨ " : "🐕 ") + "LÊN CẤP " + level + "!", Color.yellow);
    }

    public void PickUpItem(string itemName) { inventory.Add(itemName); }

    // ===== MẶC ĐỒ =====
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
        bonusDamage = (STR * 1);
        bonusDefense = 0;
        maxHealth = 100 + (VIT * 8);

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
        if (n.Contains("Huyết Kiếm")) bonusDamage += 40;
        if (n.Contains("Áo Da")) bonusDefense += 12;
        if (n.Contains("Mũ Sắt")) bonusDefense += 5;
        if (n.Contains("Giày")) bonusDefense += 3;
        if (n.Contains("Khiên")) bonusDefense += 20;
        if (n.Contains("Nhẫn Kim Cương")) { bonusDamage += 5; bonusDefense += 5; }
        if (n.Contains("Dây Chuyền Bạc")) maxHealth += 30;
        if (n.Contains("Vàng Cổ")) { bonusDamage += 10; bonusDefense += 10; maxHealth += 50; }
        if (n.Contains("Kiếm Sắt")) bonusDamage += 15;
        if (n.Contains("Áo Giáp")) bonusDefense += 18;
        if (n.Contains("Khiên Gỗ")) bonusDefense += 10;
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

    // ===== LƯU / LOAD =====
    public void SaveGame()
    {
        if (!isPlayer) return; // Chỉ lưu thông số người chơi chính vào Prefs chính
        PlayerPrefs.SetInt("Level", level); PlayerPrefs.SetInt("Gold", gold);
        PlayerPrefs.SetInt("SkillPts", skillPoints); PlayerPrefs.SetInt("HP", currentHealth);
        PlayerPrefs.SetString("Skills", string.Join(",", unlockedSkills));
        PlayerPrefs.SetString("Inv", string.Join(",", inventory));
        PlayerPrefs.SetString("WepM", eqWeaponMain); PlayerPrefs.SetString("WepO", eqWeaponOff);
        PlayerPrefs.SetString("R1", eqRing1); PlayerPrefs.SetString("R2", eqRing2);
        PlayerPrefs.SetString("Head", eqHead); PlayerPrefs.SetString("Body", eqBody);
        PlayerPrefs.SetString("Legs", eqLegs); PlayerPrefs.SetString("Neck", eqNecklace);
        PlayerPrefs.SetString("Anc", eqAncientGold);
        PlayerPrefs.SetInt("STR", STR); PlayerPrefs.SetInt("VIT", VIT); PlayerPrefs.SetInt("AGI", AGI);
        PlayerPrefs.Save();
        Debug.Log("💾 Đã lưu game!");
    }

    public void LoadGame()
    {
        if (!isPlayer || !PlayerPrefs.HasKey("Level")) return;
        level = PlayerPrefs.GetInt("Level"); gold = PlayerPrefs.GetInt("Gold");
        skillPoints = PlayerPrefs.GetInt("SkillPts");
        STR = PlayerPrefs.GetInt("STR"); VIT = PlayerPrefs.GetInt("VIT"); AGI = PlayerPrefs.GetInt("AGI");
        eqWeaponMain = PlayerPrefs.GetString("WepM"); eqWeaponOff = PlayerPrefs.GetString("WepO");
        eqRing1 = PlayerPrefs.GetString("R1"); eqRing2 = PlayerPrefs.GetString("R2");
        eqHead = PlayerPrefs.GetString("Head"); eqBody = PlayerPrefs.GetString("Body");
        eqLegs = PlayerPrefs.GetString("Legs"); eqNecklace = PlayerPrefs.GetString("Neck");
        eqAncientGold = PlayerPrefs.GetString("Anc");
        unlockedSkills = new List<string>(PlayerPrefs.GetString("Skills").Split(','));
        if (unlockedSkills.Count == 1 && unlockedSkills[0] == "") unlockedSkills.Clear();
        inventory = new List<string>(PlayerPrefs.GetString("Inv").Split(','));
        if (inventory.Count == 1 && inventory[0] == "") inventory.Clear();
        CalculateBonus();
        currentHealth = PlayerPrefs.GetInt("HP", maxHealth);
        Debug.Log("📂 Đã load game!");
    }
}
