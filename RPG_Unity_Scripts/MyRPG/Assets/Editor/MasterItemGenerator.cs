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

        // 1. ĐÚC NGỌC "RẺ RÁCH" (Mảnh kính vụn)
        CreateGems("Thủy tinh Đỏ", "Ngoc_Do", ItemData.ItemType.Gem, 10, 0, 0, 10);
        CreateGems("Đá cuội Xanh", "Ngoc_Xanh", ItemData.ItemType.Gem, 0, 5, 0, 8);
        CreateGems("Nhựa vụn Tím", "Ngoc_Tim", ItemData.ItemType.Gem, 0, 0, 50, 5);

        // 2. ĐÚC THỨC ĂN "NHẶT ĐƯỢC"
        CreateItem("Bánh Mì Mốc", "Thuc_Pham", ItemData.ItemType.Consumable, 0, 0, 0, 20, "Mẩu bánh mì nhặt ở bãi rác, hồi 20 HP.", 5);
        CreateItem("Xương Gặm Dở", "Thuc_Pham", ItemData.ItemType.Consumable, 0, 0, 0, 50, "Miếng xương còn sót lại tí thịt cháy.", 12);

        // 3. ĐÚC LINH HỒN (Vía)
        CreateItem("Vía Hiệp sĩ", "Ngoc_Tim", ItemData.ItemType.Quest, 0, 0, 0, 0, "Tàn dư linh hồn nghèo khổ.", 500);
        CreateItem("Vía Archer", "Ngoc_Tim", ItemData.ItemType.Quest, 0, 0, 0, 0, "Linh hồn kẻ bắn trộm.", 500);
        CreateItem("Vía Smile_detu", "Ngoc_Tim", ItemData.ItemType.Quest, 0, 0, 0, 0, "Linh hồn bùn đất.", 500);

        // 4. ĐÚC VŨ KHÍ TẦM XA
        CreateItem("Nỏ Gỗ Mục", "Cung_va_Ten", ItemData.ItemType.Weapon, 20, 0, 0, 0, "Cái nỏ cổ lỗ sĩ sắp gãy.", 150, true);

        // 5. ĐÚC TRANG BỊ CÁI BANG (9 SLOT)
        CreateItem("Dao Phay Gỉ", "Huyet_Kiem", ItemData.ItemType.Weapon, 150, 0, 0, 0, "Thanh sắt gỉ bám đầy bùn đất.", 500);
        CreateItem("Bao Tải Rách", "Ao_Giap", ItemData.ItemType.Armor, 0, 40, 200, 0, "Một chiếc bao tải vá víu để che thân.", 200);
        CreateItem("Nồi Rỉ Sét", "Mu_Sat", ItemData.ItemType.Armor, 0, 15, 50, 0, "Cái nồi thủng đáy dùng làm mũ.", 100);
        CreateItem("Dép Rơm", "Giay_Chien_Than", ItemData.ItemType.Armor, 10, 10, 10, 0, "Đôi dép rơm đi bộ mòn cả gót.", 50);
        CreateItem("Dây Thép Buộc", "Nhan_Vang", ItemData.ItemType.Accessory, 20, 5, 20, 0, "Một đoạn dây thép uốn thành nhẫn.", 80);
        CreateItem("Sợi Dây Thừng", "Day_Chuyen", ItemData.ItemType.Accessory, 5, 5, 100, 0, "Sợi dây thừng nhặt ở bến tàu.", 100);
        
        CreateItem("Nắp Thùng Rác", "Ao_Giap", ItemData.ItemType.Armor, 0, 30, 0, 0, "Nắp thùng rác méo mó bảo vệ tay.", 150);
        CreateItem("Hòn Đá Bùn", "Nhan_Vang", ItemData.ItemType.Accessory, 50, 50, 50, 0, "Vật phẩm tổ truyền của kẻ ăn xin.", 1000);

        // 6. ĐÚC BÌNH MÁU "HẾT DATE"
        CreateItem("Nước Đỏ Đục", "Binh_Mau_Lon", ItemData.ItemType.Consumable, 0, 0, 0, 200, "Thứ nước màu đỏ không rõ nguồn gốc uốn vào hồi 200 HP.", 30);

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

    void CreateItem(string name, string iconName, ItemData.ItemType type, int atk, int def, int hp, int heal, string desc, int price, bool isTwoHanded = false)
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
        item.isTwoHanded = isTwoHanded;

        // Tìm icon
        string iconPath = $"Assets/Resources/Icons/{iconName}.png";
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
        if (sprite != null) item.icon = sprite;

        string fileName = $"Assets/Resources/Items/{name.Replace(" ", "_")}.asset";
        AssetDatabase.CreateAsset(item, fileName);
    }
}
