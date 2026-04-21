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
    public Texture2D warriorPortrait;

    public bool isBagOpen = false;
    private int currentTab = 0;
    private int selectedItemIdx = -1;
    private string selectedSlot = "";
    private Vector2 scrollPos;

    private Texture2D panelBgTex;
    private Texture2D slotBgTex;
    // ===== UI Map: Layered Architecture Assets =====
    // 1. Backgrounds
    private Texture2D bgDungeon, bgLightOverlay, bgVignette, bgParticles;
    // 2. Main Frame (9-Slice)
    private Texture2D frTL, frTR, frBL, frBR, frT, frB, frL, frR, frFill;
    // 3. Left Panel (Character)
    private Texture2D lpBg, lpAvatar, lpSil;
    private Texture2D lpSHead, lpSChest, lpSGlove, lpSBoot, lpSWeapon, lpSEmpty;
    // 4. Inventory (Center)
    private Texture2D invGridBg, invSlot, invSlotHov, invSlotSel;
    // 5. Right Panel (Tooltip)
    private Texture2D rpBg, rpHeader, rpDivider, rpTextArea;
    // 6. Buttons & Tabs
    private Texture2D btnClose, btnCloseHov, btnClosePre, tabBtn, tabAct;
    // 7. Effects
    private Texture2D fxRuneTL, fxRuneTR, fxRuneBL, fxRuneBR, fxOrnTop, fxOrnBot, fxGlow, fxLight, fxDust, fxMagic;

    private int uiVersion = 1; // 1 = A (Old), 2 = B (New)

    // ===== UI Map: 9-Slice Frame Textures =====
    private Texture2D uiSlotNormal;     // inventory/slot.png
    private Texture2D uiSlotSelected;   // inventory/slot_selected.png (golden)
    private Texture2D uiSlotHover;      // inventory/slot_hover.png
    private bool isHoveringSlot = false;
    private int hoverSlotIdx = -1;
    private string hoverSlotStr = "";
    
    private PlayerStats hudClickedCompanion = null;

    void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    void Start() { 
        TryFindPlayer(); 
        LoadResources();
    }

    void LoadResources()
    {
        if (warriorPortrait == null) warriorPortrait = Resources.Load<Texture2D>("warrior_portrait");
        
        // 1. Backgrounds
        bgDungeon      = Resources.Load<Texture2D>("Sprites/background/bg_dungeon");
        bgLightOverlay = Resources.Load<Texture2D>("Sprites/background/bg_light_overlay");
        bgVignette     = Resources.Load<Texture2D>("Sprites/background/bg_vignette");
        bgParticles    = Resources.Load<Texture2D>("Sprites/background/bg_particles");

        // 2. Frame
        frTL   = Resources.Load<Texture2D>("Sprites/frame/corner_top_left");
        frTR   = Resources.Load<Texture2D>("Sprites/frame/corner_top_right");
        frBL   = Resources.Load<Texture2D>("Sprites/frame/corner_bottom_left");
        frBR   = Resources.Load<Texture2D>("Sprites/frame/corner_bottom_right");
        frT    = Resources.Load<Texture2D>("Sprites/frame/edge_top");
        frB    = Resources.Load<Texture2D>("Sprites/frame/edge_bottom");
        frL    = Resources.Load<Texture2D>("Sprites/frame/edge_left");
        frR    = Resources.Load<Texture2D>("Sprites/frame/edge_right");
        frFill = Resources.Load<Texture2D>("Sprites/frame/center_fill");

        // 3. Left Panel
        lpBg      = Resources.Load<Texture2D>("Sprites/left_panel/panel_bg");
        lpAvatar  = Resources.Load<Texture2D>("Sprites/left_panel/avatar_circle");
        lpSil     = Resources.Load<Texture2D>("Sprites/left_panel/silhouette");
        lpSHead   = Resources.Load<Texture2D>("Sprites/left_panel/slot_head");
        lpSChest  = Resources.Load<Texture2D>("Sprites/left_panel/slot_chest");
        lpSGlove  = Resources.Load<Texture2D>("Sprites/left_panel/slot_glove");
        lpSBoot   = Resources.Load<Texture2D>("Sprites/left_panel/slot_boot");
        lpSWeapon = Resources.Load<Texture2D>("Sprites/left_panel/slot_weapon");
        lpSEmpty  = Resources.Load<Texture2D>("Sprites/left_panel/slot_empty");

        // 4. Inventory
        invGridBg  = Resources.Load<Texture2D>("Sprites/inventory/grid_bg");
        invSlot    = Resources.Load<Texture2D>("Sprites/inventory/slot");
        invSlotHov = Resources.Load<Texture2D>("Sprites/inventory/slot_hover");
        invSlotSel = Resources.Load<Texture2D>("Sprites/inventory/slot_selected");

        // 5. Right Panel
        rpBg      = Resources.Load<Texture2D>("Sprites/right_panel/panel_bg");
        rpHeader  = Resources.Load<Texture2D>("Sprites/right_panel/header_bar");
        rpDivider = Resources.Load<Texture2D>("Sprites/right_panel/divider_line");
        rpTextArea = Resources.Load<Texture2D>("Sprites/right_panel/text_area");

        // 6. Buttons
        btnClose    = Resources.Load<Texture2D>("Sprites/ui/btn_close");
        btnCloseHov = Resources.Load<Texture2D>("Sprites/ui/btn_close_hover");
        btnClosePre = Resources.Load<Texture2D>("Sprites/ui/btn_close_pressed");
        tabBtn      = Resources.Load<Texture2D>("Sprites/ui/tab_button");
        tabAct      = Resources.Load<Texture2D>("Sprites/ui/tab_active");

        // 7. Effects
        fxRuneTL = Resources.Load<Texture2D>("Sprites/effects/rune_top_left");
        fxRuneTR = Resources.Load<Texture2D>("Sprites/effects/rune_top_right");
        fxRuneBL = Resources.Load<Texture2D>("Sprites/effects/rune_bottom_left");
        fxRuneBR = Resources.Load<Texture2D>("Sprites/effects/rune_bottom_right");
        fxOrnTop = Resources.Load<Texture2D>("Sprites/effects/ornament_center_top");
        fxOrnBot = Resources.Load<Texture2D>("Sprites/effects/ornament_center_bottom");
        fxGlow   = Resources.Load<Texture2D>("Sprites/effects/glow_border");
        fxLight  = Resources.Load<Texture2D>("Sprites/effects/light_overlay");
        fxDust   = Resources.Load<Texture2D>("Sprites/effects/dust_particles");
        fxMagic  = Resources.Load<Texture2D>("Sprites/effects/magic_glow");

        // Compatibility
        panelBgTex = lpBg;
        slotBgTex = invSlot;
    }

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
        if (Keyboard.current != null) {
            if (Keyboard.current.vKey.wasPressedThisFrame) { // Đổi A thành V
                if (isBagOpen && uiVersion == 1) isBagOpen = false;
                else { isBagOpen = true; uiVersion = 1; }
                ResetSelection(); 
            }
            if (Keyboard.current.bKey.wasPressedThisFrame) { 
                if (isBagOpen && uiVersion == 2) isBagOpen = false;
                else { isBagOpen = true; uiVersion = 2; }
                ResetSelection(); 
            }
        }
#else
        if (Input.GetKeyDown(KeyCode.V)) { // Đổi A thành V
            if (isBagOpen && uiVersion == 1) isBagOpen = false;
            else { isBagOpen = true; uiVersion = 1; }
            ResetSelection(); 
        }
        if (Input.GetKeyDown(KeyCode.B)) { 
            if (isBagOpen && uiVersion == 2) isBagOpen = false;
            else { isBagOpen = true; uiVersion = 2; }
            ResetSelection(); 
        }
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
        if (headerStyle == null) {
            headerStyle = new GUIStyle(GUI.skin.label) { fontSize = 13, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            headerStyle.normal.textColor = new Color(0.9f, 0.7f, 0.3f); // Golden
        }
        if (slotLabelStyle == null) { 
            slotLabelStyle = new GUIStyle(GUI.skin.label) { fontSize = 9, alignment = TextAnchor.MiddleCenter, wordWrap = true }; 
            slotLabelStyle.normal.textColor = new Color(0.8f, 0.8f, 0.7f); 
        }
        if (statValueStyle == null) { 
            statValueStyle = new GUIStyle(GUI.skin.label) { fontSize = 11, fontStyle = FontStyle.Bold };
            statValueStyle.normal.textColor = new Color(0.9f, 0.8f, 0.4f);
        }
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
            GUI.color = new Color(0, 0, 0, 0.7f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);

            if (uiVersion == 1) DrawVersionOld();
            else DrawVersionPro();
        }
    }

    void DrawVersionOld() {
        float uiW = Mathf.Min(900, Screen.width * 0.95f);
        float uiH = Mathf.Min(560, Screen.height * 0.9f);
        Rect mainR = new Rect(Screen.width / 2 - uiW / 2, Screen.height / 2 - uiH / 2, uiW, uiH);
        
        DrawDarkFantasyPanelOld(mainR);
        GUI.color = new Color(0.8f, 0.2f, 0.2f);
        if (GUI.Button(new Rect(mainR.xMax-35, mainR.y+10, 24, 24), "X")) isBagOpen = false;

        DrawTabsOld(mainR);
        if (currentTab == 0) DrawCharacterTabOld(mainR);
        else DrawSkillsTabOld(mainR);
        DrawFooterOld(mainR);
    }

    // ===========================================================================================
    // UI PRO - LAYER 0: Khung tổng thể
    // Layering: BACKGROUND → FRAME → [LEFT | CENTER | RIGHT] → EFFECTS → TABS → CLOSE BTN
    // ===========================================================================================
    void DrawVersionPro()
    {
        float uiW = 960f; float uiH = 540f;
        if (Screen.width < 960 || Screen.height < 540) { uiW = Screen.width * 0.98f; uiH = Screen.height * 0.98f; }
        Rect mainR = new Rect(Screen.width / 2f - uiW / 2f, Screen.height / 2f - uiH / 2f, uiW, uiH);

        // LAYER 1: BACKGROUND (Layered Dungeon)
        DrawDarkFantasyPanelPro(mainR);

        // LAYER 2: FRAME (9-Slice)
        Draw9Slice(mainR);

        // EXTRA EFFECTS (Static)
        if (fxGlow != null) { GUI.color = new Color(1f, 1f, 1f, 0.4f); GUI.DrawTexture(mainR, fxGlow); }
        if (fxRuneTL != null) GUI.DrawTexture(new Rect(mainR.x + 25, mainR.y + 25, 100, 45), fxRuneTL);
        if (fxRuneTR != null) GUI.DrawTexture(new Rect(mainR.xMax - 125, mainR.y + 25, 100, 45), fxRuneTR);
        if (fxOrnTop != null) GUI.DrawTexture(new Rect(mainR.center.x - 120, mainR.y + 10, 240, 50), fxOrnTop);
        if (fxLight != null) { GUI.color = new Color(1f, 1f, 1f, 0.3f); GUI.DrawTexture(mainR, fxLight); }

        GUI.color = Color.white;

        // LAYER 3: CONTENT
        if (currentTab == 0) DrawCharacterTabPro(mainR);
        else DrawSkillsTabPro(mainR);

        // LAYER 4: TABS + CLOSE
        DrawTabsPro(mainR);

        // Close button using sliced assets
        Rect closeRect = new Rect(mainR.xMax - 45, mainR.y + 15, 30, 30);
        Texture2D curClose = btnClose;
        if (closeRect.Contains(Event.current.mousePosition)) {
            curClose = btnCloseHov;
            if (Event.current.type == EventType.MouseDown) curClose = btnClosePre;
        }
        if (GUI.Button(closeRect, curClose, GUIStyle.none)) isBagOpen = false;
    }

    void DrawDarkFantasyPanelOld(Rect r) {
        if (panelBgTex != null) {
            GUI.color = Color.white;
            GUI.DrawTexture(r, panelBgTex);
        } else {
            GUI.color = new Color(0.08f, 0.08f, 0.08f, 0.98f);
            GUI.DrawTexture(r, Texture2D.whiteTexture);
            GUI.color = new Color(0.25f, 0.2f, 0.15f);
            DrawBorder(r, 4);
        }
    }

    void DrawDarkFantasyPanelPro(Rect r) {
        // LAYER 1: Dungeon Background
        if (bgDungeon != null) GUI.DrawTexture(r, bgDungeon, ScaleMode.ScaleAndCrop);
        else GUI.DrawTexture(r, Texture2D.whiteTexture); // Fallback

        // LAYER 2: Light Overlay
        if (bgLightOverlay != null) {
            GUI.color = new Color(1f, 1f, 1f, 0.4f);
            GUI.DrawTexture(r, bgLightOverlay, ScaleMode.ScaleAndCrop);
        }

        // LAYER 3: Particles (Static)
        if (bgParticles != null) {
            GUI.color = new Color(1f, 1f, 1f, 0.5f);
            GUI.DrawTexture(new Rect(r.x, r.yMax - 140, r.width, 140), bgParticles);
        }

        // LAYER 4: Vignette
        if (bgVignette != null) {
            GUI.color = new Color(1f, 1f, 1f, 0.85f);
            GUI.DrawTexture(r, bgVignette, ScaleMode.StretchToFill);
        }

        GUI.color = Color.white;
    }

    void Draw9Slice(Rect r)
    {
        if (frTL == null) return;
        float cw = frTL.width; float ch = frTL.height;
        float ew = frT.width;  float eh = frT.height;

        // Fill
        if (frFill != null) GUI.DrawTexture(new Rect(r.x + cw, r.y + ch, r.width - cw * 2, r.height - ch * 2), frFill, ScaleMode.StretchToFill);

        // Corners
        GUI.DrawTexture(new Rect(r.x, r.y, cw, ch), frTL);
        GUI.DrawTexture(new Rect(r.xMax - cw, r.y, cw, ch), frTR);
        GUI.DrawTexture(new Rect(r.x, r.yMax - ch, cw, ch), frBL);
        GUI.DrawTexture(new Rect(r.xMax - cw, r.yMax - ch, cw, ch), frBR);

        // Edges
        GUI.DrawTexture(new Rect(r.x + cw, r.y, r.width - cw * 2, eh), frT, ScaleMode.StretchToFill);
        GUI.DrawTexture(new Rect(r.x + cw, r.yMax - eh, r.width - cw * 2, eh), frB, ScaleMode.StretchToFill);
        GUI.DrawTexture(new Rect(r.x, r.y + ch, cw, r.height - ch * 2), frL, ScaleMode.StretchToFill);
        GUI.DrawTexture(new Rect(r.xMax - cw, r.y + ch, cw, r.height - ch * 2), frR, ScaleMode.StretchToFill);
    }

    void DrawTabsOld(Rect mainR)
    {
        float tabX = mainR.x + 15;
        float tabY = mainR.y + 10;
        GUI.color = currentTab == 0 ? new Color(0.6f, 0.5f, 0.3f) : new Color(0.2f, 0.15f, 0.12f);
        if (GUI.Button(new Rect(tabX, tabY, 130, 32), "TRANG BỊ", new GUIStyle(GUI.skin.button){fontSize=12, fontStyle=FontStyle.Bold})) currentTab = 0;
        GUI.color = currentTab == 1 ? new Color(0.6f, 0.5f, 0.3f) : new Color(0.2f, 0.15f, 0.12f);
        if (GUI.Button(new Rect(tabX + 140, tabY, 130, 32), "KỸ NĂNG", new GUIStyle(GUI.skin.button){fontSize=12, fontStyle=FontStyle.Bold})) currentTab = 1;

        GUI.color = new Color(0.8f, 0.7f, 0.4f);
        string viewName = (currentView == player) ? "HIỆP SĨ" : currentView.characterName;
        GUI.Label(new Rect(mainR.x + 300, tabY, 200, 32), viewName, new GUIStyle(GUI.skin.label){fontSize=15, fontStyle=FontStyle.Bold});
        GUI.color = Color.white;
    }

    void DrawTabsPro(Rect mainR)
    {
        float tabX = mainR.x + 162; // Bắt đầu sau sidebar
        float tabY = mainR.y + 8;
        GUIStyle tabStyle = new GUIStyle(GUI.skin.button) { fontSize = 12, fontStyle = FontStyle.Bold };

        // Tab TRANG BỊ
        GUI.color = currentTab == 0
            ? new Color(0.95f, 0.82f, 0.48f)   // Active: vàng đồng sáng
            : new Color(0.22f, 0.18f, 0.14f);   // Inactive: nâu tối
        if (GUI.Button(new Rect(tabX, tabY, 120, 34), "TRANG BI", tabStyle)) currentTab = 0;

        // Tab KY NANG
        GUI.color = currentTab == 1
            ? new Color(0.95f, 0.82f, 0.48f)
            : new Color(0.22f, 0.18f, 0.14f);
        if (GUI.Button(new Rect(tabX + 128, tabY, 120, 34), "KY NANG", tabStyle)) currentTab = 1;

        GUI.color = Color.white;
    }

    void DrawCharacterTabOld(Rect mainR)
    {
        if (currentView == null) return;
        float sidebarW = 160;
        float pdX = mainR.x + sidebarW + 10; float pdY = mainR.y + 55;
        DrawSidebarOld(mainR.x + 5, pdY, sidebarW);

        GUI.color = new Color(0.09f, 0.07f, 0.06f, 1f); // Dark gritty background
        GUI.DrawTexture(new Rect(pdX, pdY, 240, 440), Texture2D.whiteTexture);
        GUI.color = new Color(0.8f, 0.7f, 0.4f); // Old gold text
        string headerText = currentView.characterName;
        if (!string.IsNullOrEmpty(currentView.characterRank)) headerText += " [" + currentView.characterRank + "]";
        GUI.Label(new Rect(pdX, pdY+5, 240, 25), headerText, headerStyle);

        GUI.color = new Color(0.8f, 0.7f, 0.4f);
        GUI.Label(new Rect(pdX+10, pdY+28, 220, 20), "Vàng: " + player.gold, new GUIStyle(GUI.skin.label){fontSize=11, fontStyle=FontStyle.Bold});
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

        DrawInventoryOld(pdX + 250, pdY);
        DrawTooltipOld(pdX + 250 + 245, pdY);
    }

    // ===========================================================================================
    // UI PRO - LAYER 3: Nội dung Tab Trang Bi
    // Sub-layers: SIDEBAR → LEFT_PANEL → CENTER_INVENTORY → RIGHT_PANEL
    // ===========================================================================================
    void DrawCharacterTabPro(Rect mainR)
    {
        if (currentView == null) return;
        float colY = mainR.y + 50;

        // ── SUB-LAYER A: SIDEBAR (Cột 1) - x: 5~158 ────────────────────────────────────────
        DrawSidebarPro(mainR.x + 5, colY, 150);

        // ── SUB-LAYER B: LEFT PANEL (Cột 2 - Character & Equip) - x: 162~412 ──────────────
        DrawLeftEquipPanel(mainR.x + 162, colY);

        // ── SUB-LAYER C: CENTER INVENTORY (Cột 3) - x: 416~733 ─────────────────────────────
        DrawInventoryPro(mainR.x + 416, colY);

        // ── SUB-LAYER D: RIGHT INFO PANEL (Cột 4) - x: 737~956 ─────────────────────────────
        DrawTooltipPro(mainR.x + 737, colY);
    }

    // ─── LEFT PANEL: Avatar + 9 Equip Slots + Stats ─────────────────────────────────────────
    void DrawLeftEquipPanel(float x, float y)
    {
        float panelW = 248f;
        float panelH = 482f;
        Rect r = new Rect(x, y, panelW, panelH);

        // LAYER B1: Sidebar-specific panel background
        if (lpBg != null) GUI.DrawTexture(r, lpBg);
        else DrawSolidRect(r, new Color(0.08f, 0.07f, 0.06f, 0.8f));

        // LAYER B2: Silhouette
        if (lpSil != null) {
            GUI.color = new Color(1f, 1f, 1f, 0.4f);
            GUI.DrawTexture(new Rect(x + 50, y + 100, 140, 240), lpSil);
            GUI.color = Color.white;
        }

        // LAYER B3: Avatar Circle + Portrait
        float avS = 180f; // Scale from sheet
        float avX = x + panelW/2f - 90f;
        float avY = y + 20f;
        if (lpAvatar != null) GUI.DrawTexture(new Rect(avX, avY, 180, 150), lpAvatar);
        
        Rect portraitRect = new Rect(x + panelW/2f - 35, y + 55, 70, 70);
        if (currentView.characterPortrait != null) GUI.DrawTexture(portraitRect, currentView.characterPortrait.texture);
        else if (warriorPortrait != null) GUI.DrawTexture(portraitRect, warriorPortrait);

        // LAYER B4: 9 Equip Slots
        float sq = 64f; float gap = 6f;
        float sX = x + 20f; float sY = y + 160f;

        // Implementation matches the 3x3 grid from the screenshot
        if (DrawEquipSlot(sX,           sY,       currentView.eqAncientGold, selectedSlot=="Ancient", lpSEmpty))  { selectedSlot="Ancient"; selectedItemIdx=-1; }
        if (DrawEquipSlot(sX+sq+gap,    sY,       currentView.eqHead,        selectedSlot=="Head",    lpSHead))   { selectedSlot="Head";    selectedItemIdx=-1; }
        if (DrawEquipSlot(sX+2*(sq+gap),sY,       currentView.eqNecklace,    selectedSlot=="Neck",    lpSEmpty))  { selectedSlot="Neck"; selectedItemIdx=-1; }
        
        if (DrawEquipSlot(sX,           sY+sq+gap, currentView.eqWeaponMain, selectedSlot=="WepMain", lpSWeapon)) { selectedSlot="WepMain"; selectedItemIdx=-1; }
        if (DrawEquipSlot(sX+sq+gap,    sY+sq+gap, currentView.eqBody,       selectedSlot=="Body",    lpSChest))  { selectedSlot="Body";    selectedItemIdx=-1; }
        if (DrawEquipSlot(sX+2*(sq+gap),sY+sq+gap, currentView.eqWeaponOff,  selectedSlot=="WepOff",  lpSEmpty))  { selectedSlot="WepOff";  selectedItemIdx=-1; }
        
        if (DrawEquipSlot(sX,           sY+2*(sq+gap), currentView.eqRing1,  selectedSlot=="Ring1",   lpSEmpty))  { selectedSlot="Ring1";   selectedItemIdx=-1; }
        if (DrawEquipSlot(sX+sq+gap,    sY+2*(sq+gap), currentView.eqLegs,   selectedSlot=="Legs",    lpSBoot))   { selectedSlot="Legs";    selectedItemIdx=-1; }
        if (DrawEquipSlot(sX+2*(sq+gap),sY+2*(sq+gap), currentView.eqRing2,  selectedSlot=="Ring2",   lpSEmpty))  { selectedSlot="Ring2";   selectedItemIdx=-1; }

        // STATS (Static placement)
        float stY = sY + (sq+gap)*3 + 15f;
        GUIStyle stS = new GUIStyle(GUI.skin.label) { fontSize = 12, fontStyle = FontStyle.Bold };
        DrawShadowLabel(new Rect(x + 15, stY,      200, 20), "DMG: " + currentView.bonusDamage,  stS, Color.white);
        DrawShadowLabel(new Rect(x + 15, stY + 22, 200, 20), "DEF: " + currentView.bonusDefense, stS, Color.white);
        DrawShadowLabel(new Rect(x + 15, stY + 44, 200, 20), "HP : " + currentView.currentHealth,  stS, new Color(1f, 0.4f, 0.4f));
    }

    bool DrawEquipSlot(float x, float y, ItemInstance inst, bool sel, Texture2D placeholder)
    {
        Rect r = new Rect(x, y, 64, 64);
        
        // 1. Slot BG
        Texture2D st = invSlot;
        if (sel) st = invSlotSel;
        else if (r.Contains(Event.current.mousePosition)) st = invSlotHov;
        if (st != null) GUI.DrawTexture(r, st);
        
        // 2. Placeholder icon (if empty)
        if (inst == null || inst.data == null) {
            if (placeholder != null) {
                GUI.color = new Color(1f, 1f, 1f, 0.3f);
                GUI.DrawTexture(new Rect(x+12, y+12, 40, 40), placeholder);
                GUI.color = Color.white;
            }
        } else {
            // 3. Item Icon
            Sprite icon = inst.GetIcon();
            if (icon != null) GUI.DrawTexture(new Rect(x+8, y+8, 48, 48), icon.texture);
            
            // 4. Rank Symbol
            if (inst.itemRank > 0) {
                GUI.color = inst.GetRankColor();
                GUI.Label(new Rect(x+45, y+45, 15, 15), GetRankSymbol(inst.itemRank), 
                    new GUIStyle(GUI.skin.label){fontSize=10, fontStyle=FontStyle.Bold});
                GUI.color = Color.white;
            }
        }

        return GUI.Button(r, "", GUIStyle.none);
    }

    string GetRankSymbol(int rank) {
        string[] s = { "F","E","D","C","B","A","S" };
        return rank >= 0 && rank < s.Length ? s[rank] : "F";
    }

    // DrawSlotPro kept as compatibility alias -> goi sang DrawEquipSlot
    bool DrawSlotPro(float x, float y, ItemInstance inst, bool selected, string placeholder)
    {
        // Convert string placeholder to Texture2D placeholder (lpSEmpty as default)
        return DrawEquipSlot(x, y, inst, selected, lpSEmpty);
    }

    void DrawStatRowPro(float x, float y, string label, int val, string statKey) {
        GUI.color = Color.white; GUI.Label(new Rect(x, y, 160, 22), label + ": " + val);
        if (currentView.statPoints > 0) { 
            GUI.color = Color.green; 
            if (GUI.Button(new Rect(x + 175, y, 32, 22), "+", new GUIStyle(GUI.skin.button){fontSize=14, fontStyle=FontStyle.Bold})) currentView.AddStat(statKey); 
        }
    }

    void DrawSidebarOld(float x, float y, float w) {
        GUI.color = new Color(0.06f, 0.05f, 0.05f, 0.98f); // Dark bark
        GUI.DrawTexture(new Rect(x, y, w, 440), Texture2D.whiteTexture);
        GUI.color = new Color(0.8f, 0.7f, 0.4f); 
        GUI.Label(new Rect(x, y+5, w, 25), "ĐỘI NGŨ", headerStyle);
        float btnY = y + 40;
        bool isP = (currentView == player); GUI.color = isP ? new Color(0.4f, 0.3f, 0.1f) : new Color(0.15f, 0.12f, 0.1f);
        if (GUI.Button(new Rect(x+8, btnY, w-16, 40), "HIỆP SĨ", new GUIStyle(GUI.skin.button){fontSize=11, fontStyle=FontStyle.Bold})) { currentView = player; ResetSelection(); }
        btnY += 45;
        foreach(var c in companions) {
            if (c == null) continue;
            bool isS = (currentView == c); GUI.color = isS ? new Color(0.4f, 0.3f, 0.1f) : new Color(0.15f, 0.12f, 0.1f);
            if (GUI.Button(new Rect(x+8, btnY, w-16, 40), c.characterName, new GUIStyle(GUI.skin.button){fontSize=11, fontStyle=FontStyle.Bold})) { currentView = c; ResetSelection(); }
            btnY += 45;
        }
    }

    void DrawSidebarPro(float x, float y, float w) {
        // Sidebar Pro tích hợp vào nền master
        GUI.color = new Color(0.8f, 0.7f, 0.4f);
        GUI.Label(new Rect(x, y, w, 25), "ĐỘI NGŨ", headerStyle);
        float btnY = y + 35;
        
        bool isP = (currentView == player); GUI.color = isP ? new Color(0.9f, 0.7f, 0.2f) : new Color(0.2f, 0.18f, 0.15f);
        if (GUI.Button(new Rect(x+5, btnY, w-10, 45), "HIỆP SĨ", new GUIStyle(GUI.skin.button){fontSize=11, fontStyle=FontStyle.Bold})) { currentView = player; ResetSelection(); }
        btnY += 50;
        
        foreach(var c in companions) {
            if (c == null) continue;
            bool isS = (currentView == c); GUI.color = isS ? new Color(0.9f, 0.7f, 0.2f) : new Color(0.2f, 0.18f, 0.15f);
            if (GUI.Button(new Rect(x+5, btnY, w-10, 45), c.characterName.ToUpper(), new GUIStyle(GUI.skin.button){fontSize=11, fontStyle=FontStyle.Bold})) { currentView = c; ResetSelection(); }
            btnY += 50;
        }
    }

    void DrawStatRow(float x, float y, string label, int val, string statKey) {
        GUI.color = Color.white; GUI.Label(new Rect(x, y, 150, 22), label + ": " + val);
        if (currentView.statPoints > 0) { GUI.color = Color.green; if (GUI.Button(new Rect(x + 155, y, 28, 18), "+")) currentView.AddStat(statKey); }
    }

    void DrawInventoryOld(float x, float y) {
        GUI.color = new Color(0.9f, 0.7f, 0.3f); GUI.Label(new Rect(x, y, 200, 25), "HÒM ĐỒ CHUNG", headerStyle);
        float slotSize = 58; float padding = 8; int cols = 4;
        Rect vR = new Rect(x, y + 30, 280, 480); List<ItemInstance> inv = player.SharedInventory;
        int rows = Mathf.CeilToInt((float)inv.Count / cols); if (rows < 8) rows = 8;
        Rect cR = new Rect(0, 0, 260, rows * (slotSize + padding) + 10); 
        scrollPos = GUI.BeginScrollView(vR, scrollPos, cR);
        for (int i = 0; i < inv.Count; i++) {
            if (inv[i] == null || inv[i].data == null) continue;
            int col = i % cols; int row = i / cols;
            float sx = col * (slotSize + padding); float sy = row * (slotSize + padding);
            Rect sRect = new Rect(sx, sy, slotSize, slotSize);
            bool sel = (selectedItemIdx == i);
            if (slotBgTex != null) { GUI.color = sel ? Color.yellow : Color.white; GUI.DrawTexture(sRect, slotBgTex); }
            else { GUI.color = sel ? Color.yellow : new Color(0.15f, 0.12f, 0.1f); GUI.DrawTexture(sRect, Texture2D.whiteTexture); }
            Sprite itemIcon = inv[i].GetIcon();
            if (itemIcon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(sx + 4, sy + 4, slotSize - 8, slotSize - 8), itemIcon.texture); }
            else { GUI.color = GetItemTypeColor(inv[i].data.type); GUI.DrawTexture(new Rect(sx + 4, sy + 4, slotSize - 8, slotSize - 8), Texture2D.whiteTexture); GUI.color = Color.white; GUI.Label(new Rect(sx, sy, slotSize, slotSize), GetItemTypeIcon(inv[i].data.type), new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter, fontSize=18}); }
            for (int k = 0; k < inv[i].sockets.Count && k < 4; k++) {
                var gemData = inv[i].sockets[k]; if (gemData == null) continue;
                float gx = sx + 4 + (k * 11f); float gy = sy + slotSize - 12;
                if (gemData.icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(gx, gy, 10, 10), gemData.icon.texture); }
            }
            if (GUI.Button(sRect, "", GUIStyle.none)) { selectedItemIdx = i; selectedSlot = ""; }
        }
        GUI.EndScrollView(); GUI.color = Color.white;
    }

    // ===========================================================================================
    // UI PRO - SUB-LAYER C: CENTER INVENTORY
    // Structure: grid_bg → slot (Normal/Hover/Selected) → item_icon → gem dots → type_color
    // ===========================================================================================
    void DrawInventoryPro(float x, float y)
    {
        float panelW = 314f;
        float panelH = 482f;
        int cols = 4;
        float slotSize = 64f; float gap = 6f;

        // LAYER C1: Inventory Grid Background
        if (invGridBg != null) GUI.DrawTexture(new Rect(x, y, panelW, panelH), invGridBg);
        else DrawSolidRect(new Rect(x, y, panelW, panelH), new Color(0.05f, 0.05f, 0.04f, 0.8f));

        // LAYER C2: Grid Content
        float gridX = x + 15f; float gridY = y + 45f;
        Rect viewRect = new Rect(gridX, gridY, panelW - 30f, panelH - 60f);
        
        List<ItemInstance> inv = player.SharedInventory;
        int total = inv.Count;
        int displayRows = Mathf.Max(6, Mathf.CeilToInt((float)total / cols));
        float contentH = displayRows * (slotSize + gap) + 10f;
        Rect contentRect = new Rect(0, 0, viewRect.width - 20, contentH);

        scrollPos = GUI.BeginScrollView(viewRect, scrollPos, contentRect, false, true);
        for (int i = 0; i < displayRows * cols; i++) {
            int col = i % cols; int row = i / cols;
            float sx = col * (slotSize + gap); float sy = row * (slotSize + gap);
            Rect slotR = new Rect(sx, sy, slotSize, slotSize);

            bool hasItem = (i < total && inv[i] != null && inv[i].data != null);
            bool sel = (selectedItemIdx == i && hasItem);

            // Using common slot drawer
            Texture2D st = invSlot;
            if (sel) st = invSlotSel;
            else if (new Rect(gridX+sx, gridY+sy-scrollPos.y, 64, 64).Contains(Event.current.mousePosition)) st = invSlotHov;
            
            if (st != null) GUI.DrawTexture(slotR, st);

            if (hasItem) {
                Sprite icon = inv[i].GetIcon();
                if (icon != null) GUI.DrawTexture(new Rect(sx+8, sy+8, 48, 48), icon.texture);
                
                // Gem Dots
                for (int k=0; k<inv[i].sockets.Count && k<4; k++) {
                    GUI.color = Color.cyan;
                    GUI.DrawTexture(new Rect(sx+8+(k*10), sy+slotSize-12, 8, 8), Texture2D.whiteTexture);
                }
                GUI.color = Color.white;
            } else {
                GUI.color = new Color(1f, 1f, 1f, 0.05f);
                GUI.DrawTexture(slotR, invSlot);
                GUI.color = Color.white;
            }

            if (GUI.Button(slotR, "", GUIStyle.none)) { selectedItemIdx = i; selectedSlot = ""; }
        }
        GUI.EndScrollView();
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

    void DrawTooltipOld(float x, float y) {
        GUI.color = new Color(0.06f, 0.05f, 0.05f, 0.95f); // Dark bark background
        GUI.DrawTexture(new Rect(x, y, 210, 480), Texture2D.whiteTexture);
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
        Sprite tooltipIcon = inst.GetIcon();
        if (tooltipIcon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(x + 70, curY, 70, 70), tooltipIcon.texture); curY += 78; }
        else { GUI.color = GetItemTypeColor(d.type); GUI.DrawTexture(new Rect(x + 70, curY, 70, 70), Texture2D.whiteTexture); GUI.color = Color.white; GUI.Label(new Rect(x+70, curY+20, 70, 30), GetItemTypeIcon(d.type), new GUIStyle(GUI.skin.label){fontSize=28, alignment=TextAnchor.MiddleCenter}); curY += 78; }
        
        GUI.color = inst.GetRankColor(); GUI.Label(new Rect(x+5, curY, 200, 30), inst.GetDisplayName(), new GUIStyle(GUI.skin.label){fontSize=12, fontStyle=FontStyle.Bold, alignment=TextAnchor.MiddleCenter}); curY += 32;
        GUI.color = new Color(1f, 0.75f, 0.2f); GUI.Label(new Rect(x+5, curY, 200, 18), "Giá: " + d.price + " Vàng", new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter, fontSize=10}); curY += 20;
        GUI.color = Color.gray; GUI.Label(new Rect(x+8, curY, 195, 30), d.description, new GUIStyle(GUI.skin.label){fontSize=9, wordWrap=true}); curY += 35;
        
        GUIStyle stType = new GUIStyle(GUI.skin.label){fontSize=10, fontStyle=FontStyle.Bold};
        if (inst.GetBaseAtk() > 0) { GUI.color = Color.red; GUI.Label(new Rect(x+10, curY, 190, 18), "Tấn Công: " + inst.GetBaseAtk(), stType); curY += 18; }
        if (inst.GetBaseDef() > 0) { GUI.color = Color.cyan; GUI.Label(new Rect(x+10, curY, 190, 18), "Phòng Thủ: " + inst.GetBaseDef(), stType); curY += 18; }
        if (inst.GetBaseHp() > 0) { GUI.color = Color.green; GUI.Label(new Rect(x+10, curY, 190, 18), "Sinh Mệnh: " + inst.GetBaseHp(), stType); curY += 18; }

        if (inst.itemRank > 0) {
            curY += 5; GUI.color = Color.gray; GUI.DrawTexture(new Rect(x+10, curY, 190, 1), Texture2D.whiteTexture); curY += 4;
            if (inst.rankBonusAtk > 0) { GUI.color = new Color(1f, 0.4f, 0.4f); GUI.Label(new Rect(x+20, curY, 180, 18), "+ " + inst.rankBonusAtk + " Tấn Công", stType); curY += 18; }
            if (inst.rankBonusDef > 0) { GUI.color = new Color(0.4f, 0.8f, 1f); GUI.Label(new Rect(x+20, curY, 180, 18), "+ " + inst.rankBonusDef + " Phòng Thủ", stType); curY += 18; }
            if (inst.rankBonusHp > 0)  { GUI.color = new Color(0.4f, 1f, 0.4f);  GUI.Label(new Rect(x+20, curY, 180, 18), "+ " + inst.rankBonusHp + " Sinh Mệnh", stType); curY += 18; }
        }

        if (inst.sockets.Count > 0) {
            curY += 5; GUI.color = Color.gray; GUI.DrawTexture(new Rect(x+10, curY, 190, 1), Texture2D.whiteTexture); curY += 4;
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

        float btnY = y + 420;
        if (fromInv) {
            string targetName = (currentView == player) ? "HIEP SI" : currentView.characterName.ToUpper();
            if (d.type == ItemData.ItemType.Weapon || d.type == ItemData.ItemType.Armor || d.type == ItemData.ItemType.Accessory) {
                GUI.color = new Color(0.9f, 0.6f, 0.1f);
                if (GUI.Button(new Rect(x+8, btnY, 195, 45), "MAC CHO: " + targetName,
                    new GUIStyle(GUI.skin.button){fontSize=12, fontStyle=FontStyle.Bold})) {
                    currentView.EquipItem(inst); ResetSelection();
                }
                btnY += 36;
            } else if (d.type == ItemData.ItemType.Consumable) {
                GUI.color = Color.green;
                if (GUI.Button(new Rect(x+8, btnY, 195, 45), "SU DUNG",
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
            GUI.color = new Color(0.8f, 0.2f, 0.2f);
            if (GUI.Button(new Rect(x+8, btnY, 195, 45), "GO BO",
                new GUIStyle(GUI.skin.button){fontSize=12, fontStyle=FontStyle.Bold})) {
                currentView.UnequipItem(selectedSlot); ResetSelection();
            }
        }
        GUI.color = Color.white;
    }

    // ===========================================================================================
    // UI PRO - SUB-LAYER D: RIGHT INFO PANEL
    // Structure: panel_bg → header_bar → icon_area → divider_line → stat_list → action_button
    // ===========================================================================================
    void DrawTooltipPro(float x, float y)
    {
        float panelW = 213f; float panelH = 482f;
        Rect r = new Rect(x, y, panelW, panelH);

        // LAYER D1: Right Panel Background
        if (rpBg != null) GUI.DrawTexture(r, rpBg);
        else DrawSolidRect(r, new Color(0.06f, 0.05f, 0.05f, 0.9f));

        ItemInstance inst = GetSelectedItem(out bool fromInv);
        if (inst == null || inst.data == null) {
            GUI.Label(r, "Chon vat pham...", headerStyle);
            return;
        }

        // LAYER D2: Header Bar
        if (rpHeader != null) GUI.DrawTexture(new Rect(x, y+5, panelW, 50), rpHeader);
        DrawShadowLabel(new Rect(x, y+15, panelW, 25), inst.GetDisplayName(), headerStyle, inst.GetRankColor());

        // LAYER D3: Icon Area
        float iconS = 80f;
        GUI.DrawTexture(new Rect(x + (panelW-iconS)/2, y+60, iconS, iconS), Texture2D.whiteTexture);
        Sprite icon = inst.GetIcon();
        if (icon != null) GUI.DrawTexture(new Rect(x + (panelW-64)/2, y+68, 64, 64), icon.texture);

        // LAYER D4: Divider
        if (rpDivider != null) GUI.DrawTexture(new Rect(x+10, y+145, panelW-20, 20), rpDivider);

        // LAYER D5: Text Area
        if (rpTextArea != null) GUI.DrawTexture(new Rect(x+10, y+170, panelW-20, 100), rpTextArea);
        GUI.Label(new Rect(x+15, y+175, panelW-30, 90), inst.data.description, new GUIStyle(GUI.skin.label){fontSize=10, wordWrap=true});

        // LAYER D6: Stats
        float stY = y + 280;
        GUIStyle stS = new GUIStyle(GUI.skin.label) { fontSize = 11, fontStyle = FontStyle.Bold };
        if (inst.GetBaseAtk() > 0) DrawShadowLabel(new Rect(x+15, stY, 180, 20), "[x] ATK: +" + inst.GetBaseAtk(), stS, Color.white);
        stY += 22;
        if (inst.GetBaseDef() > 0) DrawShadowLabel(new Rect(x+15, stY, 180, 20), "[o] DEF: +" + inst.GetBaseDef(), stS, Color.white);

        // Button area
        float btnY = y + panelH - 65;
        if (fromInv) {
            if (GUI.Button(new Rect(x+10, btnY, panelW-20, 50), "EQUIP", new GUIStyle(GUI.skin.button){fontSize=14, fontStyle=FontStyle.Bold})) {
                currentView.EquipItem(inst); ResetSelection();
            }
        } else {
            if (GUI.Button(new Rect(x+10, btnY, panelW-20, 50), "UNEQUIP", new GUIStyle(GUI.skin.button){fontSize=14, fontStyle=FontStyle.Bold})) {
                currentView.UnequipItem(selectedSlot); ResetSelection();
            }
        }
    }

    ItemInstance GetSelectedItem(out bool fromInv) {
        fromInv = false;
        List<ItemInstance> sharedInv = player.SharedInventory;
        if (selectedItemIdx >= 0 && selectedItemIdx < sharedInv.Count) { fromInv = true; return sharedInv[selectedItemIdx]; }
        if (selectedSlot != "") {
            switch (selectedSlot) {
                case "Head": return currentView.eqHead; case "Body": return currentView.eqBody; case "Legs": return currentView.eqLegs;
                case "WepMain": return currentView.eqWeaponMain; case "WepOff": return currentView.eqWeaponOff;
                case "Ring1": return currentView.eqRing1; case "Ring2": return currentView.eqRing2;
                case "Neck": return currentView.eqNecklace; case "Ancient": return currentView.eqAncientGold;
            }
        }
        return null;
    }

    void DrawSkillsTabOld(Rect mainR) {
        float sidebarW = 160; DrawSidebarOld(mainR.x + 5, mainR.y + 55, sidebarW);
        float sx = mainR.x + sidebarW + 20; float sy = mainR.y + 60;
        GUI.color = Color.white; GUI.Label(new Rect(sx, sy, 300, 25), "ĐIỂM KỸ NĂNG: " + currentView.skillPoints, headerStyle);
        SkillData[] all = Resources.LoadAll<SkillData>("Skills");
        for (int i=0; i<all.Length; i++) {
            Rect skR = new Rect(sx, sy + 30 + (i*90), 650, 80); GUI.color = new Color(0.15f, 0.15f, 0.2f); GUI.DrawTexture(skR, Texture2D.whiteTexture);
            int sLv = currentView.GetSkillLevel(all[i].skillName);
            if (all[i].icon != null) { GUI.color = Color.white; GUI.DrawTexture(new Rect(skR.x + 15, skR.y + 15, 50, 50), all[i].icon.texture); }
            if (sLv < 10 && currentView.skillPoints > 0) { GUI.color = Color.yellow; if (GUI.Button(new Rect(skR.xMax-100, skR.y+20, 80, 40), "NÂNG")) currentView.LearnSkill(all[i]); }
        }
    }

    void DrawSkillsTabPro(Rect mainR) {
         float sidebarW = 150; DrawSidebarPro(mainR.x + 10, mainR.y + 55, sidebarW);
         GUI.color = Color.white; GUI.Label(new Rect(mainR.x+170, mainR.y+60, 300, 30), "KỸ NĂNG", headerStyle);
    }

    void DrawFooterOld(Rect r) {
        if (player == null) return; GUI.color = Color.green; if (GUI.Button(new Rect(r.x + 15, r.yMax - 40, 110, 30), "💾 LƯU")) player.SaveGame();
        GUI.color = Color.red; if (GUI.Button(new Rect(r.x + 130, r.yMax - 40, 110, 30), "🗑 NEW")) player.ResetGame();
    }

    void DrawHUD() {
        if (player == null) return;
        
        DrawPlayerUnitHUD(20, 20);
        DrawGoldHUD(Screen.width - 150, 20);
        DrawSkillHotbar();
        
        // Companions - List on the left, below player HUD
        float spacing = 85f; // Spacing for 0.8x scale HUDs
        for (int i = 0; i < companions.Count; i++) {
            if (i >= 5) break; 
            DrawCompanionHUD(20, 120 + (i * spacing), companions[i]);
        }
    }

    void DrawPlayerUnitHUD(float x, float y) {
        float hSize = 80f;
        
        // Portrait Drawing
        GUI.color = Color.white;
        if (warriorPortrait != null && player.characterPortrait == null) {
            GUI.DrawTexture(new Rect(x, y, hSize, hSize), warriorPortrait);
        } else if (player.characterPortrait != null) {
            GUI.DrawTexture(new Rect(x, y, hSize, hSize), player.characterPortrait.texture);
        } else {
            GUI.color = new Color(0.12f, 0.1f, 0.08f, 1f);
            GUI.DrawTexture(new Rect(x, y, hSize, hSize), Texture2D.whiteTexture);
        }
        
        GUI.color = new Color(0.35f, 0.28f, 0.22f); 
        DrawBorder(new Rect(x, y, hSize, hSize), 2);
        
        float lvSize = 28f;
        Rect lvR = new Rect(x + 65, y + 55, lvSize, lvSize);
        GUI.color = new Color(0.2f, 0.15f, 0.1f);
        GUI.DrawTexture(lvR, Texture2D.whiteTexture);
        GUI.color = new Color(0.8f, 0.7f, 0.4f);
        DrawBorder(lvR, 2);
        GUI.Label(lvR, player.level.ToString(), new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter, fontStyle=FontStyle.Bold, fontSize=12});

        float barX = x + 90, barW = 200;
        DrawFantasyBar(barX, y + 5, barW, 18, (float)player.currentHealth / player.maxHealth, new Color(0.7f, 0.1f, 0.1f), player.currentHealth + "/" + player.maxHealth);
        DrawFantasyBar(barX, y + 28, barW - 20, 14, (float)player.currentMana / player.maxMana, new Color(0.1f, 0.4f, 0.8f), player.currentMana + "/" + player.maxMana);
        
        float expR = Mathf.Clamp01((float)player.currentExp / player.expToNextLevel);
        DrawSimpleBar(barX, y + 48, barW - 40, 6, expR, new Color(0.8f, 0.7f, 0.3f));
        
        GUI.color = new Color(0.8f, 0.7f, 0.4f);
        GUI.Label(new Rect(barX, y + 55, 200, 25), player.characterName.ToUpper(), new GUIStyle(GUI.skin.label){fontStyle=FontStyle.Bold, fontSize=13});
    }

    void DrawCompanionHUD(float x, float y, PlayerStats st) {
        if (st == null) return;
        float sc = 0.8f; // Scale down for companions
        float hSize = 80f * sc;
        
        // Portrait Drawing
        GUI.color = Color.white;
        if (st.characterPortrait != null) {
            GUI.DrawTexture(new Rect(x, y, hSize, hSize), st.characterPortrait.texture);
        } else {
            GUI.color = new Color(0.12f, 0.1f, 0.08f, 1f);
            GUI.DrawTexture(new Rect(x, y, hSize, hSize), Texture2D.whiteTexture);
        }
        
        GUI.color = new Color(0.35f, 0.28f, 0.22f); 
        DrawBorder(new Rect(x, y, hSize, hSize), 2);
        
        float lvSize = 28f * sc;
        Rect lvR = new Rect(x + 65f * sc, y + 55f * sc, lvSize, lvSize);
        GUI.color = new Color(0.2f, 0.15f, 0.1f);
        GUI.DrawTexture(lvR, Texture2D.whiteTexture);
        GUI.color = new Color(0.8f, 0.7f, 0.4f);
        DrawBorder(lvR, 2);
        GUI.Label(lvR, st.level.ToString(), new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter, fontStyle=FontStyle.Bold, fontSize=(int)(12*sc)});

        float barX = x + 90f * sc, barW = 200f * sc;
        
        // HP
        float hpRatio = st.maxHealth > 0 ? (float)st.currentHealth / st.maxHealth : 0f;
        DrawFantasyBar(barX, y + 5f*sc, barW, 18f*sc, hpRatio, new Color(0.7f, 0.1f, 0.1f), st.currentHealth + "/" + st.maxHealth);
        
        // Mana
        float mpRatio = st.maxMana > 0 ? (float)st.currentMana / st.maxMana : 0f;
        DrawFantasyBar(barX, y + 28f*sc, barW - 20f*sc, 14f*sc, mpRatio, new Color(0.1f, 0.4f, 0.8f), st.currentMana + "/" + st.maxMana);
        
        // EXP
        float expR = st.expToNextLevel > 0 ? Mathf.Clamp01((float)st.currentExp / st.expToNextLevel) : 0f;
        DrawSimpleBar(barX, y + 48f*sc, barW - 40f*sc, 6f*sc, expR, new Color(0.8f, 0.7f, 0.3f));
        
        // Name & Button
        GUI.color = new Color(0.8f, 0.7f, 0.4f);
        GUI.Label(new Rect(barX, y + 55f*sc, 150, 25), st.characterName.ToUpper(), new GUIStyle(GUI.skin.label){fontStyle=FontStyle.Bold, fontSize=(int)(13*sc)});
        GUI.color = Color.white;
        if (GUI.Button(new Rect(barX + barW - 40, y + 55f*sc, 40, 20), "XEM", new GUIStyle(GUI.skin.button){fontSize=9})) { currentView = st; hudClickedCompanion = st; }
    }

    void DrawGoldHUD(float x, float y) {
        GUI.color = new Color(0.1f, 0.08f, 0.07f, 0.9f);
        GUI.DrawTexture(new Rect(x, y, 130, 32), Texture2D.whiteTexture);
        GUI.color = new Color(0.4f, 0.3f, 0.2f);
        DrawBorder(new Rect(x, y, 130, 32), 2);
        
        GUI.color = Color.yellow;
        GUI.Label(new Rect(x + 5, y + 5, 25, 25), "💰");
        GUI.Label(new Rect(x + 30, y + 5, 90, 25), player.gold.ToString(), new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleRight, fontStyle=FontStyle.Bold, fontSize=13});
    }

    void DrawSkillHotbar() {
        if (combat == null && player != null) combat = player.GetComponent<PlayerCombat>();
        float slotSize = 54f, pad = 8f;
        int slots = 4;
        float totalW = (slotSize + pad) * slots;
        float x = Screen.width / 2f - totalW / 2f + 150f; // Dịch qua phải một chút
        float y = Screen.height - slotSize - 30;

        // Background panel for skills
        GUI.color = new Color(0.08f, 0.07f, 0.06f, 0.85f);
        GUI.DrawTexture(new Rect(x - 10, y - 5, totalW + 15, slotSize + 15), Texture2D.whiteTexture);
        GUI.color = new Color(0.3f, 0.25f, 0.2f);
        DrawBorder(new Rect(x - 10, y - 5, totalW + 15, slotSize + 15), 2);

        for (int i = 0; i < slots; i++) {
            Rect sR = new Rect(x + i * (slotSize + pad), y, slotSize, slotSize);
            GUI.color = new Color(0.15f, 0.12f, 0.1f);
            GUI.DrawTexture(sR, Texture2D.whiteTexture);
            GUI.color = new Color(0.4f, 0.35f, 0.3f);
            DrawBorder(sR, 1);
            
            // Skill numbers
            GUI.color = Color.gray;
            GUI.Label(new Rect(sR.x+2, sR.y+2, 20, 20), (i+1).ToString(), new GUIStyle(GUI.skin.label){fontSize=9});
            
            // Vẽ Skill thực tế và CD
            if (combat != null && combat.equippedSkills != null && i < combat.equippedSkills.Length && combat.equippedSkills[i] != null) {
                var sk = combat.equippedSkills[i];
                float cd = combat.skillCooldowns[i];
                float maxCd = sk.baseCooldown;
                
                if (sk.icon != null) {
                    GUI.color = (cd > 0) ? new Color(0.4f, 0.4f, 0.4f, 1f) : Color.white;
                    GUI.DrawTexture(new Rect(sR.x+4, sR.y+4, slotSize-8, slotSize-8), sk.icon.texture);
                }
                
                if (cd > 0 && maxCd > 0) {
                    float ratio = Mathf.Clamp01(cd / maxCd);
                    GUI.color = new Color(0, 0, 0, 0.65f);
                    GUI.DrawTexture(new Rect(sR.x+4, sR.y+4 + (slotSize-8)*(1-ratio), slotSize-8, (slotSize-8)*ratio), Texture2D.whiteTexture);
                    GUI.color = Color.white;
                    GUI.Label(sR, Mathf.CeilToInt(cd).ToString(), new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter, fontSize=22, fontStyle=FontStyle.Bold});
                }
            }
        }
    }



    void DrawFantasyBar(float x, float y, float w, float h, float ratio, Color barCol, string label) {
        // Clamp tỉ lệ để tránh bar bắn ra ngoài giới hạn
        float safeRatio = Mathf.Clamp01(ratio);

        // Background
        GUI.color = new Color(0.1f, 0.05f, 0.05f, 1f);
        GUI.DrawTexture(new Rect(x, y, w, h), Texture2D.whiteTexture);
        
        // Progress
        GUI.color = barCol;
        GUI.DrawTexture(new Rect(x + 1, y + 1, (w - 2) * safeRatio, h - 2), Texture2D.whiteTexture);
        
        // Text
        if (!string.IsNullOrEmpty(label)) {
            GUI.color = Color.white;
            DrawShadowLabel(new Rect(x, y, w, h), label, new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter, fontSize=(int)(h*0.7f), fontStyle=FontStyle.Bold}, Color.white);
        }
        
        // Border
        GUI.color = new Color(0.3f, 0.25f, 0.2f);
        DrawBorder(new Rect(x, y, w, h), 1);
    }

    void DrawSimpleBar(float x, float y, float w, float h, float ratio, Color col) {
        GUI.color = Color.black; GUI.DrawTexture(new Rect(x, y, w, h), Texture2D.whiteTexture);
        GUI.color = col; GUI.DrawTexture(new Rect(x, y, w * ratio, h), Texture2D.whiteTexture);
    }

    void DrawBorder(Rect r, int thickness) {
        GUI.DrawTexture(new Rect(r.x, r.y, r.width, thickness), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(r.x, r.yMax - thickness, r.width, thickness), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(r.x, r.y, thickness, r.height), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(r.xMax - thickness, r.y, thickness, r.height), Texture2D.whiteTexture);
    }

    void DrawSolidRect(Rect r, Color c) {
        Color old = GUI.color;
        GUI.color = c;
        GUI.DrawTexture(r, Texture2D.whiteTexture);
        GUI.color = old;
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

    bool DrawSlot(float x, float y, ItemInstance inst, bool s, string defaultLabel) {
        if (slotBgTex != null) {
            GUI.color = s ? Color.yellow : Color.white;
            GUI.DrawTexture(new Rect(x, y, 68, 68), slotBgTex);
        } else {
            GUI.color = s ? Color.yellow : new Color(0.2f, 0.2f, 0.3f);
            GUI.DrawTexture(new Rect(x, y, 68, 68), Texture2D.whiteTexture);
        }
        
        bool clicked = false;
        if (inst != null && inst.data != null) {
            Sprite itemIcon = inst.GetIcon();
            if (itemIcon != null) { GUI.color = Color.white; clicked = GUI.Button(new Rect(x+2, y+2, 64, 64), itemIcon.texture); }
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