using UnityEngine;

// Gắn script này vào một khoảng không (GameObject trống) nằm giữa bản đồ
public class MonsterSpawner : MonoBehaviour
{
    [Header("Cài đặt Lò Đẻ Tuyệt Đối")]
    public GameObject monsterPrefab; // Nhét cái bản mẫu con quái vật vào đây để máy Photocopy
    public float thoiGianChuyenDa = 4f; // Cứ 4 giây là đẻ một lần

    private float timerCoiGioi = 0f;

    void Update()
    {
        timerCoiGioi += Time.deltaTime;

        if (timerCoiGioi >= thoiGianChuyenDa)
        {
            SinhSanNgayTao();
            timerCoiGioi = 0f; // Bấm đồng hồ về số 0
        }
    }

    void SinhSanNgayTao()
    {
        // 1. Lọc ra một điểm rớt ngẫu nhiên xung quanh khu vực Lò Đẻ (Bán kính 8 mét)
        float ranX = Random.Range(-8f, 8f);
        float ranY = Random.Range(-5f, 5f);
        Vector3 toaDoDe = new Vector3(transform.position.x + ranX, transform.position.y + ranY, 0f);

        // 2. Chế tạo Quái
        if (monsterPrefab != null)
        {
            Instantiate(monsterPrefab, toaDoDe, Quaternion.identity);
            Debug.Log("⚠️ ⚠️ CẢNH BÁO: BÓNG TỐI VỪA ĐẺ RA MỘT CON QUÁI VẬT MỚI!");
        }
    }
}
