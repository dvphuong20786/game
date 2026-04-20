using UnityEngine;
using System.Collections.Generic;

public class VillageBuilder : MonoBehaviour
{
    [Header("Cấu hình Làng")]
    public int villageSize = 20;      // Kích thước khu vực làng
    public int houseCount = 15;       // Số lượng nhà tối đa
    public float housesDensity = 0.6f; // Mật độ (độ gần nhau)

    [Header("Tài nguyên (Prefabs)")]
    public List<GameObject> housePrefabs; // Danh sách các mẫu nhà tranh/lều rách
    public List<GameObject> junkPrefabs;  // Thùng gỗ, xe đẩy, bao tải rác

    [ContextMenu("🏗️ GENERATE VILLAGE (XÂY LÀNG)")]
    public void Generate()
    {
        ClearVillage();
        
        // Tạo một mê cung nhà cửa
        for (int i = 0; i < houseCount; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-villageSize/2, villageSize/2),
                Random.Range(-villageSize/2, villageSize/2),
                0
            );

            // Kiểm tra xem có quá gần nhà cũ không để tạo cảm giác "san sát"
            GameObject prefab = housePrefabs[Random.Range(0, housePrefabs.Count)];
            GameObject h = Instantiate(prefab, pos, Quaternion.identity, transform);
            h.name = "Slum_House_" + i;
        }

        // Rải rác rác bẩn (Junk) vào các kẽ hở
        for (int j = 0; j < houseCount * 2; j++)
        {
            Vector3 junkPos = new Vector3(
                Random.Range(-villageSize/2, villageSize/2),
                Random.Range(-villageSize/2, villageSize/2),
                0
            );
            
            GameObject junkPrefab = junkPrefabs[Random.Range(0, junkPrefabs.Count)];
            Instantiate(junkPrefab, junkPos, Quaternion.identity, transform);
        }

        Debug.Log("⚒️ LÀNG Ổ CHUỘT ĐÃ ĐƯỢC KIẾN TẠO TỰ ĐỘNG!");
    }

    [ContextMenu("🧹 CLEAR VILLAGE (XÓA SẠCH)")]
    public void ClearVillage()
    {
        var children = new List<GameObject>();
        foreach (Transform child in transform) children.Add(child.gameObject);
        foreach (var child in children) DestroyImmediate(child);
    }
}
