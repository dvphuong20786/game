# 🏰 Kế hoạch: Đại tu Toàn diện Giao diện "Triệu Người Dùng"

Tôi chân thành xin lỗi vì sự sai sót trong việc căn chỉnh ở phiên bản trước. Kế hoạch này sẽ tập trung vào việc "mổ xẻ" lại toàn bộ logic vẽ UI để đạt được độ chính xác tuyệt đối như ảnh mẫu bạn mong muốn.

## 🛠️ Trọng tâm thay đổi
### 1. Phân tách Logic Tuyệt đối (A/B Test)
- **Hệ thống phím bấm**: 
    - Nhấn **A**: Chạy hàm `DrawV1Old()` - Giữ nguyên giao diện hiện tại bạn đang dùng ổn định.
    - Nhấn **B**: Chạy hàm `DrawV2Pro()` - Một logic vẽ hoàn toàn mới, không dùng chung code cũ để đảm bảo không bị chồng lấn.

### 2. Mỹ thuật 2.0 (True Blank Master)
- **Vấn đề hiện tại**: Ảnh nền cũ có vẽ sẵn các ô vuông nên khi code vẽ đè lên bị lệch (Double Grid).
- **Giải pháp**: Tôi sẽ tạo một ảnh nền Master mới chỉ có **Khung viền sắt** và **Các thanh vách ngăn** dọc chia thành 4 cột. Lòng bên trong sẽ để trống hoàn toàn. Toàn bộ ô vuông sẽ được vẽ bằng Code để đảm bảo khớp 100% với logic game.

### 3. Cấu trúc Layout "Siêu Chuyên Nghiệp" cho Mobile
Tôi sẽ chia màn hình thành 4 cột cố định:
1. **Cột 1 (Sidebar)**: Chọn Hiệp sĩ/Đệ tử.
2. **Cột 2 (Thông tin & Trang bị)**: Hiện chỉ số, Level, Vàng và lưới trang bị 3x3 (vẽ bằng code).
3. **Cột 3 (Túi đồ lớn)**: Lưới 5x8 hoặc 6x8 tùy diện tích, có thanh cuộn mượt mà.
4. **Cột 4 (Chi tiết & Thao tác)**: Info vật phẩm và nút "TRANG BỊ" nằm dưới cùng.

## 🛠️ Triển khai Kỹ thuật
- **Asset**: Generate lại `mobile_master_panel_clean.png`.
- **Code**: Refactor `GameUI.Canvas` (hoặc logic OnGUI) để tách biệt 2 version.
- **UI B**: Sử dụng tọa độ tuyệt đối (Absolute Positioning) dựa trên tỉ lệ màn hình Mobile để không bị trôi (Floating).

## 🧪 Kế hoạch Xác minh
- **Bản A**: Phải hoạt động y hệt như lúc chưa sửa.
- **Bản B**: Phải khớp hoàn toàn với ảnh mockup bạn gửi (không lệch ô, không chồng chéo).

---
> [!IMPORTANT]
> Tôi sẽ thực hiện mổ xẻ mã nguồn hiện có để cấu trúc lại sạch sẽ nhất. Bạn hãy xác nhận để tôi bắt đầu.
