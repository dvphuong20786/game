# Kế hoạch: Nâng cấp UI Quản lý Đội hình (Multi-Companion HUD)

Hiện tại hệ thống UI chỉ nhận diện 1 đệ tử đầu tiên. Tôi sẽ nâng cấp để bạn có thể quản lý tối đa 4 đệ tử cùng lúc với đầy đủ thanh máu và chỉ số.

## User Review Required

> [!IMPORTANT]
> **Danh sách Đệ tử**: Tôi sẽ thay đổi cách UI tìm kiếm đệ tử. Thay vì chỉ tìm 1 người, nó sẽ quét toàn bộ đội ngũ và hiển thị danh sách thanh máu bên trái.
> **Sửa lỗi Máu 0/175**: Tôi sẽ thêm lệnh "Cấp cứu" vào `PlayerStats` để đảm bảo đệ tử vừa được thuê sẽ có máu đầy đủ ngay lập tức.

## Proposed Changes

---

### [Component] Giao diện người dùng (GameUI.cs)

#### [MODIFY] [GameUI.cs](file:///f:/WORK/GAME/RPG_Unity_Scripts/GameUI.cs)
- Thay đổi biến `companion` đơn lẻ thành `List<PlayerStats> companions`.
- Cập nhật `TryFindPlayer()` để thu thập toàn bộ đệ tử trong Scene (không giới hạn 1 người).
- Cập nhật `DrawHUD()` để vẽ nhiều thanh máu đệ tử xếp chồng lên nhau (cách nhau 90px).
- Nâng cấp nút đổi nhân vật để có thể xoay vòng qua: Người chơi -> Đệ tử 1 -> Đệ tử 2 -> ...

---

### [Component] Chỉ số nhân vật (PlayerStats.cs)

#### [MODIFY] [PlayerStats.cs](file:///f:/WORK/GAME/RPG_Unity_Scripts/PlayerStats.cs)
- Cải thiện logic khởi tạo trong `Start()` để đảm bảo `currentHealth` luôn bằng `maxHealth` khi mới sinh ra cho đệ tử.

---

## Verification Plan

### Manual Verification
- Bạn chạy game, thuê 3 Archers.
- **Kết quả mong đợi**: 
    1. Xuất hiện 3 thanh máu đệ tử bên trái màn hình.
    2. Máu của họ phải hiện đầy đủ (ví dụ 175/175), không được là 0/175.
    3. Bạn có thể nhấn nút "XEM ĐỆ TỬ" trong túi đồ để soi đồ của từng người một.
