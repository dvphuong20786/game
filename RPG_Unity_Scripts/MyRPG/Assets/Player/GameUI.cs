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
    private string selectedItemInfo = "";

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

        GUI.color = Color.black;
        GUI.DrawTexture(new Rect(20, 20, 250, 30), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(20, 52, 250, 15), Texture2D.whiteTexture);

        GUI.color = Color.red;
        float mauDoDai = ((float)player.currentHealth / player.maxHealth) * 246f;
        GUI.DrawTexture(new Rect(22, 22, mauDoDai, 26), Texture2D.whiteTexture);

        GUI.color = Color.blue;
        float expDoDai = ((float)player.currentExp / player.expToNextLevel) * 246f;
        GUI.DrawTexture(new Rect(22, 54, expDoDai, 11), Texture2D.whiteTexture);

        GUI.color = Color.white;
        GUI.Label(new Rect(30, 25, 200, 30), "HP: " + player.currentHealth + " / " + player.maxHealth);
        GUI.Label(new Rect(30, 51, 200, 30), "LV " + player.level + " - Bấm B Ký Sự");

        if (isBagOpen)
        {
            GUI.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
            GUI.DrawTexture(new Rect(Screen.width/2 - 400, 80, 800, 480), Texture2D.whiteTexture);
            
            GUI.color = Color.yellow;
            GUI.Label(new Rect(Screen.width/2 - 80, 90, 200, 30), "--- BẢNG NHÂN VẬT & TÚI ĐỒ ---");

            float pdX = Screen.width/2 - 380;
            float pdY = 140;

            GUI.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            GUI.DrawTexture(new Rect(pdX, pdY, 200, 300), Texture2D.whiteTexture); 

            GUI.color = Color.white;
            GUI.Label(new Rect(pdX + 40, pdY + 10, 150, 30), "TRANG BỊ ĐANG MẶC");

            DrawSlot(pdX + 70, pdY + 40, "Đầu\n" + player.eqHead);      
            DrawSlot(pdX + 10, pdY + 110, "Trái\n" + player.eqWeapon);  
            DrawSlot(pdX + 70, pdY + 110, "Áo\n" + player.eqBody);      
            DrawSlot(pdX + 130, pdY + 110, "Dây\n" + player.eqNecklace);
            DrawSlot(pdX + 40, pdY + 180, "Nhẫn\n" + player.eqRing);    
            DrawSlot(pdX + 100, pdY + 180, "Giày\n" + player.eqLegs);    

            int totalAtk = (combat != null ? combat.attackDamage : 0) + player.bonusDamage;
            int totalDef = player.bonusDefense;

            GUI.color = Color.green;
            GUI.Label(new Rect(pdX + 10, pdY + 250, 200, 30), "Sức Mạnh (ATK): " + totalAtk);
            GUI.color = Color.cyan;
            GUI.Label(new Rect(pdX + 10, pdY + 270, 200, 30), "Phòng Thủ (DEF): " + totalDef);

            float stX = Screen.width/2 - 150;
            GUI.color = Color.white;
            GUI.Label(new Rect(stX, 140, 200, 30), "ĐIỂM TIỀM NĂNG: " + player.statPoints);
            
            GUI.Label(new Rect(stX, 180, 100, 30), "Sức Mạnh: " + player.STR);
            if (player.statPoints > 0 && GUI.Button(new Rect(stX + 100, 180, 30, 20), "+"))
            {
                player.STR++; player.statPoints--; player.CalculateBonus();
            }

            GUI.Label(new Rect(stX, 220, 100, 30), "Thể Lực: " + player.VIT);
            if (player.statPoints > 0 && GUI.Button(new Rect(stX + 100, 220, 30, 20), "+"))
            {
                player.VIT++; player.statPoints--; player.CalculateBonus();
            }

            GUI.Label(new Rect(stX, 260, 100, 30), "Nhanh Nhẹn: " + player.AGI);
            if (player.statPoints > 0 && GUI.Button(new Rect(stX + 100, 260, 30, 20), "+"))
            {
                player.AGI++; player.statPoints--; player.CalculateBonus();
            }

            float invX = Screen.width/2 + 20;
            GUI.color = Color.white;
            GUI.Label(new Rect(invX, 140, 150, 30), "HÒM ĐỒ (Nhấn đúp mặc)");

            if (player.inventory.Count == 0)
            {
                GUI.color = Color.gray;
                GUI.Label(new Rect(invX, 180, 200, 30), "- Hòm rỗng bọ chét -");
            }
            else
            {
                for (int i = 0; i < player.inventory.Count; i++)
                {
                    GUI.color = Color.white;
                    if (GUI.Button(new Rect(invX, 170 + (i * 35), 180, 30), player.inventory[i]))
                    {
                        selectedItemInfo = player.inventory[i]; 
                        player.EquipItem(i); 
                    }
                }
            }

            float ttX = Screen.width/2 + 220;
            GUI.color = new Color(0.1f, 0.2f, 0.3f, 1f);
            GUI.DrawTexture(new Rect(ttX, 140, 160, 150), Texture2D.whiteTexture);
            
            if (selectedItemInfo != "") 
            {
                GUI.color = Color.yellow;
                GUI.Label(new Rect(ttX + 10, 150, 140, 50), "SOI: " + selectedItemInfo);
                
                string desc = "Vật phẩm cùi.\nKhông có tác dụng.";
                if (selectedItemInfo.Contains("Kiếm")) desc = "Sinh ra để chém giết.\nTăng cực mạnh ATK";
                if (selectedItemInfo.Contains("Áo")) desc = "Giáp dày.\nCản 25 sát thương";
                if (selectedItemInfo.Contains("Nhẫn")) desc = "Ma thuật.\nTăng cường chỉ số.";
                if (selectedItemInfo.Contains("Mũ")) desc = "Tránh nắng cản đòn.\nTăng 10 Giáp.";
                if (selectedItemInfo.Contains("Giày")) desc = "Giày trượt siêu việt.\nTăng 5 Giáp, lướt.";
                
                GUI.color = Color.white;
                GUI.Label(new Rect(ttX + 10, 190, 140, 100), desc);
            }
            else
            {
                GUI.color = Color.gray;
                GUI.Label(new Rect(ttX + 10, 200, 140, 100), "Nhấn vào một\nmón đồ bên trái\nđể soi chỉ số.");
            }

            GUI.color = Color.green;
            if (GUI.Button(new Rect(Screen.width/2 - 100, 520, 100, 30), "💾 LƯU GAME")) player.SaveGame();
            
            GUI.color = Color.cyan;
            if (GUI.Button(new Rect(Screen.width/2 + 10, 520, 100, 30), "📂 TẢI GAME")) player.LoadGame();
        }

        DrawFloatingHealthBar();
    }

    void DrawSlot(float x, float y, string content)
    {
        GUI.color = Color.gray;
        GUI.DrawTexture(new Rect(x, y, 60, 60), Texture2D.whiteTexture);
        GUI.color = Color.black;
        GUI.Label(new Rect(x + 5, y + 15, 60, 40), content); 
    }

    void DrawFloatingHealthBar()
    {
        Camera cam = Camera.main;
        if (cam != null && player != null && player.currentHealth > 0)
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
}
