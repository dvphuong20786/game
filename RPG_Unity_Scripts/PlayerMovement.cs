using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Script để gắn vào Nhân vật (Player), chịu trách nhiệm chức năng di chuyển
public class PlayerMovement : MonoBehaviour
{
    [Header("Cài đặt Di chuyển")]
    public float moveSpeed = 5f;

    private Animator anim;
    private SpriteRenderer sr;

    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
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

        // ===== GỬI TÍN HIỆU ANIMATOR =====
        if (anim != null)
        {
            bool isMoving = (moveX != 0 || moveY != 0);

            // Dùng CẢ HAI: Float Speed (để transition) và Bool IsMoving (để chắc chắn hơn)
            anim.SetFloat("Speed", isMoving ? 1f : 0f);

            // Nếu Animator có bool "IsMoving" thì dùng thêm
            // (Tạo trong Unity: Parameters → [+] → Bool → đặt tên "IsMoving")
            foreach (AnimatorControllerParameter p in anim.parameters)
            {
                if (p.name == "IsMoving" && p.type == AnimatorControllerParameterType.Bool)
                {
                    anim.SetBool("IsMoving", isMoving);
                    break;
                }
            }
        }

        // ===== LẬT MẶT NHÂN VẬT =====
        // Dùng SpriteRenderer.flipX thay vì scale để tránh xung đột với Camera
        if (sr != null)
        {
            if (moveX > 0) sr.flipX = false;  // Đi phải → mặt phải
            else if (moveX < 0) sr.flipX = true;  // Đi trái → lật mặt
        }
        else
        {
            // Fallback dùng localScale nếu không có SpriteRenderer trực tiếp
            if (moveX > 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
            else if (moveX < 0) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        }
    }
}
