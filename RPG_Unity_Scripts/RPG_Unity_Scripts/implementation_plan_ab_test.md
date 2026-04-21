# 🧪 Kế hoạch: Hệ thống So sánh Giao diện (A/B UI Test)

Kế hoạch này cho phép bạn so sánh trực tiếp giữa giao diện hiện tại (đang bị lệch ảnh nền) và giao diện mới đã được tối ưu hóa.

## 🛠️ Trọng tâm thay đổi
### 1. Quản lý Tài nguyên (Assets)
- **UI A (Hiện tại)**: Giữ nguyên `dark_fantasy_panel_bg_new.png`.
- **UI B (Mới)**: Tạo ảnh `dark_fantasy_panel_v2.png` với khung trống hoàn toàn bên trong (chỉ có viền và vân gỗ tối) để khớp với Grid vẽ bằng code.

### 2. Cập nhật `GameUI.cs` (Logic chuyển đổi)
- **Hệ thống Phím tắt**:
    - Nhấn **phím A**: Mở/Tắt túi đồ sử dụng giao diện hiện tại (v1).
    - Nhấn **phím B**: Mở/Tắt túi đồ sử dụng giao diện mới (v2).
- **Hỗ trợ đa cấu trúc**:
    - Tạo biến `uiVersion` để ghi nhớ bạn đang chọn xem bản nào.
    - Trong hàm `DrawDarkFantasyPanel`, script sẽ tự động chọn Texture phù hợp dựa trên `uiVersion`.

### 3. Tối ưu hóa UI B
- Khi chọn UI B, tôi sẽ căn chỉnh lại các cột (Sidebar, Inventory, Tooltip) để chúng nằm lọt lòng trong khung sắt rỉ sét một cách đẹp nhất.

## 🧪 Kế hoạch Xác minh
- **Kiểm tra phím bấm**: Đảm bảo cả phím A và B đều hoạt động và không gây lỗi khi đang di chuyển.
- **So sánh trực quan**: Người dùng có thể nhấn phím qua lại giữa A và B để thấy sự khác biệt về độ khớp của các ô đồ.

---
> [!IMPORTANT]
> Việc giữ lại bản cũ trên phím A giúp bạn luôn có phương án dự phòng an toàn nếu bản mẫu mới chưa ưng ý.
