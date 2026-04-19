using UnityEngine;
using UnityEditor;
using System.IO;

public class MasterItemGenerator : EditorWindow
{
    [MenuItem("⚒️ RPG Tools/Lò Đúc Vật Phẩm Thần Cấp")]
    public static void ShowWindow()
    {
        GetWindow<MasterItemGenerator>("Lò Đúc Ngọc");
    }

    void OnGUI()
    {
        GUILayout.Label("🛠 HỆ THỐNG KHỞI TẠO VẬT PHẨM TỰ ĐỘNG", EditorStyles.boldLabel);
        if (GUILayout.Button("🔥 KÍCH HOẠT ĐÚC 15 LOẠI NGỌC & THỨC ĂN", GUILayout.Height(50)))
        {
            GenerateAllItems();
        }
    }

    void GenerateAllItems()
    {
        string path = "Assets/Resources/Items";
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        // 1. ĐÚC NGỌC ĐỎ (CÔNG)
        CreateGems("Ngọc Đỏ", "Ngoc_Do", ItemData.ItemType.Gem, 10, 0, 0, 200);
        
        // 2. ĐÚC NGỌC XANH (THỦ)
        CreateGems("Ngọc Xanh", "Ngoc_Xanh", ItemData.ItemType.Gem, 0, 5, 0, 150);
        
        // 3. ĐÚC NGỌC TÍM (MÁU)
        CreateGems("Ngọc Tím", "Ngoc_Tim", ItemData.ItemType.Gem, 0, 0, 50, 100);

        // 4. ĐÚC THỨC ĂN
        CreateItem("Bánh Mì", "Thuc_Pham", ItemData.ItemType.Consumable, 0, 0, 0, 20, "Bánh mì thơm ngon, hồi 20 HP.", 50);
        CreateItem("Thịt Nướng", "Thuc_Pham", ItemData.ItemType.Consumable, 0, 0, 0, 50, "Thịt nướng mọng nước, hồi 50 HP.", 120);

        // 5. ĐÚC LINH HỒN (Để đột phá RANK)
        CreateItem("Linh hồn Hiệp sĩ", "Ngoc_Tim", ItemData.ItemType.Quest, 0, 0, 0, 0, "Dùng để đột phá Rank cho Hiệp Sĩ.", 5000);
        CreateItem("Linh hồn Archer", "Ngoc_Tim", ItemData.ItemType.Quest, 0, 0, 0, 0, "Dùng để đột phá Rank cho Cung Thủ.", 5000);
        CreateItem("Linh hồn Smile_detu", "Ngoc_Tim", ItemData.ItemType.Quest, 0, 0, 0, 0, "Dùng để đột phá Rank cho Slime.", 5000);

        // 6. ĐÚC VŨ KHÍ TẦM XA (BOW) (MỚI)
        CreateItem("Cung Gỗ", "Cung_va_Ten", ItemData.ItemType.Weapon, 20, 0, 0, 0, "Cung gỗ cơ bản cho Archer. Tầm xa: 6m.", 350);
        CreateItem("Cung Thợ Săn", "Cung_va_Ten", ItemData.ItemType.Weapon, 45, 0, 0, 0, "Cung dành cho thợ săn chuyên nghiệp.", 1200);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Thành công", "✅ Đã đúc xong 15 loại Ngọc và Thực phẩm vào Resources/Items!", "Tuyệt vời");
    }

    void CreateGems(string baseName, string iconName, ItemData.ItemType type, int atk, int def, int hp, int basePrice)
    {
        for (int i = 1; i <= 5; i++)
        {
            string tier = "";
            switch(i) {
                case 1: tier = "I"; break;
                case 2: tier = "II"; break;
                case 3: tier = "III"; break;
                case 4: tier = "IV"; break;
                case 5: tier = "V"; break;
            }

            int multiplier = (int)Mathf.Pow(2.2f, i - 1); // Tăng tiến sức mạnh theo bậc
            CreateItem($"{baseName} {tier}", iconName, type, atk * multiplier, def * multiplier, hp * multiplier, 0, $"Ngọc {baseName} bậc {tier}. Linh khí hội tụ.", basePrice * multiplier);
        }
    }

    void CreateItem(string name, string iconName, ItemData.ItemType type, int atk, int def, int hp, int heal, string desc, int price)
    {
        ItemData item = ScriptableObject.CreateInstance<ItemData>();
        item.itemName = name;
        item.type = type;
        item.atkBonus = atk;
        item.defBonus = def;
        item.hpBonus = hp;
        item.healAmount = heal;
        item.description = desc;
        item.price = price;

        // Tìm icon
        string iconPath = $"Assets/Resources/Icons/{iconName}.png";
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
        if (sprite != null) item.icon = sprite;

        string fileName = $"Assets/Resources/Items/{name.Replace(" ", "_")}.asset";
        AssetDatabase.CreateAsset(item, fileName);
    }
}
