using UnityEngine;

// Gắn cái này vô cục vàng hoặc vật phẩm rơi ra từ quái
public class ItemDrop : MonoBehaviour
{
    private string itemName = "Vàng";
    private bool isItem = false; // false = Vàng ăn liền, true = Đồ bỏ túi

    // Màu sắc hiển thị
    private Color glowColor = Color.yellow;
    private SpriteRenderer sr;
    private float pulseTimer = 0f;
    private Vector3 initialScale;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        initialScale = transform.localScale; // Lưu lại kích thước ban đầu bạn đặt trong Unity

        // ===== TỈ LỆ RỚT ĐỒ =====
        // ... (giữ nguyên)
        int luc = Random.Range(1, 101);

        if (luc <= 10) // 10% — Rớt trang bị
        {
            string[] items = { "Huyết Kiếm", "Áo Da Lộn", "Mũ Sắt", "Giày Siêu Tốc", "Nhẫn Kim Cương", "Dây Chuyền Bạc", "Khiên Gỗ" };
            itemName = items[Random.Range(0, items.Length)];
            isItem = true;
            glowColor = new Color(0f, 1f, 1f); // Cyan = đồ hiếm
        }
        else if (luc <= 40) // 30% — Rớt vàng (11 → 40)
        {
            itemName = "Vàng";
            isItem = false;
            glowColor = new Color(1f, 0.9f, 0f); // Vàng
        }
        else // 60% — Không có gì
        {
            Destroy(gameObject);
            return;
        }

        // Gán màu ban đầu
        if (sr != null) sr.color = glowColor;
    }

    void Update()
    {
        if (sr == null) return;

        // ===== HIỆU ỨNG PHÁT SÁNG RÕ HƠN =====
        pulseTimer += Time.deltaTime * 3f;

        // Nhấp nháy giữa màu sáng và tối (biên độ to hơn trước)
        float brightness = 0.6f + Mathf.Sin(pulseTimer) * 0.4f; // dao động 0.2 → 1.0
        sr.color = new Color(glowColor.r * brightness, glowColor.g * brightness, glowColor.b * brightness, 1f);

        // Scale to nhỏ nhấp nháy dựa trên kích thước thực tế bạn đặt trong Unity
        float pulse = 1f + Mathf.Sin(pulseTimer * 1.5f) * 0.15f; 
        transform.localScale = initialScale * pulse;

        // ===== TỰ ĐỘNG HÚT VÀO NGƯỜI CHƠI =====
        PlayerStats player = FindAnyObjectByType<PlayerStats>();
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance <= 1.2f)
            {
                if (!isItem)
                {
                    player.AddGold(10);
                    if (GameUI.instance != null)
                        GameUI.instance.ShowDamage(transform.position, "+10G", new Color(1f, 0.9f, 0f));
                }
                else
                {
                    player.PickUpItem(itemName);
                    if (GameUI.instance != null)
                        GameUI.instance.ShowDamage(transform.position, "+" + itemName, Color.cyan);
                }
                Destroy(gameObject);
            }
        }
    }
}
