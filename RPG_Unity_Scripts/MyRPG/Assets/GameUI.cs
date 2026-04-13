using UnityEngine;
using UnityEngine.UI;

// Gắn script này vào bức tranh vô hình CANVAS của hệ thống UI
public class GameUI : MonoBehaviour
{
    [Header("Thanh Máu (Giao diện)")]
    public Slider healthBar; // Ô trống này để bạn kéo thanh Slider ngoài giao diện vào

    // Móc nối với trái tim của Người chơi
    private PlayerStats player;

    void Start()
    {
        // Đoạn code thông minh: Nó sẽ tự động lùng sục khắp màn hình để tìm xem 
        // ai là người giữ file code PlayerStats (Đó chính là bạn - Player). 
        // Làm vậy tiết kiệm cho bạn 1 bước kéo thả cực nhọc.
        player = FindAnyObjectByType<PlayerStats>();

        // Cài đặt mức tối đa cho thanh máu bằng Máu Khởi Điểm
        if (healthBar != null && player != null)
        {
            healthBar.maxValue = player.maxHealth;
            healthBar.value = player.currentHealth;
        }
    }

    void Update()
    {
        if (healthBar != null && player != null)
        {
            // Liên tục bơm/rút thanh Slider đỏ theo số máu hiện tại của người chơi
            healthBar.value = player.currentHealth;
            
            // Đề phòng trường hợp bạn lên cấp và máu trâu hơn
            healthBar.maxValue = player.maxHealth; 
        }
    }
}
