using UnityEngine;
using System.Collections.Generic;

public class BlacksmithUI : MonoBehaviour
{
    public static BlacksmithUI instance;
    public bool isOpen = false;

    // Các ô Slot
    public ItemInstance targetItem;   // Đồ cần nâng/khảm
    public ItemInstance materialItem; // Ngọc hoặc nguyên liệu
    public int targetIdx = -1;
    public int materialIdx = -1;

    public enum Mode { Enhance, Socket, Craft }
    public Mode currentMode = Mode.Enhance;

    void Awake() { instance = this; }

    public void Open() { isOpen = true; ResetSlots(); }
    public void Close() { isOpen = false; }

    public void ResetSlots() {
        targetItem = null; materialItem = null;
        targetIdx = -1; materialIdx = -1;
    }

    public void SetTarget(int invIdx) {
        if (GameUI.instance == null) return;
        var inv = GameUI.instance.player.SharedInventory;
        if (invIdx >= 0 && invIdx < inv.Count) {
            targetItem = inv[invIdx];
            targetIdx = invIdx;
        }
    }

    public void SetMaterial(int invIdx) {
        if (GameUI.instance == null) return;
        var inv = GameUI.instance.player.SharedInventory;
        if (invIdx >= 0 && invIdx < inv.Count) {
            materialItem = inv[invIdx];
            materialIdx = invIdx;
        }
    }

    public void Execute() {
        if (targetItem == null || GameUI.instance == null) return;
        var player = GameUI.instance.player;

        // 1. KIỂM TRA CHẾ TẠO (QUAN TRỌNG NHẤT)
        ItemData craftResult = RPG_BlacksmithLogic.CheckRecipe(targetItem, materialItem);
        if (craftResult != null) {
            // Thực hiện chế tạo
            player.SharedInventory.RemoveAt(targetIdx);
            // materialIdx có thể đã thay đổi sau khi xóa targetIdx, nên phải check cẩn thận
            int newMatIdx = player.SharedInventory.IndexOf(materialItem);
            if (newMatIdx != -1) player.SharedInventory.RemoveAt(newMatIdx);
            
            player.PickUpItem(craftResult);
            if (GameUI.instance != null) GameUI.instance.ShowDamage(player.transform.position, "⚒️ CHẾ TẠO THÀNH CÔNG: " + craftResult.itemName, Color.yellow);
            ResetSlots();
            return;
        }

        // 2. KHẢM NGỌC HOẶC CƯỜNG HÓA NHƯ CŨ
        if (materialItem != null && materialItem.data.type == ItemData.ItemType.Gem) {
            RPG_BlacksmithLogic.SocketByInventoryIndex(player, targetIdx, materialIdx);
        } else {
            RPG_BlacksmithLogic.EnhanceItem(player, targetIdx);
        }
        
        ResetSlots();
    }
}
