using UnityEngine;
using UnityEditor;
using System.IO;

// ===========================
// AUTO ICON ASSIGNER
// Tu dong gan icon cho toan bo ItemData trong Resources/Items/
// Dua vao ItemType de chon icon phu hop
// ===========================
public class AutoIconAssigner : EditorWindow
{
    [MenuItem("⚔️ RPG Tools/Gan Icon Tu Dong Cho Tat Ca Item")]
    public static void AssignAllIcons()
    {
        ItemData[] allItems = Resources.LoadAll<ItemData>("Items");
        int count = 0;

        foreach (var item in allItems)
        {
            if (item == null) continue;

            // 1. Thu tim theo ten iconName (neu ItemData co field nay)
            // 2. Neu khong co, dua vao loai item de chon icon mac dinh
            Sprite icon = FindBestIcon(item);

            if (icon != null && item.icon != icon)
            {
                item.icon = icon;
                EditorUtility.SetDirty(item);
                count++;
                Debug.Log("[AutoIcon] Da gan icon cho: " + item.itemName + " -> " + icon.name);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "Hoan thanh!",
            "Da gan icon cho " + count + " / " + allItems.Length + " vat pham!\nKiem tra console de xem chi tiet.",
            "OK"
        );
    }

    static Sprite FindBestIcon(ItemData item)
    {
        // Danh sach thu tu uu tien: ten chinh xac -> ten goc -> loai item
        string itemName = item.itemName ?? "";

        // --- WEAPON ---
        if (item.type == ItemData.ItemType.Weapon)
        {
            if (itemName.Contains("Kiem") || itemName.Contains("kiếm") || itemName.Contains("Dao") || itemName.Contains("Phay"))
                return LoadSprite("Huyet_Kiem");
            if (itemName.Contains("Cung") || itemName.Contains("No") || itemName.Contains("Nỏ"))
                return LoadSprite("Cung_va_Ten");
            if (itemName.Contains("Khien") || itemName.Contains("Khiên"))
                return LoadSprite("Khien_Thep");
            return LoadSprite("Huyet_Kiem"); // Mac dinh vu khi
        }

        // --- ARMOR ---
        if (item.type == ItemData.ItemType.Armor)
        {
            if (itemName.Contains("Mu") || itemName.Contains("Noi") || itemName.Contains("Nồi") || itemName.Contains("Nap") || itemName.Contains("Nắp"))
                return LoadSprite("Mu_Sat");
            if (itemName.Contains("Giay") || itemName.Contains("Giày") || itemName.Contains("Dep") || itemName.Contains("Dép"))
                return LoadSprite("Giay_Chien_Than");
            if (itemName.Contains("Ao") || itemName.Contains("Áo") || itemName.Contains("Bao") || itemName.Contains("Giap") || itemName.Contains("Giáp"))
                return LoadSprite("Ao_Giap");
            return LoadSprite("Ao_Giap"); // Mac dinh giap
        }

        // --- ACCESSORY ---
        if (item.type == ItemData.ItemType.Accessory)
        {
            if (itemName.Contains("Day") || itemName.Contains("Dây") || itemName.Contains("Soi") || itemName.Contains("Sợi"))
                return LoadSprite("Day_Chuyen");
            return LoadSprite("Nhan_Vang"); // Mac dinh phu kien
        }

        // --- GEM ---
        if (item.type == ItemData.ItemType.Gem)
        {
            if (itemName.Contains("Do") || itemName.Contains("Đỏ") || itemName.Contains("D?"))
                return LoadSprite("Ngoc_Do");
            if (itemName.Contains("Xanh"))
                return LoadSprite("Ngoc_Xanh");
            if (itemName.Contains("Tim") || itemName.Contains("Tím") || itemName.Contains("T?m"))
                return LoadSprite("Ngoc_Tim");
            return LoadSprite("Ngoc_Do"); // Mac dinh ngoc
        }

        // --- CONSUMABLE ---
        if (item.type == ItemData.ItemType.Consumable)
        {
            if (itemName.Contains("Binh") || itemName.Contains("Bình") || itemName.Contains("Nuoc") || itemName.Contains("Nước"))
                return LoadSprite("Binh_Mau_Lon");
            return LoadSprite("Thuc_Pham"); // Mac dinh thuc pham
        }

        // --- QUEST ITEM ---
        if (item.type == ItemData.ItemType.Quest)
            return LoadSprite("Ngoc_Tim");

        return null;
    }

    static Sprite LoadSprite(string name)
    {
        // Thu load truc tiep tu Resources/Icons/
        Sprite sp = Resources.Load<Sprite>("Icons/" + name);
        if (sp != null) return sp;

        // Thu load bang duong dan day du trong Editor
        string path = "Assets/Resources/Icons/" + name + ".png";
        sp = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        return sp;
    }
}
