using UnityEngine;

// Gắn cái này vô cục vàng hoặc vật phẩm rơi ra từ quái
public class ItemDrop : MonoBehaviour
{
    private string itemName = "Vàng";
    private bool isItem = false; // Phân biệt Ăn Liền (Vàng) vs Đồ Bỏ Túi (Trang bị)

    void Start()
    {
        // Tỉ lệ: 10% Đồ, 30% Vàng, 60% Xịt (Xịt thì tự xóa)
        int luc = Random.Range(1, 101); // 1 đến 100
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (luc <= 10) // 10% Rớt trang bị hiếm
        {
            string[] items = { "Huyết Kiếm", "Áo Da Lộn", "Mũ Sắt", "Giày Siêu Tốc", "Nhẫn Kim Cương", "Dây Chuyền Bạc" };
            itemName = items[Random.Range(0, items.Length)];
            isItem = true;
            if (sr != null) sr.color = Color.cyan;
        }
        else if (luc <= 40) // 30% Rớt vàng (11 - 40)
        {
            itemName = "Vàng";
            isItem = false;
            if (sr != null) sr.color = Color.yellow; 
        }
        else // 60% Không có gì (Xịt)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        PlayerStats player = FindAnyObjectByType<PlayerStats>();
        if (player != null)
        {
            // Tự động hút vào người chơi nếu ở gần
            float distance = Vector2.Distance(transform.position, player.transform.position);

            if (distance <= 1.0f)
            {
                if (isItem == false)
                {
                    player.AddGold(10); 
                }
                else
                {
                    player.PickUpItem(itemName);
                }
                Destroy(gameObject);
            }
        }
    }
}
