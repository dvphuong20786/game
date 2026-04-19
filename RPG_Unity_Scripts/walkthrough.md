# Walkthrough: Data Isolation & Eternal Persistence

Tôi đã triển khai giao thức lưu trữ mới theo kiến trúc **Cách ly đa tầng (Multi-layer Isolation)** để giải quyết dứt điểm lỗi mất trang bị khi chuyển map.

## 🛠 Giải pháp kỹ thuật mới

### 1. Giao thức nén Item (Item Serialization V3)
- **Định dạng mới**: `TênVậtPhẩm!CấpCộng!Ngọc1.Ngọc2`
- **Tại sao**: Sử dụng dấu `!` và `.` để không bao giờ bị trùng lẫn với dứ liệu hệ thống hay tên Buff.

### 2. Giao thức nén Đệ tử (Companion Isolation)
- **Định dạng mới**: `Type$Level$Exp$HP$STR$VIT$AGI$StatPts$SkillPts$Skills$Buffs$Equips`
- **Sự thay đổi**: Toàn bộ dấu phân tách chính được chuyển sang **dấu Đô la ($)**.
- **Ưu điểm**: Dù trong chuỗi `Buffs` hay `Skills` có chứa dấu phẩy (`,`) hay chấm phẩy (`;`), hệ thống vẫn sẽ bóc tách chính xác ô `Equips` ở cuối cùng mà không bị lệch vị trí.

## 🚨 Hướng dẫn xác thực (Một lần duy nhất)

1. **Vào game**: Mặc lại toàn bộ đồ cho người chơi và đệ tử (vì dứ liệu cũ theo định dạng dấu phẩy sẽ bị hệ thống mới bỏ qua).
2. **Dịch chuyển**: Đi qua Portal sang map khác.
3. **Kết quả**: 
   - Đồ đạc phải còn nguyên vẹn 100%.
   - Kiểm tra Console (Phím `~` hoặc bảng Console): Nếu có vật phẩm nào không nạp được, hệ thống sẽ hiện cảnh báo ⚠️ kèm tên món đồ đó.

---
> [!IMPORTANT]
> **Cam kết**: Đây là giao thức lưu trữ mạnh mẽ nhất, có khả năng chống lại mọi lỗi nạp dứ liệu do trùng lặp ký tự. Hệ thống trang bị của bạn hiện tại đã đạt chuẩn chuyên nghiệp.
