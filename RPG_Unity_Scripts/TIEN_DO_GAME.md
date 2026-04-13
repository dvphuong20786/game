# NHẬT KÝ LÀM GAME RPG 2D - BÁO CÁO TIẾN ĐỘ 
*(Dùng để lưu trữ và đọc lại khi về nhà)*

## 1. CÁC TÍNH NĂNG "KHỦNG" ĐÃ LÀM XONG TRONG HÔM NAY:
- **Di Chuyển & Chiến Đấu Cốt Lõi:** Đã làm xong chạy WASD chuẩn Unity 6. Đánh quái bằng phím Space. 
- **Trí Tuệ Nhân Tạo (AI Quái Vật):** Quái vật hiện tại cực khôn, biết đánh hơi người chơi ở xa và lướt trên đất bám theo rượt đuổi đến cùng.
- **Tính Năng Lò Đẻ (Monster Spawner):** Đã lắp cỗ máy tự động sao chép quái vật, tạo thành vòng lặp vô tận. Cứ 4 giây quái sẽ "bò" từ ngoài màn hình vào.
- **Hệ Thống Loot Đồ Rớt (Gacha Cổ Điển):** Quái chết rớt đồ xác suất - 70% rơi Kẹo (Vàng) tăng EXP, 20% rơi Kiếm Gỗ Cùn (Nâu), 10% rơi Huyết Kiếm Huyền Thoại (Cam).
- **Hệ Thống Kho Đồ - Trang Bị (Giao Diện Bằng Code 100%):** 
  + Thêm công tắc phím **B** để bật/tắt cái Ba Lô chứa danh sách vũ khí lụm được trên tay. 
  + Chọn Mặc Đồ trong Ba lô sẽ trực tiếp đẩy Lực Chiến (Sát thương chém) tăng vọt.
- **Đồ Họa Nhân Vật (Nâng Cấp Cuối Cùng):** Đã lột xác từ Hình Vuông trắng trọc lóc sang hình Hiệp Sĩ vung kiếm và Quái Vật Slime răng nhọn màu sắc sặc sỡ sắc nét HD.

## 2. KẾ HOẠCH BƯỚC TIẾP THEO (DÀNH CHO NGÀY MAI)
Với một cái nền móng siêu vững chãi bằng code nhúng như bên trên, công việc ngày mai ở nhà sẽ thiên về Đồ họa và Cân Bằng:
1. **Âm Thanh (Audio):** Tìm 3 file file nhạc MP3 (Tiếng chém gươm "Phập", Tiếng cắn "Ục", Nhạc nền dồn dập). Kéo vào và gọi lệnh `AudioSource.PlayClipAtPoint()`.
2. **Ảnh Động (Animation):** Cho người gỗ cử động tay chân khi đi lùi hoặc vung bảo kiếm thay vì trôi tuột như khúc gỗ.
3. **Cơ Chế Bơm Máu / Level Mở Khóa Đồ:** Có EXP rồi thì cho phép đánh Quái rơi ra Bình Máu để ăn hồi HP. Hoặc thêm nhiều loại quái vật Khổng Lồ đẻ ra sau phút thứ 5.

---
**LƯU Ý:**
- Mọi code đều để nằm ở ổ tĩnh `D:\work\1\RPG_Unity_Scripts\` công ty.
- Mọi code đều để nằm ở ổ tĩnh `F:\WORK\GAME\RPG_Unity_Scripts\` ở nhà.
- Đừng quên gõ lệnh Push lên Git để lấy Source Code về nhà. Gặp lỗi hỏng hóc Lò Đẻ ở đâu, cứ quăng ảnh cho tớ bắt bệnh tiếp! Chúc ngài Giám đốc tựa Game đi làm về nhà an toàn!
