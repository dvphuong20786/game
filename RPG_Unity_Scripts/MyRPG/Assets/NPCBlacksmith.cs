using UnityEngine;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// ===========================
// NPC THO REN (BLACKSMITH NPC)
// Giao dich: Mua/Ban + Rèn đúc
// Ket noi toi BlacksmithUI.cs de hien thi giao dien day du
// ===========================
public class NPCBlacksmith : MonoBehaviour
{
    [Header("Thong tin NPC")]
    public string npcName = "Thương Nhân Lão Trần";
    public float interactRange = 2.5f;

    private PlayerStats player;
    private List<ItemData> shopStock = new List<ItemData>();

    void Start()
    {
        FindPlayer();
        InitializeStock();
    }

    void InitializeStock()
    {
        shopStock.Clear();
        ItemData[] allItems = Resources.LoadAll<ItemData>("Items");
        foreach (var item in allItems)
        {
            if (item.type == ItemData.ItemType.Weapon ||
                item.type == ItemData.ItemType.Armor ||
                item.type == ItemData.ItemType.Accessory)
            {
                shopStock.Add(item);
            }
        }
    }

    void FindPlayer()
    {
        PlayerStats[] all = Object.FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
        foreach (var s in all) if (s.isPlayer) { player = s; break; }
    }

    void Update()
    {
        if (player == null) { FindPlayer(); return; }
        
        // TỰ ĐỘNG TÌM KIẾM BỘ NÃO UI NẾU CHƯA CÓ
        if (BlacksmithUI.instance == null) {
            BlacksmithUI.instance = FindObjectOfType<BlacksmithUI>();
        }

        float dist = Vector2.Distance(transform.position, player.transform.position);

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
#else
        if (Input.GetKeyDown(KeyCode.E))
#endif
        {
            if (dist <= interactRange)
            {
                // Mo/Dong BlacksmithUI
                if (BlacksmithUI.instance != null)
                {
                    if (BlacksmithUI.instance.isOpen)
                        BlacksmithUI.instance.Close();
                    else
                        BlacksmithUI.instance.Open(shopStock);
                }
            }
        }

        // Tu dong dong khi ra xa
        if (dist > interactRange + 1f && BlacksmithUI.instance != null && BlacksmithUI.instance.isOpen)
            BlacksmithUI.instance.Close();
    }

    void OnGUI()
    {
        if (player == null) return;
        
        // KIỂM TRA LỖI THIẾU COMPONENT
        if (BlacksmithUI.instance == null)
        {
            float d = Vector2.Distance(transform.position, player.transform.position);
            if (d <= interactRange) {
                Camera c = Camera.main;
                if (c != null) {
                    Vector3 p = c.WorldToScreenPoint(transform.position);
                    if (p.z > 0) {
                        GUI.color = Color.red;
                        GUI.Label(new Rect(p.x - 150, Screen.height - p.y - 100, 300, 40), 
                            "⚠️ THIẾU BLACKSMITH UI TRONG SCENE!\nHãy tạo 1 GameObject và kéo script BlacksmithUI vào.", 
                            new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter, fontStyle=FontStyle.Bold, fontSize=12});
                    }
                }
            }
            return;
        }

        bool bsOpen = BlacksmithUI.instance.isOpen;
        float dist = Vector2.Distance(transform.position, player.transform.position);

        // Hiện label [E] khi lại gần
        if (dist <= interactRange && !bsOpen)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                Vector3 p = cam.WorldToScreenPoint(transform.position);
                if (p.z > 0)
                {
                    GUI.color = Color.cyan;
                    GUIStyle st = new GUIStyle(GUI.skin.label)
                    {
                        fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter
                    };
                    GUI.Label(new Rect(p.x - 100, Screen.height - p.y - 60, 200, 30),
                        "[E] Rèn đúc & Mua bán", st);
                }
            }
        }

        // Vẽ cửa sổ BlacksmithUI
        if (bsOpen) BlacksmithUI.instance.DrawWindow();
    }
}
