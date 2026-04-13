using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("Chỉ số người chơi")]
    public int level = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;
    
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Điểm Tiềm Năng (MỚI)")]
    public int statPoints = 0;
    public int STR = 5; 
    public int VIT = 5; 
    public int AGI = 5; 

    [Header("Kho đồ (Inventory) V.IP")]
    public List<string> inventory = new List<string>();
    
    [Header("Trang bị Đang Mặc")]
    public string eqHead = "Trống";
    public string eqBody = "Trống";
    public string eqLegs = "Trống";
    public string eqWeapon = "Tay Không";
    public string eqRing = "Trống";
    public string eqNecklace = "Trống";

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
        Debug.Log("🛡 Bị đánh mất " + thucTeBiTru + " HP, Còn: " + currentHealth);

        if (currentHealth <= 0) Debug.Log("CHẾT MẤT RỒI! GAME OVER!");
    }

    public void AddExp(int amount)
    {
        currentExp += amount;
        if (currentExp >= expToNextLevel) LevelUp();
    }

    void LevelUp()
    {
        currentExp -= expToNextLevel; 
        level++;
        expToNextLevel += 50; 
        statPoints += 5; 
        maxHealth += 20;      
        currentHealth = maxHealth; 
    }

    public void PickUpItem(string itemName)
    {
        inventory.Add(itemName);
    }

    public void EquipItem(int index)
    {
        if (inventory.Count <= index) return;
        string itemName = inventory[index];

        if (itemName.Contains("Mũ")) eqHead = itemName;
        else if (itemName.Contains("Áo")) eqBody = itemName;
        else if (itemName.Contains("Giày")) eqLegs = itemName;
        else if (itemName.Contains("Nhẫn")) eqRing = itemName;
        else if (itemName.Contains("Dây")) eqNecklace = itemName;
        else eqWeapon = itemName; 

        CalculateBonus();
    }

    public void CalculateBonus()
    {
        bonusDamage = (STR * 3); 
        bonusDefense = 0;
        maxHealth = 100 + (VIT * 10);
        
        if (eqWeapon == "Kiếm Gỗ Cùn") bonusDamage += 15;
        else if (eqWeapon == "Huyết Kiếm") bonusDamage += 100;
        
        if (eqHead == "Mũ Sắt") bonusDefense += 10;
        if (eqBody == "Áo Da Lộn") bonusDefense += 25;
        if (eqLegs == "Giày Siêu Tốc") bonusDefense += 5; 

        if (eqRing == "Nhẫn Kim Cương") { bonusDamage += 10; bonusDefense += 10; }
        if (eqNecklace == "Dây Chuyền Bạc") { maxHealth += 50; } 
        
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("Exp", currentExp);
        PlayerPrefs.SetInt("Points", statPoints);
        PlayerPrefs.SetInt("STR", STR);
        PlayerPrefs.SetInt("VIT", VIT);
        PlayerPrefs.SetInt("AGI", AGI);
        PlayerPrefs.SetString("Wep", eqWeapon);
        PlayerPrefs.SetString("Head", eqHead);
        PlayerPrefs.SetString("Body", eqBody);
        PlayerPrefs.SetString("Legs", eqLegs);
        PlayerPrefs.SetString("Ring", eqRing);
        PlayerPrefs.SetString("Necklace", eqNecklace);

        string invStr = string.Join(",", inventory);
        PlayerPrefs.SetString("Inv", invStr);
        PlayerPrefs.Save();
        Debug.Log("💾 ĐÃ LƯU GAME!");
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("Level"))
        {
            level = PlayerPrefs.GetInt("Level");
            currentExp = PlayerPrefs.GetInt("Exp");
            statPoints = PlayerPrefs.GetInt("Points");
            STR = PlayerPrefs.GetInt("STR");
            VIT = PlayerPrefs.GetInt("VIT");
            AGI = PlayerPrefs.GetInt("AGI");
            
            eqWeapon = PlayerPrefs.GetString("Wep");
            eqHead = PlayerPrefs.GetString("Head");
            eqBody = PlayerPrefs.GetString("Body");
            eqLegs = PlayerPrefs.GetString("Legs");
            eqRing = PlayerPrefs.GetString("Ring");
            eqNecklace = PlayerPrefs.GetString("Necklace");

            string invStr = PlayerPrefs.GetString("Inv");
            inventory = new List<string>(invStr.Split(','));
            if(inventory.Count == 1 && inventory[0] == "") inventory.Clear();

            expToNextLevel = 100 + (level - 1) * 50;
            CalculateBonus();
            currentHealth = maxHealth;
            Debug.Log("📂 ĐÃ LOAD GAME!");
        }
    }
}
