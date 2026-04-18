using UnityEngine;
using System.Collections.Generic;

public class MonsterPit : MonoBehaviour
{
    [Header("Cài đặt sinh quái")]
    public GameObject monsterPrefab;
    public int maxMonsters = 5;
    public float thoiGianHoiSinh = 8f;
    public float spawnRadius = 2f;

    [Header("Phạm vi AI")]
    public float aggroRange = 10f;
    public float resetRange = 20f; // Phạm vi gấp đôi aggroRange theo yêu cầu

    private List<GameObject> spawnedMonsters = new List<GameObject>();
    private float respawnTimer = 0f;

    void Start()
    {
        // Khởi tạo ban đầu
        for (int i = 0; i < maxMonsters; i++)
        {
            SpawnMonster();
        }
    }

    void Update()
    {
        // Loại bỏ các quái đã chết (null) khỏi danh sách
        spawnedMonsters.RemoveAll(m => m == null);

        // Nếu thiếu quái, bắt đầu đếm ngược hồi sinh
        if (spawnedMonsters.Count < maxMonsters)
        {
            respawnTimer += Time.deltaTime;
            if (respawnTimer >= thoiGianHoiSinh)
            {
                SpawnMonster();
                respawnTimer = 0f;
            }
        }

        // Kiểm tra mục tiêu trong vùng
        PlayerStats target = FindNearestTarget();
        
        foreach (var m in spawnedMonsters)
        {
            if (m == null) continue;
            Monster script = m.GetComponent<Monster>();
            if (script != null)
            {
                script.SetPitLogic(transform.position, aggroRange, resetRange, target);
            }
        }
    }

    void SpawnMonster()
    {
        if (monsterPrefab == null) return;

        Vector3 spawnPos = transform.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius), 0);
        GameObject g = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
        spawnedMonsters.Add(g);
        
        Monster m = g.GetComponent<Monster>();
        if (m != null)
        {
            m.monsterName = "Quái Lỗ #" + spawnedMonsters.Count;
        }
    }

    PlayerStats FindNearestTarget()
    {
        PlayerStats[] targets = Object.FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
        PlayerStats nearest = null;
        float minD = aggroRange;

        foreach (var t in targets)
        {
            float d = Vector2.Distance(transform.position, t.transform.position);
            if (d < minD)
            {
                minD = d;
                nearest = t;
            }
        }
        return nearest;
    }

    void OnDrawGizmosSelected()
    {
        // Vẽ vùng nhận diện trong editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, resetRange);
    }
}
