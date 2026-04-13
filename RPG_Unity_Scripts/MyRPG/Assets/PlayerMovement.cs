using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Script để gắn vào Nhân vật (Player), chịu trách nhiệm chức năng rảo bộ quanh bản đồ
public class PlayerMovement : MonoBehaviour
{
    [Header("Cài đặt Di chuyển")]
    public float moveSpeed = 5f; // Tốc độ di chuyển, có thể thay đổi số này trong Unity (Inspector)

    // Update is called once per frame (Khung hình chạy liên tục)
    void Update()
    {
        float moveX = 0f;
        float moveY = 0f;

#if ENABLE_INPUT_SYSTEM
        // Nếu Unity dùng hệ thống Nhập (Input System) đời mới
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveY = 1f; // Sang Lên
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveY = -1f; // Sang Xuống
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1f; // Chạy Trái
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1f; // Chạy Phải
        }
#else
        // Nếu Unity dùng hệ thống cũ
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
#endif

        // Gom số liệu lại thành một hướng đi (Vector3)
        // Dùng Normalized để tránh tình trạng đi chéo (W+D) bị nhanh hơn đi thẳng
        Vector3 movement = new Vector3(moveX, moveY, 0f).normalized;

        // Tiến hành kéo tọa độ của khối vật thể đi một khoảng = (Hướng đi * Tốc độ * Thời gian)
        transform.Translate(movement * moveSpeed * Time.deltaTime);
    }
}
