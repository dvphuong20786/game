using UnityEngine;

// Gắn cái này vô cục vàng hoặc vật phẩm rơi ra từ quái
public class ItemDrop : MonoBehaviour
{
    public ItemData itemData;
    private bool isItem = false; 
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
            // Tự động load một món đồ ngẫu nhiên từ Resources nếu chưa gán thủ công
            if (itemData == null) {
                ItemData[] allItems = Resources.LoadAll<ItemData>("Items");
                if (allItems.Length > 0) itemData = allItems[Random.Range(0, allItems.Length)];
            }
            isItem = (itemData != null);
            glowColor = new Color(0f, 1f, 1f); 
        }
        else if (luc <= 40) // 30% — Rớt vàng
        {
            isItem = false;
            glowColor = new Color(1f, 0.9f, 0f); 
        }
        else 
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

        // ===== TỰ ĐỘNG HÚT VÀO NGƯỜI CHƠI HOẶC ĐỆ TỬ =====
        PlayerStats[] allStats = Object.FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
        foreach (PlayerStats stats in allStats)
        {
            float distance = Vector2.Distance(transform.position, stats.transform.position);
            if (distance <= 1.2f)
            {
                if (!isItem)
                {
                    stats.AddGold(10);
                    if (GameUI.instance != null)
                        GameUI.instance.ShowDamage(transform.position, "+10G", new Color(1f, 0.9f, 0f));
                }
                else
                {
                    stats.PickUpItem(itemData);
                    if (GameUI.instance != null)
                        GameUI.instance.ShowDamage(transform.position, "+" + itemData.itemName, Color.cyan);
                }
                Destroy(gameObject);
                break;
            }
        }
    }
}
