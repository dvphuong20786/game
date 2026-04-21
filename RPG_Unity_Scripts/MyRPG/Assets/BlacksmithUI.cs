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

        // Dark fantasy shadow overlay
        GUI.color = new Color(0, 0, 0, 0.4f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);

        float panelW = 420f, panelH = 580f;
        float gap = 40f;
        float startX = Screen.width / 2f - panelW - gap / 2f;
        float startY = Screen.height / 2f - panelH / 2f;

        Rect leftRect = new Rect(startX, startY, panelW, panelH);
        Rect rightRect = new Rect(startX + panelW + gap, startY, panelW, panelH);

        // Vẽ Khung Trái (Danh sách / Túi đồ)
        DrawDarkFantasyPanel(leftRect);
        
        // Vẽ Khung Phải (Lò rèn / Thông tin)
        DrawDarkFantasyPanel(rightRect);
        
        // Nút Đóng đè lên góc phải của rightRect
        GUI.color = new Color(0.8f, 0.2f, 0.2f);
        if (GUI.Button(new Rect(rightRect.xMax - 36, rightRect.y + 8, 28, 24), "X")) Close();

        // Tiêu đề
        GUI.color = new Color(0.8f, 0.7f, 0.4f); // Vàng cổ
        GUI.Label(new Rect(leftRect.x, leftRect.y + 10, leftRect.width, 30), "DANH SÁCH", new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });
        GUI.Label(new Rect(rightRect.x, rightRect.y + 10, rightRect.width, 30), "THỢ RÈN BA ĐÔ", new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });

        // TABS nằm ở Right Rect
        float tabY = rightRect.y + 50;
        float tabW = rightRect.width / 3f - 4f;
        DrawTab(rightRect.x + 4,              tabY, tabW, "MUA TRANG BỊ", 0);
        DrawTab(rightRect.x + 4 + tabW,       tabY, tabW, "BÁN VẬT PHẨM", 1);
        DrawTab(rightRect.x + 4 + tabW * 2,   tabY, tabW, "CƯỜNG HÓA",    2);
        DrawTab(rightRect.x + 4,              tabY + 35, tabW, "KHẢM NGỌC",    3);
        DrawTab(rightRect.x + 4 + tabW,       tabY + 35, tabW, "CHẾ TẠO",      4);
        DrawTab(rightRect.x + 4 + tabW * 2,   tabY + 35, tabW, "GHÉP ĐỒ",      5);

        GUI.color = Color.white;
        float contentY = tabY + 75f; // Chừa chỗ cho 2 hàng tab

        switch (bsTab)
        {
            case 0: DrawBuyTabDF(leftRect, rightRect, contentY); break;
            case 1: DrawSellTabDF(leftRect, rightRect, contentY); break;
            case 2: DrawEnhanceTabDF(leftRect, rightRect, contentY); break;
            case 3: DrawSocketTabDF(leftRect, rightRect, contentY); break;
            case 4: DrawCraftTabDF(leftRect, rightRect, contentY); break;
            case 5: DrawMergeTabDF(leftRect, rightRect, contentY); break;
        }
    }

    void DrawDarkFantasyPanel(Rect r) {
        // Nền tối xù xì
        GUI.color = new Color(0.08f, 0.08f, 0.08f, 0.98f);
        GUI.DrawTexture(r, Texture2D.whiteTexture);
        // Viền rỉ sét
        GUI.color = new Color(0.25f, 0.2f, 0.15f);
        GUI.DrawTexture(new Rect(r.x, r.y, r.width, 4), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(r.x, r.yMax-4, r.width, 4), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(r.x, r.y, 4, r.height), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(r.xMax-4, r.y, 4, r.height), Texture2D.whiteTexture);
    }

    void DrawTab(float x, float y, float w, string label, int idx)
    {
        GUI.color = (bsTab == idx) ? new Color(0.5f, 0.4f, 0.2f) : new Color(0.15f, 0.12f, 0.1f);
        if (GUI.Button(new Rect(x, y, w, 30), label, new GUIStyle(GUI.skin.button) { fontSize = 11, fontStyle = FontStyle.Bold }))
        {
            bsTab = idx;
            ResetSlots();
            sellSelectedIdx = -1;
            previewItem = null;
        }
    }

    void DrawBuyTabDF(Rect leftRect, Rect rightRect, float contentY)
    {
        // TRÁI: Dành cho Danh Sách Mua
        float slotSize = 88f, padding = 8f;
        int cols = Mathf.FloorToInt((leftRect.width - 20) / (slotSize + padding));
        int rows = Mathf.CeilToInt((float)shopStock.Count / cols);
        
        Rect scrollArea = new Rect(leftRect.x + 10, leftRect.y + 50, leftRect.width - 20, leftRect.height - 60);
        Rect content = new Rect(0, 0, cols * (slotSize + padding), rows * (slotSize + padding + 20));
        
        scrollBuy = GUI.BeginScrollView(scrollArea, scrollBuy, content);
        for (int i = 0; i < shopStock.Count; i++)
        {
            var item = shopStock[i];
            int col = i % cols, row = i / cols;
            float sx = col * (slotSize + padding), sy = row * (slotSize + padding + 20);
            bool selected = (previewItem == item);

            GUI.color = selected ? new Color(1f, 0.85f, 0.2f) : new Color(0.15f, 0.12f, 0.1f);
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

        // PHẢI: Thông tin và Mua
        if (previewItem != null)
        {
            float py = contentY + 20;
            float previewW = rightRect.width - 40;
            float pvX = rightRect.x + 20;

            GUI.color = Color.white;
            if (previewItem.icon != null) GUI.DrawTexture(new Rect(rightRect.x + rightRect.width / 2 - 40, py, 80, 80), previewItem.icon.texture);
            py += 90;

            GUIStyle bold = new GUIStyle(GUI.skin.label) { fontSize = 15, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, wordWrap = true };
            GUI.color = Color.yellow;
            bool isEq = previewItem.type == ItemData.ItemType.Weapon || previewItem.type == ItemData.ItemType.Armor || previewItem.type == ItemData.ItemType.Accessory;
            string shopNamePrev = isEq ? "[F] " + previewItem.itemName : previewItem.itemName;
            GUI.Label(new Rect(pvX, py, previewW, 30), shopNamePrev, bold); py += 35;

            GUIStyle sm = new GUIStyle(GUI.skin.label) { fontSize = 12, wordWrap = true, alignment = TextAnchor.UpperCenter };
            GUI.color = Color.gray;
            GUI.Label(new Rect(pvX, py, previewW, 60), previewItem.description, sm); py += 70;

            GUI.color = Color.white;
            sm.alignment = TextAnchor.MiddleLeft;
            if (previewItem.atkBonus > 0) GUI.Label(new Rect(pvX + 20, py, previewW, 20), "Tấn Công: +" + previewItem.atkBonus, sm); py += 22;
            if (previewItem.defBonus > 0) GUI.Label(new Rect(pvX + 20, py, previewW, 20), "Phòng Thủ: +" + previewItem.defBonus, sm); py += 22;
            if (previewItem.hpBonus > 0) GUI.Label(new Rect(pvX + 20, py, previewW, 20), "Sinh Mệnh: +" + previewItem.hpBonus, sm); py += 30;

            GUI.color = new Color(0.8f, 0.7f, 0.4f);
            GUI.Label(new Rect(pvX, py, previewW, 25), "Giá: " + previewItem.price + " Vàng", new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter }); py += 40;

            bool canAfford = player.gold >= previewItem.price;
            GUI.color = canAfford ? new Color(0.2f, 0.6f, 0.2f) : Color.grey;
            if (GUI.Button(new Rect(pvX + 20, py, previewW - 40, 45), canAfford ? "MUA NGAY" : "KHÔNG ĐỦ VÀNG", new GUIStyle(GUI.skin.button) { fontSize = 14, fontStyle = FontStyle.Bold }))
            {
                player.gold -= previewItem.price;
                player.PickUpItem(previewItem);
            }
        }
    }

    void DrawSellTabDF(Rect leftRect, Rect rightRect, float contentY)
    {
        List<ItemInstance> inv = player.SharedInventory;
        
        // Nút Multi-Sell ở TRÁI
        GUI.color = isMultiSellMode ? Color.green : Color.white;
        if (GUI.Button(new Rect(leftRect.x + leftRect.width - 110, leftRect.y + 12, 100, 24), isMultiSellMode ? "BÁN NHIỀU: BẬT" : "BÁN NHIỀU: TẮT")) {
            isMultiSellMode = !isMultiSellMode;
            multiSellSelectedIdxs.Clear();
        }

        // TÚI ĐỒ Ở TRÁI
        float slotSize = 68f, padding = 6f;
        int cols = Mathf.FloorToInt((leftRect.width - 20) / (slotSize + padding));
        int rows = Mathf.CeilToInt((float)inv.Count / cols);
        
        Rect scrollArea = new Rect(leftRect.x + 10, leftRect.y + 50, leftRect.width - 20, leftRect.height - 60);
        scrollInv = GUI.BeginScrollView(scrollArea, scrollInv, new Rect(0, 0, cols * (slotSize + padding), rows * (slotSize + padding + 20)));
        for (int i = 0; i < inv.Count; i++)
        {
            var inst = inv[i]; if (inst == null || inst.data == null) continue;
            int col = i % cols, row = i / cols;
            float sx = col * (slotSize + padding), sy = row * (slotSize + padding + 20);
            
            bool isSel = isMultiSellMode ? multiSellSelectedIdxs.Contains(i) : (sellSelectedIdx == i);
            GUI.color = isSel ? Color.green : new Color(0.15f, 0.12f, 0.1f);
            GUI.DrawTexture(new Rect(sx, sy, slotSize, slotSize + 20), Texture2D.whiteTexture);
            
            if (inst.data.icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(sx + 3, sy + 3, slotSize - 6, slotSize - 6), inst.data.icon.texture); }
            GUI.color = Color.white;
            GUI.Label(new Rect(sx, sy + slotSize, slotSize, 20), inst.GetDisplayName(), new GUIStyle(GUI.skin.label) { fontSize = 8, alignment = TextAnchor.MiddleCenter, wordWrap = true });
            
            GUI.color = Color.clear;
            if (GUI.Button(new Rect(sx, sy, slotSize, slotSize + 20), "")) {
                if (isMultiSellMode) { if (multiSellSelectedIdxs.Contains(i)) multiSellSelectedIdxs.Remove(i); else multiSellSelectedIdxs.Add(i); }
                else { sellSelectedIdx = i; }
            }
        }
        GUI.EndScrollView();

        // THÔNG TIN BÁN Ở PHẢI
        float pvX = rightRect.x + 20;
        float previewW = rightRect.width - 40;

        if (isMultiSellMode)
        {
            float py = contentY + 100;
            int totalGold = 0;
            foreach (int i in multiSellSelectedIdxs) if (i < inv.Count && inv[i] != null && inv[i].data != null) totalGold += inv[i].data.price / 2;
            
            GUI.color = Color.white;
            GUI.Label(new Rect(pvX, py, previewW, 30), "ĐÃ CHỌN " + multiSellSelectedIdxs.Count + " MÓN", new GUIStyle(GUI.skin.label) { fontSize = 16, alignment = TextAnchor.MiddleCenter }); py+=40;
            
            GUI.color = new Color(0.8f, 0.7f, 0.4f);
            GUI.Label(new Rect(pvX, py, previewW, 30), "TỔNG VÀNG: " + totalGold, new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle=FontStyle.Bold, alignment = TextAnchor.MiddleCenter }); py+=60;
            
            GUI.color = multiSellSelectedIdxs.Count > 0 ? Color.green : Color.gray;
            if (GUI.Button(new Rect(pvX+10, py, previewW-20, 50), "XÁC NHẬN BÁN HẾT", new GUIStyle(GUI.skin.button){fontSize=14, fontStyle=FontStyle.Bold}) && multiSellSelectedIdxs.Count > 0) {
                player.gold += totalGold;
                var sortedIdxs = new List<int>(multiSellSelectedIdxs);
                sortedIdxs.Sort((a,b) => b.CompareTo(a)); // remove from last to first
                foreach (int idx in sortedIdxs) {
                    if (idx < player.SharedInventory.Count)
                        player.SharedInventory.RemoveAt(idx);
                }
                multiSellSelectedIdxs.Clear();
            }
        }
        else if (sellSelectedIdx >= 0 && sellSelectedIdx < inv.Count)
        {
            var inst = inv[sellSelectedIdx];
            if (inst == null || inst.data == null) return;
            
            float py = contentY + 20;
            GUI.color = Color.white;
            if (inst.data.icon != null) GUI.DrawTexture(new Rect(rightRect.x + rightRect.width/2 - 40, py, 80, 80), inst.data.icon.texture);
            py += 90;

            GUI.color = inst.GetRankColor();
            GUI.Label(new Rect(pvX, py, previewW, 30), inst.GetDisplayName(), new GUIStyle(GUI.skin.label) { fontSize = 15, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter }); py += 40;

            GUI.color = Color.white;
            GUIStyle smLeft = new GUIStyle(GUI.skin.label) { fontSize = 12, alignment = TextAnchor.MiddleLeft };
            int tatk = inst.GetTotalAtk(), tdef = inst.GetTotalDef(), thp = inst.GetTotalHP();
            if (tatk > 0) GUI.Label(new Rect(pvX + 20, py, previewW, 20), "Tấn Công: " + tatk, smLeft); py += 20;
            if (tdef > 0) GUI.Label(new Rect(pvX + 20, py, previewW, 20), "Phòng Thủ: " + tdef, smLeft); py += 20;
            if (thp > 0) GUI.Label(new Rect(pvX + 20, py, previewW, 20), "Sinh Mệnh: " + thp, smLeft); py += 30;

            int price = inst.data.price / 2;
            GUI.color = new Color(0.8f, 0.7f, 0.4f);
            GUI.Label(new Rect(pvX, py, previewW, 25), "Thu về: " + price + " Vàng", new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter }); py += 40;

            GUI.color = new Color(0.8f, 0.2f, 0.2f);
            if (GUI.Button(new Rect(pvX + 20, py, previewW - 40, 45), "BÁN", new GUIStyle(GUI.skin.button){fontSize=14, fontStyle=FontStyle.Bold})) {
                player.gold += price;
                player.SharedInventory.RemoveAt(sellSelectedIdx);
                sellSelectedIdx = -1;
            }
        }
    }

    void DrawEnhanceTabDF(Rect leftRect, Rect rightRect, float contentY)
    {
        // TRÁI: Dùng DrawFilteredInvSlotsInRect nhưng sửa tọa độ lại khớp
        Rect listArea = new Rect(leftRect.x + 10, leftRect.y + 50, leftRect.width - 20, leftRect.height - 60);
        DrawFilteredInvSlotsInRect(listArea, ref scrollInv, (i) => i.data.type == ItemData.ItemType.Weapon || i.data.type == ItemData.ItemType.Armor || i.data.type == ItemData.ItemType.Accessory, true, false, false);

        // PHẢI: Lò rèn nâng cấp
        float pvX = rightRect.x + 20;
        float rightW = rightRect.width - 40;
        
        GUI.color = Color.white;
        DrawBigSlot(rightRect.x + rightRect.width/2 - 40, contentY + 20, 80f, targetItem, "Trang bị", true);
        
        if (targetItem != null && targetItem.data != null) {
            float py = contentY + 130;
            int cost = (targetItem.plusLevel + 1) * 200;
            bool canAfford = player.gold >= cost;

            GUI.color = Color.white;
            GUI.Label(new Rect(pvX, py, rightW, 30), targetItem.GetDisplayName() + "  ->  +" + (targetItem.plusLevel + 1), new GUIStyle(GUI.skin.label){fontSize=14, fontStyle=FontStyle.Bold, alignment=TextAnchor.MiddleCenter}); py += 40;
            
            GUI.color = new Color(0.8f, 0.7f, 0.4f);
            GUI.Label(new Rect(pvX, py, rightW, 25), "Phí: " + cost + " Vàng", new GUIStyle(GUI.skin.label){fontSize=12, alignment=TextAnchor.MiddleCenter}); py += 40;

            GUI.color = canAfford ? Color.green : Color.gray;
            if (GUI.Button(new Rect(pvX + 20, py, rightW - 40, 45), canAfford ? "NÂNG CẤP" : "KHÔNG ĐỦ VÀNG", new GUIStyle(GUI.skin.button){fontSize=14, fontStyle=FontStyle.Bold}) && canAfford) {
                Execute();
            }
        }
    }

    void DrawSocketTabDF(Rect leftRect, Rect rightRect, float contentY)
    {
        Rect listArea = new Rect(leftRect.x + 10, leftRect.y + 50, leftRect.width - 20, leftRect.height - 60);
        DrawFilteredInvSlotsInRect(listArea, ref scrollInv, (i) => true, true, true, true); // CHẾ TẠO DÙNG CẢ 2
        
        // PHẢI
        float pvX = rightRect.x + 20;
        float rightW = rightRect.width - 40;

        float slotDim = 80f;
        DrawBigSlot(rightRect.x + rightRect.width/2 - slotDim - 10, contentY + 20, slotDim, targetItem, "Trang bị", true);
        DrawBigSlot(rightRect.x + rightRect.width/2 + 10, contentY + 20, slotDim, materialItem, "Ngọc Khảm", false);

        if (targetItem != null && materialItem != null) {
            float py = contentY + 130;
            bool isEquip = targetItem.data.type == ItemData.ItemType.Weapon || targetItem.data.type == ItemData.ItemType.Armor || targetItem.data.type == ItemData.ItemType.Accessory;
            bool isGem = materialItem.data.type == ItemData.ItemType.Gem;
            bool space = targetItem.sockets.Count < 5;
            int cost = 500;
            bool afford = player.gold >= cost;
            bool isValid = isEquip && isGem && space && afford;

            if (!space) GUI.Label(new Rect(pvX, py, rightW, 30), "Đã khảm tối đa ngọc!", new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter});
            else if (!isEquip || !isGem) GUI.Label(new Rect(pvX, py, rightW, 30), "Cần Trang bị và Ngọc!", new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter});
            else {
                GUI.color = new Color(0.8f, 0.7f, 0.4f);
                GUI.Label(new Rect(pvX, py, rightW, 25), "Phí: " + cost + " Vàng", new GUIStyle(GUI.skin.label){fontSize=13, alignment=TextAnchor.MiddleCenter}); py += 40;
                
                GUI.color = isValid ? Color.green : Color.gray;
                if (GUI.Button(new Rect(pvX + 20, py, rightW - 40, 45), isValid ? "KHẢM NGỌC" : "KHÔNG ĐỦ VÀNG", new GUIStyle(GUI.skin.button){fontSize=14, fontStyle=FontStyle.Bold}) && isValid) {
                    Execute();
                }
            }
        }
    }

    void DrawCraftTabDF(Rect leftRect, Rect rightRect, float contentY)
    {
        Rect listArea = new Rect(leftRect.x + 10, leftRect.y + 50, leftRect.width - 20, leftRect.height - 60);
        DrawFilteredInvSlotsInRect(listArea, ref scrollInv, (i) => i.data.type == ItemData.ItemType.Quest || i.data.itemName.Contains("Mảnh") || i.itemRank < 6, false, true, false);

        float pvX = rightRect.x + 20;
        float rightW = rightRect.width - 40;

        DrawBigSlot(rightRect.x + rightRect.width/2 - 40, contentY + 20, 80f, materialItem, "Nguyên liệu", false);

        if (materialItem != null && materialItem.data != null) {
            float py = contentY + 130;
            int cost = 1000;
            bool afford = player.gold >= cost;

            GUI.color = new Color(0.8f, 0.7f, 0.4f);
            GUI.Label(new Rect(pvX, py, rightW, 25), "Chế tạo mất: " + cost + " Vàng", new GUIStyle(GUI.skin.label){fontSize=13, alignment=TextAnchor.MiddleCenter}); py += 40;

            GUI.color = afford ? new Color(1f, 0.5f, 0f) : Color.gray;
            if (GUI.Button(new Rect(pvX + 20, py, rightW - 40, 45), afford ? "CHẾ TẠO ĐỒ MỚI" : "KHÔNG ĐỦ VÀNG", new GUIStyle(GUI.skin.button){fontSize=14, fontStyle=FontStyle.Bold}) && afford) {
                Execute();
            }
        }
    }

    void DrawMergeTabDF(Rect leftRect, Rect rightRect, float contentY)
    {
        Rect listArea = new Rect(leftRect.x + 10, leftRect.y + 50, leftRect.width - 20, leftRect.height - 60);
        DrawFilteredInvSlotsInRect(listArea, ref scrollInv, (i) => i.itemRank < 6 && i.data.type != ItemData.ItemType.Gem && i.data.type != ItemData.ItemType.Consumable, false, false, true);

        float pvX = rightRect.x + 20;
        float rightW = rightRect.width - 40;
        float slotDim = 80f;

        DrawBigSlot(rightRect.x + rightRect.width/2 - slotDim - 10, contentY + 20, slotDim, targetItem, "Vật phẩm 1", true);
        DrawBigSlot(rightRect.x + rightRect.width/2 + 10, contentY + 20, slotDim, materialItem, "Vật phẩm 2", false);

        if (targetItem != null && materialItem != null) {
            float py = contentY + 130;
            bool matches = (targetItem.data.itemName == materialItem.data.itemName) && (targetItem.itemRank == materialItem.itemRank);
            int cost = (targetItem.itemRank + 1) * 1000;
            bool afford = player.gold >= cost;

            if (matches) {
                string nextRank = "F";
                string[] ranks = {"F", "E", "D", "C", "B", "A", "S"};
                if (targetItem.itemRank + 1 < ranks.Length) nextRank = ranks[targetItem.itemRank + 1];

                GUI.color = new Color(0.8f, 0.7f, 0.4f);
                GUI.Label(new Rect(pvX, py, rightW, 25), "Lên hạng: [" + nextRank + "]", new GUIStyle(GUI.skin.label){fontSize=14, fontStyle=FontStyle.Bold, alignment=TextAnchor.MiddleCenter}); py += 30;
                GUI.Label(new Rect(pvX, py, rightW, 25), "Phí: " + cost + " Vàng", new GUIStyle(GUI.skin.label){fontSize=12, alignment=TextAnchor.MiddleCenter}); py += 40;

                GUI.color = afford ? Color.red : Color.gray;
                if (GUI.Button(new Rect(pvX + 20, py, rightW - 40, 45), afford ? "GHÉP NÂNG HẠNG" : "KHÔNG ĐỦ VÀNG", new GUIStyle(GUI.skin.button){fontSize=14, fontStyle=FontStyle.Bold}) && afford) {
                    Execute();
                }
            } else {
                GUI.color = Color.white;
                GUI.Label(new Rect(pvX, py, rightW, 60), "KHÔNG HỢP LỆ!\nPhải giống hệt tên và Hạng.", new GUIStyle(GUI.skin.label){fontSize=12, alignment=TextAnchor.MiddleCenter}); 
            }
        }
    }

    void DrawBigSlot(float x, float y, float dim, ItemInstance inst, string title, bool isTarget) {
        GUI.color = new Color(0.15f, 0.12f, 0.1f);
        GUI.DrawTexture(new Rect(x, y, dim, dim), Texture2D.whiteTexture);
        GUI.color = Color.gray;
        GUI.Label(new Rect(x - 20, y - 20, dim + 40, 20), title, new GUIStyle(GUI.skin.label){fontSize=10, alignment=TextAnchor.MiddleCenter});
        if (inst != null) {
            Sprite s = inst.GetIcon();
            if (s != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(x+5, y+5, dim-10, dim-10), s.texture); }
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
            Sprite sIcon = inst.GetIcon();
            if (sIcon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(sx+3, sy+3, slotSz-6, slotSz-6), sIcon.texture); }
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
            Sprite scIcon = inst.GetIcon();
            if (scIcon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(sx+3, sy+3, slotSz-6, slotSz-6), scIcon.texture); }
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