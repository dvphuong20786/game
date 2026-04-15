using UnityEngine;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// ===========================
// Gán script này vào NPC Huấn Luyện (Trainer)
// Cho phép chiêu mộ tối đa 4 đệ tử
// ===========================
public class TrainerNPC : MonoBehaviour
{
    public string npcName = "Đại Sư Huấn Luyện";
    public float interactRange = 3f;
    public int maxCompanions = 4;

    [Header("Danh sách Đệ tử có thể chiêu mộ")]
    public GameObject warriorPrefab;
    public GameObject archerPrefab;
    public GameObject slimePrefab;

    private bool menuOpen = false;
    private PlayerStats player;
    private Vector2 scrollPos;

    void Start() { player = FindAnyObjectByType<PlayerStats>(); }

    void Update()
    {
        if (player == null) return;
        float dist = Vector2.Distance(transform.position, player.transform.position);

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
#else
        if (Input.GetKeyDown(KeyCode.E))
#endif
        {
            if (dist <= interactRange) menuOpen = !menuOpen;
            else menuOpen = false;
        }
        if (dist > interactRange + 1f) menuOpen = false;
    }

    void OnGUI()
    {
        if (player == null) return;
        float dist = Vector2.Distance(transform.position, player.transform.position);

        if (dist <= interactRange && !menuOpen)
        {
            Camera cam = Camera.main;
            if (cam != null) {
                Vector3 screenPos = cam.WorldToScreenPoint(transform.position);
                if (screenPos.z > 0) {
                    GUI.color = Color.yellow;
                    GUI.Label(new Rect(screenPos.x - 80, Screen.height - screenPos.y - 60, 160, 30), "[E] Chiêu mộ đệ tử", new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter, fontStyle=FontStyle.Bold});
                }
            }
        }

        if (menuOpen) DrawHireMenu();
    }

    void DrawHireMenu()
    {
        GUI.color = new Color(0, 0, 0, 0.6f);
        GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), Texture2D.whiteTexture);

        float w=600, h=450;
        float sx=Screen.width/2-w/2, sy=Screen.height/2-h/2;
        GUI.color = new Color(0.1f, 0.1f, 0.2f, 0.95f);
        GUI.DrawTexture(new Rect(sx, sy, w, h), Texture2D.whiteTexture);
        GUI.color = Color.yellow;
        GUI.Label(new Rect(sx, sy+10, w, 30), "🎖 CHIÊU MỘ ĐỒNG ĐỘI", new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter, fontSize=18, fontStyle=FontStyle.Bold});
        
        GUI.color = Color.white;
        GUI.Label(new Rect(sx+20, sy+10, 200, 30), "💰 Vàng: " + player.gold);
        
        // Đếm số đệ tử hiện có
        int currentCount = 0;
        CompanionAI[] comps = FindObjectsOfType<CompanionAI>();
        currentCount = comps.Length;
        GUI.Label(new Rect(sx+w-180, sy+10, 160, 30), "Đội ngũ: " + currentCount + "/" + maxCompanions);

        // Danh sách chiêu mộ
        float rowY = sy + 50;
        DrawHireRow(sx+20, rowY, "Kiếm Khách", 500, "Cận chiến, Máu trâu, có Chém Gió", warriorPrefab, currentCount);
        DrawHireRow(sx+20, rowY+90, "Cung Thủ", 800, "Tầm xa, Dame cực cao, Máu giấy", archerPrefab, currentCount);
        DrawHireRow(sx+20, rowY+180, "Slime Xanh", 300, "Hỗ trợ, Phản dame, làm chậm quái", slimePrefab, currentCount);

        GUI.color = Color.red;
        if (GUI.Button(new Rect(sx+w-40, sy+10, 30, 30), "X")) menuOpen = false;
    }

    void DrawHireRow(float x, float y, string name, int price, string desc, GameObject prefab, int count)
    {
        GUI.color = new Color(0.2f, 0.2f, 0.3f);
        GUI.DrawTexture(new Rect(x, y, 560, 80), Texture2D.whiteTexture);
        GUI.color = Color.cyan;
        GUI.Label(new Rect(x+10, y+10, 200, 25), name, new GUIStyle(GUI.skin.label){fontSize=14, fontStyle=FontStyle.Bold});
        GUI.color = Color.gray;
        GUI.Label(new Rect(x+10, y+35, 300, 40), desc);
        
        GUI.color = player.gold >= price ? Color.yellow : Color.gray;
        GUI.Label(new Rect(x+380, y+25, 80, 25), price + " 💰");
        
        if (player.gold >= price && count < maxCompanions && prefab != null) {
            GUI.color = Color.green;
            if (GUI.Button(new Rect(x+470, y+20, 80, 40), "THUÊ")) Hire(prefab, price);
        } else if (count >= maxCompanions) {
            GUI.color = Color.red;
            GUI.Box(new Rect(x+470, y+20, 80, 40), "Đầy đội");
        } else {
            GUI.color = Color.gray;
            GUI.Box(new Rect(x+470, y+20, 80, 40), "---");
        }
    }

    void Hire(GameObject prefab, int price)
    {
        player.gold -= price;
        Vector3 spawnPos = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        GameObject g = Instantiate(prefab, spawnPos, Quaternion.identity);
        
        // Phát hiệu ứng nếu có GameUI
        if (GameUI.instance != null)
            GameUI.instance.ShowDamage(player.transform.position, "Chào mừng " + name, Color.cyan);
            
        Debug.Log("🛡 Đã chiêu mộ đệ tử mới! Còn lại: " + player.gold + " Vàng");
    }
}
