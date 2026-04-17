using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;
    private Transform target;
    public float smoothSpeed = 12f; // Tốc độ bám đuôi nhanh hơn (User yêu cầu)
    public Vector3 offset = new Vector3(0, 0, -10);

    private float findTimer = 0f;
    private const float FIND_INTERVAL = 2f;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        FindPlayer();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null) Debug.Log("[Camera] ✅ Đã chuyển mục tiêu theo dõi sang: " + target.name);
    }

    void FindPlayer()
    {
        if (target != null) return;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
            return;
        }

        PlayerMovement pm = FindAnyObjectByType<PlayerMovement>();
        if (pm != null)
        {
            target = pm.transform;
            return;
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            findTimer += Time.deltaTime;
            if (findTimer >= FIND_INTERVAL) { findTimer = 0f; FindPlayer(); }
            return;
        }

        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z) + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}

