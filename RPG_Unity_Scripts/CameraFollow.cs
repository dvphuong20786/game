using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;
    public float smoothSpeed = 5f; // Tăng lên để camera mượt hơn (cũ là 0.125f quá chậm)
    public Vector3 offset = new Vector3(0, 0, -10);

    private float findTimer = 0f;
    private const float FIND_INTERVAL = 2f; // Tìm lại player mỗi 2 giây nếu bị mất

    void Start()
    {
        FindPlayer();
    }

    void FindPlayer()
    {
        // Cách 1: Tìm theo Tag "Player" (nhanh nhất, ít tốn tài nguyên nhất)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
            Debug.Log("[CameraFollow] ✅ Tìm thấy Player bằng Tag!");
            return;
        }

        // Cách 2: Fallback - tìm theo script PlayerMovement
        PlayerMovement pm = FindAnyObjectByType<PlayerMovement>();
        if (pm != null)
        {
            target = pm.transform;
            Debug.Log("[CameraFollow] ✅ Tìm thấy Player bằng script PlayerMovement!");
            return;
        }

        Debug.LogWarning("[CameraFollow] ⚠️ Chưa tìm thấy Player! Hãy đảm bảo:\n" +
                         "1. Nhân vật có Tag là 'Player', HOẶC\n" +
                         "2. Nhân vật có script PlayerMovement gắn vào.");
    }

    void LateUpdate()
    {
        // Nếu mất target, thử tìm lại định kỳ
        if (target == null)
        {
            findTimer += Time.deltaTime;
            if (findTimer >= FIND_INTERVAL)
            {
                findTimer = 0f;
                FindPlayer();
            }
            return;
        }

        // Di chuyển camera theo player
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z) + offset;

        // Dùng Lerp với Time.deltaTime để mượt đều trên mọi máy tính
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}
