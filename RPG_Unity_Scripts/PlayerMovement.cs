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
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float moveX = 0f;
        float moveY = 0f;

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveY = 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveY = -1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1f;
        }
#else
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
#endif

        Vector3 movement = new Vector3(moveX, moveY, 0f).normalized;
        transform.Translate(movement * moveSpeed * Time.deltaTime);

        // Đẩy tín hiệu sang Animator
        if (anim != null)
        {
            // Nếu có di chuyển thì Speed > 0, không thì Speed = 0 giúp chuyển về Idle
            float currentSpeed = (moveX != 0 || moveY != 0) ? 1f : 0f;
            anim.SetFloat("Speed", currentSpeed);
            
            // Xoay hướng nhân vật (Flip) dựa trên hướng đi của X
            if (moveX > 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
            else if (moveX < 0) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        }
    }
}
