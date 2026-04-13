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

    [Header("Kho đồ (Inventory)")]
    public List<string> inventory = new List<string>();

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

    // Nhặt đồ vào túi
    public void PickUpItem(string itemName)
    {
        inventory.Add(itemName);
        Debug.Log("Đã nhặt được: " + itemName + ". Số đồ trong túi: " + inventory.Count);
    }
}
