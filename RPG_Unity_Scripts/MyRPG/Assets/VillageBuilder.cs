using UnityEngine;

// ===========================
// CÔNG CỤ XÂY LỰNG LÀNG TỰ ĐỘNG
// Gán vào một GameObject trống trong Scene Village
// Nhấn Play là có ngay cái nền làng và hàng rào!
// ===========================
public class VillageBuilder : MonoBehaviour
{
    [Header("Cấu hình Làng")]
    public int width = 20;
    public int height = 20;
    public float tileSize = 1f;

    [Header("Prefabs Thành phần")]
    public GameObject floorPrefab;   // Kéo hình cái Cỏ vào đây
    public GameObject fencePrefab;   // Kéo hàng rào vào đây
    public GameObject treePrefab;    // Kéo cây vào đây

    [Header("Tùy chỉnh Kích thước (Scale)")]
    public Vector3 floorScale = Vector3.one;
    public Vector3 fenceScale = new Vector3(0.5f, 0.5f, 1f); // Mặc định hàng rào nhỏ lại 1 nửa
    public Vector3 treeScale = Vector3.one;

    [Header("Trang trí")]
    [Range(0, 10)] public int treeCount = 6;

    void Start()
    {
        BuildVillage();
    }

    public void BuildVillage()
    {
        // Xóa map cũ nếu có
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }

        Debug.Log("🏗️ Đang xây dựng nền móng cho Làng...");

        // 1. Sinh Nền và Hàng Rào bao quanh
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x * tileSize, y * tileSize, 0);

                // Luôn sinh nền gạch/cỏ
                if (floorPrefab != null) {
                    GameObject floor = Instantiate(floorPrefab, pos, Quaternion.identity);
                    floor.transform.localScale = floorScale;
                    floor.transform.SetParent(this.transform);
                }

                // Nếu là sát biên thì đặt hàng rào (Fence)
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    if (fencePrefab != null) {
                        GameObject fence = Instantiate(fencePrefab, pos + new Vector3(0,0,-0.1f), Quaternion.identity);
                        fence.transform.localScale = fenceScale;
                        fence.transform.SetParent(this.transform);
                    }
                }
            }
        }

        // 2. Đặt cây ở các vị trí ngẫu nhiên trong làng (tránh trung tâm)
        int placedTrees = 0;
        int attempts = 0;
        while (placedTrees < treeCount && attempts < 50)
        {
            attempts++;
            if (treePrefab == null) break;
            
            int rx = Random.Range(2, width - 2);
            int ry = Random.Range(2, height - 2);
            
            // Chỉ đặt cây ở khu vực rìa trong (không đè lên trung tâm 0-0)
            if (Mathf.Abs(rx - width/2) > 2 || Mathf.Abs(ry - height/2) > 2)
            {
                GameObject tree = Instantiate(treePrefab, new Vector3(rx * tileSize, ry * tileSize, -0.2f), Quaternion.identity);
                tree.transform.localScale = treeScale;
                tree.transform.SetParent(this.transform);
                placedTrees++;
            }
        }

        Debug.Log("✅ Xây dựng Làng hoàn tất! Bạn có thể chỉnh Fence Scale trong Inspector để hàng rào nhỏ lại.");
    }
}
