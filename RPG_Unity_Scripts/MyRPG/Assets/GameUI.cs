using UnityEngine;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// ===========================
// GIAO DIỆN NGƯỜI CHƠI - PHIÊN BẢN SẠCH CHỮ (UTF-8 BOM)
// Fix: HUD gọn, vàng chung, click đệ tử, ảnh item, tooltip chi tiết
// ===========================
public class GameUI : MonoBehaviour
{
    public static GameUI instance;

    // ===== Damage Text =====
    class DamageText { public Vector3 worldPos; public string text; public Color color; public float timer; public float alpha = 1f; }
    private List<DamageText> damageTexts = new List<DamageText>();
    private GUIStyle dmgStyle;

    // ===== Styles =====
    private GUIStyle expStyle;
    private GUIStyle goldStyle;
    private GUIStyle headerStyle;
    private GUIStyle slotLabelStyle;
    private GUIStyle statValueStyle;

    public PlayerStats player;
    public BlacksmithUI blacksmithUI;
    private List<PlayerStats> companions = new List<PlayerStats>();
    private PlayerStats currentView;
    private PlayerCombat combat;

    public bool isBagOpen = false;
    private int currentTab = 0;
    private int selectedItemIdx = -1;
    private string selectedSlot = "";
    private Vector2 scrollPos;

    private PlayerStats hudClickedCompanion = null;

    void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    void Start() { TryFindPlayer(); }

    void TryFindPlayer()
    {
        PlayerStats[] allStats = Object.FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
        if (player == null) {
            foreach(var s in allStats) if(s.isPlayer) { player = s; break; }
        }
        if (player == null && allStats.Length > 0) player = allStats[0];
        
        companions.Clear();
        foreach(var s in allStats) if(s != player) companions.Add(s);

        if (combat == null) combat = FindObjectOfType<PlayerCombat>();
        if (currentView == null) {
            if (player != null) currentView = player;
            else if (companions.Count > 0) currentView = companions[0];
        }
    }

    public void ShowDamage(Vector3 pos, string txt, Color col)
    {
        damageTexts.Add(new DamageText { worldPos = pos, text = txt, color = col, timer = 1.2f });
    }

    void Update()
    {
        TryFindPlayer();
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame) { isBagOpen = !isBagOpen; ResetSelection(); }
#else
        if (Input.GetKeyDown(KeyCode.B)) { isBagOpen = !isBagOpen; ResetSelection(); }
#endif
        for (int i = damageTexts.Count - 1; i >= 0; i--) {
            damageTexts[i].timer -= Time.deltaTime;
            damageTexts[i].worldPos += Vector3.up * Time.deltaTime * 0.6f;
            damageTexts[i].alpha = Mathf.Clamp01(damageTexts[i].timer);
            if (damageTexts[i].timer <= 0) damageTexts.RemoveAt(i);
        }

        if (hudClickedCompanion != null) {
            isBagOpen = true;
            currentView = hudClickedCompanion;
            currentTab = 0;
            ResetSelection();
            hudClickedCompanion = null;
        }
    }

    void ResetSelection() { selectedItemIdx = -1; selectedSlot = ""; }

    void InitStyles()
    {
        if (dmgStyle == null) dmgStyle = new GUIStyle(GUI.skin.label) { fontSize = 22, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        if (expStyle == null) expStyle = new GUIStyle(GUI.skin.label) { fontSize = 11, fontStyle = FontStyle.Bold };
        if (headerStyle == null) headerStyle = new GUIStyle(GUI.skin.label) { fontSize = 13, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        if (slotLabelStyle == null) { slotLabelStyle = new GUIStyle(GUI.skin.label) { fontSize = 9, alignment = TextAnchor.MiddleCenter, wordWrap = true }; slotLabelStyle.normal.textColor = Color.white; }
        if (statValueStyle == null) { statValueStyle = new GUIStyle(GUI.skin.label) { fontSize = 11, fontStyle = FontStyle.Bold }; }
    }

    void OnGUI()
    {
        InitStyles();
        DrawFloatingDamage();
        GUI.depth = 0;

        if (player == null) { TryFindPlayer(); if(player == null) return; }

        DrawHUD();

        if (blacksmithUI != null && blacksmithUI.isOpen) {
            blacksmithUI.DrawWindow();
            return;
        }

        if (isBagOpen)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            GUI.color = new Color(0, 0, 0, 0.5f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);

            float uiW = Mathf.Min(900, Screen.width * 0.95f);
            float uiH = Mathf.Min(560, Screen.height * 0.9f);
            Rect mainR = new Rect(Screen.width / 2 - uiW / 2, Screen.height / 2 - uiH / 2, uiW, uiH);

            GUI.color = new Color(0.08f, 0.08f, 0.15f, 0.98f);
            GUI.DrawTexture(mainR, Texture2D.whiteTexture);
            GUI.color = Color.white;
            if (GUI.Button(new Rect(mainR.xMax-45, mainR.y+8, 35, 28), "✕")) isBagOpen = false;

            DrawTabs(mainR);
            if (currentTab == 0) DrawCharacterTab(mainR);
            else DrawSkillsTab(mainR);
            DrawFooter(mainR);
        }
    }

    void DrawTabs(Rect mainR)
    {
        float tabX = mainR.x + 15;
        float tabY = mainR.y + 10;
        GUI.color = currentTab == 0 ? new Color(0.3f, 0.6f, 1f) : new Color(0.2f, 0.2f, 0.3f);
        if (GUI.Button(new Rect(tabX, tabY, 130, 32), "🛡 TRANG BỊ")) currentTab = 0;
        GUI.color = currentTab == 1 ? new Color(1f, 0.6f, 0.1f) : new Color(0.2f, 0.2f, 0.3f);
        if (GUI.Button(new Rect(tabX + 140, tabY, 130, 32), "⚡ KỸ NĂNG")) currentTab = 1;

        GUI.color = Color.cyan;
        string viewName = (currentView == player) ? "👑 HIỆP SĨ" : "🔹 " + currentView.characterName;
        GUI.Label(new Rect(mainR.x + 300, tabY, 200, 32), viewName, new GUIStyle(GUI.skin.label){fontSize=13, fontStyle=FontStyle.Bold});
        GUI.color = Color.white;
    }

    void DrawCharacterTab(Rect mainR)
    {
        if (currentView == null) return;
        float sidebarW = 160;
        float pdX = mainR.x + sidebarW + 10; float pdY = mainR.y + 55;
        DrawSidebar(mainR.x + 5, pdY, sidebarW);

        GUI.color = new Color(0.12f, 0.12f, 0.22f, 1f);
        GUI.DrawTexture(new Rect(pdX, pdY, 240, 440), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(pdX, pdY+5, 240, 25), "Archer [RANK D]", headerStyle);

        GUI.color = new Color(1f, 0.85f, 0.2f);
        GUI.Label(new Rect(pdX+10, pdY+28, 220, 20), "💰 Vàng: " + player.gold, new GUIStyle(GUI.skin.label){fontSize=11, fontStyle=FontStyle.Bold});
        GUI.color = Color.white;

        float sX = pdX + 15;
        if (DrawSlot(sX, pdY+52, currentView.eqHead, selectedSlot=="Head", "Đầu")) {selectedSlot="Head"; selectedItemIdx=-1;}
        if (DrawSlot(sX+70, pdY+52, currentView.eqNecklace, selectedSlot=="Neck", "Dây")) {selectedSlot="Neck"; selectedItemIdx=-1;}
        if (DrawSlot(sX+140, pdY+52, currentView.eqAncientGold, selectedSlot=="Ancient", "Vàng")) {selectedSlot="Ancient"; selectedItemIdx=-1;}
        if (DrawSlot(sX, pdY+125, currentView.eqWeaponMain, selectedSlot=="WepMain", "Vũ Khí")) {selectedSlot="WepMain"; selectedItemIdx=-1;}
        if (DrawSlot(sX+70, pdY+125, currentView.eqBody, selectedSlot=="Body", "Áo")) {selectedSlot="Body"; selectedItemIdx=-1;}
        if (DrawSlot(sX+140, pdY+125, currentView.eqWeaponOff, selectedSlot=="WepOff", "Khiên")) {selectedSlot="WepOff"; selectedItemIdx=-1;}
        if (DrawSlot(sX, pdY+198, currentView.eqRing1, selectedSlot=="Ring1", "Nhẫn 1")) {selectedSlot="Ring1"; selectedItemIdx=-1;}
        if (DrawSlot(sX+70, pdY+198, currentView.eqLegs, selectedSlot=="Legs", "Giày")) {selectedSlot="Legs"; selectedItemIdx=-1;}
        if (DrawSlot(sX+140, pdY+198, currentView.eqRing2, selectedSlot=="Ring2", "Nhẫn 2")) {selectedSlot="Ring2"; selectedItemIdx=-1;}

        float statY = pdY + 275;
        GUIStyle statS = new GUIStyle(GUI.skin.label) { fontSize = 12, fontStyle = FontStyle.Bold };
        DrawShadowLabel(new Rect(pdX + 15, statY, 210, 22), "⚔ Sức mạnh: " + currentView.bonusDamage, statS, Color.white);
        DrawShadowLabel(new Rect(pdX + 15, statY + 25, 210, 22), "🛡 Phòng ngự: " + currentView.bonusDefense, statS, Color.white);
        DrawShadowLabel(new Rect(pdX + 15, statY + 50, 210, 22), "❤ Sinh mệnh: " + currentView.currentHealth + "/" + currentView.maxHealth, statS, new Color(1f, 0.4f, 0.4f));

        float stY = pdY + 360;
        GUI.color = Color.yellow;
        DrawShadowLabel(new Rect(pdX+15, stY, 220, 22), "ĐIỂM TIỀM NĂNG: " + currentView.statPoints, statValueStyle, Color.yellow);
        DrawStatRow(pdX+15, stY+25, "Sức mạnh (STR)", currentView.STR, "STR");
        DrawStatRow(pdX+15, stY+50, "Thể chất (VIT)", currentView.VIT, "VIT");
        DrawStatRow(pdX+15, stY+75, "Nhanh nhẹn (AGI)", currentView.AGI, "AGI");

        DrawInventory(pdX + 250, pdY);
        DrawTooltip(pdX + 250 + 245, pdY);
    }

    void DrawSidebar(float x, float y, float w) {
        GUI.color = new Color(0.12f, 0.12f, 0.18f, 0.98f); GUI.DrawTexture(new Rect(x, y, w, 440), Texture2D.whiteTexture);
        GUI.color = Color.white; GUI.Label(new Rect(x, y+5, w, 25), "👥 ĐỘI NGŨ", headerStyle);
        float btnY = y + 40;
        bool isP = (currentView == player); GUI.color = isP ? new Color(0.3f, 0.6f, 1f) : Color.white;
        if (GUI.Button(new Rect(x+8, btnY, w-16, 40), (isP?"👑 ":"") + "HIỆP SĨ")) { currentView = player; ResetSelection(); }
        btnY += 45;
        foreach(var c in companions) {
            if (c == null) continue;
            bool isS = (currentView == c); GUI.color = isS ? new Color(0.3f, 0.8f, 0.4f) : Color.white;
            if (GUI.Button(new Rect(x+8, btnY, w-16, 40), (isS?"🔹 ":"") + c.characterName)) { currentView = c; ResetSelection(); }
            btnY += 45;
        }
    }

    void DrawStatRow(float x, float y, string label, int val, string statKey) {
        GUI.color = Color.white; GUI.Label(new Rect(x, y, 150, 22), label + ": " + val);
        if (currentView.statPoints > 0) { GUI.color = Color.green; if (GUI.Button(new Rect(x + 155, y, 28, 18), "+")) currentView.AddStat(statKey); }
    }

    void DrawInventory(float x, float y) {
        GUI.color = Color.white; GUI.Label(new Rect(x, y, 120, 25), "📦 HÒM ĐỒ CHUNG", headerStyle);
        Rect vR = new Rect(x, y+30, 240, 420); List<ItemInstance> inv = player.SharedInventory;
        Rect cR = new Rect(0,0, 220, Mathf.Max(425, inv.Count * 48)); scrollPos = GUI.BeginScrollView(vR, scrollPos, cR);
        for (int i=0; i<inv.Count; i++) {
            if (inv[i] == null || inv[i].data == null) continue;
            bool sel = (selectedItemIdx == i); GUI.color = sel ? new Color(0.3f, 0.5f, 0.2f) : new Color(0.15f, 0.15f, 0.25f);
            GUI.DrawTexture(new Rect(0, i*48, 220, 44), Texture2D.whiteTexture);
            if (inv[i].data.icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(4, i*48+4, 36, 36), inv[i].data.icon.texture); }
            else { GUI.color = GetItemTypeColor(inv[i].data.type); GUI.DrawTexture(new Rect(4, i*48+4, 36, 36), Texture2D.whiteTexture); GUI.color = Color.white; GUI.Label(new Rect(4, i*48+12, 36, 20), GetItemTypeIcon(inv[i].data.type), new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter, fontSize=16}); }
            
            // Vẽ ngọc mini
            // Vẽ ngọc mini (tối đa 5 viên xếp dọc)
            for (int k = 0; k < inv[i].sockets.Count && k < 5; k++) {
                var gemData = inv[i].sockets[k];
                if (gemData == null) continue;
                float gx = 32f; float gy = i*48 + 4 + (k * 7f);
                if (gemData.icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(gx, gy, 8, 8), gemData.icon.texture); }
                else { GUI.color = Color.magenta; GUI.DrawTexture(new Rect(gx, gy, 8, 8), Texture2D.whiteTexture); }
            }

            GUI.color = sel ? Color.white : inv[i].GetRankColor();
            GUIStyle btnStyle = new GUIStyle(GUI.skin.button){alignment=TextAnchor.MiddleLeft};
            btnStyle.normal.textColor = GUI.color; btnStyle.hover.textColor = Color.white;
            GUI.color = Color.clear; // nut trong suot
            if (GUI.Button(new Rect(45, i*48, 175, 44), "   " + inv[i].GetDisplayName(), btnStyle)) { selectedItemIdx = i; selectedSlot = ""; if (BlacksmithUI.instance != null && BlacksmithUI.instance.isOpen) { if (BlacksmithUI.instance.targetItem == null) BlacksmithUI.instance.SetTarget(i); else BlacksmithUI.instance.SetMaterial(i); } }
            
            GUI.color = inv[i].GetRankColor(); // ve chu binh thuong
            GUI.Label(new Rect(55, i*48+12, 160, 20), inv[i].GetDisplayName(), new GUIStyle(GUI.skin.label){fontStyle=FontStyle.Bold});
        }
        GUI.EndScrollView(); GUI.color = Color.white;
    }

    public Color GetItemTypeColor(ItemData.ItemType t) {
        switch(t) {
            case ItemData.ItemType.Weapon: return new Color(0.8f, 0.2f, 0.2f, 0.7f);
            case ItemData.ItemType.Armor:  return new Color(0.2f, 0.4f, 0.8f, 0.7f);
            case ItemData.ItemType.Gem:    return new Color(0.8f, 0.2f, 0.8f, 0.7f);
            case ItemData.ItemType.Consumable: return new Color(0.2f, 0.8f, 0.3f, 0.7f);
            default: return new Color(0.5f, 0.5f, 0.5f, 0.7f);
        }
    }
    public string GetItemTypeIcon(ItemData.ItemType t) {
        switch(t) {
            case ItemData.ItemType.Weapon: return "⚔";
            case ItemData.ItemType.Armor:  return "🛡";
            case ItemData.ItemType.Gem:    return "💎";
            case ItemData.ItemType.Consumable: return "🧪";
            default: return "📦";
        }
    }

    void DrawTooltip(float x, float y) {
        GUI.color = new Color(0.08f, 0.1f, 0.15f, 0.95f); GUI.DrawTexture(new Rect(x, y, 210, 480), Texture2D.whiteTexture);
        ItemInstance inst = null; bool fromInv = false; List<ItemInstance> sharedInv = player.SharedInventory;
        if (selectedItemIdx >= 0 && selectedItemIdx < sharedInv.Count) { inst = sharedInv[selectedItemIdx]; fromInv = true; }
        else if (selectedSlot != "") {
            switch(selectedSlot) {
                case "Head": inst=currentView.eqHead; break; case "Body": inst=currentView.eqBody; break; case "Legs": inst=currentView.eqLegs; break;
                case "WepMain": inst=currentView.eqWeaponMain; break; case "WepOff": inst=currentView.eqWeaponOff; break;
                case "Ring1": inst=currentView.eqRing1; break; case "Ring2": inst=currentView.eqRing2; break;
                case "Neck": inst=currentView.eqNecklace; break; case "Ancient": inst=currentView.eqAncientGold; break;
            }
        }
        if (inst == null || inst.data == null) { GUI.color = Color.gray; GUI.Label(new Rect(x+10, y+200, 190, 50), "← Chọn vật phẩm\nđể xem chi tiết", new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter}); return; }
        var d = inst.data; float curY = y + 8;
        if (d.icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(x + 70, curY, 70, 70), d.icon.texture); curY += 78; }
        else { GUI.color = GetItemTypeColor(d.type); GUI.DrawTexture(new Rect(x + 70, curY, 70, 70), Texture2D.whiteTexture); GUI.color = Color.white; GUI.Label(new Rect(x+70, curY+20, 70, 30), GetItemTypeIcon(d.type), new GUIStyle(GUI.skin.label){fontSize=28, alignment=TextAnchor.MiddleCenter}); curY += 78; }
        
        GUI.color = inst.GetRankColor(); GUI.Label(new Rect(x+5, curY, 200, 30), inst.GetDisplayName(), new GUIStyle(GUI.skin.label){fontSize=12, fontStyle=FontStyle.Bold, alignment=TextAnchor.MiddleCenter}); curY += 32;
        GUI.color = new Color(1f, 0.75f, 0.2f); GUI.Label(new Rect(x+5, curY, 200, 18), "Giá: " + d.price + " Vàng", new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter, fontSize=10}); curY += 20;
        GUI.color = Color.gray; GUI.Label(new Rect(x+8, curY, 195, 30), d.description, new GUIStyle(GUI.skin.label){fontSize=9, wordWrap=true}); curY += 35;
        
        GUIStyle stType = new GUIStyle(GUI.skin.label){fontSize=10, fontStyle=FontStyle.Bold};
        if (inst.GetBaseAtk() > 0) { GUI.color = Color.red; GUI.Label(new Rect(x+10, curY, 190, 18), "Tấn Công: " + inst.GetBaseAtk() + " (Gốc)", stType); curY += 18; }
        if (inst.GetBaseDef() > 0) { GUI.color = Color.cyan; GUI.Label(new Rect(x+10, curY, 190, 18), "Phòng Thủ: " + inst.GetBaseDef() + " (Gốc)", stType); curY += 18; }
        if (inst.GetBaseHp() > 0) { GUI.color = Color.green; GUI.Label(new Rect(x+10, curY, 190, 18), "Sinh Mệnh: " + inst.GetBaseHp() + " (Gốc)", stType); curY += 18; }

        if (inst.itemRank > 0) {
            curY += 5; GUI.color = Color.gray; GUI.DrawTexture(new Rect(x+10, curY, 190, 1), Texture2D.whiteTexture); curY += 4;
            GUI.color = inst.GetRankColor(); GUI.Label(new Rect(x+10, curY, 190, 18), "Thuộc tính Hạng [" + inst.GetRankString() + "]", stType); curY += 18;
            if (inst.rankBonusAtk > 0) { GUI.color = new Color(1f, 0.4f, 0.4f); GUI.Label(new Rect(x+20, curY, 180, 18), "+ " + inst.rankBonusAtk + " Tấn Công (Bonus)", stType); curY += 18; }
            if (inst.rankBonusDef > 0) { GUI.color = new Color(0.4f, 0.8f, 1f); GUI.Label(new Rect(x+20, curY, 180, 18), "+ " + inst.rankBonusDef + " Phòng Thủ (Bonus)", stType); curY += 18; }
            if (inst.rankBonusHp > 0)  { GUI.color = new Color(0.4f, 1f, 0.4f);  GUI.Label(new Rect(x+20, curY, 180, 18), "+ " + inst.rankBonusHp + " Sinh Mệnh (Bonus)", stType); curY += 18; }
        }

        // 3. PHÂN ĐOẠN NGỌC
        if (inst.sockets.Count > 0) {
            curY += 5; GUI.color = Color.gray; GUI.DrawTexture(new Rect(x+10, curY, 190, 1), Texture2D.whiteTexture); curY += 4;
            GUI.color = Color.magenta; GUI.Label(new Rect(x+10, curY, 190, 18), "Ngọc Khảm (" + inst.sockets.Count + "/5)", stType); curY += 20;
            foreach (var gem in inst.sockets) {
                if (gem == null) continue;
                if (gem.icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(x+12, curY+2, 14, 14), gem.icon.texture); }
                else { GUI.color = Color.magenta; GUI.DrawTexture(new Rect(x+12, curY+2, 14, 14), Texture2D.whiteTexture); }
                
                GUI.color = Color.white;
                string gemStat = "";
                if (gem.atkBonus > 0) gemStat = "+" + gem.atkBonus + " Tấn Công";
                else if (gem.defBonus > 0) gemStat = "+" + gem.defBonus + " Phòng Thủ";
                else if (gem.hpBonus > 0) gemStat = "+" + gem.hpBonus + " Sinh Mệnh";
                GUI.Label(new Rect(x+30, curY, 170, 18), gemStat, new GUIStyle(GUI.skin.label){fontSize=9});
                curY += 18;
            }
        }

        float btnY = y + 370;
        if (fromInv) {
            // 1 nut duy nhat - mac cho currentView dang chon o sidebar
            string targetName = (currentView == player) ? "HIEP SI" : currentView.characterName;
            if (d.type == ItemData.ItemType.Weapon || d.type == ItemData.ItemType.Armor || d.type == ItemData.ItemType.Accessory) {
                GUI.color = new Color(0.2f, 0.6f, 1f);
                if (GUI.Button(new Rect(x+8, btnY, 195, 32), "MAC CHO: " + targetName,
                    new GUIStyle(GUI.skin.button){fontSize=12, fontStyle=FontStyle.Bold})) {
                    currentView.EquipItem(selectedItemIdx); ResetSelection();
                }
                btnY += 36;
            } else if (d.type == ItemData.ItemType.Consumable) {
                GUI.color = Color.green;
                if (GUI.Button(new Rect(x+8, btnY, 195, 32), "SU DUNG",
                    new GUIStyle(GUI.skin.button){fontSize=12, fontStyle=FontStyle.Bold})) {
                    currentView.UseConsumable(selectedItemIdx); ResetSelection();
                }
                btnY += 36;
            } else if (d.type == ItemData.ItemType.Gem) {
                GUI.color = Color.magenta;
                GUI.Label(new Rect(x+8, btnY, 195, 40), "Ngoc: Dung trong Lo Ren",
                    new GUIStyle(GUI.skin.label){fontSize=10, wordWrap=true});
                btnY += 36;
            }
        } else {
            GUI.color = Color.red;
            if (GUI.Button(new Rect(x+8, btnY, 195, 32), "GO BO",
                new GUIStyle(GUI.skin.button){fontSize=12})) {
                currentView.UnequipItem(selectedSlot); ResetSelection();
            }
        }
        GUI.color = Color.white;
    }

    void DrawSkillsTab(Rect mainR) {
        float sidebarW = 160; DrawSidebar(mainR.x + 5, mainR.y + 55, sidebarW);
        float sx = mainR.x + sidebarW + 20; float sy = mainR.y + 60;
        GUI.color = Color.white; GUI.Label(new Rect(sx, sy, 300, 25), "ĐIỂM KỸ NĂNG: " + currentView.skillPoints, headerStyle);
        SkillData[] all = Resources.LoadAll<SkillData>("Skills");
        for (int i=0; i<all.Length; i++) {
            Rect skR = new Rect(sx, sy + 30 + (i*90), 650, 80); GUI.color = new Color(0.15f, 0.15f, 0.2f); GUI.DrawTexture(skR, Texture2D.whiteTexture);
            int sLv = currentView.GetSkillLevel(all[i].skillName); GUI.color = (sLv > 0) ? Color.green : Color.white;
            GUI.Label(new Rect(skR.x+80, skR.y+10, 400, 25), all[i].skillName + " (LV " + sLv + "/10)", new GUIStyle(GUI.skin.label){fontStyle=FontStyle.Bold, fontSize=14});
            if (sLv < 10 && currentView.skillPoints > 0) { GUI.color = Color.yellow; if (GUI.Button(new Rect(skR.xMax-100, skR.y+20, 80, 40), "NÂNG")) currentView.LearnSkill(all[i]); }
        }
    }

    void DrawHUD() {
        DrawSingleHUD(10, 10, player, "NGUOI CHOI", Color.red, true);
        for (int i = 0; i < companions.Count; i++) { if (i >= 4) break; DrawSingleHUD(10, 68 + (i * 58), companions[i], companions[i].characterName, Color.red, false); }
    }

    void DrawSingleHUD(float x, float y, PlayerStats st, string label, Color barCol, bool isPlayerHUD) {
        float hudW = 180f;
        float hudH = isPlayerHUD ? 58f : 52f;
        // Nen
        GUI.color = new Color(0, 0, 0, 0.82f); GUI.DrawTexture(new Rect(x, y, hudW, hudH), Texture2D.whiteTexture);
        // Thanh mau
        float barH = isPlayerHUD ? 13f : 11f;
        GUI.color = new Color(0.25f, 0, 0); GUI.DrawTexture(new Rect(x+3, y+3, hudW-6, barH), Texture2D.whiteTexture);
        float hpR = Mathf.Clamp01((float)st.currentHealth / st.maxHealth);
        GUI.color = barCol; GUI.DrawTexture(new Rect(x+3, y+3, (hudW-6) * hpR, barH), Texture2D.whiteTexture);
        // Thanh EXP
        GUI.color = new Color(0, 0.08f, 0.22f); GUI.DrawTexture(new Rect(x+3, y+3+barH+1, hudW-6, 4), Texture2D.whiteTexture);
        float expR = Mathf.Clamp01((float)st.currentExp / st.expToNextLevel);
        GUI.color = new Color(0.1f, 0.3f, 1f); GUI.DrawTexture(new Rect(x+3, y+3+barH+1, (hudW-6)*expR, 4), Texture2D.whiteTexture);
        // Text HP
        int fs = isPlayerHUD ? 10 : 9;
        string hpText = label + " [" + st.level + "]: " + st.currentHealth + "/" + st.maxHealth;
        DrawShadowLabel(new Rect(x+4, y+2, hudW-6, barH), hpText, new GUIStyle(GUI.skin.label){fontSize=fs}, Color.white);
        // Dong INFO
        if (isPlayerHUD) {
            GUI.color = Color.yellow;
            DrawShadowLabel(new Rect(x+4, y+barH+8, hudW-4, 16), "Vang: " + st.gold + "  [" + st.characterRank + "]", new GUIStyle(GUI.skin.label){fontSize=9}, Color.yellow);
        } else {
            GUI.color = Color.gray;
            DrawShadowLabel(new Rect(x+4, y+barH+7, hudW-52, 14), "[" + st.characterRank + "]", new GUIStyle(GUI.skin.label){fontSize=9}, Color.gray);
            if (GUI.Button(new Rect(x+hudW-46, y+barH+5, 42, 16), "XEM", new GUIStyle(GUI.skin.button){fontSize=9})) hudClickedCompanion = st;
        }
    }

    void DrawSkillIcon(float x, float y, SkillData skill, float cd) {
        if (skill == null) return; Rect r = new Rect(x, y, 24, 24); GUI.color = new Color(0.1f, 0.1f, 0.1f, 0.8f); GUI.DrawTexture(r, Texture2D.whiteTexture);
        if (skill.icon != null) { GUI.color = (cd > 0) ? Color.gray : Color.white; GUI.DrawTexture(r, skill.icon.texture); }
    }

    void DrawBuffIcon(float x, float y, string buffName) {
        Rect r = new Rect(x, y, 24, 24); GUI.color = Color.blue; GUI.DrawTexture(r, Texture2D.whiteTexture); GUI.color = Color.white; GUI.Label(r, buffName.Length > 0 ? buffName.Substring(0, 1) : "✨");
    }

    void DrawShadowLabel(Rect r, string txt, GUIStyle style, Color mainCol) {
        GUI.color = new Color(0,0,0,0.8f); GUI.Label(new Rect(r.x+1, r.y+1, r.width, r.height), txt, style); GUI.color = mainCol; GUI.Label(r, txt, style); GUI.color = Color.white;
    }

    void DrawFooter(Rect r) {
        if (player == null) return; GUI.color = Color.green; if (GUI.Button(new Rect(r.x + 15, r.yMax - 40, 110, 30), "💾 LƯU")) player.SaveGame();
        GUI.color = Color.red; if (GUI.Button(new Rect(r.x + 130, r.yMax - 40, 110, 30), "🗑 NEW")) player.ResetGame();
    }

    bool DrawSlot(float x, float y, ItemInstance inst, bool s, string defaultLabel) {
        GUI.color = s ? Color.yellow : new Color(0.2f, 0.2f, 0.3f); GUI.DrawTexture(new Rect(x,y,68,68), Texture2D.whiteTexture);
        bool clicked = false;
        if (inst != null && inst.data != null) {
            if (inst.data.icon != null) { GUI.color = Color.white; clicked = GUI.Button(new Rect(x+2, y+2, 64, 64), inst.data.icon.texture); }
            else { GUI.color = GetItemTypeColor(inst.data.type); GUI.DrawTexture(new Rect(x+2, y+2, 64, 64), Texture2D.whiteTexture); GUI.color = Color.white; GUI.Label(new Rect(x+2, y+18, 64, 28), GetItemTypeIcon(inst.data.type)); clicked = GUI.Button(new Rect(x+2, y+2, 64, 64), "", GUIStyle.none); }
            GUI.color = Color.white; GUI.Label(new Rect(x, y+50, 68, 18), inst.GetDisplayName(), slotLabelStyle);
        } else { GUI.color = Color.white; clicked = GUI.Button(new Rect(x+2, y+2, 64, 64), defaultLabel, slotLabelStyle); }
        return clicked;
    }

    void DrawFloatingDamage() {
        Camera c = Camera.main; if (c == null) return;
        foreach (var d in damageTexts) { Vector3 p = c.WorldToScreenPoint(d.worldPos); if (p.z > 0) { GUI.color = new Color(d.color.r, d.color.g, d.color.b, d.alpha); GUI.Label(new Rect(p.x-50, Screen.height-p.y-25, 100, 30), d.text, dmgStyle); } }
    }
}