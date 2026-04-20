using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ===========================
// HỆ THỐNG CHỈ SỐ, RANK & TRANG BỊ NÂNG CAO
// ===========================
public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    [Header("Loại Nhân Vật & Rank")]
    public bool isPlayer = true;
    public string characterName = "Hiệp Sĩ";
    public string characterRank = "D"; // Rank: D, C, B, A, S

    [Header("Trạng thái Buff")]
    public List<string> activeBuffs = new List<string>();

    void Awake()
    {
        if (isPlayer)
        {
            if (instance == null) { 
                instance = this; 
                DontDestroyOnLoad(gameObject); 
                SceneManager.sceneLoaded += OnSceneLoaded;
                Debug.Log("🛡️ [PLAYERSTATS]: Khởi tạo HIỆP SĨ CHÍNH (Persisted): " + characterName);
            }
            else { 
                Debug.Log("⚠️ [PLAYERSTATS]: Phát hiện HIỆP SĨ NHÂN BẢN! Đang tự hủy bản sao map mới...");
                gameObject.SetActive(false); // Vô hiệu hóa ngay lập tức
                Destroy(gameObject); 
                return; 
            }
        }
        else {
            Debug.Log("👥 [PLAYERSTATS]: Khởi tạo Đồng đội: " + characterName);
        }
        CalculateBonus();
        currentHealth = maxHealth;
    }

    void OnDestroy() { if (isPlayer) SceneManager.sceneLoaded -= OnSceneLoaded; }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    { 
        if (isPlayer && instance == this) 
        {
            // Quét và xóa mọi bản sao khác "vô tình" xuất hiện trong map mới
            PlayerStats[] allPlayers = FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
            foreach (var p in allPlayers) {
                if (p != this && p.isPlayer) {
                    Debug.Log("🚫 [PlayerStats] Đã phát hiện và xóa bản sao Hiệp sĩ tại map mới.");
                    p.gameObject.SetActive(false);
                    Destroy(p.gameObject);
                }
            }

            if (PlayerPrefs.HasKey("Level")) LoadGame(); 
        }
    }

    [Header("Chỉ số cơ bản")]
    public int level = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Tiềm năng & Tài nguyên")]
    public int statPoints = 0;
    public int skillPoints = 0;
    public int gold = 0;
    public int STR = 5; public int VIT = 5; public int AGI = 5;

    [System.Serializable]
    public class SkillProgress { 
        public SkillData data; public int level; 
        public SkillProgress(SkillData d, int l) { data = d; level = l; }
    }

    [Header("Kho đồ & Kỹ năng (DẠNG MỚI)")]
    public List<ItemInstance> inventory = new List<ItemInstance>();
    public List<SkillProgress> unlockedSkills = new List<SkillProgress>();

    public List<ItemInstance> SharedInventory {
        get { if (isPlayer) return inventory; if (instance != null) return instance.inventory; return inventory; }
    }

    [Header("Trang bị Đang Mặc (DẠNG MỚI)")]
    public ItemInstance eqHead;
    public ItemInstance eqBody;
    public ItemInstance eqLegs;
    public ItemInstance eqWeaponMain;
    public ItemInstance eqWeaponOff;
    public ItemInstance eqRing1;
    public ItemInstance eqRing2;
    public ItemInstance eqNecklace;
    public ItemInstance eqAncientGold;

    [Header("Sức mạnh Tăng Cường")]
    public int bonusDamage = 0;
    public int bonusDefense = 0;

    void Start()
    {
        if (isPlayer) { if (PlayerPrefs.HasKey("Level")) LoadGame(); else { CalculateBonus(); currentHealth = maxHealth; } }
        else { CalculateBonus(); currentHealth = maxHealth; }

        // --- TỰ ĐỘNG THIẾT LẬP HIỂN THỊ VŨ KHÍ (MỚI) ---
        GameObject weaponHand = new GameObject("WeaponVisualHand");
        weaponHand.transform.SetParent(this.transform);
        weaponHand.transform.localPosition = new Vector3(0.3f, 0, 0); // Vị trí tay
        weaponHand.AddComponent<WeaponVisual>().stats = this;
    }

    // --- LOGIC CHIẾN ĐẤU ---
    public void TakeDamage(int damage)
    {
        int giapTuNhien = VIT / 2;
        int thucTe = damage - bonusDefense - giapTuNhien;
        if (thucTe < 1) thucTe = 1;
        currentHealth -= thucTe;
        if (GameUI.instance != null) GameUI.instance.ShowDamage(transform.position, "-" + thucTe, isPlayer ? Color.red : new Color(1f, 0.5f, 0.5f));
        if (currentHealth <= 0) currentHealth = 0;
    }

    public void AddExp(int amount) { currentExp += amount; while (currentExp >= expToNextLevel) LevelUp(); }
    void LevelUp()
    {
        currentExp -= expToNextLevel; level++;
        expToNextLevel = (int)(expToNextLevel * 1.2f) + 50;
        statPoints += 5; if (level % 3 == 0) skillPoints++;
        CalculateBonus(); currentHealth = maxHealth;
        if (GameUI.instance != null) GameUI.instance.ShowDamage(transform.position, "LÊN CẤP " + level + "!", Color.yellow);
    }

    // --- HỆ THỐNG ĐỘT PHÁ RANK ---
    public void PromoteCharacter()
    {
        int cost = GetPromoteCost();
        string targetSoulName = GetRequiredSoulName();
        int soulIdx = -1;
        
        // KIỂM TRA ĐIỀU KIỆN
        if (gold < cost) {
             if (GameUI.instance != null) GameUI.instance.ShowDamage(transform.position, "THIẾU VÀNG!", Color.red);
             return; 
        }

        // Kiểm tra Linh hồn trong túi đồ chung
        ItemInstance soulItem = null;
        for(int i = 0; i < SharedInventory.Count; i++) {
            var item = SharedInventory[i];
            if (item != null && item.data != null && item.data.itemName == targetSoulName) {
                soulItem = item;
                soulIdx = i;
                break;
            }
        }

        if (soulItem == null) {
            if (GameUI.instance != null) GameUI.instance.ShowDamage(transform.position, "THIẾU LINH HỒN!", Color.magenta);
            Debug.Log($"⚠️ Cần có {targetSoulName} trong túi đồ chung để đột phá!");
            return;
        }

        // Thực hiện đột phá
        gold -= cost;
        SharedInventory.RemoveAt(soulIdx); // Tiêu thụ linh hồn
        
        string oldRank = characterRank;
        if      (characterRank == "D") characterRank = "C";
        else if (characterRank == "C") characterRank = "B";
        else if (characterRank == "B") characterRank = "A";
        else if (characterRank == "A") characterRank = "S";

        statPoints += 20; // Thưởng điểm khi lên Rank
        CalculateBonus();
        
        if (GameUI.instance != null)
            GameUI.instance.ShowDamage(transform.position, $"✨ ĐỘT PHÁ {oldRank} -> {characterRank}!", Color.cyan);
    }

    public string GetRequiredSoulName() {
        if (isPlayer) return "Linh hồn Hiệp sĩ";
        // Đối với đệ tử, lấy tên Class của họ
        return "Linh hồn " + characterName;
    }

    public int GetPromoteCost() {
        if (characterRank == "D") return 500;
        if (characterRank == "C") return 2000;
        if (characterRank == "B") return 10000;
        if (characterRank == "A") return 50000;
        return 0;
    }

    float GetPromoteFailChance() {
        if (characterRank == "D") return 0.1f;
        if (characterRank == "C") return 0.25f;
        if (characterRank == "B") return 0.4f;
        if (characterRank == "A") return 0.6f;
        return 0;
    }

    // --- HỆ THỐNG TRANG BỊ ---
    public void PickUpItem(ItemData data) { SharedInventory.Add(new ItemInstance(data)); }

    public bool EquipItem(int index, string subSlot = "")
    {
        if (index < 0 || index >= SharedInventory.Count) return false;
        ItemInstance newItem = SharedInventory[index];
                if (newItem.data == null) return false;
        if (level < newItem.data.requiredLevel) {
            if (GameUI.instance != null) GameUI.instance.ShowDamage(transform.position, "CHUA �? TR�NH!", Color.red);
            return false;
        }
        
        ItemInstance oldItem = null;
        var data = newItem.data;

        if (data.type == ItemData.ItemType.Armor) {
             if (data.itemName.Contains("Mũ") || data.itemName.Contains("Nồi") || data.itemName.Contains("Nắp") ) { oldItem = eqHead; eqHead = newItem; }
             else if (data.itemName.Contains("Áo") || data.itemName.Contains("Bao") || data.itemName.Contains("Giáp") ) { oldItem = eqBody; eqBody = newItem; }
             else if (data.itemName.Contains("Giày") || data.itemName.Contains("Dép") ) { oldItem = eqLegs; eqLegs = newItem; }
        }
        else if (data.type == ItemData.ItemType.Accessory) {
             if (data.itemName.Contains("Dây") || data.itemName.Contains("Sợi") ) { oldItem = eqNecklace; eqNecklace = newItem; }
             else if (data.itemName.Contains("Vàng Cổ")) { oldItem = eqAncientGold; eqAncientGold = newItem; }
             else if (data.itemName.Contains("Nhẫn") || data.itemName.Contains("Đá") ) {
                if (subSlot == "Ring1") { oldItem = eqRing1; eqRing1 = newItem; }
                else { oldItem = eqRing2; eqRing2 = newItem; }
             }
        }
        else if (data.itemName.Contains("Khiên") || data.itemName.Contains("Nắp") ) { 
            // Nếu đang cầm vũ khí 2 tay thì phải tháo ra trước (YÊU CẦU MỤC 7)
            if (eqWeaponMain != null && eqWeaponMain.data != null && eqWeaponMain.data.isTwoHanded) {
                UnequipItem("WepMain");
            }
            oldItem = eqWeaponOff; eqWeaponOff = newItem; 
        }
        else if (data.type == ItemData.ItemType.Weapon) { 
            // Nếu là vũ khí 2 tay thì tháo Khiên ra (YÊU CẦU MỤC 7)
            if (data.isTwoHanded) {
                UnequipItem("WepOff");
            }
            oldItem = eqWeaponMain; eqWeaponMain = newItem; 
        }

        SharedInventory.RemoveAt(index);
        // Đưa món đồ vào túi chung
        SharedInventory.Add(oldItem);
        CalculateBonus();
        return true;
    }

    // --- GỠ ĐỒ (UNEQUIP) (YÊU CẦU MỤC 5) ---
    public void UnequipItem(string slot)
    {
        ItemInstance target = null;
        if      (slot == "Head") { target = eqHead; eqHead = null; }
        else if (slot == "Body") { target = eqBody; eqBody = null; }
        else if (slot == "Legs") { target = eqLegs; eqLegs = null; }
        else if (slot == "WepMain") { target = eqWeaponMain; eqWeaponMain = null; }
        else if (slot == "WepOff")  { target = eqWeaponOff;  eqWeaponOff = null; }
        else if (slot == "Ring1")   { target = eqRing1;      eqRing1 = null; }
        else if (slot == "Ring2")   { target = eqRing2;      eqRing2 = null; }
        else if (slot == "Neck")    { target = eqNecklace;   eqNecklace = null; }
        else if (slot == "Ancient") { target = eqAncientGold; eqAncientGold = null; }

        if (target != null) {
            SharedInventory.Add(target);
            CalculateBonus();
            Debug.Log($"❌ Đã tháo {target.data.itemName} về túi đồ chung.");
        }
    }

    public void Unequip(string slot)
    {
        ItemInstance item = null;
        if (slot == "Head") { item = eqHead; eqHead = null; }
        else if (slot == "Body") { item = eqBody; eqBody = null; }
        else if (slot == "Legs") { item = eqLegs; eqLegs = null; }
        else if (slot == "WepMain") { item = eqWeaponMain; eqWeaponMain = null; }
        else if (slot == "WepOff") { item = eqWeaponOff; eqWeaponOff = null; }
        else if (slot == "Ring1") { item = eqRing1; eqRing1 = null; }
        else if (slot == "Ring2") { item = eqRing2; eqRing2 = null; }
        else if (slot == "Neck") { item = eqNecklace; eqNecklace = null; }
        else if (slot == "Ancient") { item = eqAncientGold; eqAncientGold = null; }

        if (item != null) { SharedInventory.Add(item); CalculateBonus(); }
    }

    public void CalculateBonus()
    {
        // Rank Bonus Multiplier
        float rankMult = 1.0f;
        if (characterRank == "C") rankMult = 1.2f;
        else if (characterRank == "B") rankMult = 1.5f;
        else if (characterRank == "A") rankMult = 2.0f;
        else if (characterRank == "S") rankMult = 3.0f;

        bonusDamage = (int)(STR * 2 * rankMult);
        maxHealth = (int)((100 + (VIT * 15)) * rankMult + 50); // Cộng thêm base health cho cứng cáp
        bonusDefense = 0;

        ApplyInstanceStats(eqHead); ApplyInstanceStats(eqBody); ApplyInstanceStats(eqLegs);
        ApplyInstanceStats(eqWeaponMain); ApplyInstanceStats(eqWeaponOff);
        ApplyInstanceStats(eqRing1); ApplyInstanceStats(eqRing2);
        ApplyInstanceStats(eqNecklace); ApplyInstanceStats(eqAncientGold);

        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    void ApplyInstanceStats(ItemInstance inst) {
        if (inst == null) return;
        bonusDamage += inst.GetTotalAtk();
        bonusDefense += inst.GetTotalDef();
        maxHealth += inst.GetTotalHP();
    }

    public void UseConsumable(int index) {
        if (index < 0 || index >= SharedInventory.Count) return;
        ItemInstance inst = SharedInventory[index];
        if (inst.data != null && inst.data.type == ItemData.ItemType.Consumable) {
            currentHealth = Mathf.Min(currentHealth + inst.data.healAmount, maxHealth); 
            SharedInventory.RemoveAt(index); 
        }
    }

    public int GetSkillLevel(string skillName) {
        foreach (var s in unlockedSkills) if (s.data != null && s.data.skillName == skillName) return s.level;
        return 0;
    }

    // --- LƯU & TẢI (HỖ TRỢ ITEM INSTANCE) ---
    public void SaveGame()
    {
        if (!isPlayer) return;
        PlayerPrefs.SetInt("Level", level); PlayerPrefs.SetInt("Gold", gold);
        PlayerPrefs.SetInt("HP", currentHealth); PlayerPrefs.SetInt("StatPts", statPoints);
        PlayerPrefs.SetInt("SkillPts", skillPoints); PlayerPrefs.SetString("Rank", characterRank);
        PlayerPrefs.SetInt("STR", STR); PlayerPrefs.SetInt("VIT", VIT); PlayerPrefs.SetInt("AGI", AGI);
        
        // Lưu Inventory theo định dạng: Name:Plus:Gems;...
        List<string> invS = new List<string>();
        foreach(var inst in inventory) invS.Add(SerializeInstance(inst));
        PlayerPrefs.SetString("InvV2", string.Join(";", invS));

        // Lưu Equip
        PlayerPrefs.SetString("E_Head", SerializeInstance(eqHead));
        PlayerPrefs.SetString("E_Body", SerializeInstance(eqBody));
        PlayerPrefs.SetString("E_Legs", SerializeInstance(eqLegs));
        PlayerPrefs.SetString("E_WMain", SerializeInstance(eqWeaponMain));
        PlayerPrefs.SetString("E_WOff", SerializeInstance(eqWeaponOff));
        PlayerPrefs.SetString("E_R1", SerializeInstance(eqRing1));
        PlayerPrefs.SetString("E_R2", SerializeInstance(eqRing2));
        PlayerPrefs.SetString("E_Neck", SerializeInstance(eqNecklace));
        PlayerPrefs.SetString("E_Anc", SerializeInstance(eqAncientGold));

        // 1. Lưu Kỹ năng & Buff
        List<string> skillDat = new List<string>();
        foreach(var sk in unlockedSkills) if(sk.data != null) skillDat.Add(sk.data.name + ":" + sk.level);
        PlayerPrefs.SetString("SavedSkills", string.Join(";", skillDat));
        PlayerPrefs.SetString("SavedBuffs", string.Join(",", activeBuffs));

        // 2. Lưu Đệ tử đi kèm
        if (CompanionManager.instance != null) CompanionManager.instance.SaveCompanions();

        PlayerPrefs.Save();
        Debug.Log("💾 [PLAYERSTATS]: Đã lưu TOÀN BỘ dữ liệu (Skills, Buffs, Companions).");
    }

    public void LoadGame()
    {
        if (!isPlayer) return;
        level = PlayerPrefs.GetInt("Level", 1);
        gold = PlayerPrefs.GetInt("Gold", 0);
        statPoints = PlayerPrefs.GetInt("StatPts", 0);
        skillPoints = PlayerPrefs.GetInt("SkillPts", 0);
        characterRank = PlayerPrefs.GetString("Rank", "D");
        STR = PlayerPrefs.GetInt("STR", 5); VIT = PlayerPrefs.GetInt("VIT", 5); AGI = PlayerPrefs.GetInt("AGI", 5);

        inventory.Clear();
        string[] iv = PlayerPrefs.GetString("InvV2", "").Split(';');
        foreach(var s in iv) { var inst = DeserializeInstance(s); if(inst != null) inventory.Add(inst); }

        eqHead = DeserializeInstance(PlayerPrefs.GetString("E_Head"));
        eqBody = DeserializeInstance(PlayerPrefs.GetString("E_Body"));
        eqLegs = DeserializeInstance(PlayerPrefs.GetString("E_Legs"));
        eqWeaponMain = DeserializeInstance(PlayerPrefs.GetString("E_WMain"));
        eqWeaponOff = DeserializeInstance(PlayerPrefs.GetString("E_WOff"));
        eqRing1 = DeserializeInstance(PlayerPrefs.GetString("E_R1"));
        eqRing2 = DeserializeInstance(PlayerPrefs.GetString("E_R2"));
        eqNecklace = DeserializeInstance(PlayerPrefs.GetString("E_Neck"));
        eqAncientGold = DeserializeInstance(PlayerPrefs.GetString("E_Anc"));

        CalculateBonus();
        currentHealth = PlayerPrefs.GetInt("HP", maxHealth);

        // 1. Tải Kỹ năng & Buff
        unlockedSkills.Clear();
        string skStr = PlayerPrefs.GetString("SavedSkills", "");
        if (!string.IsNullOrEmpty(skStr)) {
            foreach(var sPart in skStr.Split(';')) {
                var sSub = sPart.Split(':');
                if (sSub.Length == 2) {
                    SkillData sData = Resources.Load<SkillData>("Skills/" + sSub[0]);
                    if (sData != null) unlockedSkills.Add(new SkillProgress(sData, int.Parse(sSub[1])));
                }
            }
        }
        string buffStr = PlayerPrefs.GetString("SavedBuffs", "");
        activeBuffs = new List<string>(buffStr.Split(new char[]{','}, System.StringSplitOptions.RemoveEmptyEntries));

        // 2. Tải và triệu hồi Đệ tử
        if (CompanionManager.instance != null) {
            CompanionManager.instance.LoadAndSpawnCompanions(transform);
        }
        
        Debug.Log("📂 [PLAYERSTATS]: Tải dữ liệu hoàn tất! Đã khôi phục Skills & Buffs.");
    }

    public string SerializeInstance(ItemInstance inst) {
        if (inst == null || inst.data == null) return "";
        string gems = "";
        foreach(var g in inst.sockets) if(g != null) gems += (gems==""?"":".") + g.name;
        // Format mới: name!plus!rank!bAtk!bDef!bHp!gems
        return $"{inst.data.name}!{inst.plusLevel}!{inst.itemRank}!{inst.rankBonusAtk}!{inst.rankBonusDef}!{inst.rankBonusHp}!{gems}";
    }

    public ItemInstance DeserializeInstance(string data) {
        if (string.IsNullOrEmpty(data)) return null;
        string[] p = data.Split('!');
        if (p.Length < 2) return null;
        
        string itemName = p[0].Trim();
        ItemData b = Resources.Load<ItemData>("Items/" + itemName);
        if (b == null) return null;
        
        ItemInstance inst = new ItemInstance(b);
        inst.plusLevel = int.Parse(p[1]);
        
        // Hỗ trợ định dạng mới (p.Length >= 6)
        if (p.Length >= 6) {
            inst.itemRank = int.Parse(p[2]);
            inst.rankBonusAtk = int.Parse(p[3]);
            inst.rankBonusDef = int.Parse(p[4]);
            inst.rankBonusHp = int.Parse(p[5]);
            
            // Ngọc (gems) nằm ở p[6]
            if (p.Length > 6 && !string.IsNullOrEmpty(p[6])) {
                foreach(var gName in p[6].Split('.')) {
                    ItemData gem = Resources.Load<ItemData>("Items/" + gName.Trim());
                    if (gem != null) inst.sockets.Add(gem);
                }
            }
        } 
        else {
            // Định dạng cũ: name!plus!gems (gems nằm ở p[2])
            if (p.Length > 2 && !string.IsNullOrEmpty(p[2])) {
                foreach(var gName in p[2].Split('.')) {
                    ItemData gem = Resources.Load<ItemData>("Items/" + gName.Trim());
                    if (gem != null) inst.sockets.Add(gem);
                }
            }
        }
        
        return inst;
    }

    public void AddGold(int amount) { gold += amount; SaveGame(); }

    public void AddStat(string stat) {
        if (statPoints <= 0) return;
        if (stat == "STR") STR++;
        else if (stat == "VIT") VIT++;
        else if (stat == "AGI") AGI++;
        statPoints--;
        CalculateBonus();
    }

    public static void ShareExp(int totalExp) {
        PlayerStats[] all = Object.FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
        List<PlayerStats> targets = new List<PlayerStats>();
        foreach (var s in all) if (s.gameObject.activeInHierarchy) targets.Add(s);
        int share = totalExp / Mathf.Max(1, targets.Count);
        foreach (var s in targets) s.AddExp(share);
    }

    public void SellItem(int index) {
        if (index < 0 || index >= inventory.Count) return;
        gold += Mathf.Max(1, inventory[index].data.price / 2);
        inventory.RemoveAt(index);
        CalculateBonus();
        if (GameUI.instance != null) GameUI.instance.ShowDamage(transform.position, "ĐÃ BÁN!", Color.yellow);
    }

    public void LearnSkill(SkillData skill)
    {
        if (skillPoints < 1) return;
        SkillProgress target = null;
        foreach (var s in unlockedSkills) if (s.data == skill) { target = s; break; }

        if (target == null) {
            unlockedSkills.Add(new SkillProgress(skill, 1));
            skillPoints--;
        } else if (target.level < 10) {
            target.level++;
            skillPoints--;
        }
        if (GameUI.instance != null) GameUI.instance.ShowDamage(transform.position, "✨ KỸ NĂNG MỚI!", Color.cyan);
    }

    public void ResetGame()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
