using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class RPGAssetGenerator : EditorWindow
{
    [MenuItem("RPG/✨ Tạo TẤT CẢ Vật Phẩm & Kỹ Năng")]
    public static void GenerateAllAssets()
    {
        // 1. Tạo thư mục
        CreateFolder("Assets/Resources");
        CreateFolder("Assets/Resources/Items");
        CreateFolder("Assets/Resources/Skills");

        // 2. Tạo Vật phẩm mẫu
        CreateItem("Kiếm Gỗ", "Vũ khí tân thủ", ItemData.ItemType.Weapon, 8, 0, 0, 50);
        CreateItem("Kiếm Sắt", "Kiếm rèn từ thép tốt", ItemData.ItemType.Weapon, 15, 0, 0, 200);
        CreateItem("Huyết Kiếm", "Thanh bảo kiếm nhuốm máu (Vũ khí 2 tay)", ItemData.ItemType.Weapon, 40, 0, 0, 1000);
        CreateItem("Áo Giáp", "Giáp sắt kiên cố", ItemData.ItemType.Armor, 0, 18, 0, 350);
        CreateItem("Bình Máu Nhỏ", "Hồi 30 HP", ItemData.ItemType.Consumable, 0, 0, 30, 20);
        CreateItem("Bình Máu Lớn", "Hồi 100 HP", ItemData.ItemType.Consumable, 0, 0, 100, 60);

        // 3. Tạo Kỹ năng mẫu
        CreateSkill("Chém Gió", "Chém gió cực mạnh gây sát thương AOE xung quanh.", 4f, 2f, 0.2f, 0.1f);
        CreateSkill("Lôi Đình", "Gọi sấm sét đánh từ trên trời xuống diện rộng.", 8f, 3f, 0.5f, 0.2f);
        CreateSkill("Hộ Vệ", "Tăng phòng thủ cho đồng đội đứng gần.", 10f, 0f, 0f, 0f, 10, 2);
        CreateSkill("Trị Thương", "Hồi phục vết thương cho cả đội.", 15f, 0f, 0f, 0f, 15, 5);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("<color=green>✅ Đã tạo thành công thư mục và toàn bộ Vật phẩm/Kỹ năng mẫu trong Resources!</color>");
        EditorUtility.DisplayDialog("Thành công", "Đã tạo xong toàn bộ vật phẩm và kỹ năng mẫu trong thư mục Resources!", "Tuyệt vời");
    }

    static void CreateFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parent = Path.GetDirectoryName(path).Replace("\\", "/");
            string folderName = Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, folderName);
        }
    }

    static void CreateItem(string name, string desc, ItemData.ItemType type, int atk, int def, int heal, int price)
    {
        string path = $"Assets/Resources/Items/{name}.asset";
        if (File.Exists(path)) return;

        ItemData item = ScriptableObject.CreateInstance<ItemData>();
        item.itemName = name;
        item.description = desc;
        item.type = type;
        item.atkBonus = atk;
        item.defBonus = def;
        item.healAmount = heal;
        item.price = price;

        AssetDatabase.CreateAsset(item, path);
    }

    static void CreateSkill(string name, string desc, float cd, float baseDmg, float incDmg, float incRange, int baseHealDef = 0, int incHealDef = 0)
    {
        string path = $"Assets/Resources/Skills/{name}.asset";
        if (File.Exists(path)) return;

        SkillData skill = ScriptableObject.CreateInstance<SkillData>();
        skill.skillName = name;
        skill.baseCooldown = cd;
        skill.baseDamageMultiplier = baseDmg;
        skill.damageIncreasePerLevel = incDmg;
        skill.rangeIncreasePerLevel = incRange;
        skill.baseHealOrDef = baseHealDef;
        skill.valueIncreasePerLevel = incHealDef;

        // Tạo mô tả cho 10 cấp độ (Dùng List thay vì Array)
        skill.levelDescriptions = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            skill.levelDescriptions.Add($"{desc} (Cấp {i + 1})");
        }

        AssetDatabase.CreateAsset(skill, path);
    }
}
