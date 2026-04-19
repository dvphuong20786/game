using UnityEngine;

// Gắn script này vào một GameObject con của Player/Companion (ví dụ: đặt tên là "WeaponHand")
public class WeaponVisual : MonoBehaviour
{
    public PlayerStats stats; // Gán PlayerStats của nhân vật vào đây
    private SpriteRenderer sr;
    private ItemData currentWeapon;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (stats == null) stats = GetComponentInParent<PlayerStats>();
        
        // Cấu hình ban đầu cho SpriteRenderer vũ khí
        if (sr == null) sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 10; // Đảm bảo hiện đè lên thân nhân vật
    }

    void Update()
    {
        if (stats == null) return;

        // Kiểm tra xem vũ khí chính có thay đổi không
        ItemData newWeapon = (stats.eqWeaponMain != null) ? stats.eqWeaponMain.data : null;

        if (newWeapon != currentWeapon)
        {
            currentWeapon = newWeapon;
            UpdateVisual();
        }

        // Tự động xoay vũ khí theo hướng nhân vật (Lật theo FlipX của thân)
        SpriteRenderer parentSR = transform.parent.GetComponent<SpriteRenderer>();
        if (parentSR != null)
        {
            sr.flipX = parentSR.flipX;
            // Chỉnh vị trí tay tùy theo hướng lật (nếu cần)
            float xOffset = parentSR.flipX ? -0.3f : 0.3f;
            transform.localPosition = new Vector3(xOffset, -0.1f, 0);
        }
    }

    void UpdateVisual()
    {
        if (currentWeapon != null)
        {
            sr.sprite = currentWeapon.icon;
            sr.enabled = true;
            // Thu nhỏ vũ khí một chút để vừa tay nhân vật
            transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            Debug.Log($"⚔️ Đã hiển thị {currentWeapon.itemName} trên tay {stats.characterName}");
        }
        else
        {
            sr.enabled = false;
        }
    }
}
