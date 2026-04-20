using UnityEngine;

// Gan script nay vao mot GameObject con cua Player/Companion (vi du: WeaponHand)
// Chinh Scale cua GameObject WeaponHand trong Inspector cho phu hop voi tile size game
public class WeaponVisual : MonoBehaviour
{
    public PlayerStats stats;
    [Tooltip("Scale vu khi (0.2 = nho, 0.4 = lon)")]
    public float weaponScale = 0.22f;
    [Tooltip("Offset ngang (duong = phai, am = trai)")]
    public float xOffsetRight = 0.28f;
    [Tooltip("Offset doc (am = xuong thap)")]
    public float yOffset = -0.08f;

    private SpriteRenderer sr;
    private ItemData currentWeapon;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = gameObject.AddComponent<SpriteRenderer>();
        if (stats == null) stats = GetComponentInParent<PlayerStats>();

        sr.sortingOrder = 5; // Hien de len than nhan vat
    }

    void Update()
    {
        if (stats == null) return;

        ItemData newWeapon = (stats.eqWeaponMain != null) ? stats.eqWeaponMain.data : null;
        if (newWeapon != currentWeapon)
        {
            currentWeapon = newWeapon;
            UpdateVisual();
        }

        // Lat theo huong nhan vat
        SpriteRenderer parentSR = transform.parent != null ? transform.parent.GetComponent<SpriteRenderer>() : null;
        if (parentSR != null)
        {
            sr.flipX = parentSR.flipX;
            float xOff = parentSR.flipX ? -xOffsetRight : xOffsetRight;
            transform.localPosition = new Vector3(xOff, yOffset, 0f);
        }
    }

    void UpdateVisual()
    {
        if (currentWeapon != null && currentWeapon.icon != null)
        {
            sr.sprite = currentWeapon.icon;
            sr.enabled = true;
            // Dat scale tuy loai vu khi
            float s = weaponScale;
            if (currentWeapon.type == ItemData.ItemType.Weapon)
            {
                bool isRanged = currentWeapon.itemName.Contains("Cung") ||
                                currentWeapon.itemName.Contains("No") ||
                                currentWeapon.itemName.Contains("Nu");
                s = isRanged ? weaponScale * 1.2f : weaponScale;
            }
            transform.localScale = new Vector3(s, s, 1f);
        }
        else
        {
            sr.enabled = false;
        }
    }
}
