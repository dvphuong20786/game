# Kế hoạch triển khai: Đại tu hệ thống Đệ tử Hỗ trợ & UI Nâng cao

Kế hoạch này sẽ thực hiện các nâng cấp lớn về hệ thống đệ tử, giao diện và camera dựa trên phản hồi mới nhất của bạn.

## User Review Required

> [!IMPORTANT]
> - **Kinh nghiệm (XP):** Chia đều 50/50 cho cả Người chơi và Đệ tử.
> - **Kỹ năng Đệ tử:** Thiết kế 3-4 kỹ năng hỗ trợ (Hồi máu, Tăng thủ, Phản sát thương).
> - **Camera Focus:** Camera sẽ tự động chuyển mục tiêu theo dõi sang nhân vật bạn đang xem trong bảng Bag.

## Proposed Changes

### 1. Camera & Hệ thống Theo dõi

#### [MODIFY] [CameraFollow.cs](file:///d:/work/1/RPG_Unity_Scripts/CameraFollow.cs)
- Tăng `smoothSpeed` lên 12 (nhanh hơn đáng kể).
- Thêm hàm `SetTarget(Transform newTarget)` để chuyển đổi tiêu điểm giữa Người chơi và Đệ tử.

---

### 2. Hệ thống Chỉ số & Kỹ năng Đệ tử

#### [MODIFY] [PlayerStats.cs](file:///d:/work/1/RPG_Unity_Scripts/PlayerStats.cs)
- Triển khai logic cộng điểm tiềm năng:
    - **STR:** Mỗi điện cộng thêm 2 Dame.
    - **VIT:** Mỗi điểm cộng thêm 15 HP và 1 Def.
    - **AGI:** Mỗi điểm cộng thêm 0.2 tốc độ đánh (trong PlayerCombat).
- Thêm các kỹ năng hỗ trợ riêng cho đệ tử:
    - *🛡 Hộ Vệ:* Tăng phòng thủ cho chủ nhân.
    - *❤ Trị Thương:* Hồi máu cho cả đội.
    - *💠 Phản Nguyên:* Phản lại 20% sát thương nhận vào.

#### [MODIFY] [CompanionAI.cs](file:///d:/work/1/RPG_Unity_Scripts/CompanionAI.cs)
- Cập nhật logic đánh quái để ưu tiên dùng kỹ năng hỗ trợ cho người chơi khi đứng gần.

---

### 3. Giao diện (UI) & Trải nghiệm người dùng

#### [MODIFY] [GameUI.cs](file:///d:/work/1/RPG_Unity_Scripts/GameUI.cs)
- **HUD Màn hình chính:** Thêm thanh HP/EXP đệ tử nằm ngay dưới thanh người chơi ở góc trên bên trái.
- **Bảng Bag (Nhân vật):**
    - Thêm nút [+] bên cạnh STR, VIT, AGI để cộng điểm tiềm năng.
    - Hiển thị bảng kỹ năng hỗ trợ nếu đang xem đệ tử.
    - Khi bấm nút "XEM ĐỆ TỬ" -> Tự động gọi Camera focus vào đệ tử.
- **Tooltip:** Hiển thị chi tiết (vd: "Áo Giáp: +18 Phòng thủ") thay vì chỉ hiện mô tả chung.

---

### 4. Logic Quái vật

#### [MODIFY] [Monster.cs](file:///d:/work/1/RPG_Unity_Scripts/Monster.cs)
- Chia đều 50% XP cho Người chơi và 50% cho Đệ tử khi quái bị tiêu diệt.

---

### 5. Tài liệu (Note chi tiết cho bạn)

#### [MODIFY] [TIEN_DO_GAME.md](file:///d:/work/1/RPG_Unity_Scripts/TIEN_DO_GAME.md)
#### [MODIFY] [HUONG_DAN_CHI_TIET_GAME.md](file:///d:/work/1/RPG_Unity_Scripts/HUONG_DAN_CHI_TIET_GAME.md)
- **Hướng dẫn Trainer:** Note cực kỳ chi tiết các bước kéo thả Prefab, gán Sound, chỉnh giá và cách "kích hoạt" đệ tử đi lại.
- Thêm phần hướng dẫn mặc đồ cho đệ tử trong Unity.

## Verification Plan

### Automated Tests
- Kiểm tra Camera chuyển mục tiêu trơn tru khi switch Tab.
- Kiểm tra nút [+] chỉ hiện khi có điểm tiềm năng (`statPoints > 0`).

### Manual Verification
- Đánh quái và quan sát cả 2 thanh EXP đều tăng.
- Chọn đệ tử trong bảng Bag và xem Camera có di chuyển tới đệ tử không.
- Kiểm tra các kỹ năng hỗ trợ của đệ tử có tác dụng lên người chơi không.
