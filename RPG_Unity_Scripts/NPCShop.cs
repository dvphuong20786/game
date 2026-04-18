using UnityEngine;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// ===========================
// Gắn script này vào một GameObject đại diện cho NPC người bán hàng
// Người chơi đứng gần + bấm phím E để mở cửa hàng
// ===========================
public class NPCShop : MonoBehaviour
{
    [Header("Tên NPC")]
    public string npcName = "Thương Nhân Lão Trần";

    [Header("Tầm tương tác (đứng gần bao nhiêu mới mở được)")]
    public float interactRange = 2.5f;

    private bool shopOpen = false;
    private PlayerStats player;
    private Vector2 scrollBuy;
    private Vector2 scrollSell;
    private int sellTab = 0; // 0=Mua, 1=Bán

    // ===== DANH SÁCH HÀNG HÓA =====
    class ShopItem
    {
        public string name;
        public int price;
        public string description;
        public ShopItem(string n, int p, string d) { name = n; price = p; description = d; }
    }

    private List<ShopItem> shopInventory = new List<ShopItem>
    {
        // Tiêu hao
        new ShopItem("Bình Máu Nhỏ",     50,  "Hồi 30 HP. Dùng ngay từ túi đồ"),
        new ShopItem("Bình Máu Lớn",     150, "Hồi 100 HP. Dùng ngay từ túi đồ"),
        // Vũ khí
        new ShopItem("Kiếm Gỗ",          80,  "⚔ +8 Tấn công | Vũ khí 1 tay"),
        new ShopItem("Kiếm Sắt",         200, "⚔ +15 Tấn công | Vũ khí 1 tay"),
        // Giáp
        new ShopItem("Mũ Sắt",           60,  "🛡 +5 Phòng thủ"),
        new ShopItem("Áo Da Lộn",        100, "🛡 +12 Phòng thủ"),
        new ShopItem("Áo Giáp",          220, "🛡 +18 Phòng thủ"),
        new ShopItem("Giày Siêu Tốc",    75,  "🛡 +3 Phòng thủ"),
        // Khiên
        new ShopItem("Khiên Gỗ",         120, "🛡 +10 Phòng thủ | Chỉ dùng với vũ khí 1 tay"),
        // Trang sức
        new ShopItem("Nhẫn Kim Cương",   250, "⚔ +5 Tấn công | 🛡 +5 Phòng thủ"),
        new ShopItem("Dây Chuyền Bạc",   180, "❤ +30 HP tối đa"),
        // Ngọc khảm
        new ShopItem("Ngọc Đỏ",          300, "⚔ +20 Tấn công (khảm vào tên đồ)"),
        new ShopItem("Ngọc Xanh",        300, "🛡 +15 Phòng thủ (khảm vào tên đồ)"),
        new ShopItem("Ngọc Tím",         300, "❤ +80 HP tối đa (khảm vào tên đồ)"),
    };

    void Start()
    {
        FindMainPlayer();
    }

    void FindMainPlayer()
    {
        PlayerStats[] all = Object.FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
        foreach (var s in all) if (s.isPlayer) { player = s; break; }
    }

    void Update()
    {
        if (player == null) { FindMainPlayer(); return; }

        float dist = Vector2.Distance(transform.position, player.transform.position);

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
#else
        if (Input.GetKeyDown(KeyCode.E))
#endif
        {
            if (dist <= interactRange)
            {
                shopOpen = !shopOpen;
            }
            else if (shopOpen)
            {
                shopOpen = false;
            }
        }

        // Tự đóng khi đi xa
        if (dist > interactRange + 1f) shopOpen = false;
    }

    void OnGUI()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.transform.position);

        // Hiển thị nhãn [E] khi đứng gần
        if (dist <= interactRange && !shopOpen)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                Vector3 screenPos = cam.WorldToScreenPoint(transform.position);
                if (screenPos.z > 0)
                {
                    GUI.color = Color.yellow;
                    GUIStyle hint = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
                    GUI.Label(new Rect(screenPos.x - 80, Screen.height - screenPos.y - 60, 160, 30), "[E] Nói chuyện", hint);
                }
            }
        }

        if (!shopOpen) return;

        DrawShopUI();
    }

    void DrawShopUI()
    {
        // Nền mờ
        GUI.color = new Color(0, 0, 0, 0.5f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);

        // Cửa sổ shop
        float w = 700, h = 520;
        float sx = Screen.width / 2f - w / 2f;
        float sy = Screen.height / 2f - h / 2f;

        GUI.color = new Color(0.07f, 0.07f, 0.12f, 0.98f);
        GUI.DrawTexture(new Rect(sx, sy, w, h), Texture2D.whiteTexture);

        // Viền vàng
        GUI.color = new Color(0.8f, 0.6f, 0.1f);
        GUI.DrawTexture(new Rect(sx,         sy,          w, 3), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(sx,         sy + h - 3,  w, 3), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(sx,         sy,          3, h), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(sx + w - 3, sy,          3, h), Texture2D.whiteTexture);

        // Tiêu đề
        GUIStyle title = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        GUI.color = new Color(1f, 0.85f, 0.2f);
        GUI.Label(new Rect(sx, sy + 8, w, 35), "🏪 " + npcName, title);

        // Nút đóng
        GUI.color = new Color(0.7f, 0.1f, 0.1f);
        if (GUI.Button(new Rect(sx + w - 42, sy + 10, 30, 28), "✕")) shopOpen = false;

        // Vàng người chơi
        GUIStyle goldS = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold };
        GUI.color = new Color(1f, 0.85f, 0f);
        GUI.Label(new Rect(sx + 15, sy + 10, 250, 30), "💰 Vàng của bạn: " + player.gold, goldS);

        // Tab Mua / Bán
        GUI.color = sellTab == 0 ? new Color(0.3f, 0.6f, 0.2f) : new Color(0.2f, 0.2f, 0.3f);
        if (GUI.Button(new Rect(sx + 15, sy + 50, 150, 32), "🛒 MUA HÀNG")) sellTab = 0;
        GUI.color = sellTab == 1 ? new Color(0.7f, 0.3f, 0.1f) : new Color(0.2f, 0.2f, 0.3f);
        if (GUI.Button(new Rect(sx + 175, sy + 50, 150, 32), "💸 BÁN ĐỒ")) sellTab = 1;

        if (sellTab == 0) DrawBuyTab(sx, sy, w, h);
        else DrawSellTab(sx, sy, w, h);
    }

    // ===== TAB MUA =====
    void DrawBuyTab(float sx, float sy, float w, float h)
    {
        // Header cột
        GUI.color = new Color(0.6f, 0.6f, 0.6f);
        GUIStyle col = new GUIStyle(GUI.skin.label) { fontSize = 11, fontStyle = FontStyle.Bold };
        GUI.Label(new Rect(sx + 15, sy + 88, 300, 20), "Tên hàng", col);
        GUI.Label(new Rect(sx + 430, sy + 88, 100, 20), "Giá", col);
        GUI.Label(new Rect(sx + 540, sy + 88, 120, 20), "Hành động", col);

        Rect scrollArea = new Rect(sx + 10, sy + 108, w - 20, h - 165);
        Rect contentR   = new Rect(0, 0, w - 40, shopInventory.Count * 55);
        scrollBuy = GUI.BeginScrollView(scrollArea, scrollBuy, contentR);

        for (int i = 0; i < shopInventory.Count; i++)
        {
            var item = shopInventory[i];
            float rowY = i * 55;

            // Nền hàng
            GUI.color = (i % 2 == 0) ? new Color(0.12f, 0.12f, 0.18f) : new Color(0.08f, 0.08f, 0.13f);
            GUI.DrawTexture(new Rect(0, rowY, w - 20, 50), Texture2D.whiteTexture);

            // Tên + mô tả
            GUI.color = Color.white;
            GUIStyle nameS = new GUIStyle(GUI.skin.label) { fontSize = 13, fontStyle = FontStyle.Bold };
            GUI.Label(new Rect(10, rowY + 5, 380, 22), item.name, nameS);
            GUI.color = new Color(0.7f, 0.7f, 0.7f);
            GUI.Label(new Rect(10, rowY + 28, 380, 18), item.description, new GUIStyle(GUI.skin.label) { fontSize = 10 });

            // Giá
            bool canAfford = player.gold >= item.price;
            GUI.color = canAfford ? new Color(1f, 0.85f, 0f) : new Color(0.6f, 0.3f, 0.3f);
            GUI.Label(new Rect(430, rowY + 15, 100, 25), item.price + " 💰");

            // Nút MUA
            if (canAfford)
            {
                GUI.color = new Color(0.2f, 0.6f, 0.2f);
                if (GUI.Button(new Rect(540, rowY + 10, 100, 30), "🛒 MUA"))
                {
                    BuyItem(item);
                }
            }
            else
            {
                GUI.color = new Color(0.4f, 0.2f, 0.2f);
                GUI.Box(new Rect(540, rowY + 10, 100, 30), "Thiếu vàng");
            }
        }
        GUI.EndScrollView();

        // Gợi ý
        GUI.color = new Color(0.5f, 0.5f, 0.5f);
        GUI.Label(new Rect(sx + 15, sy + h - 50, w - 20, 40),
            "💡 Bình Máu có thể dùng từ Túi Đồ (phím B → chọn bình → Dùng Ngay)   |   Ngọc: đặt tên đồ có chứa 'Ngọc Đỏ/Xanh/Tím' để nhận bonus.",
            new GUIStyle(GUI.skin.label) { fontSize = 10, wordWrap = true });
    }

    // ===== TAB BÁN =====
    void DrawSellTab(float sx, float sy, float w, float h)
    {
        GUI.color = new Color(0.6f, 0.6f, 0.6f);
        GUIStyle col = new GUIStyle(GUI.skin.label) { fontSize = 11, fontStyle = FontStyle.Bold };
        GUI.Label(new Rect(sx + 15, sy + 88, 350, 20), "Đồ trong túi", col);
        GUI.Label(new Rect(sx + 430, sy + 88, 100, 20), "Giá thu", col);
        GUI.Label(new Rect(sx + 540, sy + 88, 120, 20), "Hành động", col);

        if (player.inventory.Count == 0)
        {
            GUI.color = Color.gray;
            GUI.Label(new Rect(sx + 15, sy + 130, w, 40), "Túi đồ trống! Đi săn quái để kiếm đồ nhé 🗡", new GUIStyle(GUI.skin.label) { fontSize = 14 });
            return;
        }

        Rect scrollArea = new Rect(sx + 10, sy + 108, w - 20, h - 165);
        Rect contentR   = new Rect(0, 0, w - 40, player.inventory.Count * 55);
        scrollSell = GUI.BeginScrollView(scrollArea, scrollSell, contentR);

        for (int i = player.inventory.Count - 1; i >= 0; i--)
        {
            string itemName = player.inventory[i];
            int sellPrice = Mathf.Max(1, player.GetItemPrice(itemName) / 2); // Bán được 50% giá mua
            float rowY = (player.inventory.Count - 1 - i) * 55;

            // Nền
            GUI.color = new Color(0.12f, 0.08f, 0.08f);
            GUI.DrawTexture(new Rect(0, rowY, w - 20, 50), Texture2D.whiteTexture);

            // Tên đồ
            GUI.color = Color.white;
            GUIStyle nameS = new GUIStyle(GUI.skin.label) { fontSize = 13 };
            GUI.Label(new Rect(10, rowY + 15, 400, 22), itemName, nameS);

            // Giá thu
            GUI.color = new Color(1f, 0.7f, 0.2f);
            GUI.Label(new Rect(430, rowY + 15, 100, 25), "+" + sellPrice + " 💰");

            // Nút BÁN
            GUI.color = new Color(0.7f, 0.2f, 0.1f);
            if (GUI.Button(new Rect(540, rowY + 10, 100, 30), "💸 BÁN"))
            {
                player.gold += sellPrice;
                player.inventory.RemoveAt(i);
                if (GameUI.instance != null)
                    GameUI.instance.ShowDamage(player.transform.position, "+" + sellPrice + "G", new Color(1f, 0.85f, 0f));
            }
        }
        GUI.EndScrollView();

        // Nút bán tất cả
        GUI.color = new Color(0.8f, 0.1f, 0.1f);
        if (player.inventory.Count > 0 && GUI.Button(new Rect(sx + w - 180, sy + h - 50, 165, 35), "💸 BÁN TẤT CẢ"))
        {
            int total = 0;
            foreach (string it in player.inventory)
                total += Mathf.Max(1, player.GetItemPrice(it) / 2);
            player.gold += total;
            player.inventory.Clear();
            if (GameUI.instance != null)
                GameUI.instance.ShowDamage(player.transform.position, "+" + total + "G", new Color(1f, 0.85f, 0f));
        }
    }

    // ===== XỬ LÝ MUA HÀNG =====
    void BuyItem(ShopItem item)
    {
        if (player.gold < item.price) return;
        player.gold -= item.price;

        // Bình máu — dùng luôn nếu HP thiếu, ngược lại bỏ túi
        if (item.name.Contains("Bình Máu") && player.currentHealth < player.maxHealth)
        {
            player.inventory.Add(item.name);
        }
        else
        {
            player.inventory.Add(item.name);
        }

        if (GameUI.instance != null)
            GameUI.instance.ShowDamage(player.transform.position, "Mua: " + item.name, Color.cyan);

        Debug.Log($"🛒 Mua: {item.name} (-{item.price} Vàng). Còn lại: {player.gold} Vàng");
    }

    // Vẽ vòng tròn tầm tương tác trong Scene View
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
