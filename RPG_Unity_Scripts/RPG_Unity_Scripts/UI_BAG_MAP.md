# 🗺️ BẢN ĐỒ KIẾN TRÚC UI BAG - DARK FANTASY RPG

> Tài liệu tham khảo vĩnh viễn. **KHÔNG XÓA.**
> Lưu lại để đây để bất cứ lúc nào build lại UI đều dùng map này làm chuẩn.

---

## 📐 CẤU TRÚC PHỤ CƠ BẢN (Layering Order)
```
[BACKGROUND]
    ↓
[PANEL FRAME]
    ↓
[LEFT EQUIP PANEL] [CENTER INVENTORY] [RIGHT INFO PANEL]
    ↓
[DECOR + EFFECT + ICON SLOT]
```

---

## 🗂️ CẤU TRÚC THỨ BẬC UI (Hierarchy)
```
Inventory Window
 ├── Background
 ├── Frame
 │    ├── LeftPanel
 │    │     ├── Avatar (Ảnh/Vòng tròn nhân vật)
 │    │     ├── Silhouette (Hình bóng nhân vật)
 │    │     ├── EquipSlots
 │    │           ├── slot_head     → eqHead
 │    │           ├── slot_chest    → eqBody
 │    │           ├── slot_glove    → (mở rộng sau)
 │    │           ├── slot_boot     → eqLegs
 │    │           ├── slot_weapon   → eqWeaponMain
 │    │           ├── slot_weapon2  → eqWeaponOff (Khiên)
 │    │           ├── slot_ring1    → eqRing1
 │    │           ├── slot_ring2    → eqRing2
 │    │           ├── slot_necklace → eqNecklace (Dây chuyền)
 │    │           └── slot_ancient  → eqAncientGold (Vòng cổ)
 │    │
 │    ├── InventoryGrid (HÒM ĐỒ CHUNG)
 │    │     └── Slot (x N) - Lưới 4-5 cột
 │    │
 │    └── RightPanel (THÔNG TIN VẬT PHẨM)
 │          ├── header_bar (Tên item)
 │          ├── icon_area  (Ảnh item lớn)
 │          ├── divider_line
 │          ├── text_area  (Mô tả + chỉ số)
 │          └── action_button (Trang bị / Gỡ bỏ)
 │
 └── Popup / Tooltip
```

---

## 📁 DANH SÁCH TÀI NGUYÊN CẦN CÓ (Assets Map)

### frame/ (Khung viền UI chính - 9-Slice)
| File | Mô tả | Trạng thái |
|------|-------|-----------|
| `corner_top_left.png` | Góc viền trên-trái | ⏳ Chưa có |
| `corner_top_right.png` | Góc viền trên-phải | ⏳ Chưa có |
| `corner_bottom_left.png` | Góc viền dưới-trái | ⏳ Chưa có |
| `corner_bottom_right.png` | Góc viền dưới-phải | ⏳ Chưa có |
| `edge_top.png` | Cạnh trên | ⏳ Chưa có |
| `edge_bottom.png` | Cạnh dưới | ⏳ Chưa có |
| `edge_left.png` | Cạnh trái | ⏳ Chưa có |
| `edge_right.png` | Cạnh phải | ⏳ Chưa có |
| `center_fill.png` | Lấp đầy trung tâm | ⏳ Chưa có |

### left_panel/ (Bảng nhân vật bên trái)
| File | Mô tả | Trạng thái |
|------|-------|-----------|
| `panel_bg.png` | Nền panel trái | ✅ Dùng `dark_fantasy_panel_bg_new.png` |
| `avatar_circle.png` | Vòng tròn chứa ảnh avatar | ⏳ Chưa có |
| `slot_head.png` | Ô trang bị - Đầu | ✅ Dùng `ragged_slot_bg_new.png` |
| `slot_chest.png` | Ô trang bị - Ngực | ✅ Dùng `ragged_slot_bg_new.png` |
| `slot_glove.png` | Ô trang bị - Găng tay | ⏳ Chưa có (slot mới) |
| `slot_boot.png` | Ô trang bị - Giày | ✅ Dùng `ragged_slot_bg_new.png` |
| `slot_weapon.png` | Ô trang bị - Vũ khí | ✅ Dùng `ragged_slot_bg_new.png` |
| `slot_empty.png` | Ô trang bị trống | ⏳ Chưa có |
| `silhouette.png` | Hình bóng nhân vật | ⏳ Chưa có |

### inventory/ (Kho đồ trung tâm)
| File | Mô tả | Trạng thái |
|------|-------|-----------|
| `grid_bg.png` | Nền lưới kho đồ | ✅ Dùng `mobile_master_panel_v3.png` |
| `slot.png` | Ô bình thường | ✅ Dùng `ragged_slot_bg_new.png` |
| `slot_hover.png` | Ô khi hover | ⏳ Code bằng màu |
| `slot_selected.png` | Ô khi chọn | ⏳ Code bằng màu vàng |

### right_panel/ (Bảng thông tin bên phải)
| File | Mô tả | Trạng thái |
|------|-------|-----------|
| `panel_bg.png` | Nền panel phải | ✅ Dùng `dark_fantasy_panel_bg_new.png` |
| `header_bar.png` | Thanh tiêu đề | ⏳ Chưa có |
| `divider_line.png` | Đường ngăn cách | ⏳ Code bằng màu |
| `text_area.png` | Vùng hiển thị text | ⏳ Chưa có |

### ui/ (Các nút và tab)
| File | Mô tả | Trạng thái |
|------|-------|-----------|
| `btn_close.png` | Nút đóng | ⏳ Code bằng GUI.Button |
| `btn_close_hover.png` | Nút đóng hover | ⏳ Code bằng màu |
| `btn_close_pressed.png` | Nút đóng nhấn | ⏳ Code bằng màu |
| `tab_button.png` | Nút Tab | ⏳ Code bằng GUI.Button |
| `tab_active.png` | Tab đang kích hoạt | ⏳ Code bằng màu |

### effects/ (Hiệu ứng)
| File | Mô tả | Trạng thái |
|------|-------|-----------|
| `glow_border.png` | Viền phát sáng | ⏳ Chưa có |
| `light_overlay.png` | Lớp ánh sáng phủ | ⏳ Chưa có |
| `dust_particles.png` | Hạt bụi | ⏳ Chưa có |
| `magic_glow.png` | Glow ma thuật | ⏳ Chưa có |

---

## 📏 LAYOUT / TỌA ĐỘ (Dùng cho code)

### Kích thước chuẩn toàn bộ UI Panel
- **Width:** 960px | **Height:** 540px (16:9)
- **Vị trí:** Căn giữa màn hình

### Phân chia cột (3 panels)
| Khu vực | X bắt đầu | Width | Ghi chú |
|---------|-----------|-------|---------|
| Sidebar (Đội ngũ) | +5 | 155px | Chọn nhân vật trong đội |
| Left Panel (Trang bị) | +160 | 250px | 9 ô trang bị + ảnh nhân vật |
| Center (Kho đồ) | +415 | 320px | Lưới scroll item 4-5 cột |
| Right Panel (Info) | +740 | 215px | Chi tiết item + nút hành động |

### Kích thước ô trang bị
- **Normal Slot:** 64×64px | **Gap:** 10px

### Bố cục 3x3 ô trang bị (Map tọa độ)
```
[Vòng cổ]  [Mũ]   [Dây chuyền]
[Vũ khí]   [Giáp] [Khiên]
[Nhẫn 1]   [Ủng]  [Nhẫn 2]
```

---

## 🔑 TÊN SLOT → FIELD CODE

| Tên hiển thị | Tên Slot (selectedSlot) | Field trong PlayerStats |
|-------------|------------------------|------------------------|
| Vòng cổ | "Ancient" | `eqAncientGold` |
| Mũ | "Head" | `eqHead` |
| Dây chuyền | "Neck" | `eqNecklace` |
| Vũ khí | "WepMain" | `eqWeaponMain` |
| Giáp | "Body" | `eqBody` |
| Khiên | "WepOff" | `eqWeaponOff` |
| Nhẫn 1 | "Ring1" | `eqRing1` |
| Ủng | "Legs" | `eqLegs` |
| Nhẫn 2 | "Ring2" | `eqRing2` |

---

## 🎯 QUY TẮC CODE CHO UI

1. **Tất cả màu** dùng `GUI.color = Color.white` sau mỗi lần vẽ để reset.
2. **Background Layer:** Vẽ trước → Frame → Content → Effects.
3. **Slot Empty State:** Hiển thị chữ placeholder với màu `(0.7f, 0.7f, 0.7f, 0.9f)` font bold.
4. **Slot Selected State:** Highlight viền vàng `Color.yellow`.
5. **Slot Hover State:** Làm sáng màu nền lên khoảng +0.1f mỗi kênh RGB.
6. **Tooltip Panel:** Vẽ sau tất cả các element khác (z-order cao nhất).

---

*Cập nhật lần cuối: 21/04/2026*
