using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class GameUI : MonoBehaviour
{
    public static GameUI instance;

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

    private PlayerStats player;
    private PlayerCombat combat;
    private bool isBagOpen = false;

    void Start()
    {
        player = FindAnyObjectByType<PlayerStats>();
        combat = FindAnyObjectByType<PlayerCombat>();
    }

    void Update()
    {
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

        // 1. THANH MÁU VÀ EXP 
        GUI.color = Color.black;
        GUI.DrawTexture(new Rect(20, 20, 204, 30), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(20, 52, 204, 15), Texture2D.whiteTexture);

        GUI.color = Color.red;
        float mauDoDai = ((float)player.currentHealth / player.maxHealth) * 200f;
        GUI.DrawTexture(new Rect(22, 22, mauDoDai, 26), Texture2D.whiteTexture);

        GUI.color = Color.blue;
        float expDoDai = ((float)player.currentExp / player.expToNextLevel) * 200f;
        GUI.DrawTexture(new Rect(22, 54, expDoDai, 11), Texture2D.whiteTexture);

        GUI.color = Color.white;
        GUI.Label(new Rect(30, 25, 200, 30), "HP: " + player.currentHealth + " / " + player.maxHealth);
        GUI.Label(new Rect(30, 51, 200, 30), "LV " + player.level);

        GUI.color = Color.yellow;
        GUI.Label(new Rect(Screen.width - 220, 20, 200, 30), "[ Bấm 'B' Mở Túi Đồ Toàn Diện ]");

        // KHUNG UI INVENTORY ĐẠI TU
        if (isBagOpen)
        {
            GUI.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
            GUI.DrawTexture(new Rect(Screen.width/2 - 300, 80, 600, 450), Texture2D.whiteTexture);
            
            GUI.color = Color.yellow;
            GUI.Label(new Rect(Screen.width/2 - 80, 90, 200, 30), "--- BẢNG NHÂN VẬT & TÚI ĐỒ ---");

            // --- BÊN TRÁI: DÁNG PAPERDOLL NỘM NGƯỜI (6 Ô Trang Bị) --- //
            float pdX = Screen.width/2 - 250;
            float pdY = 140;

            GUI.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            GUI.DrawTexture(new Rect(pdX, pdY, 200, 300), Texture2D.whiteTexture); 

            GUI.color = Color.white;
            GUI.Label(new Rect(pdX + 40, pdY + 10, 150, 30), "TRANG BỊ ĐANG MẶC");

            DrawSlot(pdX + 70, pdY + 40, "Đầu\n" + player.eqHead);      // Mũ 
            DrawSlot(pdX + 10, pdY + 110, "Trái\n" + player.eqWeapon);  // Vũ Khí 
            DrawSlot(pdX + 70, pdY + 110, "Áo\n" + player.eqBody);      // Áo 
            DrawSlot(pdX + 130, pdY + 110, "Dây\n" + player.eqNecklace);// Vòng Cổ 
            DrawSlot(pdX + 40, pdY + 180, "Nhẫn\n" + player.eqRing);    // Nhẫn 
            DrawSlot(pdX + 100, pdY + 180, "Giày\n" + player.eqLegs);    // Giày 

            int totalAtk = (combat != null ? combat.attackDamage : 0) + player.bonusDamage;
            int totalDef = player.bonusDefense;

            GUI.color = Color.green;
            GUI.Label(new Rect(pdX + 10, pdY + 250, 200, 30), "Sức Mạnh (ATK): " + totalAtk);
            GUI.color = Color.cyan;
            GUI.Label(new Rect(pdX + 10, pdY + 270, 200, 30), "Phòng Thủ (DEF): " + totalDef);

            // --- BÊN PHẢI: KHO ĐỒ LOOT ĐƯỢC --- //
            float invX = Screen.width/2;
            float invY = 140;

            GUI.color = Color.white;
            GUI.Label(new Rect(invX, invY, 150, 30), "HÒM ĐỒ CỦA BẠN (Click để mặc)");

            if (player.inventory.Count == 0)
            {
                GUI.color = Color.gray;
                GUI.Label(new Rect(invX, invY + 40, 200, 30), "- Hòm rỗng bọ chét -");
            }
            else
            {
                for (int i = 0; i < player.inventory.Count; i++)
                {
                    GUI.color = Color.white;
                    if (GUI.Button(new Rect(invX, invY + 30 + (i * 35), 250, 30), player.inventory[i]))
                    {
                        player.EquipItem(i);
                    }
                }
            }
        }

        Camera cam = Camera.main;
        if (cam != null && player.currentHealth > 0)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(player.transform.position);
            if (screenPos.z > 0)
            {
                float screenY = Screen.height - screenPos.y;
                float startX = screenPos.x - 30f;
                float startY = screenY - 50f;

                GUI.color = Color.black;
                GUI.DrawTexture(new Rect(startX, startY, 60f, 8f), Texture2D.whiteTexture);

                GUI.color = Color.green;
                float healthRatio = (float)player.currentHealth / player.maxHealth;
                GUI.DrawTexture(new Rect(startX, startY, 60f * healthRatio, 8f), Texture2D.whiteTexture);
                
                GUI.color = Color.white;
            }
        }
    }

    void DrawSlot(float x, float y, string content)
    {
        GUI.color = Color.gray;
        GUI.DrawTexture(new Rect(x, y, 60, 60), Texture2D.whiteTexture);
        GUI.color = Color.black;
        GUI.Label(new Rect(x + 5, y + 15, 60, 40), content); 
    }
}
