using UnityEngine;
using System.Collections.Generic;

// ===========================
// BLACKSMITH UI - PHIEN BAN THONG MINH
// 5 Tab: Mua / Ban (chon truoc ban sau) / Cuong Hoa (loc item) / Kham Ngoc / Che Tao
// ===========================
public class BlacksmithUI : MonoBehaviour
{
    public static BlacksmithUI instance;
    public bool isOpen = false;

    public ItemInstance targetItem;
    public ItemInstance materialItem;
    public int targetIdx = -1;
    public int materialIdx = -1;

    // 0=Mua, 1=Ban, 2=CuongHoa, 3=KhamNgoc, 4=CheTao
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
    //  LOGIC THUC HIEN
    // ============================================================
    public void Execute()
    {
        if (targetItem == null || player == null) return;

        ItemData craftResult = RPG_BlacksmithLogic.CheckRecipe(targetItem, materialItem);
        if (craftResult != null)
        {
            player.SharedInventory.RemoveAt(targetIdx);
            int mIdx = player.SharedInventory.IndexOf(materialItem);
            if (mIdx >= 0) player.SharedInventory.RemoveAt(mIdx);
            player.PickUpItem(craftResult);
            GameUI.instance.ShowDamage(player.transform.position, "THANH CONG: " + craftResult.itemName, Color.yellow);
            ResetSlots(); return;
        }

        if (bsTab == 3 && materialItem != null && materialItem.data != null && materialItem.data.type == ItemData.ItemType.Gem)
            RPG_BlacksmithLogic.SocketByInventoryIndex(player, targetIdx, materialIdx);
        else if (bsTab == 2)
            RPG_BlacksmithLogic.EnhanceItem(player, targetIdx);
        else if (bsTab == 5)
            RPG_BlacksmithLogic.MergeItem(player, targetIdx, materialIdx);
        ResetSlots();
    }

    // ============================================================
    //  VE CUNG SO CHINH
    // ============================================================
    public void DrawWindow()
    {
        if (player == null) return;

        GUI.color = new Color(0, 0, 0, 0.72f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);

        float w = 740f, h = 540f;
        Rect r = new Rect(Screen.width / 2f - w / 2f, Screen.height / 2f - h / 2f, w, h);

        GUI.color = new Color(0.09f, 0.07f, 0.06f, 0.99f);
        GUI.DrawTexture(r, Texture2D.whiteTexture);
        GUI.color = new Color(0.8f, 0.6f, 0.1f);
        GUI.DrawTexture(new Rect(r.x, r.y, w, 2), Texture2D.whiteTexture);

        GUI.color = Color.yellow;
        GUI.Label(new Rect(r.x, r.y + 6, w, 28), "THO REN BA DO",
            new GUIStyle(GUI.skin.label) { fontSize = 17, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });

        GUI.color = Color.red;
        if (GUI.Button(new Rect(r.xMax - 36, r.y + 8, 28, 24), "X")) Close();

        // 5 TABS
        float tabY = r.y + 36;
        float tabW = 120f;
        DrawTab(r.x + 5,          tabY, tabW, "MUA TRANG BI", 0, new Color(0.2f, 0.5f, 1f));
        DrawTab(r.x + 5 + tabW,   tabY, tabW, "BAN VAT PHAM", 1, new Color(1f, 0.6f, 0.1f));
        DrawTab(r.x + 5 + tabW*2, tabY, tabW, "CUONG HOA",    2, new Color(0.2f, 0.8f, 0.3f));
        DrawTab(r.x + 5 + tabW*3, tabY, tabW, "KHAM NGOC",    3, new Color(0.8f, 0.2f, 0.9f));
        DrawTab(r.x + 5 + tabW*4, tabY, tabW, "CHE TAO",      4, new Color(0.9f, 0.4f, 0.1f));
        DrawTab(r.x + 5 + tabW*5, tabY, tabW, "GHEP DO",      5, new Color(1f, 0.2f, 0.2f));

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

    // ============================================================
    //  TAB 0: MUA - SLOT + PREVIEW
    // ============================================================
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
            GUI.Label(new Rect(sx, sy + slotSize, slotSize, 20), item.itemName, new GUIStyle(GUI.skin.label) { fontSize = 8, alignment = TextAnchor.MiddleCenter, wordWrap = true });
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
            float py = pv.y + 8f;
            if (previewItem.icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(pv.x + pv.width / 2 - 50, py, 100, 100), previewItem.icon.texture); }
            else { GUI.color = GetTypeColor(previewItem.type); GUI.DrawTexture(new Rect(pv.x + pv.width / 2 - 50, py, 100, 100), Texture2D.whiteTexture); }
            py += 108f;

            GUIStyle bold = new GUIStyle(GUI.skin.label) { fontSize = 12, fontStyle = FontStyle.Bold, wordWrap = true, alignment = TextAnchor.MiddleCenter };
            GUIStyle sm = new GUIStyle(GUI.skin.label) { fontSize = 11, wordWrap = true };

            GUI.color = Color.yellow; GUI.Label(new Rect(pv.x + 4, py, previewW - 8, 36), previewItem.itemName, bold); py += 38f;
            GUI.color = Color.gray; GUI.Label(new Rect(pv.x + 4, py, previewW - 8, 36), previewItem.description, new GUIStyle(sm) { fontSize = 10 }); py += 38f;
            if (previewItem.atkBonus > 0) { GUI.color = Color.red; GUI.Label(new Rect(pv.x + 8, py, previewW - 12, 20), "ATK: +" + previewItem.atkBonus, sm); py += 22f; }
            if (previewItem.defBonus > 0) { GUI.color = Color.cyan; GUI.Label(new Rect(pv.x + 8, py, previewW - 12, 20), "DEF: +" + previewItem.defBonus, sm); py += 22f; }
            if (previewItem.hpBonus > 0) { GUI.color = Color.green; GUI.Label(new Rect(pv.x + 8, py, previewW - 12, 20), "HP: +" + previewItem.hpBonus, sm); py += 22f; }
            GUI.color = new Color(1f, 0.75f, 0.1f); GUI.Label(new Rect(pv.x + 4, py, previewW - 8, 22), "Gia: " + previewItem.price + " Vang", bold); py += 26f;

            bool canBuy = player.gold >= previewItem.price;
            GUI.color = canBuy ? Color.green : Color.gray;
            if (GUI.Button(new Rect(pv.x + 10, py, previewW - 20, 34), canBuy ? "MUA NGAY" : "KHONG DU VANG",
                new GUIStyle(GUI.skin.button) { fontSize = 12, fontStyle = FontStyle.Bold }))
            {
                if (canBuy) { player.gold -= previewItem.price; player.SharedInventory.Add(new ItemInstance(previewItem)); }
            }
        }
        else
        {
            GUI.color = Color.gray;
            GUI.Label(new Rect(pv.x + 10, pv.y + pv.height / 2 - 20, previewW - 20, 40),
                "Chon vat pham\nde xem chi tiet",
                new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 11 });
        }
        GUI.color = Color.white;
    }

    // ============================================================
    //  TAB 1: BAN - CHON TRUOC, XEM INFO, ROI MOI BAN
    // ============================================================
    void DrawSellTab(Rect r, float contentY)
    {
        List<ItemInstance> inv = player.SharedInventory;
        float previewW = 200f;
        float listW = r.width - previewW - 20f;
        int cols = 4;
        float slotSize = 80f, padding = 6f;

        Rect scrollArea = new Rect(r.x + 5, contentY, listW, r.height - (contentY - r.y) - 10);
        int rows = Mathf.CeilToInt((float)inv.Count / cols);
        Rect content = new Rect(0, 0, cols * (slotSize + padding), Mathf.Max(200, rows * (slotSize + padding + 18)));
        scrollSell = GUI.BeginScrollView(scrollArea, scrollSell, content);

        for (int i = 0; i < inv.Count; i++)
        {
            var inst = inv[i];
            if (inst == null || inst.data == null) continue;
            int col = i % cols, row = i / cols;
            float sx = col * (slotSize + padding), sy = row * (slotSize + padding + 18);

            bool selected = (sellSelectedIdx == i);
            GUI.color = selected ? new Color(1f, 0.6f, 0.1f) : new Color(0.2f, 0.15f, 0.12f);
            GUI.DrawTexture(new Rect(sx, sy, slotSize, slotSize + 18), Texture2D.whiteTexture);

            if (inst.data.icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(sx + 4, sy + 4, slotSize - 8, slotSize - 8), inst.data.icon.texture); }
            else { GUI.color = GetTypeColor(inst.data.type); GUI.DrawTexture(new Rect(sx + 4, sy + 4, slotSize - 8, slotSize - 8), Texture2D.whiteTexture); GUI.color = Color.white; GUI.Label(new Rect(sx + 4, sy + 18, slotSize - 8, 36), GetTypeIcon(inst.data.type), new GUIStyle(GUI.skin.label) { fontSize = 22, alignment = TextAnchor.MiddleCenter }); }
            // Ve 5 ngoc mini
            for (int k=0; k<inst.sockets.Count && k<5; k++) {
                float gx = sx + slotSize - 10; float gy = sy + 3 + (k * 7f);
                if (inst.sockets[k].icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(gx, gy, 7, 7), inst.sockets[k].icon.texture); }
                else { GUI.color = Color.magenta; GUI.DrawTexture(new Rect(gx, gy, 7, 7), Texture2D.whiteTexture); }
            }


            GUI.color = selected ? Color.yellow : Color.gray;
            GUI.Label(new Rect(sx, sy + slotSize, slotSize, 18), inst.GetDisplayName(),
                new GUIStyle(GUI.skin.label) { fontSize = 8, alignment = TextAnchor.MiddleCenter, wordWrap = true });

            GUI.color = Color.clear;
            if (GUI.Button(new Rect(sx, sy, slotSize, slotSize + 18), ""))
                sellSelectedIdx = (sellSelectedIdx == i) ? -1 : i; // Toggle chon/bo chon
        }
        GUI.EndScrollView();

        // PANEL THONG TIN + NUT BAN
        Rect pv = new Rect(r.xMax - previewW - 5, contentY, previewW, r.height - (contentY - r.y) - 10);
        GUI.color = new Color(0.12f, 0.1f, 0.08f);
        GUI.DrawTexture(pv, Texture2D.whiteTexture);

        if (sellSelectedIdx >= 0 && sellSelectedIdx < inv.Count && inv[sellSelectedIdx] != null)
        {
            var inst = inv[sellSelectedIdx];
            var d = inst.data;
            float py = pv.y + 8f;

            if (d.icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(pv.x + pv.width / 2 - 45, py, 90, 90), d.icon.texture); }
            else { GUI.color = GetTypeColor(d.type); GUI.DrawTexture(new Rect(pv.x + pv.width / 2 - 45, py, 90, 90), Texture2D.whiteTexture); }
            py += 96f;

            GUIStyle bold = new GUIStyle(GUI.skin.label) { fontSize = 12, fontStyle = FontStyle.Bold, wordWrap = true, alignment = TextAnchor.MiddleCenter };
            GUIStyle sm = new GUIStyle(GUI.skin.label) { fontSize = 10, wordWrap = true };

            GUI.color = Color.yellow; GUI.Label(new Rect(pv.x + 4, py, previewW - 8, 36), inst.GetDisplayName(), bold); py += 36f;
            GUI.color = Color.gray; GUI.Label(new Rect(pv.x + 4, py, previewW - 8, 40), d.description, sm); py += 42f;
            
            // Chi so goc
            if (inst.GetBaseAtk() > 0) { GUI.color = Color.red; GUI.Label(new Rect(pv.x + 8, py, previewW - 12, 18), "T.Công: " + inst.GetBaseAtk() + " (Gốc)", sm); py += 18f; }
            if (inst.GetBaseDef() > 0) { GUI.color = Color.cyan; GUI.Label(new Rect(pv.x + 8, py, previewW - 12, 18), "P.Thủ: " + inst.GetBaseDef() + " (Gốc)", sm); py += 18f; }
            if (inst.GetBaseHp() > 0) { GUI.color = Color.green; GUI.Label(new Rect(pv.x + 8, py, previewW - 12, 18), "S.Mệnh: " + inst.GetBaseHp() + " (Gốc)", sm); py += 18f; }
            
            // Rank
            if (inst.itemRank > 0) {
                GUI.color = inst.GetRankColor();
                GUI.Label(new Rect(pv.x + 8, py, previewW - 12, 18), "Hạng [" + inst.GetRankString() + "]: +" + inst.rankBonusAtk + " / +" + inst.rankBonusDef + " / +" + inst.rankBonusHp, sm);
                py += 18f;
            }
            
            // Ngoc
            if (inst.sockets.Count > 0) {
                GUI.color = Color.magenta;
                GUI.Label(new Rect(pv.x + 8, py, previewW - 12, 18), "Ngọc: +" + inst.GetGemAtk() + " / +" + inst.GetGemDef() + " / +" + inst.GetGemHp(), sm);
                py += 18f;
            }

            int sellPrice = Mathf.Max(1, d.price / 2);
            GUI.color = new Color(1f, 0.75f, 0.2f);
            GUI.Label(new Rect(pv.x + 4, py, previewW - 8, 22), "Thu ve: +" + sellPrice + " Vang", bold); py += 28f;

            GUI.color = Color.red;
            if (GUI.Button(new Rect(pv.x + 10, py, previewW - 20, 36), "BAN NGAY",
                new GUIStyle(GUI.skin.button) { fontSize = 13, fontStyle = FontStyle.Bold }))
            {
                player.gold += sellPrice;
                inv.RemoveAt(sellSelectedIdx);
                sellSelectedIdx = -1;
            }
        }
        else
        {
            GUI.color = Color.gray;
            GUI.Label(new Rect(pv.x + 10, pv.y + pv.height / 2 - 30, previewW - 20, 60),
                "Chon vat pham\nde xem thong tin\nva ban",
                new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 11 });
        }
        GUI.color = Color.white;
    }

    // ============================================================
    //  TAB 2: CUONG HOA - Chi hien Weapon/Armor/Accessory
    // ============================================================
    void DrawEnhanceTab(Rect r, float contentY)
    {
        GUI.color = Color.white;
        GUI.Label(new Rect(r.x + 10, contentY, r.width - 20, 20),
            "Chon trang bi can cuong hoa (Vu khi / Giap / Phu kien):",
            new GUIStyle(GUI.skin.label) { fontSize = 11 });

        // Chi hien trang bi co the cuong hoa
        DrawFilteredInvSlots(r, contentY + 24, EnhanceableOnly, false);

        if (targetItem != null)
        {
            float infoY = contentY + 280;
            GUI.color = new Color(0.1f, 0.15f, 0.1f);
            GUI.DrawTexture(new Rect(r.x + 10, infoY, r.width - 20, 110), Texture2D.whiteTexture);

            int cost = (targetItem.plusLevel + 1) * 200;
            float rate = Mathf.Max(10f, 90f - targetItem.plusLevel * 15f);

            GUI.color = Color.yellow;
            GUI.Label(new Rect(r.x + 16, infoY + 6, 400, 22),
                targetItem.GetDisplayName() + "  [+" + targetItem.plusLevel + "]",
                new GUIStyle(GUI.skin.label) { fontSize = 13, fontStyle = FontStyle.Bold });
            GUI.color = Color.white;
            GUI.Label(new Rect(r.x + 16, infoY + 30, 400, 20),
                "ATK:" + targetItem.GetTotalAtk() + "  DEF:" + targetItem.GetTotalDef() + "  HP:" + targetItem.GetTotalHP());
            GUI.color = new Color(1f, 0.8f, 0.2f);
            GUI.Label(new Rect(r.x + 16, infoY + 52, 400, 20),
                "Chi phi: " + cost + " Vang   |   Ti le: " + rate.ToString("F0") + "%");

            bool canAfford = player.gold >= cost;
            GUI.color = canAfford ? Color.green : Color.gray;
            if (GUI.Button(new Rect(r.x + r.width / 2 - 110, infoY + 76, 220, 30),
                canAfford ? "CUONG HOA +" : "KHONG DU VANG",
                new GUIStyle(GUI.skin.button) { fontSize = 13, fontStyle = FontStyle.Bold }))
                Execute();
        }
        GUI.color = Color.white;
    }

    bool EnhanceableOnly(ItemInstance inst)
    {
        return inst != null && inst.data != null &&
               (inst.data.type == ItemData.ItemType.Weapon ||
                inst.data.type == ItemData.ItemType.Armor ||
                inst.data.type == ItemData.ItemType.Accessory);
    }

    // ============================================================
    //  TAB 3: KHAM NGOC - Slot 1: Trang bi, Slot 2: Chi Gem
    // ============================================================
    void DrawSocketTab(Rect r, float contentY)
    {
        float halfW = (r.width - 20) / 2f;

        // PANEL TRAI: Chon Trang bi
        GUI.color = new Color(0.12f, 0.1f, 0.08f);
        GUI.DrawTexture(new Rect(r.x + 5, contentY, halfW, 22), Texture2D.whiteTexture);
        GUI.color = Color.cyan;
        GUI.Label(new Rect(r.x + 8, contentY + 2, halfW - 4, 18),
            targetItem == null ? "[1] Chon Trang Bi" : "[1] " + targetItem.GetDisplayName(),
            new GUIStyle(GUI.skin.label) { fontSize = 10, fontStyle = FontStyle.Bold });

        // PANEL PHAI: Chon Ngoc
        GUI.color = new Color(0.12f, 0.1f, 0.08f);
        GUI.DrawTexture(new Rect(r.x + 10 + halfW, contentY, halfW, 22), Texture2D.whiteTexture);
        GUI.color = Color.magenta;
        GUI.Label(new Rect(r.x + 14 + halfW, contentY + 2, halfW - 4, 18),
            materialItem == null ? "[2] Chon Ngoc" : "[2] " + materialItem.GetDisplayName(),
            new GUIStyle(GUI.skin.label) { fontSize = 10, fontStyle = FontStyle.Bold });

        float slotAreaH = r.height - (contentY - r.y) - 180;

        // TRAI: Trang bi co the kham
        DrawFilteredInvSlotsInRect(new Rect(r.x + 5, contentY + 26, halfW, slotAreaH),
            ref scrollInv, SocketableEquip, true, false, false);

        // PHAI: Chi Gem
        Vector2 tmpScroll = Vector2.zero;
        DrawFilteredInvSlotsInRect(new Rect(r.x + 10 + halfW, contentY + 26, halfW, slotAreaH),
            ref tmpScroll, IsGem, false, true, false);

        // ------------------
        // PREVIEW INFO BELOW
        // ------------------
        float infoY = contentY + 30 + slotAreaH;
        GUI.color = new Color(0.12f, 0.1f, 0.12f);
        GUI.DrawTexture(new Rect(r.x + 5, infoY, halfW, 90), Texture2D.whiteTexture); // Info Trang bi
        GUI.DrawTexture(new Rect(r.x + 10 + halfW, infoY, halfW, 90), Texture2D.whiteTexture); // Info Ngoc
        
        GUIStyle smText = new GUIStyle(GUI.skin.label) { fontSize = 10, wordWrap = true };
        if (targetItem != null) {
            GUI.color = targetItem.GetRankColor();
            GUI.Label(new Rect(r.x + 10, infoY + 5, halfW - 10, 20), targetItem.GetDisplayName(), new GUIStyle(GUI.skin.label){fontStyle=FontStyle.Bold, fontSize=11});
            GUI.color = Color.white;
            string s = "Goc: atk:" + targetItem.GetBaseAtk() + " def:" + targetItem.GetBaseDef() + " hp:" + targetItem.GetBaseHp();
            GUI.Label(new Rect(r.x + 10, infoY + 22, halfW - 10, 18), s, smText);
            
            if (targetItem.itemRank > 0) {
                GUI.color = new Color(0.8f, 1f, 0.8f);
                GUI.Label(new Rect(r.x + 10, infoY + 38, halfW - 10, 18), "Rank: +" + targetItem.rankBonusAtk + "a +" + targetItem.rankBonusDef + "d +" + targetItem.rankBonusHp + "h", smText);
            }
            
            GUI.color = Color.cyan;
            int sockets = targetItem.sockets.Count;
            GUI.Label(new Rect(r.x + 10, infoY + 54, halfW - 10, 18), "Da kham: " + sockets + "/5 ngoc", smText);
            if (targetItem.GetGemDef() > 0 || targetItem.GetGemAtk() > 0 || targetItem.GetGemHp() > 0) {
                GUI.color = Color.magenta;
                GUI.Label(new Rect(r.x + 10, infoY + 70, halfW - 10, 18), "Ngoc: +" + targetItem.GetGemAtk() + "a +" + targetItem.GetGemDef() + "d +" + targetItem.GetGemHp() + "h", smText);
            }
        } else {
            GUI.color = Color.gray; GUI.Label(new Rect(r.x + 5, infoY, halfW, 90), "Chon trang bi...", new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter});
        }

        if (materialItem != null) {
            GUI.color = Color.magenta;
            GUI.Label(new Rect(r.x + 15 + halfW, infoY + 5, halfW - 10, 20), materialItem.GetDisplayName(), new GUIStyle(GUI.skin.label){fontStyle=FontStyle.Bold, fontSize=11});
            GUI.color = Color.white;
            if (materialItem.data.atkBonus > 0) GUI.Label(new Rect(r.x + 15 + halfW, infoY + 25, halfW - 10, 20), "Tang ATK: +" + materialItem.data.atkBonus, smText);
            if (materialItem.data.defBonus > 0) GUI.Label(new Rect(r.x + 15 + halfW, infoY + 25, halfW - 10, 20), "Tang DEF: +" + materialItem.data.defBonus, smText);
            if (materialItem.data.hpBonus > 0) GUI.Label(new Rect(r.x + 15 + halfW, infoY + 25, halfW - 10, 20), "Tang HP: +" + materialItem.data.hpBonus, smText);
            GUI.color = Color.yellow;
            GUI.Label(new Rect(r.x + 15 + halfW, infoY + 65, halfW - 10, 20), materialItem.data.description, new GUIStyle(smText){fontSize=9});
        } else {
            GUI.color = Color.gray; GUI.Label(new Rect(r.x + 10 + halfW, infoY, halfW, 90), "Chon ngoc...", new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter});
        }

        // Nut kham
        GUI.color = (targetItem != null && materialItem != null) ? Color.magenta : Color.gray;
        if (GUI.Button(new Rect(r.x + r.width / 2 - 100, r.yMax - 48, 200, 36),
            "KHAM NGOC VAO TRANG BI",
            new GUIStyle(GUI.skin.button) { fontSize = 11, fontStyle = FontStyle.Bold }))
            Execute();
        GUI.color = Color.white;
    }

    bool SocketableEquip(ItemInstance inst)
    {
        return inst != null && inst.data != null &&
               (inst.data.type == ItemData.ItemType.Weapon ||
                inst.data.type == ItemData.ItemType.Armor ||
                inst.data.type == ItemData.ItemType.Accessory);
    }

    bool IsGem(ItemInstance inst)
    {
        return inst != null && inst.data != null && inst.data.type == ItemData.ItemType.Gem;
    }

    // ============================================================
    //  TAB 4: CHE TAO - Loc theo cong thuc co the
    // ============================================================
    void DrawCraftTab(Rect r, float contentY)
    {
        float halfW = (r.width - 20) / 2f;

        GUI.color = new Color(0.12f, 0.1f, 0.08f);
        GUI.DrawTexture(new Rect(r.x + 5, contentY, halfW, 22), Texture2D.whiteTexture);
        GUI.color = new Color(0.9f, 0.5f, 0.1f);
        GUI.Label(new Rect(r.x + 8, contentY + 2, halfW - 4, 18),
            targetItem == null ? "[1] Nguyen Lieu 1" : "[1] " + targetItem.GetDisplayName(),
            new GUIStyle(GUI.skin.label) { fontSize = 10, fontStyle = FontStyle.Bold });

        GUI.color = new Color(0.12f, 0.1f, 0.08f);
        GUI.DrawTexture(new Rect(r.x + 10 + halfW, contentY, halfW, 22), Texture2D.whiteTexture);
        GUI.color = new Color(0.9f, 0.5f, 0.1f);
        GUI.Label(new Rect(r.x + 14 + halfW, contentY + 2, halfW - 4, 18),
            materialItem == null ? "[2] Nguyen Lieu 2" : "[2] " + materialItem.GetDisplayName(),
            new GUIStyle(GUI.skin.label) { fontSize = 10, fontStyle = FontStyle.Bold });

        float slotAreaH = r.height - (contentY - r.y) - 110;
        Vector2 sc1 = scrollInv, sc2 = Vector2.zero;
        DrawFilteredInvSlotsInRect(new Rect(r.x + 5, contentY + 26, halfW, slotAreaH), ref sc1, (_) => true, true, false, false);
        DrawFilteredInvSlotsInRect(new Rect(r.x + 10 + halfW, contentY + 26, halfW, slotAreaH), ref sc2, (_) => true, false, true, false);
        scrollInv = sc1;

        // Preview ket qua
        if (targetItem != null && materialItem != null)
        {
            ItemData recipe = RPG_BlacksmithLogic.CheckRecipe(targetItem, materialItem);
            float py = r.yMax - 90;
            GUI.color = new Color(0.14f, 0.12f, 0.1f);
            GUI.DrawTexture(new Rect(r.x + 10, py, r.width - 20, 44), Texture2D.whiteTexture);
            if (recipe != null)
            {
                GUI.color = Color.green;
                GUI.Label(new Rect(r.x + 16, py + 6, r.width - 30, 22), "Ket qua: " + recipe.itemName,
                    new GUIStyle(GUI.skin.label) { fontSize = 13, fontStyle = FontStyle.Bold });
                GUI.color = Color.yellow;
                if (GUI.Button(new Rect(r.x + r.width / 2 - 100, py + 28, 200, 32), "CHE TAO NGAY",
                    new GUIStyle(GUI.skin.button) { fontSize = 12, fontStyle = FontStyle.Bold }))
                    Execute();
            }
            else
            {
                GUI.color = Color.gray;
                GUI.Label(new Rect(r.x + 16, py + 10, r.width - 30, 22), "Khong co cong thuc phu hop...",
                    new GUIStyle(GUI.skin.label) { fontSize = 11 });
            }
        }
        else
        {
            GUI.color = Color.gray;
            float py = r.yMax - 45;
            GUI.Label(new Rect(r.x + 10, py, r.width - 20, 22),
                "Chon 2 vat pham be trai va phai de xem cong thuc",
                new GUIStyle(GUI.skin.label) { fontSize = 10, alignment = TextAnchor.MiddleCenter });
        }
        GUI.color = Color.white;
    }

    // ============================================================
    //  TAB 5: GHÉP ĐỒ (NÂNG HẠNG)
    // ============================================================
    void DrawMergeTab(Rect r, float contentY)
    {
        float halfW = (r.width - 20) / 2f;

        GUI.color = new Color(0.12f, 0.1f, 0.08f);
        GUI.DrawTexture(new Rect(r.x + 5, contentY, halfW, 22), Texture2D.whiteTexture);
        GUI.color = new Color(1f, 0.4f, 0.4f);
        GUI.Label(new Rect(r.x + 8, contentY + 2, halfW - 4, 18),
            targetItem == null ? "[1] Chon Vat Pham 1" : "[1] " + targetItem.GetDisplayName(),
            new GUIStyle(GUI.skin.label) { fontSize = 10, fontStyle = FontStyle.Bold });

        GUI.color = new Color(0.12f, 0.1f, 0.08f);
        GUI.DrawTexture(new Rect(r.x + 10 + halfW, contentY, halfW, 22), Texture2D.whiteTexture);
        GUI.color = new Color(1f, 0.4f, 0.4f);
        GUI.Label(new Rect(r.x + 14 + halfW, contentY + 2, halfW - 4, 18),
            materialItem == null ? "[2] Chon Vat Pham 2 (Giong het)" : "[2] " + materialItem.GetDisplayName(),
            new GUIStyle(GUI.skin.label) { fontSize = 10, fontStyle = FontStyle.Bold });

        float slotAreaH = r.height - (contentY - r.y) - 110;
        Vector2 sc1 = scrollInv, sc2 = Vector2.zero;
        
        bool CanMerge(ItemInstance inst) {
            return inst != null && inst.data != null && inst.data.type != ItemData.ItemType.Consumable && inst.itemRank < 6;
        }

        DrawFilteredInvSlotsInRect(new Rect(r.x + 5, contentY + 26, halfW, slotAreaH), ref sc1, CanMerge, true, false, false);
        DrawFilteredInvSlotsInRect(new Rect(r.x + 10 + halfW, contentY + 26, halfW, slotAreaH), ref sc2, CanMerge, false, true, false);
        scrollInv = sc1;

        if (targetItem != null && materialItem != null)
        {
            float py = r.yMax - 90;
            GUI.color = new Color(0.14f, 0.12f, 0.1f);
            GUI.DrawTexture(new Rect(r.x + 10, py, r.width - 20, 60), Texture2D.whiteTexture);
            
            if (targetItem.data.itemName == materialItem.data.itemName && targetItem.itemRank == materialItem.itemRank)
            {
                int cost = (targetItem.itemRank + 1) * 1000;
                float[] rates = {40f, 30f, 20f, 15f, 10f, 5f, 0f};
                float rate = rates[targetItem.itemRank];

                GUI.color = Color.yellow;
                GUI.Label(new Rect(r.x + 16, py + 6, r.width - 30, 22), "Len Hang: " + (targetItem.itemRank+1) + "  |  Ti le: " + rate + "%  |  Phi: " + cost + " Vang",
                    new GUIStyle(GUI.skin.label) { fontSize = 11, fontStyle = FontStyle.Bold });
                
                bool canAfford = player.gold >= cost;
                GUI.color = canAfford ? Color.red : Color.gray;
                if (GUI.Button(new Rect(r.x + r.width / 2 - 100, py + 28, 200, 26), canAfford ? "EP HANG (XIT MAT DO)" : "KHONG DU VANG",
                    new GUIStyle(GUI.skin.button) { fontSize = 11, fontStyle = FontStyle.Bold }))
                    Execute();
            }
            else
            {
                GUI.color = Color.gray;
                GUI.Label(new Rect(r.x + 16, py + 10, r.width - 30, 22), "2 Vat Pham khong giong mhau hoac khac Hang!",
                    new GUIStyle(GUI.skin.label) { fontSize = 11 });
            }
        }
    }

    // ============================================================
    //  HELPER: Ve slot loc theo filter
    // ============================================================
    delegate bool ItemFilter(ItemInstance inst);

    void DrawFilteredInvSlots(Rect r, float startY, ItemFilter filter, bool forMaterial)
    {
        List<ItemInstance> inv = player.SharedInventory;
        int cols = 7;
        float slotSz = 72f, pad = 5f;
        float areaH = r.height - (startY - r.y) - 10;
        Rect scrollArea = new Rect(r.x + 5, startY, r.width - 10, Mathf.Min(areaH, 250));
        var filtered = new List<(ItemInstance inst, int idx)>();
        for (int i = 0; i < inv.Count; i++)
            if (filter(inv[i])) filtered.Add((inv[i], i));

        int rows2 = Mathf.CeilToInt((float)filtered.Count / cols);
        Rect content = new Rect(0, 0, cols * (slotSz + pad), Mathf.Max(240, rows2 * (slotSz + pad)));
        scrollInv = GUI.BeginScrollView(scrollArea, scrollInv, content);
        DrawSlotsCore(filtered, slotSz, pad, cols, forMaterial);
        GUI.EndScrollView();
        GUI.color = Color.white;
    }

    void DrawFilteredInvSlotsInRect(Rect area, ref Vector2 scroll, ItemFilter filter,
        bool asTarget, bool asMaterial, bool forCraft)
    {
        List<ItemInstance> inv = player.SharedInventory;
        float slotSz = 68f, pad = 4f;
        int cols = Mathf.FloorToInt(area.width / (slotSz + pad));
        if (cols < 1) cols = 1;
        var filtered = new List<(ItemInstance inst, int idx)>();
        for (int i = 0; i < inv.Count; i++)
            if (filter(inv[i])) filtered.Add((inv[i], i));

        int rows2 = Mathf.CeilToInt((float)filtered.Count / cols);
        Rect content = new Rect(0, 0, cols * (slotSz + pad), Mathf.Max(200, rows2 * (slotSz + pad)));
        scroll = GUI.BeginScrollView(area, scroll, content);

        for (int vi = 0; vi < filtered.Count; vi++)
        {
            var (inst, realIdx) = filtered[vi];
            int col = vi % cols, row = vi / cols;
            float sx = col * (slotSz + pad), sy = row * (slotSz + pad);

            bool isT = (targetIdx == realIdx), isM = (materialIdx == realIdx);
            GUI.color = isT ? new Color(1f, 0.5f, 0.1f) : isM ? new Color(0.6f, 0.1f, 1f) : new Color(0.2f, 0.18f, 0.14f);
            GUI.DrawTexture(new Rect(sx, sy, slotSz, slotSz), Texture2D.whiteTexture);

            if (inst.data.icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(sx + 3, sy + 3, slotSz - 6, slotSz - 6), inst.data.icon.texture); }
            else { GUI.color = GetTypeColor(inst.data.type); GUI.DrawTexture(new Rect(sx + 3, sy + 3, slotSz - 6, slotSz - 6), Texture2D.whiteTexture); GUI.color = Color.white; GUI.Label(new Rect(sx, sy + 18, slotSz, 28), GetTypeIcon(inst.data.type), new GUIStyle(GUI.skin.label) { fontSize = 20, alignment = TextAnchor.MiddleCenter }); }
            // Ve 5 ngoc mini
            for (int k=0; k<inst.sockets.Count && k<5; k++) {
                float gx = sx + slotSz - 10; float gy = sy + 3 + (k * 7f);
                if (inst.sockets[k].icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(gx, gy, 7, 7), inst.sockets[k].icon.texture); }
                else { GUI.color = Color.magenta; GUI.DrawTexture(new Rect(gx, gy, 7, 7), Texture2D.whiteTexture); }
            }

            GUI.color = isT ? Color.yellow : isM ? Color.magenta : Color.white;
            GUI.Label(new Rect(sx, sy + slotSz - 14, slotSz, 14), inst.GetDisplayName(),
                new GUIStyle(GUI.skin.label) { fontSize = 7, alignment = TextAnchor.MiddleCenter, wordWrap = true });

            GUI.color = Color.clear;
            if (GUI.Button(new Rect(sx, sy, slotSz, slotSz), ""))
            {
                if (asTarget) {
                    if (isT) { targetItem = null; targetIdx = -1; }
                    else { targetItem = inst; targetIdx = realIdx; }
                } else if (asMaterial) {
                    if (isM) { materialItem = null; materialIdx = -1; }
                    else { materialItem = inst; materialIdx = realIdx; }
                }
            }
        }
        GUI.EndScrollView();
        GUI.color = Color.white;
    }

    void DrawSlotsCore(List<(ItemInstance inst, int idx)> filtered, float slotSz, float pad, int cols, bool forMaterial)
    {
        for (int vi = 0; vi < filtered.Count; vi++)
        {
            var (inst, realIdx) = filtered[vi];
            int col = vi % cols, row = vi / cols;
            float sx = col * (slotSz + pad), sy = row * (slotSz + pad);

            bool isT = (targetIdx == realIdx), isM = (materialIdx == realIdx);
            GUI.color = isT ? new Color(1f, 0.5f, 0.1f) : isM ? new Color(0.6f, 0.1f, 1f) : new Color(0.2f, 0.18f, 0.14f);
            GUI.DrawTexture(new Rect(sx, sy, slotSz, slotSz), Texture2D.whiteTexture);

            if (inst.data.icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(sx + 3, sy + 3, slotSz - 6, slotSz - 6), inst.data.icon.texture); }
            else { GUI.color = GetTypeColor(inst.data.type); GUI.DrawTexture(new Rect(sx + 3, sy + 3, slotSz - 6, slotSz - 6), Texture2D.whiteTexture); GUI.color = Color.white; GUI.Label(new Rect(sx, sy + 18, slotSz, 28), GetTypeIcon(inst.data.type), new GUIStyle(GUI.skin.label) { fontSize = 20, alignment = TextAnchor.MiddleCenter }); }
            // Ve 5 ngoc mini
            for (int k=0; k<inst.sockets.Count && k<5; k++) {
                float gx = sx + slotSz - 10; float gy = sy + 3 + (k * 7f);
                if (inst.sockets[k].icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(gx, gy, 7, 7), inst.sockets[k].icon.texture); }
                else { GUI.color = Color.magenta; GUI.DrawTexture(new Rect(gx, gy, 7, 7), Texture2D.whiteTexture); }
            }


            GUI.color = isT ? Color.yellow : isM ? Color.magenta : Color.white;
            GUI.Label(new Rect(sx, sy + slotSz - 14, slotSz, 14), inst.GetDisplayName(),
                new GUIStyle(GUI.skin.label) { fontSize = 7, alignment = TextAnchor.MiddleCenter, wordWrap = true });

            GUI.color = Color.clear;
            if (GUI.Button(new Rect(sx, sy, slotSz, slotSz), ""))
            {
                if (!forMaterial) {
                    if (isT) { targetItem = null; targetIdx = -1; }
                    else { targetItem = inst; targetIdx = realIdx; }
                } else {
                    if (isM) { materialItem = null; materialIdx = -1; }
                    else { materialItem = inst; materialIdx = realIdx; }
                }
            }
        }
    }

    Color GetTypeColor(ItemData.ItemType t)
    {
        switch (t)
        {
            case ItemData.ItemType.Weapon: return new Color(0.8f, 0.2f, 0.2f, 0.8f);
            case ItemData.ItemType.Armor: return new Color(0.2f, 0.4f, 0.8f, 0.8f);
            case ItemData.ItemType.Gem: return new Color(0.7f, 0.1f, 0.9f, 0.8f);
            case ItemData.ItemType.Consumable: return new Color(0.1f, 0.7f, 0.2f, 0.8f);
            default: return new Color(0.5f, 0.5f, 0.5f, 0.8f);
        }
    }

    string GetTypeIcon(ItemData.ItemType t)
    {
        switch (t)
        {
            case ItemData.ItemType.Weapon: return "W";
            case ItemData.ItemType.Armor: return "A";
            case ItemData.ItemType.Gem: return "G";
            case ItemData.ItemType.Consumable: return "C";
            default: return "?";
        }
    }
}