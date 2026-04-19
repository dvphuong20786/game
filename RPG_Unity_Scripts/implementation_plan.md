# Kế hoạch: Dọn dẹp mã nguồn & Xử lý lỗi trùng lặp

Hiện tại Unity đang báo lỗi vì có 3 file `PlayerStats.cs` khác nhau trong dự án. Kế hoạch này giúp dọn dẹp rác và đảm bảo Unity chỉ nhận diện 1 file duy nhất.

## Các thay đổi dự kiến

### 1. Dọn dẹp [DELETE]
- Xóa file cũ tại: `F:\WORK\GAME\RPG_Unity_Scripts\MyRPG\Assets\Player\PlayerStats.cs`
- Xóa file trung gian tại: `F:\WORK\GAME\RPG_Unity_Scripts\PlayerStats.cs` (Nếu bạn đồng ý chỉ giữ trong Assets).

### 2. Thống nhất [MODIFY]
- Cập nhật phiên bản mới nhất (có đầy đủ Atk/Def/HP) vào file: `F:\WORK\GAME\RPG_Unity_Scripts\MyRPG\Assets\PlayerStats.cs`.
- Đây sẽ là file duy nhất Unity dùng để chạy game.

## Xác minh
- Sau khi xóa, chỉ còn duy nhất 1 file `PlayerStats.cs` trong dự án.
- Kiểm tra Console của Unity: Toàn bộ lỗi "already defines a member" sẽ biến mất.
