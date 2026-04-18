using UnityEngine;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

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

    // ===== Refs =====
    private PlayerStats player;
    private PlayerStats companion;
    private PlayerStats currentView; 
    private PlayerCombat combat;

    // ===== Trạng thái UI =====
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
        if (player == null) foreach(var s in allStats) if(s.isPlayer) { player = s; break; }
        if (companion == null) foreach(var s in allStats) if(!s.isPlayer) { companion = s; break; }
        if (combat == null) combat = FindObjectOfType<PlayerCombat>();
        if (player == null || companion == null || combat == null)
        {
            allStats = Object.FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
            foreach (var s in allStats)
            {
                if (s.isPlayer) player = s;
                else companion = s;
            }
            if (combat == null) combat = FindObjectOfType<PlayerCombat>();
        }
        
        // ĐẢM BẢO LUÔN CÓ NHÂN VẬT ĐỂ XEM
        if (currentView == null) {
            if (player != null) currentView = player;
            else if (companion != null) currentView = companion;
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
        if (Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame)
        { isBagOpen = !isBagOpen; ResetSelection(); }
#else
        if (Input.GetKeyDown(KeyCode.B)) { isBagOpen = !isBagOpen; ResetSelection(); }
#endif
        for (int i = damageTexts.Count - 1; i >= 0; i--)
        {
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

        // Đảm bảo túi đồ luôn nằm trên cùng để bấm được
        GUI.depth = 0;

        // Kiểm tra Player để hiện HUD
        if (player == null) {
             TryFindPlayer();
             if(player == null) {
                 GUI.color = Color.red;
                 GUI.Label(new Rect(10, 10, 500, 30), "⚠️ LỖI: Không tìm thấy nhân vật chính (HiepSi). Hãy kiểm tra ô 'Is Player'!");
                 return;
             }
        }

        DrawHUD();

        if (isBagOpen)
        {
            // GIẢI PHÓNG CHUỘT KHI MỞ TÚI ĐỒ
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            GUI.color = new Color(0, 0, 0, 0.5f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            
            // TÍNH TOÁN BẢNG TRANG BỊ CO DÃN (RESPONSIVE)
            float uiW = Mathf.Min(860, Screen.width * 0.95f);
            float uiH = Mathf.Min(530, Screen.height * 0.9f);
            Rect mainR = new Rect(Screen.width / 2 - uiW / 2, Screen.height / 2 - uiH / 2, uiW, uiH);

            GUI.color = new Color(0.08f, 0.08f, 0.15f, 0.98f);
            GUI.DrawTexture(mainR, Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUI.color = new Color(0.4f, 0.4f, 0.8f, 1f);
            GUI.DrawTexture(new Rect(mainR.x, mainR.y, mainR.width, 2), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(mainR.x, mainR.yMax-2, mainR.width, 2), Texture2D.whiteTexture);
            GUI.color = new Color(0.8f, 0.1f, 0.1f, 1f);
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

        if (companion != null)
        {
            bool viewingPlayer = (currentView == player);
            GUI.color = viewingPlayer ? Color.white : Color.cyan;
            string btnText = viewingPlayer ? "🕹 XEM NGƯỜI CHƠI" : "🐕 XEM ĐỆ TỬ";
            if (GUI.Button(new Rect(mainR.xMax - 250, tabY, 180, 32), btnText))
            {
                currentView = viewingPlayer ? companion : player;
                ResetSelection();
                // Focus camera vào nhân vật đang xem
                if (CameraFollow.instance != null) CameraFollow.instance.SetTarget(currentView.transform);
            }
        }
    }

    void DrawCharacterTab(Rect mainR)
    {
        if (currentView == null) return;
        float pdX = mainR.x + 10; float pdY = mainR.y + 55;
        GUI.color = currentView.isPlayer ? new Color(0.12f, 0.12f, 0.22f, 1f) : new Color(0.1f, 0.22f, 0.22f, 1f);
        GUI.DrawTexture(new Rect(pdX, pdY, 250, 440), Texture2D.whiteTexture);
        GUI.color = Color.white; GUI.Label(new Rect(pdX+50, pdY+5, 160, 25), currentView.characterName.ToUpper(), headerStyle);

        if (DrawSlot(pdX+20, pdY+35,  "Đầu\n"+currentView.eqHead, selectedSlot=="Head")) {selectedSlot="Head"; selectedItemIdx=-1;}
        if (DrawSlot(pdX+95, pdY+35,  "Dây\n"+currentView.eqNecklace, selectedSlot=="Neck")) {selectedSlot="Neck"; selectedItemIdx=-1;}
        if (DrawSlot(pdX+170, pdY+35, "Vàng\n"+currentView.eqAncientGold, selectedSlot=="Ancient")) {selectedSlot="Ancient"; selectedItemIdx=-1;}
        if (DrawSlot(pdX+20, pdY+110, "Vũ khí\n"+currentView.eqWeaponMain, selectedSlot=="WepMain")) {selectedSlot="WepMain"; selectedItemIdx=-1;}
        if (DrawSlot(pdX+95, pdY+110, "Áo\n"+currentView.eqBody, selectedSlot=="Body")) {selectedSlot="Body"; selectedItemIdx=-1;}
        if (DrawSlot(pdX+170, pdY+110, "Khiên\n"+currentView.eqWeaponOff, selectedSlot=="WepOff")) {selectedSlot="WepOff"; selectedItemIdx=-1;}
        if (DrawSlot(pdX+20, pdY+185, "Nhẫn 1\n"+currentView.eqRing1, selectedSlot=="Ring1")) {selectedSlot="Ring1"; selectedItemIdx=-1;}
        if (DrawSlot(pdX+95, pdY+185, "Giày\n"+currentView.eqLegs, selectedSlot=="Legs")) {selectedSlot="Legs"; selectedItemIdx=-1;}
        if (DrawSlot(pdX+170, pdY+185, "Nhẫn 2\n"+currentView.eqRing2, selectedSlot=="Ring2")) {selectedSlot="Ring2"; selectedItemIdx=-1;}

        // Điểm tiềm năng & Chỉ số
        float stY = pdY + 265;
        GUI.color = Color.yellow; GUI.Label(new Rect(pdX+10, stY, 230, 25), "ĐIỂM TIỀM NĂNG: " + currentView.statPoints, statValueStyle);
        
        DrawStatRow(pdX+10, stY+25, "Sức mạnh (STR)", currentView.STR, "STR");
        DrawStatRow(pdX+10, stY+55, "Thể chất (VIT)", currentView.VIT, "VIT");
        DrawStatRow(pdX+10, stY+85, "Nhanh nhẹn (AGI)", currentView.AGI, "AGI");

        int totalAtk = (currentView.isPlayer && combat != null ? combat.attackDamage : 15) + currentView.bonusDamage;
        GUI.color = new Color(1f, 0.5f, 0.3f); GUI.Label(new Rect(pdX+10, stY+115, 235, 22), "⚔ Tấn công: " + totalAtk);
        GUI.color = new Color(0.4f, 0.8f, 1f); GUI.Label(new Rect(pdX+10, stY+135, 235, 22), "🛡 Phòng thủ: " + currentView.bonusDefense);
        GUI.color = new Color(0.4f, 1f, 0.4f); GUI.Label(new Rect(pdX+10, stY+155, 235, 22), "❤ HP tối đa: " + currentView.maxHealth);

        DrawInventory(mainR.x + 270, pdY);
        DrawTooltip(mainR.x + 620, pdY);
    }

    void DrawStatRow(float x, float y, string label, int val, string statKey)
    {
        GUI.color = Color.white;
        GUI.Label(new Rect(x, y, 150, 25), label + ": " + val);
        if (currentView.statPoints > 0)
        {
            GUI.color = Color.green;
            if (GUI.Button(new Rect(x + 180, y, 30, 22), "+")) { currentView.AddStat(statKey); }
        }
    }

    void DrawInventory(float x, float y)
    {
        GUI.color = Color.white; GUI.Label(new Rect(x, y, 120, 25), "HÒM ĐỒ", headerStyle);
        Rect vR = new Rect(x, y+28, 340, 412);
        Rect cR = new Rect(0,0, 320, Mathf.Max(412, currentView.inventory.Count * 38));
        scrollPos = GUI.BeginScrollView(vR, scrollPos, cR);
        for (int i=0; i<currentView.inventory.Count; i++) {
            bool sel = (selectedItemIdx == i);
            GUI.color = sel ? new Color(0.3f, 0.5f, 0.2f) : new Color(0.15f, 0.15f, 0.25f);
            GUI.DrawTexture(new Rect(0, i*38, 320, 34), Texture2D.whiteTexture);
            GUI.color = sel ? Color.yellow : Color.white;
            if (GUI.Button(new Rect(0, i*38, 320, 34), currentView.inventory[i], GUI.skin.label)) { selectedItemIdx = i; selectedSlot = ""; }
        }
        GUI.EndScrollView();
    }

    void DrawTooltip(float x, float y)
    {
        GUI.color = new Color(0.1f, 0.15f, 0.3f, 1f); GUI.DrawTexture(new Rect(x,y, 220, 412), Texture2D.whiteTexture);
        string n = ""; bool fromInv = false;
        if (selectedItemIdx >= 0) { n = currentView.inventory[selectedItemIdx]; fromInv = true; }
        else if (selectedSlot != "") {
            switch(selectedSlot) {
                case "Head": n=currentView.eqHead; break; case "Body": n=currentView.eqBody; break; case "Legs": n=currentView.eqLegs; break;
                case "WepMain": n=currentView.eqWeaponMain; break; case "WepOff": n=currentView.eqWeaponOff; break;
                case "Ring1": n=currentView.eqRing1; break; case "Ring2": n=currentView.eqRing2; break;
                case "Neck": n=currentView.eqNecklace; break; case "Ancient": n=currentView.eqAncientGold; break;
            }
        }
        if (n == "" || n == "Trống" || n == "Tay Không") return;
        GUI.color = Color.yellow; GUI.Label(new Rect(x+10, y+10, 200, 45), n, new GUIStyle(GUI.skin.label){fontSize=13, fontStyle=FontStyle.Bold, wordWrap=true});
        
        GUI.color = Color.white;
        string statInfo = GetItemDescription(n);
        GUI.Label(new Rect(x+10, y+60, 200, 150), statInfo, new GUIStyle(GUI.skin.label){fontSize=11, wordWrap=true});

        if (fromInv) {
            GUI.color = Color.green; if (GUI.Button(new Rect(x+10, y+240, 200, 30), "MẶC / DÙNG")) { currentView.EquipItem(selectedItemIdx); ResetSelection(); }
            
            if (currentView == player && companion != null) {
                GUI.color = new Color(0.2f, 0.8f, 1f); 
                if (GUI.Button(new Rect(x+10, y+275, 200, 30), "TRANG BỊ CHO ĐỆ")) {
                    string itemToGive = player.inventory[selectedItemIdx];
                    companion.inventory.Add(itemToGive);
                    player.inventory.RemoveAt(selectedItemIdx);
                    companion.EquipItem(companion.inventory.Count - 1);
                    ResetSelection();
                }
                GUI.color = Color.cyan; if (GUI.Button(new Rect(x+10, y+310, 200, 30), "GIAO CHO ĐỆ")) { companion.inventory.Add(player.inventory[selectedItemIdx]); player.inventory.RemoveAt(selectedItemIdx); ResetSelection(); }
            } else if (currentView == companion && player != null) {
                GUI.color = Color.cyan; if (GUI.Button(new Rect(x+10, y+275, 200, 30), "LẤY VỀ TÚI")) { player.inventory.Add(companion.inventory[selectedItemIdx]); companion.inventory.RemoveAt(selectedItemIdx); ResetSelection(); }
            }
            GUI.color = Color.red; if (GUI.Button(new Rect(x+10, y+345, 200, 25), "BÁN: +" + currentView.GetItemPrice(n) + "G")) { currentView.SellItem(selectedItemIdx); ResetSelection(); }
        } else {
            GUI.color = new Color(1f, 0.5f, 0f); if (GUI.Button(new Rect(x+10, y+310, 200, 30), "GỠ RA")) { currentView.Unequip(selectedSlot); ResetSelection(); }
        }
    }

    string GetItemDescription(string n) {
        if (n.Contains("Kiếm Gỗ")) return "⚔ +8 Sát thương";
        if (n.Contains("Kiếm Sắt")) return "⚔ +15 Sát thương";
        if (n.Contains("Huyết Kiếm")) return "⚔ +40 Sát thương\n🔥 Sức mạnh quỷ dữ\n(Vũ khí 2 tay)";
        
        if (n.Contains("Áo Da")) return "🛡 +12 Phòng thủ\n🍃 Nhẹ nhàng linh hoạt";
        if (n.Contains("Áo Giáp")) return "🛡 +18 Phòng thủ\n📦 Chống chịu vật lý";
        
        if (n.Contains("Mũ Sắt")) return "🛡 +8 Phòng thủ";
        if (n.Contains("Giày")) return "🛡 +5 Phòng thủ\n🏃 Tăng tốc chạy";
        
        if (n.Contains("Khiên Gỗ")) return "🛡 +10 Phòng thủ";
        if (n.Contains("Khiên Sắt") || n.Contains("Khiên")) return "🛡 +20 Phòng thủ";
        
        if (n.Contains("Nhẫn Kim Cương")) return "⚔ +5 Sát thương\n🛡 +5 Phòng thủ";
        if (n.Contains("Dây Chuyền Bạc")) return "❤ +30 Máu tối đa";
        if (n.Contains("Vàng Cổ")) return "⭐ Bảo vật thượng cổ\n⚔ +10 Sát thương\n🛡 +10 Phòng thủ\n❤ +50 Máu";
        
        if (n.Contains("Ngọc Đỏ")) return "🔴 Ngọc Tinh Nhuệ\n⚔ +20 Sát thương";
        if (n.Contains("Ngọc Xanh")) return "🔵 Ngọc Hộ Mệnh\n🛡 +15 Phòng thủ";
        if (n.Contains("Ngọc Tím")) return "🟣 Ngọc Sinh Mệnh\n❤ +80 Máu tối đa";
        
        if (n.Contains("Bình Máu")) return "💊 Hồi phục sinh lực tức thời";
        return "📦 Vật phẩm RPG cổ điển\nHãy mặc thử để xem sự thay đổi!";
    }

    void DrawSkillsTab(Rect mainR) {
        float sx = mainR.x+20; float sy = mainR.y+60;
        GUI.Label(new Rect(sx, sy, 300, 25), "ĐIỂM KỸ NĂNG: " + currentView.skillPoints, headerStyle);
        
        string[] pSkills = { "Chém Gió (Lv3)", "Lôi Đình (Lv6)", "Bất Tử (Lv9)", "Thần Tiên (Lv12)" };
        string[] pDescs = { "Chém AOE x2 dame xung quanh. Phím 1", "Sét đánh AOE x3 dame. Phím 2", "Hồi 30% HP ngay lập tức. Phím 3", "Tăng toàn bộ chỉ số trong 10s. Phím 4" };
        
        string[] cSkills = { "🛡 Hộ Vệ (Lv3)", "❤ Trị Thương (Lv6)", "💠 Phản Nguyên (Lv9)", "🔥 Cuồng Nộ (Lv12)" };
        string[] cDescs = { "Tăng 20% thủ cho chủ nhân khi đứng gần.", "Hồi 10 HP mỗi 2 giây cho đồng đội xung quanh.", "Phản lại 20% sát thương nhận vào thành sát thương phép.", "Tăng 30% tốc độ đánh cho cả đội." };

        string[] currSkills = currentView.isPlayer ? pSkills : cSkills;
        string[] currDescs = currentView.isPlayer ? pDescs : cDescs;
        
        for (int i=0; i<currSkills.Length; i++) {
            Rect skR = new Rect(sx, sy + 30 + (i*90), 800, 80);
            GUI.color = new Color(0.15f, 0.15f, 0.2f); GUI.DrawTexture(skR, Texture2D.whiteTexture);
            
            bool unlocked = currentView.unlockedSkills.Contains(currSkills[i]);
            GUI.color = unlocked ? Color.green : Color.white;
            GUI.Label(new Rect(skR.x+10, skR.y+10, 300, 25), (unlocked?"✅ ":"")+currSkills[i], new GUIStyle(GUI.skin.label){fontStyle=FontStyle.Bold, fontSize=14});
            GUI.color = Color.gray; GUI.Label(new Rect(skR.x+10, skR.y+35, 500, 40), currDescs[i]);
            
            if (!unlocked && currentView.level >= (i+1)*3 && currentView.skillPoints > 0) {
                GUI.color = Color.yellow; if (GUI.Button(new Rect(skR.xMax-100, skR.y+20, 80, 40), "HỌC")) currentView.LearnSkill(currSkills[i]);
            }
        }
    }

    void DrawHUD() {
        // --- 1. HUD NGƯỜI CHƠI ---
        DrawSingleHUD(10, 10, player, "NGƯỜI CHƠI", Color.red);
        
        // --- 2. HUD ĐỆ TỬ ---
        if (companion != null)
        {
            DrawSingleHUD(10, 100, companion, "ĐỆ TỬ", new Color(1f, 0.5f, 0.2f));
        }
    }

    void DrawSingleHUD(float x, float y, PlayerStats st, string label, Color barCol)
    {
        GUI.color = new Color(0,0,0,0.6f); GUI.DrawTexture(new Rect(x, y, 260, 85), Texture2D.whiteTexture);
        
        // Thanh HP
        GUI.color = new Color(0.2f,0,0); GUI.DrawTexture(new Rect(x+5, y+5, 250, 20), Texture2D.whiteTexture);
        float hpRatio = (float)st.currentHealth / st.maxHealth;
        GUI.color = barCol; GUI.DrawTexture(new Rect(x+5, y+5, 250 * hpRatio, 20), Texture2D.whiteTexture);
        GUI.color = Color.white; GUI.Label(new Rect(x+10, y+5, 240, 20), label + ": " + st.currentHealth + "/" + st.maxHealth);
        
        // Thanh EXP
        GUI.color = new Color(0,0,0.2f); GUI.DrawTexture(new Rect(x+5, y+30, 250, 12), Texture2D.whiteTexture);
        float expRatio = (float)st.currentExp / st.expToNextLevel;
        GUI.color = new Color(0.2f, 0.6f, 1f); GUI.DrawTexture(new Rect(x+5, y+30, 250 * expRatio, 12), Texture2D.whiteTexture);
        GUI.color = Color.cyan; GUI.Label(new Rect(x+10, y+27, 240, 18), "Lv " + st.level + "  EXP: " + st.currentExp + "/" + st.expToNextLevel, new GUIStyle(GUI.skin.label){fontSize=11, fontStyle=FontStyle.Bold});

        // Vàng (Chỉ hiện cho player hoặc tổng vàng)
        if (st.isPlayer) {
            GUI.color = Color.yellow; GUI.Label(new Rect(x+5, y+50, 240, 25), "💰 " + st.gold + " Vàng", new GUIStyle(GUI.skin.label){fontSize=14, fontStyle=FontStyle.Bold});
        } else {
            GUI.color = Color.white; GUI.Label(new Rect(x+5, y+50, 240, 25), "🌟 " + st.characterName, new GUIStyle(GUI.skin.label){fontSize=12});
        }
    }

    void DrawFooter(Rect r) { if (currentView != null && currentView.isPlayer) { GUI.color = Color.green; if (GUI.Button(new Rect(r.x+15, r.yMax-45, 120, 32), "💾 LƯU GAME")) player.SaveGame(); } }

    bool DrawSlot(float x, float y, string c, bool s) {
        GUI.color = s ? Color.yellow : new Color(0.2f, 0.2f, 0.3f); GUI.DrawTexture(new Rect(x,y,68,68), Texture2D.whiteTexture);
        GUI.color = Color.white; return GUI.Button(new Rect(x+2, y+2, 64, 64), c, slotLabelStyle);
    }

    void DrawFloatingDamage() {
        Camera c = Camera.main; if (c == null) return;
        foreach (var d in damageTexts) {
            Vector3 p = c.WorldToScreenPoint(d.worldPos);
            if (p.z > 0) { GUI.color = new Color(d.color.r, d.color.g, d.color.b, d.alpha); GUI.Label(new Rect(p.x-50, Screen.height-p.y-25, 100, 30), d.text, dmgStyle); }
        }
    }
}

