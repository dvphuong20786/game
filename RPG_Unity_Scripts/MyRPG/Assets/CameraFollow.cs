using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target; // Nhân vật để camera đuổi theo
    public float smoothSpeed = 0.125f; // Độ mượt của camera
    public Vector3 offset = new Vector3(0, 0, -10); // Khoảng cách giữa cam và người

    void Start()
    {
        // Tự động tìm nhân vật có tên là "Player" hoặc có gắn PlayerMovement
        PlayerMovement p = FindAnyObjectByType<PlayerMovement>();
        if (p != null) target = p.transform;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Tính toán vị trí mới của camera
        Vector3 desiredPosition = target.position + offset;
        
        // Di chuyển mượt mà tới đó (Lerp)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
        transform.position = smoothedPosition;
    }
}
