using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class GameUI : MonoBehaviour
{
    private PlayerStats player;
    private PlayerCombat combat;
    
    // Nút gạt tắt mở Túi Đồ
    private bool isBagOpen = false;

    void Start()
    {
        player = FindAnyObjectByType<PlayerStats>();
        combat = FindAnyObjectByType<PlayerCombat>();
    }

    void Update()
    {
        // Sửa lỗi: Dùng hệ thống Phím Bấm đời mới của Unity 6 để quét phím B
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame)
        {
            isBagOpen = !isBagOpen;
        }
#else
        if (Input.GetKeyDown(KeyCode.B))
        {
            isBagOpen = !isBagOpen;
        }
#endif
    }

    void OnGUI()
    {
        if (player == null || combat == null) return;

        // ==========================================
        // 1. THANH MÁU (ĐỎ) VÀ KINH NGHIỆM (XANH) GÓC TRÁI
        // ==========================================
        // Khung đen
        GUI.color = Color.black;
        GUI.DrawTexture(new Rect(20, 20, 204, 30), Texture2D.whiteTexture); // Khung Máu
        GUI.DrawTexture(new Rect(20, 52, 204, 15), Texture2D.whiteTexture); // Khung EXP

        // Lõi Đỏ (Máu)
        GUI.color = Color.red;
        float mauDoDai = ((float)player.currentHealth / player.maxHealth) * 200f;
        GUI.DrawTexture(new Rect(22, 22, mauDoDai, 26), Texture2D.whiteTexture);

        // Lõi Xanh Dương (EXP)
        GUI.color = Color.blue;
        float expDoDai = ((float)player.currentExp / player.expToNextLevel) * 200f;
        GUI.DrawTexture(new Rect(22, 54, expDoDai, 11), Texture2D.whiteTexture);

        // Chấm chữ
        GUI.color = Color.white;
        GUI.Label(new Rect(30, 25, 200, 30), "HP: " + player.currentHealth + " / " + player.maxHealth);
        GUI.Label(new Rect(30, 51, 200, 30), "LV " + player.level);

        // ==========================================
        // 2. KHUNG TRANG BỊ HIỆN TẠI GÓC PHẢI
        // ==========================================
        GUI.color = new Color(0, 0, 0, 0.7f); // Nền đen mờ trong suốt
        GUI.DrawTexture(new Rect(Screen.width - 230, 20, 220, 60), Texture2D.whiteTexture);
        
        GUI.color = Color.white;
        int tongLucChien = combat.attackDamage + player.bonusDamage;
        GUI.Label(new Rect(Screen.width - 220, 25, 200, 30), "Vũ Khí: " + player.equippedWeapon);
        GUI.Label(new Rect(Screen.width - 220, 45, 200, 30), "Lực Chiến: " + tongLucChien + " Đame");

        // Hướng dẫn mở túi cực kỳ rõ nét
        GUI.color = Color.yellow;
        GUI.Label(new Rect(Screen.width - 220, 85, 200, 30), "[ Bấm 'B' để Mở Túi Đồ Của Bạn ]");

        // ==========================================
        // 3. TÚI ĐỒ (INVENTORY) Ở GIỮA MÀN HÌNH (Chỉ hiện khi bật công tắc isBagOpen)
        // ==========================================
        if (isBagOpen == true)
        {
            // Vẽ hộp vuông khổng lồ làm Túi đồ (Nằm ngay giữa màn hình)
            GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.95f);
            GUI.DrawTexture(new Rect(Screen.width/2 - 150, 100, 300, 400), Texture2D.whiteTexture);
            
            GUI.color = Color.yellow;
            GUI.Label(new Rect(Screen.width/2 - 60, 110, 200, 30), "--- TÚI ĐỒ CỦA BẠN ---");

            if (player.inventory.Count == 0)
            {
                GUI.color = Color.white;
                GUI.Label(new Rect(Screen.width/2 - 80, 150, 200, 30), "Bạn đang nghèo rớt mồng tơi...");
            }
            else
            {
                // Liệt kê mọi thứ trong túi để đúc thành Nút bấm
                for (int i = 0; i < player.inventory.Count; i++)
                {
                    GUI.color = Color.white;
                    // Lệnh GUI.Button vẽ ra một Nút bấm thực thụ. Nếu lấy chuột nhấp vào nút này, nó sẽ ném ra chữ 'true'
                    bool isClicked = GUI.Button(new Rect(Screen.width/2 - 130, 150 + (i * 45), 260, 35), "Trang Bị: " + player.inventory[i]);
                    
                    if (isClicked)
                    {
                        // Máy chạy lệnh Mặc Vũ Khí vào người
                        player.EquipWeapon(i);
                    }
                }
            }
        }

        // ==========================================
        // 4. VẼ THANH MÁU NỔI TRÊN ĐẦU NHÂN VẬT (Tính năng kinh điển hồi nãy)
        // ==========================================
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(player.transform.position);

            if (screenPos.z > 0 && player.currentHealth > 0)
            {
                float screenY = Screen.height - screenPos.y;
                float barWidth = 60f;
                float barHeight = 8f;
                float yOffset = 50f; 
                float startX = screenPos.x - (barWidth / 2);
                float startY = screenY - yOffset;

                GUI.color = Color.black;
                GUI.DrawTexture(new Rect(startX, startY, barWidth, barHeight), Texture2D.whiteTexture);

                GUI.color = Color.green;
                float healthRatio = (float)player.currentHealth / player.maxHealth;
                GUI.DrawTexture(new Rect(startX, startY, barWidth * healthRatio, barHeight), Texture2D.whiteTexture);
                
                GUI.color = Color.white;
            }
        }
    }
}
