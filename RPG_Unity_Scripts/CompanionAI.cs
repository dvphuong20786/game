using UnityEngine;

// Gắn cái này vào một Cục Gạch Tròn làm Đệ Tử
public class CompanionAI : MonoBehaviour
{
    public float movementSpeed = 3f;
    public float attackRange = 1.5f;
    public int attackDamage = 30; // Đệ tử chém khá đau
    private float attackTimer;

    private PlayerStats master;

    void Start()
    {
        master = FindAnyObjectByType<PlayerStats>();
        // Đệ tử màu Xanh Dương để phân biệt cẩn thận khỏi đánh lầm
        GetComponent<SpriteRenderer>().color = Color.cyan;
    }

    void Update()
    {
        if (master == null) return;

        // Tự tìm Quái gần nhất
        Monster[] allMonsters = FindObjectsOfType<Monster>();
        Monster qTarget = null;
        float minDis = 999f;

        foreach(Monster mm in allMonsters)
        {
            // Quái chết không đánh
            if(mm.currentHealth <= 0) continue;

            float d = Vector2.Distance(transform.position, mm.transform.position);
            if (d < minDis) { minDis = d; qTarget = mm; }
        }

        // Trí tuệ hung hãn: Hễ thấy quái lảng vảng trong vòng 6 mét là nhào dô cấu xé
        if (qTarget != null && minDis < 6f) 
        {
            if (minDis > attackRange)
            {
                // Đang tiếp cận
                transform.position = Vector3.MoveTowards(transform.position, qTarget.transform.position, movementSpeed * Time.deltaTime);
            }
            else
            {
                // Đứng sát rồi, vung kiếm nhai đầu quái
                attackTimer += Time.deltaTime;
                if (attackTimer >= 1f)
                {
                    qTarget.TakeDamage(attackDamage);
                    attackTimer = 0f;
                    Debug.Log("🐕🐕 Đệ tử Tử Long vừa chém quái " + attackDamage + " điểm!");
                }
            }
        }
        else
        {
            // Rảnh rỗi: Đi lẽo đẽo sau đít chủ nhân
            if (Vector2.Distance(transform.position, master.transform.position) > 2f)
            {
                transform.position = Vector3.MoveTowards(transform.position, master.transform.position, (movementSpeed + 1) * Time.deltaTime);
            }
        }
    }
}
