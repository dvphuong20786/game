# BẢN THIẾT KẾ TOÀN DIỆN (Master Requirement List)

File này lưu trữ toàn bộ các yêu cầu của USER để Antigravity không bao giờ quên.

| STT | Yêu cầu | Trạng thái | Ghi chú |
|:---:|:---|:---:|:---|
| 1 | Hoạt ảnh Đứng yên (Idle) không được chạy animation di chuyển | ✅ Hoàn thành | Đã fix logic trôi animation |
| 2 | Camera/Map di chuyển theo nhân vật | ✅ Hoàn thành | CameraFollow đã hoạt động |
| 3 | Tỉ lệ rơi đồ (10%), rơi Vàng (30%) | [/] Đang làm | Cần chỉnh trong Monster.cs (Mục 3) |
| 4 | Hiệu ứng sáng cho Item rớt dưới đất | ⏳ Đang nợ | Cần thêm Trail/Particle |
| 5 | Nút SỬ DỤNG/GỠ BỎ trong hòm đồ | ✅ Hoàn thành | Đã thêm Tooltip và nút hành động |
| 6 | Chữ vàng khó nhìn | ✅ Hoàn thành | Đã đổi màu sang Vàng Cam (Amber) |
| 7 | Vũ khí 2 tay, Khiên, 2 Nhẫn, Vàng cổ, Khảm ngọc | [/] Đang làm | Đang code logic 2 tay (Mục 7) |
| 8 | Bảng kỹ năng (3 level học 1 chiêu) | ✅ Hoàn thành | Đã triển khai bảng kỹ năng |
| 9 | Đánh thường đơn mục tiêu, Kỹ năng mới AOE | ⏳ Đang nợ | Cần chỉnh logic PlayerCombat |
| 10 | Balance: Giảm dame nhân vật/đồ đạc | ⏳ Đang nợ | Cần hạ chỉ số base |
| 11 | NPC bán đồ, mua lại đồ người chơi | ✅ Hoàn thành | Shop đã nạp đầy đủ đồ test |
| 12 | HUD Đệ tử có XP/HP, Bảng kỹ năng riêng | ✅ Hoàn thành | HUD đã xong, Skill tab đã nạp |
| 13 | Cộng điểm khi lên LV, Chia sẻ EXP | ✅ Hoàn thành | Đạt mục tiêu |
| 14 | Monster Spawner giới hạn số quái | ✅ Hoàn thành | Đã có MonsterPit |
| 15 | Quái vật tấn công cả đệ tử | ✅ Hoàn thành | AI đã hỗ trợ |
| 16 | Tất cả item trong hòm đồ phải hiện chỉ số | ✅ Hoàn thành | Đã có Tooltip hiển thị chỉ số |
| 17 | Công cụ mặc đồ cho đệ tử từ túi mình | ✅ Hoàn thành | Đã có nút Mặc cho đệ tử |
| 18 | HUD: Skill Countdown & Buff Icons | ✅ Hoàn thành | Đã có cooldown và icon buff |
| 19 | Hệ thống Ngọc (Gems): 3 dòng Công, Thủ, HP bậc I-V | ✅ Hoàn thành | Đã có Lò đúc và Icon |
| 20 | Đột phá Rank Yêu cầu Linh hồn (Souls) | ✅ Hoàn thành | Đã có vật phẩm Soul và logic tiêu thụ |
| 21 | Cường hóa trang bị (+) tốn Vàng | ✅ Hoàn thành | Đã có logic thợ rèn |
| 22 | Khảm ngọc vào trang bị (Socketing) | ✅ Hoàn thành | Đã có tỷ lệ thành công/thất bại |
| 23 | Điểm tiềm năng (STR, VIT, AGI) và Điểm kỹ năng | ✅ Hoàn thành | Đã có bảng cộng điểm khi lên level |
| 24 | Archer bắn cung (Projectile), Slime phun độc (DOT) | ✅ Hoàn thành | Cơ chế chiến đấu riêng biệt |
| 25 | Mỹ thuật "Rẻ rách" (Cái Bang Style) | [/] Đang làm | Chờ Quota vẽ: Ngọc bẩn, Bao tải, Dao gỉ |
| 26 | Đầy đủ 9 slot trang bị (Mũ, Dây, Vàng, Nhẫn...) | ✅ Hoàn thành | Đã nạp vào Lò đúc |

---
## 🚧 Công việc đang nợ (Pending Tools/Art):
- **Phụ lục Art**: Cần vẽ Mảnh kính (Ngọc), Mẫu bánh mốc, Nồi thủng (Mũ), Dép rơm (Giày), Dây thừng (Dây chuyền), Nắp thùng rác (Khiên).
- **Phụ lục Logic**: Cân bằng Loot Table (Monster.cs), Chặn mặc Khiên khi cầm Cung (PlayerStats.cs).

---
## 📂 Kế hoạch trước mắt (Sprints):
1. **Hoàn thiện Logic "Cái Bang"**: Vũ khí 2 tay và Rơi đồ ngẫu nhiên.
2. **Nạp Art "Nghèo"**: Thay thế toàn bộ icon hoành tráng bằng icon rẻ rách khi hồi Quota.
3. **Hiệu ứng rớt đồ**: Thêm tia sáng/hào quang cho vật phẩm rơi trên đất.
