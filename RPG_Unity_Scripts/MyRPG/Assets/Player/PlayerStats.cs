using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Gắn script này vào nhân vật của bạn (Player)
public class PlayerStats : MonoBehaviour
{
    [Header("Chỉ số người chơi")]
    public int level = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;
    
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Kho đồ (Inventory) V.IP")]
    public List<string> inventory = new List<string>();
    
    [Header("Trang bị Đang Mặc")]
    public string equippedWeapon = "Tay Không";
    public int bonusDamage = 0; // Sức mạnh cộng thêm từ Trang bị

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Hàm nhận sát thương (quái đánh)
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Người chơi bị mất máu! HP còn: " + currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("CHẾT MẤT RỒI! GAME OVER!");
            // Gọi hàm game over...
        }
    }

    // Hàm nhận EXP khi diệt quái
    public void AddExp(int amount)
    {
        currentExp += amount;
        Debug.Log($"Nhận được {amount} Kinh nghiệm! Tổng EXP: {currentExp}/{expToNextLevel}");

        if (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    // Hàm thăng cấp
    void LevelUp()
    {
        currentExp -= expToNextLevel; // Giữ lại phần dư
        level++;
        expToNextLevel += 50; // Cấp sau cần nhiều EXP hơn
        maxHealth += 20;      // Máu trâu hơn
        currentHealth = maxHealth; // Hồi đầy máu
        
        Debug.Log("CHÚC MỪNG BẠN ĐÃ THĂNG CẤP LÊN CẤP " + level + "!");
    }

    // Nhặt vũ khí vào túi đồ
    public void PickUpWeapon(string weaponName)
    {
        inventory.Add(weaponName);
        Debug.Log("🧰 BẠN VỪA NHẶT ĐƯỢC: " + weaponName);
    }

    // Cơ chế ấn nút Mặc Đồ
    public void EquipWeapon(int indexViTriTrongTui)
    {
        if (inventory.Count > indexViTriTrongTui)
        {
            equippedWeapon = inventory[indexViTriTrongTui];
            
            // Soi tên vũ khí để truyền sức mạnh
            if (equippedWeapon == "Kiếm Gỗ Cùn") bonusDamage = 15;
            else if (equippedWeapon == "Huyết Kiếm") bonusDamage = 100;
            else bonusDamage = 0;

            Debug.Log("⚔️ ĐÃ TRANG BỊ: " + equippedWeapon + " (Sức mạnh được Buff: +" + bonusDamage + ")");
        }
    }
}
