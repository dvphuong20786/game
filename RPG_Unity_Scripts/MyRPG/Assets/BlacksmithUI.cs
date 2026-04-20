using UnityEngine;
using System.Collections.Generic;

// ===========================
// BLACKSMITH UI - PHIÊN BẢN THỚT RÈN CAO CẤP
// 5 Tab: Mua / Bán / Cường Hóa / Khảm Ngọc / Chế Tạo / Ghép Đồ
// ===========================
public class BlacksmithUI : MonoBehaviour
{
    public static BlacksmithUI instance;
    public bool isOpen = false;

    public ItemInstance targetItem;
    public ItemInstance materialItem;
    public int targetIdx = -1;
    public int materialIdx = -1;
    public bool isMultiSellMode = false;
    public HashSet<int> multiSellSelectedIdxs = new HashSet<int>();

    // 0=Mua, 1=Ban, 2=CuongHoa, 3=KhamNgoc, 4=CheTao, 5=GhepDo
    private int bsTab = 0;
    private Vector2 scrollBuy, scrollSell, scrollInv;

    private List<ItemData> shopStock = new List<ItemData>();
    private PlayerStats player => GameUI.instance != null ? GameUI.instance.player : null;

    // Tab Ban: item dang chon de xem truoc khi ban
    private int sellSelectedIdx = -1;

    // Tab Mua: item dang chon de xem truoc khi mua
    private ItemData previewItem = null;

    void Awake() { instance = this; }

    public void Open(List<ItemData> stock = null)
    {
        isOpen = true;
        ResetSlots();
        sellSelectedIdx = -1;
        previewItem = null;
        if (stock != null) shopStock = stock;
        else
        {
            shopStock.Clear();
            ItemData[] all = Resources.LoadAll<ItemData>("Items");
            foreach (var item in all)
                if (item.type == ItemData.ItemType.Weapon ||
                    item.type == ItemData.ItemType.Armor ||
                    item.type == ItemData.ItemType.Accessory)
                    shopStock.Add(item);
        }
    }

    public void Close() { isOpen = false; }

    public void ResetSlots()
    {
        targetItem = null; materialItem = null;
        targetIdx = -1; materialIdx = -1;
        isMultiSellMode = false;
        multiSellSelectedIdxs.Clear();
    }

    public void SetTarget(int invIdx)
    {
        if (player == null) return;
        var inv = player.SharedInventory;
        if (invIdx >= 0 && invIdx < inv.Count) { targetItem = inv[invIdx]; targetIdx = invIdx; }
    }

    public void SetMaterial(int invIdx)
    {
        if (player == null) return;
        var inv = player.SharedInventory;
        if (invIdx >= 0 && invIdx < inv.Count) { materialItem = inv[invIdx]; materialIdx = invIdx; }
    }

    // ============================================================
    //  LOGIC THỰC HIỆN
    // ============================================================
    public void Execute()
    {
        if (targetItem == null || player == null) return;

        ItemData craftResult = RPG_BlacksmithLogic.CheckRecipe(targetItem, materialItem);
        if (craftResult != null)
        {
            player.SharedInventory.Remove(targetItem);
            player.SharedInventory.Remove(materialItem);
            player.PickUpItem(craftResult);
            GameUI.instance.ShowDamage(player.transform.position, "THÀNH CÔNG: " + craftResult.itemName, Color.yellow);
            ResetSlots(); return;
        }

        if (bsTab == 3 && targetItem != null && materialItem != null && materialItem.data != null && materialItem.data.type == ItemData.ItemType.Gem) {
            RPG_BlacksmithLogic.SocketItem(player, targetItem, materialItem);
            materialItem = null;
            materialIdx = -1;
            return;
        }
        else if (bsTab == 2)
            RPG_BlacksmithLogic.EnhanceItem(player, targetIdx);
        else if (bsTab == 5)
            RPG_BlacksmithLogic.MergeItem(player, targetIdx, materialIdx);
        ResetSlots();
    }

    // ============================================================
    //  VẼ CỬA SỔ CHÍNH
    // ============================================================
    public void DrawWindow()
    {
        if (!isOpen || player == null) return;

        GUI.color = new Color(0, 0, 0, 0.72f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);

        float w = 740f, h = 540f;
        Rect r = new Rect(Screen.width / 2f - w / 2f, Screen.height / 2f - h / 2f, w, h);

        GUI.color = new Color(0.09f, 0.07f, 0.06f, 0.99f);
        GUI.DrawTexture(r, Texture2D.whiteTexture);
        GUI.color = new Color(0.8f, 0.6f, 0.1f);
        GUI.DrawTexture(new Rect(r.x, r.y, w, 2), Texture2D.whiteTexture);

        GUI.color = Color.yellow;
        GUI.Label(new Rect(r.x, r.y + 6, w, 28), "THỢ RÈN BA ĐÔ",
            new GUIStyle(GUI.skin.label) { fontSize = 17, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });

        GUI.color = Color.red;
        if (GUI.Button(new Rect(r.xMax - 36, r.y + 8, 28, 24), "X")) Close();

        // 5 TABS
        float tabY = r.y + 36;
        float tabW = 120f;
        DrawTab(r.x + 5,          tabY, tabW, "MUA TRANG BỊ", 0, new Color(0.2f, 0.5f, 1f));
        DrawTab(r.x + 5 + tabW,   tabY, tabW, "BÁN VẬT PHẨM", 1, new Color(1f, 0.6f, 0.1f));
        DrawTab(r.x + 5 + tabW*2, tabY, tabW, "CƯỜNG HÓA",    2, new Color(0.2f, 0.8f, 0.3f));
        DrawTab(r.x + 5 + tabW*3, tabY, tabW, "KHẢM NGỌC",    3, new Color(0.8f, 0.2f, 0.9f));
        DrawTab(r.x + 5 + tabW*4, tabY, tabW, "CHẾ TẠO",      4, new Color(0.9f, 0.4f, 0.1f));
        DrawTab(r.x + 5 + tabW*5, tabY, tabW, "GHÉP ĐỒ",      5, new Color(1f, 0.2f, 0.2f));

        GUI.color = Color.white;
        float contentY = tabY + 32f;

        switch (bsTab)
        {
            case 0: DrawBuyTab(r, contentY); break;
            case 1: DrawSellTab(r, contentY); break;
            case 2: DrawEnhanceTab(r, contentY); break;
            case 3: DrawSocketTab(r, contentY); break;
            case 4: DrawCraftTab(r, contentY); break;
            case 5: DrawMergeTab(r, contentY); break;
        }
    }

    void DrawTab(float x, float y, float w, string label, int idx, Color activeCol)
    {
        GUI.color = (bsTab == idx) ? activeCol : new Color(0.25f, 0.22f, 0.18f);
        if (GUI.Button(new Rect(x, y, w - 2, 30), label,
            new GUIStyle(GUI.skin.button) { fontSize = 10, fontStyle = FontStyle.Bold }))
        {
            bsTab = idx;
            ResetSlots();
            sellSelectedIdx = -1;
            previewItem = null;
        }
    }

    void DrawBuyTab(Rect r, float contentY)
    {
        float previewW = 200f;
        float listW = r.width - previewW - 20f;
        int cols = 4;
        float slotSize = 88f, padding = 8f;

        Rect scrollArea = new Rect(r.x + 5, contentY, listW, r.height - (contentY - r.y) - 10);
        int rows = Mathf.CeilToInt((float)shopStock.Count / cols);
        Rect content = new Rect(0, 0, cols * (slotSize + padding), rows * (slotSize + padding + 20));
        scrollBuy = GUI.BeginScrollView(scrollArea, scrollBuy, content);

        for (int i = 0; i < shopStock.Count; i++)
        {
            var item = shopStock[i];
            int col = i % cols, row = i / cols;
            float sx = col * (slotSize + padding), sy = row * (slotSize + padding + 20);
            bool selected = (previewItem == item);

            GUI.color = selected ? new Color(1f, 0.85f, 0.2f) : new Color(0.2f, 0.18f, 0.14f);
            GUI.DrawTexture(new Rect(sx, sy, slotSize, slotSize + 20), Texture2D.whiteTexture);

            if (item.icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(sx + 4, sy + 4, slotSize - 8, slotSize - 8), item.icon.texture); }
            else { GUI.color = GetTypeColor(item.type); GUI.DrawTexture(new Rect(sx + 4, sy + 4, slotSize - 8, slotSize - 8), Texture2D.whiteTexture); GUI.color = Color.white; GUI.Label(new Rect(sx + 4, sy + 20, slotSize - 8, 36), GetTypeIcon(item.type), new GUIStyle(GUI.skin.label) { fontSize = 26, alignment = TextAnchor.MiddleCenter }); }

            GUI.color = Color.white;
            bool isEquip = item.type == ItemData.ItemType.Weapon || item.type == ItemData.ItemType.Armor || item.type == ItemData.ItemType.Accessory;
            string shopName = isEquip ? "[F] " + item.itemName : item.itemName;
            GUI.Label(new Rect(sx, sy + slotSize, slotSize, 20), shopName, new GUIStyle(GUI.skin.label) { fontSize = 8, alignment = TextAnchor.MiddleCenter, wordWrap = true });
            GUI.color = Color.clear;
            if (GUI.Button(new Rect(sx, sy, slotSize, slotSize + 20), "")) previewItem = item;
        }
        GUI.EndScrollView();

        // PREVIEW PANEL
        Rect pv = new Rect(r.xMax - previewW - 5, contentY, previewW, r.height - (contentY - r.y) - 10);
        GUI.color = new Color(0.12f, 0.1f, 0.16f);
        GUI.DrawTexture(pv, Texture2D.whiteTexture);

        if (previewItem != null)
        {
            float py = pv.y + 10;
            GUI.color = Color.white;
            if (previewItem.icon != null) GUI.DrawTexture(new Rect(pv.x + previewW / 2 - 40, py, 80, 80), previewItem.icon.texture);
            py += 90;

            GUIStyle bold = new GUIStyle(GUI.skin.label) { fontSize = 13, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, wordWrap = true };
            GUI.color = Color.yellow;
            bool isEquip = previewItem.type == ItemData.ItemType.Weapon || previewItem.type == ItemData.ItemType.Armor || previewItem.type == ItemData.ItemType.Accessory;
            string shopNamePrev = isEquip ? "[F] " + previewItem.itemName : previewItem.itemName;
            GUI.Label(new Rect(pv.x + 5, py, previewW - 10, 30), shopNamePrev, bold); py += 35;

            GUIStyle sm = new GUIStyle(GUI.skin.label) { fontSize = 10, wordWrap = true };
            GUI.color = Color.gray;
            GUI.Label(new Rect(pv.x + 10, py, previewW - 20, 60), previewItem.description, sm); py += 70;

            GUI.color = Color.white;
            if (previewItem.atkBonus > 0) GUI.Label(new Rect(pv.x + 15, py, previewW - 30, 20), "Tấn Công: +" + previewItem.atkBonus, sm); py += 20;
            if (previewItem.defBonus > 0) GUI.Label(new Rect(pv.x + 15, py, previewW - 30, 20), "Phòng Thủ: +" + previewItem.defBonus, sm); py += 20;
            if (previewItem.hpBonus > 0) GUI.Label(new Rect(pv.x + 15, py, previewW - 30, 20), "Sinh Mệnh: +" + previewItem.hpBonus, sm); py += 25;

            GUI.color = Color.yellow;
            GUI.Label(new Rect(pv.x, py, previewW, 25), "Giá: " + previewItem.price + " Vàng", bold); py += 35;

            bool canAfford = player.gold >= previewItem.price;
            GUI.color = canAfford ? Color.green : Color.gray;
            if (GUI.Button(new Rect(pv.x + 15, py, previewW - 30, 40), canAfford ? "MUA NGAY" : "KHÔNG ĐỦ VÀNG",
                new GUIStyle(GUI.skin.button) { fontSize = 12, fontStyle = FontStyle.Bold }))
            {
                player.gold -= previewItem.price;
                player.PickUpItem(previewItem);
            }
        }
    }

    void DrawSellTab(Rect r, float contentY)
    {
        List<ItemInstance> inv = player.SharedInventory;
        float previewW = 200f;
        float listW = r.width - previewW - 20f;
        int cols = 4;
        float slotSize = 80f, padding = 6f;

        // MULTI-SELL TOGGLE
        GUI.color = isMultiSellMode ? Color.green : Color.white;
        if (GUI.Button(new Rect(r.x + 5, contentY, 150, 22), isMultiSellMode ? "✔ BÁN NHIỀU (BẬT)" : "✖ BÁN NHIỀU (TẮT)")) {
            isMultiSellMode = !isMultiSellMode;
            multiSellSelectedIdxs.Clear();
            sellSelectedIdx = -1;
        }
        GUI.color = Color.white;

        Rect scrollArea = new Rect(r.x + 5, contentY + 25, listW, r.height - (contentY - r.y) - 35);
        int rows = Mathf.CeilToInt((float)inv.Count / cols);
        Rect content = new Rect(0, 0, cols * (slotSize + padding), Mathf.Max(200, rows * (slotSize + padding + 18)));
        scrollSell = GUI.BeginScrollView(scrollArea, scrollSell, content);

        for (int i = 0; i < inv.Count; i++)
        {
            var inst = inv[i];
            if (inst == null || inst.data == null) continue;
            int col = i % cols, row = i / cols;
            float sx = col * (slotSize + padding), sy = row * (slotSize + padding + 18);

            bool selected = isMultiSellMode ? multiSellSelectedIdxs.Contains(i) : (sellSelectedIdx == i);
            GUI.color = selected ? (isMultiSellMode ? Color.green : new Color(1f, 0.6f, 0.1f)) : new Color(0.2f, 0.15f, 0.12f);
            GUI.DrawTexture(new Rect(sx, sy, slotSize, slotSize + 18), Texture2D.whiteTexture);

            if (inst.data.icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(sx + 4, sy + 4, slotSize - 8, slotSize - 8), inst.data.icon.texture); }
            else { GUI.color = GetTypeColor(inst.data.type); GUI.DrawTexture(new Rect(sx + 4, sy + 4, slotSize - 8, slotSize - 8), Texture2D.whiteTexture); GUI.color = Color.white; GUI.Label(new Rect(sx + 4, sy + 18, slotSize - 8, 36), GetTypeIcon(inst.data.type), new GUIStyle(GUI.skin.label) { fontSize = 22, alignment = TextAnchor.MiddleCenter }); }
            
            for (int k=0; k<inst.sockets.Count && k<5; k++) {
                float gx = sx + slotSize - 10; float gy = sy + 3 + (k * 7f);
                if (inst.sockets[k].icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(gx, gy, 7, 7), inst.sockets[k].icon.texture); }
                else { GUI.color = Color.magenta; GUI.DrawTexture(new Rect(gx, gy, 7, 7), Texture2D.whiteTexture); }
            }

            GUI.color = selected ? Color.yellow : Color.gray;
            GUI.Label(new Rect(sx, sy + slotSize, slotSize, 18), inst.GetDisplayName(), new GUIStyle(GUI.skin.label) { fontSize = 8, alignment = TextAnchor.MiddleCenter, wordWrap = true });

            GUI.color = Color.clear;
            if (GUI.Button(new Rect(sx, sy, slotSize, slotSize + 18), ""))
            {
                if (isMultiSellMode) {
                    if (multiSellSelectedIdxs.Contains(i)) multiSellSelectedIdxs.Remove(i);
                    else multiSellSelectedIdxs.Add(i);
                } else {
                    sellSelectedIdx = i;
                    targetItem = inst;
                }
            }
        }
        GUI.EndScrollView();

        // PREVIEW
        Rect pv = new Rect(r.xMax - previewW - 5, contentY, previewW, r.height - (contentY - r.y) - 10);
        GUI.color = new Color(0.12f, 0.12f, 0.12f);
        GUI.DrawTexture(pv, Texture2D.whiteTexture);
        GUI.color = Color.white;

        GUIStyle bold = new GUIStyle(GUI.skin.label) { fontSize = 12, fontStyle = FontStyle.Bold, wordWrap = true, alignment = TextAnchor.MiddleCenter };
        GUIStyle sm = new GUIStyle(GUI.skin.label) { fontSize = 10, wordWrap = true };

        if (isMultiSellMode) {
            float py = pv.y + 10;
            GUI.color = Color.green;
            GUI.Label(new Rect(pv.x + 5, py, previewW - 10, 30), "CHẾ ĐỘ BÁN NHIỀU", bold); py += 40;
            GUI.color = Color.white;
            GUI.Label(new Rect(pv.x + 10, py, previewW - 20, 100), "Đã chọn: " + multiSellSelectedIdxs.Count + " món đồ.\nHãy chọn đồ trong danh sách và nhấn nút bên dưới để bán.", sm);
            
            int totalValue = 0;
            foreach(int idx in multiSellSelectedIdxs) if (idx < inv.Count) totalValue += Mathf.Max(1, inv[idx].data.price / 2);
            
            float btnY = pv.yMax - 60;
            GUI.color = Color.yellow;
            GUI.Label(new Rect(pv.x, btnY - 30, previewW, 25), "Tổng cộng: +" + totalValue + " Vàng", bold);
            
            GUI.color = (multiSellSelectedIdxs.Count > 0) ? Color.red : Color.gray;
            if (GUI.Button(new Rect(pv.x + 10, btnY, previewW - 20, 40), "XÁC NHẬN BÁN HẾT") && multiSellSelectedIdxs.Count > 0) {
                player.gold += totalValue;
                List<int> sortedIdx = new List<int>(multiSellSelectedIdxs);
                sortedIdx.Sort((a,b) => b.CompareTo(a)); 
                foreach(int idx in sortedIdx) inv.RemoveAt(idx);
                multiSellSelectedIdxs.Clear();
            }
        }
        else if (sellSelectedIdx >= 0 && sellSelectedIdx < inv.Count)
        {
            ItemInstance inst = inv[sellSelectedIdx];
            ItemData d = inst.data;
            float py = pv.y + 10;
            if (d.icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(pv.x + previewW / 2 - 40, py, 80, 80), d.icon.texture); }
            py += 90f;

            GUI.color = Color.yellow; GUI.Label(new Rect(pv.x + 4, py, previewW - 8, 36), inst.GetDisplayName(), bold); py += 36f;
            GUI.color = Color.gray; GUI.Label(new Rect(pv.x + 4, py, previewW - 8, 40), d.description, sm); py += 42f;
            
            if (inst.GetBaseAtk() > 0) { GUI.color = Color.red; GUI.Label(new Rect(pv.x + 8, py, previewW - 12, 18), "Tấn Công: " + inst.GetBaseAtk(), sm); py += 18f; }
            if (inst.GetBaseDef() > 0) { GUI.color = Color.cyan; GUI.Label(new Rect(pv.x + 8, py, previewW - 12, 18), "Phòng Thủ: " + inst.GetBaseDef(), sm); py += 18f; }
            if (inst.GetBaseHp() > 0) { GUI.color = Color.green; GUI.Label(new Rect(pv.x + 8, py, previewW - 12, 18), "Sinh Mệnh: " + inst.GetBaseHp(), sm); py += 18f; }
            
            if (inst.itemRank > 0) {
                GUI.color = Color.gray; GUI.DrawTexture(new Rect(pv.x + 8, py + 2, previewW - 16, 1), Texture2D.whiteTexture); py += 6f;
                if (inst.rankBonusAtk > 0) { GUI.color = new Color(1f, 0.4f, 0.4f); GUI.Label(new Rect(pv.x + 20, py, previewW - 32, 18), "+ " + inst.rankBonusAtk + " Tấn Công", sm); py += 18f; }
                if (inst.rankBonusDef > 0) { GUI.color = new Color(0.4f, 0.8f, 1f); GUI.Label(new Rect(pv.x + 20, py, previewW - 32, 18), "+ " + inst.rankBonusDef + " Phòng Thủ", sm); py += 18f; }
                if (inst.rankBonusHp > 0)  { GUI.color = new Color(0.4f, 1f, 0.4f);  GUI.Label(new Rect(pv.x + 20, py, previewW - 32, 18), "+ " + inst.rankBonusHp + " Sinh Mệnh", sm); py += 18f; }
            }
            
            if (inst.sockets.Count > 0) {
                GUI.color = Color.gray; GUI.DrawTexture(new Rect(pv.x + 8, py + 2, previewW - 16, 1), Texture2D.whiteTexture); py += 6f;
                foreach (var gem in inst.sockets) {
                    if (gem == null) continue;
                    if (gem.icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(pv.x + 12, py + 2, 14, 14), gem.icon.texture); }
                    GUI.color = Color.white;
                    string stat = gem.atkBonus > 0 ? "+" + gem.atkBonus + " Tấn Công" : gem.defBonus > 0 ? "+" + gem.defBonus + " Phòng Thủ" : "+" + gem.hpBonus + " Sinh Mệnh";
                    GUI.Label(new Rect(pv.x + 30, py, previewW - 40, 18), stat, new GUIStyle(GUI.skin.label){fontSize=9});
                    py += 18f;
                }
            }

            int sellPrice = Mathf.Max(1, d.price / 2);
            GUI.color = new Color(1f, 0.75f, 0.2f);
            GUI.Label(new Rect(pv.x + 4, py, previewW - 8, 22), "Giá: +" + sellPrice + " Vàng", bold); py += 28f;

            GUI.color = Color.red;
            if (GUI.Button(new Rect(pv.x + 10, py, previewW - 20, 36), "BÁN",
                new GUIStyle(GUI.skin.button) { fontSize = 13, fontStyle = FontStyle.Bold }))
            {
                player.gold += sellPrice;
                inv.RemoveAt(sellSelectedIdx);
                sellSelectedIdx = -1;
            }
        }
    }

    void DrawEnhanceTab(Rect r, float contentY)
    {
        float listW = r.width - 20;
        float slotAreaH = r.height - (contentY - r.y) - 60;
        DrawFilteredInvSlots(new Rect(r.x + 5, contentY, listW, slotAreaH), contentY, (inst) => inst.data.type != ItemData.ItemType.Consumable && inst.data.type != ItemData.ItemType.Gem, false);

        if (targetItem != null)
        {
            int cost = (targetItem.plusLevel + 1) * 200;
            float rate = Mathf.Max(10f, (0.9f - targetItem.plusLevel * 0.15f) * 100f);
            GUI.color = Color.yellow;
            GUI.Label(new Rect(r.x, r.yMax - 45, r.width, 22), "Cường Hóa [" + targetItem.GetDisplayName() + "] | Tỉ lệ: " + (int)rate + "% | Phí: " + cost + " Vàng", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });

            bool can = player.gold >= cost;
            GUI.color = can ? Color.green : Color.gray;
            if (GUI.Button(new Rect(r.x + r.width / 2 - 80, r.yMax - 22, 160, 20), can ? "CƯỜNG HÓA" : "KHÔNG ĐỦ VÀNG")) Execute();
        }
    }

        void DrawSocketTab(Rect r, float contentY)
    {
        float leftW = 340f;
        float rightW = r.width - leftW - 20f;
        GUI.color = new Color(0.12f, 0.1f, 0.08f);
        GUI.DrawTexture(new Rect(r.x + 5, contentY, leftW, 22), Texture2D.whiteTexture);
        GUI.color = new Color(0.8f, 0.2f, 0.9f);
        GUI.Label(new Rect(r.x + 8, contentY + 2, leftW - 4, 18), "Kho Đồ (Trang Bị & Ngọc)", new GUIStyle(GUI.skin.label) { fontSize = 10, fontStyle = FontStyle.Bold });

        float slotAreaH = r.height - (contentY - r.y) - 30;
        DrawFilteredInvSlotsInRect(new Rect(r.x + 5, contentY + 26, leftW, slotAreaH), ref scrollInv, (i) => i.data.type != ItemData.ItemType.Consumable, false, false, true);

        float rx = r.x + 15 + leftW;
        GUI.color = new Color(0.08f, 0.06f, 0.05f);
        GUI.DrawTexture(new Rect(rx, contentY, rightW, slotAreaH + 26), Texture2D.whiteTexture);
        GUI.color = Color.yellow;
        GUI.Label(new Rect(rx, contentY + 10, rightW, 24), "LÒ RÈN KHẢM NGỌC", new GUIStyle(GUI.skin.label){fontSize = 14, fontStyle=FontStyle.Bold, alignment=TextAnchor.MiddleCenter});

        float slotDim = 80f;
        DrawBigSlot(rx + 60f, contentY + 60f, slotDim, (targetItem != null && targetItem.data.type != ItemData.ItemType.Gem) ? targetItem : null, "Trang Bị", true);
        DrawBigSlot(rx + rightW - 60f - slotDim, contentY + 60f, slotDim, (materialItem != null && materialItem.data.type == ItemData.ItemType.Gem) ? materialItem : null, "Ngọc", false);

        GUI.color = Color.white;
        GUI.Label(new Rect(rx + rightW/2 - 20, contentY + 80, 40, 40), "+", new GUIStyle(GUI.skin.label){fontSize=30, alignment=TextAnchor.MiddleCenter});

        float rxInfo = rx + 10; float ryInfo = contentY + 160f;
        if (targetItem != null && targetItem.data.type != ItemData.ItemType.Gem) {
            GUIStyle sm = new GUIStyle(GUI.skin.label) { fontSize = 10, wordWrap = true };
            GUI.color = targetItem.GetRankColor();
            GUI.Label(new Rect(rxInfo, ryInfo, rightW - 20, 20), targetItem.GetDisplayName(), new GUIStyle(GUI.skin.label){fontStyle=FontStyle.Bold, fontSize=11});
            ryInfo += 22f;
            GUI.color = Color.white;
            GUI.Label(new Rect(rxInfo, ryInfo, rightW - 20, 18), "Tấn Công: " + targetItem.GetBaseAtk() + " | Phòng Thủ: " + targetItem.GetBaseDef(), sm);
            ryInfo += 20f;
            if (targetItem.sockets.Count > 0) {
                // Header row removed
                ryInfo += 5f;
                foreach (var gem in targetItem.sockets) {
                    if (gem == null) continue;
                    if (gem.icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(rxInfo + 5, ryInfo + 2, 12, 12), gem.icon.texture); }
                    GUI.color = Color.white;
                    string stat = gem.atkBonus > 0 ? "+" + gem.atkBonus + " Tấn Công" : gem.defBonus > 0 ? "+" + gem.defBonus + " Phòng Thủ" : "+" + gem.hpBonus + " Sinh Mệnh";
                    GUI.Label(new Rect(rxInfo + 20, ryInfo, rightW - 40, 18), stat, new GUIStyle(GUI.skin.label){fontSize=9});
                    ryInfo += 16f;
                }
            }
        }

        int socketCost = 500;
        bool canSocket = targetItem != null && targetItem.data.type != ItemData.ItemType.Gem && materialItem != null && materialItem.data.type == ItemData.ItemType.Gem && player.gold >= socketCost && targetItem.sockets.Count < 5;
        
        float btnY = r.yMax - 60;
        GUI.color = Color.yellow;
        GUI.Label(new Rect(rx, btnY - 25, rightW, 22), "Phí: " + player.gold + " / " + socketCost, new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter, fontSize=11, fontStyle=FontStyle.Bold});
        
        GUI.color = canSocket ? Color.magenta : Color.gray;
        if (GUI.Button(new Rect(rx + 50, btnY, rightW - 100, 40), canSocket ? "KHẢM NGỌC" : "CHƯA ĐỦ ĐIỀU KIỆN", new GUIStyle(GUI.skin.button){fontSize=12, fontStyle=FontStyle.Bold}) && canSocket)
            Execute();
    }

    void DrawCraftTab(Rect r, float contentY)
    {
        float leftW = 340f;
        float rightW = r.width - leftW - 20f;
        GUI.color = new Color(0.12f, 0.1f, 0.08f);
        GUI.DrawTexture(new Rect(r.x + 5, contentY, leftW, 22), Texture2D.whiteTexture);
        GUI.color = Color.orange;
        GUI.Label(new Rect(r.x + 8, contentY + 2, leftW - 4, 18), "Kho Đồ (Chọn Nguyên Liệu)", new GUIStyle(GUI.skin.label) { fontSize = 10, fontStyle = FontStyle.Bold });

        float slotAreaH = r.height - (contentY - r.y) - 30;
        DrawFilteredInvSlotsInRect(new Rect(r.x + 5, contentY + 26, leftW, slotAreaH), ref scrollInv, (_) => true, false, false, true);

        float rx = r.x + 15 + leftW;
        GUI.color = new Color(0.08f, 0.06f, 0.05f);
        GUI.DrawTexture(new Rect(rx, contentY, rightW, slotAreaH + 26), Texture2D.whiteTexture);
        GUI.color = Color.yellow;
        GUI.Label(new Rect(rx, contentY + 10, rightW, 24), "LÒ RÈN CHẾ TẠO", new GUIStyle(GUI.skin.label){fontSize = 14, fontStyle=FontStyle.Bold, alignment=TextAnchor.MiddleCenter});

        float slotDim = 80f;
        DrawBigSlot(rx + 60f, contentY + 60f, slotDim, targetItem, "Nguyên Liệu 1", true);
        DrawBigSlot(rx + rightW - 60f - slotDim, contentY + 60f, slotDim, materialItem, "Nguyên Liệu 2", false);

        GUI.color = Color.white;
        GUI.Label(new Rect(rx + rightW/2 - 20, contentY + 80, 40, 40), "➡", new GUIStyle(GUI.skin.label){fontSize=30, alignment=TextAnchor.MiddleCenter});

        float rxC = rx + rightW/2 - slotDim/2; float ryC = contentY + 160f;
        ItemData recipe = (targetItem!=null && materialItem!=null) ? RPG_BlacksmithLogic.CheckRecipe(targetItem, materialItem) : null;
        GUI.color = new Color(0.15f, 0.2f, 0.15f);
        GUI.DrawTexture(new Rect(rxC, ryC, slotDim, slotDim), Texture2D.whiteTexture);

        if (recipe != null) {
            if (recipe.icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(rxC+5, ryC+5, slotDim-10, slotDim-10), recipe.icon.texture); }
            GUI.color = Color.green;
            GUI.Label(new Rect(rxC - 40, ryC + slotDim + 5, slotDim + 80, 20), recipe.itemName, new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter, fontStyle=FontStyle.Bold, fontSize=11});
            GUI.color = Color.yellow;
            if (GUI.Button(new Rect(rx + 50, ryC + slotDim + 40, rightW - 100, 40), "CHẾ TẠO NGAY", new GUIStyle(GUI.skin.button){fontSize=14, fontStyle=FontStyle.Bold})) Execute();
        }
    }

    void DrawMergeTab(Rect r, float contentY)
    {
        float leftW = 340f;
        float rightW = r.width - leftW - 20f;

        GUI.color = new Color(0.12f, 0.1f, 0.08f);
        GUI.DrawTexture(new Rect(r.x + 5, contentY, leftW, 22), Texture2D.whiteTexture);
        GUI.color = new Color(1f, 0.4f, 0.4f);
        GUI.Label(new Rect(r.x + 8, contentY + 2, leftW - 4, 18), "Kho Đồ (Ghép Đồ)", new GUIStyle(GUI.skin.label) { fontSize = 10, fontStyle = FontStyle.Bold });

        float slotAreaH = r.height - (contentY - r.y) - 30;
        DrawFilteredInvSlotsInRect(new Rect(r.x + 5, contentY + 26, leftW, slotAreaH), ref scrollInv, (i) => i.itemRank < 6 && i.data.type != ItemData.ItemType.Gem && i.data.type != ItemData.ItemType.Consumable, false, false, true);

        float rx = r.x + 15 + leftW;
        GUI.color = new Color(0.08f, 0.06f, 0.05f);
        GUI.DrawTexture(new Rect(rx, contentY, rightW, slotAreaH + 26), Texture2D.whiteTexture);
        
        GUI.color = Color.yellow;
        GUI.Label(new Rect(rx, contentY + 10, rightW, 24), "LÒ RÈN NÂNG HẠNG", new GUIStyle(GUI.skin.label){fontSize = 14, fontStyle=FontStyle.Bold, alignment=TextAnchor.MiddleCenter});

        float slotDim = 80f;
        DrawBigSlot(rx + 60f, contentY + 60f, slotDim, targetItem, "Vật Phẩm", true);
        DrawBigSlot(rx + rightW - 60f - slotDim, contentY + 60f, slotDim, materialItem, "Phôi (Giống hệt)", false);

        GUI.color = Color.white;
        GUI.Label(new Rect(rx + rightW/2 - 20, contentY + 80, 40, 40), "+", new GUIStyle(GUI.skin.label){fontSize=30, alignment=TextAnchor.MiddleCenter});

        if (targetItem != null && materialItem != null) {
            bool matches = targetItem.data.itemName == materialItem.data.itemName && targetItem.itemRank == materialItem.itemRank;
            int mergeCost = (targetItem.itemRank + 1) * 1000;
            bool enoughGold = player.gold >= mergeCost;
            bool canMerge = matches && enoughGold;

            float infoY = contentY + 160f;
            if (matches) {
                string nextRank = "F";
                string[] ranks = {"F", "E", "D", "C", "B", "A", "S"};
                if (targetItem.itemRank + 1 < ranks.Length) nextRank = ranks[targetItem.itemRank + 1];

                GUI.color = Color.yellow;
                GUI.Label(new Rect(rx, infoY, rightW, 22), "Lên Hạng: [" + nextRank + "]", new GUIStyle(GUI.skin.label) { fontSize = 14, alignment = TextAnchor.MiddleCenter, fontStyle=FontStyle.Bold });
                GUI.Label(new Rect(rx, infoY + 25, rightW, 22), "Phí: " + player.gold + " / " + mergeCost, new GUIStyle(GUI.skin.label) { fontSize = 11, alignment = TextAnchor.MiddleCenter, fontStyle=FontStyle.Bold });
                
                GUI.color = canMerge ? Color.red : Color.gray;
                if (GUI.Button(new Rect(rx + 50, infoY + 60, rightW - 100, 40), canMerge ? "GHÉP NÂNG HẠNG" : (enoughGold ? "NGUYÊN LIỆU SAI" : "KHÔNG ĐỦ VÀNG"), new GUIStyle(GUI.skin.button) { fontSize = 12, fontStyle = FontStyle.Bold }) && canMerge)
                    Execute();
            } else {
                GUI.color = Color.gray;
                GUI.Label(new Rect(rx, infoY, rightW, 60), "❌ Sai nguyên liệu!\nPhải giống hệt tên và Hạng mục.", new GUIStyle(GUI.skin.label) { fontSize = 12, alignment = TextAnchor.MiddleCenter });
            }
        }
        GUI.color = Color.white;
    }

    void DrawBigSlot(float x, float y, float dim, ItemInstance inst, string title, bool isTarget) {
        GUI.color = new Color(0.15f, 0.12f, 0.1f);
        GUI.DrawTexture(new Rect(x, y, dim, dim), Texture2D.whiteTexture);
        GUI.color = Color.gray;
        GUI.Label(new Rect(x - 20, y - 20, dim + 40, 20), title, new GUIStyle(GUI.skin.label){fontSize=10, alignment=TextAnchor.MiddleCenter});
        if (inst != null) {
            if (inst.data.icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(x+5, y+5, dim-10, dim-10), inst.data.icon.texture); }
            GUI.color = Color.red;
            if (GUI.Button(new Rect(x+dim-20, y, 20, 20), "X")) { if (isTarget) { targetItem = null; targetIdx = -1; } else { materialItem = null; materialIdx = -1; } }
        }
    }

    delegate bool ItemFilter(ItemInstance inst);
    void DrawFilteredInvSlots(Rect r, float startY, ItemFilter filter, bool forMaterial) {
        List<ItemInstance> inv = player.SharedInventory;
        int cols = 7; float slotSz = 72f, pad = 5f;
        Rect scrollArea = new Rect(r.x + 5, startY, r.width - 10, r.height - 10);
        var filtered = new List<(ItemInstance inst, int idx)>();
        for (int i = 0; i < inv.Count; i++) if (filter(inv[i])) filtered.Add((inv[i], i));
        scrollInv = GUI.BeginScrollView(scrollArea, scrollInv, new Rect(0,0, cols*(slotSz+pad), Mathf.CeilToInt((float)filtered.Count/cols)*(slotSz+pad)));
        DrawSlotsCore(filtered, slotSz, pad, cols, forMaterial);
        GUI.EndScrollView();
    }

    void DrawFilteredInvSlotsInRect(Rect area, ref Vector2 scroll, ItemFilter filter, bool asTarget, bool asMaterial, bool forCraft) {
        List<ItemInstance> inv = player.SharedInventory;
        float slotSz = 68f, pad = 4f; int cols = Mathf.FloorToInt(area.width / (slotSz + pad));
        var filtered = new List<(ItemInstance inst, int idx)>();
        for (int i = 0; i < inv.Count; i++) if (filter(inv[i])) filtered.Add((inv[i], i));
        scroll = GUI.BeginScrollView(area, scroll, new Rect(0,0, area.width-20, Mathf.CeilToInt((float)filtered.Count/cols)*(slotSz+pad)));
        for (int vi = 0; vi < filtered.Count; vi++) {
            var (inst, realIdx) = filtered[vi];
            int col = vi % cols, row = vi / cols;
            float sx = col * (slotSz + pad), sy = row * (slotSz + pad);
            bool isT = (targetIdx == realIdx), isM = (materialIdx == realIdx);
            GUI.color = isT ? Color.orange : isM ? Color.magenta : new Color(0.2f, 0.18f, 0.14f);
            GUI.DrawTexture(new Rect(sx, sy, slotSz, slotSz), Texture2D.whiteTexture);
            if (inst.data.icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(sx+3, sy+3, slotSz-6, slotSz-6), inst.data.icon.texture); }
            GUI.color = Color.clear;
            if (GUI.Button(new Rect(sx, sy, slotSz, slotSz), "")) {
                if (forCraft) {
                    if (isT) { targetItem = null; targetIdx = -1; }
                    else if (isM) { materialItem = null; materialIdx = -1; }
                    else if (targetItem == null) { targetItem = inst; targetIdx = realIdx; }
                    else { materialItem = inst; materialIdx = realIdx; }
                } else if (asTarget) { targetItem = inst; targetIdx = realIdx; }
                else if (asMaterial) { materialItem = inst; materialIdx = realIdx; }
            }
        }
        GUI.EndScrollView();
    }

    void DrawSlotsCore(List<(ItemInstance inst, int idx)> filtered, float slotSz, float pad, int cols, bool forMaterial) {
        for (int vi = 0; vi < filtered.Count; vi++) {
            var (inst, realIdx) = filtered[vi];
            int col = vi % cols, row = vi / cols;
            float sx = col * (slotSz + pad), sy = row * (slotSz + pad);
            bool isT = (targetIdx == realIdx), isM = (materialIdx == realIdx);
            GUI.color = isT ? Color.orange : isM ? Color.magenta : new Color(0.2f, 0.18f, 0.14f);
            GUI.DrawTexture(new Rect(sx, sy, slotSz, slotSz), Texture2D.whiteTexture);
            if (inst.data.icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(sx+3, sy+3, slotSz-6, slotSz-6), inst.data.icon.texture); }
            GUI.color = Color.clear;
            if (GUI.Button(new Rect(sx, sy, slotSz, slotSz), "")) {
                if (!forMaterial) { targetItem = inst; targetIdx = realIdx; }
                else { materialItem = inst; materialIdx = realIdx; }
            }
        }
    }

    Color GetTypeColor(ItemData.ItemType t) {
        switch (t) {
            case ItemData.ItemType.Weapon: return new Color(0.8f, 0.2f, 0.2f, 0.8f);
            case ItemData.ItemType.Armor: return new Color(0.2f, 0.4f, 0.8f, 0.8f);
            default: return new Color(0.5f, 0.5f, 0.5f, 0.8f);
        }
    }

    string GetTypeIcon(ItemData.ItemType t) {
        switch (t) {
            case ItemData.ItemType.Weapon: return "W";
            case ItemData.ItemType.Armor: return "A";
            default: return "?";
        }
    }
}