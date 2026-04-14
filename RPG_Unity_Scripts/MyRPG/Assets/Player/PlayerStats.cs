using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    [Header("Chỉ số cơ bản")]
    public int level = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Tiềm năng & Vàng")]
    public int statPoints = 0;
    public int skillPoints = 0; // Điểm học kỹ năng
    public int gold = 0; 
    public int STR = 5; public int VIT = 5; public int AGI = 5; 

    [Header("Kho đồ & Kỹ năng")]
    public List<string> inventory = new List<string>();
    public List<string> unlockedSkills = new List<string>(); // Danh sách chiêu đã học

    [Header("Trang bị Đang Mặc")]
    public string eqHead = "Trống";
    public string eqBody = "Trống";
    public string eqLegs = "Trống";
    public string eqWeaponMain = "Tay Không"; // Vũ khí chính (1H hoặc 2H)
    public string eqWeaponOff = "Trống";      // Khiên hoặc Vũ khí phụ (Chỉ khi Main là 1H)
    public string eqRing1 = "Trống";
    public string eqRing2 = "Trống";
    public string eqNecklace = "Trống";
    public string eqAncientGold = "Trống";    // Ô Vàng cổ

    [Header("Sức mạnh Tăng Cường")]
    public int bonusDamage = 0; 
    public int bonusDefense = 0;

    void Start()
    {
        currentHealth = maxHealth;
        CalculateBonus();
    }

    public void TakeDamage(int damage)
    {
        int thucTeBiTru = damage - bonusDefense - (VIT * 2);
        if (thucTeBiTru < 1) thucTeBiTru = 1;
        currentHealth -= thucTeBiTru;
        if (GameUI.instance != null) GameUI.instance.ShowDamage(transform.position, "-" + thucTeBiTru, Color.red);
        if (currentHealth <= 0) Debug.Log("GAME OVER!");
    }

    public void AddGold(int amount) { gold += amount; }
    public void AddExp(int amount) { currentExp += amount; if (currentExp >= expToNextLevel) LevelUp(); }

    void LevelUp()
    {
        currentExp -= expToNextLevel; level++;
        expToNextLevel += 50; 
        statPoints += 5; 
        if (level % 3 == 0) skillPoints++; // Cứ 3 level được 1 điểm kỹ năng
        maxHealth += 20; currentHealth = maxHealth; 
        Debug.Log("CHÚC MỪNG! LÊN CẤP " + level);
    }

    public void PickUpItem(string itemName) { inventory.Add(itemName); }

    // --- MẶC ĐỒ (Logic phức tạp cho 1H/2H/Khiên) ---
    public void EquipItem(int index, string subSlot = "")
    {
        if (inventory.Count <= index) return;
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
            // Nếu đang cầm hàng 2 tay thì không cho mặc khiên
            if (IsTwoHanded(eqWeaponMain)) { Debug.Log("Vũ khí 2 tay không thể dùng khiên!"); return; }
            oldItem = eqWeaponOff; eqWeaponOff = newItem;
        }
        else // Vũ khí (Kiếm, Đao...)
        {
            if (IsTwoHanded(newItem))
            {
                // Mặc đồ 2 tay -> Phải tháo khiên
                if (eqWeaponOff != "Trống") { inventory.Add(eqWeaponOff); eqWeaponOff = "Trống"; }
                oldItem = eqWeaponMain; eqWeaponMain = newItem;
            }
            else { oldItem = eqWeaponMain; eqWeaponMain = newItem; }
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

    public void SellItem(int index) { if (index < 0 || index >= inventory.Count) return; gold += GetItemPrice(inventory[index]); inventory.RemoveAt(index); }
    public int GetItemPrice(string n) { 
        if (n.Contains("Huyết")) return 500; if (n.Contains("Khiên")) return 80; if (n.Contains("Nhẫn")) return 200; 
        if (n.Contains("Ngọc")) return 300; return 20; 
    }

    public void CalculateBonus()
    {
        bonusDamage = (STR * 3); bonusDefense = 0; maxHealth = 100 + (VIT * 10);
        
        ApplyStats(eqHead); ApplyStats(eqBody); ApplyStats(eqLegs);
        ApplyStats(eqWeaponMain); ApplyStats(eqWeaponOff);
        ApplyStats(eqRing1); ApplyStats(eqRing2); 
        ApplyStats(eqNecklace); ApplyStats(eqAncientGold);

        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    void ApplyStats(string itemName)
    {
        if (itemName == "Trống" || itemName == "Tay Không") return;
        
        // Chỉ số gốc
        if (itemName.Contains("Kiếm Gỗ")) bonusDamage += 15;
        if (itemName.Contains("Huyết Kiếm")) bonusDamage += 100;
        if (itemName.Contains("Áo Da")) bonusDefense += 25;
        if (itemName.Contains("Mũ Sắt")) bonusDefense += 10;
        if (itemName.Contains("Giày")) { bonusDefense += 5; }
        if (itemName.Contains("Khiên")) bonusDefense += 40;
        if (itemName.Contains("Nhẫn Kim Cương")) { bonusDamage += 10; bonusDefense += 10; }
        if (itemName.Contains("Dây Chuyền Bạc")) { maxHealth += 50; }
        if (itemName.Contains("Vàng Cổ")) { bonusDamage += 20; bonusDefense += 20; maxHealth += 100; }

        // Logic KHẢM NGỌC (Nâng cao)
        if (itemName.Contains("Ngọc Đỏ")) bonusDamage += 50;
        if (itemName.Contains("Ngọc Xanh")) bonusDefense += 30;
        if (itemName.Contains("Ngọc Tím")) maxHealth += 200;
    }

    public void LearnSkill(string skillName)
    {
        if (skillPoints > 0 && !unlockedSkills.Contains(skillName))
        {
            unlockedSkills.Add(skillName);
            skillPoints--;
            Debug.Log("Đã học tuyệt kỹ: " + skillName);
        }
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("Level", level); PlayerPrefs.SetInt("Gold", gold); PlayerPrefs.SetInt("SkillPts", skillPoints);
        PlayerPrefs.SetString("Skills", string.Join(",", unlockedSkills));
        PlayerPrefs.SetString("Inv", string.Join(",", inventory));
        PlayerPrefs.SetString("WepM", eqWeaponMain); PlayerPrefs.SetString("WepO", eqWeaponOff);
        PlayerPrefs.SetString("R1", eqRing1); PlayerPrefs.SetString("R2", eqRing2);
        PlayerPrefs.SetString("Head", eqHead); PlayerPrefs.SetString("Body", eqBody);
        PlayerPrefs.SetString("Legs", eqLegs); PlayerPrefs.SetString("Neck", eqNecklace);
        PlayerPrefs.SetString("Anc", eqAncientGold);
        PlayerPrefs.SetInt("STR", STR); PlayerPrefs.SetInt("VIT", VIT); PlayerPrefs.SetInt("AGI", AGI);
        PlayerPrefs.SetFloat("PosX", transform.position.x); PlayerPrefs.SetFloat("PosY", transform.position.y);
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("Level")) {
            level = PlayerPrefs.GetInt("Level"); gold = PlayerPrefs.GetInt("Gold"); skillPoints = PlayerPrefs.GetInt("SkillPts");
            STR = PlayerPrefs.GetInt("STR"); VIT = PlayerPrefs.GetInt("VIT"); AGI = PlayerPrefs.GetInt("AGI");
            eqWeaponMain = PlayerPrefs.GetString("WepM"); eqWeaponOff = PlayerPrefs.GetString("WepO");
            eqRing1 = PlayerPrefs.GetString("R1"); eqRing2 = PlayerPrefs.GetString("R2");
            eqHead = PlayerPrefs.GetString("Head"); eqBody = PlayerPrefs.GetString("Body");
            eqLegs = PlayerPrefs.GetString("Legs"); eqNecklace = PlayerPrefs.GetString("Neck");
            eqAncientGold = PlayerPrefs.GetString("Anc");
            unlockedSkills = new List<string>(PlayerPrefs.GetString("Skills").Split(','));
            if (unlockedSkills.Count == 1 && unlockedSkills[0] == "") unlockedSkills.Clear();
            inventory = new List<string>(PlayerPrefs.GetString("Inv").Split(','));
            if(inventory.Count == 1 && inventory[0] == "") inventory.Clear();
            transform.position = new Vector3(PlayerPrefs.GetFloat("PosX"), PlayerPrefs.GetFloat("PosY"), transform.position.z);
            CalculateBonus(); currentHealth = maxHealth;
        }
    }
}
