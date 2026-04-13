using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Header("Bản Đồ Muốn Dịch Chuyển Tới")]
    public string tenMapTiepTheo = "RungSau"; 

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Chạm vào HiepSi thì mới nhảy map
        if (collision.GetComponent<PlayerStats>() != null)
        {
            Debug.Log("🌀 Kích Hoạt Cửa Không Gian! Cùng đi tới: " + tenMapTiepTheo);
            SceneManager.LoadScene(tenMapTiepTheo);
        }
    }
}
