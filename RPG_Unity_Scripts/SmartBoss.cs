using UnityEngine;

// Gắn script này vào chung với Monster.cs trên con Boss
public class SmartBoss : MonoBehaviour
{
    private Monster m;
    private PlayerStats player;
    
    private float timer = 0f;
    private int state = 0; // 0: Đuổi, 1: Gồng Lướt
    private Vector3 docTieuToaDo; 

    void Start()
    {
        m = GetComponent<Monster>();
        player = FindAnyObjectByType<PlayerStats>();
        if (m != null)
        {
            // Boss quá khôn, nó thu hồi quyền di chuyển của Monster cơ bản để tự nó đi theo thuật toán
            m.moveSpeed = 0f; 
        }
    }

    void Update()
    {
        if (m == null || player == null) return;
        if (m.currentHealth <= 0) return; // Chết rồi khỏi múa

        timer += Time.deltaTime;

        if (state == 0) // Đi bộ xáp lá cà
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 1.5f * Time.deltaTime);

            // Sau 4 giây, Boss bắt đầu Gồng để lướt
            if (timer > 4f) 
            {
                state = 1;
                timer = 0;
                docTieuToaDo = player.transform.position;
                GetComponent<SpriteRenderer>().color = Color.red; // Nháy đỏ lên gồng lực
            }
        }
        else if (state == 1) // Phase lao lướt đâm sầm
        {
            transform.position = Vector3.MoveTowards(transform.position, docTieuToaDo, 15f * Time.deltaTime);

            if (Vector3.Distance(transform.position, docTieuToaDo) < 0.1f || timer > 0.5f)
            {
                state = 0; 
                timer = 0;
                GetComponent<SpriteRenderer>().color = Color.white;
            }
        }

        // Hóa điên (Enrage Mode)
        if (m.currentHealth < (m.maxHealth * 0.3f))
        {
            // Máu tụt mốc 30%, Boss hóa tím.
            GetComponent<SpriteRenderer>().color = Color.magenta;
            transform.localScale = new Vector3(4, 4, 1);
            if (state == 0) 
            {
                // Điên lên chạy siêu nhanh chứ không đủng đỉnh nữa
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 4f * Time.deltaTime);
            }
        }
    }
}
