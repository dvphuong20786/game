using UnityEngine;

// Gắn cái này vô cục vàng. Nó sẽ tự biến đổi thành Vàng, Kiếm Gỗ hoặc Huyết Kiếm!
public class ItemDrop : MonoBehaviour
{
    private string itemName = "Khoai Lang";
    private bool isWeapon = false;

    void Start()
    {
        // Tung xúc xắc ngẫu nhiên từ 0 đến 100
        int tyLeRuy = Random.Range(0, 100);
        
        // Cố gắng tìm chức năng tô màu của Unity để đổi màu sắc vật phẩm cho ngầu
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (tyLeRuy <= 70) // 70% cơ hội rớt Kinh Nghiệm
        {
            itemName = "Kẹo Kinh Nghiệm";
            isWeapon = false;
            // Ép màu vàng cho quen thuộc
            if (sr != null) sr.color = Color.yellow; 
        }
        else if (tyLeRuy <= 90) // 20% rớt Kiếm Cùi
        {
            itemName = "Kiếm Gỗ Cùn";
            isWeapon = true;
            // Ép màu Nâu Đất cho kiếm gỗ
            if (sr != null) sr.color = new Color(0.6f, 0.3f, 0f); 
        }
        else // 10% cực hiếm rớt Đồ Siêu Khủng
        {
            itemName = "Huyết Kiếm";
            isWeapon = true;
            // Ép đồ hiếm màu Cam rực rỡ
            if (sr != null) sr.color = new Color(1f, 0.5f, 0f); 
        }
    }

    void Update()
    {
        PlayerStats player = FindAnyObjectByType<PlayerStats>();
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);

            // Bán kính nhặt đồ: Đứng đè lên là nhặt
            if (distance <= 1.5f)
            {
                if (isWeapon == false)
                {
                    player.AddExp(30); // Nhai Kẹo tăng thẳng 30 EXP
                }
                else
                {
                    player.PickUpWeapon(itemName); // Cất kiếm vào Túi Đồ
                }
                
                // Thu gom rác
                Destroy(gameObject);
            }
        }
    }
}
