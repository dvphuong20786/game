using UnityEngine;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// ===========================
// GIAO DIỆN NGƯỜI CHƠI (NÂNG CẤP RANK & ITEM INSTANCE)
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

    private PlayerStats player;
    private List<PlayerStats> companions = new List<PlayerStats>();
    private int companionViewIdx = -1; 
    private PlayerStats currentView; 
    private PlayerCombat combat;

    private bool isBagOpen = false;
    private int currentTab = 0; 
    private int selectedItemIdx = -1;
    private string selectedSlot = "";
    private Vector2 scrollPos;

    void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    void Start() { TryFindPlayer(); }

    void TryFindPlayer()
    {
        PlayerStats[] allStats = Object.FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
        
        // 1. Tìm người chơi chính thức
        if (player == null) {
            foreach(var s in allStats) if(s.isPlayer) { player = s; break; }
        }

        // 2. FALLBACK: Nếu vẫn không thấy, tự nhận vật thể PlayerStats đầu tiên làm người chơi để cứu vãn UI
        if (player == null && allStats.Length > 0) {
            player = allStats[0];
            Debug.LogWarning("⚠️ [GAMEUI]: Không thấy nhân vật nào được tích 'Is Player'. Đã tự động chọn " + player.characterName + " làm người chơi.");
        }

        // 3. Cảnh báo đỏ nếu tuyệt nhiên không có script PlayerStats nào
        if (player == null && Time.frameCount % 100 == 0) {
            Debug.LogError("‼️ [GAMEUI ERROR]: Không thấy bất kỳ script PlayerStats nào trong Scene! Vui lòng kéo script vào nhân vật HiepSi.");
        }

        companions.Clear();
        foreach(var s in allStats) if(s != player) { companions.Add(s); }
        
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
    }

    void ResetSelection() { selectedItemIdx = -1; selectedSlot = ""; }

    void InitStyles()
    {
        if (dmgStyle == null) dmgStyle = new GUIStyle(GUI.skin.label) { fontSize = 22, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        if (expStyle == null) expStyle = new GUIStyle(GUI.skin.label) { fontSize = 13, fontStyle = FontStyle.Bold };
        if (goldStyle == null) goldStyle = new GUIStyle(GUI.skin.label) { fontSize = 15, fontStyle = FontStyle.Bold };
        if (headerStyle == null) headerStyle = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        if (slotLabelStyle == null) { slotLabelStyle = new GUIStyle(GUI.skin.label) { fontSize = 10, alignment = TextAnchor.MiddleCenter, wordWrap = true }; slotLabelStyle.normal.textColor = Color.white; }
        if (statValueStyle == null) { statValueStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, fontStyle = FontStyle.Bold }; }
    }

    void OnGUI()
    {
        InitStyles();
        DrawFloatingDamage();
        GUI.depth = 0;

        if (player == null) { TryFindPlayer(); if(player == null) return; }

        DrawHUD();

        if (isBagOpen)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            GUI.color = new Color(0, 0, 0, 0.5f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            
            float uiW = Mathf.Min(860, Screen.width * 0.95f);
            float uiH = Mathf.Min(530, Screen.height * 0.9f);
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
        float tabY = mainR.y + 10;
        GUI.color = currentTab == 0 ? new Color(0.3f, 0.6f, 1f) : new Color(0.2f, 0.2f, 0.3f);
        if (GUI.Button(new Rect(mainR.x + 15, tabY, 130, 32), "🛡 TRANG BỊ")) currentTab = 0;
        GUI.color = currentTab == 1 ? new Color(1f, 0.6f, 0.1f) : new Color(0.2f, 0.2f, 0.3f);
        if (GUI.Button(new Rect(mainR.x + 155, tabY, 130, 32), "⚡ KỸ NĂNG")) currentTab = 1;

        if (companions.Count > 0)
        {
            string btnText = (currentView == player) ? "🐕 ĐỆ TỬ" : "🐕 TIẾP THEO";
            if (GUI.Button(new Rect(mainR.xMax - 180, tabY, 120, 32), btnText)) {
                companionViewIdx++; if (companionViewIdx >= companions.Count) companionViewIdx = -1;
                currentView = (companionViewIdx == -1) ? player : companions[companionViewIdx];
                ResetSelection();
            }
        }
    }

    void DrawCharacterTab(Rect mainR)
    {
        if (currentView == null) return;
        float sidebarW = 160;
        float pdX = mainR.x + sidebarW + 10; float pdY = mainR.y + 55;

        DrawSidebar(mainR.x + 5, pdY, sidebarW);

        // --- CHI TIẾT NHÂN VẬT & RANK ---
        GUI.color = new Color(0.12f, 0.12f, 0.22f, 1f);
        GUI.DrawTexture(new Rect(pdX, pdY, 240, 440), Texture2D.whiteTexture);
        GUI.color = Color.white; 
        GUI.Label(new Rect(pdX, pdY+5, 240, 25), $"{currentView.characterName} [RANK {currentView.characterRank}]", headerStyle);

        float sX = pdX + 15;
        if (DrawSlot(sX, pdY+35,  currentView.eqHead, selectedSlot=="Head", "Đầu")) {selectedSlot="Head"; selectedItemIdx=-1;}
        if (DrawSlot(sX+70, pdY+35,  currentView.eqNecklace, selectedSlot=="Neck", "Dây")) {selectedSlot="Neck"; selectedItemIdx=-1;}
        if (DrawSlot(sX+140, pdY+35, currentView.eqAncientGold, selectedSlot=="Ancient", "Vàng")) {selectedSlot="Ancient"; selectedItemIdx=-1;}
        
        if (DrawSlot(sX, pdY+105, currentView.eqWeaponMain, selectedSlot=="WepMain", "Vũ Khí")) {selectedSlot="WepMain"; selectedItemIdx=-1;}
        if (DrawSlot(sX+70, pdY+105, currentView.eqBody, selectedSlot=="Body", "Áo")) {selectedSlot="Body"; selectedItemIdx=-1;}
        if (DrawSlot(sX+140, pdY+105, currentView.eqWeaponOff, selectedSlot=="WepOff", "Khiên")) {selectedSlot="WepOff"; selectedItemIdx=-1;}
        
        if (DrawSlot(sX, pdY+175, currentView.eqRing1, selectedSlot=="Ring1", "Nhẫn 1")) {selectedSlot="Ring1"; selectedItemIdx=-1;}
        if (DrawSlot(sX+70, pdY+175, currentView.eqLegs, selectedSlot=="Legs", "Giày")) {selectedSlot="Legs"; selectedItemIdx=-1;}
        if (DrawSlot(sX+140, pdY+175, currentView.eqRing2, selectedSlot=="Ring2", "Nhẫn 2")) {selectedSlot="Ring2"; selectedItemIdx=-1;}

        // Chỉ số CHIẾN ĐẤU thực tế (Hạ thấp xuống pdY+245 để tránh đè lên trang bị)
        float statY = pdY + 245;
        GUIStyle statS = new GUIStyle(GUI.skin.label) { fontSize = 13, fontStyle = FontStyle.Bold };
        GUI.color = Color.white;
        DrawShadowLabel(new Rect(pdX + 15, statY, 210, 22), $"⚔ Sức mạnh: {currentView.bonusDamage}", statS, Color.white);
        DrawShadowLabel(new Rect(pdX + 15, statY + 28, 210, 22), $"🛡 Phòng ngự: {currentView.bonusDefense}", statS, Color.white);
        DrawShadowLabel(new Rect(pdX + 15, statY + 56, 210, 22), $"❤ Sinh mệnh: {currentView.currentHealth}/{currentView.maxHealth}", statS, new Color(1f, 0.4f, 0.4f));
        
        // Nâng cấp thuộc tính (Đẩy xuống pdY+335 cho thoáng)
        float stY = pdY + 335;
        GUI.color = Color.yellow; 
        DrawShadowLabel(new Rect(pdX+15, stY, 220, 22), "ĐIỂM TIỀM NĂNG: " + currentView.statPoints, statValueStyle, Color.yellow);
        DrawStatRow(pdX+15, stY+28, "Sức mạnh (STR)", currentView.STR, "STR");
        DrawStatRow(pdX+15, stY+56, "Thể chất (VIT)", currentView.VIT, "VIT");
        DrawStatRow(pdX+15, stY+84, "Nhanh nhẹn (AGI)", currentView.AGI, "AGI");

        DrawInventory(pdX + 250, pdY);
        DrawTooltip(mainR.xMax - 210, pdY);
    }

    void DrawSidebar(float x, float y, float w) {
        GUI.color = new Color(0.12f, 0.12f, 0.18f, 0.98f);
        GUI.DrawTexture(new Rect(x, y, w, 440), Texture2D.whiteTexture);
        GUI.color = Color.white; GUI.Label(new Rect(x, y+5, w, 25), "👥 ĐỘI NGŨ", headerStyle);
        float btnY = y + 40;
        
        // Button Hiệp sĩ
        bool isP = (currentView == player);
        GUI.color = isP ? new Color(0.3f, 0.6f, 1f) : Color.white;
        if (GUI.Button(new Rect(x+8, btnY, w-16, 45), (isP?"👑 ":"") + "HIỆP SĨ")) { currentView = player; ResetSelection(); }
        btnY += 50;
        
        // Danh sách đệ tử
        foreach(var c in companions) {
            if (c == null) continue;
            bool isS = (currentView == c);
            GUI.color = isS ? new Color(0.3f, 0.8f, 0.4f) : Color.white;
            if (GUI.Button(new Rect(x+8, btnY, w-16, 45), (isS?"🔹 ":"") + c.characterName)) { currentView = c; ResetSelection(); }
            btnY += 50;
        }

        // Nút ĐỘT PHÁ (Đặt ở cuối sidebar rất chuyên nghiệp)
        if (currentView != null && currentView.characterRank != "S") {
            GUI.color = new Color(0.1f, 0.3f, 0.3f, 0.8f);
            GUI.DrawTexture(new Rect(x+5, y+340, w-10, 90), Texture2D.whiteTexture);
            GUI.color = Color.cyan;
            if (GUI.Button(new Rect(x+15, y + 350, w-30, 40), "✨ ĐỘT PHÁ RANK", new GUIStyle(GUI.skin.button){fontSize=12, fontStyle=FontStyle.Bold})) 
                currentView.PromoteCharacter();
            
            // Hiển thị yêu cầu (Mới)
            GUIStyle pS = new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter, fontSize=11, fontStyle=FontStyle.Italic};
            string soulReq = "Linh hồn " + (currentView == player ? "Hiệp sĩ" : currentView.characterName);
            GUI.color = Color.yellow;
            GUI.Label(new Rect(x+10, y + 392, w-20, 20), $"Yêu cầu: {soulReq}", pS);
            GUI.color = new Color(1f, 0.8f, 0.2f);
            GUI.Label(new Rect(x+10, y + 410, w-20, 20), $"Phí: {currentView.GetPromoteCost()} Vàng", pS);
        }
    }

    void DrawStatRow(float x, float y, string label, int val, string statKey) {
        GUI.color = Color.white; GUI.Label(new Rect(x, y, 150, 25), label + ": " + val);
        if (currentView.statPoints > 0) {
            GUI.color = Color.green; if (GUI.Button(new Rect(x + 160, y, 30, 20), "+")) currentView.AddStat(statKey);
        }
    }

    void DrawInventory(float x, float y) {
        GUI.color = Color.white; GUI.Label(new Rect(x, y, 120, 25), "📦 HÒM ĐỒ CHUNG", headerStyle);
        Rect vR = new Rect(x, y+35, 235, 405);
        List<ItemInstance> inv = currentView.SharedInventory; // Dùng túi đồ chung
        Rect cR = new Rect(0,0, 210, Mathf.Max(412, inv.Count * 42));
        scrollPos = GUI.BeginScrollView(vR, scrollPos, cR);
        for (int i=0; i<inv.Count; i++) {
            if (inv[i] == null || inv[i].data == null) continue;
            bool sel = (selectedItemIdx == i);
            GUI.color = sel ? new Color(0.3f, 0.5f, 0.2f) : new Color(0.15f, 0.15f, 0.25f);
            GUI.DrawTexture(new Rect(0, i*42, 210, 38), Texture2D.whiteTexture);
            if (inv[i].data.icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(5, i*42+3, 32, 32), inv[i].data.icon.texture); }
            GUI.color = sel ? Color.yellow : Color.white;
            if (GUI.Button(new Rect(40, i*42, 170, 38), inv[i].GetDisplayName(), GUI.skin.label)) { selectedItemIdx = i; selectedSlot = ""; }
        }
        GUI.EndScrollView();
    }

    void DrawTooltip(float x, float y) {
        GUI.color = new Color(0.1f, 0.15f, 0.3f, 1f); GUI.DrawTexture(new Rect(x,y, 200, 412), Texture2D.whiteTexture);
        ItemInstance inst = null; bool fromInv = false;
        List<ItemInstance> sharedInv = currentView.SharedInventory;
        if (selectedItemIdx >= 0 && selectedItemIdx < sharedInv.Count) { inst = sharedInv[selectedItemIdx]; fromInv = true; }
        else if (selectedSlot != "") {
            switch(selectedSlot) {
                case "Head": inst=currentView.eqHead; break; case "Body": inst=currentView.eqBody; break; case "Legs": inst=currentView.eqLegs; break;
                case "WepMain": inst=currentView.eqWeaponMain; break; case "WepOff": inst=currentView.eqWeaponOff; break;
                case "Ring1": inst=currentView.eqRing1; break; case "Ring2": inst=currentView.eqRing2; break;
                case "Neck": inst=currentView.eqNecklace; break; case "Ancient": inst=currentView.eqAncientGold; break;
            }
        }
        if (inst == null || inst.data == null) return;
        var d = inst.data;
        GUI.color = Color.yellow; GUI.Label(new Rect(x+10, y+10, 180, 45), inst.GetDisplayName(), new GUIStyle(GUI.skin.label){fontSize=12, fontStyle=FontStyle.Bold, wordWrap=true});
        GUI.color = Color.white;
        string s = d.description + $"\n⚔ Tấn công: +{inst.GetTotalAtk()}\n🛡 Phòng thủ: +{inst.GetTotalDef()}\n❤ HP: +{inst.GetTotalHP()}";
        if (inst.sockets.Count > 0) { s += "\n💎 NGỌC:"; foreach(var g in inst.sockets) s += "\n - " + g.itemName; }
        GUI.Label(new Rect(x+10, y+60, 180, 200), s, new GUIStyle(GUI.skin.label){fontSize=11, wordWrap=true});

        if (fromInv) {
            if (d.type == ItemData.ItemType.Weapon || d.type == ItemData.ItemType.Armor || d.type == ItemData.ItemType.Accessory) {
                GUI.color = Color.green; if (GUI.Button(new Rect(x+10, y+230, 180, 25), "🛡 MẶC ĐỒ")) { currentView.EquipItem(selectedItemIdx); ResetSelection(); }
                GUI.color = Color.cyan; if (GUI.Button(new Rect(x+10, y+260, 180, 25), "🔨 THỢ RÈN (+)")) RPG_BlacksmithLogic.EnhanceItem(currentView, selectedItemIdx);
            } else if (d.type == ItemData.ItemType.Consumable) {
                GUI.color = Color.yellow; if (GUI.Button(new Rect(x+10, y+230, 180, 25), "🧪 SỬ DỤNG")) { currentView.UseConsumable(selectedItemIdx); ResetSelection(); }
            } else if (d.type == ItemData.ItemType.Gem) {
                GUI.color = Color.magenta; GUI.Label(new Rect(x+10, y+230, 180, 45), "💎 Mẹo: Chọn trang bị trong rương hoặc đang mặc, sau đó nhấn khảm viên này.", new GUIStyle(GUI.skin.label){fontSize=10, wordWrap=true, fontStyle=FontStyle.Italic});
                // Thêm nút khảm nếu có đồ đang được chọn ở slot hoặc vị trí khác
                if (selectedSlot != "") {
                    GUI.color = Color.white;
                    if (GUI.Button(new Rect(x+10, y+280, 180, 30), "➡ KHẢM VÀO " + selectedSlot.ToUpper())) {
                         RPG_BlacksmithLogic.SocketIntoSlot(currentView, selectedSlot, selectedItemIdx);
                         ResetSelection();
                    }
                }
            }
            GUI.color = Color.red; if (GUI.Button(new Rect(x+10, y+345, 180, 25), "💸 BÁN: +" + d.price + "G")) { currentView.SellItem(selectedItemIdx); ResetSelection(); }
        } else {
            GUI.color = new Color(1f, 0.5f, 0f); if (GUI.Button(new Rect(x+10, y+310, 180, 30), "⚔ GỠ RA")) { currentView.Unequip(selectedSlot); ResetSelection(); }
        }
    }

    void DrawSkillsTab(Rect mainR) {
        float sidebarW = 160; DrawSidebar(mainR.x + 5, mainR.y + 55, sidebarW);
        float sx = mainR.x + sidebarW + 20; float sy = mainR.y + 60;
        GUI.color = Color.white; GUI.Label(new Rect(sx, sy, 300, 25), "ĐIỂM KỸ NĂNG: " + currentView.skillPoints, headerStyle);
        SkillData[] all = Resources.LoadAll<SkillData>("Skills");
        for (int i=0; i<all.Length; i++) {
            Rect skR = new Rect(sx, sy + 30 + (i*90), 650, 80);
            GUI.color = new Color(0.15f, 0.15f, 0.2f); GUI.DrawTexture(skR, Texture2D.whiteTexture);
            int sLv = currentView.GetSkillLevel(all[i].skillName);
            if (all[i].icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(skR.x+10, skR.y+10, 60, 60), all[i].icon.texture); }
            GUI.color = (sLv > 0) ? Color.green : Color.white;
            GUI.Label(new Rect(skR.x+80, skR.y+10, 400, 25), all[i].skillName + " (LV " + sLv + "/10)", new GUIStyle(GUI.skin.label){fontStyle=FontStyle.Bold, fontSize=14});
            
            // Hiện mô tả và sức mạnh (Mới)
            string desc = all[i].GetDescription(sLv);
            string stats = $"\n⏱ Hồi chiêu: {all[i].baseCooldown}s";
            
            // Hiện thông tin Dame/Heal
            if (all[i].baseDamageMultiplier > 0 && (all[i].skillName.Contains("Chém") || all[i].skillName.Contains("Lôi"))) {
                float curMult = all[i].baseDamageMultiplier + (sLv * all[i].damageIncreasePerLevel);
                stats += $" | 💥 Sát thương: {curMult*100:F0}%";
            }
            if (all[i].baseHealOrDef > 0) {
                int curVal = all[i].baseHealOrDef + (sLv * all[i].valueIncreasePerLevel);
                string label = all[i].skillName.Contains("Hộ Vệ") ? "🛡 Phòng thủ" : "✨ Hiệu lực";
                stats += $" | {label}: {curVal}";
            }
            GUI.color = new Color(0.8f, 0.8f, 0.8f);
            GUI.Label(new Rect(skR.x+80, skR.y+35, 450, 45), desc + stats, new GUIStyle(GUI.skin.label){fontSize=11, wordWrap=true});

            if (sLv < 10 && currentView.skillPoints > 0) {
                GUI.color = Color.yellow; if (GUI.Button(new Rect(skR.xMax-100, skR.y+20, 80, 40), (sLv==0?"HỌC":"NÂNG"))) currentView.LearnSkill(all[i]);
            }
        }
    }

    void DrawHUD() {
        DrawSingleHUD(10, 10, player, "NGƯỜI CHƠI", Color.red);
        for (int i = 0; i < companions.Count; i++) {
            if (i >= 4) break; 
            DrawSingleHUD(10, 120 + (i * 110), companions[i], companions[i].characterName, Color.red);
        }
    }

    void DrawSingleHUD(float x, float y, PlayerStats st, string label, Color barCol) {
        // Nền HUD đen đặc để tách biệt với thế giới
        GUI.color = new Color(0,0,0,0.98f); 
        GUI.DrawTexture(new Rect(x, y, 260, 100), Texture2D.whiteTexture);
        
        // 1. Thanh Máu (HP)
        GUI.color = new Color(0.2f,0,0); 
        GUI.DrawTexture(new Rect(x+5, y+5, 250, 20), Texture2D.whiteTexture);
        float hpR = Mathf.Clamp01((float)st.currentHealth / st.maxHealth);
        GUI.color = barCol; 
        GUI.DrawTexture(new Rect(x+5, y+5, 250 * hpR, 20), Texture2D.whiteTexture);
        
        // 2. Thanh Kinh nghiệm (EXP)
        GUI.color = new Color(0,0.1f,0.2f); 
        GUI.DrawTexture(new Rect(x+5, y+30, 250, 8), Texture2D.whiteTexture);
        float expR = Mathf.Clamp01((float)st.currentExp / st.expToNextLevel);
        GUI.color = new Color(0.2f, 0.6f, 1f); 
        GUI.DrawTexture(new Rect(x+5, y+30, 250 * expR, 8), Texture2D.whiteTexture);

        // Văn bản có bóng (Shadow) cho dễ đọc
        // 1. Vẽ thanh máu và tên
        string labelWithLevel = $"{label} [LV {st.level}]";
        string hpText = labelWithLevel + ": " + st.currentHealth + "/" + st.maxHealth;
        GUIStyle hpStyle = new GUIStyle(GUI.skin.label){fontSize=13, fontStyle=FontStyle.Bold};
        DrawShadowLabel(new Rect(x+10, y+4, 240, 20), hpText, hpStyle, Color.white);
        
        GUIStyle infoStyle = new GUIStyle(GUI.skin.label){fontSize=12, fontStyle=FontStyle.Bold};
        string infoText = "💰 " + st.gold + " Vàng  [RANK " + st.characterRank + "]";
        DrawShadowLabel(new Rect(x+5, y+60, 240, 25), infoText, infoStyle, Color.yellow);

        // ==========================================
        // 3. HIỂN THỊ KỸ NĂNG & COOLDOWN (DÀNH CHO NGƯỜI CHƠI & ĐỆ TỬ)
        // ==========================================
        CompanionAI ai = st.GetComponent<CompanionAI>();
        PlayerCombat pc = st.GetComponent<PlayerCombat>();
        float skillX = x + 5;
        float skillY = y + 42;

        if (ai != null) {
            DrawSkillIcon(skillX, skillY, ai.skillHoVe, ai.GetCooldown("HoVe"));
            DrawSkillIcon(skillX + 32, skillY, ai.skillTriThuong, ai.GetCooldown("TriThuong"));
        }
        else if (pc != null) {
            for (int i = 0; i < 4; i++) {
                if (i < pc.equippedSkills.Length) {
                    DrawSkillIcon(skillX + (i * 32), skillY, pc.equippedSkills[i], pc.GetSkillCooldown(i));
                }
            }
        }

        // ==========================================
        // 4. HIỂN THỊ BUFF (BÊN PHẢI HUD)
        // ==========================================
        float buffX = x + 265;
        float buffY = y + 5;
        foreach(var buff in st.activeBuffs) {
            DrawBuffIcon(buffX, buffY, buff);
            buffX += 32;
            if (buffX > x + 350) { buffX = x + 265; buffY += 32; } // Xuống dòng nếu quá nhiều buff
        }
    }

    void DrawSkillIcon(float x, float y, SkillData skill, float cd) {
        if (skill == null) return;
        Rect r = new Rect(x, y, 28, 28);
        GUI.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        GUI.DrawTexture(r, Texture2D.whiteTexture);
        
        if (skill.icon != null) {
            GUI.color = (cd > 0) ? new Color(0.4f, 0.4f, 0.4f, 0.6f) : Color.white;
            GUI.DrawTexture(r, skill.icon.texture);
        }
        
        if (cd > 0) {
            GUI.color = Color.white;
            GUIStyle cdS = new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter, fontSize=10, fontStyle=FontStyle.Bold};
            GUI.Label(r, Mathf.Ceil(cd).ToString(), cdS);
        }
        GUI.color = Color.white;
    }

    void DrawBuffIcon(float x, float y, string buffName) {
        Rect r = new Rect(x, y, 28, 28);
        GUI.color = new Color(0, 0.2f, 0.4f, 0.9f);
        GUI.DrawTexture(r, Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUIStyle bS = new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter, fontSize=14};
        // Lấy ký tự đầu (thường là emoji) để làm icon nếu không có ảnh
        string icon = buffName.Length > 0 ? buffName.Substring(0, 1) : "✨";
        GUI.Label(r, icon, bS);
    }

    void DrawShadowLabel(Rect r, string txt, GUIStyle style, Color mainCol) {
        GUI.color = new Color(0,0,0,0.8f);
        GUI.Label(new Rect(r.x+1, r.y+1, r.width, r.height), txt, style);
        GUI.color = mainCol;
        GUI.Label(r, txt, style);
        GUI.color = Color.white;
    }

    void DrawFooter(Rect r) {
        if (player == null) return;
        GUI.color = Color.green; if (GUI.Button(new Rect(r.x + 15, r.yMax - 45, 110, 32), "💾 LƯU")) player.SaveGame();
        GUI.color = Color.red; if (GUI.Button(new Rect(r.x + 130, r.yMax - 45, 110, 32), "🗑 NEW")) player.ResetGame();
    }

    bool DrawSlot(float x, float y, ItemInstance inst, bool s, string defaultLabel) {
        GUI.color = s ? Color.yellow : new Color(0.2f, 0.2f, 0.3f); GUI.DrawTexture(new Rect(x,y,68,68), Texture2D.whiteTexture);
        bool clicked = false;
        if (inst != null && inst.data != null) {
            GUI.color = Color.white; clicked = GUI.Button(new Rect(x+2, y+2, 64, 64), inst.data.icon != null ? inst.data.icon.texture : null);
            GUI.Label(new Rect(x, y+45, 68, 18), inst.GetDisplayName(), slotLabelStyle);
        } else {
            GUI.color = Color.white; clicked = GUI.Button(new Rect(x+2, y+2, 64, 64), defaultLabel, slotLabelStyle);
        }
        return clicked;
    }

    void DrawFloatingDamage() {
        Camera c = Camera.main; if (c == null) return;
        foreach (var d in damageTexts) {
            Vector3 p = c.WorldToScreenPoint(d.worldPos);
            if (p.z > 0) { GUI.color = new Color(d.color.r, d.color.g, d.color.b, d.alpha); GUI.Label(new Rect(p.x-50, Screen.height-p.y-25, 100, 30), d.text, dmgStyle); }
        }
    }
}
