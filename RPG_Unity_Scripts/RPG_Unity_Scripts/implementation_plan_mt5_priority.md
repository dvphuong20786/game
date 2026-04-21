# Kế hoạch Xử lý Ưu tiên Tín hiệu Phá Trendline Tăng (Giá Giảm)

Khi giá vàng biến động cực mạnh (Flash Crash), giá có thể xuyên qua cả đường hỗ trợ (Trendline TĂNG) và đường kháng cự (Trendline GIẢM) đang hội tụ, dẫn đến việc Dashboard hiện sai hoặc Bot xử lý không chuẩn. Kế hoạch này sẽ điều chỉnh logic để ưu tiên tuyệt đối cho tín hiệu **Giá Giảm** khi có xung đột.

## Thành phần thay đổi

### 1. Dashboard & Logic Trendline (MT5)

#### [MODIFY] [EMA.mqh](file:///c:/Users/dvphu/AppData/Roaming/MetaQuotes/Terminal/D0E8209F77C8CF37AD8BF550E51FF075/MQL5/Experts/BerserkTwiceSurvival/xSilver%20-%20Smart%202.0/dashboard/EMA.mqh)

- **Ưu tiên biến trạng thái**: Trong hàm `HandleTrendByPeriod` (khoảng dòng 981), bổ sung logic loại trừ: Nếu `currentBreakUp` (Phá trend tăng - Giá giảm) là `true`, thì ép `currentBreakDown = false`. Điều này đảm bảo khi sập giá, Bot không bị nhầm lẫn bởi tín hiệu phá trend giảm ảo.
- **Đảo thứ tự hiển thị Dashboard**: Đưa điều kiện kiểm tra `currentBreakUp` lên trước trong khối `if-else` (khoảng dòng 991) để bản tin Dashboard hiển thị đúng thực tế thị trường đang rơi.

## Chi tiết kỹ thuật

```mql5
// EMA.mqh:981
bool currentBreakUp   = (countUp == 2 && close_0 < valUp && valUp > 0);

// BỔ SUNG LOGIC ƯU TIÊN
if(currentBreakUp) currentBreakDown = false; 

// Sau đó mới gán vào các biến Global cho Bot dùng
if(tf == PERIOD_M15) { isBreakDown_M15 = currentBreakDown; isBreakUp_M15 = currentBreakUp; }
...
```

## Xác minh
1. **Kiểm tra hiển thị**: Khi giá sập, Dashboard phải hiện chữ "PHÁ TREND TĂNG ▼ (GIÁ ↓ GIẢM)" màu đỏ, không được hiện màu xanh dù có phá đường cản trên.
2. **Kiểm tra vận hành**: Bot phải kích hoạt đúng các hàm `thongbaobotBUY("B-22:...")` khi có sự kết hợp của `isBreakUp_H4`.

## Câu hỏi cho người dùng
> [!IMPORTANT]
> Tôi đã tìm ra cách xử lý ưu tiên này. Vì Bot của bạn đang chạy live, việc sửa code này sẽ yêu cầu Re-compile. Bạn có đồng ý để tôi thực hiện thay đổi này ngay bây giờ không?
