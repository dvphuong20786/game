# 🎒 Kế hoạch: Đại tu khung Bag (Túi đồ) thành dạng Lưới

Kế hoạch này sẽ chuyển đổi danh sách túi đồ từ dạng hàng dọc sang dạng lưới (Grid) cổ điển, sử dụng các tài nguyên rách rưới đã tạo để tạo cảm giác chuyên nghiệp hơn.

## 🛠️ Trọng tâm thay đổi
### 1. Cấu trúc Lưới (Grid Layout)
- Thay thế danh sách cuộn dọc đơn thuần bằng lưới 4 cột (hoặc 5 tùy diện tích).
- Mỗi ô trang bị trong túi sẽ sử dụng `ragged_slot_bg_new` (64x64 hoặc 58x58) để đồng bộ với các ô trang bị trên người.
- Hiển thị Icon vật phẩm và các hạt ngọc nhỏ (Socket) ngay trên ô lưới.

### 2. Giao diện (Aesthetics)
- **Nền túi**: Thêm một lớp nền vải tối hoặc gỗ rỉ sét đằng sau các ô đồ.
- **Hiệu ứng chọn**: Khi chọn một ô, viền sẽ sáng lên màu vàng đồng.
- **Tooltip**: Giữ nguyên logic hiển thị thông tin bên phải khi click vào ô.

### 3. Tối ưu hóa code `GameUI.cs`
- Refactor hàm `DrawInventory` để tính toán tọa độ X, Y theo hàng và cột.
- Sử dụng `GUI.BeginGroup` hoặc tính toán Rect chính xác trong ScrollView để đảm bảo hiệu năng.

## 🧪 Kế hoạch Xác minh
- **Kiểm tra cuộn (Scrolling)**: Đảm bảo khi túi đồ đầy, lưới vẫn cuộn mượt mà.
- **Tương tác**: Click vào các ô grid phải hiện đúng Tooltip và nút hành động (Mặc/Sử dụng).
- **Độ phân giải**: Kiểm tra lưới hiển thị ổn định trên các tỉ lệ màn hình khác nhau.

---
> [!NOTE]
> Việc chuyển sang dạng lưới sẽ giúp người chơi nhìn bao quát được nhiều món đồ hơn và mang lại cảm giác RPG truyền thống như Diablo hay PoE.
