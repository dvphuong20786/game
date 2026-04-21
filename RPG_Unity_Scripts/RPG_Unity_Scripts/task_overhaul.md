# 📝 Tiến độ: Đại tu Giao diện Bag "Mổ xẻ"

Dưới đây là danh sách chi tiết các việc đã làm và đang làm. **TUYỆT ĐỐI KHÔNG XÓA** các phần này.

## ✅ Đã hoàn thành (Done)
- [x] **Mổ xẻ mã nguồn (Dissection)**: 
    - Đã tách biệt hoàn toàn hàm `DrawVersionOld` (phiên bản A) và `DrawVersionPro` (phiên bản B).
    - Cấu trúc lại các hàm con (`DrawSidebar`, `DrawCharacterTab`, `DrawInventory`) thành phiên bản `Old` và `Pro`.
- [x] **Logic A/B Test**:
    - Nhấn **A** để gọi bản cũ, nhấn **B** để gọi bản mới.
- [x] **Layout 4 Cột (Pro structure)**:
    - Thiết lập hệ tọa độ tuyệt đối cho 4 phân vùng chính trong bản B.

## 🚧 Đang thực hiện (In Progress)
- [/] **Tài nguyên "Sạch" (Clean Assets)**:
    - Đang thử nghiệm tạo ảnh nền Master không chứa ô vuông có sẵn (tránh lỗi chồng grid). 
- [/] **Căn chỉnh Pixel-Perfect**:
    - Tinh chỉnh vị trí các ô đồ (vẽ bằng code) để khớp với vách ngăn trên nền mới.

## 📋 Việc cần làm tiếp theo (To-Do)
- [ ] **Tích hợp ảnh nền Blank**: Thay thế ảnh nền master có ô vuông bằng ảnh nền sạch.
- [ ] **Hoàn thiện Tooltip Pro**: Làm đẹp phần hiển thị chỉ số và nút Trang bị.
- [ ] **Kiểm tra Mobile**: Đảm bảo Touch Area không bị lệch trên màn hình cảm ứng.

---
> [!NOTE]
> Bạn cần kéo thả ảnh `mobile_master_panel_pro.png` (khi tôi tạo xong) vào thư mục `Resources/Sprites` nếu tôi không thể ghi đè trực tiếp.
