# 📱 Kế hoạch: Đại tu Giao diện Bag "Triệu Người Dùng" - Phiên bản Mobile

Kế hoạch này tập trung vào việc tạo ra một giao diện túi đồ đẳng cấp, tối ưu hoàn toàn cho màn hình cảm ứng của điện thoại (ngang), dựa trên phong cách Dark Fantasy chuyên nghiệp.

## 🎨 Trọng tâm Thiết kế (Mobile Optimized)
- **Kích thước Cửa sổ**: Cửa sổ giao diện trung tâm được thiết kế tỷ lệ chuẩn mobile, không quá to làm choáng ngợp nhưng đủ rộng để các nút bấm (touch targets) dễ chạm.
- **Bố cục 4 Phân vùng tích hợp**:
    1. **Sidebar (Trái)**: Nằm gọn trong khung lớn, các nút chọn nhân vật to, rõ.
    2. **Character & Stats (Giữa-Trái)**: Lưới trang bị 3x3 và bảng cộng điểm với các nút [+] lớn dễ thao tác bằng ngón cái.
    3. **Bag Grid (Giữa-Phải)**: Lưới ô đồ lớn, khoảng cách an toàn giữa các ô để tránh chạm nhầm.
    4. **Pro Tooltip (Phải)**: Hiển thị đầy đủ thông tin và quan trọng nhất là nút **"TRANG BỊ"** khổng lồ ở dưới cùng.
- **Logic Trang bị**: Người chơi phải nhấn vào nút "TRANG BỊ" trong bảng chi tiết mới thực hiện mặc đồ (không còn click-auto-equip).

## 🛠️ Triển khai Kỹ thuật
### 1. Tạo tài nguyên Mobile (Assets)
- [ ] **Mobile Master Background**: Tạo ảnh nền Panel (tỉ lệ 16:9) chia sẵn các vùng, phong cách rỉ sét/đá cổ, tối ưu cho màn hình điện thoại.
- [ ] **Nút Trang Bị & Cộng điểm**: Sprite nút bấm lớn, nổi bật với tông màu đồng/vàng.

### 2. Cập nhật `GameUI.cs`
- [ ] **Layout Mobile**: Viết lại hàm `DrawCharacterTab` sử dụng một hệ tọa độ scale theo tỉ lệ màn hình mobile (sử dụng `Screen.width` và `Screen.height` làm mốc).
- [ ] **Tích hợp Sidebar**: Gắn chặt Sidebar vào khung chính.
- [ ] **Refactor Equip Logic**: Sửa lại cơ chế chọn đồ: Click vào grid chỉ để "Xem", nhấn nút "TRANG BỊ" mới gọi hàm `EquipItem()`.
- [ ] **Loại bỏ Footer**: Xóa 2 nút Lưu/New cũ.

## 🧪 Kế hoạch Xác minh
- **Kiểm tra Touch Area**: Đảm bảo các ô đồ và nút bấm đủ to cho người dùng mobile.
- **Kiểm tra luồng Mobile**: Mở Bag -> Chọn đệ tử (Sidebar) -> Chọn đồ (Grid) -> Xem thông tin (Tooltip) -> Nhấn "TRANG BỊ".

---
> [!IMPORTANT]
> Giao diện này sẽ dính liền thành một khối thống nhất (Unified Window), không còn các bảng trôi lơ lửng như trước.
