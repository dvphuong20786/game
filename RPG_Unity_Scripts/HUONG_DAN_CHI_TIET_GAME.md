# HƯỚNG DẪN CHI TIẾT CÁC BƯỚC ĐÃ LÀM (WALKTHROUGH)
*(Dùng để đồng bộ và xem lại trên mọi máy tính)*

## 1. CÁCH LẮP RÁP CƠ BẢN TRONG UNITY
- **Gắn Scripts:** Kéo thả các file `.cs` (PlayerStats, Monster, v.v.) vào mục **Inspector** của đối tượng tương ứng trên cột **Hierarchy**.
- **Cơ chế Prefab (Bản mẫu):** Nắm kéo đối tượng từ Hierarchy xuống bảng **Project** để biến nó thành cục màu xanh dương. Sau này dùng nó để đẻ ra vô tận (như Vàng, Quái vật).

## 2. HƯỚNG DẪN ĐỒ HỌA (ÁO MỚI)
- **Texture Type:** Chọn ảnh trong Project -> Inspector -> Texture Type -> Đổi thành **Sprite (2D and UI)** -> Bấm **Apply**.
- **Gán hình:** Kéo ảnh đã đổi Sprite vào ô **Sprite** của thành phần `Sprite Renderer` trên nhân vật.

## 3. BÍ QUYẾT LÀM ANIMATION (ẢNH ĐỘNG)
- **Không dùng GIF:** Game dùng **Sprite Sheet** (Băng phim dài).
- **Cắt ảnh:** Trong Inspector, chọn Sprite Mode: **Multiple** -> Bấm **Sprite Editor** -> Slice -> Gõ số lượng khung hình -> Apply.
- **Tạo Animation:** Quét chọn các miếng ảnh nhỏ đã cắt -> Kéo thả vào nhân vật -> Lưu tên File (vd: `HiepSi_Walk`).
- **Nối dây (Animator):** Mở cửa sổ **Animator** -> Nối dây giữa các trạng thái (Idle -> Walk) -> Cài đặt tham số (Parameter) là `Speed` hoặc `Attack` để gọi lệnh từ C# code.

## 4. HƯỚNG DẪN TÚI ĐỒ & TRANG BỊ
- **Mở túi:** Nhấn phím **B** khi đang chơi để mở bảng ma thuật màu đen.
- **Trang bị:** Nhấp chuột vào nút "Trang bị: [Tên Kiếm]" để tăng Lực Chiến (Attack Damage) ngay lập tức.
- **Quy tắc rớt đồ:** Quái chết rớt đồ hên xui. Kẹo vàng (70%) cộng EXP, Kiếm (30%) chui vào túi đồ.

---
> [!TIP]
> **Lưu ý khi đồng bộ:** 
> Khi về nhà, bạn chỉ cần `git pull` để lấy toàn bộ code và file này về. Nếu Unity báo lỗi "Missing Script", hãy kiểm tra xem các file đã được gắn đúng vào đối tượng trong Inspector chưa nhé!
