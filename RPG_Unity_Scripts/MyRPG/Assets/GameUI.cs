using UnityEngine;

// Kịch bản vẽ Giao Diện Cổ Điển siêu tốc (Không cần Canvas lằng nhằng)
public class GameUI : MonoBehaviour
{
    private PlayerStats player;

    void Start()
    {
        // Tự động tìm kịch bản báo cáo Máu của nhân vật
        player = FindAnyObjectByType<PlayerStats>();
    }

    // Đây là hàm thần thánh của Unity: Vẽ hình ảnh bay lơ lửng đè lên màn hình
    void OnGUI()
    {
        if (player != null)
        {
            // 1. Vẽ một cái đáy khung ngang màu ĐEN (Tượng trưng cho viền cây máu)
            GUI.color = Color.black;
            // Vẽ ở góc (x=20, y=20), dài 204, rộng 30
            GUI.DrawTexture(new Rect(20, 20, 204, 30), Texture2D.whiteTexture);

            // 2. Vẽ Lõi Máu màu ĐỎ đè lên trên (Sẽ bị rụt ngắn lại nếu máu giảm)
            GUI.color = Color.red;
            // Tính toán ra chiều dài cây máu (Máu càng thấp thanh đỏ này càng ngắn)
            float doDaiThanhMau = ((float)player.currentHealth / player.maxHealth) * 200f;
            GUI.DrawTexture(new Rect(22, 22, doDaiThanhMau, 26), Texture2D.whiteTexture);

            // 3. Khắc cái chữ (HP: 100/100) lên thanh máu cho bá đạo
            GUI.color = Color.white;
            GUI.Label(new Rect(80, 25, 200, 30), "HP: " + player.currentHealth + " / " + player.maxHealth);

            // ==========================================
            // VẼ THÊM THANH MÁU NỔI TRÊN ĐẦU NHÂN VẬT!
            // ==========================================
            Camera cam = Camera.main;
            if (cam != null)
            {
                Vector3 screenPos = cam.WorldToScreenPoint(player.transform.position);

                if (screenPos.z > 0 && player.currentHealth > 0)
                {
                    float screenY = Screen.height - screenPos.y;

                    float barWidth = 60f;
                    float barHeight = 8f;
                    float yOffset = 50f; // Cao hơn đầu 1 đoạn
                    float startX = screenPos.x - (barWidth / 2);
                    float startY = screenY - yOffset;

                    // Viền đen
                    GUI.color = Color.black;
                    GUI.DrawTexture(new Rect(startX, startY, barWidth, barHeight), Texture2D.whiteTexture);

                    // Lõi xanh lá cây (Phân biệt với quái vật màu Đỏ)
                    GUI.color = Color.green;
                    float healthRatio = (float)player.currentHealth / player.maxHealth;
                    GUI.DrawTexture(new Rect(startX, startY, barWidth * healthRatio, barHeight), Texture2D.whiteTexture);
                    
                    // Xóa màu để xài cho cái khác
                    GUI.color = Color.white;
                }
            }
        }
    }
}
