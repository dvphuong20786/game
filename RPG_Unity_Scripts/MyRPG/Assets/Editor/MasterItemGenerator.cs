using UnityEngine;
using UnityEditor;
using System.IO;

// ===========================
// LÒ ĐÚC VẬT PHẨM - PHIÊN BẢN TIẾNG VIỆT CHUẨN (CÁI BANG)
// Mục tiêu: Tạo vật phẩm với tên "Nghèo Khổ" đã bàn bạc.
// Fix: Dùng tên có dấu cho hiển thị, nhưng tên File không dấu để tránh lỗi Win/Unity.
// ===========================
public class MasterItemGenerator : EditorWindow
{
    [MenuItem("⚔️ RPG Tools/Kích Hoạt Đúc Vật Phẩm")]
    public static void RunGenerate()
    {
        GetWindow<MasterItemGenerator>("Lò Đúc Items").GenerateAllItems();
    }

    [MenuItem("⚔️ RPG Tools/Lò Đúc Vật Phẩm (Mở Cửa Sổ)")]
    public static void ShowWindow()
    {
        GetWindow<MasterItemGenerator>("Lò Đúc Items");
    }

    void OnGUI()
    {
        GUILayout.Label("HỆ THỐNG KHỞI TẠO VẬT PHẨM (CÁI BANG)", EditorStyles.boldLabel);
        GUILayout.Space(10);
        if (GUILayout.Button("🔥 KÍCH HOẠT ĐÚC TOÀN BỘ VẬT PHẨM", GUILayout.Height(50)))
            GenerateAllItems();
        GUILayout.Space(5);
        if (GUILayout.Button("✨ Gán Icon Tự Động (Cho Item Hiện Tại)", GUILayout.Height(35)))
            AutoIconAssigner.AssignAllIcons();
    }

    void GenerateAllItems()
    {
        string path = "Assets/Resources/Items";
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        // === NGỌC ===
        CreateGems("Ngọc Thủy Tinh Đỏ",   "Ngoc_Do",   ItemData.ItemType.Gem, 10, 0,  0,  10);
        CreateGems("Ngọc Thủy Tinh Xanh", "Ngoc_Xanh", ItemData.ItemType.Gem, 0,  5,  0,  8);
        CreateGems("Ngọc Thủy Tinh Tím",  "Ngoc_Tim",  ItemData.ItemType.Gem, 0,  0,  50, 5);

        // === THỨC ĂN ===
        CreateItem("Bánh Mì Mốc",    "Banh_Mi_Moc",   "Thuc_Pham", ItemData.ItemType.Consumable, 0,0,0,20,  "Mẩu bánh mì nhặt ở bãi rác, hồi 20 HP.", 5);
        CreateItem("Thịt Nướng",     "Thit_Nuong", "Thuc_Pham", ItemData.ItemType.Consumable, 0,0,0,80,  "Miếng thịt nướng còn nóng, hồi 80 HP.", 25);
        CreateItem("Xương Gặm Dở",   "Xuong_Gam_Do", "Thuc_Pham", ItemData.ItemType.Consumable, 0,0,0,50,  "Miếng xương còn sót lại tí thịt dai.", 12);
        CreateItem("Nước Dự Đục",    "Nuoc_Du_Duc", "Binh_Mau_Lon", ItemData.ItemType.Consumable, 0,0,0,200, "Thứ nước màu đỏ lạ, uống vào hồi 200 HP.", 30);

        // === LINH HỒN (Quest) ===
        CreateItem("Linh Hồn Hiệp Sĩ", "Linh_Hon_Hiep_Si", "Ngoc_Tim", ItemData.ItemType.Quest, 0,0,0,0, "Tàn dư linh hồn nghèo khổ.", 500);
        CreateItem("Linh Hồn Archer",  "Linh_Hon_Archer",  "Ngoc_Tim", ItemData.ItemType.Quest, 0,0,0,0, "Linh hồn kẻ săn trộm.", 500);
        CreateItem("Linh Hồn Slime",   "Linh_Hon_Slime",   "Ngoc_Tim", ItemData.ItemType.Quest, 0,0,0,0, "Linh hồn bùn xỉn.", 500);

        // === VŨ KHÍ ===
        CreateItem("Nỏ Gỗ Mục",      "No_Go_Muc",    "Cung_va_Ten", ItemData.ItemType.Weapon, 20,0,0,0, "Cái nỏ cũ rơ lỏng sắp gãy.", 150, true);
        CreateItem("Dao Phay Gỉ",    "Dao_Phay_Gi",  "Huyet_Kiem",  ItemData.ItemType.Weapon, 150,0,0,0,"Thanh sắt gỉ bám đầy bùn đất.", 500);
        CreateItem("Kiếm Nẹp Sắt",   "Kiem_Nep_Sat", "Huyet_Kiem",  ItemData.ItemType.Weapon, 350,0,0,0, "Kiếm rẻ nẹp thêm sắt. (Bậc 2)", 1500, false, 5);
        CreateItem("Cung Thợ Săn",   "Cung_Tho_San", "Cung_va_Ten", ItemData.ItemType.Weapon, 250,0,0,0, "Cung săn thú chuyên dụng. (Bậc 2)", 1200, false, 5);

        // === GIÁP / ĐỒ CŨ ===
        CreateItem("Bao Tải Rách",    "Bao_Tai_Rach",   "Ao_Giap",        ItemData.ItemType.Armor,    0,40,200,0, "Một chiếc bao tải vá víu để che thân.", 200);
        CreateItem("Nồi Rỉ Sét",      "Noi_Ri_Set",     "Mu_Sat",         ItemData.ItemType.Armor,    0,15,50,0,  "Cái nồi thủng đáy dùng làm mũ.", 100);
        CreateItem("Dép Rơm",         "Dep_Rom",        "Giay_Chien_Than", ItemData.ItemType.Armor,    10,10,10,0, "Đôi dép rơm đi bộ mòn cả gót.", 50);
        CreateItem("Nắp Thùng Rác",   "Nap_Thung_Rac",  "Ao_Giap",        ItemData.ItemType.Armor,    0,30,0,0,   "Nắp thùng rác móp méo bảo vệ tay.", 150);
        CreateItem("Khiên Thép Cũ",   "Khien_Thep_Cu",  "Khien_Thep",     ItemData.ItemType.Armor,    0,80,100,0, "Tấm khiên thép cũ nhưng chắc chắn.", 800);
        CreateItem("Giáp Bao Tải Bọc Tôn", "Giap_Ton",  "Ao_Giap",        ItemData.ItemType.Armor,    0,150,1000,0, "Áo bao tải nẹp tôn gò. (Bậc 2)", 1200, false, 5);

        // === TRANG SỨC ===
        CreateItem("Dây Thép Buộc",   "Day_Thep_Buoc", "Nhan_Vang",  ItemData.ItemType.Accessory, 20,5,20,0,   "Một đoạn dây thép uốn thành nhẫn.", 80);
        CreateItem("Sợi Dây Thừng",   "Soi_Day_Thung", "Day_Chuyen", ItemData.ItemType.Accessory, 5,5,100,0,   "Sợi dây thừng nhặt ở bến tàu.", 100);
        CreateItem("Nhẫn Vòng Cỏ",    "Nhan_Vong_Co",  "Nhan_Vang",  ItemData.ItemType.Accessory, 50,50,50,0,  "Vật phẩm tổ truyền của kẻ ăn xin.", 1000);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Chạy tự động gán icon sau khi đúc xong
        AutoIconAssigner.AssignAllIcons();

        EditorUtility.DisplayDialog(
            "Thành công!",
            "Đã đúc xong toàn bộ vật phẩm 'Cái Bang' và gán Icon tự động!\nNhấn OK để tận hưởng thành quả.",
            "Tuyệt vời"
        );
    }

    void CreateGems(string baseName, string iconName, ItemData.ItemType type, int atk, int def, int hp, int basePrice)
    {
        for (int i = 1; i <= 5; i++)
        {
            string tier = (i==1?"I":i==2?"II":i==3?"III":i==4?"IV":"V");
            string tierAscii = (i==1?"I":i==2?"II":i==3?"III":i==4?"IV":"V");
            int mult = (int)Mathf.Pow(2.2f, i - 1);
            
            // Tên file khong dau de tranh loi
            string fileName = baseName.Replace(" ", "_") + "_" + tierAscii;
            // Tên hien thi co dau chuẩn
            string displayName = baseName + " " + tier;
            
            CreateItem(displayName, fileName, iconName, type,
                atk * mult, def * mult, hp * mult, 0,
                displayName + " bậc " + tier + ". Linh khí hội tụ.", basePrice * mult);
        }
    }

    void CreateItem(string displayName, string fileName, string iconName, ItemData.ItemType type,
        int atk, int def, int hp, int heal, string desc, int price,
        bool isTwoHanded = false, int reqLvl = 0)
    {
        string fullPath = "Assets/Resources/Items/" + fileName.Replace(" ", "_") + ".asset";
        ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(fullPath);
        bool isNew = (item == null);
        if (isNew) item = ScriptableObject.CreateInstance<ItemData>();

        item.itemName     = displayName; // Tên có dấu hiện trong Game
        item.type         = type;
        item.atkBonus     = atk;
        item.defBonus     = def;
        item.hpBonus      = hp;
        item.healAmount   = heal;
        item.description  = desc;
        item.price        = price;
        item.isTwoHanded  = isTwoHanded;
        item.requiredLevel= reqLvl;

        // Gán icon
        string iconPath = "Assets/Resources/Icons/" + iconName + ".png";
        Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
        if (sp != null) item.icon = sp;

        if (isNew)
            AssetDatabase.CreateAsset(item, fullPath);
        else
            EditorUtility.SetDirty(item);
    }
}