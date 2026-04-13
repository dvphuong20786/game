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
    private bool daXuatBoss = false;

    void Update()
    {
        if (daXuatBoss == true) return; // Đã sinh Boss thì lò đình công

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
                Instantiate(monsterPrefab, toaDoDe, Quaternion.identity);
                soQuaiDaDe++;
                Debug.Log($"😈 Bóng tối đẻ quái thú thứ {soQuaiDaDe} / {tongSoQuaiMoiWave}");
            }
        }
        else 
        {
            // Tràn wave, tung Boss lấp ló
            if (bossPrefab != null)
            {
                GameObject boss = Instantiate(bossPrefab, toaDoDe, Quaternion.identity);
                // Biến Boss thành khổng lồ x3
                boss.transform.localScale = new Vector3(3, 3, 1);
                
                // Buff máu x10
                Monster bossStats = boss.GetComponent<Monster>();
                if (bossStats != null)
                {
                    bossStats.monsterName = "SIÊU BOSS ÁC MỘNG";
                    bossStats.maxHealth *= 10;
                    bossStats.attackDamage *= 5;
                    bossStats.expReward *= 20;
                    // Nạp thanh máu đầy lại
                    bossStats.SendMessage("Start", SendMessageOptions.DontRequireReceiver);
                }

                Debug.Log("💀 SIÊU BOSS ĐÃ XUẤT HIỆN! SKY IS FALLING!");
            }
            daXuatBoss = true;
        }
    }
}
