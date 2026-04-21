# Kế hoạch Sửa lỗi Giao diện & Thêm ô Vòng cổ

Người dùng báo cáo giao diện Pro (Version B) bị thiếu ô "Vòng cổ" và các ô trang bị không khớp với hình nền nhân vật. Chúng ta sẽ tiến hành tái cấu trúc lưới trang bị để đạt được sự cân xứng và chính xác tuyệt đối.

## User Review Required

> [!IMPORTANT]
> **Thay đổi bố cục (Layout):** Tôi sẽ chuyển các ô trang bị sang dạng lưới 3x3 cân xứng. Ô **Mũ**, **Giáp**, và **Giày** sẽ nằm ở cột chính giữa để khớp hoàn hảo với hình bóng nhân vật (silhouette) trong hình nền.
> 
> **Tên gọi mới cho các ô Accessory:**
> - `eqNecklace` (Cũ là "Dây") -> Chuyển thành **"Dây chuyền"**.
> - `eqAncientGold` (Cũ là "Vàng", đang bị thiếu) -> Chuyển thành **"Vòng cổ"**.

## Proposed Changes

### 🎨 Cấu trúc Lưới Trang bị Mới (3x3 Pro)
| Cột Trái | Cột Giữa (Khớp Nhân Vật) | Cột Phải |
| :--- | :--- | :--- |
| **Vòng cổ** (`eqAncientGold`) | **Mũ** (`eqHead`) | **Dây chuyền** (`eqNecklace`) |
| **Vũ khí** (`eqWeaponMain`) | **Giáp** (`eqBody`) | **Khiên** (`eqWeaponOff`) |
| **Nhẫn 1** (`eqRing1`) | **Ủng** (`eqLegs`) | **Nhẫn 2** (`eqRing2`) |

---

### [Component: UI System]

#### [MODIFY] [GameUI.cs](file:///f:/WORK/GAME/RPG_Unity_Scripts/MyRPG/Assets/GameUI.cs)
- Cập nhật hàm `DrawCharacterTabPro` để vẽ lưới 9 ô (thêm ô `eqAncientGold`).
- Điều chỉnh tọa độ các ô để căn giữa tuyệt đối so với khu vực nhân vật.
- Cập nhật nhãn (label) rõ ràng hơn cho các ô trang bị.

### [Component: Player Stats]

#### [MODIFY] [PlayerStats.cs](file:///f:/WORK/GAME/RPG_Unity_Scripts/MyRPG/Assets/Player/PlayerStats.cs)
- Cập nhật logic `EquipItem` để tự động nhận diện vật phẩm có chứa chữ "vòng cổ" và đưa vào đúng slot `AncientGold`.

---

## Open Questions

- Bạn có muốn đổi tên "Hiệp Sĩ" (Warrior) thành tên khác cho chuyên nghiệp hơn không? (Ví dụ: "Chiến Binh", "Kiếm Sĩ").
- Bạn có muốn tôi tăng độ tương phản của các chữ placeholder (Mũ, Nhẫn...) để dễ đọc hơn trên nền tối không?

## Verification Plan

### Automated Tests
- Khởi động game, nhấn phím `B` để mở UI Version B.
- Kiểm tra xem 9 ô đã xuất hiện đầy đủ chưa.
- Kiểm tra sự cân xứng của Mũ/Giáp/Boots so với hình bóng nhân vật phía sau.

### Manual Verification
- Nhặt 1 vật phẩm "Dây chuyền" và 1 "Vòng cổ" để xem chúng có tự động vào đúng 2 ô trên cùng không.
- Thử trang bị/gỡ bỏ để đảm bảo logic vẫn hoạt động trơn tru.
