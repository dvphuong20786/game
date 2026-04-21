# 🏆 Kế hoạch: Đại tu Giao diện Bag "Triệu Người Dùng"

Kế hoạch này sẽ thay đổi hoàn toàn cấu trúc hiện tại của `GameUI.cs` để đạt được độ hoàn thiện và thẩm mỹ như bức ảnh mẫu bạn cung cấp.

## 🎨 Trọng tâm Mỹ thuật & Layout
- **Giao diện Hợp nhất**: Thay vì các bảng rời rạc, tôi sẽ tạo một nền Panel khổng lồ duy nhất bao trùm toàn bộ các khu vực: Đội ngũ, Chỉ số, Túi đồ và Chi tiết vật phẩm.
- **Bố cục 4 Phân vùng**:
    1. **Sidebar (Trái)**: Chọn nhân vật trong đội.
    2. **Character (Giữa-Trái)**: Hiện Portrait, lưới trang bị 3x3 và bảng cộng điểm tiềm năng.
    3. **Grid Bag (Giữa-Phải)**: Lưới ô đồ lớn (ví dụ 6x10) hiển thị toàn bộ trang bị.
    4. **Tooltip (Phải)**: Bảng thông tin chi tiết cố định với nút "TRANG BỊ" lớn và chuyên nghiệp.
- **Lược bỏ**: Tạm thời xóa các nút "LƯU" và "NEW" ở góc trái dưới để dọn dẹp không gian.

## 🛠️ Triển khai Kỹ thuật
### 1. Tạo tài nguyên mới (Assets)
- [ ] **Master Background**: Tạo một ảnh nền Panel cực đại (1200x800) với các ngăn được phân chia bằng viền sắt rỉ sét.
- [ ] **Pro Buttons**: Tạo Sprite nút bấm kiểu kim loại/vàng cho nút "TRANG BỊ" và các nút cộng điểm.

### 2. Cập nhật `GameUI.cs`
- [ ] **Tăng kích thước UI**: Mở rộng `uiW` lên khoảng 1100-1200 để chứa đủ 4 phân vùng.
- [ ] **Viết lại `DrawCharacterTab`**: Sử dụng tọa độ tuyệt đối để gióng hàng chuẩn xác như ảnh mẫu.
- [ ] **Cải tiến Tooltip**: Hiển thị ảnh Item lớn hơn, chỉ số có icon riêng (kiếm cho ATK, khiên cho DEF).
- [ ] **Xóa Footer cũ**: Loại bỏ hàm `DrawFooter`.

### 3. Đánh bóng (Polishing)
- [ ] Sử dụng bảng màu: Vàng đồng (#D4AF37), Xám sắt, Đỏ máu.
- [ ] Font chữ: Căn chỉnh size và độ đậm (Bold) cho từng phân đoạn.

## 🧪 Kế hoạch Xác minh
- **Kiểm tra độ phân giải**: Đảm bảo UI co giãn tốt trên các màn hình khác nhau (chế độ Windowed/Fullscreen).
- **Trải nghiệm người dùng**: Thử nghiệm luồng: Chọn đệ tử -> Chọn đồ trong túi -> Xem Tooltip bên phải -> Chọn "TRANG BỊ" -> Đồ bay vào đúng slot.

---
> [!IMPORTANT]
> Đây là một cuộc "đại tu" về cấu trúc. Tôi sẽ cần tạo ra một ảnh nền Master mới để làm bản lề cho toàn bộ các thành phần khác.
