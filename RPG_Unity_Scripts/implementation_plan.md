# Kế hoạch Nâng cấp Hệ thống Đệ tử (Companion AI)

Nâng cấp Đệ tử từ một AI đơn giản thành một nhân vật có chiều sâu, có thể thăng cấp, mặc trang bị và học kỹ năng riêng biệt.

## User Review Required

> [!IMPORTANT]
> **Cơ chế Chia sẻ Đồ:** Tôi đề xuất thêm nút "Giao cho Đệ Tử" trong túi đồ của Người chơi để chuyển vật phẩm sang cho Đệ tử. Bạn có đồng ý không?
>
> **Cơ chế Vàng:** Đệ tử nên dùng chung túi tiền với Người chơi hay có tiền riêng? (Tạm thời tôi sẽ để dùng chung tiền để dễ quản lý).

## Proposed Changes

### [Component] Core Stats & RPG Logic

#### [MODIFY] [PlayerStats.cs](file:///d:/work/1/RPG_Unity_Scripts/PlayerStats.cs)
- Chỉnh sửa để script này có thể gắn vào cả Người chơi và Đệ tử.
- Bỏ ràng buộc Singleton cứng nhắc (chỉ người chơi chính mới giữ `instance`).
- Thêm biến `isCompanion` để phân biệt logic (ví dụ: Đệ tử không tự lưu game đè lên Player).
- Cập nhật hàm `TakeDamage`, `LevelUp` để thông báo damage text đúng vị trí của Đệ tử.

### [Component] Companion AI Logic

#### [MODIFY] [CompanionAI.cs](file:///d:/work/1/RPG_Unity_Scripts/CompanionAI.cs)
- Xóa các biến hardcode (`attackDamage`, `movementSpeed`).
- Lấy chỉ số trực tiếp từ `PlayerStats` gắn trên chính nó.
- Cải tiến AI: Biết tự dùng kỹ năng (ví dụ: Chém Gió) khi quái đông.
- Sử dụng tầm đánh (`attackRange`) từ vũ khí đang mặc.

### [Component] User Interface (UI)

#### [MODIFY] [GameUI.cs](file:///d:/work/1/RPG_Unity_Scripts/GameUI.cs)
- Thêm nút chuyển đổi (Chế độ: Người chơi / Đệ tử) trong bảng túi đồ (phím B).
- Khi chọn "Đệ tử", toàn bộ bảng Trang bị, Túi đồ và Kỹ năng sẽ hiển thị dữ liệu của Đệ tử.
- Thêm nút "Giao cho đệ tử" khi đang xem túi đồ người chơi.

## Verification Plan

### Automated Tests
- Kiểm tra Đệ tử có nhận được chỉ số bonus khi mặc đồ không.
- Kiểm tra Đệ tử có lên cấp khi cùng tiêu diệt quái không.

### Manual Verification
- Mở túi đồ (phím B), bấm nút chuyển sang Đệ tử, thử mặc một cái áo cho đệ tử và xem máu của nó có tăng lên không.
- Đứng nhìn Đệ tử đánh quái xem nó có dùng kỹ năng đã học không.
