using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "RPG/Item")]
public class ItemData : ScriptableObject
{
    public enum ItemType { Weapon, Armor, Consumable, Accessory, Quest, Gem }

    [Header("Thông tin cơ bản")]
    public string itemName = "Vật phẩm mới";
    public Sprite icon;
    [TextArea(3, 5)]
    public string description = "Mô tả vật phẩm...";
    public int price = 100;
    public int requiredLevel = 0;
    public ItemType type = ItemType.Quest;

    [Header("Chỉ số cộng thêm")]
    public int atkBonus = 0;
    public int defBonus = 0;
    public int hpBonus = 0;
    public bool isTwoHanded = false;

    [Header("Dành cho Vật phẩm Tiêu thụ")]
    public int healAmount = 0;
    
    // Kiểm tra xem có phải bình máu không
    public bool IsPotion() {
        return itemName.Contains("Bình Máu") || (type == ItemType.Consumable && healAmount > 0);
    }
}
