using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ===========================
// HỆ THỐNG CHỈ SỐ, RANK & TRANG BỊ NÂNG CAO
// ===========================
public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;
    private static Dictionary<string, ItemData> itemCache = null;

    void Start() {
        if (itemCache == null) {
            itemCache = new Dictionary<string, ItemData>();
            ItemData[] allItems = Resources.LoadAll<ItemData>("");
            foreach (var item in allItems) {
                if (item != null && !itemCache.ContainsKey(item.name)) 
                    itemCache.Add(item.name, item);
            }
            Debug.Log("📦 [PlayerStats] Đã Cache " + itemCache.Count + " vật phẩm.");
        }
    }

    [Header("Loại Nhân Vật & Rank")]
    public bool isPlayer = true;
    public string characterName = "Hiệp Sĩ";
    public string characterRank = "D"; // Rank: D, C, B, A, S
    public Sprite characterPortrait;

    [Header("Trạng thái Buff")]
    public List<string> activeBuffs = new List<string>();

    void Awake()
    {
        if (characterPortrait == null) {
            if (isPlayer) characterPortrait = Resources.Load<Sprite>("Sprites/warrior_portrait");
            else if (characterName.Contains("Archer")) characterPortrait = Resources.Load<Sprite>("NPC/DETU/Archer");
            else if (characterName.Contains("Slime") || characterName.Contains("Smile")) characterPortrait = Resources.Load<Sprite>("NPC/DETU/Smile0_Idle_0"); // Example path based on screenshot
        }

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
        currentHealth = maxHealth; currentMana = maxMana;
        
        // Tải dữ liệu ngay khi khởi tạo nếu đã có file lưu
        if (isPlayer && PlayerPrefs.HasKey("Level")) LoadGame();
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
    public int maxMana = 50;
    public int currentMana;

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

    [Header("Chỉ số sau khi tính Bonus")]
    public int bonusDamage;
    public int bonusDefense;
    public int hBonusTotal;

    public void CalculateBonus()
    {
        bonusDamage = STR * 2;
        bonusDefense = VIT * 1;
        int maxH = 100 + (VIT * 10);
        
        ApplyEquipmentStats(ref bonusDamage, ref bonusDefense, ref hBonusTotal);
        
        maxHealth = maxH + hBonusTotal;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    void ApplyEquipmentStats(ref int a, ref int d, ref int h)
    {
        if (eqHead != null) { a += eqHead.GetTotalAtk(); d += eqHead.GetTotalDef(); h += eqHead.GetTotalHP(); }
        if (eqBody != null) { a += eqBody.GetTotalAtk(); d += eqBody.GetTotalDef(); h += eqBody.GetTotalHP(); }
        if (eqLegs != null) { a += eqLegs.GetTotalAtk(); d += eqLegs.GetTotalDef(); h += eqLegs.GetTotalHP(); }
        if (eqWeaponMain != null) { a += eqWeaponMain.GetTotalAtk(); d += eqWeaponMain.GetTotalDef(); h += eqWeaponMain.GetTotalHP(); }
        if (eqWeaponOff != null) { a += eqWeaponOff.GetTotalAtk(); d += eqWeaponOff.GetTotalDef(); h += eqWeaponOff.GetTotalHP(); }
        if (eqRing1 != null) { a += eqRing1.GetTotalAtk(); d += eqRing1.GetTotalDef(); h += eqRing1.GetTotalHP(); }
        if (eqRing2 != null) { a += eqRing2.GetTotalAtk(); d += eqRing2.GetTotalDef(); h += eqRing2.GetTotalHP(); }
        if (eqNecklace != null) { a += eqNecklace.GetTotalAtk(); d += eqNecklace.GetTotalDef(); h += eqNecklace.GetTotalHP(); }
    }

    public void GainExp(int amount)
    {
        currentExp += amount;
        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    public static void ShareExp(int amount)
    {
        PlayerStats[] all = FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
        if (all.Length == 0) return;
        
        int expEach = amount / all.Length;
        foreach(PlayerStats p in all)
        {
            if (p.currentHealth > 0) p.GainExp(expEach);
        }
        Debug.Log("🌟 [EXP] Nhận " + amount + " kinh nghiệm, chia cho " + all.Length + " người (" + expEach + "/người)");
    }

    public void AddGold(int amount)
    {
        if (isPlayer) gold += amount;
        else if (instance != null) instance.gold += amount;
        else gold += amount;
    }

    void LevelUp()
    {
        currentExp -= expToNextLevel;
        level++;
        statPoints += 3;
        skillPoints += 1;
        expToNextLevel = (int)(expToNextLevel * 1.5f);
        CalculateBonus();
        currentHealth = maxHealth;
        Debug.Log("🎉 [LEVEL UP]: Lên cấp " + level + "! Nhận 3 điểm tiềm năng.");
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

    public void TakeDamage(int dmg)
    {
        int realDmg = Mathf.Max(1, dmg - (bonusDefense / 2));
        currentHealth -= realDmg;
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        currentHealth = 0;
        Debug.Log("💀 " + characterName + " đã tử trận!");
        if (isPlayer) {
            currentHealth = maxHealth;
            transform.position = Vector3.zero;
        }
    }

    // ===========================
    // HỆ THỐNG TRANG BỊ & KHO ĐỒ
    // ===========================
    public void PickUpItem(ItemData data, int rank = 1)
    {
        ItemInstance newItem = new ItemInstance(data);
        newItem.itemRank = rank;
        newItem.GenerateRankBonus();
        inventory.Add(newItem);
        Debug.Log("🎒 Đã nhặt: " + newItem.GetDisplayName());
    }

    public void EquipItem(ItemInstance item)
    {
        if (item == null || item.data == null) return;
        
        string slotToEquip = "";
        string itemNameLow = item.data.itemName.ToLower();
        
        if (item.data.type == ItemData.ItemType.Weapon) {
            if (itemNameLow.Contains("khiên")) slotToEquip = "WepOff";
            else slotToEquip = "WepMain";
        } else if (item.data.type == ItemData.ItemType.Armor) {
            if (itemNameLow.Contains("mũ") || itemNameLow.Contains("đầu")) slotToEquip = "Head";
            else if (itemNameLow.Contains("giày") || itemNameLow.Contains("ủng")) slotToEquip = "Legs";
            else slotToEquip = "Body";
        } else if (item.data.type == ItemData.ItemType.Accessory) {
            if (itemNameLow.Contains("dây")) slotToEquip = "Neck";
            else if (itemNameLow.Contains("vàng") || itemNameLow.Contains("cổ đại") || itemNameLow.Contains("vòng cổ")) slotToEquip = "Ancient";
            else {
                if (eqRing1 == null) slotToEquip = "Ring1";
                else slotToEquip = "Ring2";
            }
        }
        
        if (slotToEquip == "") return;

        UnequipItem(slotToEquip);

        switch (slotToEquip)
        {
            case "Head": eqHead = item; break;
            case "Neck": eqNecklace = item; break;
            case "Ancient": eqAncientGold = item; break;
            case "WepMain": eqWeaponMain = item; break;
            case "Body": eqBody = item; break;
            case "WepOff": eqWeaponOff = item; break;
            case "Ring1": eqRing1 = item; break;
            case "Ring2": eqRing2 = item; break;
            case "Legs": eqLegs = item; break;
        }

        // Xóa item khỏi túi đồ CHUNG chứ không phải túi đồ ảo của đệ tử
        if (isPlayer) inventory.Remove(item);
        else if (instance != null) instance.inventory.Remove(item);

        CalculateBonus();
    }

    public void UnequipItem(string slot)
    {
        ItemInstance item = null;
        switch (slot) {
            case "Head": item = eqHead; eqHead = null; break;
            case "Neck": item = eqNecklace; eqNecklace = null; break;
            case "Ancient": item = eqAncientGold; eqAncientGold = null; break;
            case "WepMain": item = eqWeaponMain; eqWeaponMain = null; break;
            case "Body": item = eqBody; eqBody = null; break;
            case "WepOff": item = eqWeaponOff; eqWeaponOff = null; break;
            case "Ring1": item = eqRing1; eqRing1 = null; break;
            case "Ring2": item = eqRing2; eqRing2 = null; break;
            case "Legs": item = eqLegs; eqLegs = null; break;
            case "Weapon": item = eqWeaponMain; eqWeaponMain = null; break; // fallback
            case "Armor": item = eqBody; eqBody = null; break; // fallback
        }
        
        if (item != null) {
            if (isPlayer) inventory.Add(item);
            else if (instance != null) instance.inventory.Add(item);
        }
        CalculateBonus();
    }

    public void LearnSkill(SkillData skill) {
        if (skillPoints <= 0) return;
        var found = unlockedSkills.Find(s => s.data.skillName == skill.skillName);
        if (found != null) { if (found.level < 10) { found.level++; skillPoints--; } }
        else { unlockedSkills.Add(new SkillProgress(skill, 1)); skillPoints--; }
    }

    public int GetSkillLevel(string skillName) {
        if (unlockedSkills == null) return 0;
        var found = unlockedSkills.Find(s => s.data != null && s.data.skillName == skillName);
        return (found != null) ? found.level : 0;
    }

    public void SaveGame() {
        if (!isPlayer) return;
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("Exp", currentExp);
        PlayerPrefs.SetInt("Gold", gold);
        PlayerPrefs.SetInt("STR", STR);
        PlayerPrefs.SetInt("VIT", VIT);
        PlayerPrefs.SetInt("AGI", AGI);
        PlayerPrefs.SetInt("StatPoints", statPoints);
        PlayerPrefs.SetInt("SkillPoints", skillPoints);
        
        // Lưu Kho đồ
        List<string> invStrings = new List<string>();
        foreach (var item in inventory) {
            if (item != null) invStrings.Add(SerializeInstance(item));
        }
        PlayerPrefs.SetString("Inventory", string.Join("|", invStrings));

        // Lưu Trang bị đang mặc
        PlayerPrefs.SetString("EqHead", SerializeInstance(eqHead));
        PlayerPrefs.SetString("EqBody", SerializeInstance(eqBody));
        PlayerPrefs.SetString("EqLegs", SerializeInstance(eqLegs));
        PlayerPrefs.SetString("EqWMain", SerializeInstance(eqWeaponMain));
        PlayerPrefs.SetString("EqWOff", SerializeInstance(eqWeaponOff));
        PlayerPrefs.SetString("EqR1", SerializeInstance(eqRing1));
        PlayerPrefs.SetString("EqR2", SerializeInstance(eqRing2));
        PlayerPrefs.SetString("EqNeck", SerializeInstance(eqNecklace));
        PlayerPrefs.SetString("EqAnc", SerializeInstance(eqAncientGold));

        // Lưu Kỹ năng
        List<string> skillStrings = new List<string>();
        foreach (var sk in unlockedSkills) {
            if(sk.data != null) skillStrings.Add(sk.data.name + ":" + sk.level);
        }
        PlayerPrefs.SetString("UnlockedSkills", string.Join("|", skillStrings));

        // Lưu Đệ tử
        if (CompanionManager.instance != null) CompanionManager.instance.SaveCompanions();

        PlayerPrefs.Save();
        Debug.Log("💾 Đã lưu game đầy đủ.");
    }

    public void LoadGame() {
        if (!isPlayer) return;
        level = PlayerPrefs.GetInt("Level", 1);
        currentExp = PlayerPrefs.GetInt("Exp", 0);
        gold = PlayerPrefs.GetInt("Gold", 0);
        STR = PlayerPrefs.GetInt("STR", 5);
        VIT = PlayerPrefs.GetInt("VIT", 5);
        AGI = PlayerPrefs.GetInt("AGI", 5);
        statPoints = PlayerPrefs.GetInt("StatPoints", 0);
        skillPoints = PlayerPrefs.GetInt("SkillPoints", 0);

        // Tải Kho đồ
        inventory.Clear();
        string invStr = PlayerPrefs.GetString("Inventory", "");
        // Fallback cho key cũ nếu key mới trống
        if (string.IsNullOrEmpty(invStr)) invStr = PlayerPrefs.GetString("inventory", "");
        
        if (!string.IsNullOrEmpty(invStr)) {
            foreach (string s in invStr.Split('|')) {
                if (string.IsNullOrEmpty(s)) continue;
                var inst = DeserializeInstance(s);
                if (inst != null) inventory.Add(inst);
            }
        }

        // Tải Trang bị
        eqHead = DeserializeInstance(PlayerPrefs.GetString("EqHead", "None"));
        eqBody = DeserializeInstance(PlayerPrefs.GetString("EqBody", "None"));
        eqLegs = DeserializeInstance(PlayerPrefs.GetString("EqLegs", "None"));
        eqWeaponMain = DeserializeInstance(PlayerPrefs.GetString("EqWMain", "None"));
        eqWeaponOff = DeserializeInstance(PlayerPrefs.GetString("EqWOff", "None"));
        eqRing1 = DeserializeInstance(PlayerPrefs.GetString("EqR1", "None"));
        eqRing2 = DeserializeInstance(PlayerPrefs.GetString("EqR2", "None"));
        eqNecklace = DeserializeInstance(PlayerPrefs.GetString("EqNeck", "None"));
        eqAncientGold = DeserializeInstance(PlayerPrefs.GetString("EqAnc", "None"));

        // Tải Kỹ năng
        unlockedSkills.Clear();
        string skillStr = PlayerPrefs.GetString("UnlockedSkills", "");
        if (!string.IsNullOrEmpty(skillStr)) {
            foreach (string s in skillStr.Split('|')) {
                if (string.IsNullOrEmpty(s)) continue;
                string[] p = s.Split(':');
                if (p.Length == 2) {
                    SkillData sd = Resources.Load<SkillData>("Skills/" + p[0]);
                    if (sd != null) unlockedSkills.Add(new SkillProgress(sd, int.Parse(p[1])));
                }
            }
        }

        // Tải Đệ tử
        if (CompanionManager.instance != null) {
            CompanionManager.instance.LoadAndSpawnCompanions(transform);
        }

        CalculateBonus();
        currentHealth = maxHealth;
        Debug.Log("📂 Đã tải game cấp " + level + " kèm đội hình.");
    }

    public void ResetGame() {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void UseConsumable(int idx) {
        if (idx < 0 || idx >= SharedInventory.Count) return;
        ItemInstance inst = SharedInventory[idx];
        if (inst.data.type == ItemData.ItemType.Consumable) {
            currentHealth = Mathf.Min(maxHealth, currentHealth + inst.data.healAmount);
            SharedInventory.RemoveAt(idx);
            CalculateBonus();
        }
    }
    public string SerializeInstance(ItemInstance inst) {
        if (inst == null || inst.data == null) return "None";
        string sStr = "0";
        if (inst.sockets != null && inst.sockets.Count > 0) {
            List<string> sl = new List<string>();
            foreach(var ok in inst.sockets) if(ok != null) sl.Add(ok.name);
            if (sl.Count > 0) sStr = string.Join("&", sl);
        }
        return inst.data.name + ":" + inst.plusLevel + ":" + inst.itemRank + ":" + sStr;
    }
    
    public ItemInstance DeserializeInstance(string data) {
        if (string.IsNullOrEmpty(data) || data == "None") return null;
        char sep = data.Contains(":") ? ':' : ',';
        string[] p = data.Split(sep);
        
        if (p.Length >= 1) {
            string itemName = p[0].Trim();
            ItemData d = null;
            
            // Ưu tiên dùng Cache để tìm chính xác bất kể thư mục
            if (itemCache != null && itemCache.ContainsKey(itemName)) {
                d = itemCache[itemName];
            } else {
                // Fallback nếu chưa Cache kịp
                d = Resources.Load<ItemData>("Items/" + itemName);
                if (d == null) d = Resources.Load<ItemData>(itemName);
            }

            if (d == null) return null;
            
            ItemInstance inst = new ItemInstance(d);
            if (p.Length >= 2) int.TryParse(p[1], out inst.plusLevel);
            if (p.Length >= 3) {
                int.TryParse(p[2], out inst.itemRank);
                inst.GenerateRankBonus();
            }
            if (p.Length >= 4 && p[3] != "0") {
                string[] skl = p[3].Split('&');
                foreach(string g in skl) {
                    ItemData gd = Resources.Load<ItemData>("Items/" + g);
                    if (gd != null) inst.sockets.Add(gd);
                }
            }
            return inst;
        }
        return null;
    }
}
