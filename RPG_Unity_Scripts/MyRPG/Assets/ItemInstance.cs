using System.Collections.Generic;
using UnityEngine;

// ===========================
// LỚP ITEM INSTANCE (BẢN SAO VẬT PHẨM)
// Dùng để quản lý từng món đồ riêng biệt với cấp độ nâng cấp và ngọc khảm khác nhau
// ===========================
[System.Serializable]
public class ItemInstance
{
    public ItemData data;          // Tham chiếu đến dữ liệu gốc (Kiếm, Giáp...)
    public int plusLevel = 0;      // Cấp cường hóa (+1, +2...)
    public List<ItemData> sockets = new List<ItemData>(); // Danh sách ngọc đã khảm

    public ItemInstance(ItemData source)
    {
        data = source;
        plusLevel = 0;
        sockets = new List<ItemData>();
    }

    // Lấy tên hiển thị kèm cấp độ nâng cấp
    public string GetDisplayName()
    {
        if (data == null) return "Trống";
        return data.itemName + (plusLevel > 0 ? " +" + plusLevel : "");
    }

    // Tính toán sức tấn công cuối cùng (Gốc + Cường hóa + Ngọc)
    public int GetTotalAtk()
    {
        if (data == null) return 0;
        int bonus = data.atkBonus;
        // Mỗi cấp cộng thêm 10% chỉ số gốc
        bonus += (int)(data.atkBonus * 0.1f * plusLevel);
        // Cộng chỉ số từ ngọc khảm (Ví dụ: Ngọc Đỏ tăng ATK)
        foreach (var gem in sockets) {
            if (gem != null) bonus += gem.atkBonus;
        }
        return bonus;
    }

    // Tính toán phòng thủ cuối cùng
    public int GetTotalDef()
    {
        if (data == null) return 0;
        int bonus = data.defBonus;
        bonus += (int)(data.defBonus * 0.1f * plusLevel);
        foreach (var gem in sockets) {
            if (gem != null) bonus += gem.defBonus;
        }
        return bonus;
    }

    // Tính toán HP cuối cùng
    public int GetTotalHP()
    {
        if (data == null) return 0;
        int bonus = data.hpBonus;
        bonus += (int)(data.hpBonus * 0.1f * plusLevel);
        foreach (var gem in sockets) {
            if (gem != null) bonus += gem.hpBonus;
        }
        return bonus;
    }
}

// ===========================
// HỆ THỐNG LOGIC THỢ RÈN (Hợp nhất để sửa lỗi Load Asset)
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

    public static void SocketIntoSlot(PlayerStats character, string slot, int gemIdx)
    {
        var inv = character.SharedInventory;
        if (character == null || gemIdx < 0 || gemIdx >= inv.Count) return;
        ItemInstance gem = inv[gemIdx];
        if (gem.data == null || gem.data.type != ItemData.ItemType.Gem || character.gold < 500) return;

        ItemInstance target = null;
        if      (slot == "Head") target = character.eqHead;
        else if (slot == "Body") target = character.eqBody;
        else if (slot == "Legs") target = character.eqLegs;
        else if (slot == "WepMain") target = character.eqWeaponMain;
        else if (slot == "WepOff") target = character.eqWeaponOff;
        else if (slot == "Ring1") target = character.eqRing1;
        else if (slot == "Ring2") target = character.eqRing2;
        else if (slot == "Neck") target = character.eqNecklace;
        else if (slot == "Ancient") target = character.eqAncientGold;

        if (target == null) return;

        character.gold -= 500;
        if (Random.value <= 0.7f) {
            target.sockets.Add(gem.data);
            inv.RemoveAt(gemIdx);
            if (GameUI.instance != null) GameUI.instance.ShowDamage(character.transform.position, "💎 KHẢM THÀNH CÔNG!", Color.cyan);
        } else {
            inv.RemoveAt(gemIdx);
            if (GameUI.instance != null) GameUI.instance.ShowDamage(character.transform.position, "💥 KHẢM THẤT BẠI!", Color.red);
        }
        character.CalculateBonus();
    }
}
