using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Header("Bản Đồ Muốn Dịch Chuyển Tới")]
    public string tenMapTiepTheo = "RungSau"; 

    private PlayerStats player;

    void Update()
    {
        // Tự tìm người chơi nếu chưa có
        if (player == null)
        {
            PlayerStats[] all = Object.FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
            foreach (var s in all) if (s.isPlayer) { player = s; break; }
        }

        if (player != null)
        {
            // Kiểm tra khoảng cách thủ công (Phòng trường hợp Collider/Physics bị lỗi)
            float dist = Vector2.Distance(transform.position, player.transform.position);
            if (dist < 1.5f)
            {
                Debug.Log("🌀 Cảm biến khoảng cách kích hoạt! Đang dịch chuyển...");
                ExecuteTeleport();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerStats>() != null)
        {
            Debug.Log("🔔 Va chạm vật lý kích hoạt!");
            ExecuteTeleport();
        }
    }

    void ExecuteTeleport()
    {
        Debug.Log("🚀 CHUYỂN CẢNH TỚI: " + tenMapTiepTheo);
        SceneManager.LoadScene(tenMapTiepTheo);
    }
}
