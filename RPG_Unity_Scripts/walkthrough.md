# 🐾 WALKTHROUGH: HỆ THỐNG ĐỆ TỬ CAO CẤP

Tôi đã nâng cấp Đệ tử từ một AI di chuyển đơn giản thành một hệ thống RPG hoàn chỉnh. Giờ đây, Đệ tử của bạn thực sự là một "Người chơi thứ hai" với đầy đủ sức mạnh!

## 🌟 Các tính năng mới

### 1. Chỉ số và Trang bị (RPG Stats)
- Đệ tử giờ dùng chung script `PlayerStats` với Người chơi. 
- Nó có **9 ô trang bị riêng**: Đầu, Áo, Giày, Nhẫn, Vũ khí... 
- Khi bạn cho Đệ tử mặc đồ, máu và dame của nó sẽ tăng lên thực tế.

### 2. Quản lý Túi đồ (Inventory Management)
- Trong bảng Túi đồ (Phím **B**), nay có nút **[🐕 XEM ĐỆ TỬ]** ở góc trên.
- Bấm nút này để hoán đổi xem Trang bị/Kỹ năng của bạn hoặc của Đệ tử.
- Khi đang xem túi đồ của mình, chọn 1 món đồ sẽ thấy nút **[🐕 GIAO CHO ĐỆ TỬ]** để chuyển đồ sang cho nó.

### 3. Trí tuệ chiến đấu (Smart AI)
- Đệ tử biết tự dùng tuyệt chiêu **Chém Gió (AOE)** khi có nhiều quái bao quanh (nếu bạn đã học kỹ năng đó cho nó).
- Tốc độ đánh và chạy của Đệ tử tỉ lệ thuận với điểm **AGI**.
- Mặc định Đệ tử nhặt vàng sẽ tự động bay vào túi tiền của bạn.

## 🛠️ Cách cài đặt trong Unity (Nếu tạo mới)
1. Gắn `PlayerStats.cs` vào Đệ tử.
2. Trong Inspector của Đệ tử: **Bỏ tích** ô `isPlayer`. (Quan trọng!)
3. Gắn `CompanionAI.cs` vào Đệ tử.
4. Gắn `Animator` và các sprite như người chơi thường.

## ✅ Kết quả kiểm thử
- [x] Đệ tử nhận đồ từ người chơi chính xác.
- [x] Đệ tử tăng dame khi mặc kiếm, tăng thủ khi mặc giáp.
- [x] Nút chuyển đổi UI hoạt động mượt mà.
- [x] Đệ tử tự dùng skill khi đủ điều kiện.

---
> [!TIP]
> Bạn hãy thử cày lên Level 3 cho Đệ tử, sau đó mở bảng kỹ năng của nó và bấm **Học "Chém Gió"**. Bạn sẽ thấy nó bắt đầu chém AOE cực mạnh để bảo vệ bạn!
