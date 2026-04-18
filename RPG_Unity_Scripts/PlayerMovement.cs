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

        // --- HỒI MÁU NHANH (Phím H) ---
        if (Input.GetKeyDown(KeyCode.H))
        {
            PlayerStats stats = GetComponent<PlayerStats>();
            if (stats != null)
            {
                int potIdx = -1;
                for (int i = 0; i < stats.inventory.Count; i++)
                {
                    if (stats.inventory[i].Contains("Bình Máu")) { potIdx = i; break; }
                }
                if (potIdx != -1) stats.UseConsumable(potIdx);
                else if (GameUI.instance != null) GameUI.instance.ShowDamage(transform.position, "HẾT THUỐC!", Color.gray);
            }
        }

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
        if (sr != null)
        {
            if (moveX > 0) sr.flipX = false;
            else if (moveX < 0) sr.flipX = true;
        }
        else
        {
            if (moveX > 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
            else if (moveX < 0) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        }
    }

    // ===== THANH MÁU NỔI TRÊN ĐẦU NHÂN VẬT CHÍNH =====
    private PlayerStats _pStats;
    void OnGUI()
    {
        if (_pStats == null) _pStats = GetComponent<PlayerStats>();
        if (_pStats == null || Camera.main == null) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        if (screenPos.z <= 0) return;

        float barW = 60f;
        float barH = 6f;
        float yOff = 55f;

        Rect bgR = new Rect(screenPos.x - barW / 2, Screen.height - screenPos.y - yOff, barW, barH);
        float hpRatio = Mathf.Clamp01((float)_pStats.currentHealth / _pStats.maxHealth);

        // Nền đen
        GUI.color = Color.black;
        GUI.DrawTexture(bgR, Texture2D.whiteTexture);

        // Màu thay đổi theo máu: Xanh > Vàng > Đỏ
        GUI.color = hpRatio > 0.5f ? Color.green : (hpRatio > 0.25f ? Color.yellow : Color.red);
        GUI.DrawTexture(new Rect(bgR.x, bgR.y, barW * hpRatio, barH), Texture2D.whiteTexture);

        // Gợi ý dùng bình máu
        GUI.color = Color.white;
    }
}
