using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Sóng Địch (Waves)")]
    public GameObject monsterPrefab; 
    public GameObject bossPrefab; // Kéo xác quái vật vào đây để làm Mẫu Boss

    [Header("Thời gian xuất hiện")]
    public float thoiGianChuyenDa = 4f; 
    private float timerCoiGioi = 0f;

    [Header("Quản lý lượng quái")]
    public int tongSoQuaiMoiWave = 10;
    private int soQuaiDaDe = 0;

    [Header("Độ khó tăng dần")]
    public float multiplier = 1.1f; // Mỗi đợt quái sẽ mạnh hơn 10%
    private int waveNumber = 1;

    void Update()
    {
        timerCoiGioi += Time.deltaTime;

        if (timerCoiGioi >= thoiGianChuyenDa)
        {
            SinhSanNgayTao();
            timerCoiGioi = 0f; 
        }
    }

    void SinhSanNgayTao()
    {
        float ranX = Random.Range(-8f, 8f);
        float ranY = Random.Range(-5f, 5f);
        Vector3 toaDoDe = new Vector3(transform.position.x + ranX, transform.position.y + ranY, 0f);

        if (soQuaiDaDe < tongSoQuaiMoiWave)
        {
            if (monsterPrefab != null)
            {
                GameObject q = Instantiate(monsterPrefab, toaDoDe, Quaternion.identity);
                soQuaiDaDe++;
                
                // Buff quái theo số đợt (Wave)
                Monster qStats = q.GetComponent<Monster>();
                if (qStats != null)
                {
                    qStats.maxHealth = Mathf.RoundToInt(qStats.maxHealth * Mathf.Pow(multiplier, waveNumber - 1));
                    qStats.attackDamage = Mathf.RoundToInt(qStats.attackDamage * Mathf.Pow(multiplier, waveNumber - 1));
                }

                Debug.Log($"😈 Đợt {waveNumber}: Đẻ quái thú thứ {soQuaiDaDe} / {tongSoQuaiMoiWave}");
            }
        }
        else 
        {
            // Xuất BOSS và bắt đầu Wave mới
            if (bossPrefab != null)
            {
                GameObject boss = Instantiate(bossPrefab, toaDoDe, Quaternion.identity);
                boss.transform.localScale = new Vector3(3, 3, 1);
                
                Monster bossStats = boss.GetComponent<Monster>();
                if (bossStats != null)
                {
                    bossStats.monsterName = "BOSS W" + waveNumber;
                    bossStats.maxHealth *= (10 + waveNumber);
                    bossStats.attackDamage *= (5 + waveNumber);
                    bossStats.expReward *= 20;
                    bossStats.SendMessage("Start", SendMessageOptions.DontRequireReceiver);
                }

                Debug.Log($"💀 BOSS ĐỢT {waveNumber} ĐÃ XUẤT HIỆN!");
            }

            // RESET GAME ĐỂ TIẾP TỤC ĐỢT MỚI
            soQuaiDaDe = 0;
            waveNumber++;
            thoiGianChuyenDa *= 0.95f; // Đẻ quái ngày càng nhanh hơn
            if (thoiGianChuyenDa < 1f) thoiGianChuyenDa = 1f;
        }
    }
}
