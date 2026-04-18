using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [Header("Cấu hình Bản đồ")]
    public int width = 50;
    public int height = 50;
    public float tileSize = 1f;

    [Header("Prefabs Nền (Cỏ, Đất...)")]
    public GameObject[] floorPrefabs;

    [Header("Prefabs Vật cản (Cây, Đá...)")]
    public GameObject[] obstaclePrefabs;
    [Range(0, 1)] public float obstacleDensity = 0.15f;

    [Header("Prefabs Đặc biệt")]
    public GameObject monsterPitPrefab;
    public int monsterPitCount = 8;
    public GameObject exitPortalPrefab;

    private List<Vector2Int> occupiedPositions = new List<Vector2Int>();

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        // 1. Xóa map cũ nếu có (Trường hợp muốn sinh lại)
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
        occupiedPositions.Clear();

        Debug.Log($"🌍 Bắt đầu sinh bản đồ {width}x{height}...");

        // 2. Sinh Nền và Tường Bao
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Luôn sinh nền trước
                GameObject floor = Instantiate(floorPrefabs[Random.Range(0, floorPrefabs.Length)], new Vector3(x * tileSize, y * tileSize, 0), Quaternion.identity);
                floor.transform.SetParent(this.transform);

                // Nếu là biên thì đặt vật cản dày đặc làm tường
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    SpawnObstacle(x, y, true);
                }
                else
                {
                    // Ngẫu nhiên đặt vật cản bên trong (tránh vị trí xuất phát 2,2)
                    if (Random.value < obstacleDensity && (x > 3 || y > 3))
                    {
                        SpawnObstacle(x, y, false);
                    }
                }
            }
        }

        // 3. Đặt Lỗ Quái Vật (Monster Pits)
        int pitsPlaced = 0;
        int attempts = 0;
        while (pitsPlaced < monsterPitCount && attempts < 100)
        {
            attempts++;
            int rx = Random.Range(5, width - 5);
            int ry = Random.Range(5, height - 5);
            Vector2Int pos = new Vector2Int(rx, ry);

            if (!occupiedPositions.Contains(pos))
            {
                Instantiate(monsterPitPrefab, new Vector3(rx * tileSize, ry * tileSize, 0), Quaternion.identity);
                occupiedPositions.Add(pos);
                pitsPlaced++;
            }
        }

        // 4. Đặt Cổng Thoát (Exit Portal)
        Vector3 portalPos = new Vector3((width - 3) * tileSize, (height - 3) * tileSize, 0);
        GameObject portal = Instantiate(exitPortalPrefab, portalPos, Quaternion.identity);
        Portal portalScript = portal.GetComponent<Portal>();
        if (portalScript != null) portalScript.tenMapTiepTheo = "Village"; // Giả định tên scene làng là Village

        Debug.Log("✅ Sinh bản đồ hoàn tất!");

        // TỰ ĐỘNG KẾT NỐI CAMERA
        if (CameraFollow.instance != null)
        {
            CameraFollow.instance.SetTarget(null); // Force Camera tìm lại Player chính xác
        }
    }

    void SpawnObstacle(int x, int y, bool isWall)
    {
        GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        GameObject obs = Instantiate(prefab, new Vector3(x * tileSize, y * tileSize, 0), Quaternion.identity);
        obs.transform.SetParent(this.transform);
        occupiedPositions.Add(new Vector2Int(x, y));
    }
}
