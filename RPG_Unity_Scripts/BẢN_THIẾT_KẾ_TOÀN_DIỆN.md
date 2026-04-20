# 📜 BẢN THIẾT KẾ TOÀN DIỆN: RPG COMPANION SYSTEM (TRASHY STYLE)

Tài liệu này dùng để theo dõi tiến độ và quy chuẩn kỹ thuật cho dự án RPG phong cách "Nghèo khổ".

## 🛠️ Trạng thái Tính năng

| ID | Tính năng | Trạng thái | Ghi chú |
|:---|:---|:---|:---|
| 01 | Di chuyển & Tấn công cơ bản | ✅ Hoàn thành | Player & Companion |
| 02 | Hệ thống Companion (Đệ tử) | ✅ Hoàn thành | Đi theo, nhặt đồ, chiến đấu |
| 03 | Cân bằng rơi đồ (Loot Table) | ✅ Hoàn thành | Tỷ lệ rơi rác cao, đồ xịn cực thấp |
| 04 | Tooltip thông minh | ✅ Hoàn thành | Hiện chỉ số, yêu cầu cấp độ |
| 05 | Hệ thống Kinh nghiệm (EXP) | ✅ Hoàn thành | Chia sẻ EXP cho đệ tử |
| 06 | Lưu/Tải game (Save/Load) | ✅ Hoàn thành | Lưu vị trí, túi đồ, stats |
| 07 | Cơ chế Vũ khí 2 tay | ✅ Hoàn thành | Không cho phép dùng khi đã có khiên |
| 08 | Damage Text (Hiển thị sát thương) | ✅ Hoàn thành | Màu sắc phân biệt Công/Thủ |
| 09 | AI Đối thủ (Monster AI) | ✅ Hoàn thành | Đuổi theo và tấn công |
| 10 | Hệ thống Portal (Dịch chuyển) | ✅ Hoàn thành | Chuyển map Wilderness |
| 11 | Hệ thống Túi đồ (Shared Inventory) | ✅ Hoàn thành | Dùng chung cho cả đội |
| 12 | Hệ thống Trang bị (Equip System) | ✅ Hoàn thành | 9 slot chi tiết |
| 13 | Monster Spawner (Lỗ quái vật) | ✅ Hoàn thành | Sinh quái liên tục |
| 14 | Monster Spawner giới hạn số quái | ✅ Hoàn thành | Đã có MonsterPit |
| 15 | Quái vật tấn công cả đệ tử | ✅ Hoàn thành | AI đã hỗ trợ |
| 16 | Tất cả item trong hòm đồ phải hiện chỉ số | ✅ Hoàn thành | Đã có Tooltip hiển thị chỉ số |
| 17 | Công cụ mặc đồ cho đệ tử từ túi mình | ✅ Hoàn thành | Đã có nút Mặc cho đệ tử |
| 18 | HUD: Skill Countdown & Buff Icons | ✅ Hoàn thành | Đã có cooldown và icon buff |
| 19 | Hệ thống Ngọc (Gems): 3 dòng Công, Thủ, HP bậc I-V | ✅ Hoàn thành | Đã có Lò đúc và Icon |
| 20 | Đột phá Rank Yêu cầu Linh hồn (Souls) | ✅ Hoàn thành | Đã có vật phẩm Soul và logic tiêu thụ |
| 21 | Cường hóa trang bị (+) tốn Vàng | ✅ Hoàn thành | Đã có logic thợ rèn |
| 22 | Khảm ngọc vào trang bị (Socketing) | ✅ Hoàn thành | Đã có tỷ lệ thành công/thất bại |
| 23 | Điểm tiềm năng (STR, VIT, AGI) | ✅ Hoàn thành | Bảng cộng điểm khi lên level |
| 24 | Archer bắn cung, Slime phun độc | ✅ Hoàn thành | Cơ chế chiến đấu riêng biệt |
| 25 | Mỹ thuật "Rẻ rách" (Cái Bang Style) | [/] Đang làm | Cấp cao chỉ thêm chi tiết nhỏ (vá, đinh) |
| 26 | Đầy đủ 9 slot trang bị | ✅ Hoàn thành | Đã nạp vào Lò đúc |
| 27 | Hệ Thống Thợ Rèn Slot UI | ✅ Hoàn thành | Giao diện 3 ô (Chế, Cường hóa, Khảm) |
| 28 | Map Ngôi làng ổ chuột (Đông đúc) | [/] Đang làm | Bố cục mê cung, đầy rác và rỉ sét |

---

## 📜 Phụ lục Chi tiết Hệ thống:

### 1. Chiến lược Mỹ thuật Vật phẩm
- **Mục tiêu**: Tuyệt đối không được làm trang bị trông "giàu sang" ở cấp độ cao.
- **Giải pháp**: Thêm vết vá bao tải, dây kẽm quấn quanh, hoặc đinh gỉ gắn thêm vào mũ để nâng cấp "độ nát" tinh tế.

### 2. Hệ thống Thợ Rèn (Slot-based)
- **Cơ chế**: Kéo thả hoặc click vật phẩm để đưa vào các ô Slot.
- **Tính năng**: Ưu tiên Chế tạo nếu khớp công thức, ngược lại thực hiện Cường hóa hoặc Khảm ngọc.

### 3. Công cụ Xây Làng (VillageBuilder)
- Tự động sắp xếp nhà cửa và vật cản theo mật độ định sẵn để tạo cảm giác đông đúc, ngột ngạt.

### 4. Danh sách Quái vật "Nghèo Khổ" (Dự kiến)
- **Mud Slime (Slime Bùn Lầy)**: Phun bùn gây gỉ sét (giảm thủ).
- **Plague Rat (Chuột Cống Đột Biến)**: Quấn dây kẽm gỉ, cắn gây độc.
- **Trash Scarecrow (Bù Nhìn Rác)**: Ném gạch vỡ từ xa.
- **Beggar Zombie (Xác Sống Ăn Mày)**: Tụ tập thành nhóm đông với gậy tre gãy.

---
*Bản thiết kế này là kim chỉ nam cho mọi hoạt động phát triển của dự án MyRPG.*
