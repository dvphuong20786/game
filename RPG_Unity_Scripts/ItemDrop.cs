using UnityEngine;

// Gắn cái này vô cục vàng.
public class ItemDrop : MonoBehaviour
{
    private string itemName = "Kẹo Kinh Nghiệm";
    private bool isItem = false; // Phân biệt Ăn Liền vs Đồ Bỏ Túi

    void Start()
    {
        int luc = Random.Range(0, 100);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (luc <= 50) 
        {
            itemName = "Kẹo Kinh Nghiệm";
            isItem = false;
            if (sr != null) sr.color = Color.yellow; 
        }
        else if (luc <= 60)
        {
            itemName = "Kiếm Gỗ Cùn"; isItem = true;
            if (sr != null) sr.color = new Color(0.6f, 0.3f, 0f); 
        }
        else if (luc <= 70)
        {
            itemName = "Mũ Sắt"; isItem = true;
            if (sr != null) sr.color = Color.gray; 
        }
        else if (luc <= 80)
        {
            itemName = "Áo Da Lộn"; isItem = true;
            if (sr != null) sr.color = new Color(0.8f, 0.5f, 0.2f); 
        }
        else if (luc <= 85)
        {
            itemName = "Giày Siêu Tốc"; isItem = true;
            if (sr != null) sr.color = Color.blue; 
        }
        else if (luc <= 90)
        {
            itemName = "Nhẫn Kim Cương"; isItem = true;
            if (sr != null) sr.color = Color.cyan; 
        }
        else if (luc <= 95)
        {
            itemName = "Dây Chuyền Bạc"; isItem = true;
            if (sr != null) sr.color = Color.white; 
        }
        else 
        {
            itemName = "Huyết Kiếm"; isItem = true;
            if (sr != null) sr.color = new Color(1f, 0.5f, 0f); 
        }
    }

    void Update()
    {
        PlayerStats player = FindAnyObjectByType<PlayerStats>();
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);

            if (distance <= 1.0f)
            {
                if (isItem == false)
                {
                    player.AddExp(30);
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
