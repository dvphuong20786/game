# 🧭 HƯỚNG DẪN CHI TIẾT: CÁCH TẠO VẬT PHẨM & KỸ NĂNG (DÀNH CHO NGƯỜI MỚI)

Chào bạn! Hệ thống mới này giúp bạn quản lý game giống như các chuyên gia. Bạn chỉ cần làm theo 3 bước đơn giản dưới đây:

---

## 📂 BƯỚC 1: TẠO "KHO LƯU TRỮ" (BẮT BUỘC)
Code của chúng ta cần một nơi cố định để tìm thấy vật phẩm. Bạn hãy làm đúng trình tự sau trong cửa sổ **Project** của Unity:

1. Click chuột phải vào thư mục **Assets** -> chọn **Create** -> **Folder**. Đặt tên chính xác là: `Resources` (phải viết hoa chữ R).
2. Vào trong thư mục `Resources`, tạo thêm 2 thư mục con:
   - Thư mục đặt tên là: `Items` (chứa vũ khí, giáp, thuốc).
   - Thư mục đặt tên là: `Skills` (chứa các chiêu thức).

> [!CAUTION]
> **Lưu ý cực kỳ quan trọng:** Tên thư mục phải viết đúng từng chữ cái như trên, nếu không game sẽ không tìm thấy đồ bạn tạo!

---

## ⚔️ BƯỚC 2: TẠO MỘT MÓN ĐỒ MỚI (VÍ DỤ: KIẾM SẮT)

1. Vào thư mục `Assets/Resources/Items`.
2. Click chuột phải vào khoảng trống -> chọn **Create** -> **RPG** -> **Item**.
3. Một file mới hiện ra, hãy đặt tên file là: `Kiếm Sắt`.
4. Nhìn sang cột **Inspector** (bên phải màn hình), bạn hãy điền:
   - **Item Name**: `Kiếm Sắt` (nên giống tên file).
   - **Icon**: Kéo một hình ảnh (Sprite) bất kỳ vào ô này.
   - **Type**: Chọn `Weapon` (Vũ khí).
   - **Atk Bonus**: Điền `15` (Sát thương cộng thêm).

---

## ⚡ BƯỚC 3: TẠO KỸ NĂNG (VÍ DỤ: CHÉM GIÓ)

1. Vào thư mục `Assets/Resources/Skills`.
2. Click chuột phải -> chọn **Create** -> **RPG** -> **Skill**.
3. Đặt tên file là: `Chém Gió`.
4. Trong cột **Inspector**:
   - **Skill Name**: Điền `Chém Gió`.
   - **Icon**: Kéo ảnh chiêu thức vào.
   - **Base Cooldown**: Điền `4` (Thời gian hồi chiêu là 4 giây).
   - **Level Descriptions**: Nhấn nút **(+)** 10 lần để tạo ra 10 dòng. Mỗi dòng hãy viết mô tả (Ví dụ: "Level 1: Chém nhẹ", "Level 2: Chém mạnh hơn"...).

---

## 🔗 BƯỚC 4: KẾT NỐI VÀO NHÂN VẬT

Để nhân vật biết sử dụng chiêu thức bạn vừa tạo:

1. Chọn nhân vật **HiepSi** trong cửa sổ Hierarchy.
2. Tìm đến phần **Player Combat (Script)** ở cột Inspector.
3. Bạn sẽ thấy ô **Skill Chem Gio**. Hãy kéo file `Chém Gió` từ thư mục `Skills` vào ô đó.
4. Làm tương tự với các đệ tử trong phần **Companion AI**.

---

## 💡 MẸO NHỎ (TIPS)
- Mỗi khi bạn thay đổi chỉ số trong các file dữ liệu này (Asset), game sẽ tự cập nhật ngay cả khi bạn đang chơi.
- Bạn có thể tạo hàng trăm món đồ khác nhau chỉ bằng cách lặp lại **Bước 2**.

Chúc bạn tạo được những món bảo khí thật mạnh mẽ cho Hiệp sĩ của mình!
