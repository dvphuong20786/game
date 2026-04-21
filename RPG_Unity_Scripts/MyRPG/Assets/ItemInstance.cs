using System.Collections.Generic;
using UnityEngine;

// ===========================
// L?P ITEM INSTANCE (B?N SAO V?T PH?M)
// Dng qu?n ly item voi H?ng (Rank), Cuong Hoa, va Ng?c.
// ===========================
[System.Serializable]
public class ItemInstance
{
    public ItemData data;
    public int plusLevel = 0;      // Cuong hoa (+1, +2...)
    public int itemRank = 0;       // 0=F, 1=E, 2=D, 3=C, 4=B, 5=A, 6=S
    public List<ItemData> sockets = new List<ItemData>();
    
    // Dong chi so phu tu Hang (Rank)
    public int rankBonusAtk = 0;
    public int rankBonusDef = 0;
    public int rankBonusHp = 0;

    public ItemInstance(ItemData source)
    {
        data = source;
        plusLevel = 0;
        itemRank = 0; // Mac dinh la F
        sockets = new List<ItemData>();
        GenerateRankBonus();
    }

    public string GetRankString() {
        string[] ranks = {"F", "E", "D", "C", "B", "A", "S"};
        return itemRank >= 0 && itemRank < ranks.Length ? ranks[itemRank] : "F";
    }

    public Color GetRankColor() {
        switch(itemRank) {
            case 0: return Color.white;
            case 1: return Color.green;
            case 2: return new Color(0.2f, 0.6f, 1f); // Blue
            case 3: return new Color(0.7f, 0.2f, 1f); // Purple
            case 4: return Color.yellow;
            case 5: return new Color(1f, 0.5f, 0f); // Orange
            case 6: return Color.red;
            default: return Color.white;
        }
    }

    public string GetDisplayName()
    {
        if (data == null) return "Trống";
        string n = data.itemName;
        // Chỉ hiện Rank cho Trang bị (Vũ khí, Giáp, Phụ kiện)
        bool isEquip = data.type == ItemData.ItemType.Weapon || data.type == ItemData.ItemType.Armor || data.type == ItemData.ItemType.Accessory;
        if (isEquip) n = "[" + GetRankString() + "] " + n;
        if (plusLevel > 0) n += " +" + plusLevel;
        return n;
    }

    public Sprite GetIcon() {
        if (data == null) return null;
        if (data.icon != null) return data.icon;
        
        string n = data.itemName.ToLower();
        if (n.Contains("kiếm")) return Resources.Load<Sprite>("Sprites/rusty_blood_sword");
        if (n.Contains("giáp")) return Resources.Load<Sprite>("Sprites/worn_armor");
        if (n.Contains("dây thép") || n.Contains("vòng cổ")) return Resources.Load<Sprite>("Sprites/wire_necklace");
        if (n.Contains("bánh mì")) return Resources.Load<Sprite>("Sprites/moldy_bread");
        if (n.Contains("bình máu")) return Resources.Load<Sprite>("Sprites/dirty_potion");
        if (n.Contains("ngọc")) {
            if (n.Contains("công") || n.Contains("tấn")) return Resources.Load<Sprite>("Sprites/gem_red");
            if (n.Contains("thủ") || n.Contains("phòng")) return Resources.Load<Sprite>("Sprites/gem_blue");
            if (n.Contains("lực") || n.Contains("sinh")) return Resources.Load<Sprite>("Sprites/gem_purple");
        }
        return null;
    }

    public void GenerateRankBonus() {
        // Reset chi so
        rankBonusAtk = 0; rankBonusDef = 0; rankBonusHp = 0;
        if (itemRank == 0) return; // Hang F khong co dong
        
        // Random ra "itemRank" so luong dong
        for (int i=0; i<itemRank; i++) {
            int type = Random.Range(0, 3);
            int power = (itemRank * 2) + Random.Range(1, 5);
            if (type == 0) rankBonusAtk += power;
            else if (type == 1) rankBonusDef += power;
            else rankBonusHp += power * 5;
        }
    }

    // --- XU LY CHI SO CHI TIET ---
    public int GetBaseAtk() { return data != null ? data.atkBonus + (int)(data.atkBonus * 0.1f * plusLevel) : 0; }
    public int GetBaseDef() { return data != null ? data.defBonus + (int)(data.defBonus * 0.1f * plusLevel) : 0; }
    public int GetBaseHp() { return data != null ? data.hpBonus + (int)(data.hpBonus * 0.1f * plusLevel) : 0; }

    public int GetGemAtk() { int b=0; foreach(var g in sockets) if(g!=null) b+=g.atkBonus; return b; }
    public int GetGemDef() { int b=0; foreach(var g in sockets) if(g!=null) b+=g.defBonus; return b; }
    public int GetGemHp() { int b=0; foreach(var g in sockets) if(g!=null) b+=g.hpBonus; return b; }

    public int GetTotalAtk() { return GetBaseAtk() + rankBonusAtk + GetGemAtk(); }
    public int GetTotalDef() { return GetBaseDef() + rankBonusDef + GetGemDef(); }
    public int GetTotalHP() { return GetBaseHp() + rankBonusHp + GetGemHp(); }
}

// ===========================
// H? TH?NG LOGIC TH? REN (H?p nh?t d? s?a l?i Load Asset)
// ===========================
public static class RPG_BlacksmithLogic
{
    public static void EnhanceItem(PlayerStats character, int invIndex)
    {
        var inv = character.SharedInventory;
        if (character == null || invIndex < 0 || invIndex >= inv.Count) return;
        ItemInstance inst = inv[invIndex];
        if (inst.data.type == ItemData.ItemType.Consumable || inst.data.type == ItemData.ItemType.Gem) return;

        int cost = (inst.plusLevel + 1) * 200;
        if (character.gold < cost) return;

        character.gold -= cost;
        float successRate = Mathf.Max(0.1f, 0.9f - (inst.plusLevel * 0.15f));
        
        if (Random.value <= successRate) {
            inst.plusLevel++;
            if (GameUI.instance != null) GameUI.instance.ShowDamage(character.transform.position, "✨ THÀNH CÔNG: +" + inst.plusLevel, Color.green);
        } else {
            if (GameUI.instance != null) GameUI.instance.ShowDamage(character.transform.position, "💥 XỊT!", Color.red);
        }
        character.CalculateBonus();
    }

    // --- GHÉP ĐỒ LÊN HẠNG (RANK UP) ---
    public static void MergeItem(PlayerStats character, int targetIdx, int matIdx)
    {
        var inv = character.SharedInventory;
        if (targetIdx < 0 || matIdx < 0 || targetIdx >= inv.Count || matIdx >= inv.Count) return;
        
        ItemInstance main = inv[targetIdx];
        ItemInstance mat = inv[matIdx];

        if (main.data == null || mat.data == null) return;
        if (main.data.itemName != mat.data.itemName || main.itemRank != mat.itemRank) return;
        if (main.itemRank >= 6) return; // S la max
        
        int cost = (main.itemRank + 1) * 1000;
        if (character.gold < cost) return;

        character.gold -= cost;
        
        // Ti le cuc thap nhu yeu cau
        // F->E: 40%, E->D: 30%, D->C: 20%, C->B: 15%, B->A: 10%, A->S: 5%
        float[] rates = {0.4f, 0.3f, 0.2f, 0.15f, 0.1f, 0.05f, 0f};
        float successRate = rates[main.itemRank];

        if (Random.value <= successRate) {
            main.itemRank++;
            main.GenerateRankBonus(); // Random dong phu
            inv.RemoveAt(matIdx);
            if (GameUI.instance != null) GameUI.instance.ShowDamage(character.transform.position, "🚀 ĐỘT PHÁ LÊN HẠNG " + main.GetRankString(), Color.yellow);
        } else {
            inv.RemoveAt(matIdx); // Xap mat do
            if (GameUI.instance != null) GameUI.instance.ShowDamage(character.transform.position, "💥 ĐỘT PHÁ THẤT BẠI (MẤT ĐỒ)", Color.red);
        }
        character.CalculateBonus();
    }

    // --- HỆ THỐNG CHẾ TẠO (RECIPES) ---
    public static ItemData CheckRecipe(ItemInstance main, ItemInstance mat) {
        if (main == null || mat == null) return null;
        string mainName = main.data.itemName;
        string matName = mat.data.itemName;

        // Công thức 1: Kiếm Nẹp Sắt (Lv 5)
        if (mainName == "Dao Phay Gỉ" && matName == "Ngọc Thủy tinh Đỏ I") return Resources.Load<ItemData>("Items/Kiếm_Nẹp_Sắt");
        
        // Công thức 2: Giáp Tôn Gỉ (Lv 5)
        if (mainName == "Bao Tải Rách" && matName == "Ngọc Thủy tinh Xanh I") return Resources.Load<ItemData>("Items/Giáp_Bao_Tải_Bọc_Tôn");

        // Cong thuc 3: Cung Tho San
        if (mainName == "Nỏ Gỗ Mục" && matName == "Ngọc Thủy tinh Tím I") return Resources.Load<ItemData>("Items/Cung_Thợ_Săn");

        return null; // Không khớp công thức nào
    }

    public static void SocketItem(PlayerStats character, ItemInstance targetItem, ItemInstance gemItem)
    {
        var inv = character.SharedInventory;
        if (!inv.Contains(targetItem) || !inv.Contains(gemItem)) return;
        
        if (gemItem.data.type != ItemData.ItemType.Gem || character.gold < 500) return;

        // Gioi han 5 ngoc
        if (targetItem.sockets.Count >= 5) {
            if (GameUI.instance != null) GameUI.instance.ShowDamage(character.transform.position, "TRANG BỊ ĐÃ ĐẦY NGỌC!", Color.red);
            return;
        }

        character.gold -= 500;
        if (Random.value <= 0.8f) {
            targetItem.sockets.Add(gemItem.data);
            inv.Remove(gemItem);
            if (GameUI.instance != null) GameUI.instance.ShowDamage(character.transform.position, "💎 KHẢM THÀNH CÔNG!", Color.cyan);
        } else {
            inv.Remove(gemItem);
            if (GameUI.instance != null) GameUI.instance.ShowDamage(character.transform.position, "💥 KHẢM THẤT BẠI (MẤT NGỌC)!", Color.red);
        }
        character.CalculateBonus();
    }
}
