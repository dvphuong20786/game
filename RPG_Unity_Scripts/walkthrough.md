# Walkthrough: Đệ Tử Nâng Cao & Quái Vật Tuần Tra

Tôi đã hoàn tất việc sửa lỗi và bổ sung các tính năng nâng cao cho đệ tử và quái vật theo yêu cầu của bạn.

## Các thay đổi quan trọng

### 1. Sửa lỗi đệ tử "tàng hình" [NPCTrainer.cs]
- **Vấn đề:** Đệ tử được thuê nhưng không thấy xuất hiện.
- **Giải pháp:** Cố định tọa độ **Z = 0** khi sinh ra và gán **Sorting Order = 5** (hiển thị trên lớp Player/Map).
- **Kiểm tra:** Khi thuê, bạn sẽ thấy thông báo `🛡️ ĐÃ SINH ĐỆ TỬ` kèm tọa độ trong bảng Console.

### 2. Nâng cấp đệ tử: Hào quang & Hồi máu [CompanionAI.cs]
- **Hào quang (Aura):** Đệ tử khi còn sống sẽ có một vòng tròn màu xanh Cyan nhạt dưới chân để bạn dễ phân biệt với NPC khác.
- **Hồi máu (Regen):** Khi không có quái vật, đệ tử tự hồi **2% HP mỗi 3 giây**.
- **AI nhạy bén hơn:** Tầm quét quái vật tăng lên **12m** (thay vì 8m), đệ tử sẽ chủ động tìm quái từ xa hơn.

### 3. Quái vật đi tuần (Patrol) [Monster.cs]
- **Hành động:** Quái vật sinh ra từ Lỗ (Monster Pit) giờ đây sẽ không đứng im một chỗ mà sẽ đi lại chậm rãi xung quanh lỗ.
- **Cơ chế:** Nếu không có mục tiêu để đuổi, chúng sẽ tự tìm một điểm ngẫu nhiên trong bán kính 3m quanh lỗ để đi dạo.

### 4. Điều chỉnh chiến đấu [PlayerCombat.cs]
- **Tầm đánh:** Giảm xuống **1.2f** đúng theo yêu cầu.
- **An toàn:** Đòn đánh của người chơi và đệ tử giờ đây hoàn toàn tách biệt, không gây sát thương lẫn nhau (Friendly Fire).

### 5. Tối ưu hóa & Sửa lỗi linh tinh
- **Monster.cs:** Xóa các dòng tiêu đề trùng lặp, tối ưu hóa việc lật hình ảnh (FlipSprite) để tiết kiệm tài nguyên máy tính.
- **Chia sẻ EXP:** Nếu bạn có nhiều đệ tử, kinh nghiệm (EXP) sẽ được chia đều cho tất cả chúng thay vì chỉ người đầu tiên.
- **Fix Camera:** Sửa lỗi bám đuôi bị "trôi" và tăng tốc độ lên mức 20 cực kỳ mượt mà.
- **Fix Cú pháp:** Sửa lỗi thừa dấu ngoặc nhọn gây lỗi biên dịch trong `CompanionAI.cs`.

## Hướng dẫn cho bạn

- Bạn hãy mở bảng **Console** trong Unity để theo dõi các thông báo sinh đệ tử.
- Nếu thấy quái vật "hiền" hơn hoặc "hung dữ" hơn, bạn có thể chỉnh lại `Attack Range` và `Attack Speed` trực tiếp trong Inspector của Monster Prefab.

---
Mọi chi tiết về lịch sử thay đổi đã được cập nhật tại [TIEN_DO_GAME.md](file:///f:/WORK/GAME/RPG_Unity_Scripts/TIEN_DO_GAME.md).
