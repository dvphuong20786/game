# 📜 NHẬT KÝ TIẾN ĐỘ PHÁT TRIỂN RPG 2D (TỔNG HỢP TOÀN DIỆN)

Đây là file ghi chép tất cả các cột mốc quan trọng của dự án. **Không xóa bất kỳ mục nào**, chỉ bổ sung thêm để bạn và tôi theo dõi toàn bộ quá trình.

---

## ✅ 1. LỊCH SỬ NHỮNG GÌ TÔI ĐÃ CODE XONG (SOFTWARE CORE)

### 💎 Giai đoạn Mới nhất: Hệ thống RPG Cao cấp
- **9 Ô Trang bị (9 slots):** Hỗ trợ đầy đủ: Đầu, Áo, Giày, Dây chuyền, Nhẫn 1, Nhẫn 2, Vàng cổ, Vũ khí chính, Khiên/Vũ khí phụ.
- **Logic 1 Tay & 2 Tay (1H/2H):**
    - Cầm vũ khí "Đại" hoặc "2 Tay" -> Tự động tháo Khiên.
    - Cầm vũ khí "1 Tay" -> Cho phép dùng Khiên tăng thủ.
- **Hệ thống Khảm Ngọc (Socket System):** Tự động nhận diện Ngọc trong tên đồ để cộng chỉ số mạnh mẽ (Ngọc Đỏ: +ATK, Ngọc Xanh: +DEF, Ngọc Tím: +HP).
- **Tuyệt Chiêu Theo Cấp Độ (Skill Tree):**
    - Hệ thống học kỹ năng mỗi 3 Level.
    - Chiêu **Chém Gió (Lv3)**: Nhấn phím **số 1** để tung đòn sát thương x2.
- **Giao diện Tab (UI Tabs):** Bảng nhân vật chuyên nghiệp với 2 Tab: **[TRANG BỊ]** và **[KỸ NĂNG]**.
- **Hệ thống "An toàn & Chống lỗi":** Đã fix lỗi đứng game (ArgumentOutOfRange) khi bạn mặc hoặc bán đồ quá nhanh.

### 🛡️ Giai đoạn Trước: Giao diện & Hiệu ứng
- **Chữ nhảy số (Damage Text):** Hiện số đỏ khi bạn bị đánh, số trắng/vàng khi bạn chém quái.
- **Thanh cuộn (Scroll View):** Túi đồ không giới hạn, có thể kéo lên xuống.
- **Nút tắt [X]:** Giúp đóng UI bằng chuột cực nhanh.
- **Cải thiện độ rõ nét:** Chữ VÀNG và EXP đã được thêm bóng đổ (Shadow) để nhìn rõ trên mọi địa hình.
- **Camera Follow:** Script camera bám đuổi nhân vật mượt mà.

### ⚔️ Giai đoạn Khởi đầu: Gameplay Cơ bản
- **Di chuyển & Chiến đấu:** Chạy 8 hướng (Shift để tăng tốc), Chém thường (Space).
- **Lò đẻ quái vô tận (Spawner):** Hệ thống Wave (10 quái -> 1 Boss). Tự động tăng độ khó theo thời gian.
- **Rơi đồ & Vàng:** Quái chết rớt vàng (30%) hoặc đồ (10%). Tự động hút đồ khi đứng gần.

---

## 🛠️ 2. NHỮNG GÌ BẠN ĐANG LÀM & CẦN "KÉO THẢ" TRONG UNITY

Phần này là nhiệm vụ của bạn trong phần mềm Unity để "kích hoạt" những gì tôi đã viết:

### 🔹 NHỮNG VIỆC CẦN LÀM NGAY (QUAN TRỌNG):
- [ ] **Sửa lỗi Animator:** Mở bảng Animator -> Tab Parameters -> Bấm dấu [+] tạo 1 biến **Float** tên là **`Speed`** (viết hoa chữ S). Nếu không làm bước này, nhân vật sẽ đứng im hoặc báo lỗi vàng.
- [ ] **Gắn Camera:** Chọn **Main Camera** trong Unity -> Kéo script **`CameraFollow`** thả vào đó.
- [ ] **Gắn Lò đẻ quái:** Bạn phải kéo 1 con Quái (Prefab) vào ô **Monster Prefab** trong script `MonsterSpawner` thì quái mới hiện ra.
- [ ] **Cắt ảnh (Slice):** Sử dụng Sprite Editor để cắt bộ ảnh Combo (Chạy & Chém).

### 🔹 NHỮNG VIỆC ĐÃ LÀM XONG TRONG UNITY (DO BẠN THỰC HIỆN):
1. Đã đưa Map vào game (Tilemap).
2. Đã đưa mẫu Hero Knight vào chạy thử.
3. Đã tạo ra các cục Prefab cho quái (Slime).

---

## 🚩 3. CÁC VẤN ĐỀ ĐÃ GIẢI QUYẾT (HISTORY FIXES)
- **Fix lỗi Input System:** Chuyển từ hệ cũ sang Unity 6 Input System.
- **Fix lỗi Spawner:** Sửa lỗi quái ngừng đẻ sau khi Boss chết.
- **Fix lỗi Transparency:** Hướng dẫn bật Alpha Is Transparency để mất nền trắng của nhân vật.
- **Fix lỗi currentHealth:** Chuyển sang Public để Script AI bên ngoài có thể đọc được máu quái.

---
> [!IMPORTANT]
> **Hướng dẫn sử dụng Kỹ năng:** Cày lên **Cấp 3**, mở bảng **Kỹ năng**, bấm nút **Học**, sau đó nhấn phím **số 1** trên bàn phím để chém sấm sét!