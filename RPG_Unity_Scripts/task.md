# TIẾN ĐỘ PHÁT TRIỂN RPG - DỰ ÁN VỀ LÀNG

## 📅 Cập nhật ngày: 18/04/2026

### ✅ CÁC CÔNG VIỆC ĐÃ HOÀN THÀNH (Đã CODE):
1. **Hệ thống Chuyển Cảnh (Portal)**:
   - [x] Portal Hybrid (Va chạm + Khoảng cách).
   - [x] Tự động Lưu Game trước khi nhảy Scene.
   - [x] Chống Spam chuyển cảnh.

2. **Hệ thống Camera & Lỗi (CameraFollow)**:
   - [x] Tự động gắn AudioListener (Xóa 999+ lỗi Console).
   - [x] Tự động khóa mục tiêu Player sau khi chuyển cảnh/sinh map.

3. **Giao diện & Chỉ số (GameUI & PlayerStats)**:
   - [x] **Multi-Companion HUD**: Hiện tối đa 4 thanh máu đệ tử (Đã sửa lỗi 0/175 HP).
   - [x] **Floating HP**: Thanh máu đổi màu (Xanh/Vàng/Đỏ) trên đầu hiệp sĩ và đệ tử.
   - [x] **Lưu/Tải Game**: Tự động lưu mọi chỉ số, đồ đạc, vị trí khi chuyển map.
   - [x] **Nút SỬ DỤNG**: Bấm vào bình máu có nút 'SỬ DỤNG' để uống máu
   - [x] **Phím tắt [H]**: Thêm phím tắt [H] để hồi máu nhanh ngoài map
   - [x] **Bộ đếm thuốc**: Hiển thị số lượng bình máu hiện có ngay trên HUD chính.

### 🧠 CẬP NHẬT AI ĐỆ TỬ (CompanionAI.cs):
- **Logic "Dây xích" (Leash Logic)**: Nếu đệ tử cách xa chủ nhân quá 8m, họ sẽ bỏ qua quái vật và chạy ngay về phía chủ.
- **Vùng bảo vệ**: Đệ tử chỉ đánh những con quái ở gần chủ (trong bán kính 10m quanh chủ).
- **Đồng bộ Animator**: Đảm bảo sử dụng đúng các tham số `IsWalking` và `Attack` như trong bảng Animator bạn đã gửi.

### 🛠️ NHỮNG VIỆC BẠN CẦN LÀM (Kéo thả trong Unity):
- [x] Đã kéo thả `HiepSi` vào Prefab.
- [x] Đã kéo thả `Main Camera` vào Prefab.
- [x] Đã kéo thả các loại đệ tử (Warrior, Archer, Slime) vào Prefab.
- [ ] **Kiểm tra**: Nếu thuê đệ tử mới, hãy đảm bảo chọn đúng Prefab trong Object `TrainerNPC`.
- [ ] **Thanh máu đầu**: Thanh máu đầu của quái vật đã có sẵn trong script `Monster`, chỉ cần đảm bảo quái có script này.

### ⏳ CÔNG VIỆC ĐANG LÀM / CHƯA XONG:
- [/] Kiểm tra tính ổn định khi chuyển cảnh liên tục nhiều lần.
- [ ] Thêm hiệu ứng âm thanh uống thuốc (Nếu bạn có file sound).
- [ ] Tối ưu hóa hiệu suất khi có quá nhiều đệ tử (trên 10 người).

---
> [!NOTE]
> Tất cả các file Save đều được lưu vào `PlayerPrefs`. Nếu bạn muốn chơi lại từ đầu, hãy nhấn nút **[NEW GAME]** trong túi đồ (Phím B).
