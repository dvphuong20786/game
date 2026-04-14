using UnityEngine;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class GameUI : MonoBehaviour
{
    public static GameUI instance;

    class DamageText { public Vector3 worldPos; public string text; public Color color; public float timer; public float alpha = 1f; }
    private List<DamageText> damageTexts = new List<DamageText>();
    private GUIStyle dmgStyle; private GUIStyle expStyle; private GUIStyle goldStyle;

    private PlayerStats player;
    private PlayerCombat combat;
    private bool isBagOpen = false;
    private int currentTab = 0; // 0: Trang bị, 1: Kỹ năng

    private int selectedItemIdx = -1;
    private string selectedSlot = "";
    private Vector2 scrollPos;

    void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    void Start() { player = FindAnyObjectByType<PlayerStats>(); combat = FindAnyObjectByType<PlayerCombat>(); }

    public void ShowDamage(Vector3 pos, string txt, Color col) { damageTexts.Add(new DamageText { worldPos = pos, text = txt, color = col, timer = 1.0f }); }

    void Update()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame) { isBagOpen = !isBagOpen; ResetSelection(); }
#else
        if (Input.GetKeyDown(KeyCode.B)) { isBagOpen = !isBagOpen; ResetSelection(); }
#endif
        for (int i = damageTexts.Count - 1; i >= 0; i--) {
            damageTexts[i].timer -= Time.deltaTime; damageTexts[i].worldPos += Vector3.up * Time.deltaTime * 0.5f;
            damageTexts[i].alpha = damageTexts[i].timer; if (damageTexts[i].timer <= 0) damageTexts.RemoveAt(i);
        }
    }

    void ResetSelection() { selectedItemIdx = -1; selectedSlot = ""; }

    void OnGUI()
    {
        if (player == null || combat == null) return;
        DrawHUD();

        if (isBagOpen)
        {
            GUI.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
            Rect mainR = new Rect(Screen.width/2 - 425, 60, 850, 520);
            GUI.DrawTexture(mainR, Texture2D.whiteTexture);
            
            GUI.color = Color.red; if (GUI.Button(new Rect(mainR.xMax - 40, mainR.y + 10, 30, 30), "X")) isBagOpen = false;

            // HỆ THỐNG TAB
            GUI.color = currentTab == 0 ? Color.yellow : Color.gray;
            if (GUI.Button(new Rect(mainR.x + 20, mainR.y + 15, 120, 35), "🛡 TRANG BỊ")) currentTab = 0;
            GUI.color = currentTab == 1 ? Color.yellow : Color.gray;
            if (GUI.Button(new Rect(mainR.x + 150, mainR.y + 15, 120, 35), "🔥 KỸ NĂNG")) currentTab = 1;

            if (currentTab == 0) DrawCharacterTab();
            else DrawSkillsTab();
            
            DrawFooter(mainR);
        }
        DrawFloatingDamage();
    }

    void DrawCharacterTab()
    {
        float pdX = Screen.width/2 - 405; float pdY = 120;
        GUI.color = new Color(0.15f, 0.15f, 0.15f, 1f);
        GUI.DrawTexture(new Rect(pdX, pdY, 240, 320), Texture2D.whiteTexture);
        GUI.color = Color.white; GUI.Label(new Rect(pdX + 60, pdY + 5, 150, 25), "THÂN PHÁP & ĐỒ");

        // Hàng 1: Đầu - Cổ
        if (DrawSlot(pdX+20, pdY+35, "Đầu\n"+player.eqHead, selectedSlot=="Head")) { selectedSlot="Head"; selectedItemIdx=-1; }
        if (DrawSlot(pdX+90, pdY+35, "Dây\n"+player.eqNecklace, selectedSlot=="Neck")) { selectedSlot="Neck"; selectedItemIdx=-1; }
        if (DrawSlot(pdX+160, pdY+35, "Vàng Cổ\n"+player.eqAncientGold, selectedSlot=="Ancient")) { selectedSlot="Ancient"; selectedItemIdx=-1; }

        // Hàng 2: Trái (Main) - Thân - Phải (Shield)
        if (DrawSlot(pdX+20, pdY+105, "Vũ khí\n"+player.eqWeaponMain, selectedSlot=="WepMain")) { selectedSlot="WepMain"; selectedItemIdx=-1; }
        if (DrawSlot(pdX+90, pdY+105, "Áo\n"+player.eqBody, selectedSlot=="Body")) { selectedSlot="Body"; selectedItemIdx=-1; }
        if (DrawSlot(pdX+160, pdY+105, "Khiên\n"+player.eqWeaponOff, selectedSlot=="WepOff")) { selectedSlot="WepOff"; selectedItemIdx=-1; }

        // Hàng 3: Nhẫn 1 - Giày - Nhẫn 2
        if (DrawSlot(pdX+20, pdY+175, "Nhẫn 1\n"+player.eqRing1, selectedSlot=="Ring1")) { selectedSlot="Ring1"; selectedItemIdx=-1; }
        if (DrawSlot(pdX+90, pdY+175, "Giày\n"+player.eqLegs, selectedSlot=="Legs")) { selectedSlot="Legs"; selectedItemIdx=-1; }
        if (DrawSlot(pdX+160, pdY+175, "Nhẫn 2\n"+player.eqRing2, selectedSlot=="Ring2")) { selectedSlot="Ring2"; selectedItemIdx=-1; }

        GUI.color = Color.green; GUI.Label(new Rect(pdX+10, pdY+245, 230, 25), "Lực chiến: " + (player.bonusDamage + (combat!=null?combat.attackDamage:0)));
        GUI.color = Color.cyan; GUI.Label(new Rect(pdX+10, pdY+265, 230, 25), "Phòng thủ: " + player.bonusDefense);
        
        DrawInventory(Screen.width/2 - 145, pdY);
        DrawTooltip(Screen.width/2 + 250, pdY);
    }

    void DrawSkillsTab()
    {
        float skX = Screen.width/2 - 405; float skY = 120;
        GUI.color = Color.white;
        GUI.Label(new Rect(skX, skY, 400, 30), "ĐIỂM KỸ NĂNG: " + player.skillPoints + " (Tăng 1 điểm sau mỗi 3 level)");
        
        string[] skills = { "Chém Gió (Lv3)", "Lôi Đình (Lv6)", "Bất Tử (Lv9)", "Thần Tiên (Lv12)" };
        for (int i = 0; i < skills.Length; i++)
        {
            Rect skR = new Rect(skX, skY + 40 + (i * 60), 300, 50);
            bool unlocked = player.unlockedSkills.Contains(skills[i]);
            GUI.color = unlocked ? Color.green : Color.gray;
            GUI.Box(skR, "");
            GUI.Label(new Rect(skR.x+10, skR.y+15, 200, 30), skills[i] + (unlocked ? " [ĐÃ HỌC]" : ""));
            
            if (!unlocked && player.level >= (i+1)*3 && player.skillPoints > 0)
            {
                GUI.color = Color.yellow;
                if (GUI.Button(new Rect(skR.xMax - 80, skR.y + 10, 70, 30), "HỌC")) player.LearnSkill(skills[i]);
            }
        }
    }

    void DrawInventory(float x, float y)
    {
        GUI.color = Color.white; GUI.Label(new Rect(x, y, 150, 30), "HÒM ĐỒ");
        Rect vR = new Rect(x, y+30, 380, 320); Rect cR = new Rect(0,0, 360, Mathf.Max(320, player.inventory.Count*35));
        scrollPos = GUI.BeginScrollView(vR, scrollPos, cR);
        for (int i = 0; i < player.inventory.Count; i++) {
            GUI.color = selectedItemIdx == i ? Color.yellow : Color.white;
            if (GUI.Button(new Rect(0, i*35, 360, 30), player.inventory[i])) { selectedItemIdx = i; selectedSlot = ""; }
        }
        GUI.EndScrollView();
    }

    void DrawTooltip(float x, float y)
    {
        GUI.color = new Color(0.1f, 0.2f, 0.4f, 1f);
        GUI.DrawTexture(new Rect(x, y, 160, 320), Texture2D.whiteTexture);
        string n = "";
        if (selectedItemIdx >= 0 && selectedItemIdx < player.inventory.Count) n = player.inventory[selectedItemIdx];
        else if (selectedSlot!="") {
            if (selectedSlot=="Head") n=player.eqHead; else if (selectedSlot=="Body") n=player.eqBody; else if (selectedSlot=="Legs") n=player.eqLegs;
            else if (selectedSlot=="WepMain") n=player.eqWeaponMain; else if (selectedSlot=="WepOff") n=player.eqWeaponOff;
            else if (selectedSlot=="Ring1") n=player.eqRing1; else if (selectedSlot=="Ring2") n=player.eqRing2;
            else if (selectedSlot=="Neck") n=player.eqNecklace; else if (selectedSlot=="Ancient") n=player.eqAncientGold;
        }

        if (n != "" && n != "Trống" && n != "Tay Không") {
            GUI.color = Color.yellow; GUI.Label(new Rect(x+10, y+10, 140, 50), "SOI: " + n);
            GUI.color = Color.white; GUI.Label(new Rect(x+10, y+60, 140, 100), "Giá: "+player.GetItemPrice(n)+" Vàng\n(Đồ có Ngọc sẽ mạnh hơn)");
            if (selectedItemIdx!=-1) {
                GUI.color = Color.green;
                if (n.Contains("Nhẫn")) {
                   if (GUI.Button(new Rect(x+10, y+180, 140, 30), "MẶC Ô 1")) player.EquipItem(selectedItemIdx, "Ring1");
                   if (GUI.Button(new Rect(x+10, y+215, 140, 30), "MẶC Ô 2")) player.EquipItem(selectedItemIdx, "Ring2");
                } else if (GUI.Button(new Rect(x+10, y+180, 140, 40), "MẶC VÀO")) player.EquipItem(selectedItemIdx);
                GUI.color = Color.red; if (GUI.Button(new Rect(x+10, y+260, 140, 30), "BÁN ĐI")) player.SellItem(selectedItemIdx);
            } else {
                GUI.color = Color.red; if (GUI.Button(new Rect(x+10, y+180, 140, 40), "GỠ RA")) player.Unequip(selectedSlot);
            }
        }
    }

    void DrawHUD()
    {
        GUI.color = Color.black; GUI.DrawTexture(new Rect(20, 20, 250, 30), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(20, 52, 250, 15), Texture2D.whiteTexture);
        GUI.color = Color.red; GUI.DrawTexture(new Rect(22, 22, ((float)player.currentHealth/player.maxHealth)*246f, 26), Texture2D.whiteTexture);
        GUI.color = Color.blue; GUI.DrawTexture(new Rect(22, 54, ((float)player.currentExp/player.expToNextLevel)*246f, 11), Texture2D.whiteTexture);
        GUI.color = Color.white; GUI.Label(new Rect(30, 25, 200, 30), "HP: " + player.currentHealth);
        if (expStyle==null) expStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
        GUI.color = Color.black; GUI.Label(new Rect(31, 51, 250, 30), "LV "+player.level+" | EXP: "+player.currentExp, expStyle);
        GUI.color = Color.cyan; GUI.Label(new Rect(30, 50, 250, 30), "LV "+player.level+" | EXP: "+player.currentExp, expStyle);
        if (goldStyle==null) goldStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
        GUI.color = Color.black; GUI.Label(new Rect(31, 71, 200, 30), "VÀNG: " + player.gold, goldStyle);
        GUI.color = Color.yellow; GUI.Label(new Rect(30, 70, 200, 30), "VÀNG: " + player.gold, goldStyle);
    }

    void DrawFooter(Rect r) {
        GUI.color = Color.green; if (GUI.Button(new Rect(r.x+20, r.yMax-50, 100, 30), "💾 LƯU")) player.SaveGame();
        GUI.color = Color.cyan; if (GUI.Button(new Rect(r.x+130, r.yMax-50, 100, 30), "📂 LOAD")) player.LoadGame();
    }

    bool DrawSlot(float x, float y, string c, bool s) {
        GUI.color = s ? Color.yellow : Color.gray; GUI.DrawTexture(new Rect(x,y,60,60), Texture2D.whiteTexture);
        GUI.color = Color.black; return GUI.Button(new Rect(x,y,60,60), c, GUIStyle.none) || GUI.Button(new Rect(x+5,y+10,50,45), c, GUI.skin.label);
    }

    void DrawFloatingDamage() {
        if (dmgStyle == null) dmgStyle = new GUIStyle(GUI.skin.label) { fontSize = 22, fontStyle = FontStyle.Bold };
        Camera cam = Camera.main;
        foreach (var dt in damageTexts) {
            Vector3 sPos = cam.WorldToScreenPoint(dt.worldPos);
            if (sPos.z > 0) { GUI.color = new Color(dt.color.r, dt.color.g, dt.color.b, dt.alpha); GUI.Label(new Rect(sPos.x-20, Screen.height-sPos.y-25, 100, 50), dt.text, dmgStyle); }
        }
    }
}
