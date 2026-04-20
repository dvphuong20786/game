using UnityEngine;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// ===========================
// NPC THỢ RÈN (BLACKSMITH NPC)
// Chuyên bán Vũ khí, Giáp, Trang sức và cung cấp dịch vụ rèn đúc
// ===========================
public class NPCBlacksmith : MonoBehaviour
{
    [Header("Thông tin NPC")]
    public string npcName = "Thợ Rèn Ba Đô";
    public float interactRange = 2.5f;

    private bool shopOpen = false;
    private PlayerStats player;
    private Vector2 scrollBuy;
    private Vector2 scrollSell;
    private int tab = 0; // 0=Mua, 1=Bán

    private List<ItemData> shopStock = new List<ItemData>();

    void Start()
    {
        FindPlayer();
        InitializeStock();
    }

    void InitializeStock()
    {
        // LỌC: Chỉ lấy Vũ khí, Giáp và Trang sức để bán
        ItemData[] allItems = Resources.LoadAll<ItemData>("Items");
        foreach(var item in allItems) {
            if (item.type == ItemData.ItemType.Weapon || 
                item.type == ItemData.ItemType.Armor || 
                item.type == ItemData.ItemType.Accessory) 
            {
                shopStock.Add(item);
            }
        }
    }

    void FindPlayer() {
        PlayerStats[] all = Object.FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
        foreach (var s in all) if (s.isPlayer) { player = s; break; }
    }

    void Update() {
        if (player == null) { FindPlayer(); return; }
        float dist = Vector2.Distance(transform.position, player.transform.position);

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
#else
        if (Input.GetKeyDown(KeyCode.E))
#endif
        {
            if (dist <= interactRange) shopOpen = !shopOpen;
            else shopOpen = false;
        }
        if (dist > interactRange + 1f) shopOpen = false;
    }

    void OnGUI()
    {
        if (player == null) return;
        float dist = Vector2.Distance(transform.position, player.transform.position);

        if (dist <= interactRange && !shopOpen) {
            Camera cam = Camera.main;
            if (cam != null) {
                Vector3 p = cam.WorldToScreenPoint(transform.position);
                if (p.z > 0) {
                    GUI.color = Color.cyan;
                    GUIStyle st = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
                    GUI.Label(new Rect(p.x - 80, Screen.height - p.y - 60, 160, 30), "[E] Rèn đúc & Giao dịch", st);
                }
            }
        }

        if (shopOpen) DrawUI();
    }

    void DrawUI()
    {
        GUI.color = new Color(0, 0, 0, 0.5f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);

        float w = 700, h = 520;
        float sx = Screen.width / 2f - w / 2f;
        float sy = Screen.height / 2f - h / 2f;

        GUI.color = new Color(0.1f, 0.08f, 0.08f, 0.98f); // Màu tối hơn, cảm giác kim loại
        GUI.DrawTexture(new Rect(sx, sy, w, h), Texture2D.whiteTexture);
        GUI.color = Color.cyan;
        GUI.DrawTexture(new Rect(sx, sy, w, 2), Texture2D.whiteTexture); // Viền trên xanh neon

        GUI.color = Color.white;
        GUIStyle titleS = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        GUI.Label(new Rect(sx, sy + 10, w, 30), "🛠 " + npcName, titleS);

        if (GUI.Button(new Rect(sx + w - 40, sy + 10, 30, 25), "✕")) shopOpen = false;

        GUI.color = tab == 0 ? Color.cyan : Color.white;
        if (GUI.Button(new Rect(sx + 20, sy + 50, 150, 30), "MUA TRANG BỊ")) tab = 0;
        GUI.color = tab == 1 ? Color.yellow : Color.white;
        if (GUI.Button(new Rect(sx + 180, sy + 50, 150, 30), "BÁN VẬT PHẨM")) tab = 1;

        if (tab == 0) DrawBuy(); else DrawSell();
    }

    void DrawBuy() {
        Rect scrollArea = new Rect(Screen.width/2 - 340, Screen.height/2 - 190, 680, 340);
        Rect contentR = new Rect(0, 0, 660, shopStock.Count * 55);
        scrollBuy = GUI.BeginScrollView(scrollArea, scrollBuy, contentR);
        for (int i = 0; i < shopStock.Count; i++) {
            var item = shopStock[i];
            float y = i * 55;
            GUI.color = new Color(0.15f, 0.15f, 0.15f);
            GUI.DrawTexture(new Rect(0, y, 660, 50), Texture2D.whiteTexture);
            GUI.color = Color.white;
            if (item.icon != null) GUI.DrawTexture(new Rect(5, y+5, 40, 40), item.icon.texture);
            GUI.Label(new Rect(55, y+5, 300, 20), item.itemName, new GUIStyle(GUI.skin.label){fontStyle=FontStyle.Bold});
            GUI.color = Color.yellow;
            GUI.Label(new Rect(55, y+25, 300, 20), "Giá: " + item.price + " Vàng");
            if (player.gold >= item.price) {
                GUI.color = Color.green;
                if (GUI.Button(new Rect(550, y+10, 80, 30), "MUA")) { 
                    player.gold -= item.price; 
                    player.inventory.Add(new ItemInstance(item)); 
                }
            } else {
                GUI.color = Color.gray;
                GUI.Box(new Rect(550, y+10, 80, 30), "Thiếu vàng");
            }
        }
        GUI.EndScrollView();
    }

    void DrawSell() {
        Rect scrollArea = new Rect(Screen.width/2 - 340, Screen.height/2 - 190, 680, 340);
        Rect contentR = new Rect(0, 0, 660, player.inventory.Count * 55);
        scrollSell = GUI.BeginScrollView(scrollArea, scrollSell, contentR);
        for (int i = player.inventory.Count - 1; i >= 0; i--) {
            var inst = player.inventory[i];
            if (inst == null || inst.data == null) continue;
            float y = (player.inventory.Count - 1 - i) * 55;
            GUI.color = new Color(0.12f, 0.1f, 0.1f);
            GUI.DrawTexture(new Rect(0, y, 660, 50), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(new Rect(55, y+15, 300, 20), inst.GetDisplayName());
            int p = Mathf.Max(1, inst.data.price / 2);
            GUI.color = Color.yellow; GUI.Label(new Rect(400, y+15, 100, 20), "+" + p + "G");
            GUI.color = Color.red;
            if (GUI.Button(new Rect(550, y+10, 80, 30), "BÁN")) { player.gold += p; player.inventory.RemoveAt(i); }
        }
        GUI.EndScrollView();
    }
}

