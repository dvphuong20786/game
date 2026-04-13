using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Gắn file này vào một hình ảnh rơi trên đất rớt ra từ túi quà
public class ItemDrop : MonoBehaviour
{
    [Header("Thông tin vật phẩm")]
    public string itemName = "Bé Tinh Linh (EXP Item)";

    // Update được gọi liên tục mỗi khung hình
    void Update()
    {
        // Gợi ý cho người mới: Để nhặt vật phẩm đơn giản nhất,
        // Ta lại kiểm tra nếu Cầu thủ tiến lại gần món đồ nhỏ hơn cự ly 1 mét.
        
        PlayerStats player = FindObjectOfType<PlayerStats>();
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= 1.5f)
            {
                // Nhặt đồ!
                player.PickUpItem(itemName);
                
                // Mất cục đồ trên mặt đất
                Destroy(gameObject);
            }
        }
    }
}
