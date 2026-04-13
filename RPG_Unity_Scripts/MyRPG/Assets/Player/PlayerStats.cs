using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Gắn script này vào nhân vật của bạn (Player)
public class PlayerStats : MonoBehaviour
{
    // Bản thể duy nhất để mang sang Map mới
    public static PlayerStats instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Phép thuật Bất tử khi trôi qua Cổng Map
        }
        else
        {
            // Khi quay lại map cũ, diệt bản sao lậu
            Destroy(gameObject);
        }
    }

    [Header("Chỉ số người chơi")]
    public int level = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;
    
    public int maxHealth = 100;
    public int currentHealth;

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
        // Sử dụng bonusDefense để cản đòn, tối thiểu là mất 1 máu
        int thucTeBiTru = damage - bonusDefense;
        if (thucTeBiTru < 1) thucTeBiTru = 1;

        currentHealth -= thucTeBiTru;
        Debug.Log("🛡 Ai da! Bị đánh mất " + thucTeBiTru + " HP, Còn: " + currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("CHẾT MẤT RỒI! GAME OVER!");
        }
    }

    public void AddExp(int amount)
    {
        currentExp += amount;
        if (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentExp -= expToNextLevel; 
        level++;
        expToNextLevel += 50; 
        maxHealth += 20;      
        currentHealth = maxHealth; 
    }

    public void PickUpItem(string itemName)
    {
        inventory.Add(itemName);
        Debug.Log("🧰 BẠN VỪA NHẶT ĐƯỢC: " + itemName);
    }

    // Logic Tự Động Định Ví Trí Mặc Đồ
    public void EquipItem(int index)
    {
        if (inventory.Count <= index) return;
        string itemName = inventory[index];

        if (itemName.Contains("Mũ")) eqHead = itemName;
        else if (itemName.Contains("Áo")) eqBody = itemName;
        else if (itemName.Contains("Giày")) eqLegs = itemName;
        else if (itemName.Contains("Nhẫn")) eqRing = itemName;
        else if (itemName.Contains("Dây")) eqNecklace = itemName;
        else eqWeapon = itemName; // Mặc định là vũ khí

        CalculateBonus();
    }

    public void CalculateBonus()
    {
        bonusDamage = 0;
        bonusDefense = 0;

        // Tính Vũ Khí
        if (eqWeapon == "Kiếm Gỗ Cùn") bonusDamage += 15;
        else if (eqWeapon == "Huyết Kiếm") bonusDamage += 100;
        
        // Tính Giáp
        if (eqHead == "Mũ Sắt") bonusDefense += 10;
        if (eqBody == "Áo Da Lộn") bonusDefense += 25;
        if (eqLegs == "Giày Siêu Tốc") bonusDefense += 5;

        // Tính Trang sức
        if (eqRing == "Nhẫn Kim Cương") { bonusDamage += 10; bonusDefense += 10; }
        if (eqNecklace == "Dây Chuyền Bạc") { maxHealth += 50; currentHealth += 50; } 
    }
}
