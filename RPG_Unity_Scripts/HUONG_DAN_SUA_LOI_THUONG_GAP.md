# 🆘 Hướng dẫn: Cách Sửa Các Lỗi Thường Gặp (Troubleshooting)

Đây là bản "bí kíp" để bạn tự sửa nhanh các lỗi phổ biến khi kéo thả hoặc vô tình xóa nhầm linh kiện trong Unity.

---

## 1. Lỗi: Màn hình đen ("No cameras rendering")
**Dấu hiệu**: Bạn nhấn Play nhưng màn hình Game đen kịt hoặc hiện chữ "No cameras rendering".
- **Nguyên nhân**: Đối tượng Camera bị tắt hoặc bị xóa mất linh kiện Camera.
- **Cách sửa**: 
    1. Chọn **Main Camera** trong Hierarchy.
    2. Đảm bảo ô vuông tích ở trên cùng (cạnh tên đối tượng) đã được **Tích (V)**.
    3. Kiểm tra xem có mục **Camera** trong Inspector không. Nếu không, bấm **Add Component** -> Gõ **Camera** để gắn lại.

## 2. Lỗi: Không hiện số Dame khi đánh quái
**Dấu hiệu**: Đánh quái vẫn mất máu nhưng không nhảy số chữ trên đầu.
- **Nguyên nhân**: Camera chưa được đánh dấu là "Camera Chính".
- **Cách sửa**:
    1. Chọn **Main Camera**.
    2. Ở phần **Tag** (trên cùng Inspector), chọn đúng chữ: **MainCamera**. (Chính tả rất quan trọng, không được chọn Untagged).

## 3. Lỗi: Camera không nhìn thấy mặt đất
**Dấu hiệu**: Màn hình toàn màu xanh/đen của nền mà không thấy cỏ cây đâu.
- **Nguyên nhân**: Camera đang nằm cùng độ cao với mặt đất.
- **Cách sửa**:
    1. Chọn **Main Camera**.
    2. Ở phần **Transform -> Position**, sửa ô **Z** thành **-10**.

## 4. Lỗi: Không nhặt được đồ / Không đánh được quái
**Dấu hiệu**: Chạy đè lên vàng/vũ khí nhưng không có chuyện gì xảy ra.
- **Nguyên nhân**: Nhân vật của bạn đã hết máu (HP = 0) nên hệ thống coi như đã chết và ngừng nhận vật phẩm.
- **Cách sửa**:
    1. Chọn **HiepSi** trong Hierarchy.
    2. Tìm script **Player Stats**.
    3. Sửa ô **Current Health** thành số máu tối đa (ví dụ **175**).
    4. Nhấn **Play** lại.

---

> [!TIP]
> **Mẹo nhỏ**: Mỗi khi Unity báo lỗi đỏ ở cửa sổ **Console**, bạn có thể nhấn nút **Clear** (góc trái) để xóa sạch các thông báo cũ, giúp bạn dễ dàng nhìn thấy lỗi mới phát sinh hơn.

*File này sẽ được lưu mãi mãi để bạn tra cứu bất cứ khi nào gặp trục trặc!*
