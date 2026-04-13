# NHẬT KÝ KIẾN TRÚC & TIẾN ĐỘ GAME RPG 2D
*(Cập nhật hệ thống Bất tử, Save/Load và Đệ Tử)*

## 1. CÁC TÍNH NĂNG MÃ LỆNH ĐÃ HOÀN TẤT (AI ĐÃ CODE XONG)
- **Hệ thống Lưu/Tải:** File `PlayerStats.cs` và `GameUI.cs` đã full logic ghi/đọc ổ cứng. Hệ thống Tooltip (soi đồ), Cộng Điểm (Tiềm năng) đã chạy tốt trên nền móng lệnh.
- **Thế Giới Bất Tử:** `DontDestroyOnLoad` đã chèn vào PlayerStats và GameUI để qua Map không bị reset.
- **Cổng Chuyển Map:** Code `Portal.cs` tải bản đồ mới.
- **AI Boss Thông Minh:** `SmartBoss.cs` trạng thái Lướt Đỏ và Điên cuồng màu Tím.
- **Đệ Tử Lính Đánh Thuê:** `CompanionAI.cs` đã sinh ra thuật toán đi theo và chém quái bảo vệ chủ.
- **Lò Đẻ Biến Biến:** `MonsterSpawner.cs` rặn 10 quái xuất Boss.

## 2. NHỮNG TÍNH NĂNG CHƯA ĐƯA VÀO UNITY (USER CHƯA GẮN BẰNG TAY)
*Đây là khối lượng đồ họa User cần thực hiện trên Editor để Code thực sự sống dậy:*
- Chưa chia Map (Chưa có File Làng và File Rừng, chưa Add Build Settings).
- Chưa kéo `Portal.cs` vào Cục dịch chuyển.
- Cục Lò Đẻ Quái vẫn chưa gán `bossPrefab` vào ô trống của bảng `MonsterSpawner`.
- Cục Boss bự ngoài cảnh chưa có file `SmartBoss.cs` gắn vào mông.
- Chưa tạo cục Gạch Xanh `CompanionAI` để làm Đệ Tử.
- Hoạt ảnh (Hiệp sĩ) còn dang dở chưa ráp.

## 3. LỊCH TRÌNH THI CÔNG TRONG TƯƠNG LAI
- Áp dụng các tính năng đồ họa trên vào Unity theo từng bước nhỏ đan xen.
- Thêm Thương Nhân NPC.
- Hoàn thiện âm thanh nền và âm thanh chém.
- Thêm ảnh cho từng Item
- hoạt ảnh animation quá nhanh